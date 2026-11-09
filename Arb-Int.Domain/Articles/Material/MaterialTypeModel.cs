using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Articles
{
    /// <summary>
    /// Exempel på MaterialTyper kan vara
    /// <para>Code                    Name                         IsRawMaterial<br />
    /// ----------------------  ---------------------------  -------------<br />
    /// whole-biomass = Whole biomass = true<br />
    /// seed = Seed = true<br />
    /// mineral-raw-material = Mineral raw material = true<br />
    /// dried-material = Dried material = false<br />
    /// powder = Powder = false<br />
    /// oil = Oil = false<br />
    /// extract = Extract = false<br />
    /// isolate = Isolate = false<br />
    /// blend = Blend = false<br />
    /// finished-material = Finished material = false</para>
    /// </summary>
    public class MaterialType : NamedReferenceEntity, IEntity<int>, ISortOrder
    {
        public int Id { get; set; }

        public int SortOrder { get; set; }

        /// <summary>
        /// Anger att material av denna typ är en rot i
        /// förädlingskedjan och inte ska ha en producerande transformation.
        /// </summary>
        public bool IsRawMaterial { get; set; }

        public int? ParentMaterialTypeId { get; set; }
        public MaterialType? ParentMaterialType { get; set; }

        public ICollection<MaterialType> Children { get; set; } = [];

        public ICollection<Material> Materials { get; set; } = [];
    }
}