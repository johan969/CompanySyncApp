namespace Arb_Int.Domain.Sourcing;

/// <summary>
/// Innehåller grunddata för ett nytt supplier material offering.
/// </summary>
public sealed class CreateSupplierMaterialOfferingRequest
{
    /// <summary>
    /// Stabil deterministisk nyckel inom supplier-relationen.
    /// </summary>
    public required string OfferingKey { get; init; }

    /// <summary>
    /// Anger offeringens visningsnamn.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Beskriver det observerade eller deklarerade erbjudandet.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Anger leverantörens eget artikelnummer när det är känt.
    /// </summary>
    public string? SupplierSku { get; init; }

    /// <summary>
    /// Anger formatet så som det deklarerades av leverantören eller källan.
    /// <para>Exempel:<br />
    /// freeze-dried powder<br />
    /// IQF frozen</para>
    /// </summary>
    public string? DeclaredFormat { get; init; }

    /// <summary>
    /// Anger när offeringet först observerades.
    /// </summary>
    public DateTimeOffset? FirstObservedUtc { get; init; }

    /// <summary>
    /// Anger när offeringet senast observerades.
    /// </summary>
    public DateTimeOffset? LastObservedUtc { get; init; }

    /// <summary>
    /// Anger när offeringet senast verifierades.
    /// </summary>
    public DateTimeOffset? LastVerifiedUtc { get; init; }
}
