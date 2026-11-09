using System.Linq.Expressions;

namespace Arb_Int.Domain.Location.Projections;

/// <summary>
/// Innehåller återanvändbara projektioner för kontinenter.
/// </summary>
public static class ContinentProjections
{
    /// <summary>
    /// Projekterar en kontinent till referensrespons.
    /// </summary>
    public static Expression<Func<Continent, ContinentResponse>> Response => x => new ContinentResponse
    {
        Id = x.Id,
        Name = x.Name,
        ContinentCode = x.ContinentCode
    };
}
