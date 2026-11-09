using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Crm;
using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Trading.Projections;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar RFQ-rader, demand-scope provenance och materialalternativ inom draft-revisioner.
/// </summary>
public sealed class BuyerRfqLineService : IBuyerRfqLineService
{
    private readonly Arb_IntDbContext _db;
    private readonly IValidationService _validationService;

    /// <summary>
    /// Skapar RFQ-radservicen med databas- och valideringsberoenden.
    /// </summary>
    public BuyerRfqLineService(Arb_IntDbContext db, IValidationService validationService)
    {
        _db = db;
        _validationService = validationService;
    }

    /// <summary>
    /// Hämtar samtliga rader i en revision i radnummerordning.
    /// </summary>
    public async Task<Result<IReadOnlyList<BuyerRfqLineResponse>>> GetByRevisionAsync(
        Guid buyerRfqRevisionId,
        CancellationToken cancellationToken = default)
    {
        var revisionExists = await _db.Set<BuyerRfqRevision>()
            .AsNoTracking()
            .AnyAsync(x => x.Id == buyerRfqRevisionId, cancellationToken);

        if (!revisionExists)
        {
            return Result.Fail<IReadOnlyList<BuyerRfqLineResponse>>(
                new NotFoundError(nameof(BuyerRfqRevision), buyerRfqRevisionId));
        }

        var lines = await _db.Set<BuyerRfqLine>()
            .AsNoTracking()
            .Where(x => x.BuyerRfqRevisionId == buyerRfqRevisionId)
            .OrderBy(x => x.LineNumber)
            .ThenBy(x => x.Id)
            .Select(BuyerRfqLineProjections.Details)
            .ToListAsync(cancellationToken);

        return Result.Ok<IReadOnlyList<BuyerRfqLineResponse>>(lines);
    }

    /// <summary>
    /// Hämtar en enskild RFQ-rad.
    /// </summary>
    public async Task<Result<BuyerRfqLineResponse>> GetByIdAsync(
        Guid buyerRfqLineId,
        CancellationToken cancellationToken = default)
    {
        var response = await ProjectAsync(buyerRfqLineId, cancellationToken);

        if (response is null)
        {
            return Result.Fail<BuyerRfqLineResponse>(
                new NotFoundError(nameof(BuyerRfqLine), buyerRfqLineId));
        }

        return Result.Ok(response);
    }

    /// <summary>
    /// Validerar och skapar en fristående RFQ-rad i en draft-revision.
    /// </summary>
    public async Task<Result<BuyerRfqLineResponse>> CreateAsync(
        Guid buyerRfqRevisionId,
        CreateBuyerRfqLineRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<BuyerRfqLineResponse>(validationResult.Errors);
        }

        var revision = await _db.Set<BuyerRfqRevision>()
            .SingleOrDefaultAsync(x => x.Id == buyerRfqRevisionId, cancellationToken);

        if (revision is null)
        {
            return Result.Fail<BuyerRfqLineResponse>(
                new NotFoundError(nameof(BuyerRfqRevision), buyerRfqRevisionId));
        }

        var editableResult = ValidateRevisionIsEditable(revision);

        if (editableResult.IsFailed)
        {
            return Result.Fail<BuyerRfqLineResponse>(editableResult.Errors);
        }

        var lineNumberResult = await ValidateLineNumberAsync(
            revision.Id,
            request.LineNumber,
            excludeLineId: null,
            cancellationToken);

        if (lineNumberResult.IsFailed)
        {
            return Result.Fail<BuyerRfqLineResponse>(lineNumberResult.Errors);
        }

        var referenceResult = await ValidateLineReferencesAsync(
            request.MaterialId,
            request.MaterialTypeId,
            request.IntendedUseCharacteristicId,
            request.QuantityUnitId,
            cancellationToken);

        if (referenceResult.IsFailed)
        {
            return Result.Fail<BuyerRfqLineResponse>(referenceResult.Errors);
        }

