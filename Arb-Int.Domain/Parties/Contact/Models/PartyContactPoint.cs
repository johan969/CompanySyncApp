using Arb_Int.Domain.Base;
using Arb_Int.Domain.Worker;

namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// PartyContactPoint kan tillhöra både en organisation och en person.
    /// <para>För personer finns dessutom ett valfritt OrganizationPersonId. Det gör att det går att skilja mellan:<br />
    /// Personens privata e-post:<br />
    /// - anna@example.com<br />
    /// Personens arbetsmail hos Företag A:<br />
    /// - anna@foretag-a.se<br />
    /// Personens arbetsmail hos Företag B:<br />
    /// - anna@foretag-b.se</para>
    /// </summary>
    public class PartyContactPoint : IEntity<Guid>, ISystemVersioned
    {
        public Guid Id { get; set; }

        public Guid PartyId { get; set; }
        public Party Party { get; set; } = null!;

        /// <summary>
        /// När detta är satt gäller kontaktuppgiften specifikt personens
        /// relation till en viss organisation.
        /// <para>PartyId måste då motsvara OrganizationPerson.PersonId.
        /// Detta säkerställs genom en sammansatt foreign key.</para>
        /// </summary>
        public Guid? OrganizationPersonId { get; set; }
        public OrganizationPerson? OrganizationPerson { get; set; }

        public ContactPointKind Kind { get; set; }

        /// <summary>
        /// Exempelvis "Växel", "Arbetsmail", "Privat mobil" eller "LinkedIn".
        /// </summary>
        public string? Label { get; set; }

        public string Value { get; set; } = null!;

        /// <summary>
        /// Exempel:
        /// <para>Email: trim + lowercase<br />
        /// Telefon: E.164-format<br />
        /// URL: kanonisk URI</para>
        /// </summary>
        public string NormalizedValue { get; set; } = null!;

        public string? Extension { get; set; }

        public bool IsPrimary { get; set; }

        public bool IsVerified { get; set; }

        public ContactPermissionStatus PermissionStatus { get; set; }

        public DateTimeOffset ValidFromUtc { get; set; }

        public DateTimeOffset? ValidToUtc { get; set; }

        public string? Notes { get; set; }

        public ICollection<ScheduledTask> ScheduledTaskLinks { get; set; } = [];
    }
}