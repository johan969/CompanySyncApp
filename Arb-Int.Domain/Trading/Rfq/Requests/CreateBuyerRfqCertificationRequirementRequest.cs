using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller data för att skapa ett certifieringskrav.
/// </summary>
public sealed class CreateBuyerRfqCertificationRequirementRequest : BuyerRfqRequirementRequestBase
{
    /// <summary>
    /// Identifierar certifieringssystemet.
    /// </summary>
    public int CertificationSchemeId { get; init; }

    /// <summary>
    /// Anger operatorn som används vid utvärdering.
    /// </summary>
    public RequirementOperator Operator { get; init; }

    /// <summary>
    /// Anger vilket certifieringsscope som krävs.
    /// </summary>
    public CertificationScope RequiredScope { get; init; }

    /// <summary>
    /// Identifierar eventuell jurisdiktion inom certifieringssystemet.
    /// </summary>
    public int? CertificationSchemeJurisdictionId { get; init; }

    /// <summary>
    /// Anger om certifieringen måste vara giltig vid leverans.
    /// </summary>
    public bool MustBeValidAtDelivery { get; init; } = true;
}