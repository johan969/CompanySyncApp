using System.Text;
using System.Text.RegularExpressions;

namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Skapar ett stabilt normaliserat värde från deklarerade biologiska namn.
    /// </summary>
    public sealed class BiologicalSourceClaimNormalizer : IBiologicalSourceClaimNormalizer
    {
        /// <summary>
        /// Normaliserar Unicode, whitespace och casing utan att tolka det biologiska namnet.
        /// </summary>
        public string NormalizeDeclaredName(string declaredName)
        {
            var normalized = declaredName
                .Normalize(NormalizationForm.FormKC)
                .Trim();

            normalized = Regex.Replace(
                normalized,
                @"\s+",
                " ");

            return normalized.ToLowerInvariant();
        }
    }
}