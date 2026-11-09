using System.Text;
using System.Text.RegularExpressions;

namespace Arb_Int.Domain.Sourcing;

/// <summary>
/// Skapar stabila normaliserade supplier-identiteter för material offerings.
/// </summary>
public sealed class SupplierMaterialOfferingNormalizer : ISupplierMaterialOfferingNormalizer
{
    /// <summary>
    /// Normaliserar Unicode, whitespace och casing för supplier SKU.
    /// </summary>
    public string NormalizeSupplierSku(string supplierSku)
    {
        var normalized = supplierSku
            .Normalize(NormalizationForm.FormKC)
            .Trim();

        normalized = Regex.Replace(
            normalized,
            @"\s+",
            " ");

        return normalized.ToLowerInvariant();
    }
}
