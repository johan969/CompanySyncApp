using Arb_Int.Domain.Crm;
using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Sourcing.Projections;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Hanterar biologiska source-kopplingar med offeringet som concurrency-root.
    /// </summary>
    public sealed class SupplierMaterialOfferingBiologicalSourceService : ISupplierMaterialOfferingBiologicalSourceService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;

        /// <summary>
        /// Skapar servicen med databas- och valideringsberoenden.
        /// </summary>
        public SupplierMaterialOfferingBiologicalSourceService(Arb_IntDbContext db, IValidationService validationService)
        {
            _db = db;
            _validationService = validationService;
        }

        /// <summary>
        /// Lägger till ett claim som tillhör samma supplier-relation som offeringet.
        /// </summary>
        public async Task<Result<SupplierMaterialOfferingResponse>> AddAsync(Guid supplierMaterialOfferingId, CreateSupplierMaterialOfferingBiologicalSourceRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(validationResult.Errors);
            }

            var offering = await _db.Set<SupplierMaterialOffering>()
                .SingleOrDefaultAsync(x => x.Id == supplierMaterialOfferingId, cancellationToken);

            if (offering is null)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    new NotFoundError(nameof(SupplierMaterialOffering), supplierMaterialOfferingId));
            }

            var claim = await _db.Set<BusinessRelationshipBiologicalSourceClaim>()
                .AsNoTracking()
                .Where(x => x.Id == request.BusinessRelationshipBiologicalSourceClaimId)
                .Select(x => new
                {
                    x.Id,
                    x.BusinessRelationshipId
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (claim is null)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    new NotFoundError(
                        nameof(BusinessRelationshipBiologicalSourceClaim),
                        request.BusinessRelationshipBiologicalSourceClaimId));
            }

            if (claim.BusinessRelationshipId != offering.BusinessRelationshipId)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    new ConflictError(
                        "The biological source claim must belong to the same business relationship as the supplier material offering."));
            }

            var duplicateExists = await _db.Set<SupplierMaterialOfferingBiologicalSource>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.SupplierMaterialOfferingId == offering.Id
                        && x.BusinessRelationshipBiologicalSourceClaimId == claim.Id,
                    cancellationToken);

            if (duplicateExists)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    new ConflictError(
                        "The supplier material offering already contains this biological source claim."));
            }

            var proportionResult = await ValidateProportionTotalAsync(
                offering.Id,
                request.ProportionPercent,
                excludeLinkId: null,
                cancellationToken);

            if (proportionResult.IsFailed)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    proportionResult.Errors);
            }

            _db.Entry(offering)
                .Property(x => x.RowVersion)
                .OriginalValue = request.SupplierMaterialOfferingRowVersion;

            var link = new SupplierMaterialOfferingBiologicalSource
            {
                Id = Guid.CreateVersion7(),
                BusinessRelationshipId = offering.BusinessRelationshipId,
                SupplierMaterialOfferingId = offering.Id,
                BusinessRelationshipBiologicalSourceClaimId = claim.Id,
                Role = request.Role,
                ProportionPercent = request.ProportionPercent,
                Notes = TrimToNull(request.Notes)
            };

            _db.Set<SupplierMaterialOfferingBiologicalSource>()
                .Add(link);

            TouchOffering(offering);

            var saveResult = await SaveChangesAsync(
                offering.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    offering.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Uppdaterar metadata på en befintlig länk och behåller claim-identiteten oförändrad.
        /// </summary>
        public async Task<Result<SupplierMaterialOfferingResponse>> UpdateAsync(Guid supplierMaterialOfferingBiologicalSourceId, UpdateSupplierMaterialOfferingBiologicalSourceRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    validationResult.Errors);
            }

            var link = await _db.Set<SupplierMaterialOfferingBiologicalSource>()
                .SingleOrDefaultAsync(
                    x => x.Id == supplierMaterialOfferingBiologicalSourceId,
                    cancellationToken);

            if (link is null)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    new NotFoundError(
                        nameof(SupplierMaterialOfferingBiologicalSource),
                        supplierMaterialOfferingBiologicalSourceId));
            }

            var offering = await _db.Set<SupplierMaterialOffering>()
                .SingleOrDefaultAsync(
                    x => x.Id == link.SupplierMaterialOfferingId,
                    cancellationToken);

            if (offering is null)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    new NotFoundError(
                        nameof(SupplierMaterialOffering),
                        link.SupplierMaterialOfferingId));
            }

            var proportionResult = await ValidateProportionTotalAsync(
                offering.Id,
                request.ProportionPercent,
                link.Id,
                cancellationToken);

            if (proportionResult.IsFailed)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    proportionResult.Errors);
            }

            _db.Entry(offering)
                .Property(x => x.RowVersion)
                .OriginalValue = request.SupplierMaterialOfferingRowVersion;

            link.Role = request.Role;
            link.ProportionPercent = request.ProportionPercent;
            link.Notes = TrimToNull(request.Notes);

            TouchOffering(offering);

            var saveResult = await SaveChangesAsync(
                offering.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    offering.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Tar bort länken och låter CRM-claimet leva vidare oberoende av offeringet.
        /// </summary>
        public async Task<Result<SupplierMaterialOfferingResponse>> DeleteAsync(Guid supplierMaterialOfferingBiologicalSourceId, uint supplierMaterialOfferingRowVersion, CancellationToken cancellationToken = default)
        {
            var link = await _db.Set<SupplierMaterialOfferingBiologicalSource>()
                .SingleOrDefaultAsync(
                    x => x.Id == supplierMaterialOfferingBiologicalSourceId,
                    cancellationToken);

            if (link is null)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    new NotFoundError(
                        nameof(SupplierMaterialOfferingBiologicalSource),
                        supplierMaterialOfferingBiologicalSourceId));
            }

            var offering = await _db.Set<SupplierMaterialOffering>()
                .SingleOrDefaultAsync(
                    x => x.Id == link.SupplierMaterialOfferingId,
                    cancellationToken);

            if (offering is null)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    new NotFoundError(
                        nameof(SupplierMaterialOffering),
                        link.SupplierMaterialOfferingId));
            }

            _db.Entry(offering)
                .Property(x => x.RowVersion)
                .OriginalValue = supplierMaterialOfferingRowVersion;

            _db.Set<SupplierMaterialOfferingBiologicalSource>()
                .Remove(link);

            TouchOffering(offering);

            var saveResult = await SaveChangesAsync(
                offering.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    offering.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Säkerställer att angivna kända proportioner inte överstiger 100 procent tillsammans.
        /// </summary>
        private async Task<Result> ValidateProportionTotalAsync(Guid supplierMaterialOfferingId, decimal? proposedProportionPercent, Guid? excludeLinkId, CancellationToken cancellationToken)
        {
            if (!proposedProportionPercent.HasValue)
            {
                return Result.Ok();
            }

            IQueryable<SupplierMaterialOfferingBiologicalSource> query = _db
                .Set<SupplierMaterialOfferingBiologicalSource>()
                .AsNoTracking()
                .Where(x => x.SupplierMaterialOfferingId == supplierMaterialOfferingId);

            if (excludeLinkId.HasValue)
            {
                query = query.Where(
                    x => x.Id != excludeLinkId.Value);
            }

            var existingTotal = await query
                .SumAsync(
                    x => x.ProportionPercent ?? 0m,
                    cancellationToken);

            if (existingTotal + proposedProportionPercent.Value <= 100m)
            {
                return Result.Ok();
            }

            return Result.Fail(
                new ConflictError(
                    "The sum of declared biological source proportions cannot exceed 100 percent."));
        }

        /// <summary>
        /// Markerar offering-roten så att child-writes konkurrerar på samma RowVersion.
        /// </summary>
        private static void TouchOffering(SupplierMaterialOffering offering)
        {
            offering.ActionDateUtc = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Sparar länken och översätter concurrency- och unique-fel till domänfel.
        /// </summary>
        private async Task<Result> SaveChangesAsync(Guid supplierMaterialOfferingId, CancellationToken cancellationToken)
        {
            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                return Result.Ok();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result.Fail(
                    new ConcurrencyError(
                        nameof(SupplierMaterialOffering),
                        supplierMaterialOfferingId));
            }
            catch (DbUpdateException exception)
                when (IsUniqueViolation(exception))
            {
                return Result.Fail(
                    new ConflictError(
                        "The supplier material offering already contains this biological source claim."));
            }
        }

        /// <summary>
        /// Projekterar offeringet med samtliga offering-ägda child-data.
        /// </summary>
        private async Task<SupplierMaterialOfferingResponse?> ProjectAsync(Guid supplierMaterialOfferingId, CancellationToken cancellationToken)
        {
            return await _db.Set<SupplierMaterialOffering>()
                .AsNoTracking()
                .Where(x => x.Id == supplierMaterialOfferingId)
                .Select(SupplierMaterialOfferingProjections.Details)
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Projekterar ett sparat offering och kastar om det oväntat saknas.
        /// </summary>
        private async Task<SupplierMaterialOfferingResponse> ProjectRequiredAsync(Guid supplierMaterialOfferingId, CancellationToken cancellationToken)
        {
            return await ProjectAsync(
                    supplierMaterialOfferingId,
                    cancellationToken)
                ?? throw new InvalidOperationException(
                    $"SupplierMaterialOffering '{supplierMaterialOfferingId}' was saved but could not be projected.");
        }

        /// <summary>
        /// Kontrollerar om databasfelet kommer från en unik constraint.
        /// </summary>
        private static bool IsUniqueViolation(DbUpdateException exception)
        {
            return exception.InnerException
                is PostgresException
                {
                    SqlState:
                        PostgresErrorCodes.UniqueViolation
                };
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
