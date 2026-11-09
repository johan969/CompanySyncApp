using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Arb_Int.Domain.Services.Common;

/// <summary>
/// Implementerar stabil JSON-canonicalisering och SHA-256-hashning.
/// </summary>
internal sealed class CanonicalJsonService : ICanonicalJsonService
{
    /// <summary>
    /// Canonicaliserar JSON med stabil property- och nummerordning.
    /// </summary>
    public string Canonicalize(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);

        using var document = JsonDocument.Parse(json);
        using var stream = new MemoryStream();

        using (var writer = new Utf8JsonWriter(stream))
        {
            WriteCanonicalElement(writer, document.RootElement);
            writer.Flush();
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    /// <summary>
    /// Beräknar SHA-256 över canonical UTF-8 JSON.
    /// </summary>
    public string ComputeHash(string json)
    {
        var canonicalJson = Canonicalize(json);
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(canonicalJson));

        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    /// <summary>
    /// Returnerar canonical JSON tillsammans med dess hash.
    /// </summary>
    public CanonicalJsonPayload CanonicalizeAndHash(string json)
    {
        var canonicalJson = Canonicalize(json);
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(canonicalJson));

        return new CanonicalJsonPayload
        {
            Json = canonicalJson,
            Hash = Convert.ToHexString(hash).ToLowerInvariant()
        };
    }

    /// <summary>
    /// Skriver ett JSON-element rekursivt i canonical form.
    /// </summary>
    private static void WriteCanonicalElement(Utf8JsonWriter writer, JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                writer.WriteStartObject();

                foreach (var property in element.EnumerateObject().OrderBy(x => x.Name, StringComparer.Ordinal))
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
                throw new InvalidOperationException($"Unsupported JSON value kind '{element.ValueKind}'.");
        }
    }

    /// <summary>
    /// Skriver ett numeriskt JSON-värde med stabil representation.
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
            writer.WriteRawValue(decimalValue.ToString("G29", CultureInfo.InvariantCulture), skipInputValidation: true);
            return;
        }

        writer.WriteRawValue(element.GetDouble().ToString("G17", CultureInfo.InvariantCulture), skipInputValidation: true);
    }
}