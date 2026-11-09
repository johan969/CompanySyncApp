using Arb_Int.Domain.Services.Common;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Anger filtrering, sortering och paginering för supplier process capabilities.
    /// </summary>
    public sealed class SupplierProcessCapabilityQueryRequest
    {
        /// <summary>
        /// Fritext som matchas mot capability key, namn, deklarerat processnamn, anteckningar,
        /// process- och materialtyper samt biologiska källor i capabilityns scope.
        /// </summary>
        public string? Search { get; init; }
        /// <summary>
        /// Begränsar resultatet till en capability-status.
        /// </summary>
        public SupplierProcessCapabilityStatus? Status { get; init; }
        /// <summary>
        /// Begränsar resultatet till en mapping-status.
        /// </summary>
        public SourcingMappingStatus? MappingStatus { get; init; }
        /// <summary>
        /// Begränsar resultatet till en verification-status.
        /// </summary>
        public SourcingVerificationStatus? VerificationStatus { get; init; }
        /// <summary>
        /// Begränsar resultatet till en normaliserad process.
        /// </summary>
        public int? ProcessTypeId { get; init; }
        /// <summary>
        /// Begränsar resultatet till en accepterad input-materialtyp.
        /// </summary>
        public int? InputMaterialTypeId { get; init; }
        /// <summary>
        /// Begränsar resultatet till en normal output-materialtyp.
        /// </summary>
        public int? OutputMaterialTypeId { get; init; }
        /// <summary>
        /// Begränsar resultatet till capabilities vars scope är kopplat till en
        /// normaliserad biologisk källa via en claim på samma affärsrelation.
        /// <para>Matchningen är exakt. En claim mappad till genusnivå matchar inte
        /// ett artfilter och omvänt.</para>
        /// </summary>
        public Guid? BiologicalSourceId { get; init; }
        /// <summary>
        /// Begränsar en organisationsvid sökning till en enskild affärsrelation.
        /// <para>Ignoreras när sökningen redan är scopad till en relation via routen.</para>
        /// </summary>
        public Guid? BusinessRelationshipId { get; init; }
        /// <summary>
        /// Anger sorteringsfält.
        /// </summary>
        public SupplierProcessCapabilitySortField SortBy { get; init; } = SupplierProcessCapabilitySortField.Name;
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
        public int PageSize { get; init; } = 50;
    }

    /// <summary>
    /// Anger tillgängliga sorteringsfält för supplier process capabilities.
    /// </summary>
    public enum SupplierProcessCapabilitySortField
    {
        Name = 0,
        Status = 1,
        MappingStatus = 2,
        VerificationStatus = 3,
        FirstObservedUtc = 4,
        LastObservedUtc = 5,
        LastVerifiedUtc = 6,
        CreateDateUtc = 7
    }
}
