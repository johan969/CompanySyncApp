using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Articles
{
    public class MaterialTransformation : IEntity<Guid>, INamed
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public int ProcessTypeId { get; set; }
        public ProcessType ProcessType { get; set; } = null!;

        public string Description { get; set; } = string.Empty;

        public string? Notes { get; set; }

        /// <summary>
        /// Gör det möjligt att versionshantera ett recept eller en process.
        /// </summary>
        public string? Version { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<MaterialTransformationInput> Inputs { get; set; } = [];

        public ICollection<MaterialTransformationOutput> Outputs { get; set; } = [];
    }
}