using FluentResults;

namespace Arb_Int.Domain.Crm
{
    /// <summary>
    /// Hanterar CRM-aktiviteter, status och deltagare.
    /// </summary>
    public interface ICrmActivityService
    {
        /// <summary>
        /// Hämtar en CRM-aktivitet.
        /// </summary>
        Task<Result<CrmActivityResponse>> GetByIdAsync(Guid crmActivityId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Söker, filtrerar och paginerar CRM-aktiviteter.
        /// </summary>
        Task<Result<PagedResult<CrmActivityListResponse>>> QueryAsync(CrmActivityQueryRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Skapar en CRM-aktivitet.
        /// </summary>
        Task<Result<CrmActivityResponse>> CreateAsync(CreateCrmActivityRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uppdaterar aktivitetens metadata utan att ändra status eller relationens kontext.
        /// </summary>
        Task<Result<CrmActivityResponse>> UpdateAsync(Guid crmActivityId, UpdateCrmActivityRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Ändrar aktivitetens status och hanterar CompletedUtc.
        /// </summary>
        Task<Result<CrmActivityResponse>> ChangeStatusAsync(Guid crmActivityId, ChangeCrmActivityStatusRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lägger till en deltagare på aktiviteten.
        /// </summary>
        Task<Result<CrmActivityResponse>> AddParticipantAsync(Guid crmActivityId, CreateCrmActivityParticipantRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Ändrar rollen för en aktivitetsdeltagare.
        /// </summary>
        Task<Result<CrmActivityResponse>> UpdateParticipantAsync(Guid participantId, UpdateCrmActivityParticipantRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tar bort en deltagare från aktiviteten.
        /// </summary>
        Task<Result<CrmActivityResponse>> DeleteParticipantAsync(Guid participantId, CancellationToken cancellationToken = default);
    }
}