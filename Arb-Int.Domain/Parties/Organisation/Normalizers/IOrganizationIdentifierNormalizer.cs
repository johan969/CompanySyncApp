namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Normaliserar organisationsidentifierare och skapar deras deterministiska nycklar.
    /// </summary>
    public interface IOrganizationIdentifierNormalizer
    {
        /// <summary>
        /// Normaliserar identifierartypens maskinkod.
        /// </summary>
        string NormalizeScheme(string scheme);

        /// <summary>
        /// Normaliserar identifierarens värde för jämförelse och dubblettkontroll.
        /// </summary>
        string NormalizeValue(string scheme, string value);

        /// <summary>
        /// Skapar den deterministiska nyckeln från scheme, land och normaliserat värde.
        /// </summary>
        string CreateIdentifierKey(string normalizedScheme, Guid? issuingCountryId, string normalizedValue);
    }
}