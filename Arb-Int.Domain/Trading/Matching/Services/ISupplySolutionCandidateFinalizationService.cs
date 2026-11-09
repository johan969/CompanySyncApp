using FluentResults;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar readiness, immutable snapshot och verifiering av maskinellt genererade candidates.
/// </summary>
public interface ISupplySolutionCandidateFinalizationService
{
    /// <summary>
    /// Kontrollerar om kandidaten är komplett nog för att frysas.
    /// </summary>
    Task<Result<SupplySolutionCandidateFinalizationReadinessResponse>> GetReadinessAsync(Guid supplySolutionCandidateId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fryser kandidatens maskinella resultat till canonical JSON och SHA-256.
    /// </summary>
    Task<Result<SupplySolutionCandidateResponse>> FinalizeAsync(Guid supplySolutionCandidateId, FinalizeSupplySolutionCandidateRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifierar ett redan lagrat kandidat-snapshot.
    /// </summary>
    Task<Result<SupplySolutionCandidateSnapshotVerificationResponse>> VerifySnapshotAsync(Guid supplySolutionCandidateId, CancellationToken cancellationToken = default);
}