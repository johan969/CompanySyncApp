using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Sourcing.Projections;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Hanterar idempotenta offering-evidensposter med SupplierMaterialOffering som concurrency-root.
    /// </summary>
    public sealed class SupplierMaterialOfferingEvidenceService : ISupplierMaterialOfferingEvidenceService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;

        /// <summary>
        /// Skapar servicen med databas- och valideringsberoenden.
        /// </summary>
        public SupplierMaterialOfferingEvidenceService(Arb_IntDbContext db, IValidationService validationService)
        {
            _db = db;
            _validationService = validationService;
        }

        /// <summary>
        /// Skapar eller uppdaterar evidens med immutable source-identitet inom offeringet.
        /// </summary>
        public async Task<Result<SupplierMaterialOfferingResponse>> UpsertAsync(Guid supplierMaterialOfferingId, UpsertSupplierMaterialOfferingEvidenceRequest request, CancellationToken cancellationToken = default)
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

            var sourceSystem = request.SourceSystem.Trim();
            var sourceRecordKey = request.SourceRecordKey.Trim();
            var sourceUrl = TrimToNull(request.SourceUrl);
            var notes = TrimToNull(request.Notes);
            var contentHash = TrimToNull(request.ContentHash);

            var evidence = await _db.Set<SupplierMaterialOfferingEvidence>()
                .SingleOrDefaultAsync(
                    x => x.SupplierMaterialOfferingId == offering.Id
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
                        offering.Id,
                        cancellationToken));
            }

            _db.Entry(offering)
                .Property(x => x.RowVersion)
                .OriginalValue = request.SupplierMaterialOfferingRowVersion;

            if (evidence is null)
            {
                evidence = new SupplierMaterialOfferingEvidence
                {
                    Id = Guid.CreateVersion7(),
                    SupplierMaterialOfferingId = offering.Id,
                    EvidenceType = request.EvidenceType,
                    SourceSystem = sourceSystem,
                    SourceRecordKey = sourceRecordKey,
                    SourceUrl = sourceUrl,
                    FirstObservedUtc = request.FirstObservedUtc,
                    LastObservedUtc = request.LastObservedUtc,
                    Notes = notes,
                    ContentHash = contentHash
                };

                _db.Set<SupplierMaterialOfferingEvidence>()
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

            TouchOffering(offering);

            var saveResult = await SaveChangesAsync(
                offering.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    offering.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Tar bort en evidenspost som korrigering och touch:ar offeringets concurrency-root.
        /// </summary>
        public async Task<Result<SupplierMaterialOfferingResponse>> DeleteAsync(Guid supplierMaterialOfferingEvidenceId, uint supplierMaterialOfferingRowVersion, CancellationToken cancellationToken = default)
        {
            var evidence = await _db.Set<SupplierMaterialOfferingEvidence>()
                .SingleOrDefaultAsync(
                    x => x.Id == supplierMaterialOfferingEvidenceId,
                    cancellationToken);

            if (evidence is null)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    new NotFoundError(
                        nameof(SupplierMaterialOfferingEvidence),
                        supplierMaterialOfferingEvidenceId));
            }

            var offering = await _db.Set<SupplierMaterialOffering>()
                .SingleOrDefaultAsync(
                    x => x.Id == evidence.SupplierMaterialOfferingId,
                    cancellationToken);

            if (offering is null)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(
                    new NotFoundError(
                        nameof(SupplierMaterialOffering),
                        evidence.SupplierMaterialOfferingId));
            }

            _db.Entry(offering)
                .Property(x => x.RowVersion)
                .OriginalValue = supplierMaterialOfferingRowVersion;

            _db.Set<SupplierMaterialOfferingEvidence>()
                .Remove(evidence);

            TouchOffering(offering);

            var saveResult = await SaveChangesAsync(
                offering.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<SupplierMaterialOfferingResponse>(saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    offering.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Kontrollerar om en upsert är en faktisk no-op så identiska återimporter inte skapar en onödig write.
        /// </summary>
        private static bool IsUnchanged(SupplierMaterialOfferingEvidence evidence, SourcingEvidenceType evidenceType, string? sourceUrl, DateTimeOffset firstObservedUtc, DateTimeOffset lastObservedUtc, string? notes, string? contentHash)
        {
            return evidence.EvidenceType == evidenceType
                && evidence.SourceUrl == sourceUrl
                && evidence.FirstObservedUtc == firstObservedUtc
                && evidence.LastObservedUtc == lastObservedUtc
                && evidence.Notes == notes
                && evidence.ContentHash == contentHash;
        }

        /// <summary>
        /// Markerar offering-roten så att evidens-writes konkurrerar på samma RowVersion.
        /// </summary>
        private static void TouchOffering(SupplierMaterialOffering offering)
        {
            offering.ActionDateUtc = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Sparar evidensändringen och översätter concurrency- och unique-fel till domänfel.
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
                        "Another evidence row already uses the same source system and source record key for this supplier material offering."));
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
