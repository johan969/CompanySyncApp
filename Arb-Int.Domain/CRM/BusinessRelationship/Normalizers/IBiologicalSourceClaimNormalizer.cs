namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Normaliserar deklarerade namn för biologiska source-claims.
    /// </summary>
    public interface IBiologicalSourceClaimNormalizer
    {
        /// <summary>
        /// Normaliserar ett deklarerat namn för sökning och dubblettkontroll.
        /// </summary>
        string NormalizeDeclaredName(string declaredName);
    }
}