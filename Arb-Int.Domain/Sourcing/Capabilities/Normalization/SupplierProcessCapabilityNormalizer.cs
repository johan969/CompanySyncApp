using System.Text;
using System.Text.RegularExpressions;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Skapar stabila normaliserade processnamn för supplier process capabilities.
    /// </summary>
    public sealed class SupplierProcessCapabilityNormalizer : ISupplierProcessCapabilityNormalizer
    {
        /// <summary>
        /// Normaliserar Unicode, whitespace och casing i ett deklarerat processnamn.
        /// </summary>
        public string NormalizeDeclaredProcessName(string declaredProcessName)
        {
            var normalized = declaredProcessName.Normalize(NormalizationForm.FormKC).Trim();
            normalized = Regex.Replace(normalized, @"\s+", " ");
            return normalized.ToLowerInvariant();
        }
    }
}
