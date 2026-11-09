using System.Linq.Expressions;

namespace Arb_Int.Domain.Location.Projections;

/// <summary>
/// Innehåller återanvändbara projektioner för vattenområden.
/// </summary>
public static class WaterProjections
{
    /// <summary>
    /// Projekterar ett vattenområde till en kompakt listrespons.
    /// </summary>
    public static Expression<Func<Water, WaterListResponse>> List => x => new WaterListResponse
    {
        Id = x.Id,
        Name = x.Name,
        Type = x.Type,
        FeatureType = x.FeatureType,
        ParentWaterId = x.ParentWaterId,
        ParentWaterName = x.ParentWater != null ? x.ParentWater.Name : null
    };
}
