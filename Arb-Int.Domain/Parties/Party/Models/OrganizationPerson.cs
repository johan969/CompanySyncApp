using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// En persons relation till en organisation.
    /// <para>JobTitle ligger här och inte på Person, eftersom samma person
    /// kan ha olika roller hos olika organisationer.</para>
    /// </summary>
    public class OrganizationPerson : IEntity<Guid>, ISystemVersioned
    {
        public Guid Id { get; set; }

        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; } = null!;

        public Guid PersonId { get; set; }
        public Person Person { get; set; } = null!;

        public OrganizationPersonRelationshipType RelationshipType { get; set; }

        public string? JobTitle { get; set; }

        public string? Department { get; set; }

        /// <summary>
        /// Organisationens generella huvudkontakt.
        /// <para>En specifik köpar- eller leverantörsrelation kan ha en annan
        /// huvudkontakt via BusinessRelationshipContact.</para>
        /// </summary>
        public bool IsPrimaryContact { get; set; }

        public DateTimeOffset? ValidFromUtc { get; set; }

        public DateTimeOffset? ValidToUtc { get; set; }

        public string? Notes { get; set; }

        public ICollection<OrganizationPersonFunction> FunctionLinks { get; set; } = [];

        public ICollection<PartyContactPoint> ContextualContactPoints { get; set; } = [];
    }
}