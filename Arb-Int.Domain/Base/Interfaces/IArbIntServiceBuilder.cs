
using Microsoft.Extensions.DependencyInjection;

namespace Arb_Int.Domain.Base
{
    public interface IArbIntServiceBuilder
    {
        IServiceCollection Services { get; }
    }
}