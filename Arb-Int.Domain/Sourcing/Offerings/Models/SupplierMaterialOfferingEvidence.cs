using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// En importerad källa eller observation som stöder att ett leverantörserbjudande finns.
    /// <para>Flera källor kan stödja samma kanoniska erbjudande.</para>
    /// </summary>
    public class SupplierMaterialOfferingEvidence : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid SupplierMaterialOfferingId { get; set; }

        public SupplierMaterialOffering SupplierMaterialOffering { get; set; } = null!;

        public SourcingEvidenceType EvidenceType { get; set; }

        /// <summary>
        /// Exempel:<br />
        /// supplier-csv<br />
        /// supplier-website<br />
        /// marketplace<br />
        /// legacy-mariadb<br />
        /// manual
        /// </summary>
        public string SourceSystem { get; set; } = null!;

        /// <summary>
        /// Stabil identifierare för just denna extraherade post i källsystemet.
        /// </summary>
        public string SourceRecordKey { get; set; } = null!;

        public string? SourceUrl { get; set; }

        public DateTimeOffset FirstObservedUtc { get; set; }

        public DateTimeOffset LastObservedUtc { get; set; }

        public string? Notes { get; set; }

        public string? ContentHash { get; set; }
    }
}