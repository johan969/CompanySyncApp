namespace Arb_Int.Domain.Sourcing;

/// <summary>
/// Normaliserar supplier-specifika identifierare som används för jämförelse och unikhet.
/// </summary>
public interface ISupplierMaterialOfferingNormalizer
{
    /// <summary>
    /// Normaliserar supplier SKU utan att tolka dess affärsmässiga betydelse.
    /// <para>Exempel:<br />
    /// "  ALG-42  " blir "alg-42".</para>
    /// </summary>
    string NormalizeSupplierSku(string supplierSku);
}
