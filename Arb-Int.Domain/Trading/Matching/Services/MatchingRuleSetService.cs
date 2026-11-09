using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Services.Common;
using Arb_Int.Domain.Trading.Projections;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar regelverksroten och dess versionshistorik.
/// </summary>
public sealed class MatchingRuleSetService : IMatchingRuleSetService
{
    private readonly Arb_IntDbContext _db;
    private readonly IValidationService _validationService;
    private readonly ICanonicalJsonService _canonicalJsonService;

    /// <summary>
    /// Skapar servicen med databas-, validerings- och JSON-beroenden.
    /// </summary>
    public MatchingRuleSetService(Arb_IntDbContext db, IValidationService validationService, ICanonicalJsonService canonicalJsonService)
    {
        _db = db;
        _validationService = validationService;
        _canonicalJsonService = canonicalJsonService;
    }

    /// <summary>
    /// Hämtar ett regelverk med samtliga versioner.
    /// </summary>
    public async Task<Result<MatchingRuleSetResponse>> GetByIdAsync(Guid matchingRuleSetId, CancellationToken cancellationToken = default)
    {
        var response = await ProjectAsync(matchingRuleSetId, cancellationToken);

        return response is null
            ? Result.Fail<MatchingRuleSetResponse>(new NotFoundError(nameof(MatchingRuleSet), matchingRuleSetId))
            : Result.Ok(response);
    }

    /// <summary>
    /// Hämtar regelverk i stabil kodordning.
    /// </summary>
    public async Task<Result<IReadOnlyList<MatchingRuleSetResponse>>> GetAllAsync(bool includeRetired = false, CancellationToken cancellationToken = default)
    {
        IQueryable<MatchingRuleSet> query = _db.Set<MatchingRuleSet>().AsNoTracking();

        if (!includeRetired)
        {
            query = query.Where(x => x.Status != MatchingRuleSetStatus.Retired);
        }

        var items = await query
            .OrderBy(x => x.Code)
            .Select(MatchingProjections.RuleSetDetails)
            .ToListAsync(cancellationToken);

        return Result.Ok<IReadOnlyList<MatchingRuleSetResponse>>(items);
    }

    /// <summary>
    /// Skapar regelverksroten och version 1 som opublicerad draft.
    /// </summary>
    public async Task<Result<MatchingRuleSetResponse>> CreateAsync(CreateMatchingRuleSetRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<MatchingRuleSetResponse>(validation.Errors);
        }

        var code = request.Code.Trim();

        if (await _db.Set<MatchingRuleSet>().AsNoTracking().AnyAsync(x => x.Code == code, cancellationToken))
        {
            return Result.Fail<MatchingRuleSetResponse>(new ConflictError($"MatchingRuleSet code '{code}' already exists."));
        }

        var configuration = _canonicalJsonService.CanonicalizeAndHash(request.ConfigurationJson);

        var ruleSet = new MatchingRuleSet
        {
            Id = Guid.CreateVersion7(),
            Code = code,
            Name = request.Name.Trim(),
            Description = TrimToNull(request.Description) ?? string.Empty,
            Status = MatchingRuleSetStatus.Draft,
            LatestVersionNumber = 1
        };

        var version = new MatchingRuleSetVersion
        {
            Id = Guid.CreateVersion7(),
            MatchingRuleSetId = ruleSet.Id,
            VersionNumber = 1,
            Description = TrimToNull(request.InitialVersionDescription),
            ConfigurationJson = configuration.Json,
            ConfigurationHash = configuration.Hash,
            EffectiveFromUtc = request.EffectiveFromUtc,
            EffectiveToUtc = request.EffectiveToUtc
        };

