using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Services.Common;
using Arb_Int.Domain.Trading.Projections;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar Draft proposal-options och använder proposal-revisionen som aggregate concurrency-root.
/// </summary>
public sealed class BuyerProposalOptionService : IBuyerProposalOptionService
{
    private readonly Arb_IntDbContext _db;
    private readonly IValidationService _validationService;
    private readonly IBuyerProposalReferenceValidator _referenceValidator;
    private readonly ICanonicalJsonService _canonicalJsonService;

    /// <summary>
    /// Skapar option-servicen.
    /// </summary>
    public BuyerProposalOptionService(
        Arb_IntDbContext db,
        IValidationService validationService,
        IBuyerProposalReferenceValidator referenceValidator,
        ICanonicalJsonService canonicalJsonService)
    {
        _db = db;
        _validationService = validationService;
        _referenceValidator = referenceValidator;
        _canonicalJsonService = canonicalJsonService;
    }

    /// <summary>
    /// Skapar ett Draft-option från en finaliserad och modererad candidate.
    /// </summary>
    public async Task<Result<BuyerProposalRevisionResponse>> CreateAsync(
        Guid buyerProposalRevisionId,
        CreateBuyerProposalOptionRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(
            request,
            cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<BuyerProposalRevisionResponse>(
                validation.Errors);
        }

        var revision = await _db.Set<BuyerProposalRevision>()
            .SingleOrDefaultAsync(
                x => x.Id == buyerProposalRevisionId,
                cancellationToken);

        if (revision is null)
        {
            return Result.Fail<BuyerProposalRevisionResponse>(
                new NotFoundError(
                    nameof(BuyerProposalRevision),
                    buyerProposalRevisionId));
        }

        var writableResult =
            ValidateRevisionIsWritable(revision);

        if (writableResult.IsFailed)
        {
            return Result.Fail<BuyerProposalRevisionResponse>(
                writableResult.Errors);
        }

        var candidateResult =
            await _referenceValidator.ValidateCandidateAsync(
                revision.BuyerRfqRevisionId,
                request.SupplySolutionCandidateId,
                DateTimeOffset.UtcNow,
                cancellationToken);

        if (candidateResult.IsFailed)
        {
            return Result.Fail<BuyerProposalRevisionResponse>(
                candidateResult.Errors);
        }

        var duplicateNumber = await _db.Set<BuyerProposalOption>()
            .AsNoTracking()
            .AnyAsync(
                x => x.BuyerProposalRevisionId == revision.Id
                    && x.OptionNumber == request.OptionNumber,
                cancellationToken);

        if (duplicateNumber)
        {
            return Result.Fail<BuyerProposalRevisionResponse>(
                new ConflictError(
                    $"OptionNumber '{request.OptionNumber}' already exists in the proposal revision."));
        }

        var option = new BuyerProposalOption
        {
            Id = Guid.CreateVersion7(),
            BuyerProposalRevisionId = revision.Id,
            SupplySolutionCandidateId = request.SupplySolutionCandidateId,
            OptionNumber = request.OptionNumber,
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            Status = BuyerProposalOptionStatus.Draft,
            TotalNetAmount = 0m,
            TotalTaxAmount = null,
            TotalGrossAmount = 0m,
            CurrencyId = revision.CurrencyId,
            EstimatedLeadTimeDays = request.EstimatedLeadTimeDays,
            BuyerFacingSummary = TrimToNull(request.BuyerFacingSummary),
            InternalMarginSnapshotJson = CanonicalizeOptionalJson(
                request.InternalMarginSnapshotJson)
        };

        TouchRevision(revision);

        _db.Set<BuyerProposalOption>()
            .Add(option);

        try
        {
            await _db.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Fail<BuyerProposalRevisionResponse>(
                new ConcurrencyError(
                    nameof(BuyerProposalRevision),
                    revision.Id));
        }
        catch (DbUpdateException exception)
            when (IsUniqueViolation(exception))
        {
            return Result.Fail<BuyerProposalRevisionResponse>(
                new ConflictError(
                    $"OptionNumber '{request.OptionNumber}' already exists in the proposal revision."));
        }

        return Result.Ok(
            await ProjectRequiredRevisionAsync(
                revision.Id,
                cancellationToken));
    }

