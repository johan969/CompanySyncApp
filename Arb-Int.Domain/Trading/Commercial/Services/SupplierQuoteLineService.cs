using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Trading.Projections;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar offertrader och använder SupplierQuoteRevision som aggregate-level concurrency-root.
/// </summary>
public sealed class SupplierQuoteLineService : ISupplierQuoteLineService
{
    private readonly Arb_IntDbContext _db;
    private readonly IValidationService _validationService;
    private readonly ISupplierQuoteReferenceValidator _referenceValidator;

    /// <summary>
    /// Skapar line-servicen med databas-, request-validation- och provenance-beroenden.
    /// </summary>
    public SupplierQuoteLineService(
        Arb_IntDbContext db,
        IValidationService validationService,
        ISupplierQuoteReferenceValidator referenceValidator)
    {
        _db = db;
        _validationService = validationService;
        _referenceValidator = referenceValidator;
    }

    /// <summary>
    /// Skapar en offertrad efter att parent-revision och samtliga referenser validerats.
    /// </summary>
    public async Task<Result<SupplierQuoteRevisionResponse>> CreateAsync(
        Guid supplierQuoteRevisionId,
        CreateSupplierQuoteLineRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<SupplierQuoteRevisionResponse>(validation.Errors);
        }

        var revision = await _db.Set<SupplierQuoteRevision>()
            .Include(x => x.SupplierQuote)
            .SingleOrDefaultAsync(x => x.Id == supplierQuoteRevisionId, cancellationToken);

        if (revision is null)
        {
            return Result.Fail<SupplierQuoteRevisionResponse>(
                new NotFoundError(nameof(SupplierQuoteRevision), supplierQuoteRevisionId));
        }

        var writableResult = ValidateRevisionIsWritable(revision);

        if (writableResult.IsFailed)
        {
            return Result.Fail<SupplierQuoteRevisionResponse>(writableResult.Errors);
        }

        /*
         * Här ligger provenance-valideringen i sitt faktiska sammanhang.
         * RFQ-line måste tillhöra quote-rootens RFQ-revision.
         * Offering/capability/lot måste tillhöra quote-rootens supplier.
         * Solution step måste komma från samma RFQ och får inte peka på annan provider.
         */
        var referencesResult = await _referenceValidator.ValidateLineReferencesAsync(
            revision.SupplierBusinessRelationshipId,
            revision.SupplierQuote.BuyerRfqRevisionId,
            request.BuyerRfqLineId,
            request.SupplierMaterialOfferingId,
            request.SupplierProcessCapabilityId,
            request.SupplySolutionStepId,
            request.InventoryLotId,
            request.QuantityUnitId,
            request.IncotermRuleId,
            cancellationToken);

        if (referencesResult.IsFailed)
        {
            return Result.Fail<SupplierQuoteRevisionResponse>(referencesResult.Errors);
        }

        var duplicateLineNumber = await _db.Set<SupplierQuoteLine>()
            .AsNoTracking()
            .AnyAsync(
                x => x.SupplierQuoteRevisionId == revision.Id
                    && x.LineNumber == request.LineNumber,
                cancellationToken);

        if (duplicateLineNumber)
        {
            return Result.Fail<SupplierQuoteRevisionResponse>(
                new ConflictError(
                    $"LineNumber '{request.LineNumber}' already exists in the SupplierQuoteRevision."));
        }

        var line = new SupplierQuoteLine
        {
            Id = Guid.CreateVersion7(),
            SupplierQuoteRevisionId = revision.Id,
            SupplierBusinessRelationshipId = revision.SupplierBusinessRelationshipId,
            BuyerRfqLineId = request.BuyerRfqLineId,
            Kind = request.Kind,
            SupplierMaterialOfferingId = request.SupplierMaterialOfferingId,
            SupplierProcessCapabilityId = request.SupplierProcessCapabilityId,
            SupplySolutionStepId = request.SupplySolutionStepId,
            InventoryLotId = request.InventoryLotId,
            LineNumber = request.LineNumber,
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            OfferedQuantity = request.OfferedQuantity,
            QuantityUnitId = request.QuantityUnitId,
            UnitPrice = request.UnitPrice,
            FixedFee = request.FixedFee,
            MinimumOrderQuantity = request.MinimumOrderQuantity,
            AvailableFromUtc = request.AvailableFromUtc,
            AvailableToUtc = request.AvailableToUtc,
            IncotermRuleId = request.IncotermRuleId,
            NamedPlace = TrimToNull(request.NamedPlace),
            PriceBasis = TrimToNull(request.PriceBasis),
            SupplierLineReference = TrimToNull(request.SupplierLineReference),
            InternalNotes = TrimToNull(request.InternalNotes)
        };

