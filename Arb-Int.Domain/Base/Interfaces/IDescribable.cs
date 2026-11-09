using System.ComponentModel.DataAnnotations;

namespace Arb_Int.Domain.Base
{
    public interface IDescribable
    {
        [MaxLength(4096)]
        string Description { get; set; }
    }
}