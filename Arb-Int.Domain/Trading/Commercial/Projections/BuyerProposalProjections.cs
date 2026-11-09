using System.Linq.Expressions;

namespace Arb_Int.Domain.Trading.Projections;

/// <summary>
/// Innehåller återanvändbara EF Core-projections för buyer proposals.
/// </summary>
public static class BuyerProposalProjections
{

    /// <summary>
    /// Projekterar ett BuyerProposal till ett kompakt listobjekt.
    /// </summary>
    public static Expression<Func<BuyerProposal, BuyerProposalListResponse>> List => proposal => new BuyerProposalListResponse
    {
        Id = proposal.Id,
        BuyerBusinessRelationshipId = proposal.BuyerBusinessRelationshipId,
        BuyerPartyId = proposal.BuyerBusinessRelationship.CounterpartyId,
        BuyerName = proposal.BuyerBusinessRelationship.Counterparty.Name,
        BuyerRfqId = proposal.BuyerRfqId,
        BuyerRfqNumber = proposal.BuyerRfq.RfqNumber,
        ProposalNumber = proposal.ProposalNumber,
        Name = proposal.Name,
        Status = proposal.Status,
        LatestRevisionNumber = proposal.LatestRevisionNumber,
        OwnerUserId = proposal.OwnerUserId,
        IssuedAtUtc = proposal.IssuedAtUtc,
        ClosedAtUtc = proposal.ClosedAtUtc,
        CreateDateUtc = proposal.CreateDateUtc,
        RowVersion = proposal.RowVersion
    };

