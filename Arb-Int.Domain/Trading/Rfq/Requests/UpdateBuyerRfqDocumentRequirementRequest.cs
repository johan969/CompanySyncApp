using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller data för att uppdatera ett dokumentkrav.
/// </summary>
public sealed class UpdateBuyerRfqDocumentRequirementRequest : UpdateBuyerRfqRequirementRequestBase
{
    /// <summary>
    /// Identifierar dokumenttypen.
    /// </summary>
    public int DocumentTypeId { get; init; }

    /// <summary>
    /// Anger operatorn som används vid utvärdering.
    /// </summary>
    public RequirementOperator Operator { get; init; }

    /// <summary>
    /// Anger om dokumentet måste vara specifikt för aktuell lot.
    /// </summary>
    public bool MustBeLotSpecific { get; init; }

    /// <summary>
    /// Anger om dokumentet måste vara utfärdat av en ackrediterad part.
    /// </summary>
    public bool MustBeIssuedByAccreditedParty { get; init; }

    /// <summary>
    /// Anger eventuellt maximalt tillåten dokumentålder i dagar.
    /// </summary>
    public int? MaximumAgeDays { get; init; }
}