    /// <summary>
    /// Uppdaterar ett Draft-option utan att ändra dess candidate-provenance.
    /// </summary>
    public async Task<Result<BuyerProposalRevisionResponse>> UpdateAsync(
        Guid buyerProposalOptionId,
        UpdateBuyerProposalOptionRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(
            request,
            cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<BuyerProposalRevisionResponse>(
                validation.Errors);
        }

        var option = await LoadOptionForWriteAsync(
            buyerProposalOptionId,
            cancellationToken);

        if (option is null)
        {
            return Result.Fail<BuyerProposalRevisionResponse>(
                new NotFoundError(
                    nameof(BuyerProposalOption),
                    buyerProposalOptionId));
        }

        var writableResult =
            ValidateOptionIsWritable(option);

        if (writableResult.IsFailed)
        {
            return Result.Fail<BuyerProposalRevisionResponse>(
                writableResult.Errors);
        }

        var duplicate = await _db.Set<BuyerProposalOption>()
            .AsNoTracking()
            .AnyAsync(
                x => x.BuyerProposalRevisionId == option.BuyerProposalRevisionId
                    && x.OptionNumber == request.OptionNumber
                    && x.Id != option.Id,
                cancellationToken);

        if (duplicate)
        {
            return Result.Fail<BuyerProposalRevisionResponse>(
                new ConflictError(
                    $"OptionNumber '{request.OptionNumber}' already exists in the proposal revision."));
        }

        _db.Entry(option)
            .Property(x => x.RowVersion)
            .OriginalValue = request.RowVersion;

        option.OptionNumber = request.OptionNumber;
        option.Name = request.Name.Trim();
        option.Description = request.Description.Trim();
        option.EstimatedLeadTimeDays = request.EstimatedLeadTimeDays;
        option.BuyerFacingSummary = TrimToNull(request.BuyerFacingSummary);
        option.InternalMarginSnapshotJson = CanonicalizeOptionalJson(
            request.InternalMarginSnapshotJson);

        TouchRevision(
            option.BuyerProposalRevision);

        try
        {
            await _db.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            return Result.Fail<BuyerProposalRevisionResponse>(
                CreateConcurrencyError(
                    exception,
                    nameof(BuyerProposalOption),
                    option.Id));
        }
        catch (DbUpdateException exception)
            when (IsUniqueViolation(exception))
        {
            return Result.Fail<BuyerProposalRevisionResponse>(
                new ConflictError(
                    $"OptionNumber '{request.OptionNumber}' already exists in the proposal revision."));
        }

        return Result.Ok(
            await ProjectRequiredRevisionAsync(
                option.BuyerProposalRevisionId,
                cancellationToken));
    }

    /// <summary>
    /// Tar bort ett Draft-option och dess lines.
    /// </summary>
    public async Task<Result<BuyerProposalRevisionResponse>> DeleteAsync(
        Guid buyerProposalOptionId,
        uint rowVersion,
        CancellationToken cancellationToken = default)
    {
        var option = await LoadOptionForWriteAsync(
            buyerProposalOptionId,
            cancellationToken);

        if (option is null)
        {
            return Result.Fail<BuyerProposalRevisionResponse>(
                new NotFoundError(
                    nameof(BuyerProposalOption),
                    buyerProposalOptionId));
        }

        var writableResult =
            ValidateOptionIsWritable(option);

        if (writableResult.IsFailed)
        {
            return Result.Fail<BuyerProposalRevisionResponse>(
                writableResult.Errors);
        }

        _db.Entry(option)
            .Property(x => x.RowVersion)
            .OriginalValue = rowVersion;

        var revisionId =
            option.BuyerProposalRevisionId;

        TouchRevision(
            option.BuyerProposalRevision);

        _db.Set<BuyerProposalOption>()
            .Remove(option);

        try
        {
            await _db.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            return Result.Fail<BuyerProposalRevisionResponse>(
                CreateConcurrencyError(
                    exception,
                    nameof(BuyerProposalOption),
                    option.Id));
        }

        return Result.Ok(
            await ProjectRequiredRevisionAsync(
                revisionId,
                cancellationToken));
    }

