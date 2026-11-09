using Arb_Int.Domain;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Email
{
    public class ActivateTemplateVariantRequestValidator : AbstractValidator<ActivateTemplateVariantRequest>
    {
        public ActivateTemplateVariantRequestValidator(
            Arb_IntDbContext db,
            IEmailTemplatePolicyRegistry policyRegistry)
        {
            RuleFor(x => x.TemplateId).NotEmpty();
            RuleFor(x => x.VariantId).NotEmpty();

            When(x => x.IsActive, () =>
            {
                RuleFor(x => x).CustomAsync(async (req, context, cancellationToken) =>
                {
                    var template = await db.Set<EmailTemplate>()
                        .AsNoTracking()
                        .FirstOrDefaultAsync(t => t.Id == req.TemplateId, cancellationToken);

                    if (template is null)
                    {
                        context.AddFailure("TemplateId", "Template not found.");
                        return;
                    }

                    var variant = await db.Set<EmailTemplateVariant>()
                        .AsNoTracking()
                        .FirstOrDefaultAsync(v => v.Id == req.VariantId && v.TemplateId == req.TemplateId, cancellationToken);

                    if (variant is null)
                    {
                        context.AddFailure("VariantId", "Variant not found.");
                        return;
                    }

                    var policy = policyRegistry.Get(template.Kind);
                    var policyContext = new EmailPolicyContext(
                        ContactId: null,
                        RequiresPassword: req.RequiresPassword);

                    var result = policy.ValidateTemplate(variant.SubjectTemplate, variant.BodyHtmlTemplate, policyContext);
                    foreach (var e in result.Errors)
                        context.AddFailure(e.Message);
                });
            });
        }
    }
}
