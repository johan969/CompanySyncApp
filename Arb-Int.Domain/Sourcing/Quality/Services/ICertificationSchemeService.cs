using Arb_Int.Domain.Services.Common;
using FluentResults;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Hanterar certification scheme reference-data och dess versionidentitet.
    /// </summary>
    public interface ICertificationSchemeService
    {
        /// <summary>
        /// Hämtar en scheme-version med jurisdictions.
        /// </summary>
        Task<Result<CertificationSchemeResponse>> GetByIdAsync(int certificationSchemeId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Söker, filtrerar, sorterar och paginerar certification schemes.
        /// </summary>
        Task<Result<PagedResult<CertificationSchemeListResponse>>> QueryAsync(CertificationSchemeQueryRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Skapar en ny certification scheme-version med immutable standardidentitet.
        /// </summary>
        Task<Result<CertificationSchemeResponse>> CreateAsync(CreateCertificationSchemeRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uppdaterar presentation och validity utan att ändra code, kind, standard owner eller standard version.
        /// </summary>
        Task<Result<CertificationSchemeResponse>> UpdateAsync(int certificationSchemeId, UpdateCertificationSchemeRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Aktiverar eller inaktiverar scheme-versionen utan fysisk delete.
        /// </summary>
        Task<Result<CertificationSchemeResponse>> SetActiveAsync(int certificationSchemeId, SetCertificationSchemeActiveRequest request, CancellationToken cancellationToken = default);
    }
}
