using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Arb_Int.Domain.Sourcing;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Trading.Snapshots;

// TODO: Den här services borde använda ICanonicalJsonService.
/// <summary>
/// Bygger deterministic RFQ-snapshots som kan verifieras även efter lagring som PostgreSQL jsonb.
/// </summary>
internal sealed class BuyerRfqRevisionSnapshotService : IBuyerRfqRevisionSnapshotService
{
    private static readonly JsonSerializerOptions SerializerOptions = CreateSerializerOptions();
    private readonly Arb_IntDbContext _db;

    /// <summary>
    /// Skapar snapshot-servicen med aktuell databascontext.
    /// </summary>
    public BuyerRfqRevisionSnapshotService(Arb_IntDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Läser hela revisionsaggregatet och skapar canonical JSON samt SHA-256-hash.
    /// </summary>
    public async Task<BuyerRfqRevisionSnapshotPayload> CreateAsync(
        Guid buyerRfqRevisionId,
        CancellationToken cancellationToken = default)
    {
        var revision = await _db.Set<BuyerRfqRevision>()
            .AsNoTracking()
            .Include(x => x.BuyerRfq)
            .Include(x => x.Lines)
                .ThenInclude(x => x.MaterialOptions)
            .Include(x => x.Requirements)
            .Include(x => x.Deliveries)
                .ThenInclude(x => x.LineAllocations)
            .AsSplitQuery()
            .SingleOrDefaultAsync(
                x => x.Id == buyerRfqRevisionId,
                cancellationToken)
            ?? throw new InvalidOperationException(
                $"BuyerRfqRevision '{buyerRfqRevisionId}' could not be loaded for snapshot creation.");

        var lineNumbers = revision.Lines.ToDictionary(
            x => x.Id,
            x => x.LineNumber);

        var snapshot = new BuyerRfqRevisionSnapshot
        {
            BuyerBusinessRelationshipId = revision.BuyerRfq.BuyerBusinessRelationshipId,
            BuyerDemandId = revision.BuyerRfq.BuyerDemandId,
            RfqNumber = revision.BuyerRfq.RfqNumber,
            Name = revision.BuyerRfq.Name,
            Description = revision.BuyerRfq.Description,
            Priority = revision.BuyerRfq.Priority,
            Visibility = revision.BuyerRfq.Visibility,
            BuyerReference = revision.BuyerRfq.BuyerReference,
            Summary = revision.Summary,
            BuyerOriginalText = revision.BuyerOriginalText,
            InternalInterpretation = revision.InternalInterpretation,
            ResponseRequestedByUtc = revision.ResponseRequestedByUtc,
            Lines = revision.Lines
                .OrderBy(x => x.LineNumber)
                .Select(CreateLineSnapshot)
                .ToList(),
            Requirements = revision.Requirements
                .OrderBy(x => x.BuyerRfqLineId.HasValue ? lineNumbers[x.BuyerRfqLineId.Value] : -1)
                .ThenBy(x => x.SortOrder)
                .ThenBy(x => GetRequirementType(x))
                .ThenBy(x => x.Name)
                .Select(x => CreateRequirementSnapshot(x, lineNumbers))
                .ToList(),
            Deliveries = revision.Deliveries
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.DeliveryNumber)
                .Select(x => CreateDeliverySnapshot(x, lineNumbers))
                .ToList()
        };

        var serializedJson = JsonSerializer.Serialize(
            snapshot,
            SerializerOptions);

        var canonicalJson = Canonicalize(
            serializedJson);

        return new BuyerRfqRevisionSnapshotPayload
        {
            SnapshotJson = canonicalJson,
            SnapshotHash = ComputeHashFromCanonicalJson(canonicalJson)
        };
    }

    /// <summary>
    /// Canonicaliserar JSON och beräknar samma SHA-256-hash som används vid release.
    /// </summary>
    public string ComputeHash(string snapshotJson)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(snapshotJson);

        var canonicalJson = Canonicalize(
            snapshotJson);

