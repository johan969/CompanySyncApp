using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Trading.Projections;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Läser genererade supply-solution-candidates och deras resultat.
/// </summary>
public interface ISupplySolutionCandidateService
{
    /// <summary>
    /// Hämtar en kandidat med lines, allocations och capacity evaluations.
    /// </summary>
    Task<Result<SupplySolutionCandidateResponse>> GetByIdAsync(Guid supplySolutionCandidateId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hämtar kandidatlistan för en matching run.
    /// </summary>
    Task<Result<IReadOnlyList<SupplySolutionCandidateListResponse>>> GetByRunAsync(Guid matchingRunId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementerar read-sidan för supply-solution-candidates.
/// </summary>
public sealed class SupplySolutionCandidateService : ISupplySolutionCandidateService
{
    private readonly Arb_IntDbContext _db;

    /// <summary>
    /// Skapar read-servicen med aktuell databascontext.
    /// </summary>
    public SupplySolutionCandidateService(Arb_IntDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Hämtar en kandidat med hela första resultatslicen.
    /// </summary>
    public async Task<Result<SupplySolutionCandidateResponse>> GetByIdAsync(Guid supplySolutionCandidateId, CancellationToken cancellationToken = default)
    {
        var response = await _db.Set<SupplySolutionCandidate>()
            .AsNoTracking()
            .Where(x => x.Id == supplySolutionCandidateId)
            .Select(SupplySolutionCandidateProjections.Details)
            .SingleOrDefaultAsync(cancellationToken);

        return response is null
            ? Result.Fail<SupplySolutionCandidateResponse>(new NotFoundError(nameof(SupplySolutionCandidate), supplySolutionCandidateId))
            : Result.Ok(response);
    }

    /// <summary>
    /// Hämtar kandidater för en matching run i kandidatnummerordning.
    /// </summary>
    public async Task<Result<IReadOnlyList<SupplySolutionCandidateListResponse>>> GetByRunAsync(Guid matchingRunId, CancellationToken cancellationToken = default)
    {
        var runExists = await _db.Set<MatchingRun>().AsNoTracking().AnyAsync(x => x.Id == matchingRunId, cancellationToken);

        if (!runExists)
        {
            return Result.Fail<IReadOnlyList<SupplySolutionCandidateListResponse>>(new NotFoundError(nameof(MatchingRun), matchingRunId));
        }

        var candidates = await _db.Set<SupplySolutionCandidate>()
            .AsNoTracking()
            .Where(x => x.MatchingRunId == matchingRunId)
            .OrderBy(x => x.CandidateNumber)
            .Select(SupplySolutionCandidateProjections.List)
            .ToListAsync(cancellationToken);

        return Result.Ok<IReadOnlyList<SupplySolutionCandidateListResponse>>(candidates);
    }
}