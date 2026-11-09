using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;
using Arb_Int.Domain.Crm;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// En känd eller möjlig processkapacitet hos en supplier-relation.
    /// <para>Exempel:<br />
    /// - drying<br />
    /// - freeze-drying<br />
    /// - milling<br />
    /// - extraction<br />
    /// - fermentation<br />
    /// - refining</para>
    /// <para>Detta är inte en utförd MaterialTransformation.<br />
    /// Det beskriver vad motparten kan eller uppges kunna göra.</para>
    /// </summary>
    public class SupplierProcessCapability :
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

        public Guid BusinessRelationshipId { get; set; }

        public BusinessRelationship BusinessRelationship
        { get; set; } = null!;

        /// <summary>
        /// Deterministisk nyckel inom affärsrelationen.
        /// </summary>
        public string CapabilityKey { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string NormalizedName { get; set; } = null!;

        /// <summary>
        /// Processnamnet exakt som det stod i källan.
        /// <para>Exempel:<br />
        /// low-temperature seaweed drying<br />
        /// cryogenic milling<br />
        /// aqueous extraction</para>
        /// </summary>
        public string DeclaredProcessName { get; set; } = null!;

        public string NormalizedDeclaredProcessName { get; set; } = null!;

        /// <summary>
        /// Normaliserad process.<br />
        /// Null tills processnamnet har mappats.
        /// </summary>
        public int? ProcessTypeId { get; set; }

        public ProcessType? ProcessType { get; set; }

        /// <summary>
        /// Valfri begränsning för vilket format processen tar emot.
        /// </summary>
        public int? InputMaterialTypeId { get; set; }

        public MaterialType? InputMaterialType { get; set; }

        /// <summary>
        /// Valfri angivelse av normalt utgångsformat.
        /// </summary>
        public int? OutputMaterialTypeId { get; set; }

        public MaterialType? OutputMaterialType { get; set; }

        public string Description { get; set; } = string.Empty;

        public string? Notes { get; set; }

        public SourcingMappingStatus MappingStatus { get; set; }

        public decimal? MappingConfidence { get; set; }

        public SupplierProcessCapabilityStatus Status { get; set; }

        public SourcingVerificationStatus VerificationStatus { get; set; }

        public DateTimeOffset FirstObservedUtc { get; set; }

        public DateTimeOffset LastObservedUtc { get; set; }

        public DateTimeOffset? LastVerifiedUtc { get; set; }

        public DateTimeOffset? CreateDateUtc { get; set; }

        public string? ActionName { get; set; }

        public DateTimeOffset? ActionDateUtc { get; set; }

        public Guid? ActionUserId { get; set; }

        /// <summary>
        /// Concurrency-token för process capability-aggregatet.
        /// </summary>
        public uint RowVersion { get; set; }

        /// <summary>
        /// Valfria biologiska begränsningar.
        /// <para>Om listan är tom betraktas kapaciteten som generell eller
        /// ännu inte biologiskt avgränsad.</para>
        /// </summary>
        public ICollection<SupplierProcessCapabilityBiologicalSourceScope> BiologicalSourceScopes { get; set; } = [];

        public ICollection<SupplierProcessCapabilityEvidence> Evidence { get; set; } = [];

        public ICollection<SupplyCertification> Certifications { get; set; } = [];

        public ICollection<SupplyDocument> Documents { get; set; } = [];
    }
}