using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Sourcing.Projections;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Hanterar idempotenta capability-evidensposter med SupplierProcessCapability som concurrency-root.
    /// </summary>
    public sealed class SupplierProcessCapabilityEvidenceService : ISupplierProcessCapabilityEvidenceService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;

        /// <summary>
        /// Skapar servicen med databas- och valideringsberoenden.
        /// </summary>
        public SupplierProcessCapabilityEvidenceService(Arb_IntDbContext db, IValidationService validationService)
        {
            _db = db;
            _validationService = validationService;
        }

        /// <summary>
        /// Skapar eller uppdaterar evidens med immutable source-identitet inom capabilityn.
        /// </summary>
        public async Task<Result<SupplierProcessCapabilityResponse>> UpsertAsync(Guid supplierProcessCapabilityId, UpsertSupplierProcessCapabilityEvidenceRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<SupplierProcessCapabilityResponse>(
                    validationResult.Errors);
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

            var sourceSystem = request.SourceSystem.Trim();
            var sourceRecordKey = request.SourceRecordKey.Trim();
            var sourceUrl = TrimToNull(request.SourceUrl);
            var notes = TrimToNull(request.Notes);
            var contentHash = TrimToNull(request.ContentHash);

            var evidence = await _db.Set<SupplierProcessCapabilityEvidence>()
                .SingleOrDefaultAsync(
                    x => x.SupplierProcessCapabilityId == capability.Id
                        && x.SourceSystem == sourceSystem
                        && x.SourceRecordKey == sourceRecordKey,
                    cancellationToken);

            if (evidence is not null
                && IsUnchanged(
                    evidence,
                    request.EvidenceType,
                    sourceUrl,
                    request.FirstObservedUtc,
                    request.LastObservedUtc,
                    notes,
                    contentHash))
            {
                return Result.Ok(
                    await ProjectRequiredAsync(
                        capability.Id,
                        cancellationToken));
            }

            _db.Entry(capability)
                .Property(x => x.RowVersion)
                .OriginalValue = request.SupplierProcessCapabilityRowVersion;

            if (evidence is null)
            {
                evidence = new SupplierProcessCapabilityEvidence
                {
                    Id = Guid.CreateVersion7(),
                    SupplierProcessCapabilityId = capability.Id,
                    EvidenceType = request.EvidenceType,
                    SourceSystem = sourceSystem,
                    SourceRecordKey = sourceRecordKey,
                    SourceUrl = sourceUrl,
                    FirstObservedUtc = request.FirstObservedUtc,
                    LastObservedUtc = request.LastObservedUtc,
                    Notes = notes,
                    ContentHash = contentHash
                };

                _db.Set<SupplierProcessCapabilityEvidence>()
                    .Add(evidence);
            }
            else
            {
                evidence.EvidenceType = request.EvidenceType;
                evidence.SourceUrl = sourceUrl;
                evidence.FirstObservedUtc = request.FirstObservedUtc;
                evidence.LastObservedUtc = request.LastObservedUtc;
                evidence.Notes = notes;
                evidence.ContentHash = contentHash;
            }

            TouchCapability(capability);

            var saveResult = await SaveChangesAsync(
                capability.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<SupplierProcessCapabilityResponse>(
                    saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    capability.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Tar bort en evidenspost som korrigering och touch:ar capabilityns concurrency-root.
        /// </summary>
        public async Task<Result<SupplierProcessCapabilityResponse>> DeleteAsync(Guid supplierProcessCapabilityEvidenceId, uint supplierProcessCapabilityRowVersion, CancellationToken cancellationToken = default)
        {
            var evidence = await _db.Set<SupplierProcessCapabilityEvidence>()
                .SingleOrDefaultAsync(
                    x => x.Id == supplierProcessCapabilityEvidenceId,
                    cancellationToken);

            if (evidence is null)
            {
                return Result.Fail<SupplierProcessCapabilityResponse>(
                    new NotFoundError(
                        nameof(SupplierProcessCapabilityEvidence),
                        supplierProcessCapabilityEvidenceId));
            }

            var capability = await _db.Set<SupplierProcessCapability>()
                .SingleOrDefaultAsync(
                    x => x.Id == evidence.SupplierProcessCapabilityId,
                    cancellationToken);

            if (capability is null)
            {
                return Result.Fail<SupplierProcessCapabilityResponse>(
                    new NotFoundError(
                        nameof(SupplierProcessCapability),
                        evidence.SupplierProcessCapabilityId));
            }

            _db.Entry(capability)
                .Property(x => x.RowVersion)
                .OriginalValue = supplierProcessCapabilityRowVersion;

            _db.Set<SupplierProcessCapabilityEvidence>()
                .Remove(evidence);

            TouchCapability(capability);

            var saveResult = await SaveChangesAsync(
                capability.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<SupplierProcessCapabilityResponse>(
                    saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    capability.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Kontrollerar om en upsert är en faktisk no-op så identiska återimporter inte skapar en onödig write.
        /// </summary>
        private static bool IsUnchanged(SupplierProcessCapabilityEvidence evidence, SourcingEvidenceType evidenceType, string? sourceUrl, DateTimeOffset firstObservedUtc, DateTimeOffset lastObservedUtc, string? notes, string? contentHash)
        {
            return evidence.EvidenceType == evidenceType
                && evidence.SourceUrl == sourceUrl
                && evidence.FirstObservedUtc == firstObservedUtc
                && evidence.LastObservedUtc == lastObservedUtc
                && evidence.Notes == notes
                && evidence.ContentHash == contentHash;
        }

        /// <summary>
        /// Markerar capability-roten så att evidens-writes konkurrerar på samma RowVersion.
        /// </summary>
        private static void TouchCapability(SupplierProcessCapability capability)
        {
            capability.ActionDateUtc = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Sparar evidensändringen och översätter concurrency- och unique-fel till domänfel.
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
                        "Another evidence row already uses the same source system and source record key for this supplier process capability."));
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
