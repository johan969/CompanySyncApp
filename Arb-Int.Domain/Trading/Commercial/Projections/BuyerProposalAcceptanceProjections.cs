using System.Linq.Expressions;

namespace Arb_Int.Domain.Trading.Projections;

/// <summary>
/// Innehåller återanvändbara EF Core-projections för buyer acceptances.
/// </summary>
public static class BuyerProposalAcceptanceProjections
{
    /// <summary>
    /// Projekterar en acceptance med samtliga accepterade proposal-lines.
    /// </summary>
    public static Expression<Func<BuyerProposalAcceptance, BuyerProposalAcceptanceResponse>> Details => acceptance => new BuyerProposalAcceptanceResponse
    {
        Id = acceptance.Id,
        BuyerProposalRevisionId = acceptance.BuyerProposalRevisionId,
        BuyerProposalOptionId = acceptance.BuyerProposalOptionId,
        Status = acceptance.Status,
        BuyerAcceptanceReference = acceptance.BuyerAcceptanceReference,
        AcceptedAtUtc = acceptance.AcceptedAtUtc,
        RecordedByUserId = acceptance.RecordedByUserId,
        IsPartialAcceptance = acceptance.IsPartialAcceptance,
        InternallyApprovedAtUtc = acceptance.InternallyApprovedAtUtc,
        InternallyApprovedByUserId = acceptance.InternallyApprovedByUserId,
        InternalApprovalNotes = acceptance.InternalApprovalNotes,
        RowVersion = acceptance.RowVersion,
        Lines = acceptance.Lines
            .OrderBy(x => x.BuyerProposalLine.LineNumber)
            .Select(x => new BuyerProposalAcceptanceLineResponse
            {
                Id = x.Id,
                BuyerProposalAcceptanceId = x.BuyerProposalAcceptanceId,
                BuyerProposalOptionId = x.BuyerProposalOptionId,
                BuyerProposalLineId = x.BuyerProposalLineId,
                AcceptedQuantity = x.AcceptedQuantity,
                QuantityUnitId = x.QuantityUnitId,
                QuantityChanged = x.QuantityChanged,
                BuyerNotes = x.BuyerNotes,
                InternalNotes = x.InternalNotes,
                RowVersion = x.RowVersion
            })
            .ToList()
    };
}