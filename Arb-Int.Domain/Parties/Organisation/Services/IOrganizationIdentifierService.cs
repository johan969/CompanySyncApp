using FluentResults;

namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Hanterar identifierare för organisationer.
    /// </summary>
    public interface IOrganizationIdentifierService
    {
        /// <summary>
        /// Hämtar organisationens identifierare.
        /// </summary>
        Task<Result<IReadOnlyList<OrganizationIdentifierResponse>>> GetAsync(Guid organizationId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Hämtar en enskild organisationsidentifierare.
        /// </summary>
        Task<Result<OrganizationIdentifierResponse>> GetByIdAsync(Guid identifierId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lägger till en identifierare på en organisation.
        /// </summary>
        Task<Result<OrganizationIdentifierResponse>> CreateAsync(Guid organizationId, CreateOrganizationIdentifierRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uppdaterar en befintlig organisationsidentifierare.
        /// </summary>
        Task<Result<OrganizationIdentifierResponse>> UpdateAsync(Guid identifierId, UpdateOrganizationIdentifierRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tar bort en organisationsidentifierare.
        /// </summary>
        Task<Result> DeleteAsync(Guid identifierId, CancellationToken cancellationToken = default);
    }
}