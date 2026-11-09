using Arb_Int.Domain.Services.Common;
using FluentResults;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Hanterar supply certification-roten, validity, status och supporting document.
    /// </summary>
    public interface ISupplyCertificationService
    {
        /// <summary>
        /// Hämtar en supply certification.
        /// </summary>
        Task<Result<SupplyCertificationResponse>> GetByIdAsync(Guid supplyCertificationId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Söker, filtrerar, sorterar och paginerar supply certifications.
        /// </summary>
        Task<Result<PagedResult<SupplyCertificationResponse>>> QueryAsync(SupplyCertificationQueryRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Skapar en certification assertion mot exakt ett subject.
        /// </summary>
        Task<Result<SupplyCertificationResponse>> CreateAsync(CreateSupplyCertificationRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sätter certificationens validity-fönster.
        /// </summary>
        Task<Result<SupplyCertificationResponse>> SetValidityAsync(Guid supplyCertificationId, SetSupplyCertificationValidityRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Ändrar certificationens status.
        /// </summary>
        Task<Result<SupplyCertificationResponse>> ChangeStatusAsync(Guid supplyCertificationId, ChangeSupplyCertificationStatusRequest request, CancellationToken cancellationToken = default);


        /// <summary>
        /// Sätter eller rensar supporting supply document.
        /// </summary>
        Task<Result<SupplyCertificationResponse>> SetDocumentAsync(Guid supplyCertificationId, SetSupplyCertificationDocumentRequest request, CancellationToken cancellationToken = default);
    }
}
