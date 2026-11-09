using Arb_Int.Domain.Services.Common;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Anger filtrering, sortering och paginering för certification schemes.
    /// </summary>
    public sealed class CertificationSchemeQueryRequest
    {
        /// <summary>
        /// Fritext som matchas mot code, name, description, standard owner och standard version.
        /// </summary>
        public string? Search { get; init; }

        /// <summary>
        /// Begränsar resultatet till en certification scheme-kind.
        /// </summary>
        public CertificationSchemeKind? Kind { get; init; }

        /// <summary>
        /// Anger om inaktiva schemes ska inkluderas.
        /// </summary>
        public bool IncludeInactive { get; init; }

        /// <summary>
        /// Anger sorteringsfält.
        /// </summary>
        public CertificationSchemeSortField SortBy { get; init; } = CertificationSchemeSortField.Name;

        /// <summary>
        /// Anger sorteringsriktning.
        /// </summary>
        public SortDirection SortDirection { get; init; } = SortDirection.Ascending;

        /// <summary>
        /// Sidnummer med start på 1.
        /// </summary>
        public int Page { get; init; } = 1;

        /// <summary>
        /// Maximalt antal poster per sida.
        /// </summary>
        public int PageSize { get; init; } = 100;
    }

    /// <summary>
    /// Anger tillgängliga sorteringsfält för certification schemes.
    /// </summary>
    public enum CertificationSchemeSortField
    {
        /// <summary>
        /// Sorterar på scheme-namn.
        /// </summary>
        Name = 0,

        /// <summary>
        /// Sorterar på scheme-code.
        /// </summary>
        Code = 1,

        /// <summary>
        /// Sorterar på kind.
        /// </summary>
        Kind = 2,

        /// <summary>
        /// Sorterar på standard owner.
        /// </summary>
        StandardOwner = 3,

        /// <summary>
        /// Sorterar på standard version.
        /// </summary>
        StandardVersion = 4,

        /// <summary>
        /// Sorterar på active-status.
        /// </summary>
        IsActive = 5
    }
}
