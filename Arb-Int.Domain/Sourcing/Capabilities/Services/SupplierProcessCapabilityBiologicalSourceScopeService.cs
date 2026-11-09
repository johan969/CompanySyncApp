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
    /// Hanterar biologiska capability-scopes med SupplierProcessCapability som concurrency-root.
    /// </summary>
    public sealed class SupplierProcessCapabilityBiologicalSourceScopeService : ISupplierProcessCapabilityBiologicalSourceScopeService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;

        /// <summary>
        /// Skapar servicen med databas- och valideringsberoenden.
        /// </summary>
        public SupplierProcessCapabilityBiologicalSourceScopeService(Arb_IntDbContext db, IValidationService validationService)
        {
            _db = db;
            _validationService = validationService;
        }

        /// <summary>
        /// Lägger till ett claim som tillhör samma supplier-relation som capabilityn.
        /// </summary>
        public async Task<Result<SupplierProcessCapabilityResponse>> AddAsync(Guid supplierProcessCapabilityId, CreateSupplierProcessCapabilityBiologicalSourceScopeRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<SupplierProcessCapabilityResponse>(validationResult.Errors);
            }

            var capability = await _db.Set<SupplierProcessCapability>()
                .SingleOrDefaultAsync(
                    x => x.Id == supplierProcessCapabilityId,
                    cancellationToken);

            if (capability is null)
            {
                return Result.Fail<SupplierProcessCapabilityResponse>(
                    new NotFoundError(
                        nameof(SupplierProcessCapability),
                        supplierProcessCapabilityId));
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
                return Result.Fail<SupplierProcessCapabilityResponse>(
                    new NotFoundError(
                        nameof(BusinessRelationshipBiologicalSourceClaim),
                        request.BusinessRelationshipBiologicalSourceClaimId));
            }

            if (claim.BusinessRelationshipId != capability.BusinessRelationshipId)
            {
                return Result.Fail<SupplierProcessCapabilityResponse>(
                    new ConflictError(
                        "The biological source claim must belong to the same business relationship as the supplier process capability."));
            }

            var duplicateExists = await _db.Set<SupplierProcessCapabilityBiologicalSourceScope>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.SupplierProcessCapabilityId == capability.Id
                        && x.BusinessRelationshipBiologicalSourceClaimId == claim.Id,
                    cancellationToken);

            if (duplicateExists)
            {
                return Result.Fail<SupplierProcessCapabilityResponse>(
                    new ConflictError(
                        "The supplier process capability already contains this biological source scope."));
            }

            _db.Entry(capability)
                .Property(x => x.RowVersion)
                .OriginalValue = request.SupplierProcessCapabilityRowVersion;

            var scope = new SupplierProcessCapabilityBiologicalSourceScope
            {
                Id = Guid.CreateVersion7(),
                BusinessRelationshipId = capability.BusinessRelationshipId,
                SupplierProcessCapabilityId = capability.Id,
                BusinessRelationshipBiologicalSourceClaimId = claim.Id,
                Notes = TrimToNull(request.Notes)
            };

            _db.Set<SupplierProcessCapabilityBiologicalSourceScope>()
                .Add(scope);

            TouchCapability(capability);

            var saveResult = await SaveChangesAsync(
                capability.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<SupplierProcessCapabilityResponse>(saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    capability.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Uppdaterar anteckningar och behåller capability- och claim-identiteten oförändrad.
        /// </summary>
        public async Task<Result<SupplierProcessCapabilityResponse>> UpdateAsync(Guid supplierProcessCapabilityBiologicalSourceScopeId, UpdateSupplierProcessCapabilityBiologicalSourceScopeRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<SupplierProcessCapabilityResponse>(validationResult.Errors);
            }

            var scope = await _db.Set<SupplierProcessCapabilityBiologicalSourceScope>()
                .SingleOrDefaultAsync(
                    x => x.Id == supplierProcessCapabilityBiologicalSourceScopeId,
                    cancellationToken);

            if (scope is null)
            {
                return Result.Fail<SupplierProcessCapabilityResponse>(
                    new NotFoundError(
                        nameof(SupplierProcessCapabilityBiologicalSourceScope),
                        supplierProcessCapabilityBiologicalSourceScopeId));
            }

            var capability = await _db.Set<SupplierProcessCapability>()
                .SingleOrDefaultAsync(
                    x => x.Id == scope.SupplierProcessCapabilityId,
                    cancellationToken);

            if (capability is null)
            {
                return Result.Fail<SupplierProcessCapabilityResponse>(
                    new NotFoundError(
                        nameof(SupplierProcessCapability),
                        scope.SupplierProcessCapabilityId));
            }

            var notes = TrimToNull(request.Notes);

            if (scope.Notes == notes)
            {
                return Result.Ok(
                    await ProjectRequiredAsync(
                        capability.Id,
                        cancellationToken));
            }

            _db.Entry(capability)
                .Property(x => x.RowVersion)
                .OriginalValue = request.SupplierProcessCapabilityRowVersion;

            scope.Notes = notes;

            TouchCapability(capability);

            var saveResult = await SaveChangesAsync(
                capability.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<SupplierProcessCapabilityResponse>(saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    capability.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Tar bort scope-länken och låter CRM-claimet leva vidare oberoende av capabilityn.
        /// </summary>
        public async Task<Result<SupplierProcessCapabilityResponse>> DeleteAsync(Guid supplierProcessCapabilityBiologicalSourceScopeId, uint supplierProcessCapabilityRowVersion, CancellationToken cancellationToken = default)
        {
            var scope = await _db.Set<SupplierProcessCapabilityBiologicalSourceScope>()
                .SingleOrDefaultAsync(
                    x => x.Id == supplierProcessCapabilityBiologicalSourceScopeId,
                    cancellationToken);

            if (scope is null)
            {
                return Result.Fail<SupplierProcessCapabilityResponse>(
                    new NotFoundError(
                        nameof(SupplierProcessCapabilityBiologicalSourceScope),
                        supplierProcessCapabilityBiologicalSourceScopeId));
            }

            var capability = await _db.Set<SupplierProcessCapability>()
                .SingleOrDefaultAsync(
                    x => x.Id == scope.SupplierProcessCapabilityId,
                    cancellationToken);

            if (capability is null)
            {
                return Result.Fail<SupplierProcessCapabilityResponse>(
                    new NotFoundError(
                        nameof(SupplierProcessCapability),
                        scope.SupplierProcessCapabilityId));
            }

            _db.Entry(capability)
                .Property(x => x.RowVersion)
                .OriginalValue = supplierProcessCapabilityRowVersion;

            _db.Set<SupplierProcessCapabilityBiologicalSourceScope>()
                .Remove(scope);

            TouchCapability(capability);

            var saveResult = await SaveChangesAsync(
                capability.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<SupplierProcessCapabilityResponse>(saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    capability.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Markerar capability-roten så att scope-writes konkurrerar på samma RowVersion.
        /// </summary>
        private static void TouchCapability(SupplierProcessCapability capability)
        {
            capability.ActionDateUtc = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Sparar scope-ändringen och översätter concurrency- och unique-fel till domänfel.
        /// </summary>
        private async Task<Result> SaveChangesAsync(Guid supplierProcessCapabilityId, CancellationToken cancellationToken)
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
                        nameof(SupplierProcessCapability),
                        supplierProcessCapabilityId));
            }
            catch (DbUpdateException exception)
                when (IsUniqueViolation(exception))
            {
                return Result.Fail(
                    new ConflictError(
                        "The supplier process capability already contains this biological source scope."));
            }
        }

        /// <summary>
        /// Projekterar capabilityn med capability-ägda scopes och evidens.
        /// </summary>
        private async Task<SupplierProcessCapabilityResponse?> ProjectAsync(Guid supplierProcessCapabilityId, CancellationToken cancellationToken)
        {
            return await _db.Set<SupplierProcessCapability>()
                .AsNoTracking()
                .Where(x => x.Id == supplierProcessCapabilityId)
                .Select(SupplierProcessCapabilityProjections.Details)
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Projekterar en sparad capability och kastar om den oväntat saknas.
        /// </summary>
        private async Task<SupplierProcessCapabilityResponse> ProjectRequiredAsync(Guid supplierProcessCapabilityId, CancellationToken cancellationToken)
        {
            return await ProjectAsync(
                    supplierProcessCapabilityId,
                    cancellationToken)
                ?? throw new InvalidOperationException(
                    $"SupplierProcessCapability '{supplierProcessCapabilityId}' was saved but could not be projected.");
        }

        /// <summary>
        /// Kontrollerar om databasfelet kommer från en unik constraint.
        /// </summary>
        private static bool IsUniqueViolation(DbUpdateException exception)
        {
            return exception.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation
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
