using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Hanterar externa referenser och idempotenta importkopplingar för parter.
    /// </summary>
    public sealed class PartyExternalReferenceService : IPartyExternalReferenceService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;

        /// <summary>
        /// Skapar servicen med databas- och valideringsberoenden.
        /// </summary>
        public PartyExternalReferenceService(
            Arb_IntDbContext db,
            IValidationService validationService)
        {
            _db = db;
            _validationService = validationService;
        }

        /// <summary>
        /// Hämtar samtliga externa referenser för en Party.
        /// </summary>
        public async Task<Result<IReadOnlyList<PartyExternalReferenceResponse>>> GetByPartyAsync(
            Guid partyId,
            CancellationToken cancellationToken = default)
        {
            var partyExists = await _db.Set<Party>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == partyId,
                    cancellationToken);

            if (!partyExists)
            {
                return Result.Fail<IReadOnlyList<PartyExternalReferenceResponse>>(
                    new NotFoundError(
                        nameof(Party),
                        partyId));
            }

            var references = await _db.Set<PartyExternalReference>()
                .AsNoTracking()
                .Where(x => x.PartyId == partyId)
                .OrderBy(x => x.SourceSystem)
                .ThenBy(x => x.SourceRecordKey)
                .Select(PartyExternalReferenceProjections.Details)
                .ToListAsync(cancellationToken);

            return Result.Ok<IReadOnlyList<PartyExternalReferenceResponse>>(
                references);
        }

        /// <summary>
        /// Hämtar en extern referens via dess interna identifierare.
        /// </summary>
        public async Task<Result<PartyExternalReferenceResponse>> GetByIdAsync(
            Guid externalReferenceId,
            CancellationToken cancellationToken = default)
        {
            var response = await ProjectAsync(
                externalReferenceId,
                cancellationToken);

            if (response is null)
            {
                return Result.Fail<PartyExternalReferenceResponse>(
                    new NotFoundError(
                        nameof(PartyExternalReference),
                        externalReferenceId));
            }

            return Result.Ok(response);
        }

        /// <summary>
        /// Hämtar en extern referens via källsystem och stabil källnyckel.
        /// </summary>
        public async Task<Result<PartyExternalReferenceResponse>> GetBySourceAsync(
            string sourceSystem,
            string sourceRecordKey,
            CancellationToken cancellationToken = default)
        {
            var normalizedSourceSystem = sourceSystem.Trim();
            var normalizedSourceRecordKey = sourceRecordKey.Trim();

            var response = await _db.Set<PartyExternalReference>()
                .AsNoTracking()
                .Where(x =>
                    x.SourceSystem == normalizedSourceSystem
                    && x.SourceRecordKey == normalizedSourceRecordKey)
                .Select(PartyExternalReferenceProjections.Details)
                .SingleOrDefaultAsync(cancellationToken);

            if (response is null)
            {
                return Result.Fail<PartyExternalReferenceResponse>(
                    new NotFoundError(
                        nameof(PartyExternalReference),
                        $"{normalizedSourceSystem}:{normalizedSourceRecordKey}"));
            }

            return Result.Ok(response);
        }

        /// <summary>
        /// Validerar och skapar en ny extern referens för en Party.
        /// </summary>
        public async Task<Result<PartyExternalReferenceResponse>> CreateAsync(
            Guid partyId,
            CreatePartyExternalReferenceRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<PartyExternalReferenceResponse>(
                    validationResult.Errors);
            }

            var partyResult = await ValidatePartyAsync(
                partyId,
                cancellationToken);

            if (partyResult.IsFailed)
            {
                return Result.Fail<PartyExternalReferenceResponse>(
                    partyResult.Errors);
            }

            var sourceSystem = request.SourceSystem.Trim();
            var sourceRecordKey = request.SourceRecordKey.Trim();

            var existing = await _db.Set<PartyExternalReference>()
                .AsNoTracking()
                .Where(x =>
                    x.SourceSystem == sourceSystem
                    && x.SourceRecordKey == sourceRecordKey)
                .Select(x => new
                {
                    x.Id,
                    x.PartyId
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (existing is not null)
            {
                var message = existing.PartyId == partyId
                    ? "The external reference already exists for this party."
                    : "The external reference is already linked to another party.";

                return Result.Fail<PartyExternalReferenceResponse>(
                    new ConflictError(message));
            }

            var now = DateTimeOffset.UtcNow;
            var firstSeenUtc = request.FirstSeenUtc
                ?? request.LastSeenUtc
                ?? now;

            var lastSeenUtc = request.LastSeenUtc
                ?? firstSeenUtc;

            if (lastSeenUtc < firstSeenUtc)
            {
                return Result.Fail<PartyExternalReferenceResponse>(
                    new ConflictError(
                        "LastSeenUtc cannot be earlier than FirstSeenUtc."));
            }

            var externalReference = new PartyExternalReference
            {
                Id = Guid.CreateVersion7(),
                PartyId = partyId,
                SourceSystem = sourceSystem,
                SourceRecordKey = sourceRecordKey,
                SourceUrl = TrimToNull(request.SourceUrl),
                FirstSeenUtc = firstSeenUtc,
                LastSeenUtc = lastSeenUtc,
                ContentHash = TrimToNull(request.ContentHash)
            };

            _db.Set<PartyExternalReference>()
                .Add(externalReference);

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok(
                await ProjectRequiredAsync(
                    externalReference.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Skapar eller uppdaterar en referens med källsystemets stabila identitet som nyckel.
        /// </summary>
        public async Task<Result<PartyExternalReferenceResponse>> UpsertAsync(
            Guid partyId,
            CreatePartyExternalReferenceRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<PartyExternalReferenceResponse>(
                    validationResult.Errors);
            }

            var partyResult = await ValidatePartyAsync(
                partyId,
                cancellationToken);

            if (partyResult.IsFailed)
            {
                return Result.Fail<PartyExternalReferenceResponse>(
                    partyResult.Errors);
            }

            var sourceSystem = request.SourceSystem.Trim();
            var sourceRecordKey = request.SourceRecordKey.Trim();

            var externalReference = await _db.Set<PartyExternalReference>()
                .SingleOrDefaultAsync(
                    x =>
                        x.SourceSystem == sourceSystem
                        && x.SourceRecordKey == sourceRecordKey,
                    cancellationToken);

            if (externalReference is null)
            {
                return await CreateAsync(
                    partyId,
                    request,
                    cancellationToken);
            }

            if (externalReference.PartyId != partyId)
            {
                return Result.Fail<PartyExternalReferenceResponse>(
                    new ConflictError(
                        "The external reference is already linked to another party."));
            }

            var observedAtUtc = request.LastSeenUtc
                ?? DateTimeOffset.UtcNow;

            var firstSeenUtc = request.FirstSeenUtc
                ?? externalReference.FirstSeenUtc;

            if (observedAtUtc < firstSeenUtc)
            {
                return Result.Fail<PartyExternalReferenceResponse>(
                    new ConflictError(
                        "LastSeenUtc cannot be earlier than FirstSeenUtc."));
            }

            if (firstSeenUtc < externalReference.FirstSeenUtc)
            {
                externalReference.FirstSeenUtc = firstSeenUtc;
            }

            if (observedAtUtc > externalReference.LastSeenUtc)
            {
                externalReference.LastSeenUtc = observedAtUtc;
            }

            externalReference.SourceUrl = TrimToNull(request.SourceUrl);
            externalReference.ContentHash = TrimToNull(request.ContentHash);

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok(
                await ProjectRequiredAsync(
                    externalReference.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Validerar och uppdaterar metadata för en extern referens.
        /// </summary>
        public async Task<Result<PartyExternalReferenceResponse>> UpdateAsync(
            Guid externalReferenceId,
            UpdatePartyExternalReferenceRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<PartyExternalReferenceResponse>(
                    validationResult.Errors);
            }

            var externalReference = await _db.Set<PartyExternalReference>()
                .SingleOrDefaultAsync(
                    x => x.Id == externalReferenceId,
                    cancellationToken);

            if (externalReference is null)
            {
                return Result.Fail<PartyExternalReferenceResponse>(
                    new NotFoundError(
                        nameof(PartyExternalReference),
                        externalReferenceId));
            }

            externalReference.SourceUrl = TrimToNull(request.SourceUrl);
            externalReference.FirstSeenUtc = request.FirstSeenUtc;
            externalReference.LastSeenUtc = request.LastSeenUtc;
            externalReference.ContentHash = TrimToNull(request.ContentHash);

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok(
                await ProjectRequiredAsync(
                    externalReference.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Tar bort en felaktig extern referens.
        /// </summary>
        public async Task<Result> DeleteAsync(
            Guid externalReferenceId,
            CancellationToken cancellationToken = default)
        {
            var externalReference = await _db.Set<PartyExternalReference>()
                .SingleOrDefaultAsync(
                    x => x.Id == externalReferenceId,
                    cancellationToken);

            if (externalReference is null)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(PartyExternalReference),
                        externalReferenceId));
            }

            _db.Set<PartyExternalReference>()
                .Remove(externalReference);

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }

        /// <summary>
        /// Kontrollerar att parten finns.
        /// </summary>
        private async Task<Result> ValidatePartyAsync(
            Guid partyId,
            CancellationToken cancellationToken)
        {
            var exists = await _db.Set<Party>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == partyId,
                    cancellationToken);

            if (!exists)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(Party),
                        partyId));
            }

            return Result.Ok();
        }

        /// <summary>
        /// Projekterar en extern referens direkt från databasen.
        /// </summary>
        private async Task<PartyExternalReferenceResponse?> ProjectAsync(
            Guid externalReferenceId,
            CancellationToken cancellationToken)
        {
            return await _db.Set<PartyExternalReference>()
                .AsNoTracking()
                .Where(x => x.Id == externalReferenceId)
                .Select(PartyExternalReferenceProjections.Details)
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Projekterar en sparad extern referens och kastar om den oväntat saknas.
        /// </summary>
        private async Task<PartyExternalReferenceResponse> ProjectRequiredAsync(
            Guid externalReferenceId,
            CancellationToken cancellationToken)
        {
            return await ProjectAsync(
                    externalReferenceId,
                    cancellationToken)
                ?? throw new InvalidOperationException(
                    $"PartyExternalReference '{externalReferenceId}' was saved but could not be projected.");
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