namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller en funktionstyp som kan tilldelas en organisationskontakt.
    /// </summary>
    public sealed class ContactFunctionTypeResponse
    {
        /// <summary>
        /// Identifierar funktionstypen.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Funktionstypens stabila maskinkod.
        /// </summary>
        public required string Code { get; init; }

        /// <summary>
        /// Anger funktionstypens sorteringsordning.
        /// </summary>
        public int SortOrder { get; init; }
    }
}