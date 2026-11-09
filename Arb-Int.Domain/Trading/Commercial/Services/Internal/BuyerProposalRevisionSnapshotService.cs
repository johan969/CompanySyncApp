using System.Text.Json;
using System.Text.Json.Serialization;
using Arb_Int.Domain.Services.Common;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Trading.Snapshots;

/// <summary>
/// Skapar deterministic buyer-facing snapshots utan supplier- eller margin-information.
/// </summary>
internal sealed class BuyerProposalRevisionSnapshotService : IBuyerProposalRevisionSnapshotService
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
    public BuyerProposalRevisionSnapshotService(
        Arb_IntDbContext db,
        ICanonicalJsonService canonicalJsonService)
    {
        _db = db;
        _canonicalJsonService = canonicalJsonService;
    }

    /// <summary>
    /// Bygger snapshot från root, revision, options och buyer-facing lines.
    /// </summary>
    public async Task<CanonicalJsonPayload> CreateAsync(
        Guid buyerProposalRevisionId,
        CancellationToken cancellationToken = default)
    {
        var revision = await _db.Set<BuyerProposalRevision>()
            .AsNoTracking()
            .Include(x => x.BuyerProposal)
            .Include(x => x.Options)
                .ThenInclude(x => x.Lines)
            .AsSplitQuery()
            .SingleAsync(
                x => x.Id == buyerProposalRevisionId,
                cancellationToken);

        string? disclosureContentHash = null;

        if (revision.DisclosurePackageRevisionId.HasValue)
        {
            disclosureContentHash = await _db.Set<DisclosurePackageRevision>()
                .AsNoTracking()
                .Where(x => x.Id == revision.DisclosurePackageRevisionId.Value)
                .Select(x => x.ContentHash)
                .SingleAsync(cancellationToken);
        }

        var snapshot = new BuyerProposalRevisionSnapshot
        {
            BuyerBusinessRelationshipId = revision.BuyerProposal.BuyerBusinessRelationshipId,
            BuyerRfqId = revision.BuyerRfqId,
            BuyerRfqRevisionId = revision.BuyerRfqRevisionId,
            ProposalNumber = revision.BuyerProposal.ProposalNumber,
            Name = revision.BuyerProposal.Name,
            Description = revision.BuyerProposal.Description,
            RevisionNumber = revision.RevisionNumber,
            CurrencyId = revision.CurrencyId,
            ValidFromUtc = revision.ValidFromUtc,
            ValidToUtc = revision.ValidToUtc,
            CommercialTerms = revision.CommercialTerms,
            BuyerFacingNotes = revision.BuyerFacingNotes,
            DisclosurePackageRevisionId = revision.DisclosurePackageRevisionId,
            DisclosurePackageContentHash = disclosureContentHash,
            Options = revision.Options
                .OrderBy(x => x.OptionNumber)
                .Select(option => new BuyerProposalOptionSnapshot
                {
                    OptionNumber = option.OptionNumber,
                    Name = option.Name,
                    Description = option.Description,
                    TotalNetAmount = option.TotalNetAmount,
                    TotalTaxAmount = option.TotalTaxAmount,
                    TotalGrossAmount = option.TotalGrossAmount,
                    CurrencyId = option.CurrencyId,
                    EstimatedLeadTimeDays = option.EstimatedLeadTimeDays,
                    BuyerFacingSummary = option.BuyerFacingSummary,
                    Lines = option.Lines
                        .OrderBy(x => x.LineNumber)
                        .Select(line => new BuyerProposalLineSnapshot
                        {
                            BuyerRfqLineId = line.BuyerRfqLineId,
                            LineNumber = line.LineNumber,
                            MaterialId = line.MaterialId,
                            ProductId = line.ProductId,
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
                            BuyerFacingNotes = line.BuyerFacingNotes
                        })
                        .ToList()
                })
                .ToList()
        };

        var json = JsonSerializer.Serialize(
            snapshot,
            SerializerOptions);

        return _canonicalJsonService.CanonicalizeAndHash(
            json);
    }

    /// <summary>
    /// Beräknar hash från canonical representation av ett proposal-snapshot.
    /// </summary>
    public string ComputeHash(
        string snapshotJson)
    {
        return _canonicalJsonService.ComputeHash(
            snapshotJson);
    }
}