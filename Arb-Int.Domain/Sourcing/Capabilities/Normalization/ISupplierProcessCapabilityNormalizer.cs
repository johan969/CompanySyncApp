namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Normaliserar deklarerade processnamn som används för jämförelse och sökning.
    /// </summary>
    public interface ISupplierProcessCapabilityNormalizer
    {
        /// <summary>
        /// Normaliserar ett deklarerat processnamn utan att mappa det till ProcessType.
        /// <para>Exempel:<br />
        /// "  Aqueous   Extraction " blir "aqueous extraction".</para>
        /// </summary>
        string NormalizeDeclaredProcessName(string declaredProcessName);
    }
}
