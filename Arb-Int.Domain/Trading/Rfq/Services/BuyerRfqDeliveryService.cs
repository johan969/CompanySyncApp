using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Parties;
using Arb_Int.Domain.Trading.Projections;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar leveranstillfällen och radallokeringar inom RFQ-draftrevisioner.
/// </summary>
public sealed class BuyerRfqDeliveryService : IBuyerRfqDeliveryService
{
    private readonly Arb_IntDbContext _db;
    private readonly IValidationService _validationService;

    /// <summary>
    /// Skapar delivery-servicen med databas- och valideringsberoenden.
    /// </summary>
    public BuyerRfqDeliveryService(Arb_IntDbContext db, IValidationService validationService)
    {
        _db = db;
        _validationService = validationService;
    }

    /// <summary>
    /// Hämtar samtliga leveranstillfällen för en RFQ-revision.
    /// </summary>
    public async Task<Result<IReadOnlyList<BuyerRfqDeliveryResponse>>> GetByRevisionAsync(
        Guid buyerRfqRevisionId,
        CancellationToken cancellationToken = default)
    {
        var revisionExists = await _db.Set<BuyerRfqRevision>()
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == buyerRfqRevisionId,
                cancellationToken);

        if (!revisionExists)
        {
            return Result.Fail<IReadOnlyList<BuyerRfqDeliveryResponse>>(
                new NotFoundError(
                    nameof(BuyerRfqRevision),
                    buyerRfqRevisionId));
        }

