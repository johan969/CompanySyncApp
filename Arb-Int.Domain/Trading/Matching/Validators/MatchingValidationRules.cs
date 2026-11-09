using System.Text.Json;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Innehåller gemensamma JSON-regler för matching-requests.
/// </summary>
internal static class MatchingValidationRules
{
    /// <summary>
    /// Kontrollerar att en text innehåller giltig JSON.
    /// </summary>
    public static bool BeValidJson(string? json)
    {
        if (json is null)
            return true;
        try
        {
            using var document = JsonDocument.Parse(json);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Kontrollerar att ett effective-fönster har giltig ordning.
    /// </summary>
    public static bool HaveValidEffectiveWindow(DateTimeOffset? fromUtc, DateTimeOffset? toUtc)
    {
        return !fromUtc.HasValue || !toUtc.HasValue || toUtc.Value >= fromUtc.Value;
    }
}