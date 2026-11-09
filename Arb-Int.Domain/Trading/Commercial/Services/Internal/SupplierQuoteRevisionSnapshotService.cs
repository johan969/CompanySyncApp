using System.Text.Json;
using System.Text.Json.Serialization;
using Arb_Int.Domain.Services.Common;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Trading.Snapshots;

/// <summary>
/// Skapar deterministic snapshots av supplier-quote-revisioner.
/// </summary>
internal sealed class SupplierQuoteRevisionSnapshotService : ISupplierQuoteRevisionSnapshotService
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
    /// Skapar snapshot-servicen med databas- och canonical-JSON-beroenden.
    /// </summary>
    public SupplierQuoteRevisionSnapshotService(Arb_IntDbContext db, ICanonicalJsonService canonicalJsonService)
    {
        _db = db;
        _canonicalJsonService = canonicalJsonService;
    }

    /// <summary>
    /// Bygger snapshotet från quote-roten, revisionen och dess lines.
    /// </summary>
    public async Task<CanonicalJsonPayload> CreateAsync(Guid supplierQuoteRevisionId, CancellationToken cancellationToken = default)
    {
        var revision = await _db.Set<SupplierQuoteRevision>()
            .AsNoTracking()
            .Include(x => x.SupplierQuote)
            .Include(x => x.Lines)
            .AsSplitQuery()
            .SingleAsync(x => x.Id == supplierQuoteRevisionId, cancellationToken);

        var snapshot = new SupplierQuoteRevisionSnapshot
        {
            SupplierBusinessRelationshipId = revision.SupplierBusinessRelationshipId,
            BuyerRfqRevisionId = revision.SupplierQuote.BuyerRfqRevisionId,
            QuoteNumber = revision.SupplierQuote.QuoteNumber,
            Name = revision.SupplierQuote.Name,
            Description = revision.SupplierQuote.Description,
            SupplierReference = revision.SupplierQuote.SupplierReference,
            RevisionNumber = revision.RevisionNumber,
            CurrencyId = revision.CurrencyId,
            IncotermRuleId = revision.IncotermRuleId,
            NamedPlace = revision.NamedPlace,
            PaymentTerms = revision.PaymentTerms,
            LeadTimeDays = revision.LeadTimeDays,
            ValidFromUtc = revision.ValidFromUtc,
            ValidToUtc = revision.ValidToUtc,
            OriginalDocumentReference = revision.OriginalDocumentReference,
            Lines = revision.Lines
                .OrderBy(x => x.LineNumber)
                .Select(x => new SupplierQuoteLineSnapshot
                {
                    LineNumber = x.LineNumber,
                    BuyerRfqLineId = x.BuyerRfqLineId,
                    Kind = x.Kind,
                    SupplierMaterialOfferingId = x.SupplierMaterialOfferingId,
                    SupplierProcessCapabilityId = x.SupplierProcessCapabilityId,
                    SupplySolutionStepId = x.SupplySolutionStepId,
                    InventoryLotId = x.InventoryLotId,
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
                    SupplierLineReference = x.SupplierLineReference
                })
                .ToList()
        };

        var json = JsonSerializer.Serialize(snapshot, SerializerOptions);

        return _canonicalJsonService.CanonicalizeAndHash(json);
    }

    /// <summary>
    /// Beräknar hash från ett redan lagrat snapshot.
    /// </summary>
    public string ComputeHash(string snapshotJson)
    {
        return _canonicalJsonService.ComputeHash(snapshotJson);
    }
}