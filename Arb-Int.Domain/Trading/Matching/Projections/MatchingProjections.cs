using System.Linq.Expressions;

namespace Arb_Int.Domain.Trading.Projections;

/// <summary>
/// Innehåller återanvändbara EF-projections för matching-orchestration.
/// </summary>
public static class MatchingProjections
{
    /// <summary>
    /// Projekterar ett regelverk och dess versionshistorik.
    /// </summary>
    public static Expression<Func<MatchingRuleSet, MatchingRuleSetResponse>> RuleSetDetails => ruleSet => new MatchingRuleSetResponse
    {
        Id = ruleSet.Id,
        Code = ruleSet.Code,
        Name = ruleSet.Name,
        Description = ruleSet.Description,
        Status = ruleSet.Status,
        LatestVersionNumber = ruleSet.LatestVersionNumber,
        RowVersion = ruleSet.RowVersion,
        Versions = ruleSet.Versions
            .OrderByDescending(x => x.VersionNumber)
            .Select(x => new MatchingRuleSetVersionResponse
            {
                Id = x.Id,
                MatchingRuleSetId = x.MatchingRuleSetId,
                VersionNumber = x.VersionNumber,
                Description = x.Description,
                ConfigurationJson = x.ConfigurationJson,
                ConfigurationHash = x.ConfigurationHash,
                EffectiveFromUtc = x.EffectiveFromUtc,
                EffectiveToUtc = x.EffectiveToUtc,
                PublishedAtUtc = x.PublishedAtUtc,
                PublishedByUserId = x.PublishedByUserId,
                MatchingRunCount = x.MatchingRuns.Count,
                RowVersion = x.RowVersion
            })
            .ToList()
    };

    /// <summary>
    /// Projekterar en matchningskörning med dess frysta run-lines.
    /// </summary>
    public static Expression<Func<MatchingRun, MatchingRunResponse>> RunDetails => run => new MatchingRunResponse
    {
        Id = run.Id,
        BuyerRfqId = run.BuyerRfqRevision.BuyerRfqId,
        BuyerRfqRevisionId = run.BuyerRfqRevisionId,
        BuyerRfqRevisionNumber = run.BuyerRfqRevision.RevisionNumber,
        MatchingRuleSetVersionId = run.MatchingRuleSetVersionId,
        MatchingRuleSetId = run.MatchingRuleSetVersion.MatchingRuleSetId,
        MatchingRuleSetCode = run.MatchingRuleSetVersion.MatchingRuleSet.Code,
        MatchingRuleSetVersionNumber = run.MatchingRuleSetVersion.VersionNumber,
        Status = run.Status,
        QueuedAtUtc = run.QueuedAtUtc,
        StartedAtUtc = run.StartedAtUtc,
        CompletedAtUtc = run.CompletedAtUtc,
        StartedByUserId = run.StartedByUserId,
        TriggerName = run.TriggerName,
        CorrelationId = run.CorrelationId,
        InputSnapshotJson = run.InputSnapshotJson,
        InputSnapshotHash = run.InputSnapshotHash,
        ErrorMessage = run.ErrorMessage,
        CandidateCount = run.Candidates.Count,
        RowVersion = run.RowVersion,
        Lines = run.Lines
            .OrderBy(x => x.BuyerRfqLine.LineNumber)
            .Select(x => new MatchingRunLineResponse
            {
                Id = x.Id,
                MatchingRunId = x.MatchingRunId,
                BuyerRfqRevisionId = x.BuyerRfqRevisionId,
                BuyerRfqLineId = x.BuyerRfqLineId,
                BuyerRfqLineNumber = x.BuyerRfqLine.LineNumber,
                BuyerRfqLineName = x.BuyerRfqLine.Name,
                RequiredQuantity = x.RequiredQuantity,
                QuantityUnitId = x.QuantityUnitId,
                InputSnapshotJson = x.InputSnapshotJson,
                RowVersion = x.RowVersion
            })
            .ToList()
    };
}