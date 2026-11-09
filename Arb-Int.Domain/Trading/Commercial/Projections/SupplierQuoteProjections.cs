using System.Linq.Expressions;

namespace Arb_Int.Domain.Trading.Projections;

/// <summary>
/// Innehåller återanvändbara EF Core-projections för supplier quotes.
/// </summary>
public static class SupplierQuoteProjections
{

    /// <summary>
    /// Projekterar en SupplierQuote till ett kompakt listobjekt.
    /// </summary>
    public static Expression<Func<SupplierQuote, SupplierQuoteListResponse>> List => quote => new SupplierQuoteListResponse
    {
        Id = quote.Id,
        SupplierBusinessRelationshipId = quote.SupplierBusinessRelationshipId,
        SupplierPartyId = quote.SupplierBusinessRelationship.CounterpartyId,
        SupplierName = quote.SupplierBusinessRelationship.Counterparty.Name,
        BuyerRfqId = quote.BuyerRfqRevision == null ? null : quote.BuyerRfqRevision.BuyerRfqId,
        BuyerRfqRevisionId = quote.BuyerRfqRevisionId,
        BuyerRfqNumber = quote.BuyerRfqRevision == null ? null : quote.BuyerRfqRevision.BuyerRfq.RfqNumber,
        QuoteNumber = quote.QuoteNumber,
        Name = quote.Name,
        Status = quote.Status,
        LatestRevisionNumber = quote.LatestRevisionNumber,
        SupplierReference = quote.SupplierReference,
        ReceivedAtUtc = quote.ReceivedAtUtc,
        OwnerUserId = quote.OwnerUserId,
        CreateDateUtc = quote.CreateDateUtc,
        RowVersion = quote.RowVersion
    };

    /// <summary>
    /// Projekterar en supplier-quote-revision med dess lines.
    /// </summary>
    public static Expression<Func<SupplierQuoteRevision, SupplierQuoteRevisionResponse>> RevisionDetails => revision => new SupplierQuoteRevisionResponse
    {
        Id = revision.Id,
        SupplierQuoteId = revision.SupplierQuoteId,
        SupplierBusinessRelationshipId = revision.SupplierBusinessRelationshipId,
        RevisionNumber = revision.RevisionNumber,
        Status = revision.Status,
        CurrencyId = revision.CurrencyId,
        IncotermRuleId = revision.IncotermRuleId,
        NamedPlace = revision.NamedPlace,
        PaymentTerms = revision.PaymentTerms,
        LeadTimeDays = revision.LeadTimeDays,
        ValidFromUtc = revision.ValidFromUtc,
        ValidToUtc = revision.ValidToUtc,
        ReceivedAtUtc = revision.ReceivedAtUtc,
        OriginalDocumentReference = revision.OriginalDocumentReference,
        SnapshotJson = revision.SnapshotJson,
        SnapshotHash = revision.SnapshotHash,
        InternalNotes = revision.InternalNotes,
        RowVersion = revision.RowVersion,
        Lines = revision.Lines
            .OrderBy(x => x.LineNumber)
            .Select(x => new SupplierQuoteLineResponse
            {
                Id = x.Id,
                SupplierQuoteRevisionId = x.SupplierQuoteRevisionId,
                SupplierBusinessRelationshipId = x.SupplierBusinessRelationshipId,
                BuyerRfqLineId = x.BuyerRfqLineId,
                Kind = x.Kind,
                SupplierMaterialOfferingId = x.SupplierMaterialOfferingId,
                SupplierProcessCapabilityId = x.SupplierProcessCapabilityId,
                SupplySolutionStepId = x.SupplySolutionStepId,
                InventoryLotId = x.InventoryLotId,
                LineNumber = x.LineNumber,
                Name = x.Name,
                Description = x.Description,
                OfferedQuantity = x.OfferedQuantity,
                QuantityUnitId = x.QuantityUnitId,
                UnitPrice = x.UnitPrice,
                FixedFee = x.FixedFee,
                MinimumOrderQuantity = x.MinimumOrderQuantity,
                AvailableFromUtc = x.AvailableFromUtc,
                AvailableToUtc = x.AvailableToUtc,
                IncotermRuleId = x.IncotermRuleId,
                NamedPlace = x.NamedPlace,
                PriceBasis = x.PriceBasis,
                SupplierLineReference = x.SupplierLineReference,
                InternalNotes = x.InternalNotes,
                RowVersion = x.RowVersion
            })
            .ToList()
    };