        ruleSet.Versions.Add(version);
        _db.Set<MatchingRuleSet>().Add(ruleSet);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            return Result.Fail<MatchingRuleSetResponse>(new ConflictError($"MatchingRuleSet code '{code}' already exists."));
        }

        return Result.Ok(await ProjectRequiredAsync(ruleSet.Id, cancellationToken));
    }

    /// <summary>
    /// Uppdaterar endast metadata på regelverksroten.
    /// </summary>
    public async Task<Result<MatchingRuleSetResponse>> UpdateAsync(Guid matchingRuleSetId, UpdateMatchingRuleSetRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<MatchingRuleSetResponse>(validation.Errors);
        }

        var ruleSet = await _db.Set<MatchingRuleSet>().SingleOrDefaultAsync(x => x.Id == matchingRuleSetId, cancellationToken);

        if (ruleSet is null)
        {
            return Result.Fail<MatchingRuleSetResponse>(new NotFoundError(nameof(MatchingRuleSet), matchingRuleSetId));
        }

        if (ruleSet.Status == MatchingRuleSetStatus.Retired)
        {
            return Result.Fail<MatchingRuleSetResponse>(new ConflictError("A retired MatchingRuleSet cannot be modified."));
        }

        ruleSet.Name = request.Name.Trim();
        ruleSet.Description = TrimToNull(request.Description) ?? string.Empty;

        _db.Entry(ruleSet).Property(x => x.RowVersion).OriginalValue = request.RowVersion;

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Fail<MatchingRuleSetResponse>(new ConcurrencyError(nameof(MatchingRuleSet), ruleSet.Id));
        }

        return Result.Ok(await ProjectRequiredAsync(ruleSet.Id, cancellationToken));
    }

    /// <summary>
    /// Uppdaterar configuration och effective-period på en opublicerad version.
    /// </summary>
    public async Task<Result<MatchingRuleSetResponse>> UpdateVersionAsync(Guid matchingRuleSetVersionId, UpdateMatchingRuleSetVersionRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<MatchingRuleSetResponse>(validation.Errors);
        }

        var version = await _db.Set<MatchingRuleSetVersion>()
            .Include(x => x.MatchingRuleSet)
            .SingleOrDefaultAsync(x => x.Id == matchingRuleSetVersionId, cancellationToken);

        if (version is null)
        {
            return Result.Fail<MatchingRuleSetResponse>(new NotFoundError(nameof(MatchingRuleSetVersion), matchingRuleSetVersionId));
        }

        if (version.MatchingRuleSet.Status == MatchingRuleSetStatus.Retired)
        {
            return Result.Fail<MatchingRuleSetResponse>(new ConflictError("Versions of a retired MatchingRuleSet cannot be modified."));
        }

        if (version.PublishedAtUtc.HasValue)
        {
            return Result.Fail<MatchingRuleSetResponse>(new ConflictError("A published MatchingRuleSetVersion is immutable."));
        }

        var configuration = _canonicalJsonService.CanonicalizeAndHash(request.ConfigurationJson);

        version.Description = TrimToNull(request.Description);
        version.ConfigurationJson = configuration.Json;
        version.ConfigurationHash = configuration.Hash;
        version.EffectiveFromUtc = request.EffectiveFromUtc;
        version.EffectiveToUtc = request.EffectiveToUtc;

        _db.Entry(version).Property(x => x.RowVersion).OriginalValue = request.RowVersion;

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Fail<MatchingRuleSetResponse>(new ConcurrencyError(nameof(MatchingRuleSetVersion), version.Id));
        }

        return Result.Ok(await ProjectRequiredAsync(version.MatchingRuleSetId, cancellationToken));
    }

    /// <summary>
    /// Klonar senaste publicerade version till nästa opublicerade version.
    /// </summary>
    public async Task<Result<MatchingRuleSetResponse>> CreateNextVersionAsync(Guid matchingRuleSetId, CreateNextMatchingRuleSetVersionRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<MatchingRuleSetResponse>(validation.Errors);
        }

        var ruleSet = await _db.Set<MatchingRuleSet>()
            .Include(x => x.Versions)
            .SingleOrDefaultAsync(x => x.Id == matchingRuleSetId, cancellationToken);

        if (ruleSet is null)
        {
            return Result.Fail<MatchingRuleSetResponse>(new NotFoundError(nameof(MatchingRuleSet), matchingRuleSetId));
        }

        if (ruleSet.Status != MatchingRuleSetStatus.Active)
        {
            return Result.Fail<MatchingRuleSetResponse>(new ConflictError("Only an active MatchingRuleSet can create another version."));
        }

        if (ruleSet.Versions.Any(x => !x.PublishedAtUtc.HasValue))
        {
            return Result.Fail<MatchingRuleSetResponse>(new ConflictError("MatchingRuleSet already has an unpublished version."));
        }

        var source = ruleSet.Versions.SingleOrDefault(x => x.VersionNumber == ruleSet.LatestVersionNumber);

        if (source is null || !source.PublishedAtUtc.HasValue)
        {
            return Result.Fail<MatchingRuleSetResponse>(new ConflictError("LatestVersionNumber does not reference a published MatchingRuleSetVersion."));
        }

        var nextVersionNumber = ruleSet.LatestVersionNumber + 1;

        var version = new MatchingRuleSetVersion
        {
            Id = Guid.CreateVersion7(),
            MatchingRuleSetId = ruleSet.Id,
            VersionNumber = nextVersionNumber,
            Description = source.Description,
            ConfigurationJson = source.ConfigurationJson,
            ConfigurationHash = source.ConfigurationHash,
            EffectiveFromUtc = null,
            EffectiveToUtc = null
        };

        ruleSet.LatestVersionNumber = nextVersionNumber;
        ruleSet.Versions.Add(version);

        _db.Entry(ruleSet).Property(x => x.RowVersion).OriginalValue = request.MatchingRuleSetRowVersion;

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Fail<MatchingRuleSetResponse>(new ConcurrencyError(nameof(MatchingRuleSet), ruleSet.Id));
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            return Result.Fail<MatchingRuleSetResponse>(new ConflictError("Another MatchingRuleSetVersion was created concurrently."));
        }

        return Result.Ok(await ProjectRequiredAsync(ruleSet.Id, cancellationToken));
    }

    /// <summary>
    /// Publicerar en immutable regelversion och aktiverar regelverket vid första publiceringen.
    /// </summary>
    public async Task<Result<MatchingRuleSetResponse>> PublishVersionAsync(Guid matchingRuleSetVersionId, PublishMatchingRuleSetVersionRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<MatchingRuleSetResponse>(validation.Errors);
        }

        var version = await _db.Set<MatchingRuleSetVersion>()
            .Include(x => x.MatchingRuleSet)
            .SingleOrDefaultAsync(x => x.Id == matchingRuleSetVersionId, cancellationToken);

        if (version is null)
        {
            return Result.Fail<MatchingRuleSetResponse>(new NotFoundError(nameof(MatchingRuleSetVersion), matchingRuleSetVersionId));
        }

        var ruleSet = version.MatchingRuleSet;

        if (ruleSet.Status == MatchingRuleSetStatus.Retired)
        {
            return Result.Fail<MatchingRuleSetResponse>(new ConflictError("A retired MatchingRuleSet cannot publish new versions."));
        }

        if (version.PublishedAtUtc.HasValue)
        {
            return Result.Fail<MatchingRuleSetResponse>(new ConflictError("MatchingRuleSetVersion is already published."));
        }

        if (version.VersionNumber != ruleSet.LatestVersionNumber)
        {
            return Result.Fail<MatchingRuleSetResponse>(new ConflictError("Only the latest MatchingRuleSetVersion can be published."));
        }

        var computedHash = _canonicalJsonService.ComputeHash(version.ConfigurationJson);

        if (!string.Equals(computedHash, version.ConfigurationHash, StringComparison.OrdinalIgnoreCase))
        {
            return Result.Fail<MatchingRuleSetResponse>(new ConflictError("MatchingRuleSetVersion ConfigurationHash does not match ConfigurationJson."));
        }

        _db.Entry(ruleSet).Property(x => x.RowVersion).OriginalValue = request.MatchingRuleSetRowVersion;
        _db.Entry(version).Property(x => x.RowVersion).OriginalValue = request.MatchingRuleSetVersionRowVersion;

        version.PublishedAtUtc = DateTimeOffset.UtcNow;
        version.PublishedByUserId = request.PublishedByUserId;

        if (ruleSet.Status == MatchingRuleSetStatus.Draft)
        {
            ruleSet.Status = MatchingRuleSetStatus.Active;
        }

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            var error = exception.Entries.Any(x => x.Entity is MatchingRuleSet)
                ? new ConcurrencyError(nameof(MatchingRuleSet), ruleSet.Id)
                : new ConcurrencyError(nameof(MatchingRuleSetVersion), version.Id);

            return Result.Fail<MatchingRuleSetResponse>(error);
        }

        return Result.Ok(await ProjectRequiredAsync(ruleSet.Id, cancellationToken));
    }

    /// <summary>
    /// Pensionerar ett regelverk utan att påverka historiska versioner eller runs.
    /// </summary>
    public async Task<Result<MatchingRuleSetResponse>> RetireAsync(Guid matchingRuleSetId, RetireMatchingRuleSetRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<MatchingRuleSetResponse>(validation.Errors);
        }

        var ruleSet = await _db.Set<MatchingRuleSet>().SingleOrDefaultAsync(x => x.Id == matchingRuleSetId, cancellationToken);

        if (ruleSet is null)
        {
            return Result.Fail<MatchingRuleSetResponse>(new NotFoundError(nameof(MatchingRuleSet), matchingRuleSetId));
        }

        if (ruleSet.Status == MatchingRuleSetStatus.Retired)
        {
            return Result.Ok(await ProjectRequiredAsync(ruleSet.Id, cancellationToken));
        }

        _db.Entry(ruleSet).Property(x => x.RowVersion).OriginalValue = request.RowVersion;

        ruleSet.Status = MatchingRuleSetStatus.Retired;

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Fail<MatchingRuleSetResponse>(new ConcurrencyError(nameof(MatchingRuleSet), ruleSet.Id));
        }

        return Result.Ok(await ProjectRequiredAsync(ruleSet.Id, cancellationToken));
    }

    /// <summary>
    /// Projekterar ett regelverk från databasen.
    /// </summary>
    private Task<MatchingRuleSetResponse?> ProjectAsync(Guid matchingRuleSetId, CancellationToken cancellationToken)
    {
        return _db.Set<MatchingRuleSet>()
            .AsNoTracking()
            .Where(x => x.Id == matchingRuleSetId)
            .Select(MatchingProjections.RuleSetDetails)
            .SingleOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Projekterar ett sparat regelverk och kastar om det oväntat saknas.
    /// </summary>
    private async Task<MatchingRuleSetResponse> ProjectRequiredAsync(Guid matchingRuleSetId, CancellationToken cancellationToken)
    {
        return await ProjectAsync(matchingRuleSetId, cancellationToken)
            ?? throw new InvalidOperationException($"MatchingRuleSet '{matchingRuleSetId}' was saved but could not be projected.");
    }

    /// <summary>
    /// Kontrollerar om ett databasfel orsakades av en unique constraint.
    /// </summary>
    private static bool IsUniqueViolation(DbUpdateException exception)
    {
        return exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };
    }

    /// <summary>
    /// Trimmar text och ersätter whitespace med null.
    /// </summary>
    private static string? TrimToNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}