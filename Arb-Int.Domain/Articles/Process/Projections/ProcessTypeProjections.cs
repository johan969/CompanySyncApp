using System.Linq.Expressions;

namespace Arb_Int.Domain.Articles.Projections
{
    /// <summary>
    /// Innehåller återanvändbar EF-projektion för process types.
    /// </summary>
    public static class ProcessTypeProjections
    {
        /// <summary>
        /// Projekterar en process type till response-modellen.
        /// </summary>
        public static Expression<Func<ProcessType, ProcessTypeResponse>> Details => processType => new ProcessTypeResponse
        {
            Id = processType.Id,
            Code = processType.Code,
            Name = processType.Name,
            Description = processType.Description,
            SortOrder = processType.SortOrder,
            IsActive = processType.IsActive
        };
    }
}
