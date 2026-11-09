using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Articles
{
    public class BiologicalSourceName : IEntity<Guid>, INamed, INormalizedName
    {
        public Guid Id { get; set; }

        public Guid BiologicalSourceId { get; set; }
        public BiologicalSource BiologicalSource { get; set; } = null!;

        public BiologicalSourceNameType NameType { get; set; }

        public string Name { get; set; } = null!;

        public string NormalizedName { get; set; } = null!;

        /// <summary>
        /// BCP-47-kod eller "und" när språket är okänt.
        /// </summary>
        public string LanguageCode { get; set; } = "und";

        public bool IsPreferred { get; set; }
    }
}