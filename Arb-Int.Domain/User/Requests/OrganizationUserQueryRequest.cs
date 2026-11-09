using Arb_Int.Domain.Parties;

namespace Arb_Int.Domain.User
{
    /// <summary>
    /// Innehåller filter för organisationens personer och användarkonton.
    /// </summary>
    public sealed class OrganizationUserQueryRequest
    {
        /// <summary>
        /// Fritextsökning på personnamn, e-post eller användarnamn.
        /// </summary>
        public string? Search { get; init; }

        /// <summary>
        /// Filtrerar på PersonUserAccount-status när värdet är satt.
        /// </summary>
        public PersonUserAccountStatus? AccountStatus { get; init; }

        /// <summary>
        /// Anger om personer utan användarkonto ska inkluderas.
        /// </summary>
        public bool IncludePeopleWithoutAccounts { get; init; } = true;

        /// <summary>
        /// Anger om avslutade OrganizationPerson-kopplingar ska inkluderas.
        /// </summary>
        public bool IncludeHistory { get; init; }
    }
}
