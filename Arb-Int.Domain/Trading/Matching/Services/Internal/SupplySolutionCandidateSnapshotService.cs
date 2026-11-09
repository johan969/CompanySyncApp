using System.Text.Json;
using System.Text.Json.Serialization;
using Arb_Int.Domain.Services.Common;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Trading.Snapshots;

/// <summary>
/// Bygger ett deterministic snapshot av hela kandidatens maskinella resultatgraf.
/// </summary>
internal sealed class SupplySolutionCandidateSnapshotService : ISupplySolutionCandidateSnapshotService
{
    private readonly Arb_IntDbContext _db;
    private readonly ICanonicalJsonService _canonicalJsonService;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        WriteIndented = false,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };

    /// <summary>
    /// Skapar snapshot-servicen.
    /// </summary>
    public SupplySolutionCandidateSnapshotService(Arb_IntDbContext db, ICanonicalJsonService canonicalJsonService)
    {
        _db = db;
        _canonicalJsonService = canonicalJsonService;
    }

    /// <summary>
    /// Bygger canonical snapshot och hash för kandidaten.
    /// </summary>
    public async Task<CanonicalJsonPayload> CreateAsync(Guid supplySolutionCandidateId, CancellationToken cancellationToken = default)
    {
        var candidate = await _db.Set<SupplySolutionCandidate>()
            .AsNoTracking()
            .Include(x => x.Lines)
                .ThenInclude(x => x.MatchingRunLine)
                    .ThenInclude(x => x.BuyerRfqLine)
            .Include(x => x.Lines)
                .ThenInclude(x => x.Allocations)
            .Include(x => x.Lines)
                .ThenInclude(x => x.CapacityEvaluations)
            .Include(x => x.Steps)
            .Include(x => x.RequirementEvaluations)
                .ThenInclude(x => x.BuyerRfqRequirement)
            .Include(x => x.ScoreContributions)
            .AsSplitQuery()
            .SingleOrDefaultAsync(x => x.Id == supplySolutionCandidateId, cancellationToken)
            ?? throw new InvalidOperationException($"SupplySolutionCandidate '{supplySolutionCandidateId}' was not found.");

        var dependencies = await _db.Set<SupplySolutionStepDependency>()
            .AsNoTracking()
            .Where(x => x.SupplySolutionCandidateId == candidate.Id)
            .ToListAsync(cancellationToken);

        var lineNumberBySolutionLineId = candidate.Lines.ToDictionary(
            x => x.Id,
            x => x.MatchingRunLine.BuyerRfqLine.LineNumber);

        var allocationLocationById = candidate.Lines
            .SelectMany(
                line => line.Allocations.Select(
                    allocation => new
                    {
                        allocation.Id,
                        BuyerRfqLineNumber = line.MatchingRunLine.BuyerRfqLine.LineNumber,
                        allocation.AllocationSequence
                    }))
            .ToDictionary(
                x => x.Id,
                x => (x.BuyerRfqLineNumber, x.AllocationSequence));

        var stepSequenceById = candidate.Steps.ToDictionary(
            x => x.Id,
            x => x.SequenceNumber);

        var orderedEvaluations = candidate.RequirementEvaluations
            .OrderBy(x => x.BuyerRfqRequirement.SortOrder)
            .ThenBy(x => x.BuyerRfqRequirementId)
            .ThenBy(x => ResolveBuyerRfqLineNumber(x, lineNumberBySolutionLineId))
            .ThenBy(x => ResolveAllocationSequence(x, allocationLocationById))
            .ThenBy(x => ResolveStepSequence(x, stepSequenceById))
            .ThenBy(x => x.Id)
            .ToList();

        var evaluationNumberById = orderedEvaluations
            .Select((evaluation, index) => new
            {
                evaluation.Id,
                Number = index + 1
            })
            .ToDictionary(x => x.Id, x => x.Number);

        var snapshot = new SupplySolutionCandidateSnapshot
        {
            MatchingRunId = candidate.MatchingRunId,
            CandidateNumber = candidate.CandidateNumber,
            Name = candidate.Name,
            Description = candidate.Description,
            Kind = candidate.Kind,
            Outcome = candidate.Outcome,
            VerificationState = candidate.VerificationState,
            TotalScore = candidate.TotalScore,
            CoveragePercent = candidate.CoveragePercent,
            EstimatedPurchaseCost = candidate.EstimatedPurchaseCost,
            EstimatedLogisticsCost = candidate.EstimatedLogisticsCost,
            EstimatedProcessingCost = candidate.EstimatedProcessingCost,
            EstimatedTotalCost = candidate.EstimatedTotalCost,
            EstimatedCostCurrencyId = candidate.EstimatedCostCurrencyId,
            EstimatedLeadTimeDays = candidate.EstimatedLeadTimeDays,
            ContainsHardFail = candidate.ContainsHardFail,
            ExpiresAtUtc = candidate.ExpiresAtUtc,
            Lines = BuildLines(candidate),
            Steps = BuildSteps(candidate, lineNumberBySolutionLineId),
            StepDependencies = BuildDependencies(dependencies, stepSequenceById),
            RequirementEvaluations = BuildEvaluations(
                orderedEvaluations,
                lineNumberBySolutionLineId,
                allocationLocationById,
                stepSequenceById,
                evaluationNumberById),
            ScoreContributions = BuildScoreContributions(
                candidate,
                evaluationNumberById)
        };

        var json = JsonSerializer.Serialize(snapshot, SerializerOptions);

        return _canonicalJsonService.CanonicalizeAndHash(json);
    }

    /// <summary>
    /// Beräknar hash från canonical representation av ett lagrat snapshot.
    /// </summary>
    public string ComputeHash(string snapshotJson)
    {
        return _canonicalJsonService.ComputeHash(snapshotJson);
    }

    /// <summary>
    /// Bygger snapshotets line-resultat i stabil RFQ-line-ordning.
    /// </summary>
    private static IReadOnlyList<SupplySolutionLineSnapshot> BuildLines(SupplySolutionCandidate candidate)
    {
        return candidate.Lines
            .OrderBy(x => x.MatchingRunLine.BuyerRfqLine.LineNumber)
            .Select(line =>
            {
                var allocationSequenceById = line.Allocations.ToDictionary(
                    x => x.Id,
                    x => x.AllocationSequence);

                return new SupplySolutionLineSnapshot
                {
                    BuyerRfqLineId = line.MatchingRunLine.BuyerRfqLineId,
                    BuyerRfqLineNumber = line.MatchingRunLine.BuyerRfqLine.LineNumber,
                    RequiredQuantity = line.RequiredQuantity,
                    CoveredQuantity = line.CoveredQuantity,
                    ShortageQuantity = line.ShortageQuantity,
                    QuantityUnitId = line.QuantityUnitId,
                    IsFullyCovered = line.IsFullyCovered,
                    CoveragePercent = line.CoveragePercent,
                    Notes = line.Notes,
                    Allocations = line.Allocations
                        .OrderBy(x => x.AllocationSequence)
                        .Select(x => new SupplyAllocationSnapshot
                        {
                            AllocationSequence = x.AllocationSequence,
                            SupplierBusinessRelationshipId = x.SupplierBusinessRelationshipId,
                            SupplierMaterialOfferingId = x.SupplierMaterialOfferingId,
                            SupplierOfferingAvailabilityId = x.SupplierOfferingAvailabilityId,
                            InventoryLotId = x.InventoryLotId,
                            SupplierQuoteLineId = x.SupplierQuoteLineId,
                            Status = x.Status,
                            AllocatedQuantity = x.AllocatedQuantity,
                            QuantityUnitId = x.QuantityUnitId,
                            UnitPurchasePrice = x.UnitPurchasePrice,
                            PurchasePriceCurrencyId = x.PurchasePriceCurrencyId,
                            EstimatedAdditionalCost = x.EstimatedAdditionalCost,
                            AvailabilityVerificationState = x.AvailabilityVerificationState,
                            QualityVerificationState = x.QualityVerificationState,
                            Notes = x.Notes
                        })
                        .ToList(),
                    CapacityEvaluations = line.CapacityEvaluations
                        .OrderBy(x => x.SupplyAllocationId.HasValue ? 1 : 0)
                        .ThenBy(x => x.SupplyAllocationId.HasValue ? allocationSequenceById[x.SupplyAllocationId.Value] : int.MinValue)
                        .ThenBy(x => x.Outcome)
                        .ThenBy(x => x.Id)
                        .Select(x => new CapacityEvaluationSnapshot
                        {
                            AllocationSequence = x.SupplyAllocationId.HasValue ? allocationSequenceById[x.SupplyAllocationId.Value] : null,
                            Outcome = x.Outcome,
                            RequestedQuantity = x.RequestedQuantity,
                            AvailableQuantity = x.AvailableQuantity,
                            AllocatedQuantity = x.AllocatedQuantity,
                            QuantityUnitId = x.QuantityUnitId,
                            AvailabilityFromUtc = x.AvailabilityFromUtc,
                            AvailabilityToUtc = x.AvailabilityToUtc,
                            Explanation = x.Explanation
                        })
                        .ToList()
                };
            })
            .ToList();
    }

    /// <summary>
    /// Bygger kandidatens steps i stabil sekvensordning.
    /// </summary>
    private static IReadOnlyList<SupplySolutionStepSnapshot> BuildSteps(
        SupplySolutionCandidate candidate,
        IReadOnlyDictionary<Guid, int> lineNumberBySolutionLineId)
    {
        return candidate.Steps
            .OrderBy(x => x.SequenceNumber)
            .Select(x => new SupplySolutionStepSnapshot
            {
                SequenceNumber = x.SequenceNumber,
                BuyerRfqLineNumber = x.SupplySolutionLineId.HasValue
                    ? lineNumberBySolutionLineId[x.SupplySolutionLineId.Value]
                    : null,
                Type = x.Type,
                Status = x.Status,
                Name = x.Name,
                Description = x.Description,
                ProviderBusinessRelationshipId = x.ProviderBusinessRelationshipId,
                SupplierProcessCapabilityId = x.SupplierProcessCapabilityId,
                ProcessTypeId = x.ProcessTypeId,
                InputMaterialId = x.InputMaterialId,
                OutputMaterialId = x.OutputMaterialId,
                ExpectedInputQuantity = x.ExpectedInputQuantity,
                InputQuantityUnitId = x.InputQuantityUnitId,
                ExpectedOutputQuantity = x.ExpectedOutputQuantity,
                OutputQuantityUnitId = x.OutputQuantityUnitId,
                ExpectedYieldPercent = x.ExpectedYieldPercent,
                EstimatedLeadTimeDays = x.EstimatedLeadTimeDays,
                FromLocationId = x.FromLocationId,
                ToLocationId = x.ToLocationId,
                PlannedStartUtc = x.PlannedStartUtc,
                PlannedEndUtc = x.PlannedEndUtc,
                Parameters = ParseOptionalJson(x.ParametersJson)
            })
            .ToList();
    }

    /// <summary>
    /// Bygger step-grafens edges med sekvensnummer i stället för interna dependency-ID:n.
    /// </summary>
    private static IReadOnlyList<SupplySolutionStepDependencySnapshot> BuildDependencies(
        IEnumerable<SupplySolutionStepDependency> dependencies,
        IReadOnlyDictionary<Guid, int> stepSequenceById)
    {
        return dependencies
            .OrderBy(x => stepSequenceById[x.PredecessorStepId])
            .ThenBy(x => stepSequenceById[x.SuccessorStepId])
            .Select(x => new SupplySolutionStepDependencySnapshot
            {
                PredecessorSequenceNumber = stepSequenceById[x.PredecessorStepId],
                SuccessorSequenceNumber = stepSequenceById[x.SuccessorStepId],
                Notes = x.Notes
            })
            .ToList();
    }

    /// <summary>
    /// Bygger requirement evaluations med lokala deterministiska evaluation-nummer.
    /// </summary>
    private static IReadOnlyList<RequirementEvaluationSnapshot> BuildEvaluations(
        IReadOnlyList<RequirementEvaluation> evaluations,
        IReadOnlyDictionary<Guid, int> lineNumberBySolutionLineId,
        IReadOnlyDictionary<Guid, (int BuyerRfqLineNumber, int AllocationSequence)> allocationLocationById,
        IReadOnlyDictionary<Guid, int> stepSequenceById,
        IReadOnlyDictionary<Guid, int> evaluationNumberById)
    {
        return evaluations
            .Select(x => new RequirementEvaluationSnapshot
            {
                EvaluationNumber = evaluationNumberById[x.Id],
                BuyerRfqRequirementId = x.BuyerRfqRequirementId,
                BuyerRfqRequirementName = x.BuyerRfqRequirement.Name,
                RequirementLevel = x.BuyerRfqRequirement.Level,
                BuyerRfqLineNumber = ResolveBuyerRfqLineNumber(x, lineNumberBySolutionLineId),
                AllocationSequence = ResolveAllocationSequence(x, allocationLocationById),
                SupplySolutionStepSequenceNumber = ResolveStepSequence(x, stepSequenceById),
                Outcome = x.Outcome,
                IsHardFail = x.IsHardFail,
                VerificationState = x.VerificationState,
                ActualNumericValue = x.ActualNumericValue,
                ActualTextValue = x.ActualTextValue,
                UnitOfMeasureId = x.UnitOfMeasureId,
                Explanation = x.Explanation,
                Evidence = ParseOptionalJson(x.EvidenceSnapshotJson),
                RuleTrace = ParseOptionalJson(x.RuleTraceJson)
            })
            .ToList();
    }

    /// <summary>
    /// Bygger score contributions och ersätter evaluation-ID med lokalt evaluation-nummer.
    /// </summary>
    private static IReadOnlyList<ScoreContributionSnapshot> BuildScoreContributions(
        SupplySolutionCandidate candidate,
        IReadOnlyDictionary<Guid, int> evaluationNumberById)
    {
        return candidate.ScoreContributions
            .OrderBy(x => x.Category)
            .ThenBy(x => x.RequirementEvaluationId.HasValue ? evaluationNumberById[x.RequirementEvaluationId.Value] : int.MinValue)
            .ThenBy(x => x.Points)
            .ThenBy(x => x.Id)
            .Select(x => new ScoreContributionSnapshot
            {
                RequirementEvaluationNumber = x.RequirementEvaluationId.HasValue
                    ? evaluationNumberById[x.RequirementEvaluationId.Value]
                    : null,
                Category = x.Category,
                Weight = x.Weight,
                NormalizedScore = x.NormalizedScore,
                Points = x.Points,
                Explanation = x.Explanation
            })
            .ToList();
    }

    /// <summary>
    /// Härleder RFQ-line-nummer från evaluationens solution line.
    /// </summary>
    private static int? ResolveBuyerRfqLineNumber(
        RequirementEvaluation evaluation,
        IReadOnlyDictionary<Guid, int> lineNumberBySolutionLineId)
    {
        return evaluation.SupplySolutionLineId.HasValue
            ? lineNumberBySolutionLineId[evaluation.SupplySolutionLineId.Value]
            : null;
    }

    /// <summary>
    /// Härleder allocationens sekvensnummer från evaluationens allocation.
    /// </summary>
    private static int? ResolveAllocationSequence(
        RequirementEvaluation evaluation,
        IReadOnlyDictionary<Guid, (int BuyerRfqLineNumber, int AllocationSequence)> allocationLocationById)
    {
        return evaluation.SupplyAllocationId.HasValue
            ? allocationLocationById[evaluation.SupplyAllocationId.Value].AllocationSequence
            : null;
    }

    /// <summary>
    /// Härleder step-sekvens från evaluationens solution step.
    /// </summary>
    private static int? ResolveStepSequence(
        RequirementEvaluation evaluation,
        IReadOnlyDictionary<Guid, int> stepSequenceById)
    {
        return evaluation.SupplySolutionStepId.HasValue
            ? stepSequenceById[evaluation.SupplySolutionStepId.Value]
            : null;
    }

    /// <summary>
    /// Parsar optional JSON till ett fristående JsonElement.
    /// </summary>
    private static JsonElement? ParseOptionalJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        using var document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }
}