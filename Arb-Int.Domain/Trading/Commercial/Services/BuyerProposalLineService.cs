using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Trading.Projections;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar proposal-lines, provenance och härledda monetary totals.
/// </summary>
public sealed class BuyerProposalLineService : IBuyerProposalLineService
{
    private readonly Arb_IntDbContext _db;
    private readonly IValidationService _validationService;
    private readonly IBuyerProposalReferenceValidator _referenceValidator;

    /// <summary>
    /// Skapar line-servicen.
    /// </summary>
    public BuyerProposalLineService(
        Arb_IntDbContext db,
        IValidationService validationService,
        IBuyerProposalReferenceValidator referenceValidator)
    {
        _db = db;
        _validationService = validationService;
        _referenceValidator = referenceValidator;
    }

    /// <summary>
    /// Skapar en proposal-line och räknar om optionens totals.
    /// </summary>
    public async Task<Result<BuyerProposalRevisionResponse>> CreateAsync(
        Guid buyerProposalOptionId,
        CreateBuyerProposalLineRequest request,
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

        var references = await _referenceValidator.ValidateLineAsync(
            option.BuyerProposalRevision.BuyerRfqRevisionId,
            option.SupplySolutionCandidateId,
            request.BuyerRfqLineId,
            request.SupplySolutionLineId,
            request.MaterialId,
            request.ProductId,
            request.QuantityUnitId,
            cancellationToken);

        if (references.IsFailed)
        {
            return Result.Fail<BuyerProposalRevisionResponse>(
                references.Errors);
        }

        var duplicateNumber = option.Lines
            .Any(x => x.LineNumber == request.LineNumber);

        if (duplicateNumber)
        {
            return Result.Fail<BuyerProposalRevisionResponse>(
                new ConflictError(
                    $"LineNumber '{request.LineNumber}' already exists in the option."));
        }

        var amounts =
            BuyerProposalAmountCalculator.Calculate(
                request.Quantity,
                request.UnitPrice,
                request.TaxPercent);

        var line = new BuyerProposalLine
        {
            Id = Guid.CreateVersion7(),
            BuyerProposalOptionId = option.Id,
            BuyerRfqLineId = request.BuyerRfqLineId,
            SupplySolutionLineId = request.SupplySolutionLineId,
            MaterialId = request.MaterialId,
            ProductId = request.ProductId,
            LineNumber = request.LineNumber,
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            Quantity = request.Quantity,
            QuantityUnitId = request.QuantityUnitId,
            UnitPrice = request.UnitPrice,
            NetAmount = amounts.NetAmount,
            TaxPercent = request.TaxPercent,
            TaxAmount = amounts.TaxAmount,
            GrossAmount = amounts.GrossAmount,
            EstimatedDeliveryFromUtc = request.EstimatedDeliveryFromUtc,
            EstimatedDeliveryToUtc = request.EstimatedDeliveryToUtc,
            BuyerFacingNotes = TrimToNull(request.BuyerFacingNotes)
        };

        option.Lines.Add(
            line);

        RecalculateOptionTotals(
            option);

        TouchOptionAndRevision(
            option);

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
                    $"LineNumber '{request.LineNumber}' already exists in the option."));
        }

        return Result.Ok(
            await ProjectRequiredRevisionAsync(
                option.BuyerProposalRevisionId,
                cancellationToken));
    }

    /// <summary>
    /// Uppdaterar line-innehåll, räknar om line-beloppen och därefter optionens totals.
    /// </summary>
    public async Task<Result<BuyerProposalRevisionResponse>> UpdateAsync(
        Guid buyerProposalLineId,
        UpdateBuyerProposalLineRequest request,
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

        var line = await _db.Set<BuyerProposalLine>()
            .Include(x => x.BuyerProposalOption)
                .ThenInclude(x => x.BuyerProposalRevision)
            .Include(x => x.BuyerProposalOption)
                .ThenInclude(x => x.Lines)
            .SingleOrDefaultAsync(
                x => x.Id == buyerProposalLineId,
                cancellationToken);

        if (line is null)
        {
            return Result.Fail<BuyerProposalRevisionResponse>(
                new NotFoundError(
                    nameof(BuyerProposalLine),
                    buyerProposalLineId));
        }

        var option =
            line.BuyerProposalOption;

        var writableResult =
            ValidateOptionIsWritable(option);

        if (writableResult.IsFailed)
        {
            return Result.Fail<BuyerProposalRevisionResponse>(
                writableResult.Errors);
        }

        var references = await _referenceValidator.ValidateLineAsync(
            option.BuyerProposalRevision.BuyerRfqRevisionId,
            option.SupplySolutionCandidateId,
            request.BuyerRfqLineId,
            request.SupplySolutionLineId,
            request.MaterialId,
            request.ProductId,
            request.QuantityUnitId,
            cancellationToken);

        if (references.IsFailed)
        {
            return Result.Fail<BuyerProposalRevisionResponse>(
                references.Errors);
        }

        if (option.Lines.Any(
                x => x.Id != line.Id
                    && x.LineNumber == request.LineNumber))
        {
            return Result.Fail<BuyerProposalRevisionResponse>(
                new ConflictError(
                    $"LineNumber '{request.LineNumber}' already exists in the option."));
        }

        var amounts =
            BuyerProposalAmountCalculator.Calculate(
                request.Quantity,
                request.UnitPrice,
                request.TaxPercent);

        _db.Entry(line)
            .Property(x => x.RowVersion)
            .OriginalValue = request.RowVersion;

        line.BuyerRfqLineId = request.BuyerRfqLineId;
        line.SupplySolutionLineId = request.SupplySolutionLineId;
        line.MaterialId = request.MaterialId;
        line.ProductId = request.ProductId;
        line.LineNumber = request.LineNumber;
        line.Name = request.Name.Trim();
        line.Description = request.Description.Trim();
        line.Quantity = request.Quantity;
        line.QuantityUnitId = request.QuantityUnitId;
        line.UnitPrice = request.UnitPrice;
        line.NetAmount = amounts.NetAmount;
        line.TaxPercent = request.TaxPercent;
        line.TaxAmount = amounts.TaxAmount;
        line.GrossAmount = amounts.GrossAmount;
        line.EstimatedDeliveryFromUtc = request.EstimatedDeliveryFromUtc;
        line.EstimatedDeliveryToUtc = request.EstimatedDeliveryToUtc;
        line.BuyerFacingNotes = TrimToNull(request.BuyerFacingNotes);

        RecalculateOptionTotals(
            option);

        TouchOptionAndRevision(
            option);

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
                    nameof(BuyerProposalLine),
                    line.Id));
        }
        catch (DbUpdateException exception)
            when (IsUniqueViolation(exception))
        {
            return Result.Fail<BuyerProposalRevisionResponse>(
                new ConflictError(
                    $"LineNumber '{request.LineNumber}' already exists in the option."));
        }

        return Result.Ok(
            await ProjectRequiredRevisionAsync(
                option.BuyerProposalRevisionId,
                cancellationToken));
    }

    /// <summary>
    /// Tar bort en Draft-line och räknar om optionens totals.
    /// </summary>
    public async Task<Result<BuyerProposalRevisionResponse>> DeleteAsync(
        Guid buyerProposalLineId,
        uint rowVersion,
        CancellationToken cancellationToken = default)
    {
        var line = await _db.Set<BuyerProposalLine>()
            .Include(x => x.BuyerProposalOption)
                .ThenInclude(x => x.BuyerProposalRevision)
            .Include(x => x.BuyerProposalOption)
                .ThenInclude(x => x.Lines)
            .SingleOrDefaultAsync(
                x => x.Id == buyerProposalLineId,
                cancellationToken);

        if (line is null)
        {
            return Result.Fail<BuyerProposalRevisionResponse>(
                new NotFoundError(
                    nameof(BuyerProposalLine),
                    buyerProposalLineId));
        }

        var option =
            line.BuyerProposalOption;

        var writableResult =
            ValidateOptionIsWritable(option);

        if (writableResult.IsFailed)
        {
            return Result.Fail<BuyerProposalRevisionResponse>(
                writableResult.Errors);
        }

        _db.Entry(line)
            .Property(x => x.RowVersion)
            .OriginalValue = rowVersion;

        option.Lines.Remove(
            line);

        _db.Set<BuyerProposalLine>()
            .Remove(line);

        RecalculateOptionTotals(
            option);

        TouchOptionAndRevision(
            option);

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
                    nameof(BuyerProposalLine),
                    line.Id));
        }

        return Result.Ok(
            await ProjectRequiredRevisionAsync(
                option.BuyerProposalRevisionId,
                cancellationToken));
    }

    /// <summary>
    /// Läser option, parent-revision och alla lines för totals-beräkning.
    /// </summary>
    private Task<BuyerProposalOption?> LoadOptionForWriteAsync(
        Guid optionId,
        CancellationToken cancellationToken)
    {
        return _db.Set<BuyerProposalOption>()
            .Include(x => x.BuyerProposalRevision)
            .Include(x => x.Lines)
            .SingleOrDefaultAsync(
                x => x.Id == optionId,
                cancellationToken);
    }

    /// <summary>
    /// Kontrollerar att både option och revision fortfarande är Draft.
    /// </summary>
    private static Result ValidateOptionIsWritable(
        BuyerProposalOption option)
    {
        var revision =
            option.BuyerProposalRevision;

        if (revision.Status != BuyerProposalRevisionStatus.Draft
            || revision.SnapshotJson is not null
            || revision.SnapshotHash is not null
            || revision.IssuedAtUtc.HasValue)
        {
            return Result.Fail(
                new ConflictError(
                    "BuyerProposalRevision is immutable."));
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
    /// Räknar om optionens totals från dess lines.
    /// </summary>
    private static void RecalculateOptionTotals(
        BuyerProposalOption option)
    {
        option.TotalNetAmount =
            option.Lines.Sum(x => x.NetAmount);

        option.TotalGrossAmount =
            option.Lines.Sum(x => x.GrossAmount);

        option.TotalTaxAmount =
            option.Lines.Any(x => x.TaxAmount.HasValue)
                ? option.Lines.Sum(x => x.TaxAmount ?? 0m)
                : null;
    }

    /// <summary>
    /// Touch:ar både option och revision så att issue konkurrerar med child-write.
    /// </summary>
    private static void TouchOptionAndRevision(
        BuyerProposalOption option)
    {
        var now =
            DateTimeOffset.UtcNow;

        option.ActionDateUtc = now;
        option.BuyerProposalRevision.ActionDateUtc = now;
    }

    /// <summary>
    /// Översätter concurrency-fel till rätt proposal-entitet.
    /// </summary>
    private static ConcurrencyError CreateConcurrencyError(
        DbUpdateConcurrencyException exception,
        string fallbackEntityName,
        Guid fallbackEntityId)
    {
        foreach (var entry in exception.Entries)
        {
            switch (entry.Entity)
            {
                case BuyerProposalRevision revision:
                    return new ConcurrencyError(
                        nameof(BuyerProposalRevision),
                        revision.Id);

                case BuyerProposalOption option:
                    return new ConcurrencyError(
                        nameof(BuyerProposalOption),
                        option.Id);

                case BuyerProposalLine line:
                    return new ConcurrencyError(
                        nameof(BuyerProposalLine),
                        line.Id);
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
    /// Projekterar revisionen efter mutation.
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