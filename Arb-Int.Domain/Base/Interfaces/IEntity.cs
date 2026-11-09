using System.ComponentModel.DataAnnotations;

namespace Arb_Int.Domain.Base
{
    public interface IEntity<TKey>
            where TKey : IComparable<TKey>, IEquatable<TKey>
    {
        [Key]
        TKey Id { get; set; }

        public bool HasEmptyId()
        {
            return EqualityComparer<TKey>.Default.Equals(Id, default);
        }
    }
}
