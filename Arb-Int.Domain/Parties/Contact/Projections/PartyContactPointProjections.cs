using System.Linq.Expressions;

namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller EF Core-projections för PartyContactPoint.
    /// </summary>
    public static class PartyContactPointProjections
    {
        /// <summary>
        /// Projekterar en kontaktpunkt direkt till DTO.
        /// </summary>
        public static Expression<Func<PartyContactPoint, PartyContactPointResponse>> Details => contactPoint => new PartyContactPointResponse
        {
            Id = contactPoint.Id,
            PartyId = contactPoint.PartyId,
            OrganizationPersonId = contactPoint.OrganizationPersonId,
            OrganizationId = contactPoint.OrganizationPerson == null
                ? null
                : contactPoint.OrganizationPerson.OrganizationId,
            Kind = contactPoint.Kind,
            Label = contactPoint.Label,
            Value = contactPoint.Value,
            Extension = contactPoint.Extension,
            IsPrimary = contactPoint.IsPrimary,
            IsVerified = contactPoint.IsVerified,
            PermissionStatus = contactPoint.PermissionStatus,
            ValidFromUtc = contactPoint.ValidFromUtc,
            ValidToUtc = contactPoint.ValidToUtc,
            Notes = contactPoint.Notes
        };
    }
}