using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Email
{
    public class EmailTemplate : IEntity<Guid>
    {
        public Guid Id { get; set; }

        // Stabil nyckel per scope/owner, t.ex. "assessment.invitation"
        public string Key { get; set; } = null!;

        public EmailTemplateScope? Scope { get; set; }
        public EmailTemplateKind Kind { get; set; }

        // Owner beroende på scope
        public Guid? OrganisationId { get; set; }
        public Guid? UserId { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<EmailTemplateVariant> Variants { get; set; } = new List<EmailTemplateVariant>();
    }
}