        var deliveries = await _db.Set<BuyerRfqDelivery>()
            .AsNoTracking()
            .Where(x => x.BuyerRfqRevisionId == buyerRfqRevisionId)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.DeliveryNumber)
            .ThenBy(x => x.Id)
            .Select(BuyerRfqDeliveryProjections.Details)
            .ToListAsync(cancellationToken);

        return Result.Ok<IReadOnlyList<BuyerRfqDeliveryResponse>>(
            deliveries);
    }

    /// <summary>
    /// Hämtar ett enskilt leveranstillfälle med dess radallokeringar.
    /// </summary>
    public async Task<Result<BuyerRfqDeliveryResponse>> GetByIdAsync(
        Guid buyerRfqDeliveryId,
        CancellationToken cancellationToken = default)
    {
        var response = await ProjectAsync(
            buyerRfqDeliveryId,
            cancellationToken);

        if (response is null)
        {
            return Result.Fail<BuyerRfqDeliveryResponse>(
                new NotFoundError(
                    nameof(BuyerRfqDelivery),
                    buyerRfqDeliveryId));
        }

        return Result.Ok(response);
    }

    /// <summary>
    /// Validerar och skapar ett leveranstillfälle i en draft-revision.
    /// </summary>
    public async Task<Result<BuyerRfqDeliveryResponse>> CreateAsync(
        Guid buyerRfqRevisionId,
        CreateBuyerRfqDeliveryRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(
            request,
            cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<BuyerRfqDeliveryResponse>(
                validationResult.Errors);
        }

        var revision = await _db.Set<BuyerRfqRevision>()
            .SingleOrDefaultAsync(
                x => x.Id == buyerRfqRevisionId,
                cancellationToken);

        if (revision is null)
        {
            return Result.Fail<BuyerRfqDeliveryResponse>(
                new NotFoundError(
                    nameof(BuyerRfqRevision),
                    buyerRfqRevisionId));
        }

        var editableResult = ValidateRevisionIsEditable(
            revision);

        if (editableResult.IsFailed)
        {
            return Result.Fail<BuyerRfqDeliveryResponse>(
                editableResult.Errors);
        }

        var deliveryNumberResult = await ValidateDeliveryNumberAsync(
            revision.Id,
            request.DeliveryNumber,
            excludeDeliveryId: null,
            cancellationToken);

        if (deliveryNumberResult.IsFailed)
        {
            return Result.Fail<BuyerRfqDeliveryResponse>(
                deliveryNumberResult.Errors);
        }

        var referencesResult = await ValidateDeliveryReferencesAsync(
            request.DestinationAddressId,
            request.DestinationLocationId,
            request.IncotermRuleId,
            cancellationToken);

        if (referencesResult.IsFailed)
        {
            return Result.Fail<BuyerRfqDeliveryResponse>(
                referencesResult.Errors);
        }

        var delivery = new BuyerRfqDelivery
        {
            Id = Guid.CreateVersion7(),
            BuyerRfqRevisionId = revision.Id,
            DeliveryNumber = request.DeliveryNumber,
            DestinationAddressId = request.DestinationAddressId,
            DestinationLocationId = request.DestinationLocationId,
            RequestedDeliveryFromUtc = request.RequestedDeliveryFromUtc,
            RequestedDeliveryToUtc = request.RequestedDeliveryToUtc,
            IncotermRuleId = request.IncotermRuleId,
            NamedPlace = TrimToNull(request.NamedPlace),
            MinimumTemperatureCelsius = request.MinimumTemperatureCelsius,
            MaximumTemperatureCelsius = request.MaximumTemperatureCelsius,
            PackagingNotes = TrimToNull(request.PackagingNotes),
            DeliveryNotes = TrimToNull(request.DeliveryNotes),
            SortOrder = request.SortOrder
        };

        _db.Set<BuyerRfqDelivery>()
            .Add(delivery);

        TouchRevision(revision);

        var saveResult = await SaveChangesWithConcurrencyAsync(
            nameof(BuyerRfqRevision),
            revision.Id,
            cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<BuyerRfqDeliveryResponse>(saveResult.Errors);
        }

        return Result.Ok(
            await ProjectRequiredAsync(
                delivery.Id,
                cancellationToken));
    }

    /// <summary>
    /// Uppdaterar ett leveranstillfälle med optimistic concurrency i en draft-revision.
    /// </summary>
    public async Task<Result<BuyerRfqDeliveryResponse>> UpdateAsync(
        Guid buyerRfqDeliveryId,
        UpdateBuyerRfqDeliveryRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(
            request,
            cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<BuyerRfqDeliveryResponse>(
                validationResult.Errors);
        }

        var delivery = await _db.Set<BuyerRfqDelivery>()
            .Include(x => x.BuyerRfqRevision)
            .SingleOrDefaultAsync(
                x => x.Id == buyerRfqDeliveryId,
                cancellationToken);

        if (delivery is null)
        {
            return Result.Fail<BuyerRfqDeliveryResponse>(
                new NotFoundError(
                    nameof(BuyerRfqDelivery),
                    buyerRfqDeliveryId));
        }

        var editableResult = ValidateRevisionIsEditable(
            delivery.BuyerRfqRevision);

        if (editableResult.IsFailed)
        {
            return Result.Fail<BuyerRfqDeliveryResponse>(
                editableResult.Errors);
        }

        var deliveryNumberResult = await ValidateDeliveryNumberAsync(
            delivery.BuyerRfqRevisionId,
            request.DeliveryNumber,
            delivery.Id,
            cancellationToken);

        if (deliveryNumberResult.IsFailed)
        {
            return Result.Fail<BuyerRfqDeliveryResponse>(
                deliveryNumberResult.Errors);
        }

        var referencesResult = await ValidateDeliveryReferencesAsync(
            request.DestinationAddressId,
            request.DestinationLocationId,
            request.IncotermRuleId,
            cancellationToken);

        if (referencesResult.IsFailed)
        {
            return Result.Fail<BuyerRfqDeliveryResponse>(
                referencesResult.Errors);
        }

        delivery.DeliveryNumber = request.DeliveryNumber;
        delivery.DestinationAddressId = request.DestinationAddressId;
        delivery.DestinationLocationId = request.DestinationLocationId;
        delivery.RequestedDeliveryFromUtc = request.RequestedDeliveryFromUtc;
        delivery.RequestedDeliveryToUtc = request.RequestedDeliveryToUtc;
        delivery.IncotermRuleId = request.IncotermRuleId;
        delivery.NamedPlace = TrimToNull(request.NamedPlace);
        delivery.MinimumTemperatureCelsius = request.MinimumTemperatureCelsius;
        delivery.MaximumTemperatureCelsius = request.MaximumTemperatureCelsius;
        delivery.PackagingNotes = TrimToNull(request.PackagingNotes);
        delivery.DeliveryNotes = TrimToNull(request.DeliveryNotes);
        delivery.SortOrder = request.SortOrder;

        TouchRevision(delivery.BuyerRfqRevision);

        var saveResult = await SaveWithConcurrencyAsync(
            delivery,
            delivery.Id,
            request.RowVersion,
            cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<BuyerRfqDeliveryResponse>(
                saveResult.Errors);
        }

        return Result.Ok(
            await ProjectRequiredAsync(
                delivery.Id,
                cancellationToken));
    }

    /// <summary>
    /// Tar bort ett leveranstillfälle och låter dess allocations tas bort via cascade.
    /// </summary>
    public async Task<Result> DeleteAsync(
        Guid buyerRfqDeliveryId,
        uint rowVersion,
        CancellationToken cancellationToken = default)
    {
        var delivery = await _db.Set<BuyerRfqDelivery>()
            .Include(x => x.BuyerRfqRevision)
            .SingleOrDefaultAsync(
                x => x.Id == buyerRfqDeliveryId,
                cancellationToken);

        if (delivery is null)
        {
            return Result.Fail(
                new NotFoundError(
                    nameof(BuyerRfqDelivery),
                    buyerRfqDeliveryId));
        }

        var editableResult = ValidateRevisionIsEditable(
            delivery.BuyerRfqRevision);

        if (editableResult.IsFailed)
        {
            return editableResult;
        }

        _db.Entry(delivery)
            .Property(x => x.RowVersion)
            .OriginalValue = rowVersion;

        TouchRevision(delivery.BuyerRfqRevision);

        _db.Set<BuyerRfqDelivery>()
            .Remove(delivery);

        return await SaveChangesWithConcurrencyAsync(
            nameof(BuyerRfqDelivery),
            delivery.Id,
            cancellationToken);
    }

    /// <summary>
    /// Allokerar en RFQ-rad från samma revision till ett leveranstillfälle.
    /// </summary>
    public async Task<Result<BuyerRfqDeliveryResponse>> AddLineAllocationAsync(
        Guid buyerRfqDeliveryId,
        CreateBuyerRfqDeliveryLineAllocationRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(
            request,
            cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<BuyerRfqDeliveryResponse>(
                validationResult.Errors);
        }

        var delivery = await _db.Set<BuyerRfqDelivery>()
            .Include(x => x.BuyerRfqRevision)
            .SingleOrDefaultAsync(
                x => x.Id == buyerRfqDeliveryId,
                cancellationToken);

        if (delivery is null)
        {
            return Result.Fail<BuyerRfqDeliveryResponse>(
                new NotFoundError(
                    nameof(BuyerRfqDelivery),
                    buyerRfqDeliveryId));
        }

        var editableResult = ValidateRevisionIsEditable(
            delivery.BuyerRfqRevision);

        if (editableResult.IsFailed)
        {
            return Result.Fail<BuyerRfqDeliveryResponse>(
                editableResult.Errors);
        }

        var line = await _db.Set<BuyerRfqLine>()
            .AsNoTracking()
            .Where(x => x.Id == request.BuyerRfqLineId)
            .Select(x => new
            {
                x.Id,
                x.BuyerRfqRevisionId
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (line is null)
        {
            return Result.Fail<BuyerRfqDeliveryResponse>(
                new NotFoundError(
                    nameof(BuyerRfqLine),
                    request.BuyerRfqLineId));
        }

        if (line.BuyerRfqRevisionId != delivery.BuyerRfqRevisionId)
        {
            return Result.Fail<BuyerRfqDeliveryResponse>(
                new ConflictError(
                    "The BuyerRfqLine and BuyerRfqDelivery must belong to the same BuyerRfqRevision."));
        }

        var quantityUnitResult = await ValidateQuantityUnitAsync(
            request.QuantityUnitId,
            cancellationToken);

        if (quantityUnitResult.IsFailed)
        {
            return Result.Fail<BuyerRfqDeliveryResponse>(
                quantityUnitResult.Errors);
        }

        var allocationExists = await _db.Set<BuyerRfqDeliveryLineAllocation>()
            .AsNoTracking()
            .AnyAsync(
                x => x.BuyerRfqDeliveryId == delivery.Id
                    && x.BuyerRfqLineId == line.Id,
                cancellationToken);

        if (allocationExists)
        {
            return Result.Fail<BuyerRfqDeliveryResponse>(
                new ConflictError(
                    "The BuyerRfqLine is already allocated to this BuyerRfqDelivery."));
        }

        var allocation = new BuyerRfqDeliveryLineAllocation
        {
            Id = Guid.CreateVersion7(),
            BuyerRfqRevisionId = delivery.BuyerRfqRevisionId,
            BuyerRfqDeliveryId = delivery.Id,
            BuyerRfqLineId = line.Id,
            Quantity = request.Quantity,
            QuantityUnitId = request.QuantityUnitId
        };

        _db.Set<BuyerRfqDeliveryLineAllocation>()
            .Add(allocation);

        TouchRevision(delivery.BuyerRfqRevision);

        var saveResult = await SaveChangesWithConcurrencyAsync(
            nameof(BuyerRfqRevision),
            delivery.BuyerRfqRevision.Id,
            cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<BuyerRfqDeliveryResponse>(saveResult.Errors);
        }

        return Result.Ok(
            await ProjectRequiredAsync(
                delivery.Id,
                cancellationToken));
    }

    /// <summary>
    /// Uppdaterar kvantitet och enhet för en befintlig delivery-line-allokering.
    /// </summary>
    public async Task<Result<BuyerRfqDeliveryResponse>> UpdateLineAllocationAsync(
        Guid buyerRfqDeliveryLineAllocationId,
        UpdateBuyerRfqDeliveryLineAllocationRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(
            request,
            cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<BuyerRfqDeliveryResponse>(
                validationResult.Errors);
        }

        var allocation = await _db.Set<BuyerRfqDeliveryLineAllocation>()
            .Include(x => x.BuyerRfqDelivery)
                .ThenInclude(x => x.BuyerRfqRevision)
            .SingleOrDefaultAsync(
                x => x.Id == buyerRfqDeliveryLineAllocationId,
                cancellationToken);

        if (allocation is null)
        {
            return Result.Fail<BuyerRfqDeliveryResponse>(
                new NotFoundError(
                    nameof(BuyerRfqDeliveryLineAllocation),
                    buyerRfqDeliveryLineAllocationId));
        }

        var editableResult = ValidateRevisionIsEditable(
            allocation.BuyerRfqDelivery.BuyerRfqRevision);

        if (editableResult.IsFailed)
        {
            return Result.Fail<BuyerRfqDeliveryResponse>(
                editableResult.Errors);
        }

        var quantityUnitResult = await ValidateQuantityUnitAsync(
            request.QuantityUnitId,
            cancellationToken);

        if (quantityUnitResult.IsFailed)
        {
            return Result.Fail<BuyerRfqDeliveryResponse>(
                quantityUnitResult.Errors);
        }

        allocation.Quantity = request.Quantity;
        allocation.QuantityUnitId = request.QuantityUnitId;

        TouchRevision(allocation.BuyerRfqDelivery.BuyerRfqRevision);

        var saveResult = await SaveWithConcurrencyAsync(
            allocation,
            allocation.Id,
            request.RowVersion,
            cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<BuyerRfqDeliveryResponse>(
                saveResult.Errors);
        }

        return Result.Ok(
            await ProjectRequiredAsync(
                allocation.BuyerRfqDeliveryId,
                cancellationToken));
    }

    /// <summary>
    /// Tar bort en delivery-line-allokering med optimistic concurrency.
    /// </summary>
    public async Task<Result<BuyerRfqDeliveryResponse>> DeleteLineAllocationAsync(
        Guid buyerRfqDeliveryLineAllocationId,
        uint rowVersion,
        CancellationToken cancellationToken = default)
    {
        var allocation = await _db.Set<BuyerRfqDeliveryLineAllocation>()
            .Include(x => x.BuyerRfqDelivery)
                .ThenInclude(x => x.BuyerRfqRevision)
            .SingleOrDefaultAsync(
                x => x.Id == buyerRfqDeliveryLineAllocationId,
                cancellationToken);

        if (allocation is null)
        {
            return Result.Fail<BuyerRfqDeliveryResponse>(
                new NotFoundError(
                    nameof(BuyerRfqDeliveryLineAllocation),
                    buyerRfqDeliveryLineAllocationId));
        }

        var editableResult = ValidateRevisionIsEditable(
            allocation.BuyerRfqDelivery.BuyerRfqRevision);

        if (editableResult.IsFailed)
        {
            return Result.Fail<BuyerRfqDeliveryResponse>(
                editableResult.Errors);
        }

        var deliveryId = allocation.BuyerRfqDeliveryId;

        _db.Entry(allocation)
            .Property(x => x.RowVersion)
            .OriginalValue = rowVersion;

        TouchRevision(allocation.BuyerRfqDelivery.BuyerRfqRevision);

        _db.Set<BuyerRfqDeliveryLineAllocation>()
            .Remove(allocation);

        var saveResult = await SaveChangesWithConcurrencyAsync(
            nameof(BuyerRfqDeliveryLineAllocation),
            allocation.Id,
            cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<BuyerRfqDeliveryResponse>(
                saveResult.Errors);
        }

        return Result.Ok(
            await ProjectRequiredAsync(
                deliveryId,
                cancellationToken));
    }

    /// <summary>
    /// Markerar parent-revisionen så att delivery-ändringar konkurrerar med release på samma RowVersion.
    /// </summary>
    private static void TouchRevision(BuyerRfqRevision revision)
    {
        revision.ActionDateUtc = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Kontrollerar att revisionen fortfarande är en redigerbar Draft.
    /// </summary>
    private static Result ValidateRevisionIsEditable(
        BuyerRfqRevision revision)
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
    /// Kontrollerar att leveransnumret inte redan används inom revisionen.
    /// </summary>
    private async Task<Result> ValidateDeliveryNumberAsync(
        Guid buyerRfqRevisionId,
        int deliveryNumber,
        Guid? excludeDeliveryId,
        CancellationToken cancellationToken)
    {
        IQueryable<BuyerRfqDelivery> query = _db.Set<BuyerRfqDelivery>()
            .AsNoTracking()
            .Where(x =>
                x.BuyerRfqRevisionId == buyerRfqRevisionId
                && x.DeliveryNumber == deliveryNumber);

        if (excludeDeliveryId.HasValue)
        {
            query = query.Where(
                x => x.Id != excludeDeliveryId.Value);
        }

        if (!await query.AnyAsync(cancellationToken))
        {
            return Result.Ok();
        }

        return Result.Fail(
            new ConflictError(
                $"Delivery number '{deliveryNumber}' already exists in the BuyerRfqRevision."));
    }

    /// <summary>
    /// Kontrollerar optional destination-, location- och Incoterm-referenser.
    /// </summary>
    private async Task<Result> ValidateDeliveryReferencesAsync(
        Guid? destinationAddressId,
        Guid? destinationLocationId,
        int? incotermRuleId,
        CancellationToken cancellationToken)
    {
        if (destinationAddressId.HasValue)
        {
            var addressExists = await _db.Set<Address>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == destinationAddressId.Value,
                    cancellationToken);

            if (!addressExists)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(Address),
                        destinationAddressId.Value));
            }
        }

        if (destinationLocationId.HasValue)
        {
            var locationExists = await _db.Set<GeographicLocation>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == destinationLocationId.Value,
                    cancellationToken);

            if (!locationExists)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(GeographicLocation),
                        destinationLocationId.Value));
            }
        }

        if (incotermRuleId.HasValue)
        {
            var incotermExists = await _db.Set<IncotermRule>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == incotermRuleId.Value,
                    cancellationToken);

            if (!incotermExists)
            {
                return Result.Fail(
                    new NotFoundError(
                        nameof(IncotermRule),
                        incotermRuleId.Value));
            }
        }

        return Result.Ok();
    }

    /// <summary>
    /// Kontrollerar att angiven kvantitetsenhet finns.
    /// </summary>
    private async Task<Result> ValidateQuantityUnitAsync(
        int quantityUnitId,
        CancellationToken cancellationToken)
    {
        var quantityUnitExists = await _db.Set<UnitOfMeasure>()
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == quantityUnitId,
                cancellationToken);

        if (!quantityUnitExists)
        {
            return Result.Fail(
                new NotFoundError(
                    nameof(UnitOfMeasure),
                    quantityUnitId));
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
        _db.Entry(entity)
            .Property("RowVersion")
            .OriginalValue = expectedRowVersion;

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
            await _db.SaveChangesAsync(
                cancellationToken);

            return Result.Ok();
        }
        catch (DbUpdateConcurrencyException exception)
        {
            var revision = exception.Entries
                .Select(x => x.Entity)
                .OfType<BuyerRfqRevision>()
                .FirstOrDefault();

            return revision is not null
                ? Result.Fail(new ConcurrencyError(nameof(BuyerRfqRevision), revision.Id))
                : Result.Fail(new ConcurrencyError(entityName, entityId));
        }
    }

    /// <summary>
    /// Projekterar ett leveranstillfälle direkt från databasen.
    /// </summary>
    private async Task<BuyerRfqDeliveryResponse?> ProjectAsync(
        Guid buyerRfqDeliveryId,
        CancellationToken cancellationToken)
    {
        return await _db.Set<BuyerRfqDelivery>()
            .AsNoTracking()
            .Where(x => x.Id == buyerRfqDeliveryId)
            .Select(BuyerRfqDeliveryProjections.Details)
            .SingleOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Projekterar ett sparat leveranstillfälle och kastar om det oväntat saknas.
    /// </summary>
    private async Task<BuyerRfqDeliveryResponse> ProjectRequiredAsync(
        Guid buyerRfqDeliveryId,
        CancellationToken cancellationToken)
    {
        return await ProjectAsync(
                buyerRfqDeliveryId,
                cancellationToken)
            ?? throw new InvalidOperationException(
                $"BuyerRfqDelivery '{buyerRfqDeliveryId}' was saved but could not be projected.");
    }

    /// <summary>
    /// Trimmar text och ersätter tomma värden med null.
    /// </summary>
    private static string? TrimToNull(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}