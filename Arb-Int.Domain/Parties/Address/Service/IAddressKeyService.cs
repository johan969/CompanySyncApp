namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Skapar deterministiska nycklar för normaliserade adresser.
    /// </summary>
    public interface IAddressKeyService
    {
        /// <summary>
        /// Skapar en stabil nyckel från adressens normaliserade innehåll.
        /// </summary>
        string CreateKey(
            string? careOf,
            string? postOfficeBox,
            string? addressLine1,
            string? addressLine2,
            string? addressLine3,
            string? postalCode,
            string? locality,
            string? administrativeArea,
            Guid countryId);
    }
}