using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Articles
{
    /// <summary>
    /// Exempel på processType.Codes:
    /// <para>drying<br />
    /// milling<br />
    /// pressing<br />
    /// extraction<br />
    /// refining<br />
    /// fermentation<br />
    /// blending<br />
    /// encapsulation<br />
    /// packaging<br />
    /// fractionation<br />
    /// filtration</para>
    /// </summary>
    public class ProcessType : NamedReferenceEntity, IEntity<int>, ISortOrder
    {
        public int Id { get; set; }
        public int SortOrder { get; set; }

        public ICollection<MaterialTransformation> Transformations { get; set; } = [];
    }
}