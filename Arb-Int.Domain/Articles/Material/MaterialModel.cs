using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Articles
{
    public class Material :
        IEntity<Guid>,
        INamed,
        IDescribable,
        ICreateTracker,
        IActionTracker
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string Slug { get; set; } = null!;

        public string Description { get; set; } = string.Empty;

        public string? Notes { get; set; }

        public int MaterialTypeId { get; set; }
        public MaterialType MaterialType { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public DateTimeOffset? CreateDateUtc { get; set; }

        public string? ActionName { get; set; }

        public DateTimeOffset? ActionDateUtc { get; set; }

        public Guid? ActionUserId { get; set; }

        /// <summary>
        /// Direkta biologiska källor.
        /// <para>En icke-biologisk råvara har normalt inga sådana länkar.
        /// Förädlade material behöver normalt inte duplicera länkarna,
        /// eftersom källorna kan härledas via transformationskedjan.</para>
        /// </summary>
        public ICollection<MaterialBiologicalSource> BiologicalSourceLinks { get; set; } = [];

        public ICollection<MaterialCharacteristic> CharacteristicLinks { get; set; } = [];

        public ICollection<MaterialConstituent> ConstituentLinks { get; set; } = [];

        public ICollection<MaterialOrigin> OriginLinks { get; set; } = [];

        public ICollection<MaterialReference> ReferenceLinks { get; set; } = [];

        /// <summary>
        /// Transformationer där detta material används som input.
        /// </summary>
        public ICollection<MaterialTransformationInput> UsedAsInputIn { get; set; } = [];

        /// <summary>
        /// Den transformation som producerar materialet.
        /// <para>Null betyder att materialet är en rot/råvara eller att
        /// produktionsrelationen ännu inte har registrerats.</para>
        /// </summary>
        public MaterialTransformationOutput? ProductionOutput { get; set; }

        /// <summary>
        /// Flera SKU:er eller produkter kan sälja samma material.
        /// </summary>
        public ICollection<Product> Products { get; set; } = [];
    }
}