    /// <summary>
    /// Projekterar en supplier quote med hela revisionshistoriken.
    /// </summary>
    public static Expression<Func<SupplierQuote, SupplierQuoteResponse>> Details => quote => new SupplierQuoteResponse
    {
        Id = quote.Id,
        SupplierBusinessRelationshipId = quote.SupplierBusinessRelationshipId,
        BuyerRfqRevisionId = quote.BuyerRfqRevisionId,
        QuoteNumber = quote.QuoteNumber,
        Name = quote.Name,
        Description = quote.Description,
        Status = quote.Status,
        LatestRevisionNumber = quote.LatestRevisionNumber,
        SupplierReference = quote.SupplierReference,
        ReceivedAtUtc = quote.ReceivedAtUtc,
        OwnerUserId = quote.OwnerUserId,
        InternalNotes = quote.InternalNotes,
        RowVersion = quote.RowVersion,
        Revisions = quote.Revisions
            .OrderByDescending(x => x.RevisionNumber)
            .Select(revision => new SupplierQuoteRevisionResponse
            {
                Id = revision.Id,
                SupplierQuoteId = revision.SupplierQuoteId,
                SupplierBusinessRelationshipId = revision.SupplierBusinessRelationshipId,
                RevisionNumber = revision.RevisionNumber,
                Status = revision.Status,
                CurrencyId = revision.CurrencyId,
                IncotermRuleId = revision.IncotermRuleId,
                NamedPlace = revision.NamedPlace,
                PaymentTerms = revision.PaymentTerms,
                LeadTimeDays = revision.LeadTimeDays,
                ValidFromUtc = revision.ValidFromUtc,
                ValidToUtc = revision.ValidToUtc,
                ReceivedAtUtc = revision.ReceivedAtUtc,
                OriginalDocumentReference = revision.OriginalDocumentReference,
                SnapshotJson = revision.SnapshotJson,
                SnapshotHash = revision.SnapshotHash,
                InternalNotes = revision.InternalNotes,
                RowVersion = revision.RowVersion,
                Lines = revision.Lines
                    .OrderBy(x => x.LineNumber)
                    .Select(x => new SupplierQuoteLineResponse
                    {
                        Id = x.Id,
                        SupplierQuoteRevisionId = x.SupplierQuoteRevisionId,
                        SupplierBusinessRelationshipId = x.SupplierBusinessRelationshipId,
                        BuyerRfqLineId = x.BuyerRfqLineId,
                        Kind = x.Kind,
                        SupplierMaterialOfferingId = x.SupplierMaterialOfferingId,
                        SupplierProcessCapabilityId = x.SupplierProcessCapabilityId,
                        SupplySolutionStepId = x.SupplySolutionStepId,
                        InventoryLotId = x.InventoryLotId,
                        LineNumber = x.LineNumber,
                        Name = x.Name,
                        Description = x.Description,
                        OfferedQuantity = x.OfferedQuantity,
                        QuantityUnitId = x.QuantityUnitId,
                        UnitPrice = x.UnitPrice,
                        FixedFee = x.FixedFee,
                        MinimumOrderQuantity = x.MinimumOrderQuantity,
                        AvailableFromUtc = x.AvailableFromUtc,
                        AvailableToUtc = x.AvailableToUtc,
                        IncotermRuleId = x.IncotermRuleId,
                        NamedPlace = x.NamedPlace,
                        PriceBasis = x.PriceBasis,
                        SupplierLineReference = x.SupplierLineReference,
                        InternalNotes = x.InternalNotes,
                        RowVersion = x.RowVersion
                    })
                    .ToList()
            })
            .ToList()
    };
}