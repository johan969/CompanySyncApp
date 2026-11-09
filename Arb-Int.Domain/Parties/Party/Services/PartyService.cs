using Arb_Int.Domain.Location;
using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Validation;
using Arb_Int.Domain.User;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Hanterar grundläggande Party-operationer direkt mot <see cref="Arb_IntDbContext"/>.
    /// </summary>
    public sealed class PartyService : IPartyService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;
        private readonly IUserIdentitySynchronizationService _userIdentitySynchronizationService;

        /// <summary>
        /// Skapar Party-servicen med databas- och valideringsberoenden.
        /// </summary>
        public PartyService(
            Arb_IntDbContext db,
            IValidationService validationService,
            IUserIdentitySynchronizationService userIdentitySynchronizationService)
        {
            _db = db;
            _validationService = validationService;
            _userIdentitySynchronizationService = userIdentitySynchronizationService;
        }

        /// <summary>
        /// Hämtar en Party oavsett konkret Party-typ.
        /// </summary>
        public async Task<Result<PartyDetailsResponse>> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            // TODO: Fix authorization so the caller can only read parties in permitted organizations.
            var organization = await ProjectOrganizationAsync(id, cancellationToken);

            if (organization is not null)
            {
                return Result.Ok(organization);
            }

            var person = await ProjectPersonAsync(id, cancellationToken);

            if (person is not null)
            {
                return Result.Ok(person);
            }

            return Result.Fail<PartyDetailsResponse>(
                new NotFoundError(nameof(Party), id));
        }

        /// <summary>
        /// Söker, sorterar och paginerar parter innan de projiceras till Response:er.
        /// </summary>
        public async Task<Result<PagedResult<PartyListResponse>>> QueryAsync(
            PartyQueryRequest request,
            CancellationToken cancellationToken = default)
        {
            // TODO: Fix authorization so party queries are scoped to organizations the caller may access.
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<PagedResult<PartyListResponse>>(
                    validationResult.Errors);
            }

            IQueryable<Party> query = _db.Set<Party>()
                .AsNoTracking();

            if (request.IsActive.HasValue)
            {
                query = query.Where(party =>
                    party.IsActive == request.IsActive.Value);
            }

            if (request.PartyType == PartyType.Organization)
            {
                query = query.OfType<Organization>();
            }
            else if (request.PartyType == PartyType.Person)
            {
                query = query.OfType<Person>();
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var searchPattern = $"%{request.Search.Trim()}%";

                query = query.Where(party =>
                    EF.Functions.ILike(party.Name, searchPattern)
                    || (party is Organization
                        && EF.Functions.ILike(
                            ((Organization)party).LegalName,
                            searchPattern))
                    || (party is Organization
                        && ((Organization)party).TradingName != null
                        && EF.Functions.ILike(
                            ((Organization)party).TradingName!,
                            searchPattern))
                    || (party is Person
                        && EF.Functions.ILike(
                            ((Person)party).GivenName,
                            searchPattern))
                    || (party is Person
                        && EF.Functions.ILike(
                            ((Person)party).FamilyName,
                            searchPattern)));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            query = ApplySorting(
                query,
                request.SortBy,
                request.SortDirection);

            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(PartyProjections.List)
                .ToListAsync(cancellationToken);

            return Result.Ok(new PagedResult<PartyListResponse>
            {
                Items = items,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            });
        }

        /// <summary>
        /// Validerar requesten och skapar en ny organisation.
        /// </summary>
        public async Task<Result<PartyDetailsResponse>> CreateOrganizationAsync(
            CreateOrganizationRequest request,
            CancellationToken cancellationToken = default)
        {
            // TODO: Fix authorization so the caller can only create organizations within permitted administration scope.
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<PartyDetailsResponse>(
                    validationResult.Errors);
            }

            var referenceValidation = await ValidateOrganizationReferencesAsync(
                request.RegistrationCountryId,
                request.ParentOrganizationId,
                currentOrganizationId: null,
                cancellationToken);

            if (referenceValidation.IsFailed)
            {
                return Result.Fail<PartyDetailsResponse>(
                    referenceValidation.Errors);
            }

            var organization = new Organization
            {
                Id = Guid.CreateVersion7(),
                Name = request.Name.Trim(),
                LegalName = request.LegalName.Trim(),
                TradingName = TrimToNull(request.TradingName),
                OrganizationKind = request.OrganizationKind,
                IsInternal = request.IsInternal,
                RegistrationCountryId = request.RegistrationCountryId,
                ParentOrganizationId = request.ParentOrganizationId,
                Description = request.Description.Trim(),
                Notes = TrimToNull(request.Notes),
                IsActive = true
            };

            _db.Set<Organization>().Add(organization);

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok(
                await ProjectOrganizationRequiredAsync(
                    organization.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Validerar requesten och uppdaterar en befintlig organisation.
        /// </summary>
        public async Task<Result<PartyDetailsResponse>> UpdateOrganizationAsync(
            Guid id,
            UpdateOrganizationRequest request,
            CancellationToken cancellationToken = default)
        {
            // TODO: Fix authorization so the caller can only update organizations it may administer.
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<PartyDetailsResponse>(
                    validationResult.Errors);
            }

            var organization = await _db.Set<Organization>()
                .SingleOrDefaultAsync(
                    item => item.Id == id,
                    cancellationToken);

            if (organization is null)
            {
                return Result.Fail<PartyDetailsResponse>(
                    new NotFoundError(nameof(Organization), id));
            }

            var referenceValidation = await ValidateOrganizationReferencesAsync(
                request.RegistrationCountryId,
                request.ParentOrganizationId,
                organization.Id,
                cancellationToken);

            if (referenceValidation.IsFailed)
            {
                return Result.Fail<PartyDetailsResponse>(
                    referenceValidation.Errors);
            }

            organization.Name = request.Name.Trim();
            organization.LegalName = request.LegalName.Trim();
            organization.TradingName = TrimToNull(request.TradingName);
            organization.OrganizationKind = request.OrganizationKind;
            organization.IsInternal = request.IsInternal;
            organization.RegistrationCountryId = request.RegistrationCountryId;
            organization.ParentOrganizationId = request.ParentOrganizationId;
            organization.Description = request.Description.Trim();
            organization.Notes = TrimToNull(request.Notes);
            organization.IsActive = request.IsActive;

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok(
                await ProjectOrganizationRequiredAsync(
                    organization.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Validerar requesten och skapar en ny person.
        /// </summary>
        public async Task<Result<PartyDetailsResponse>> CreatePersonAsync(
            CreatePersonRequest request,
            CancellationToken cancellationToken = default)
        {
            // TODO: Fix authorization so the caller can only create persons within permitted organization scope.
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<PartyDetailsResponse>(
                    validationResult.Errors);
            }

            var person = new Person
            {
                Id = Guid.CreateVersion7(),
                Name = request.Name.Trim(),
                GivenName = request.GivenName.Trim(),
                MiddleName = TrimToNull(request.MiddleName),
                FamilyName = request.FamilyName.Trim(),
                PreferredName = TrimToNull(request.PreferredName),
                Honorific = TrimToNull(request.Honorific),
                PreferredLanguageCode = TrimToNull(request.PreferredLanguageCode),
                Description = request.Description.Trim(),
                Notes = TrimToNull(request.Notes),
                IsActive = true
            };

            _db.Set<Person>().Add(person);

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok(
                await ProjectPersonRequiredAsync(
                    person.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Validerar requesten och uppdaterar en befintlig person.
        /// </summary>
        public async Task<Result<PartyDetailsResponse>> UpdatePersonAsync(
            Guid id,
            UpdatePersonRequest request,
            CancellationToken cancellationToken = default)
        {
            // TODO: Fix authorization so the caller can only update persons connected to organizations it may administer.
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<PartyDetailsResponse>(
                    validationResult.Errors);
            }

            var person = await _db.Set<Person>()
                .SingleOrDefaultAsync(
                    item => item.Id == id,
                    cancellationToken);

            if (person is null)
            {
                return Result.Fail<PartyDetailsResponse>(
                    new NotFoundError(nameof(Person), id));
            }

            person.Name = request.Name.Trim();
            person.GivenName = request.GivenName.Trim();
            person.MiddleName = TrimToNull(request.MiddleName);
            person.FamilyName = request.FamilyName.Trim();
            person.PreferredName = TrimToNull(request.PreferredName);
            person.Honorific = TrimToNull(request.Honorific);
            person.PreferredLanguageCode = TrimToNull(request.PreferredLanguageCode);
            person.Description = request.Description.Trim();
            person.Notes = TrimToNull(request.Notes);
            person.IsActive = request.IsActive;

            await using var transaction = await _db.Database
                .BeginTransactionAsync(cancellationToken);

            await _db.SaveChangesAsync(cancellationToken);

            var synchronizationResult = await _userIdentitySynchronizationService
                .SynchronizeIdentityFromPersonAsync(
                    person.Id,
                    cancellationToken);

            if (synchronizationResult.IsFailed)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result.Fail<PartyDetailsResponse>(
                    synchronizationResult.Errors);
            }

            await transaction.CommitAsync(cancellationToken);

            return Result.Ok(
                await ProjectPersonRequiredAsync(
                    person.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Kontrollerar att organisationsreferenser finns och inte skapar en cykel.
        /// </summary>
        private async Task<Result> ValidateOrganizationReferencesAsync(
            Guid? registrationCountryId,
            Guid? parentOrganizationId,
            Guid? currentOrganizationId,
            CancellationToken cancellationToken)
        {
            if (registrationCountryId.HasValue)
            {
                var countryExists = await _db.Set<Country>()
                    .AsNoTracking()
                    .AnyAsync(
                        country => country.Id == registrationCountryId.Value,
                        cancellationToken);

                if (!countryExists)
                {
                    return Result.Fail(
                        new NotFoundError(
                            nameof(Country),
                            registrationCountryId.Value));
                }
            }

            if (!parentOrganizationId.HasValue)
            {
                return Result.Ok();
            }

            if (currentOrganizationId == parentOrganizationId)
            {
                return Result.Fail(
                    new ConflictError(
                        "An organization cannot be its own parent."));
            }

            var parentExists = await _db.Set<Organization>()
                .AsNoTracking()
                .AnyAsync(
                    organization =>
                        organization.Id == parentOrganizationId.Value,
                    cancellationToken);

            if (!parentExists)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(Organization),
                        parentOrganizationId.Value));
            }

            if (currentOrganizationId.HasValue
                && await WouldCreateOrganizationCycleAsync(
                    currentOrganizationId.Value,
                    parentOrganizationId.Value,
                    cancellationToken))
            {
                return Result.Fail(
                    new ConflictError(
                        "The selected parent organization would create a cyclic organization hierarchy."));
            }

            return Result.Ok();
        }

        /// <summary>
        /// Kontrollerar om en ny parent-relation skulle skapa en cykel i organisationshierarkin.
        /// </summary>
        private async Task<bool> WouldCreateOrganizationCycleAsync(
            Guid organizationId,
            Guid parentOrganizationId,
            CancellationToken cancellationToken)
        {
            Guid? currentId = parentOrganizationId;
            var visited = new HashSet<Guid>();

            while (currentId.HasValue)
            {
                if (currentId.Value == organizationId)
                {
                    return true;
                }

                if (!visited.Add(currentId.Value))
                {
                    return true;
                }

                currentId = await _db.Set<Organization>()
                    .AsNoTracking()
                    .Where(organization =>
                        organization.Id == currentId.Value)
                    .Select(organization =>
                        organization.ParentOrganizationId)
                    .SingleOrDefaultAsync(cancellationToken);
            }

            return false;
        }

        /// <summary>
        /// Hämtar en organisation direkt som projekterad Response.
        /// </summary>
        private async Task<PartyDetailsResponse?> ProjectOrganizationAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return await _db.Set<Organization>()
                .AsNoTracking()
                .Where(organization => organization.Id == id)
                .Select(PartyProjections.OrganizationDetails)
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Hämtar en person direkt som projekterad Response.
        /// </summary>
        private async Task<PartyDetailsResponse?> ProjectPersonAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return await _db.Set<Person>()
                .AsNoTracking()
                .Where(person => person.Id == id)
                .Select(PartyProjections.PersonDetails)
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Hämtar en nyligen sparad organisation och kastar om den oväntat saknas.
        /// </summary>
        private async Task<PartyDetailsResponse> ProjectOrganizationRequiredAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return await ProjectOrganizationAsync(id, cancellationToken)
                ?? throw new InvalidOperationException(
                    $"Organization '{id}' was saved but could not be projected.");
        }

        /// <summary>
        /// Hämtar en nyligen sparad person och kastar om den oväntat saknas.
        /// </summary>
        private async Task<PartyDetailsResponse> ProjectPersonRequiredAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return await ProjectPersonAsync(id, cancellationToken)
                ?? throw new InvalidOperationException(
                    $"Person '{id}' was saved but could not be projected.");
        }

        /// <summary>
        /// Lägger på stabil sortering innan paginering utförs.
        /// </summary>
        private static IQueryable<Party> ApplySorting(
            IQueryable<Party> query,
            PartySortField sortBy,
            SortDirection sortDirection)
        {
            return (sortBy, sortDirection) switch
            {
                (PartySortField.Name, SortDirection.Descending) =>
                    query.OrderByDescending(party => party.Name)
                        .ThenByDescending(party => party.Id),

                (PartySortField.CreateDateUtc, SortDirection.Ascending) =>
                    query.OrderBy(party => party.CreateDateUtc)
                        .ThenBy(party => party.Id),

                (PartySortField.CreateDateUtc, SortDirection.Descending) =>
                    query.OrderByDescending(party => party.CreateDateUtc)
                        .ThenByDescending(party => party.Id),

                _ =>
                    query.OrderBy(party => party.Name)
                        .ThenBy(party => party.Id)
            };
        }

        /// <summary>
        /// Trimmar ett valfritt textvärde och ersätter tom text med null.
        /// </summary>
        private static string? TrimToNull(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return value.Trim();
        }
    }
}