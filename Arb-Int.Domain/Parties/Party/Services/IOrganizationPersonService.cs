using FluentResults;

namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Hanterar personers relationer till organisationer och deras funktioner.
    /// </summary>
    public interface IOrganizationPersonService
    {
        /// <summary>
        /// Hämtar organisationens personrelationer.
        /// </summary>
        Task<Result<IReadOnlyList<OrganizationPersonResponse>>> GetByOrganizationAsync(Guid organizationId, bool includeHistory = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Hämtar personens organisationsrelationer.
        /// </summary>
        Task<Result<IReadOnlyList<OrganizationPersonResponse>>> GetByPersonAsync(Guid personId, bool includeHistory = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Hämtar en enskild organisationsrelation.
        /// </summary>
        Task<Result<OrganizationPersonResponse>> GetByIdAsync(Guid organizationPersonId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Hämtar tillgängliga funktionstyper för organisationskontakter.
        /// </summary>
        Task<Result<IReadOnlyList<ContactFunctionTypeResponse>>> GetContactFunctionTypesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Skapar en ny relation mellan en person och en organisation.
        /// </summary>
        Task<Result<OrganizationPersonResponse>> CreateAsync(Guid organizationId, CreateOrganizationPersonRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uppdaterar metadata för en aktiv organisationsrelation.
        /// </summary>
        Task<Result<OrganizationPersonResponse>> UpdateAsync(Guid organizationPersonId, UpdateOrganizationPersonRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Ersätter funktionerna för en aktiv organisationsrelation.
        /// </summary>
        Task<Result<OrganizationPersonResponse>> SetFunctionsAsync(Guid organizationPersonId, SetOrganizationPersonFunctionsRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Avslutar en aktiv organisationsrelation och dess aktiva kontextuella kontaktpunkter.
        /// </summary>
        Task<Result> EndAsync(Guid organizationPersonId, EndOrganizationPersonRequest request, CancellationToken cancellationToken = default);
    }
}