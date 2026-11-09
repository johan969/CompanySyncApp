using System.Linq.Expressions;

namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Innehåller EF Core-projections för kontakter på affärsrelationer.
    /// </summary>
    public static class BusinessRelationshipContactProjections
    {
        /// <summary>
        /// Projekterar en relationskontakt direkt till response-objektet.
        /// </summary>
        public static Expression<Func<BusinessRelationshipContact, BusinessRelationshipContactResponse>> Details => contact => new BusinessRelationshipContactResponse
        {
            Id = contact.Id,
            BusinessRelationshipId = contact.BusinessRelationshipId,
            OrganizationPersonId = contact.OrganizationPersonId,
            PersonId = contact.OrganizationPerson.PersonId,
            PersonName = contact.OrganizationPerson.Person.Name,
            JobTitle = contact.OrganizationPerson.JobTitle,
            Department = contact.OrganizationPerson.Department,
            OrganizationRelationshipType = contact.OrganizationPerson.RelationshipType,
            OrganizationRelationshipValidFromUtc = contact.OrganizationPerson.ValidFromUtc,
            OrganizationRelationshipValidToUtc = contact.OrganizationPerson.ValidToUtc,
            Role = contact.Role,
            IsPrimary = contact.IsPrimary,
            Notes = contact.Notes
        };
    }
}