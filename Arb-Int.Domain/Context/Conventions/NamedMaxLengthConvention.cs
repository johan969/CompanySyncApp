using Arb_Int.Domain.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Arb_Int.Domain
{
    public sealed class NamedMaxLengthConvention : IModelFinalizingConvention
    {
        private readonly int _max;
        /// <summary>
        /// Sätter maxlängd på Name i INamed.Name
        /// </summary>
        /// <param name="max">Max-längd</param>
        public NamedMaxLengthConvention(int max = 256) => _max = max;

        public void ProcessModelFinalizing(
            IConventionModelBuilder modelBuilder,
            IConventionContext<IConventionModelBuilder> context)
        {
            foreach (var et in modelBuilder.Metadata.GetEntityTypes()
                         .Where(et => typeof(INamed).IsAssignableFrom(et.ClrType)))
            {
                var prop = et.FindProperty(nameof(INamed.Name));
                if (prop?.ClrType == typeof(string))
                {
                    prop.Builder.HasMaxLength(_max);
                    prop.Builder.IsRequired(true);
                }
            }
        }
    }
}