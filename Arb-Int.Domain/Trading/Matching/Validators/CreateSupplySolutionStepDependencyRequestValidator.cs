using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar nya dependencies mellan supply-solution-steps.
/// </summary>
public sealed class CreateSupplySolutionStepDependencyRequestValidator : AbstractValidator<CreateSupplySolutionStepDependencyRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för riktade dependency-edges.
    /// </summary>
    public CreateSupplySolutionStepDependencyRequestValidator()
    {
        RuleFor(x => x.PredecessorStepId).NotEmpty();
        RuleFor(x => x.SuccessorStepId).NotEmpty();
        RuleFor(x => x.Notes).MaximumLength(4096);

        RuleFor(x => x)
            .Must(x => x.PredecessorStepId != x.SuccessorStepId)
            .WithMessage("A SupplySolutionStep cannot depend on itself.");
    }
}