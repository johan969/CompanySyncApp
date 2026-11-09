using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;
using Arb_Int.Domain.Crm;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// En leverantörsspecifik katalogpost eller deklarerad
    /// leveransmöjlighet.
    /// <para>Exempel:<br />
    /// - Organic Alaria esculenta powder<br />
    /// - Frozen Alaria biomass<br />
    /// - Dried Chondrus crispus</para>
    /// <para>Ett erbjudande är inte en Product. Product beskriver hur vi
    /// säljer något. Erbjudandet beskriver hur ett material kan köpas in.</para>
    /// </summary>
    public class SupplierMaterialOffering :
        IEntity<Guid>,
        INamed,
        IDescribable,
        ICreateTracker,
        IActionTracker,
        ISystemVersioned,
        INormalizedName,
        IRowVersion
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Den supplier-relation som erbjudandet tillhör.
        /// <para>Applikationslagret ska kontrollera att relationstypens kod
        /// är BusinessRelationshipTypeCodes.Supplier.</para>
        /// </summary>
        public Guid BusinessRelationshipId { get; set; }

        public BusinessRelationship BusinessRelationship { get; set; } = null!;

        /// <summary>
        /// Stabil, deterministisk nyckel inom leverantörsrelationen.
        /// <para>Kan exempelvis byggas från leverantörens SKU eller en hash av:
        /// normaliserat namn + format + biologiska källor.</para>
        /// </summary>
        public string OfferingKey { get; set; } = null!;

        /// <summary>
        /// Leverantörens eller systemets visningsnamn för erbjudandet.
        /// </summary>
        public string Name { get; set; } = null!;

        public string NormalizedName { get; set; } = null!;

        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Leverantörens eget artikelnummer.
        /// </summary>
        public string? SupplierSku { get; set; }

        public string? NormalizedSupplierSku { get; set; }

        /// <summary>
        /// Formatet exakt så som det angavs av leverantören.
        /// <para>Exempel:<br />
        /// fine powder<br />
        /// freeze-dried powder<br />
        /// IQF frozen<br />
        /// air dried</para>
        /// </summary>
        public string? DeclaredFormat { get; set; }

        /// <summary>
        /// Normaliserat terminalt format.
        /// <para>Exempel:<br />
        /// powder<br />
        /// dried-material<br />
        /// frozen-material<br />
        /// extract</para>
        /// <para>Null tills formatet har kunnat mappas.</para>
        /// </summary>
        public int? OfferedMaterialTypeId { get; set; }

        public MaterialType? OfferedMaterialType { get; set; }

        /// <summary>
        /// Det kanoniska Material som erbjudandet har mappats till.
        /// <para>Exempel:<br />
        /// Alaria esculenta powder</para>
        /// <para>Null tills art, format och övrig identitet är tillräckligt
        /// säkra för en kanonisk materialmappning.</para>
        /// </summary>
        public Guid? MappedMaterialId { get; set; }

        public Material? MappedMaterial { get; set; }

        public SourcingMappingStatus MappingStatus { get; set; }

        /// <summary>
        /// Konfidens mellan 0 och 1 för materialmappningen.
        /// </summary>
        public decimal? MappingConfidence { get; set; }

        public string? MappingNotes { get; set; }

        public SupplierOfferingStatus Status { get; set; }

        public DateTimeOffset FirstObservedUtc { get; set; }

        public DateTimeOffset LastObservedUtc { get; set; }

        public DateTimeOffset? LastVerifiedUtc { get; set; }

        public DateTimeOffset? CreateDateUtc { get; set; }

        public string? ActionName { get; set; }

        public DateTimeOffset? ActionDateUtc { get; set; }

        public Guid? ActionUserId { get; set; }

        /// <summary>
        /// Concurrency-token för offering-aggregatet.
        /// </summary>
        public uint RowVersion { get; set; }

        /// <summary>
        /// Vilka biologiska CRM-claims erbjudandet bygger på.
        /// </summary>
        public ICollection<SupplierMaterialOfferingBiologicalSource> BiologicalSourceLinks { get; set; } = [];

        public ICollection<SupplierMaterialOfferingOrigin> OriginLinks { get; set; } = [];

        public ICollection<SupplierMaterialOfferingCharacteristic> CharacteristicLinks { get; set; } = [];

        public ICollection<SupplierMaterialOfferingEvidence> Evidence { get; set; } = [];

        public ICollection<SupplierOfferingAvailability> Availabilities { get; set; } = [];

        public ICollection<SupplierOfferingSpecification> Specifications { get; set; } = [];

        public ICollection<SupplyCertification> Certifications { get; set; } = [];

        public ICollection<SupplyDocument> Documents { get; set; } = [];

        public ICollection<SupplyAnalysis> Analyses { get; set; } = [];

        public ICollection<InventoryLot> InventoryLots { get; set; } = [];
    }
}