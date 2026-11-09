using System.Linq.Expressions;

namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller EF Core-projections för organisationsidentifierare.
    /// </summary>
    public static class OrganizationIdentifierProjections
    {
        /// <summary>
        /// Projekterar en organisationsidentifierare direkt till response-objektet.
        /// </summary>
        public static Expression<Func<OrganizationIdentifier, OrganizationIdentifierResponse>> Details => identifier => new OrganizationIdentifierResponse
        {
            Id = identifier.Id,
            OrganizationId = identifier.OrganizationId,
            Scheme = identifier.Scheme,
            Value = identifier.Value,
            Issuer = identifier.Issuer,
            IssuingCountryId = identifier.IssuingCountryId,
            IsPrimary = identifier.IsPrimary
        };
    }
}