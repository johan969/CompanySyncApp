using Arb_Int.Domain.Crm;
using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Hanterar organisationsrelationer mellan personer och organisationer.
    /// </summary>
    public sealed class OrganizationPersonService : IOrganizationPersonService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;

        /// <summary>
        /// Skapar organisationsrelationsservicen med databas- och valideringsberoenden.
        /// </summary>
        public OrganizationPersonService(
            Arb_IntDbContext db,
            IValidationService validationService)
        {
            _db = db;
            _validationService = validationService;
        }

        /// <summary>
        /// Hämtar aktiva eller historiska personrelationer för en organisation.
        /// </summary>
        public async Task<Result<IReadOnlyList<OrganizationPersonResponse>>> GetByOrganizationAsync(
            Guid organizationId,
            bool includeHistory = false,
            CancellationToken cancellationToken = default)
        {
            var organizationExists = await _db.Set<Organization>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == organizationId,
                    cancellationToken);

            if (!organizationExists)
            {
                return Result.Fail<IReadOnlyList<OrganizationPersonResponse>>(
                    new NotFoundError(
                        nameof(Organization),
                        organizationId));
            }

            IQueryable<OrganizationPerson> query = _db.Set<OrganizationPerson>()
                .AsNoTracking()
                .Where(x => x.OrganizationId == organizationId);

            if (!includeHistory)
            {
                query = query.Where(x => x.ValidToUtc == null);
            }

            var relationships = await query
                .OrderByDescending(x => x.IsPrimaryContact)
                .ThenBy(x => x.Person.Name)
                .ThenByDescending(x => x.ValidFromUtc)
                .Select(OrganizationPersonProjections.Details)
                .ToListAsync(cancellationToken);

            return Result.Ok<IReadOnlyList<OrganizationPersonResponse>>(
                relationships);
        }

        /// <summary>
        /// Hämtar aktiva eller historiska organisationsrelationer för en person.
        /// </summary>
        public async Task<Result<IReadOnlyList<OrganizationPersonResponse>>> GetByPersonAsync(
            Guid personId,
            bool includeHistory = false,
            CancellationToken cancellationToken = default)
        {
            var personExists = await _db.Set<Person>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == personId,
                    cancellationToken);

            if (!personExists)
            {
                return Result.Fail<IReadOnlyList<OrganizationPersonResponse>>(
                    new NotFoundError(
                        nameof(Person),
                        personId));
            }

            IQueryable<OrganizationPerson> query = _db.Set<OrganizationPerson>()
                .AsNoTracking()
                .Where(x => x.PersonId == personId);

            if (!includeHistory)
            {
                query = query.Where(x => x.ValidToUtc == null);
            }

            var relationships = await query
                .OrderBy(x => x.Organization.Name)
                .ThenByDescending(x => x.ValidFromUtc)
                .Select(OrganizationPersonProjections.Details)
                .ToListAsync(cancellationToken);

            return Result.Ok<IReadOnlyList<OrganizationPersonResponse>>(
                relationships);
        }

        /// <summary>
        /// Hämtar en enskild organisationsrelation direkt som response.
        /// </summary>
        public async Task<Result<OrganizationPersonResponse>> GetByIdAsync(
            Guid organizationPersonId,
            CancellationToken cancellationToken = default)
        {
            var response = await ProjectAsync(
                organizationPersonId,
                cancellationToken);

            if (response is null)
            {
                return Result.Fail<OrganizationPersonResponse>(
                    new NotFoundError(
                        nameof(OrganizationPerson),
                        organizationPersonId));
            }

            return Result.Ok(response);
        }

        /// <summary>
        /// Hämtar de funktionstyper som kan tilldelas organisationsrelationer.
        /// </summary>
        public async Task<Result<IReadOnlyList<ContactFunctionTypeResponse>>> GetContactFunctionTypesAsync(
            CancellationToken cancellationToken = default)
        {
            var functionTypes = await _db.Set<ContactFunctionType>()
                .AsNoTracking()
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Code)
                .Select(x => new ContactFunctionTypeResponse
                {
                    Id = x.Id,
                    Code = x.Code,
                    SortOrder = x.SortOrder
                })
                .ToListAsync(cancellationToken);

            return Result.Ok<IReadOnlyList<ContactFunctionTypeResponse>>(
                functionTypes);
        }

        /// <summary>
        /// Validerar och skapar en ny aktiv relation mellan organisation och person.
        /// </summary>
        public async Task<Result<OrganizationPersonResponse>> CreateAsync(
            Guid organizationId,
            CreateOrganizationPersonRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<OrganizationPersonResponse>(
                    validationResult.Errors);
            }

            var referenceResult = await ValidatePartyReferencesAsync(
                organizationId,
                request.PersonId,
                cancellationToken);

            if (referenceResult.IsFailed)
            {
                return Result.Fail<OrganizationPersonResponse>(
                    referenceResult.Errors);
            }

            var functionResult = await ValidateFunctionTypesAsync(
                request.ContactFunctionTypeIds,
                cancellationToken);

            if (functionResult.IsFailed)
            {
                return Result.Fail<OrganizationPersonResponse>(
                    functionResult.Errors);
            }

            var activeRelationshipExists = await _db.Set<OrganizationPerson>()
                .AsNoTracking()
                .AnyAsync(
                    x =>
                        x.OrganizationId == organizationId
                        && x.PersonId == request.PersonId
                        && x.ValidToUtc == null,
                    cancellationToken);

            if (activeRelationshipExists)
            {
                return Result.Fail<OrganizationPersonResponse>(
                    new ConflictError(
                        "An active relationship already exists between the organization and person."));
            }

            await using var transaction = await _db.Database
                .BeginTransactionAsync(cancellationToken);

            if (request.IsPrimaryContact)
            {
                await DemoteCurrentPrimaryContactAsync(
                    organizationId,
                    excludeOrganizationPersonId: null,
                    cancellationToken);
            }

            var relationship = new OrganizationPerson
            {
                Id = Guid.CreateVersion7(),
                OrganizationId = organizationId,
                PersonId = request.PersonId,
                RelationshipType = request.RelationshipType,
                JobTitle = TrimToNull(request.JobTitle),
                Department = TrimToNull(request.Department),
                IsPrimaryContact = request.IsPrimaryContact,
                ValidFromUtc = request.ValidFromUtc ?? DateTimeOffset.UtcNow,
                Notes = TrimToNull(request.Notes)
            };

            foreach (var functionTypeId in request.ContactFunctionTypeIds)
            {
                relationship.FunctionLinks.Add(
                    new OrganizationPersonFunction
                    {
                        Id = Guid.CreateVersion7(),
                        ContactFunctionTypeId = functionTypeId
                    });
            }

            _db.Set<OrganizationPerson>()
                .Add(relationship);

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Ok(
                await ProjectRequiredAsync(
                    relationship.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Validerar och uppdaterar metadata för en aktiv organisationsrelation.
        /// </summary>
        public async Task<Result<OrganizationPersonResponse>> UpdateAsync(
            Guid organizationPersonId,
            UpdateOrganizationPersonRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<OrganizationPersonResponse>(
                    validationResult.Errors);
            }

            var relationship = await _db.Set<OrganizationPerson>()
                .SingleOrDefaultAsync(
                    x => x.Id == organizationPersonId,
                    cancellationToken);

            if (relationship is null)
            {
                return Result.Fail<OrganizationPersonResponse>(
                    new NotFoundError(
                        nameof(OrganizationPerson),
                        organizationPersonId));
            }

            if (relationship.ValidToUtc.HasValue)
            {
                return Result.Fail<OrganizationPersonResponse>(
                    new ConflictError(
                        "An ended organization-person relationship cannot be updated."));
            }

            await using var transaction = await _db.Database
                .BeginTransactionAsync(cancellationToken);

            if (request.IsPrimaryContact)
            {
                await DemoteCurrentPrimaryContactAsync(
                    relationship.OrganizationId,
                    relationship.Id,
                    cancellationToken);
            }

            relationship.RelationshipType = request.RelationshipType;
            relationship.JobTitle = TrimToNull(request.JobTitle);
            relationship.Department = TrimToNull(request.Department);
            relationship.IsPrimaryContact = request.IsPrimaryContact;
            relationship.Notes = TrimToNull(request.Notes);

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Ok(
                await ProjectRequiredAsync(
                    relationship.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Ersätter den kompletta funktionsuppsättningen för en aktiv organisationsrelation.
        /// </summary>
        public async Task<Result<OrganizationPersonResponse>> SetFunctionsAsync(
            Guid organizationPersonId,
            SetOrganizationPersonFunctionsRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<OrganizationPersonResponse>(
                    validationResult.Errors);
            }

            var relationship = await _db.Set<OrganizationPerson>()
                .Include(x => x.FunctionLinks)
                .SingleOrDefaultAsync(
                    x => x.Id == organizationPersonId,
                    cancellationToken);

            if (relationship is null)
            {
                return Result.Fail<OrganizationPersonResponse>(
                    new NotFoundError(
                        nameof(OrganizationPerson),
                        organizationPersonId));
            }

            if (relationship.ValidToUtc.HasValue)
            {
                return Result.Fail<OrganizationPersonResponse>(
                    new ConflictError(
                        "Functions cannot be changed on an ended organization-person relationship."));
            }

            var functionResult = await ValidateFunctionTypesAsync(
                request.ContactFunctionTypeIds,
                cancellationToken);

            if (functionResult.IsFailed)
            {
                return Result.Fail<OrganizationPersonResponse>(
                    functionResult.Errors);
            }

            var requestedIds = request.ContactFunctionTypeIds.ToHashSet();

            var linksToRemove = relationship.FunctionLinks
                .Where(x => !requestedIds.Contains(x.ContactFunctionTypeId))
                .ToList();

            if (linksToRemove.Count > 0)
            {
                _db.Set<OrganizationPersonFunction>()
                    .RemoveRange(linksToRemove);
            }

            var existingIds = relationship.FunctionLinks
                .Select(x => x.ContactFunctionTypeId)
                .ToHashSet();

            foreach (var functionTypeId in requestedIds.Except(existingIds))
            {
                relationship.FunctionLinks.Add(
                    new OrganizationPersonFunction
                    {
                        Id = Guid.CreateVersion7(),
                        OrganizationPersonId = relationship.Id,
                        ContactFunctionTypeId = functionTypeId
                    });
            }

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok(
                await ProjectRequiredAsync(
                    relationship.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Avslutar relationen och dess aktiva kontextuella kontaktpunkter utan att radera historik.
        /// </summary>
        public async Task<Result> EndAsync(
            Guid organizationPersonId,
            EndOrganizationPersonRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return validationResult;
            }

            var relationship = await _db.Set<OrganizationPerson>()
                .SingleOrDefaultAsync(
                    x => x.Id == organizationPersonId,
                    cancellationToken);

            if (relationship is null)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(OrganizationPerson),
                        organizationPersonId));
            }

            if (relationship.ValidToUtc.HasValue)
            {
                return Result.Fail(
                    new ConflictError(
                        "The organization-person relationship has already ended."));
            }

            if (relationship.ValidFromUtc.HasValue
                && request.ValidToUtc < relationship.ValidFromUtc.Value)
            {
                return Result.Fail(
                    new ConflictError(
                        "ValidToUtc cannot be earlier than ValidFromUtc."));
            }

            var incompatibleContactPointExists = await _db.Set<PartyContactPoint>()
                .AsNoTracking()
                .AnyAsync(
                    x =>
                        x.OrganizationPersonId == relationship.Id
                        && x.ValidToUtc == null
                        && x.ValidFromUtc > request.ValidToUtc,
                    cancellationToken);

            if (incompatibleContactPointExists)
            {
                return Result.Fail(
                    new ConflictError(
                        "The relationship cannot end before one of its active contextual contact points starts."));
            }

            await using var transaction = await _db.Database
                .BeginTransactionAsync(cancellationToken);

            relationship.ValidToUtc = request.ValidToUtc;

            await _db.Set<PartyContactPoint>()
                .Where(x =>
                    x.OrganizationPersonId == relationship.Id
                    && x.ValidToUtc == null)
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(
                            x => x.ValidToUtc,
                            request.ValidToUtc),
                    cancellationToken);

            await _db.Set<BusinessRelationshipContact>()
                .Where(x =>
                    x.OrganizationPersonId == relationship.Id
                    && x.IsPrimary)
                .ExecuteUpdateAsync(
                    setters => setters.SetProperty(
                        x => x.IsPrimary,
                        false),
                    cancellationToken);

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Ok();
        }

        /// <summary>
        /// Kontrollerar att organisationen och personen finns.
        /// </summary>
        private async Task<Result> ValidatePartyReferencesAsync(
            Guid organizationId,
            Guid personId,
            CancellationToken cancellationToken)
        {
            var organizationExists = await _db.Set<Organization>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == organizationId,
                    cancellationToken);

            if (!organizationExists)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(Organization),
                        organizationId));
            }

            var personExists = await _db.Set<Person>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == personId,
                    cancellationToken);

            if (!personExists)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(Person),
                        personId));
            }

            return Result.Ok();
        }

        /// <summary>
        /// Kontrollerar att samtliga efterfrågade funktionstyper finns.
        /// </summary>
        private async Task<Result> ValidateFunctionTypesAsync(
            IEnumerable<int> functionTypeIds,
            CancellationToken cancellationToken)
        {
            var requestedIds = functionTypeIds
                .Distinct()
                .ToArray();

            if (requestedIds.Length == 0)
            {
                return Result.Ok();
            }

            var existingIds = await _db.Set<ContactFunctionType>()
                .AsNoTracking()
                .Where(x => requestedIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            var missingIds = requestedIds
                .Except(existingIds)
                .ToArray();

            if (missingIds.Length == 0)
            {
                return Result.Ok();
            }

            var errors = missingIds
                .Select(id =>
                    new NotFoundError(
                        nameof(ContactFunctionType),
                        id))
                .Cast<IError>()
                .ToList();

            return Result.Fail(errors);
        }

        /// <summary>
        /// Tar bort primärstatus från en annan aktiv primärkontakt i organisationen.
        /// </summary>
        private async Task DemoteCurrentPrimaryContactAsync(
            Guid organizationId,
            Guid? excludeOrganizationPersonId,
            CancellationToken cancellationToken)
        {
            IQueryable<OrganizationPerson> query = _db.Set<OrganizationPerson>()
                .Where(x =>
                    x.OrganizationId == organizationId
                    && x.IsPrimaryContact
                    && x.ValidToUtc == null);

            if (excludeOrganizationPersonId.HasValue)
            {
                query = query.Where(
                    x => x.Id != excludeOrganizationPersonId.Value);
            }

            await query.ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(
                        x => x.IsPrimaryContact,
                        false),
                cancellationToken);
        }

        /// <summary>
        /// Projekterar en organisationsrelation direkt från databasen.
        /// </summary>
        private async Task<OrganizationPersonResponse?> ProjectAsync(
            Guid organizationPersonId,
            CancellationToken cancellationToken)
        {
            return await _db.Set<OrganizationPerson>()
                .AsNoTracking()
                .Where(x => x.Id == organizationPersonId)
                .Select(OrganizationPersonProjections.Details)
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Projekterar en sparad organisationsrelation och kastar om den oväntat saknas.
        /// </summary>
        private async Task<OrganizationPersonResponse> ProjectRequiredAsync(
            Guid organizationPersonId,
            CancellationToken cancellationToken)
        {
            return await ProjectAsync(
                    organizationPersonId,
                    cancellationToken)
                ?? throw new InvalidOperationException(
                    $"OrganizationPerson '{organizationPersonId}' was saved but could not be projected.");
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