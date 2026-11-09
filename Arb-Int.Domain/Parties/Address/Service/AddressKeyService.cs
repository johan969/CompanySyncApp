using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Skapar SHA-256-baserade nycklar för normaliserade adresser.
    /// </summary>
    public sealed class AddressKeyService : IAddressKeyService
    {
        /// <summary>
        /// Skapar en deterministisk SHA-256-nyckel från adressens fält och land.
        /// </summary>
        public string CreateKey(
            string? careOf,
            string? postOfficeBox,
            string? addressLine1,
            string? addressLine2,
            string? addressLine3,
            string? postalCode,
            string? locality,
            string? administrativeArea,
            Guid countryId)
        {
            var canonicalValue = string.Join(
                "|",
                countryId.ToString("N"),
                Normalize(careOf),
                Normalize(postOfficeBox),
                Normalize(addressLine1),
                Normalize(addressLine2),
                Normalize(addressLine3),
                Normalize(postalCode),
                Normalize(locality),
                Normalize(administrativeArea));

            var hash = SHA256.HashData(
                Encoding.UTF8.GetBytes(canonicalValue));

            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        /// <summary>
        /// Normaliserar en adressdel före nyckelgenerering.
        /// </summary>
        private static string Normalize(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var normalized = value
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