        var line = new BuyerRfqLine
        {
            Id = Guid.CreateVersion7(),
            BuyerRfqRevisionId = revision.Id,
            BuyerDemandScopeId = null,
            LineNumber = request.LineNumber,
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            BuyerLineReference = TrimToNull(request.BuyerLineReference),
            BuyerOriginalText = TrimToNull(request.BuyerOriginalText),
            MaterialId = request.MaterialId,
            MaterialTypeId = request.MaterialTypeId,
            IntendedUseCharacteristicId = request.IntendedUseCharacteristicId,
            RequiredQuantity = request.RequiredQuantity,
            QuantityUnitId = request.QuantityUnitId,
            LowerQuantityTolerancePercent = request.LowerQuantityTolerancePercent,
            UpperQuantityTolerancePercent = request.UpperQuantityTolerancePercent,
            AllowPartialFulfillment = request.AllowPartialFulfillment,
            AllowMultipleSuppliers = request.AllowMultipleSuppliers,
            AllowMultipleOrigins = request.AllowMultipleOrigins,
            AllowMultipleLots = request.AllowMultipleLots,
            RequireSingleSupplier = request.RequireSingleSupplier,
            RequireSingleOrigin = request.RequireSingleOrigin,
            RequireSingleLot = request.RequireSingleLot,
            RequireUniformCertification = request.RequireUniformCertification,
            RequireUniformSpecification = request.RequireUniformSpecification,
            MaximumSupplierCount = request.MaximumSupplierCount,
            InternalNotes = TrimToNull(request.InternalNotes)
        };

