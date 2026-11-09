using FluentValidation;

namespace Arb_Int.Domain.Crm.Validation
{
    /// <summary>
    /// Validerar requests som byter steg på affärsrelationer.
    /// </summary>
    public sealed class ChangeBusinessRelationshipStageRequestValidator : AbstractValidator<ChangeBusinessRelationshipStageRequest>
    {
        /// <summary>
        /// Konfigurerar reglerna för stegbyten.
        /// </summary>
        public ChangeBusinessRelationshipStageRequestValidator()
        {
            RuleFor(x => x.BusinessRelationshipStageId)
                .GreaterThan(0);

            RuleFor(x => x.ChangedByUserId)
                .NotEqual(Guid.Empty)
                .When(x => x.ChangedByUserId.HasValue);

            RuleFor(x => x.Notes)
                .MaximumLength(4096);
        }
    }
}