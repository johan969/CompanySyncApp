using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Trading.Projections;

/// <summary>
/// Innehåller EF Core-projections för supply-solution-candidates och deras första resultatlager.
/// </summary>
public static class SupplySolutionCandidateProjections
{
    /// <summary>
    /// Projekterar en kandidat till ett kompakt listobjekt.
    /// </summary>
    public static Expression<Func<SupplySolutionCandidate, SupplySolutionCandidateListResponse>> List => candidate => new SupplySolutionCandidateListResponse
    {
        Id = candidate.Id,
        MatchingRunId = candidate.MatchingRunId,
        CandidateNumber = candidate.CandidateNumber,
        Name = candidate.Name,
        Kind = candidate.Kind,
        Outcome = candidate.Outcome,
        Status = candidate.Status,
        VerificationState = candidate.VerificationState,
        TotalScore = candidate.TotalScore,
        CoveragePercent = candidate.CoveragePercent,
        EstimatedTotalCost = candidate.EstimatedTotalCost,
        EstimatedCostCurrencyId = candidate.EstimatedCostCurrencyId,
        EstimatedLeadTimeDays = candidate.EstimatedLeadTimeDays,
        ContainsHardFail = candidate.ContainsHardFail,
        LineCount = candidate.Lines.Count,
        ExpiresAtUtc = candidate.ExpiresAtUtc,
        RowVersion = candidate.RowVersion
    };

