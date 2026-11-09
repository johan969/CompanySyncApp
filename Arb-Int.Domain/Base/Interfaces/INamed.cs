using System.ComponentModel.DataAnnotations;

namespace Arb_Int.Domain.Base
{
    public interface INamed
    {
        [MaxLength(255)]
        string Name { get; set; }
    }
}