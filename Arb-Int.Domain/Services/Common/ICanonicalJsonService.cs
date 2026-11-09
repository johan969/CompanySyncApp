namespace Arb_Int.Domain.Services.Common;

/// <summary>
/// Canonicaliserar JSON och beräknar stabila SHA-256-hashar.
/// </summary>
public interface ICanonicalJsonService
{
    /// <summary>
    /// Returnerar JSON med stabil property- och nummerrepresentation.
    /// </summary>
    string Canonicalize(string json);

    /// <summary>
    /// Beräknar SHA-256 över canonical JSON.
    /// </summary>
    string ComputeHash(string json);

    /// <summary>
    /// Canonicaliserar JSON och returnerar både JSON och hash.
    /// </summary>
    CanonicalJsonPayload CanonicalizeAndHash(string json);
}

/// <summary>
/// Innehåller canonical JSON och dess SHA-256-hash.
/// </summary>
public sealed class CanonicalJsonPayload
{
    /// <summary>
    /// Canonical JSON.
    /// </summary>
    public required string Json { get; init; }

    /// <summary>
    /// SHA-256-hash för JSON-innehållet.
    /// </summary>
    public required string Hash { get; init; }
}