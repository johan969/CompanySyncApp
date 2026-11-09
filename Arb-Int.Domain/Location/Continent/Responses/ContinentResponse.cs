namespace Arb_Int.Domain.Location;

/// <summary>
/// Representerar en kontinent som referensdata.
/// </summary>
public sealed class ContinentResponse
{
    /// <summary>
    /// Anger kontinentens id.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Anger kontinentens namn.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Anger kontinentens kod.
    /// </summary>
    public required string ContinentCode { get; init; }
}
