namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller data för att uppdatera ett processkrav.
/// </summary>
public sealed class UpdateBuyerRfqProcessRequirementRequest : UpdateBuyerRfqRequirementRequestBase
{
    /// <summary>
    /// Identifierar processtypen.
    /// </summary>
    public int ProcessTypeId { get; init; }

    /// <summary>
    /// Anger om processen krävs, föredras eller är förbjuden.
    /// </summary>
    public ProcessRequirementMode Mode { get; init; }

    /// <summary>
    /// Innehåller eventuella processparametrar som JSON.
    /// </summary>
    public string? ProcessParametersJson { get; init; }
}