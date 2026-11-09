using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Sourcing.Projections;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Hanterar analysis results med både child-concurrency och SupplyAnalysis som aggregate concurrency-root.
    /// </summary>
    public sealed class SupplyAnalysisResultService : ISupplyAnalysisResultService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;

        /// <summary>
        /// Skapar servicen med databas- och valideringsberoenden.
        /// </summary>
        public SupplyAnalysisResultService(Arb_IntDbContext db, IValidationService validationService)
        {
            _db = db;
            _validationService = validationService;
        }

        /// <summary>
        /// Lägger till ett resultat efter target- och reference-kontroll.
        /// </summary>
        public async Task<Result<SupplyAnalysisResponse>> AddAsync(Guid supplyAnalysisId, CreateSupplyAnalysisResultRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<SupplyAnalysisResponse>(validationResult.Errors);
            }

            var analysis = await _db.Set<SupplyAnalysis>()
                .SingleOrDefaultAsync(x => x.Id == supplyAnalysisId, cancellationToken);

            if (analysis is null)
            {
                return Result.Fail<SupplyAnalysisResponse>(
                    new NotFoundError(nameof(SupplyAnalysis), supplyAnalysisId));
            }

            var referencesResult = await ValidateReferencesAsync(
                request.ConstituentId,
                request.CharacteristicId,
                request.UnitOfMeasureId,
                cancellationToken);

            if (referencesResult.IsFailed)
            {
                return Result.Fail<SupplyAnalysisResponse>(referencesResult.Errors);
            }

            _db.Entry(analysis)
                .Property(x => x.RowVersion)
                .OriginalValue = request.SupplyAnalysisRowVersion;

            var result = new SupplyAnalysisResult
            {
                Id = Guid.CreateVersion7(),
                SupplyAnalysisId = analysis.Id,
                ConstituentId = request.ConstituentId,
                CharacteristicId = request.CharacteristicId,
                AnalyteName = TrimToNull(request.AnalyteName),
                Qualifier = request.Qualifier,
                NumericValue = request.NumericValue,
                RangeMaximumValue = request.RangeMaximumValue,
                TextValue = TrimToNull(request.TextValue),
                UnitOfMeasureId = request.UnitOfMeasureId,
                Basis = request.Basis,
                DetectionLimit = request.DetectionLimit,
                AnalysisMethod = TrimToNull(request.AnalysisMethod),
                VerificationState = request.VerificationState,
                Notes = TrimToNull(request.Notes)
            };

            _db.Set<SupplyAnalysisResult>().Add(result);
            TouchAnalysis(analysis);

            var saveResult = await SaveChangesAsync(
                analysis.Id,
                result.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<SupplyAnalysisResponse>(saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(analysis.Id, cancellationToken));
        }

        /// <summary>
        /// Uppdaterar ett resultats target-mapping och mätdata utan att flytta det till en annan analysis.
        /// </summary>
        public async Task<Result<SupplyAnalysisResponse>> UpdateAsync(Guid supplyAnalysisResultId, UpdateSupplyAnalysisResultRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<SupplyAnalysisResponse>(validationResult.Errors);
            }

            var result = await _db.Set<SupplyAnalysisResult>()
                .SingleOrDefaultAsync(x => x.Id == supplyAnalysisResultId, cancellationToken);

            if (result is null)
            {
                return Result.Fail<SupplyAnalysisResponse>(
                    new NotFoundError(nameof(SupplyAnalysisResult), supplyAnalysisResultId));
            }

            var analysis = await _db.Set<SupplyAnalysis>()
                .SingleOrDefaultAsync(x => x.Id == result.SupplyAnalysisId, cancellationToken);

            if (analysis is null)
            {
                return Result.Fail<SupplyAnalysisResponse>(
                    new NotFoundError(nameof(SupplyAnalysis), result.SupplyAnalysisId));
            }

            var referencesResult = await ValidateReferencesAsync(
                request.ConstituentId,
                request.CharacteristicId,
                request.UnitOfMeasureId,
                cancellationToken);

            if (referencesResult.IsFailed)
            {
                return Result.Fail<SupplyAnalysisResponse>(referencesResult.Errors);
            }

            var analyteName = TrimToNull(request.AnalyteName);
            var textValue = TrimToNull(request.TextValue);
            var analysisMethod = TrimToNull(request.AnalysisMethod);
            var notes = TrimToNull(request.Notes);

            if (result.ConstituentId == request.ConstituentId
                && result.CharacteristicId == request.CharacteristicId
                && result.AnalyteName == analyteName
                && result.Qualifier == request.Qualifier
                && result.NumericValue == request.NumericValue
                && result.RangeMaximumValue == request.RangeMaximumValue
                && result.TextValue == textValue
                && result.UnitOfMeasureId == request.UnitOfMeasureId
                && result.Basis == request.Basis
                && result.DetectionLimit == request.DetectionLimit
                && result.AnalysisMethod == analysisMethod
                && result.VerificationState == request.VerificationState
                && result.Notes == notes)
            {
                return Result.Ok(
                    await ProjectRequiredAsync(analysis.Id, cancellationToken));
            }

            _db.Entry(result)
                .Property(x => x.RowVersion)
                .OriginalValue = request.RowVersion;

            _db.Entry(analysis)
                .Property(x => x.RowVersion)
                .OriginalValue = request.SupplyAnalysisRowVersion;

            result.ConstituentId = request.ConstituentId;
            result.CharacteristicId = request.CharacteristicId;
            result.AnalyteName = analyteName;
            result.Qualifier = request.Qualifier;
            result.NumericValue = request.NumericValue;
            result.RangeMaximumValue = request.RangeMaximumValue;
            result.TextValue = textValue;
            result.UnitOfMeasureId = request.UnitOfMeasureId;
            result.Basis = request.Basis;
            result.DetectionLimit = request.DetectionLimit;
            result.AnalysisMethod = analysisMethod;
            result.VerificationState = request.VerificationState;
            result.Notes = notes;

            TouchAnalysis(analysis);

            var saveResult = await SaveChangesAsync(
                analysis.Id,
                result.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<SupplyAnalysisResponse>(saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(analysis.Id, cancellationToken));
        }

        /// <summary>
        /// Tar bort ett resultat och concurrency-skyddar både child-raden och analysis-rooten.
        /// </summary>
        public async Task<Result<SupplyAnalysisResponse>> DeleteAsync(Guid supplyAnalysisResultId, uint rowVersion, uint supplyAnalysisRowVersion, CancellationToken cancellationToken = default)
        {
            var result = await _db.Set<SupplyAnalysisResult>()
                .SingleOrDefaultAsync(x => x.Id == supplyAnalysisResultId, cancellationToken);

            if (result is null)
            {
                return Result.Fail<SupplyAnalysisResponse>(
                    new NotFoundError(nameof(SupplyAnalysisResult), supplyAnalysisResultId));
            }

            var analysis = await _db.Set<SupplyAnalysis>()
                .SingleOrDefaultAsync(x => x.Id == result.SupplyAnalysisId, cancellationToken);

            if (analysis is null)
            {
                return Result.Fail<SupplyAnalysisResponse>(
                    new NotFoundError(nameof(SupplyAnalysis), result.SupplyAnalysisId));
            }

            _db.Entry(result)
                .Property(x => x.RowVersion)
                .OriginalValue = rowVersion;

            _db.Entry(analysis)
                .Property(x => x.RowVersion)
                .OriginalValue = supplyAnalysisRowVersion;

            _db.Set<SupplyAnalysisResult>().Remove(result);
            TouchAnalysis(analysis);

            var saveResult = await SaveChangesAsync(
                analysis.Id,
                result.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<SupplyAnalysisResponse>(saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(analysis.Id, cancellationToken));
        }

        /// <summary>
        /// Kontrollerar optional canonical targets och unit-referens.
        /// </summary>
        private async Task<Result> ValidateReferencesAsync(Guid? constituentId, Guid? characteristicId, int? unitOfMeasureId, CancellationToken cancellationToken)
        {
            if (constituentId.HasValue)
            {
                var constituentExists = await _db.Set<Constituent>()
                    .AsNoTracking()
                    .AnyAsync(x => x.Id == constituentId.Value, cancellationToken);

                if (!constituentExists)
                {
                    return Result.Fail(
                        new NotFoundError(nameof(Constituent), constituentId.Value));
                }
            }

            if (characteristicId.HasValue)
            {
                var characteristicExists = await _db.Set<Characteristic>()
                    .AsNoTracking()
                    .AnyAsync(x => x.Id == characteristicId.Value, cancellationToken);

                if (!characteristicExists)
                {
                    return Result.Fail(
                        new NotFoundError(nameof(Characteristic), characteristicId.Value));
                }
            }

            if (unitOfMeasureId.HasValue)
            {
                var unitExists = await _db.Set<UnitOfMeasure>()
                    .AsNoTracking()
                    .AnyAsync(x => x.Id == unitOfMeasureId.Value, cancellationToken);

                if (!unitExists)
                {
                    return Result.Fail(
                        new NotFoundError(nameof(UnitOfMeasure), unitOfMeasureId.Value));
                }
            }

            return Result.Ok();
        }

        /// <summary>
        /// Markerar analysis-rooten så att result-writes konkurrerar med övriga aggregate-writes.
        /// </summary>
        private static void TouchAnalysis(SupplyAnalysis analysis)
        {
            analysis.ActionDateUtc = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Sparar result-ändringen och skiljer child- och aggregate-concurrency åt.
        /// </summary>
        private async Task<Result> SaveChangesAsync(Guid supplyAnalysisId, Guid supplyAnalysisResultId, CancellationToken cancellationToken)
        {
            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                return Result.Ok();
            }
            catch (DbUpdateConcurrencyException exception)
            {
                if (exception.Entries.Any(x => x.Entity is SupplyAnalysisResult))
                {
                    return Result.Fail(
                        new ConcurrencyError(
                            nameof(SupplyAnalysisResult),
                            supplyAnalysisResultId));
                }

                return Result.Fail(
                    new ConcurrencyError(
                        nameof(SupplyAnalysis),
                        supplyAnalysisId));
            }
        }

        /// <summary>
        /// Projekterar analysis-rooten med alla resultat.
        /// </summary>
        private async Task<SupplyAnalysisResponse?> ProjectAsync(Guid supplyAnalysisId, CancellationToken cancellationToken)
        {
            return await _db.Set<SupplyAnalysis>()
                .AsNoTracking()
                .Where(x => x.Id == supplyAnalysisId)
                .Select(SupplyAnalysisProjections.Details)
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Projekterar en sparad analysis och kastar om den oväntat saknas.
        /// </summary>
        private async Task<SupplyAnalysisResponse> ProjectRequiredAsync(Guid supplyAnalysisId, CancellationToken cancellationToken)
        {
            return await ProjectAsync(supplyAnalysisId, cancellationToken)
                ?? throw new InvalidOperationException(
                    $"SupplyAnalysis '{supplyAnalysisId}' was saved but could not be projected.");
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
