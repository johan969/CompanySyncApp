namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller den normaliserade adressdata som hör till en PartyAddress.
    /// </summary>
    public sealed class AddressResponse
    {
        /// <summary>
        /// Adressens identifierare.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Eventuellt c/o-namn.
        /// </summary>
        public string? CareOf { get; init; }

        /// <summary>
        /// Eventuell postbox.
        /// </summary>
        public string? PostOfficeBox { get; init; }

        /// <summary>
        /// Första adressraden.
        /// </summary>
        public string? AddressLine1 { get; init; }

        /// <summary>
        /// Andra adressraden.
        /// </summary>
        public string? AddressLine2 { get; init; }

        /// <summary>
        /// Tredje adressraden.
        /// </summary>
        public string? AddressLine3 { get; init; }

        /// <summary>
        /// Postnummer.
        /// </summary>
        public string? PostalCode { get; init; }

        /// <summary>
        /// Ort eller stad.
        /// </summary>
        public string? Locality { get; init; }

        /// <summary>
        /// Administrativ region, exempelvis delstat eller län.
        /// </summary>
        public string? AdministrativeArea { get; init; }

        /// <summary>
        /// Landets identifierare.
        /// </summary>
        public Guid CountryId { get; init; }
    }
}