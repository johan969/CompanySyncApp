using FluentResults;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Hanterar resultat som ägs av en supply analysis.
    /// </summary>
    public interface ISupplyAnalysisResultService
    {
        /// <summary>
        /// Lägger till ett resultat och returnerar den uppdaterade analysen.
        /// </summary>
        Task<Result<SupplyAnalysisResponse>> AddAsync(Guid supplyAnalysisId, CreateSupplyAnalysisResultRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uppdaterar target-mapping, värden och verifieringsmetadata för ett resultat.
        /// </summary>
        Task<Result<SupplyAnalysisResponse>> UpdateAsync(Guid supplyAnalysisResultId, UpdateSupplyAnalysisResultRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tar bort ett resultat som korrigering och returnerar den uppdaterade analysen.
        /// </summary>
        Task<Result<SupplyAnalysisResponse>> DeleteAsync(Guid supplyAnalysisResultId, uint rowVersion, uint supplyAnalysisRowVersion, CancellationToken cancellationToken = default);
    }
}