        return ComputeHashFromCanonicalJson(
            canonicalJson);
    }

    /// <summary>
    /// Skapar stabila serializer-inställningar för snapshot-formatet.
    /// </summary>
    private static JsonSerializerOptions CreateSerializerOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            WriteIndented = false
        };

        options.Converters.Add(
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

        return options;
    }

    /// <summary>
    /// Skapar snapshot-data för en RFQ-rad.
    /// </summary>
    private static BuyerRfqLineSnapshot CreateLineSnapshot(BuyerRfqLine line)
    {
        return new BuyerRfqLineSnapshot
        {
            BuyerDemandScopeId = line.BuyerDemandScopeId,
            LineNumber = line.LineNumber,
            Name = line.Name,
            Description = line.Description,
            BuyerLineReference = line.BuyerLineReference,
            BuyerOriginalText = line.BuyerOriginalText,
            MaterialId = line.MaterialId,
            MaterialTypeId = line.MaterialTypeId,
            IntendedUseCharacteristicId = line.IntendedUseCharacteristicId,
            RequiredQuantity = line.RequiredQuantity,
            QuantityUnitId = line.QuantityUnitId,
            LowerQuantityTolerancePercent = line.LowerQuantityTolerancePercent,
            UpperQuantityTolerancePercent = line.UpperQuantityTolerancePercent,
            AllowPartialFulfillment = line.AllowPartialFulfillment,
            AllowMultipleSuppliers = line.AllowMultipleSuppliers,
            AllowMultipleOrigins = line.AllowMultipleOrigins,
            AllowMultipleLots = line.AllowMultipleLots,
            RequireSingleSupplier = line.RequireSingleSupplier,
            RequireSingleOrigin = line.RequireSingleOrigin,
            RequireSingleLot = line.RequireSingleLot,
            RequireUniformCertification = line.RequireUniformCertification,
            RequireUniformSpecification = line.RequireUniformSpecification,
            MaximumSupplierCount = line.MaximumSupplierCount,
            InternalNotes = line.InternalNotes,
            MaterialOptions = line.MaterialOptions
                .OrderBy(x => x.OptionKind)
                .ThenBy(x => x.SortOrder)
                .ThenBy(x => x.MaterialId)
                .ThenBy(x => x.MaterialTypeId)
                .ThenBy(x => x.BiologicalSourceId)
                .ThenBy(x => x.BiologicalSourceClaimId)
                .ThenBy(x => x.BuyerText)
                .Select(x => new BuyerRfqLineMaterialOptionSnapshot
                {
                    OptionKind = x.OptionKind,
                    MaterialId = x.MaterialId,
                    MaterialTypeId = x.MaterialTypeId,
                    BiologicalSourceId = x.BiologicalSourceId,
                    BiologicalSourceClaimId = x.BiologicalSourceClaimId,
                    BuyerText = x.BuyerText,
                    AlternativeGroupKey = x.AlternativeGroupKey,
                    MinimumMappingConfidence = x.MinimumMappingConfidence,
                    SortOrder = x.SortOrder
                })
                .ToList()
        };
    }

    /// <summary>
    /// Skapar snapshot-data för ett polymorft RFQ-krav.
    /// </summary>
    private static BuyerRfqRequirementSnapshot CreateRequirementSnapshot(
        BuyerRfqRequirement requirement,
        IReadOnlyDictionary<Guid, int> lineNumbers)
    {
        var snapshot = new BuyerRfqRequirementSnapshot
        {
            RequirementType = GetRequirementType(requirement),
            BuyerRfqLineNumber = requirement.BuyerRfqLineId.HasValue ? lineNumbers[requirement.BuyerRfqLineId.Value] : null,
            Name = requirement.Name,
            Description = requirement.Description,
            Level = requirement.Level,
            Weight = requirement.Weight,
            AlternativeGroupKey = requirement.AlternativeGroupKey,
            IsActive = requirement.IsActive,
            SortOrder = requirement.SortOrder,
            BuyerOriginalText = requirement.BuyerOriginalText,
            InternalNotes = requirement.InternalNotes
        };

        switch (requirement)
        {
            case BuyerRfqMaterialRequirement x:
                snapshot.MaterialId = x.MaterialId;
                snapshot.MaterialTypeId = x.MaterialTypeId;
                snapshot.MaterialMatchMode = x.MatchMode;
                snapshot.AllowSubstitutes = x.AllowSubstitutes;
                break;

            case BuyerRfqBiologicalSourceRequirement x:
                snapshot.BiologicalSourceId = x.BiologicalSourceId;
                snapshot.Operator = x.Operator;
                break;

            case BuyerRfqOriginRequirement x:
                snapshot.GeographicLocationId = x.GeographicLocationId;
                snapshot.CountryId = x.CountryId;
                snapshot.OriginRole = x.OriginRole;
                snapshot.Operator = x.Operator;
                break;

            case BuyerRfqCertificationRequirement x:
                snapshot.CertificationSchemeId = x.CertificationSchemeId;
                snapshot.Operator = x.Operator;
                snapshot.CertificationScope = x.RequiredScope;
                snapshot.CertificationSchemeJurisdictionId = x.CertificationSchemeJurisdictionId;
                snapshot.MustBeValidAtDelivery = x.MustBeValidAtDelivery;
                break;

            case BuyerRfqDocumentRequirement x:
                snapshot.DocumentTypeId = x.DocumentTypeId;
                snapshot.Operator = x.Operator;
                snapshot.MustBeLotSpecific = x.MustBeLotSpecific;
                snapshot.MustBeIssuedByAccreditedParty = x.MustBeIssuedByAccreditedParty;
                snapshot.MaximumAgeDays = x.MaximumAgeDays;
                break;

            case BuyerRfqConstituentRequirement x:
                snapshot.ConstituentId = x.ConstituentId;
                snapshot.Operator = x.Operator;
                snapshot.MinimumValue = x.MinimumValue;
                snapshot.MaximumValue = x.MaximumValue;
                snapshot.TargetValue = x.TargetValue;
                snapshot.UnitOfMeasureId = x.UnitOfMeasureId;
                snapshot.MeasurementBasis = x.Basis;
                snapshot.AnalysisMethod = x.AnalysisMethod;
                snapshot.MustBeVerifiedForEveryLot = x.MustBeVerifiedForEveryLot;
                snapshot.AccreditedLaboratoryRequired = x.AccreditedLaboratoryRequired;
                break;

            case BuyerRfqCharacteristicRequirement x:
                snapshot.CharacteristicId = x.CharacteristicId;
                snapshot.Operator = x.Operator;
                snapshot.ExpectedTextValue = x.ExpectedTextValue;
                snapshot.ExpectedNumericValue = x.ExpectedNumericValue;
                snapshot.UnitOfMeasureId = x.UnitOfMeasureId;
                break;

            case BuyerRfqProcessRequirement x:
                snapshot.ProcessTypeId = x.ProcessTypeId;
                snapshot.ProcessMode = x.Mode;
                snapshot.ProcessParametersJson = x.ProcessParametersJson;
                break;

            case BuyerRfqCommercialRequirement x:
                snapshot.CommercialKind = x.Kind;
                snapshot.Operator = x.Operator;
                snapshot.NumericValue = x.NumericValue;
                snapshot.TextValue = x.TextValue;
                snapshot.UnitOfMeasureId = x.UnitOfMeasureId;
                snapshot.CurrencyId = x.CurrencyId;
                break;

            case BuyerRfqLogisticsRequirement x:
                snapshot.LogisticsKind = x.Kind;
                snapshot.Operator = x.Operator;
                snapshot.NumericValue = x.NumericValue;
                snapshot.TextValue = x.TextValue;
                snapshot.UnitOfMeasureId = x.UnitOfMeasureId;
                snapshot.IncotermRuleId = x.IncotermRuleId;
                break;

            default:
                throw new InvalidOperationException(
                    $"Unsupported BuyerRfqRequirement type '{requirement.GetType().Name}'.");
        }

        return snapshot;
    }

    /// <summary>
    /// Skapar snapshot-data för ett leveranstillfälle.
    /// </summary>
    private static BuyerRfqDeliverySnapshot CreateDeliverySnapshot(
        BuyerRfqDelivery delivery,
        IReadOnlyDictionary<Guid, int> lineNumbers)
    {
        return new BuyerRfqDeliverySnapshot
        {
            DeliveryNumber = delivery.DeliveryNumber,
            DestinationAddressId = delivery.DestinationAddressId,
            DestinationLocationId = delivery.DestinationLocationId,
            RequestedDeliveryFromUtc = delivery.RequestedDeliveryFromUtc,
            RequestedDeliveryToUtc = delivery.RequestedDeliveryToUtc,
            IncotermRuleId = delivery.IncotermRuleId,
            NamedPlace = delivery.NamedPlace,
            MinimumTemperatureCelsius = delivery.MinimumTemperatureCelsius,
            MaximumTemperatureCelsius = delivery.MaximumTemperatureCelsius,
            PackagingNotes = delivery.PackagingNotes,
            DeliveryNotes = delivery.DeliveryNotes,
            SortOrder = delivery.SortOrder,
            LineAllocations = delivery.LineAllocations
                .OrderBy(x => lineNumbers[x.BuyerRfqLineId])
                .Select(x => new BuyerRfqDeliveryLineAllocationSnapshot
                {
                    BuyerRfqLineNumber = lineNumbers[x.BuyerRfqLineId],
                    Quantity = x.Quantity,
                    QuantityUnitId = x.QuantityUnitId
                })
                .ToList()
        };
    }

    /// <summary>
    /// Identifierar TPH-typen med snapshotets stabila discriminator.
    /// </summary>
    private static BuyerRfqRequirementSnapshotType GetRequirementType(BuyerRfqRequirement requirement)
    {
        return requirement switch
        {
            BuyerRfqMaterialRequirement => BuyerRfqRequirementSnapshotType.Material,
            BuyerRfqBiologicalSourceRequirement => BuyerRfqRequirementSnapshotType.BiologicalSource,
            BuyerRfqOriginRequirement => BuyerRfqRequirementSnapshotType.Origin,
            BuyerRfqCertificationRequirement => BuyerRfqRequirementSnapshotType.Certification,
            BuyerRfqDocumentRequirement => BuyerRfqRequirementSnapshotType.Document,
            BuyerRfqConstituentRequirement => BuyerRfqRequirementSnapshotType.Constituent,
            BuyerRfqCharacteristicRequirement => BuyerRfqRequirementSnapshotType.Characteristic,
            BuyerRfqProcessRequirement => BuyerRfqRequirementSnapshotType.Process,
            BuyerRfqCommercialRequirement => BuyerRfqRequirementSnapshotType.Commercial,
            BuyerRfqLogisticsRequirement => BuyerRfqRequirementSnapshotType.Logistics,
            _ => throw new InvalidOperationException($"Unsupported BuyerRfqRequirement type '{requirement.GetType().Name}'.")
        };
    }

    /// <summary>
    /// Canonicaliserar JSON genom stabil property-ordning och normaliserade numeriska värden.
    /// </summary>
    private static string Canonicalize(string json)
    {
        using var document = JsonDocument.Parse(json);
        using var stream = new MemoryStream();

        using (var writer = new Utf8JsonWriter(stream))
        {
            WriteCanonicalElement(
                writer,
                document.RootElement);

            writer.Flush();
        }

        return Encoding.UTF8.GetString(
            stream.ToArray());
    }

    /// <summary>
    /// Skriver ett JSON-element i canonical form.
    /// </summary>
    private static void WriteCanonicalElement(Utf8JsonWriter writer, JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                writer.WriteStartObject();

                foreach (var property in element
                    .EnumerateObject()
                    .OrderBy(x => x.Name, StringComparer.Ordinal))
                {
                    writer.WritePropertyName(property.Name);
                    WriteCanonicalElement(writer, property.Value);
                }

                writer.WriteEndObject();
                break;

            case JsonValueKind.Array:
                writer.WriteStartArray();

                foreach (var item in element.EnumerateArray())
                {
                    WriteCanonicalElement(writer, item);
                }

                writer.WriteEndArray();
                break;

            case JsonValueKind.String:
                writer.WriteStringValue(element.GetString());
                break;

            case JsonValueKind.Number:
                WriteCanonicalNumber(writer, element);
                break;

            case JsonValueKind.True:
                writer.WriteBooleanValue(true);
                break;

            case JsonValueKind.False:
                writer.WriteBooleanValue(false);
                break;

            case JsonValueKind.Null:
                writer.WriteNullValue();
                break;

            default:
                throw new InvalidOperationException(
                    $"Unsupported JSON value kind '{element.ValueKind}'.");
        }
    }

    /// <summary>
    /// Skriver numeriska JSON-värden utan onödiga representationsskillnader.
    /// </summary>
    private static void WriteCanonicalNumber(Utf8JsonWriter writer, JsonElement element)
    {
        if (element.TryGetInt64(out var int64Value))
        {
            writer.WriteNumberValue(int64Value);
            return;
        }

        if (element.TryGetUInt64(out var uint64Value))
        {
            writer.WriteNumberValue(uint64Value);
            return;
        }

        if (element.TryGetDecimal(out var decimalValue))
        {
            writer.WriteRawValue(
                decimalValue.ToString("G29", CultureInfo.InvariantCulture),
                skipInputValidation: true);

            return;
        }

        var doubleValue = element.GetDouble();

        writer.WriteRawValue(
            doubleValue.ToString("G17", CultureInfo.InvariantCulture),
            skipInputValidation: true);
    }

    /// <summary>
    /// Beräknar SHA-256 över canonical UTF-8 JSON.
    /// </summary>
    private static string ComputeHashFromCanonicalJson(string canonicalJson)
    {
        var bytes = Encoding.UTF8.GetBytes(
            canonicalJson);

        var hash = SHA256.HashData(
            bytes);

        return Convert
            .ToHexString(hash)
            .ToLowerInvariant();
    }
}