        _db.Set<BuyerRfqLine>().Add(line);
        TouchRevision(revision);

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Ok(await ProjectRequiredAsync(line.Id, cancellationToken));
    }

    /// <summary>
    /// Skapar en RFQ-rad från ett scope som tillhör samma BuyerDemand och buyer-relation som RFQ:n.
    /// </summary>
    public async Task<Result<BuyerRfqLineResponse>> CreateFromDemandScopeAsync(
        Guid buyerRfqRevisionId,
        Guid buyerDemandScopeId,
        CreateBuyerRfqLineFromDemandScopeRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<BuyerRfqLineResponse>(validationResult.Errors);
        }

        var revision = await _db.Set<BuyerRfqRevision>()
            .Include(x => x.BuyerRfq)
            .SingleOrDefaultAsync(x => x.Id == buyerRfqRevisionId, cancellationToken);

        if (revision is null)
        {
            return Result.Fail<BuyerRfqLineResponse>(
                new NotFoundError(nameof(BuyerRfqRevision), buyerRfqRevisionId));
        }

        var editableResult = ValidateRevisionIsEditable(revision);

        if (editableResult.IsFailed)
        {
            return Result.Fail<BuyerRfqLineResponse>(editableResult.Errors);
        }

        if (!revision.BuyerRfq.BuyerDemandId.HasValue)
        {
            return Result.Fail<BuyerRfqLineResponse>(
                new ConflictError(
                    "A BuyerDemandScope can only be used when the BuyerRfq is linked to a BuyerDemand."));
        }

        var scope = await _db.Set<BuyerDemandScope>()
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == buyerDemandScopeId, cancellationToken);

        if (scope is null)
        {
            return Result.Fail<BuyerRfqLineResponse>(
                new NotFoundError(nameof(BuyerDemandScope), buyerDemandScopeId));
        }

        if (scope.BuyerDemandId != revision.BuyerRfq.BuyerDemandId.Value
            || scope.BusinessRelationshipId != revision.BuyerRfq.BuyerBusinessRelationshipId)
        {
            return Result.Fail<BuyerRfqLineResponse>(
                new ConflictError(
                    "The BuyerDemandScope does not belong to the BuyerDemand and buyer relationship used by this RFQ."));
        }

        var requiredQuantity = request.RequiredQuantity ?? scope.TypicalQuantity;
        var quantityUnitId = request.QuantityUnitId ?? scope.TypicalQuantityUnitId;

        if (!requiredQuantity.HasValue || requiredQuantity.Value <= 0)
        {
            return Result.Fail<BuyerRfqLineResponse>(
                new ConflictError(
                    "The BuyerDemandScope has no usable TypicalQuantity. RequiredQuantity must be supplied explicitly."));
        }

        if (!quantityUnitId.HasValue)
        {
            return Result.Fail<BuyerRfqLineResponse>(
                new ConflictError(
                    "The BuyerDemandScope has no quantity unit. QuantityUnitId must be supplied explicitly."));
        }

        var lineNumberResult = await ValidateLineNumberAsync(
            revision.Id,
            request.LineNumber,
            excludeLineId: null,
            cancellationToken);

        if (lineNumberResult.IsFailed)
        {
            return Result.Fail<BuyerRfqLineResponse>(lineNumberResult.Errors);
        }

        var referenceResult = await ValidateLineReferencesAsync(
            scope.MaterialId,
            scope.MaterialTypeId,
            scope.IntendedUseCharacteristicId,
            quantityUnitId.Value,
            cancellationToken);

        if (referenceResult.IsFailed)
        {
            return Result.Fail<BuyerRfqLineResponse>(referenceResult.Errors);
        }

        var line = new BuyerRfqLine
        {
            Id = Guid.CreateVersion7(),
            BuyerRfqRevisionId = revision.Id,
            BuyerDemandScopeId = scope.Id,
            LineNumber = request.LineNumber,
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            BuyerLineReference = TrimToNull(request.BuyerLineReference),
            BuyerOriginalText = TrimToNull(request.BuyerOriginalText),
            MaterialId = scope.MaterialId,
            MaterialTypeId = scope.MaterialTypeId,
            IntendedUseCharacteristicId = scope.IntendedUseCharacteristicId,
            RequiredQuantity = requiredQuantity.Value,
            QuantityUnitId = quantityUnitId.Value,
            LowerQuantityTolerancePercent = request.LowerQuantityTolerancePercent,
            UpperQuantityTolerancePercent = request.UpperQuantityTolerancePercent,
            AllowPartialFulfillment = request.AllowPartialFulfillment,
            AllowMultipleSuppliers = request.AllowMultipleSuppliers,
            AllowMultipleOrigins = request.AllowMultipleOrigins,
            AllowMultipleLots = request.AllowMultipleLots,
            RequireSingleSupplier = request.RequireSingleSupplier,
            RequireSingleOrigin = request.RequireSingleOrigin,
            RequireSingleLot = request.RequireSingleLot,
            RequireUniformCertification = request.RequireUniformCertification,
            RequireUniformSpecification = request.RequireUniformSpecification,
            MaximumSupplierCount = request.MaximumSupplierCount,
            InternalNotes = TrimToNull(request.InternalNotes)
        };

        _db.Set<BuyerRfqLine>().Add(line);
        TouchRevision(revision);

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Ok(await ProjectRequiredAsync(line.Id, cancellationToken));
    }

    /// <summary>
    /// Uppdaterar en RFQ-rad med optimistic concurrency så länge revisionen är Draft.
    /// </summary>
    public async Task<Result<BuyerRfqLineResponse>> UpdateAsync(
        Guid buyerRfqLineId,
        UpdateBuyerRfqLineRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<BuyerRfqLineResponse>(validationResult.Errors);
        }

        var line = await _db.Set<BuyerRfqLine>()
            .Include(x => x.BuyerRfqRevision)
            .SingleOrDefaultAsync(x => x.Id == buyerRfqLineId, cancellationToken);

        if (line is null)
        {
            return Result.Fail<BuyerRfqLineResponse>(
                new NotFoundError(nameof(BuyerRfqLine), buyerRfqLineId));
        }

        var editableResult = ValidateRevisionIsEditable(line.BuyerRfqRevision);

        if (editableResult.IsFailed)
        {
            return Result.Fail<BuyerRfqLineResponse>(editableResult.Errors);
        }

        var lineNumberResult = await ValidateLineNumberAsync(
            line.BuyerRfqRevisionId,
            request.LineNumber,
            line.Id,
            cancellationToken);

        if (lineNumberResult.IsFailed)
        {
            return Result.Fail<BuyerRfqLineResponse>(lineNumberResult.Errors);
        }

        var referenceResult = await ValidateLineReferencesAsync(
            request.MaterialId,
            request.MaterialTypeId,
            request.IntendedUseCharacteristicId,
            request.QuantityUnitId,
            cancellationToken);

        if (referenceResult.IsFailed)
        {
            return Result.Fail<BuyerRfqLineResponse>(referenceResult.Errors);
        }

        line.LineNumber = request.LineNumber;
        line.Name = request.Name.Trim();
        line.Description = request.Description.Trim();
        line.BuyerLineReference = TrimToNull(request.BuyerLineReference);
        line.BuyerOriginalText = TrimToNull(request.BuyerOriginalText);
        line.MaterialId = request.MaterialId;
        line.MaterialTypeId = request.MaterialTypeId;
        line.IntendedUseCharacteristicId = request.IntendedUseCharacteristicId;
        line.RequiredQuantity = request.RequiredQuantity;
        line.QuantityUnitId = request.QuantityUnitId;
        line.LowerQuantityTolerancePercent = request.LowerQuantityTolerancePercent;
        line.UpperQuantityTolerancePercent = request.UpperQuantityTolerancePercent;
        line.AllowPartialFulfillment = request.AllowPartialFulfillment;
        line.AllowMultipleSuppliers = request.AllowMultipleSuppliers;
        line.AllowMultipleOrigins = request.AllowMultipleOrigins;
        line.AllowMultipleLots = request.AllowMultipleLots;
        line.RequireSingleSupplier = request.RequireSingleSupplier;
        line.RequireSingleOrigin = request.RequireSingleOrigin;
        line.RequireSingleLot = request.RequireSingleLot;
        line.RequireUniformCertification = request.RequireUniformCertification;
        line.RequireUniformSpecification = request.RequireUniformSpecification;
        line.MaximumSupplierCount = request.MaximumSupplierCount;
        line.InternalNotes = TrimToNull(request.InternalNotes);

        TouchRevision(line.BuyerRfqRevision);

        var saveResult = await SaveWithConcurrencyAsync(
            line,
            line.Id,
            request.RowVersion,
            cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<BuyerRfqLineResponse>(saveResult.Errors);
        }

        return Result.Ok(await ProjectRequiredAsync(line.Id, cancellationToken));
    }

    /// <summary>
    /// Tar bort en draft-rad om inga requirements eller delivery allocations fortfarande refererar till den.
    /// </summary>
    public async Task<Result> DeleteAsync(
        Guid buyerRfqLineId,
        uint rowVersion,
        CancellationToken cancellationToken = default)
    {
        var line = await _db.Set<BuyerRfqLine>()
            .Include(x => x.BuyerRfqRevision)
            .SingleOrDefaultAsync(x => x.Id == buyerRfqLineId, cancellationToken);

        if (line is null)
        {
            return Result.Fail(
                new NotFoundError(nameof(BuyerRfqLine), buyerRfqLineId));
        }

        var editableResult = ValidateRevisionIsEditable(line.BuyerRfqRevision);

        if (editableResult.IsFailed)
        {
            return editableResult;
        }

        var hasRequirements = await _db.Set<BuyerRfqRequirement>()
            .AsNoTracking()
            .AnyAsync(x => x.BuyerRfqLineId == line.Id, cancellationToken);

        if (hasRequirements)
        {
            return Result.Fail(
                new ConflictError(
                    "A BuyerRfqLine with requirements cannot be deleted. Remove or reassign the requirements first."));
        }

        var hasDeliveryAllocations = await _db.Set<BuyerRfqDeliveryLineAllocation>()
            .AsNoTracking()
            .AnyAsync(x => x.BuyerRfqLineId == line.Id, cancellationToken);

        if (hasDeliveryAllocations)
        {
            return Result.Fail(
                new ConflictError(
                    "A BuyerRfqLine used by delivery allocations cannot be deleted."));
        }

        _db.Entry(line).Property(x => x.RowVersion).OriginalValue = rowVersion;
        TouchRevision(line.BuyerRfqRevision);
        _db.Set<BuyerRfqLine>().Remove(line);

        return await SaveChangesWithConcurrencyAsync(
            nameof(BuyerRfqLine),
            line.Id,
            cancellationToken);
    }

    /// <summary>
    /// Lägger till ett materialalternativ på en rad i en draft-revision.
    /// </summary>
    public async Task<Result<BuyerRfqLineResponse>> AddMaterialOptionAsync(
        Guid buyerRfqLineId,
        CreateBuyerRfqLineMaterialOptionRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<BuyerRfqLineResponse>(validationResult.Errors);
        }

        var line = await _db.Set<BuyerRfqLine>()
            .Include(x => x.BuyerRfqRevision)
                .ThenInclude(x => x.BuyerRfq)
            .SingleOrDefaultAsync(x => x.Id == buyerRfqLineId, cancellationToken);

        if (line is null)
        {
            return Result.Fail<BuyerRfqLineResponse>(
                new NotFoundError(nameof(BuyerRfqLine), buyerRfqLineId));
        }

        var editableResult = ValidateRevisionIsEditable(line.BuyerRfqRevision);

        if (editableResult.IsFailed)
        {
            return Result.Fail<BuyerRfqLineResponse>(editableResult.Errors);
        }

        var referenceResult = await ValidateMaterialOptionReferencesAsync(
            line.BuyerRfqRevision.BuyerRfq.BuyerBusinessRelationshipId,
            request.MaterialId,
            request.MaterialTypeId,
            request.BiologicalSourceId,
            request.BiologicalSourceClaimId,
            cancellationToken);

        if (referenceResult.IsFailed)
        {
            return Result.Fail<BuyerRfqLineResponse>(referenceResult.Errors);
        }

        var option = new BuyerRfqLineMaterialOption
        {
            Id = Guid.CreateVersion7(),
            BuyerRfqLineId = line.Id,
            OptionKind = request.OptionKind,
            MaterialId = request.MaterialId,
            MaterialTypeId = request.MaterialTypeId,
            BiologicalSourceId = request.BiologicalSourceId,
            BiologicalSourceClaimId = request.BiologicalSourceClaimId,
            BuyerText = TrimToNull(request.BuyerText),
            AlternativeGroupKey = TrimToNull(request.AlternativeGroupKey),
            MinimumMappingConfidence = request.MinimumMappingConfidence,
            SortOrder = request.SortOrder
        };

        _db.Set<BuyerRfqLineMaterialOption>().Add(option);
        TouchRevision(line.BuyerRfqRevision);

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Ok(await ProjectRequiredAsync(line.Id, cancellationToken));
    }

    /// <summary>
    /// Uppdaterar ett materialalternativ med optimistic concurrency i en draft-revision.
    /// </summary>
    public async Task<Result<BuyerRfqLineResponse>> UpdateMaterialOptionAsync(
        Guid materialOptionId,
        UpdateBuyerRfqLineMaterialOptionRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<BuyerRfqLineResponse>(validationResult.Errors);
        }

        var option = await _db.Set<BuyerRfqLineMaterialOption>()
            .Include(x => x.BuyerRfqLine)
                .ThenInclude(x => x.BuyerRfqRevision)
                    .ThenInclude(x => x.BuyerRfq)
            .SingleOrDefaultAsync(x => x.Id == materialOptionId, cancellationToken);

        if (option is null)
        {
            return Result.Fail<BuyerRfqLineResponse>(
                new NotFoundError(nameof(BuyerRfqLineMaterialOption), materialOptionId));
        }

        var editableResult = ValidateRevisionIsEditable(
            option.BuyerRfqLine.BuyerRfqRevision);

        if (editableResult.IsFailed)
        {
            return Result.Fail<BuyerRfqLineResponse>(editableResult.Errors);
        }

        var referenceResult = await ValidateMaterialOptionReferencesAsync(
            option.BuyerRfqLine.BuyerRfqRevision.BuyerRfq.BuyerBusinessRelationshipId,
            request.MaterialId,
            request.MaterialTypeId,
            request.BiologicalSourceId,
            request.BiologicalSourceClaimId,
            cancellationToken);

        if (referenceResult.IsFailed)
        {
            return Result.Fail<BuyerRfqLineResponse>(referenceResult.Errors);
        }

        option.OptionKind = request.OptionKind;
        option.MaterialId = request.MaterialId;
        option.MaterialTypeId = request.MaterialTypeId;
        option.BiologicalSourceId = request.BiologicalSourceId;
        option.BiologicalSourceClaimId = request.BiologicalSourceClaimId;
        option.BuyerText = TrimToNull(request.BuyerText);
        option.AlternativeGroupKey = TrimToNull(request.AlternativeGroupKey);
        option.MinimumMappingConfidence = request.MinimumMappingConfidence;
        option.SortOrder = request.SortOrder;

        TouchRevision(option.BuyerRfqLine.BuyerRfqRevision);

        var saveResult = await SaveWithConcurrencyAsync(
            option,
            option.Id,
            request.RowVersion,
            cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<BuyerRfqLineResponse>(saveResult.Errors);
        }

        return Result.Ok(
            await ProjectRequiredAsync(
                option.BuyerRfqLineId,
                cancellationToken));
    }

    /// <summary>
    /// Tar bort ett materialalternativ med optimistic concurrency från en draft-revision.
    /// </summary>
    public async Task<Result<BuyerRfqLineResponse>> DeleteMaterialOptionAsync(
        Guid materialOptionId,
        uint rowVersion,
        CancellationToken cancellationToken = default)
    {
        var option = await _db.Set<BuyerRfqLineMaterialOption>()
            .Include(x => x.BuyerRfqLine)
                .ThenInclude(x => x.BuyerRfqRevision)
            .SingleOrDefaultAsync(x => x.Id == materialOptionId, cancellationToken);

        if (option is null)
        {
            return Result.Fail<BuyerRfqLineResponse>(
                new NotFoundError(nameof(BuyerRfqLineMaterialOption), materialOptionId));
        }

        var editableResult = ValidateRevisionIsEditable(
            option.BuyerRfqLine.BuyerRfqRevision);

        if (editableResult.IsFailed)
        {
            return Result.Fail<BuyerRfqLineResponse>(editableResult.Errors);
        }

        var lineId = option.BuyerRfqLineId;

        _db.Entry(option).Property(x => x.RowVersion).OriginalValue = rowVersion;
        TouchRevision(option.BuyerRfqLine.BuyerRfqRevision);
        _db.Set<BuyerRfqLineMaterialOption>().Remove(option);

        var saveResult = await SaveChangesWithConcurrencyAsync(
            nameof(BuyerRfqLineMaterialOption),
            option.Id,
            cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<BuyerRfqLineResponse>(saveResult.Errors);
        }

        return Result.Ok(await ProjectRequiredAsync(lineId, cancellationToken));
    }

    /// <summary>
    /// Markerar parent-revisionen så att child-writes konkurrerar med release på samma RowVersion.
    /// </summary>
    private static void TouchRevision(BuyerRfqRevision revision)
    {
        revision.ActionDateUtc = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Kontrollerar att revisionen fortfarande är en redigerbar Draft.
    /// </summary>
    private static Result ValidateRevisionIsEditable(BuyerRfqRevision revision)
    {
        if (revision.Status != BuyerRfqRevisionStatus.Draft)
        {
            return Result.Fail(
                new ConflictError(
                    $"BuyerRfqRevision with status '{revision.Status}' cannot be modified."));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Kontrollerar att radnumret är unikt inom revisionen.
    /// </summary>
    private async Task<Result> ValidateLineNumberAsync(
        Guid buyerRfqRevisionId,
        int lineNumber,
        Guid? excludeLineId,
        CancellationToken cancellationToken)
    {
        IQueryable<BuyerRfqLine> query = _db.Set<BuyerRfqLine>()
            .AsNoTracking()
            .Where(x =>
                x.BuyerRfqRevisionId == buyerRfqRevisionId
                && x.LineNumber == lineNumber);

        if (excludeLineId.HasValue)
        {
            query = query.Where(x => x.Id != excludeLineId.Value);
        }

        if (!await query.AnyAsync(cancellationToken))
        {
            return Result.Ok();
        }

        return Result.Fail(
            new ConflictError(
                $"Line number '{lineNumber}' already exists in the BuyerRfqRevision."));
    }

    /// <summary>
    /// Kontrollerar att RFQ-radens refererade artikeldata finns.
    /// </summary>
    private async Task<Result> ValidateLineReferencesAsync(
        Guid? materialId,
        int? materialTypeId,
        Guid? intendedUseCharacteristicId,
        int quantityUnitId,
        CancellationToken cancellationToken)
    {
        if (materialId.HasValue
            && !await _db.Set<Material>().AsNoTracking().AnyAsync(x => x.Id == materialId.Value, cancellationToken))
        {
            return Result.Fail(new NotFoundError(nameof(Material), materialId.Value));
        }

        if (materialTypeId.HasValue
            && !await _db.Set<MaterialType>().AsNoTracking().AnyAsync(x => x.Id == materialTypeId.Value, cancellationToken))
        {
            return Result.Fail(new NotFoundError(nameof(MaterialType), materialTypeId.Value));
        }

        if (intendedUseCharacteristicId.HasValue
            && !await _db.Set<Characteristic>().AsNoTracking().AnyAsync(x => x.Id == intendedUseCharacteristicId.Value, cancellationToken))
        {
            return Result.Fail(new NotFoundError(nameof(Characteristic), intendedUseCharacteristicId.Value));
        }

        if (!await _db.Set<UnitOfMeasure>().AsNoTracking().AnyAsync(x => x.Id == quantityUnitId, cancellationToken))
        {
            return Result.Fail(new NotFoundError(nameof(UnitOfMeasure), quantityUnitId));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Kontrollerar materialalternativets referenser och att eventuell claim kommer från RFQ:ns buyer-relation.
    /// </summary>
    private async Task<Result> ValidateMaterialOptionReferencesAsync(
        Guid buyerBusinessRelationshipId,
        Guid? materialId,
        int? materialTypeId,
        Guid? biologicalSourceId,
        Guid? biologicalSourceClaimId,
        CancellationToken cancellationToken)
    {
        if (materialId.HasValue
            && !await _db.Set<Material>().AsNoTracking().AnyAsync(x => x.Id == materialId.Value, cancellationToken))
        {
            return Result.Fail(new NotFoundError(nameof(Material), materialId.Value));
        }

        if (materialTypeId.HasValue
            && !await _db.Set<MaterialType>().AsNoTracking().AnyAsync(x => x.Id == materialTypeId.Value, cancellationToken))
        {
            return Result.Fail(new NotFoundError(nameof(MaterialType), materialTypeId.Value));
        }

        if (biologicalSourceId.HasValue
            && !await _db.Set<BiologicalSource>().AsNoTracking().AnyAsync(x => x.Id == biologicalSourceId.Value, cancellationToken))
        {
            return Result.Fail(new NotFoundError(nameof(BiologicalSource), biologicalSourceId.Value));
        }

        if (biologicalSourceClaimId.HasValue)
        {
            var validClaim = await _db.Set<BusinessRelationshipBiologicalSourceClaim>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == biologicalSourceClaimId.Value
                        && x.BusinessRelationshipId == buyerBusinessRelationshipId,
                    cancellationToken);

            if (!validClaim)
            {
                return Result.Fail(
                    new ConflictError(
                        "The BiologicalSourceClaim does not belong to the BuyerRfq buyer relationship."));
            }
        }

        return Result.Ok();
    }

    /// <summary>
    /// Sparar en RowVersion-skyddad entitet med optimistic concurrency.
    /// </summary>
    private async Task<Result> SaveWithConcurrencyAsync<TEntity>(
        TEntity entity,
        Guid entityId,
        uint expectedRowVersion,
        CancellationToken cancellationToken)
        where TEntity : class
    {
        _db.Entry(entity).Property("RowVersion").OriginalValue = expectedRowVersion;

        return await SaveChangesWithConcurrencyAsync(
            typeof(TEntity).Name,
            entityId,
            cancellationToken);
    }

    /// <summary>
    /// Sparar ändringar och översätter EF concurrency-konflikter till domänfel.
    /// </summary>
    private async Task<Result> SaveChangesWithConcurrencyAsync(
        string entityName,
        Guid entityId,
        CancellationToken cancellationToken)
    {
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Fail(
                new ConcurrencyError(
                    entityName,
                    entityId));
        }
    }

    /// <summary>
    /// Projekterar en RFQ-rad direkt från databasen.
    /// </summary>
    private async Task<BuyerRfqLineResponse?> ProjectAsync(
        Guid buyerRfqLineId,
        CancellationToken cancellationToken)
    {
        return await _db.Set<BuyerRfqLine>()
            .AsNoTracking()
            .Where(x => x.Id == buyerRfqLineId)
            .Select(BuyerRfqLineProjections.Details)
            .SingleOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Projekterar en sparad RFQ-rad och kastar om den oväntat saknas.
    /// </summary>
    private async Task<BuyerRfqLineResponse> ProjectRequiredAsync(
        Guid buyerRfqLineId,
        CancellationToken cancellationToken)
    {
        return await ProjectAsync(buyerRfqLineId, cancellationToken)
            ?? throw new InvalidOperationException(
                $"BuyerRfqLine '{buyerRfqLineId}' was saved but could not be projected.");
    }

    /// <summary>
    /// Trimmar text och ersätter tomma värden med null.
    /// </summary>
    private static string? TrimToNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}