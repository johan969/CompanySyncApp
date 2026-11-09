using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Normaliserar organisationsidentifierare till stabila jämförelsevärden.
    /// </summary>
    public sealed class OrganizationIdentifierNormalizer : IOrganizationIdentifierNormalizer
    {
        /// <summary>
        /// Normaliserar scheme till lowercase maskinkod.
        /// </summary>
        public string NormalizeScheme(string scheme)
        {
            return scheme
                .Normalize(NormalizationForm.FormKC)
                .Trim()
                .ToLowerInvariant();
        }

        /// <summary>
        /// Normaliserar identifierarvärdet utan att förstöra scheme-specifik information.
        /// <para>
        /// Unicode normaliseras, omgivande whitespace tas bort och upprepade whitespace-tecken kollapsas.<br />
        /// Scheme-parametern finns med så att regler för exempelvis VAT, LEI eller DUNS kan specialiseras senare.
        /// </para>
        /// </summary>
        public string NormalizeValue(string scheme, string value)
        {
            var normalized = value
                .Normalize(NormalizationForm.FormKC)
                .Trim();

            normalized = Regex.Replace(
                normalized,
                @"\s+",
                " ");

            return normalized.ToUpperInvariant();
        }

        /// <summary>
        /// Skapar en SHA-256-nyckel av scheme, utfärdande land och normaliserat värde.
        /// </summary>
        public string CreateIdentifierKey(
            string normalizedScheme,
            Guid? issuingCountryId,
            string normalizedValue)
        {
            var canonicalValue = string.Join(
                "|",
                normalizedScheme,
                issuingCountryId?.ToString("N") ?? string.Empty,
                normalizedValue);

            var hash = SHA256.HashData(
                Encoding.UTF8.GetBytes(canonicalValue));

            return Convert.ToHexString(hash)
                .ToLowerInvariant();
        }
    }
}