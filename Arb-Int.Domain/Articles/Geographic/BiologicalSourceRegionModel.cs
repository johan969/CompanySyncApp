using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Articles
{
    /// <summary>
    /// BiologicalSourceRegion = var organismen förekommer, odlas eller kommersiellt förknippas
    /// </summary>
    public class BiologicalSourceRegion : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid BiologicalSourceId { get; set; }
        public BiologicalSource BiologicalSource { get; set; } = null!;

        public Guid GeographicLocationId { get; set; }
        public GeographicLocation GeographicLocation { get; set; } = null!;
    }
}