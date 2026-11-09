using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Articles
{
    /// <summary>
    /// <para>Product beskriver den kommersiella representationen:<br />
    /// namn, SKU, status, försäljningsenhet och förpackat innehåll.</para>
    /// <para>Material beskriver vad produkten faktiskt består av och används
    /// som utgångspunkt för härledning till råvaror, biologiska källor,
    /// ursprung och transformationskedjor.</para>
    /// </summary>
    public class Product :
        IEntity<Guid>,
        INamed,
        IDescribable,
        ICreateTracker,
        IActionTracker
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Produktens interna artikelnummer.
        /// <para>Exempel:<br />
        /// ALG-ALA-POW-250<br />
        /// SEED-SUNFLOWER-500<br />
        /// OIL-SUNFLOWER-REF-500</para>
        /// </summary>
        public string Sku { get; set; } = null!;

        /// <summary>
        /// Normaliserad SKU för jämförelse och unikt index.<br />
        /// Skapas med Trim och ToUpperInvariant.
        /// </summary>
        public string NormalizedSku { get; set; } = null!;

        public string Name { get; set; } = null!;

        /// <summary>
        /// Stabil URL- och importnyckel.
        /// </summary>
        public string Slug { get; set; } = null!;

        /// <summary>
        /// Kort beskrivning för listor, sökresultat och produktkort.
        /// </summary>
        public string? ShortDescription { get; set; }

        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Interna anteckningar. Publicera EJ!.
        /// </summary>
        public string? Notes { get; set; }

        public ProductStatus Status { get; set; } = ProductStatus.Draft;

        /// <summary>
        /// Det slutmaterial som säljs i denna produkt.<br />
        /// För ett algpulver kan det vara "Torkat och malt Alaria-material".<br />
        /// För en påse fröer kan detta vara materialet "Solrosfrö".<br />
        /// För en flaska olja kan det vara "Raffinerad solrosolja".
        /// </summary>
        public Guid MaterialId { get; set; }
        public Material Material { get; set; } = null!;

        /// <summary>
        /// Antalet försäljningsenheter som denna produktpost representerar.
        /// <para>Normalt 1, exempelvis:<br />
        /// 1 styck<br />
        /// 1 säck<br />
        /// 1 trumma<br />
        /// 1 kartong</para>
        /// <para>För en bulkprodukt kan det exempelvis vara:<br />
        /// 1 kilogram, 1 ton etc</para>
        /// </summary>
        public decimal SalesUnitQuantity { get; set; } = 1m;

        public int SalesUnitOfMeasureId { get; set; }
        public UnitOfMeasure SalesUnitOfMeasure { get; set; } = null!;

        /// <summary>
        /// Det deklarerade nettoinnehållet.
        /// <para>Exempel:<br />
        /// 500 g<br />
        /// 250 ml<br />
        /// 90 st</para>
        /// <para>Detta är skilt från försäljningsenheten. En flaska säljs
        /// exempelvis som 1 "each" men innehåller 500 ml.</para>
        /// </summary>
        public decimal? NetContentQuantity { get; set; }

        public int? NetContentUnitOfMeasureId { get; set; }
        public UnitOfMeasure? NetContentUnitOfMeasure { get; set; }

        /// <summary>
        /// Datum från vilket produkten får visas eller göras tillgänglig.
        /// Null betyder att inget särskilt startdatum har angetts.
        /// </summary>
        public DateTimeOffset? AvailableFromUtc { get; set; }

        /// <summary>
        /// Datum då produkten slutade eller planeras sluta säljas.
        /// </summary>
        public DateTimeOffset? DiscontinuedDateUtc { get; set; }

        public DateTimeOffset? CreateDateUtc { get; set; }

        public string? ActionName { get; set; }

        public DateTimeOffset? ActionDateUtc { get; set; }

        public Guid? ActionUserId { get; set; }

        /// <summary>
        /// Externa identifierare såsom GTIN, EAN, UPC,
        /// tillverkarens artikelnummer eller leverantörskod.
        /// </summary>
        public ICollection<ProductIdentifier> Identifiers { get; set; } = [];

        /// <summary>
        /// Datadrivna produktegenskaper.
        /// <para>Exempel:<br />
        /// produktkategori,<br />
        /// certifiering,<br />
        /// avsedd användning,<br />
        /// förpackningsformat.</para>
        /// </summary>
        public ICollection<ProductCharacteristic> CharacteristicLinks { get; set; } = [];

        /// <summary>
        /// Produktrelaterade dokument och källor.
        /// <para>Exempel:<br />
        /// produktblad,<br />
        /// säkerhetsdatablad,<br />
        /// analyscertifikat,<br />
        /// certifieringar,<br />
        /// regulatoriska dokument.</para>
        /// </summary>
        public ICollection<ProductReference> ReferenceLinks { get; set; } = [];
    }
}