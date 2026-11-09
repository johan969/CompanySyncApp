using System.Linq.Expressions;

namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller EF Core-projections för organisationsrelationer.
    /// </summary>
    public static class OrganizationPersonProjections
    {
        /// <summary>
        /// Projekterar en organisationsrelation direkt till response-objektet.
        /// </summary>
        public static Expression<Func<OrganizationPerson, OrganizationPersonResponse>> Details => relationship => new OrganizationPersonResponse
        {
            Id = relationship.Id,
            OrganizationId = relationship.OrganizationId,
            OrganizationName = relationship.Organization.Name,
            PersonId = relationship.PersonId,
            PersonName = relationship.Person.Name,
            RelationshipType = relationship.RelationshipType,
            JobTitle = relationship.JobTitle,
            Department = relationship.Department,
            IsPrimaryContact = relationship.IsPrimaryContact,
            ValidFromUtc = relationship.ValidFromUtc,
            ValidToUtc = relationship.ValidToUtc,
            Notes = relationship.Notes,

            Functions = relationship.FunctionLinks
                .OrderBy(x => x.ContactFunctionType.SortOrder)
                .ThenBy(x => x.ContactFunctionType.Code)
                .Select(x => new ContactFunctionTypeResponse
                {
                    Id = x.ContactFunctionType.Id,
                    Code = x.ContactFunctionType.Code,
                    SortOrder = x.ContactFunctionType.SortOrder
                })
                .ToList()
        };
    }
}