    /// <summary>
    /// Projekterar en kandidat med lines, allocations och capacity evaluations.
    /// </summary>
    public static Expression<Func<SupplySolutionCandidate, SupplySolutionCandidateResponse>> Details => candidate => new SupplySolutionCandidateResponse
    {
        Id = candidate.Id,
        MatchingRunId = candidate.MatchingRunId,
        MatchingRunStatus = candidate.MatchingRun.Status,
        CandidateNumber = candidate.CandidateNumber,
        Name = candidate.Name,
        Description = candidate.Description,
        Kind = candidate.Kind,
        Outcome = candidate.Outcome,
        Status = candidate.Status,
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
        SnapshotJson = candidate.SnapshotJson,
        SnapshotHash = candidate.SnapshotHash,
        ExpiresAtUtc = candidate.ExpiresAtUtc,
        Steps = candidate.Steps
            .OrderBy(x => x.SequenceNumber)
            .ThenBy(x => x.Id)
            .Select(x => new SupplySolutionStepResponse
            {
                Id = x.Id,
                SupplySolutionCandidateId = x.SupplySolutionCandidateId,
                SupplySolutionLineId = x.SupplySolutionLineId,
                SequenceNumber = x.SequenceNumber,
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
                ParametersJson = x.ParametersJson,
                RowVersion = x.RowVersion
            })
            .ToList(),

        StepDependencies = candidate.Steps
            .SelectMany(x => x.SuccessorDependencies)
            .OrderBy(x => x.PredecessorStep.SequenceNumber)
            .ThenBy(x => x.SuccessorStep.SequenceNumber)
            .ThenBy(x => x.Id)
            .Select(x => new SupplySolutionStepDependencyResponse
            {
                Id = x.Id,
                SupplySolutionCandidateId = x.SupplySolutionCandidateId,
                PredecessorStepId = x.PredecessorStepId,
                PredecessorSequenceNumber = x.PredecessorStep.SequenceNumber,
                SuccessorStepId = x.SuccessorStepId,
                SuccessorSequenceNumber = x.SuccessorStep.SequenceNumber,
                Notes = x.Notes,
                RowVersion = x.RowVersion
            })
            .ToList(),
        RequirementEvaluationCount = candidate.RequirementEvaluations.Count,
        ScoreContributionCount = candidate.ScoreContributions.Count,
        ModerationDecisionCount = candidate.ModerationDecisions.Count,
        RowVersion = candidate.RowVersion,
        RequirementEvaluations = candidate.RequirementEvaluations
            .OrderBy(x => x.BuyerRfqRequirement.SortOrder)
            .ThenBy(x => x.BuyerRfqRequirement.Name)
            .ThenBy(x => x.Id)
            .Select(x => new RequirementEvaluationResponse
            {
                Id = x.Id,
                SupplySolutionCandidateId = x.SupplySolutionCandidateId,
                BuyerRfqRequirementId = x.BuyerRfqRequirementId,
                BuyerRfqRequirementName = x.BuyerRfqRequirement.Name,
                BuyerRfqRequirementType = EF.Property<string>(x.BuyerRfqRequirement, "RequirementType"),
                RequirementLevel = x.BuyerRfqRequirement.Level,
                SupplySolutionLineId = x.SupplySolutionLineId,
                SupplyAllocationId = x.SupplyAllocationId,
                SupplySolutionStepId = x.SupplySolutionStepId,
                Outcome = x.Outcome,
                IsHardFail = x.IsHardFail,
                VerificationState = x.VerificationState,
                ActualNumericValue = x.ActualNumericValue,
                ActualTextValue = x.ActualTextValue,
                UnitOfMeasureId = x.UnitOfMeasureId,
                Explanation = x.Explanation,
                EvidenceSnapshotJson = x.EvidenceSnapshotJson,
                RuleTraceJson = x.RuleTraceJson,
                RowVersion = x.RowVersion
            })
            .ToList(),
        ScoreContributions = candidate.ScoreContributions
            .OrderBy(x => x.Category)
            .ThenBy(x => x.Id)
            .Select(x => new ScoreContributionResponse
            {
                Id = x.Id,
                SupplySolutionCandidateId = x.SupplySolutionCandidateId,
                RequirementEvaluationId = x.RequirementEvaluationId,
                Category = x.Category,
                Weight = x.Weight,
                NormalizedScore = x.NormalizedScore,
                Points = x.Points,
                Explanation = x.Explanation,
                RowVersion = x.RowVersion
            })
            .ToList(),
        EffectiveOutcome = candidate.ModerationDecisions
            .OrderByDescending(x => x.DecidedAtUtc)
            .ThenByDescending(x => x.Id)
            .Select(x => x.IsManualOverride
                ? x.OverriddenOutcome
                : null)
            .FirstOrDefault() ?? candidate.Outcome,

        EffectiveScore = candidate.ModerationDecisions
            .OrderByDescending(x => x.DecidedAtUtc)
            .ThenByDescending(x => x.Id)
            .Select(x => x.IsManualOverride
                ? x.OverriddenScore
                : null)
            .FirstOrDefault() ?? candidate.TotalScore,

        ModerationDecisions = candidate.ModerationDecisions
            .OrderByDescending(x => x.DecidedAtUtc)
            .ThenByDescending(x => x.Id)
            .Select(x => new MatchingModerationDecisionResponse
            {
                Id = x.Id,
                SupplySolutionCandidateId = x.SupplySolutionCandidateId,
                Decision = x.Decision,
                DecidedByUserId = x.DecidedByUserId,
                DecidedAtUtc = x.DecidedAtUtc,
                IsManualOverride = x.IsManualOverride,
                OverriddenOutcome = x.OverriddenOutcome,
                OverriddenScore = x.OverriddenScore,
                EffectiveOutcome = x.IsManualOverride && x.OverriddenOutcome.HasValue
                    ? x.OverriddenOutcome.Value
                    : candidate.Outcome,
                EffectiveScore = x.IsManualOverride && x.OverriddenScore.HasValue
                    ? x.OverriddenScore.Value
                    : candidate.TotalScore,
                Reason = x.Reason,
                Notes = x.Notes,
                RowVersion = x.RowVersion
            })
            .ToList(),
        Lines = candidate.Lines
            .OrderBy(x => x.MatchingRunLine.BuyerRfqLine.LineNumber)
            .Select(line => new SupplySolutionLineResponse
            {
                Id = line.Id,
                SupplySolutionCandidateId = line.SupplySolutionCandidateId,
                MatchingRunId = line.MatchingRunId,
                MatchingRunLineId = line.MatchingRunLineId,
                BuyerRfqLineId = line.MatchingRunLine.BuyerRfqLineId,
                BuyerRfqLineNumber = line.MatchingRunLine.BuyerRfqLine.LineNumber,
                RequiredQuantity = line.RequiredQuantity,
                CoveredQuantity = line.CoveredQuantity,
                ShortageQuantity = line.ShortageQuantity,
                QuantityUnitId = line.QuantityUnitId,
                IsFullyCovered = line.IsFullyCovered,
                CoveragePercent = line.CoveragePercent,
                Notes = line.Notes,
                RowVersion = line.RowVersion,
                Allocations = line.Allocations
                    .OrderBy(x => x.AllocationSequence)
                    .ThenBy(x => x.Id)
                    .Select(allocation => new SupplyAllocationResponse
                    {
                        Id = allocation.Id,
                        SupplySolutionLineId = allocation.SupplySolutionLineId,
                        SupplierBusinessRelationshipId = allocation.SupplierBusinessRelationshipId,
                        SupplierMaterialOfferingId = allocation.SupplierMaterialOfferingId,
                        SupplierOfferingAvailabilityId = allocation.SupplierOfferingAvailabilityId,
                        InventoryLotId = allocation.InventoryLotId,
                        SupplierQuoteLineId = allocation.SupplierQuoteLineId,
                        Status = allocation.Status,
                        AllocationSequence = allocation.AllocationSequence,
                        AllocatedQuantity = allocation.AllocatedQuantity,
                        QuantityUnitId = allocation.QuantityUnitId,
                        UnitPurchasePrice = allocation.UnitPurchasePrice,
                        PurchasePriceCurrencyId = allocation.PurchasePriceCurrencyId,
                        EstimatedAdditionalCost = allocation.EstimatedAdditionalCost,
                        AvailabilityVerificationState = allocation.AvailabilityVerificationState,
                        QualityVerificationState = allocation.QualityVerificationState,
                        Notes = allocation.Notes,
                        RowVersion = allocation.RowVersion
                    })
                    .ToList(),
                CapacityEvaluations = line.CapacityEvaluations
                    .OrderBy(x => x.SupplyAllocationId)
                    .ThenBy(x => x.Id)
                    .Select(evaluation => new CapacityEvaluationResponse
                    {
                        Id = evaluation.Id,
                        SupplySolutionLineId = evaluation.SupplySolutionLineId,
                        SupplyAllocationId = evaluation.SupplyAllocationId,
                        Outcome = evaluation.Outcome,
                        RequestedQuantity = evaluation.RequestedQuantity,
                        AvailableQuantity = evaluation.AvailableQuantity,
                        AllocatedQuantity = evaluation.AllocatedQuantity,
                        QuantityUnitId = evaluation.QuantityUnitId,
                        AvailabilityFromUtc = evaluation.AvailabilityFromUtc,
                        AvailabilityToUtc = evaluation.AvailabilityToUtc,
                        Explanation = evaluation.Explanation,
                        RowVersion = evaluation.RowVersion
                    })
                    .ToList()
            })
            .ToList()
    };
}