        TouchRevision(revision);

        _db.Set<SupplierQuoteLine>().Add(line);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            return Result.Fail<SupplierQuoteRevisionResponse>(
                CreateConcurrencyError(
                    exception,
                    nameof(SupplierQuoteRevision),
                    revision.Id));
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            return Result.Fail<SupplierQuoteRevisionResponse>(
                new ConflictError(
                    $"LineNumber '{request.LineNumber}' already exists in the SupplierQuoteRevision."));
        }

        return Result.Ok(
            await ProjectRequiredRevisionAsync(
                revision.Id,
                cancellationToken));
    }

    /// <summary>
    /// Uppdaterar en offertrad utan att flytta den till en annan revision eller supplier.
    /// </summary>
    public async Task<Result<SupplierQuoteRevisionResponse>> UpdateAsync(
        Guid supplierQuoteLineId,
        UpdateSupplierQuoteLineRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validationService.ValidateAsync(request, cancellationToken);

        if (validation.IsFailed)
        {
            return Result.Fail<SupplierQuoteRevisionResponse>(validation.Errors);
        }

        var line = await LoadLineForWriteAsync(
            supplierQuoteLineId,
            cancellationToken);

        if (line is null)
        {
            return Result.Fail<SupplierQuoteRevisionResponse>(
                new NotFoundError(nameof(SupplierQuoteLine), supplierQuoteLineId));
        }

        var revision = line.SupplierQuoteRevision;

        var writableResult = ValidateRevisionIsWritable(revision);

        if (writableResult.IsFailed)
        {
            return Result.Fail<SupplierQuoteRevisionResponse>(writableResult.Errors);
        }

        var referencesResult = await _referenceValidator.ValidateLineReferencesAsync(
            revision.SupplierBusinessRelationshipId,
            revision.SupplierQuote.BuyerRfqRevisionId,
            request.BuyerRfqLineId,
            request.SupplierMaterialOfferingId,
            request.SupplierProcessCapabilityId,
            request.SupplySolutionStepId,
            request.InventoryLotId,
            request.QuantityUnitId,
            request.IncotermRuleId,
            cancellationToken);

        if (referencesResult.IsFailed)
        {
            return Result.Fail<SupplierQuoteRevisionResponse>(referencesResult.Errors);
        }

        var duplicateLineNumber = await _db.Set<SupplierQuoteLine>()
            .AsNoTracking()
            .AnyAsync(
                x => x.SupplierQuoteRevisionId == revision.Id
                    && x.LineNumber == request.LineNumber
                    && x.Id != line.Id,
                cancellationToken);

        if (duplicateLineNumber)
        {
            return Result.Fail<SupplierQuoteRevisionResponse>(
                new ConflictError(
                    $"LineNumber '{request.LineNumber}' already exists in the SupplierQuoteRevision."));
        }

        _db.Entry(line)
            .Property(x => x.RowVersion)
            .OriginalValue = request.RowVersion;

        line.BuyerRfqLineId = request.BuyerRfqLineId;
        line.Kind = request.Kind;
        line.SupplierMaterialOfferingId = request.SupplierMaterialOfferingId;
        line.SupplierProcessCapabilityId = request.SupplierProcessCapabilityId;
        line.SupplySolutionStepId = request.SupplySolutionStepId;
        line.InventoryLotId = request.InventoryLotId;
        line.LineNumber = request.LineNumber;
        line.Name = request.Name.Trim();
        line.Description = request.Description.Trim();
        line.OfferedQuantity = request.OfferedQuantity;
        line.QuantityUnitId = request.QuantityUnitId;
        line.UnitPrice = request.UnitPrice;
        line.FixedFee = request.FixedFee;
        line.MinimumOrderQuantity = request.MinimumOrderQuantity;
        line.AvailableFromUtc = request.AvailableFromUtc;
        line.AvailableToUtc = request.AvailableToUtc;
        line.IncotermRuleId = request.IncotermRuleId;
        line.NamedPlace = TrimToNull(request.NamedPlace);
        line.PriceBasis = TrimToNull(request.PriceBasis);
        line.SupplierLineReference = TrimToNull(request.SupplierLineReference);
        line.InternalNotes = TrimToNull(request.InternalNotes);

        TouchRevision(revision);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            return Result.Fail<SupplierQuoteRevisionResponse>(
                CreateConcurrencyError(
                    exception,
                    nameof(SupplierQuoteLine),
                    line.Id));
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            return Result.Fail<SupplierQuoteRevisionResponse>(
                new ConflictError(
                    $"LineNumber '{request.LineNumber}' already exists in the SupplierQuoteRevision."));
        }

        return Result.Ok(
            await ProjectRequiredRevisionAsync(
                revision.Id,
                cancellationToken));
    }

    /// <summary>
    /// Tar bort en offertrad så länge parent-revisionen fortfarande är Draft.
    /// </summary>
    public async Task<Result<SupplierQuoteRevisionResponse>> DeleteAsync(
        Guid supplierQuoteLineId,
        uint rowVersion,
        CancellationToken cancellationToken = default)
    {
        var line = await LoadLineForWriteAsync(
            supplierQuoteLineId,
            cancellationToken);

        if (line is null)
        {
            return Result.Fail<SupplierQuoteRevisionResponse>(
                new NotFoundError(nameof(SupplierQuoteLine), supplierQuoteLineId));
        }

        var revision = line.SupplierQuoteRevision;

        var writableResult = ValidateRevisionIsWritable(revision);

        if (writableResult.IsFailed)
        {
            return Result.Fail<SupplierQuoteRevisionResponse>(writableResult.Errors);
        }

        _db.Entry(line)
            .Property(x => x.RowVersion)
            .OriginalValue = rowVersion;

        TouchRevision(revision);

        _db.Set<SupplierQuoteLine>().Remove(line);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            return Result.Fail<SupplierQuoteRevisionResponse>(
                CreateConcurrencyError(
                    exception,
                    nameof(SupplierQuoteLine),
                    line.Id));
        }

        return Result.Ok(
            await ProjectRequiredRevisionAsync(
                revision.Id,
                cancellationToken));
    }

    /// <summary>
    /// Läser offertraden tillsammans med parent-revision och quote-root.
    /// </summary>
    private Task<SupplierQuoteLine?> LoadLineForWriteAsync(
        Guid supplierQuoteLineId,
        CancellationToken cancellationToken)
    {
        return _db.Set<SupplierQuoteLine>()
            .Include(x => x.SupplierQuoteRevision)
                .ThenInclude(x => x.SupplierQuote)
            .SingleOrDefaultAsync(
                x => x.Id == supplierQuoteLineId,
                cancellationToken);
    }

    /// <summary>
    /// Kontrollerar att parent-revisionen fortfarande är redigerbar.
    /// </summary>
    private static Result ValidateRevisionIsWritable(
        SupplierQuoteRevision revision)
    {
        if (revision.Status != SupplierQuoteRevisionStatus.Draft)
        {
            return Result.Fail(
                new ConflictError(
                    $"SupplierQuoteRevision with status '{revision.Status}' is immutable."));
        }

        if (revision.ReceivedAtUtc.HasValue
            || revision.SnapshotJson is not null
            || revision.SnapshotHash is not null)
        {
            return Result.Fail(
                new ConflictError(
                    "SupplierQuoteRevision contains received snapshot data and is immutable."));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Markerar parent-revisionen så att child-write och ReceiveRevision konkurrerar på samma RowVersion.
    /// </summary>
    private static void TouchRevision(
        SupplierQuoteRevision revision)
    {
        revision.ActionDateUtc = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Översätter EF Core concurrency-fel till rätt domänentitet.
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
                case SupplierQuoteRevision revision:
                    return new ConcurrencyError(
                        nameof(SupplierQuoteRevision),
                        revision.Id);

                case SupplierQuoteLine line:
                    return new ConcurrencyError(
                        nameof(SupplierQuoteLine),
                        line.Id);
            }
        }

        return new ConcurrencyError(
            fallbackEntityName,
            fallbackEntityId);
    }

    /// <summary>
    /// Kontrollerar om PostgreSQL rapporterade en unique violation.
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
    /// Projekterar revisionens aktuella innehåll efter en mutation.
    /// </summary>
    private async Task<SupplierQuoteRevisionResponse> ProjectRequiredRevisionAsync(
        Guid supplierQuoteRevisionId,
        CancellationToken cancellationToken)
    {
        return await _db.Set<SupplierQuoteRevision>()
            .AsNoTracking()
            .Where(x => x.Id == supplierQuoteRevisionId)
            .Select(SupplierQuoteProjections.RevisionDetails)
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