using FluentResults;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Hanterar jurisdictions som ägs av en certification scheme-version.
    /// </summary>
    public interface ICertificationSchemeJurisdictionService
    {
        /// <summary>
        /// Lägger till en jurisdiction och returnerar den uppdaterade scheme-versionen.
        /// </summary>
        Task<Result<CertificationSchemeResponse>> AddAsync(int certificationSchemeId, CreateCertificationSchemeJurisdictionRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uppdaterar jurisdiction-metadata utan att ändra scheme eller code.
        /// </summary>
        Task<Result<CertificationSchemeResponse>> UpdateAsync(int certificationSchemeJurisdictionId, UpdateCertificationSchemeJurisdictionRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tar bort en jurisdiction från scheme-versionen.
        /// </summary>
        Task<Result<CertificationSchemeResponse>> DeleteAsync(int certificationSchemeJurisdictionId, uint rowVersion, uint certificationSchemeRowVersion, CancellationToken cancellationToken = default);
    }
}