    /// <summary>
    /// Läser option tillsammans med parent-revision.
    /// </summary>
    private Task<BuyerProposalOption?> LoadOptionForWriteAsync(
        Guid optionId,
        CancellationToken cancellationToken)
    {
        return _db.Set<BuyerProposalOption>()
            .Include(x => x.BuyerProposalRevision)
            .SingleOrDefaultAsync(
                x => x.Id == optionId,
                cancellationToken);
    }

    /// <summary>
    /// Kontrollerar att revisionen fortfarande är redigerbar.
    /// </summary>
    private static Result ValidateRevisionIsWritable(
        BuyerProposalRevision revision)
    {
        if (revision.Status != BuyerProposalRevisionStatus.Draft
            || revision.IssuedAtUtc.HasValue
            || revision.SnapshotJson is not null
            || revision.SnapshotHash is not null)
        {
            return Result.Fail(
                new ConflictError(
                    "BuyerProposalRevision is immutable because it is no longer a clean Draft."));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Kontrollerar både parent-revision och option-status.
    /// </summary>
    private static Result ValidateOptionIsWritable(
        BuyerProposalOption option)
    {
        var revisionResult =
            ValidateRevisionIsWritable(
                option.BuyerProposalRevision);

        if (revisionResult.IsFailed)
        {
            return revisionResult;
        }

        if (option.Status != BuyerProposalOptionStatus.Draft)
        {
            return Result.Fail(
                new ConflictError(
                    $"BuyerProposalOption with status '{option.Status}' is immutable."));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Markerar parent-revisionen för aggregate-level optimistic concurrency.
    /// </summary>
    private static void TouchRevision(
        BuyerProposalRevision revision)
    {
        revision.ActionDateUtc =
            DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Canonicaliserar optional intern JSON innan lagring.
    /// </summary>
    private string? CanonicalizeOptionalJson(
        string? json)
    {
        return string.IsNullOrWhiteSpace(json)
            ? null
            : _canonicalJsonService.Canonicalize(json);
    }

    /// <summary>
    /// Översätter concurrency-fel till option eller revision.
    /// </summary>
    private static ConcurrencyError CreateConcurrencyError(
        DbUpdateConcurrencyException exception,
        string fallbackEntityName,
        Guid fallbackEntityId)
    {
        foreach (var entry in exception.Entries)
        {
            if (entry.Entity is BuyerProposalRevision revision)
            {
                return new ConcurrencyError(
                    nameof(BuyerProposalRevision),
                    revision.Id);
            }

            if (entry.Entity is BuyerProposalOption option)
            {
                return new ConcurrencyError(
                    nameof(BuyerProposalOption),
                    option.Id);
            }
        }

        return new ConcurrencyError(
            fallbackEntityName,
            fallbackEntityId);
    }

    /// <summary>
    /// Kontrollerar PostgreSQL unique violation.
    /// </summary>
    private static bool IsUniqueViolation(
        DbUpdateException exception)
    {
        return exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation
        };
    }

    /// <summary>
    /// Projekterar parent-revisionen efter mutation.
    /// </summary>
    private async Task<BuyerProposalRevisionResponse> ProjectRequiredRevisionAsync(
        Guid revisionId,
        CancellationToken cancellationToken)
    {
        return await _db.Set<BuyerProposalRevision>()
            .AsNoTracking()
            .Where(x => x.Id == revisionId)
            .Select(BuyerProposalProjections.RevisionDetails)
            .SingleAsync(cancellationToken);
    }

    /// <summary>
    /// Trimmar text och ersätter whitespace med null.
    /// </summary>
    private static string? TrimToNull(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}