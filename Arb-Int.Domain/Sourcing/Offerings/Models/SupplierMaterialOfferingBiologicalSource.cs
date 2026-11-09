using Arb_Int.Domain.Base;
using Arb_Int.Domain.Crm;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Kopplar ett konkret leverantörserbjudande till ett biologiskt CRM-påstående.
    /// <para>Ett erbjudande kan vara en blandning av flera biologiska källor.</para>
    /// </summary>
    public class SupplierMaterialOfferingBiologicalSource :
        IEntity<Guid>
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Dupliceras medvetet för att kunna använda sammansatta
        /// foreign keys som säkerställer att erbjudandet och claimen
        /// tillhör samma BusinessRelationship.
        /// </summary>
        public Guid BusinessRelationshipId { get; set; }

        public Guid SupplierMaterialOfferingId { get; set; }

        public SupplierMaterialOffering SupplierMaterialOffering { get; set; } = null!;

        public Guid BusinessRelationshipBiologicalSourceClaimId { get; set; }

        public BusinessRelationshipBiologicalSourceClaim BiologicalSourceClaim { get; set; } = null!;

        public SupplierOfferingBiologicalSourceRole Role { get; set; }

        /// <summary>
        /// Valfri deklarerad andel i procent för ett blandat material.
        /// </summary>
        public decimal? ProportionPercent { get; set; }

        public string? Notes { get; set; }
    }
}