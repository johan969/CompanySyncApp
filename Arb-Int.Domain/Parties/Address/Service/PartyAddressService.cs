using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Location;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Hanterar normaliserade adresser och deras historiserade Party-kopplingar.
    /// </summary>
    public sealed class PartyAddressService : IPartyAddressService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;
        private readonly IAddressKeyService _addressKeyService;

        /// <summary>
        /// Skapar adressservicen med dess databas-, validerings- och normaliseringsberoenden.
        /// </summary>
        public PartyAddressService(
            Arb_IntDbContext db,
            IValidationService validationService,
            IAddressKeyService addressKeyService)
        {
            _db = db;
            _validationService = validationService;
            _addressKeyService = addressKeyService;
        }

        /// <summary>
        /// Hämtar aktiva adresser eller hela adresshistoriken för en Party.
        /// </summary>
        public async Task<Result<IReadOnlyList<PartyAddressResponse>>> GetAsync(
            Guid partyId,
            bool includeHistory = false,
            CancellationToken cancellationToken = default)
        {
            var partyExists = await _db.Set<Party>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == partyId, cancellationToken);

            if (!partyExists)
            {
                return Result.Fail<IReadOnlyList<PartyAddressResponse>>(
                    new NotFoundError(nameof(Party), partyId));
            }

            IQueryable<PartyAddress> query = _db.Set<PartyAddress>()
                .AsNoTracking()
                .Where(x => x.PartyId == partyId);

            if (!includeHistory)
            {
                query = query.Where(x => x.ValidToUtc == null);
            }

            var items = await query
                .OrderByDescending(x => x.IsPrimary)
                .ThenBy(x => x.UsageType)
                .ThenByDescending(x => x.ValidFromUtc)
                .Select(PartyAddressProjections.Details)
                .ToListAsync(cancellationToken);

            return Result.Ok<IReadOnlyList<PartyAddressResponse>>(items);
        }

        /// <summary>
        /// Validerar och skapar en ny aktiv adresskoppling.
        /// </summary>
        public async Task<Result<PartyAddressResponse>> CreateAsync(
            Guid partyId,
            CreatePartyAddressRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<PartyAddressResponse>(
                    validationResult.Errors);
            }

            var referenceResult = await ValidateReferencesAsync(
                partyId,
                request.Address.CountryId,
                cancellationToken);

            if (referenceResult.IsFailed)
            {
                return Result.Fail<PartyAddressResponse>(
                    referenceResult.Errors);
            }

            var validFromUtc = request.ValidFromUtc
                ?? DateTimeOffset.UtcNow;

            await using var transaction = await _db.Database
                .BeginTransactionAsync(cancellationToken);

            var addressId = await GetOrCreateAddressAsync(
                request.Address,
                cancellationToken);

            var duplicateExists = await _db.Set<PartyAddress>()
                .AsNoTracking()
                .AnyAsync(
                    x =>
                        x.PartyId == partyId
                        && x.AddressId == addressId
                        && x.UsageType == request.UsageType
                        && x.ValidToUtc == null,
                    cancellationToken);

            if (duplicateExists)
            {
                return Result.Fail<PartyAddressResponse>(
                    new ConflictError(
                        "The party already has an active link to this address for the selected usage type."));
            }

            if (request.IsPrimary)
            {
                await DemoteCurrentPrimaryAsync(
                    partyId,
                    request.UsageType,
                    excludeId: null,
                    cancellationToken);
            }

            var link = new PartyAddress
            {
                Id = Guid.CreateVersion7(),
                PartyId = partyId,
                AddressId = addressId,
                UsageType = request.UsageType,
                Label = TrimToNull(request.Label),
                Attention = TrimToNull(request.Attention),
                IsPrimary = request.IsPrimary,
                ValidFromUtc = validFromUtc
            };

            _db.Set<PartyAddress>().Add(link);

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Ok(
                await ProjectRequiredAsync(
                    link.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Uppdaterar metadata på en aktiv PartyAddress utan att ändra adressvärdet.
        /// </summary>
        public async Task<Result<PartyAddressResponse>> UpdateAsync(
            Guid partyAddressId,
            UpdatePartyAddressRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<PartyAddressResponse>(
                    validationResult.Errors);
            }

            var link = await _db.Set<PartyAddress>()
                .SingleOrDefaultAsync(
                    x =>
                        x.Id == partyAddressId
                        && x.ValidToUtc == null,
                    cancellationToken);

            if (link is null)
            {
                return Result.Fail<PartyAddressResponse>(
                    new NotFoundError(
                        nameof(PartyAddress),
                        partyAddressId));
            }

            await using var transaction = await _db.Database
                .BeginTransactionAsync(cancellationToken);

            if (request.IsPrimary)
            {
                await DemoteCurrentPrimaryAsync(
                    link.PartyId,
                    link.UsageType,
                    link.Id,
                    cancellationToken);
            }

            link.Label = TrimToNull(request.Label);
            link.Attention = TrimToNull(request.Attention);
            link.IsPrimary = request.IsPrimary;

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Ok(
                await ProjectRequiredAsync(
                    link.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Avslutar den gamla adresskopplingen och skapar en ny historisk version.
        /// </summary>
        public async Task<Result<PartyAddressResponse>> ReplaceAsync(
            Guid partyAddressId,
            ReplacePartyAddressRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<PartyAddressResponse>(
                    validationResult.Errors);
            }

            var currentLink = await _db.Set<PartyAddress>()
                .SingleOrDefaultAsync(
                    x =>
                        x.Id == partyAddressId
                        && x.ValidToUtc == null,
                    cancellationToken);

            if (currentLink is null)
            {
                return Result.Fail<PartyAddressResponse>(
                    new NotFoundError(
                        nameof(PartyAddress),
                        partyAddressId));
            }

            var validFromUtc = request.ValidFromUtc
                ?? DateTimeOffset.UtcNow;

            if (validFromUtc < currentLink.ValidFromUtc)
            {
                return Result.Fail<PartyAddressResponse>(
                    new ConflictError(
                        "The replacement address cannot start before the current address link."));
            }

            var referenceResult = await ValidateReferencesAsync(
                currentLink.PartyId,
                request.Address.CountryId,
                cancellationToken);

            if (referenceResult.IsFailed)
            {
                return Result.Fail<PartyAddressResponse>(
                    referenceResult.Errors);
            }

            await using var transaction = await _db.Database
                .BeginTransactionAsync(cancellationToken);

            currentLink.ValidToUtc = validFromUtc;

            await _db.SaveChangesAsync(cancellationToken);

            var addressId = await GetOrCreateAddressAsync(
                request.Address,
                cancellationToken);

            var duplicateExists = await _db.Set<PartyAddress>()
                .AsNoTracking()
                .AnyAsync(
                    x =>
                        x.Id != currentLink.Id
                        && x.PartyId == currentLink.PartyId
                        && x.AddressId == addressId
                        && x.UsageType == request.UsageType
                        && x.ValidToUtc == null,
                    cancellationToken);

            if (duplicateExists)
            {
                return Result.Fail<PartyAddressResponse>(
                    new ConflictError(
                        "The party already has an active link to this address for the selected usage type."));
            }

            if (request.IsPrimary)
            {
                await DemoteCurrentPrimaryAsync(
                    currentLink.PartyId,
                    request.UsageType,
                    currentLink.Id,
                    cancellationToken);
            }

            var replacement = new PartyAddress
            {
                Id = Guid.CreateVersion7(),
                PartyId = currentLink.PartyId,
                AddressId = addressId,
                UsageType = request.UsageType,
                Label = TrimToNull(request.Label),
                Attention = TrimToNull(request.Attention),
                IsPrimary = request.IsPrimary,
                ValidFromUtc = validFromUtc
            };

            _db.Set<PartyAddress>().Add(replacement);

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Ok(
                await ProjectRequiredAsync(
                    replacement.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Avslutar en aktiv PartyAddress-koppling utan att ta bort historiken.
        /// </summary>
        public async Task<Result> EndAsync(
            Guid partyAddressId,
            EndPartyAddressRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return validationResult;
            }

            var link = await _db.Set<PartyAddress>()
                .SingleOrDefaultAsync(
                    x =>
                        x.Id == partyAddressId
                        && x.ValidToUtc == null,
                    cancellationToken);

            if (link is null)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(PartyAddress),
                        partyAddressId));
            }

            if (request.ValidToUtc < link.ValidFromUtc)
            {
                return Result.Fail(
                    new ConflictError(
                        "ValidToUtc cannot be earlier than ValidFromUtc."));
            }

            link.ValidToUtc = request.ValidToUtc;

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }

        /// <summary>
        /// Kontrollerar att parten och adressens land finns.
        /// </summary>
        private async Task<Result> ValidateReferencesAsync(
            Guid partyId,
            Guid countryId,
            CancellationToken cancellationToken)
        {
            var partyExists = await _db.Set<Party>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == partyId,
                    cancellationToken);

            if (!partyExists)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(Party),
                        partyId));
            }

            var countryExists = await _db.Set<Country>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == countryId,
                    cancellationToken);

            if (!countryExists)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(Country),
                        countryId));
            }

            return Result.Ok();
        }

        /// <summary>
        /// Återanvänder en befintlig normaliserad Address eller skapar en ny.
        /// </summary>
        private async Task<Guid> GetOrCreateAddressAsync(
            AddressRequest request,
            CancellationToken cancellationToken)
        {
            var addressKey = _addressKeyService.CreateKey(
                request.CareOf,
                request.PostOfficeBox,
                request.AddressLine1,
                request.AddressLine2,
                request.AddressLine3,
                request.PostalCode,
                request.Locality,
                request.AdministrativeArea,
                request.CountryId);

            var existingId = await _db.Set<Address>()
                .AsNoTracking()
                .Where(x => x.AddressKey == addressKey)
                .Select(x => (Guid?)x.Id)
                .SingleOrDefaultAsync(cancellationToken);

            if (existingId.HasValue)
            {
                return existingId.Value;
            }

            var address = new Address
            {
                Id = Guid.CreateVersion7(),
                CareOf = TrimToNull(request.CareOf),
                PostOfficeBox = TrimToNull(request.PostOfficeBox),
                AddressLine1 = TrimToNull(request.AddressLine1),
                AddressLine2 = TrimToNull(request.AddressLine2),
                AddressLine3 = TrimToNull(request.AddressLine3),
                PostalCode = TrimToNull(request.PostalCode),
                Locality = TrimToNull(request.Locality),
                AdministrativeArea = TrimToNull(request.AdministrativeArea),
                CountryId = request.CountryId,
                AddressKey = addressKey
            };

            _db.Set<Address>().Add(address);

            return address.Id;
        }

        /// <summary>
        /// Tar bort primärstatus från andra aktiva adresser med samma användningsområde.
        /// </summary>
        private async Task DemoteCurrentPrimaryAsync(
            Guid partyId,
            AddressUsageType usageType,
            Guid? excludeId,
            CancellationToken cancellationToken)
        {
            var query = _db.Set<PartyAddress>()
                .Where(x =>
                    x.PartyId == partyId
                    && x.UsageType == usageType
                    && x.IsPrimary
                    && x.ValidToUtc == null);

            if (excludeId.HasValue)
            {
                query = query.Where(
                    x => x.Id != excludeId.Value);
            }

            await query.ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(x => x.IsPrimary, false),
                cancellationToken);
        }

        /// <summary>
        /// Projekterar en sparad PartyAddress och kastar om den oväntat saknas.
        /// </summary>
        private async Task<PartyAddressResponse> ProjectRequiredAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return await _db.Set<PartyAddress>()
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(PartyAddressProjections.Details)
                .SingleOrDefaultAsync(cancellationToken)
                ?? throw new InvalidOperationException(
                    $"PartyAddress '{id}' was saved but could not be projected.");
        }

        /// <summary>
        /// Trimmar text och ersätter tomma värden med null.
        /// </summary>
        private static string? TrimToNull(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim();
        }
    }
}