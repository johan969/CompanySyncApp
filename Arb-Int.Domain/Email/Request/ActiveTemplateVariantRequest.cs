namespace Arb_Int.Domain.Email
{
    public record ActivateTemplateVariantRequest(
        Guid TemplateId,
        Guid VariantId,
        bool IsActive,
        string? Culture,
        bool RequiresPassword // eller härledd från template/assessment settings
    );
}