using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Articles
{
    public class ProductIdentifier : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;

        /// <summary>
        /// Öppen maskinkod för identifierarens typ.
        /// <para>Exempel:<br />
        /// gtin<br />
        /// gtin-13 (07350000000001)<br />
        /// ean-13 (7350000000001)<br />
        /// upc-a<br />
        /// mpn (ALARIA-POWDER-250)<br />
        /// supplier-code (SUP-12345)</para>
        /// </summary>
        public string Scheme { get; set; } = null!;

        /// <summary>
        /// Den organisation som har utfärdat identifieraren när detta
        /// behövs för att göra identifieraren unik.
        /// <para>För GTIN kan detta normalt vara null.
        /// För leverantörskoder bör det normalt anges.</para>
        /// </summary>
        public string? Issuer { get; set; }

        /// <summary>
        /// Identifieraren i visningsformat.
        /// </summary>
        public string Value { get; set; } = null!;

        /// <summary>
        /// Deterministisk nyckel som genereras av applikationen.
        /// <para>Den kan exempelvis vara SHA-256 av:<br />
        /// normalized scheme + issuer + value.</para>
        /// </summary>
        public string IdentifierKey { get; set; } = null!;

        /// <summary>
        /// Anger vilken extern identifierare som ska visas som
        /// produktens huvudsakliga identifierare.
        /// </summary>
        public bool IsPrimary { get; set; }
    }
}