    /// <summary>
    /// Projekterar en proposal-revision med options och lines.
    /// </summary>
    public static Expression<Func<BuyerProposalRevision, BuyerProposalRevisionResponse>> RevisionDetails => revision => new BuyerProposalRevisionResponse
    {
        Id = revision.Id,
        BuyerProposalId = revision.BuyerProposalId,
        BuyerRfqId = revision.BuyerRfqId,
        BuyerRfqRevisionId = revision.BuyerRfqRevisionId,
        RevisionNumber = revision.RevisionNumber,
        Status = revision.Status,
        CurrencyId = revision.CurrencyId,
        ValidFromUtc = revision.ValidFromUtc,
        ValidToUtc = revision.ValidToUtc,
        CommercialTerms = revision.CommercialTerms,
        BuyerFacingNotes = revision.BuyerFacingNotes,
        InternalNotes = revision.InternalNotes,
        DisclosurePackageRevisionId = revision.DisclosurePackageRevisionId,
        IssuedAtUtc = revision.IssuedAtUtc,
        IssuedByUserId = revision.IssuedByUserId,
        SnapshotJson = revision.SnapshotJson,
        SnapshotHash = revision.SnapshotHash,
        RowVersion = revision.RowVersion,
        Acceptances = revision.Acceptances
            .OrderByDescending(x => x.AcceptedAtUtc)
            .ThenByDescending(x => x.Id)
            .Select(acceptance => new BuyerProposalAcceptanceResponse
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
                    .Select(line => new BuyerProposalAcceptanceLineResponse
                    {
                        Id = line.Id,
                        BuyerProposalAcceptanceId = line.BuyerProposalAcceptanceId,
                        BuyerProposalOptionId = line.BuyerProposalOptionId,
                        BuyerProposalLineId = line.BuyerProposalLineId,
                        AcceptedQuantity = line.AcceptedQuantity,
                        QuantityUnitId = line.QuantityUnitId,
                        QuantityChanged = line.QuantityChanged,
                        BuyerNotes = line.BuyerNotes,
                        InternalNotes = line.InternalNotes,
                        RowVersion = line.RowVersion
                    })
                    .ToList()
            })
            .ToList(),
        Options = revision.Options
            .OrderBy(x => x.OptionNumber)
            .Select(option => new BuyerProposalOptionResponse
            {
                Id = option.Id,
                BuyerProposalRevisionId = option.BuyerProposalRevisionId,
                SupplySolutionCandidateId = option.SupplySolutionCandidateId,
                OptionNumber = option.OptionNumber,
                Name = option.Name,
                Description = option.Description,
                Status = option.Status,
                TotalNetAmount = option.TotalNetAmount,
                TotalTaxAmount = option.TotalTaxAmount,
                TotalGrossAmount = option.TotalGrossAmount,
                CurrencyId = option.CurrencyId,
                EstimatedLeadTimeDays = option.EstimatedLeadTimeDays,
                BuyerFacingSummary = option.BuyerFacingSummary,
                InternalMarginSnapshotJson = option.InternalMarginSnapshotJson,
                RowVersion = option.RowVersion,
                Lines = option.Lines
                    .OrderBy(x => x.LineNumber)
                    .Select(line => new BuyerProposalLineResponse
                    {
                        Id = line.Id,
                        BuyerProposalOptionId = line.BuyerProposalOptionId,
                        BuyerRfqLineId = line.BuyerRfqLineId,
                        SupplySolutionLineId = line.SupplySolutionLineId,
                        MaterialId = line.MaterialId,
                        ProductId = line.ProductId,
                        LineNumber = line.LineNumber,
                        Name = line.Name,
                        Description = line.Description,
                        Quantity = line.Quantity,
                        QuantityUnitId = line.QuantityUnitId,
                        UnitPrice = line.UnitPrice,
                        NetAmount = line.NetAmount,
                        TaxPercent = line.TaxPercent,
                        TaxAmount = line.TaxAmount,
                        GrossAmount = line.GrossAmount,
                        EstimatedDeliveryFromUtc = line.EstimatedDeliveryFromUtc,
                        EstimatedDeliveryToUtc = line.EstimatedDeliveryToUtc,
                        BuyerFacingNotes = line.BuyerFacingNotes,
                        RowVersion = line.RowVersion
                    })
                    .ToList()
            })
            .ToList()
    };

    /// <summary>
    /// Projekterar proposal-roten med samtliga revisioner.
    /// </summary>
    public static Expression<Func<BuyerProposal, BuyerProposalResponse>> Details => proposal => new BuyerProposalResponse
    {
        Id = proposal.Id,
        BuyerBusinessRelationshipId = proposal.BuyerBusinessRelationshipId,
        BuyerRfqId = proposal.BuyerRfqId,
        ProposalNumber = proposal.ProposalNumber,
        Name = proposal.Name,
        Description = proposal.Description,
        Status = proposal.Status,
        LatestRevisionNumber = proposal.LatestRevisionNumber,
        OwnerUserId = proposal.OwnerUserId,
        IssuedAtUtc = proposal.IssuedAtUtc,
        ClosedAtUtc = proposal.ClosedAtUtc,
        InternalNotes = proposal.InternalNotes,
        RowVersion = proposal.RowVersion,
        Revisions = proposal.Revisions
            .OrderByDescending(x => x.RevisionNumber)
            .Select(revision => new BuyerProposalRevisionResponse
            {
                Id = revision.Id,
                BuyerProposalId = revision.BuyerProposalId,
                BuyerRfqId = revision.BuyerRfqId,
                BuyerRfqRevisionId = revision.BuyerRfqRevisionId,
                RevisionNumber = revision.RevisionNumber,
                Status = revision.Status,
                CurrencyId = revision.CurrencyId,
                ValidFromUtc = revision.ValidFromUtc,
                ValidToUtc = revision.ValidToUtc,
                CommercialTerms = revision.CommercialTerms,
                BuyerFacingNotes = revision.BuyerFacingNotes,
                InternalNotes = revision.InternalNotes,
                DisclosurePackageRevisionId = revision.DisclosurePackageRevisionId,
                IssuedAtUtc = revision.IssuedAtUtc,
                IssuedByUserId = revision.IssuedByUserId,
                SnapshotJson = revision.SnapshotJson,
                SnapshotHash = revision.SnapshotHash,
                RowVersion = revision.RowVersion,
                Acceptances = revision.Acceptances
                    .OrderByDescending(x => x.AcceptedAtUtc)
                    .ThenByDescending(x => x.Id)
                    .Select(acceptance => new BuyerProposalAcceptanceResponse
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
                            .Select(line => new BuyerProposalAcceptanceLineResponse
                            {
                                Id = line.Id,
                                BuyerProposalAcceptanceId = line.BuyerProposalAcceptanceId,
                                BuyerProposalOptionId = line.BuyerProposalOptionId,
                                BuyerProposalLineId = line.BuyerProposalLineId,
                                AcceptedQuantity = line.AcceptedQuantity,
                                QuantityUnitId = line.QuantityUnitId,
                                QuantityChanged = line.QuantityChanged,
                                BuyerNotes = line.BuyerNotes,
                                InternalNotes = line.InternalNotes,
                                RowVersion = line.RowVersion
                            })
                            .ToList()
                    })
                    .ToList(),
                Options = revision.Options
                    .OrderBy(x => x.OptionNumber)
                    .Select(option => new BuyerProposalOptionResponse
                    {
                        Id = option.Id,
                        BuyerProposalRevisionId = option.BuyerProposalRevisionId,
                        SupplySolutionCandidateId = option.SupplySolutionCandidateId,
                        OptionNumber = option.OptionNumber,
                        Name = option.Name,
                        Description = option.Description,
                        Status = option.Status,
                        TotalNetAmount = option.TotalNetAmount,
                        TotalTaxAmount = option.TotalTaxAmount,
                        TotalGrossAmount = option.TotalGrossAmount,
                        CurrencyId = option.CurrencyId,
                        EstimatedLeadTimeDays = option.EstimatedLeadTimeDays,
                        BuyerFacingSummary = option.BuyerFacingSummary,
                        InternalMarginSnapshotJson = option.InternalMarginSnapshotJson,
                        RowVersion = option.RowVersion,
                        Lines = option.Lines
                            .OrderBy(x => x.LineNumber)
                            .Select(line => new BuyerProposalLineResponse
                            {
                                Id = line.Id,
                                BuyerProposalOptionId = line.BuyerProposalOptionId,
                                BuyerRfqLineId = line.BuyerRfqLineId,
                                SupplySolutionLineId = line.SupplySolutionLineId,
                                MaterialId = line.MaterialId,
                                ProductId = line.ProductId,
                                LineNumber = line.LineNumber,
                                Name = line.Name,
                                Description = line.Description,
                                Quantity = line.Quantity,
                                QuantityUnitId = line.QuantityUnitId,
                                UnitPrice = line.UnitPrice,
                                NetAmount = line.NetAmount,
                                TaxPercent = line.TaxPercent,
                                TaxAmount = line.TaxAmount,
                                GrossAmount = line.GrossAmount,
                                EstimatedDeliveryFromUtc = line.EstimatedDeliveryFromUtc,
                                EstimatedDeliveryToUtc = line.EstimatedDeliveryToUtc,
                                BuyerFacingNotes = line.BuyerFacingNotes,
                                RowVersion = line.RowVersion
                            })
                            .ToList()
                    })
                    .ToList()
            })
            .ToList()
    };
}