using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Parties;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Hanterar kontaktpersoner inom specifika affärsrelationer.
    /// </summary>
    public sealed class BusinessRelationshipContactService : IBusinessRelationshipContactService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;

        /// <summary>
        /// Skapar relationskontaktservicen med databas- och valideringsberoenden.
        /// </summary>
        public BusinessRelationshipContactService(Arb_IntDbContext db, IValidationService validationService)
        {
            _db = db;
            _validationService = validationService;
        }

        /// <summary>
        /// Hämtar samtliga kontakter för en affärsrelation.
        /// </summary>
        public async Task<Result<IReadOnlyList<BusinessRelationshipContactResponse>>> GetAsync(
            Guid businessRelationshipId,
            CancellationToken cancellationToken = default)
        {
            var relationshipExists = await _db.Set<BusinessRelationship>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == businessRelationshipId, cancellationToken);

            if (!relationshipExists)
            {
                return Result.Fail<IReadOnlyList<BusinessRelationshipContactResponse>>(
                    new NotFoundError(nameof(BusinessRelationship), businessRelationshipId));
            }

            var contacts = await _db.Set<BusinessRelationshipContact>()
                .AsNoTracking()
                .Where(x => x.BusinessRelationshipId == businessRelationshipId)
                .OrderByDescending(x => x.IsPrimary)
                .ThenBy(x => x.OrganizationPerson.Person.Name)
                .ThenBy(x => x.Role)
                .Select(BusinessRelationshipContactProjections.Details)
                .ToListAsync(cancellationToken);

            return Result.Ok<IReadOnlyList<BusinessRelationshipContactResponse>>(contacts);
        }

        /// <summary>
        /// Hämtar en enskild relationskontakt.
        /// </summary>
        public async Task<Result<BusinessRelationshipContactResponse>> GetByIdAsync(
            Guid businessRelationshipContactId,
            CancellationToken cancellationToken = default)
        {
            var response = await ProjectAsync(
                businessRelationshipContactId,
                cancellationToken);

            if (response is null)
            {
                return Result.Fail<BusinessRelationshipContactResponse>(
                    new NotFoundError(
                        nameof(BusinessRelationshipContact),
                        businessRelationshipContactId));
            }

            return Result.Ok(response);
        }

        /// <summary>
        /// Validerar och lägger till en kontakt som tillhör relationens motpartsorganisation.
        /// </summary>
        public async Task<Result<BusinessRelationshipContactResponse>> CreateAsync(
            Guid businessRelationshipId,
            CreateBusinessRelationshipContactRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<BusinessRelationshipContactResponse>(
                    validationResult.Errors);
            }

            var contextResult = await ValidateContactContextAsync(
                businessRelationshipId,
                request.OrganizationPersonId,
                cancellationToken);

            if (contextResult.IsFailed)
            {
                return Result.Fail<BusinessRelationshipContactResponse>(
                    contextResult.Errors);
            }

            var duplicateExists = await _db.Set<BusinessRelationshipContact>()
                .AsNoTracking()
                .AnyAsync(
                    x =>
                        x.BusinessRelationshipId == businessRelationshipId
                        && x.OrganizationPersonId == request.OrganizationPersonId
                        && x.Role == request.Role,
                    cancellationToken);

            if (duplicateExists)
            {
                return Result.Fail<BusinessRelationshipContactResponse>(
                    new ConflictError(
                        "The person already has this role on the business relationship."));
            }

            await using var transaction = await _db.Database
                .BeginTransactionAsync(cancellationToken);

            if (request.IsPrimary)
            {
                await DemoteCurrentPrimaryAsync(
                    businessRelationshipId,
                    excludeContactId: null,
                    cancellationToken);
            }

            var contact = new BusinessRelationshipContact
            {
                Id = Guid.CreateVersion7(),
                BusinessRelationshipId = businessRelationshipId,
                OrganizationPersonId = request.OrganizationPersonId,
                Role = request.Role,
                IsPrimary = request.IsPrimary,
                Notes = TrimToNull(request.Notes)
            };

            _db.Set<BusinessRelationshipContact>()
                .Add(contact);

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Ok(
                await ProjectRequiredAsync(
                    contact.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Validerar och uppdaterar roll och metadata för en relationskontakt.
        /// </summary>
        public async Task<Result<BusinessRelationshipContactResponse>> UpdateAsync(
            Guid businessRelationshipContactId,
            UpdateBusinessRelationshipContactRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<BusinessRelationshipContactResponse>(
                    validationResult.Errors);
            }

            var contact = await _db.Set<BusinessRelationshipContact>()
                .SingleOrDefaultAsync(
                    x => x.Id == businessRelationshipContactId,
                    cancellationToken);

            if (contact is null)
            {
                return Result.Fail<BusinessRelationshipContactResponse>(
                    new NotFoundError(
                        nameof(BusinessRelationshipContact),
                        businessRelationshipContactId));
            }

            var contextResult = await ValidateContactContextAsync(
                contact.BusinessRelationshipId,
                contact.OrganizationPersonId,
                cancellationToken);

            if (contextResult.IsFailed)
            {
                return Result.Fail<BusinessRelationshipContactResponse>(
                    contextResult.Errors);
            }

            var duplicateExists = await _db.Set<BusinessRelationshipContact>()
                .AsNoTracking()
                .AnyAsync(
                    x =>
                        x.Id != contact.Id
                        && x.BusinessRelationshipId == contact.BusinessRelationshipId
                        && x.OrganizationPersonId == contact.OrganizationPersonId
                        && x.Role == request.Role,
                    cancellationToken);

            if (duplicateExists)
            {
                return Result.Fail<BusinessRelationshipContactResponse>(
                    new ConflictError(
                        "The person already has this role on the business relationship."));
            }

            await using var transaction = await _db.Database
                .BeginTransactionAsync(cancellationToken);

            if (request.IsPrimary)
            {
                await DemoteCurrentPrimaryAsync(
                    contact.BusinessRelationshipId,
                    contact.Id,
                    cancellationToken);
            }

            contact.Role = request.Role;
            contact.IsPrimary = request.IsPrimary;
            contact.Notes = TrimToNull(request.Notes);

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Ok(
                await ProjectRequiredAsync(
                    contact.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Tar bort en kontaktkoppling utan att påverka OrganizationPerson eller Person.
        /// </summary>
        public async Task<Result> DeleteAsync(
            Guid businessRelationshipContactId,
            CancellationToken cancellationToken = default)
        {
            var contact = await _db.Set<BusinessRelationshipContact>()
                .SingleOrDefaultAsync(
                    x => x.Id == businessRelationshipContactId,
                    cancellationToken);

            if (contact is null)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(BusinessRelationshipContact),
                        businessRelationshipContactId));
            }

            _db.Set<BusinessRelationshipContact>()
                .Remove(contact);

            await _db.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }

        /// <summary>
        /// Säkerställer att kontaktpersonen tillhör affärsrelationens motpartsorganisation.
        /// </summary>
        private async Task<Result> ValidateContactContextAsync(
            Guid businessRelationshipId,
            Guid organizationPersonId,
            CancellationToken cancellationToken)
        {
            var relationship = await _db.Set<BusinessRelationship>()
                .AsNoTracking()
                .Where(x => x.Id == businessRelationshipId)
                .Select(x => new
                {
                    x.CounterpartyId
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (relationship is null)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(BusinessRelationship),
                        businessRelationshipId));
            }

            var counterpartyIsOrganization = await _db.Set<Organization>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == relationship.CounterpartyId,
                    cancellationToken);

            if (!counterpartyIsOrganization)
            {
                return Result.Fail(
                    new ConflictError(
                        "BusinessRelationshipContact can only be used when the relationship counterparty is an organization."));
            }

            var organizationPerson = await _db.Set<OrganizationPerson>()
                .AsNoTracking()
                .Where(x => x.Id == organizationPersonId)
                .Select(x => new
                {
                    x.OrganizationId,
                    x.ValidToUtc
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (organizationPerson is null)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(OrganizationPerson),
                        organizationPersonId));
            }

            if (organizationPerson.OrganizationId != relationship.CounterpartyId)
            {
                return Result.Fail(
                    new ConflictError(
                        "The selected organization person does not belong to the relationship counterparty."));
            }

            if (organizationPerson.ValidToUtc.HasValue)
            {
                return Result.Fail(
                    new ConflictError(
                        "An ended organization-person relationship cannot be used as an active business relationship contact."));
            }

            return Result.Ok();
        }

        /// <summary>
        /// Tar bort primärstatus från övriga kontakter i samma affärsrelation.
        /// </summary>
        private async Task DemoteCurrentPrimaryAsync(
            Guid businessRelationshipId,
            Guid? excludeContactId,
            CancellationToken cancellationToken)
        {
            IQueryable<BusinessRelationshipContact> query = _db.Set<BusinessRelationshipContact>()
                .Where(x =>
                    x.BusinessRelationshipId == businessRelationshipId
                    && x.IsPrimary);

            if (excludeContactId.HasValue)
            {
                query = query.Where(
                    x => x.Id != excludeContactId.Value);
            }

            await query.ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    x => x.IsPrimary,
                    false),
                cancellationToken);
        }

        /// <summary>
        /// Projekterar en relationskontakt direkt från databasen.
        /// </summary>
        private async Task<BusinessRelationshipContactResponse?> ProjectAsync(
            Guid businessRelationshipContactId,
            CancellationToken cancellationToken)
        {
            return await _db.Set<BusinessRelationshipContact>()
                .AsNoTracking()
                .Where(x => x.Id == businessRelationshipContactId)
                .Select(BusinessRelationshipContactProjections.Details)
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Projekterar en sparad relationskontakt och kastar om den oväntat saknas.
        /// </summary>
        private async Task<BusinessRelationshipContactResponse> ProjectRequiredAsync(
            Guid businessRelationshipContactId,
            CancellationToken cancellationToken)
        {
            return await ProjectAsync(
                    businessRelationshipContactId,
                    cancellationToken)
                ?? throw new InvalidOperationException(
                    $"BusinessRelationshipContact '{businessRelationshipContactId}' was saved but could not be projected.");
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