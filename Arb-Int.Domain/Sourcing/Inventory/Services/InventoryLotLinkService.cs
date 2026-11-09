using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Sourcing.Projections;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Hanterar riktad lot-provenance med child-, parent- och link-concurrency samt cykelskydd.
    /// </summary>
    public sealed class InventoryLotLinkService : IInventoryLotLinkService
    {
        private readonly Arb_IntDbContext _db;
        private readonly IValidationService _validationService;

        /// <summary>
        /// Skapar servicen med databas- och valideringsberoenden.
        /// </summary>
        public InventoryLotLinkService(Arb_IntDbContext db, IValidationService validationService)
        {
            _db = db;
            _validationService = validationService;
        }

        /// <summary>
        /// Skapar en parent-till-child-länk efter referens-, dubblett- och cykelkontroll.
        /// </summary>
        public async Task<Result<InventoryLotResponse>> AddAsync(Guid childInventoryLotId, CreateInventoryLotLinkRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<InventoryLotResponse>(
                    validationResult.Errors);
            }

            if (request.ParentInventoryLotId == childInventoryLotId)
            {
                return Result.Fail<InventoryLotResponse>(
                    new ConflictError(
                        "An InventoryLot cannot be linked to itself."));
            }

            var lots = await _db.Set<InventoryLot>()
                .Where(
                    x => x.Id == request.ParentInventoryLotId
                        || x.Id == childInventoryLotId)
                .ToListAsync(cancellationToken);

            var parentLot = lots.SingleOrDefault(
                x => x.Id == request.ParentInventoryLotId);

            if (parentLot is null)
            {
                return Result.Fail<InventoryLotResponse>(
                    new NotFoundError(
                        nameof(InventoryLot),
                        request.ParentInventoryLotId));
            }

            var childLot = lots.SingleOrDefault(
                x => x.Id == childInventoryLotId);

            if (childLot is null)
            {
                return Result.Fail<InventoryLotResponse>(
                    new NotFoundError(
                        nameof(InventoryLot),
                        childInventoryLotId));
            }

            var referencesResult = await ValidateReferencesAsync(
                request.QuantityUnitId,
                request.MaterialTransformationId,
                cancellationToken);

            if (referencesResult.IsFailed)
            {
                return Result.Fail<InventoryLotResponse>(
                    referencesResult.Errors);
            }

            var duplicateExists = await _db.Set<InventoryLotLink>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.ParentInventoryLotId == parentLot.Id
                        && x.ChildInventoryLotId == childLot.Id
                        && x.Kind == request.Kind,
                    cancellationToken);

            if (duplicateExists)
            {
                return Result.Fail<InventoryLotResponse>(
                    new ConflictError(
                        "The same parent, child and provenance kind already exists."));
            }

            var cycleResult = await ValidateAcyclicAsync(
                parentLot.Id,
                childLot.Id,
                cancellationToken);

            if (cycleResult.IsFailed)
            {
                return Result.Fail<InventoryLotResponse>(
                    cycleResult.Errors);
            }

            _db.Entry(parentLot)
                .Property(x => x.RowVersion)
                .OriginalValue = request.ParentInventoryLotRowVersion;

            _db.Entry(childLot)
                .Property(x => x.RowVersion)
                .OriginalValue = request.ChildInventoryLotRowVersion;

            var link = new InventoryLotLink
            {
                Id = Guid.CreateVersion7(),
                ParentInventoryLotId = parentLot.Id,
                ChildInventoryLotId = childLot.Id,
                Kind = request.Kind,
                InputQuantity = request.InputQuantity,
                QuantityUnitId = request.QuantityUnitId,
                YieldPercent = request.YieldPercent,
                MaterialTransformationId = request.MaterialTransformationId,
                Notes = TrimToNull(request.Notes)
            };

            _db.Set<InventoryLotLink>()
                .Add(link);

            TouchLot(parentLot);
            TouchLot(childLot);

            var saveResult = await SaveChangesAsync(
                link.Id,
                parentLot.Id,
                childLot.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<InventoryLotResponse>(
                    saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    childLot.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Uppdaterar provenance-metadata och behåller parent- och child-identiteten oförändrad.
        /// </summary>
        public async Task<Result<InventoryLotResponse>> UpdateAsync(Guid inventoryLotLinkId, UpdateInventoryLotLinkRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validationService.ValidateAsync(
                request,
                cancellationToken);

            if (validationResult.IsFailed)
            {
                return Result.Fail<InventoryLotResponse>(
                    validationResult.Errors);
            }

            var link = await _db.Set<InventoryLotLink>()
                .SingleOrDefaultAsync(
                    x => x.Id == inventoryLotLinkId,
                    cancellationToken);

            if (link is null)
            {
                return Result.Fail<InventoryLotResponse>(
                    new NotFoundError(
                        nameof(InventoryLotLink),
                        inventoryLotLinkId));
            }

            var lots = await _db.Set<InventoryLot>()
                .Where(
                    x => x.Id == link.ParentInventoryLotId
                        || x.Id == link.ChildInventoryLotId)
                .ToListAsync(cancellationToken);

            var parentLot = lots.SingleOrDefault(
                x => x.Id == link.ParentInventoryLotId);

            if (parentLot is null)
            {
                return Result.Fail<InventoryLotResponse>(
                    new NotFoundError(
                        nameof(InventoryLot),
                        link.ParentInventoryLotId));
            }

            var childLot = lots.SingleOrDefault(
                x => x.Id == link.ChildInventoryLotId);

            if (childLot is null)
            {
                return Result.Fail<InventoryLotResponse>(
                    new NotFoundError(
                        nameof(InventoryLot),
                        link.ChildInventoryLotId));
            }

            var referencesResult = await ValidateReferencesAsync(
                request.QuantityUnitId,
                request.MaterialTransformationId,
                cancellationToken);

            if (referencesResult.IsFailed)
            {
                return Result.Fail<InventoryLotResponse>(
                    referencesResult.Errors);
            }

            var duplicateExists = await _db.Set<InventoryLotLink>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id != link.Id
                        && x.ParentInventoryLotId == parentLot.Id
                        && x.ChildInventoryLotId == childLot.Id
                        && x.Kind == request.Kind,
                    cancellationToken);

            if (duplicateExists)
            {
                return Result.Fail<InventoryLotResponse>(
                    new ConflictError(
                        "The same parent, child and provenance kind already exists."));
            }

            var notes = TrimToNull(request.Notes);

            if (link.Kind == request.Kind
                && link.InputQuantity == request.InputQuantity
                && link.QuantityUnitId == request.QuantityUnitId
                && link.YieldPercent == request.YieldPercent
                && link.MaterialTransformationId == request.MaterialTransformationId
                && link.Notes == notes)
            {
                return Result.Ok(
                    await ProjectRequiredAsync(
                        childLot.Id,
                        cancellationToken));
            }

            _db.Entry(link)
                .Property(x => x.RowVersion)
                .OriginalValue = request.RowVersion;

            _db.Entry(parentLot)
                .Property(x => x.RowVersion)
                .OriginalValue = request.ParentInventoryLotRowVersion;

            _db.Entry(childLot)
                .Property(x => x.RowVersion)
                .OriginalValue = request.ChildInventoryLotRowVersion;

            link.Kind = request.Kind;
            link.InputQuantity = request.InputQuantity;
            link.QuantityUnitId = request.QuantityUnitId;
            link.YieldPercent = request.YieldPercent;
            link.MaterialTransformationId = request.MaterialTransformationId;
            link.Notes = notes;

            TouchLot(parentLot);
            TouchLot(childLot);

            var saveResult = await SaveChangesAsync(
                link.Id,
                parentLot.Id,
                childLot.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<InventoryLotResponse>(
                    saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    childLot.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Tar bort provenance-länken och concurrency-skyddar länken samt båda lot-roots.
        /// </summary>
        public async Task<Result<InventoryLotResponse>> DeleteAsync(Guid inventoryLotLinkId, uint rowVersion, uint parentInventoryLotRowVersion, uint childInventoryLotRowVersion, CancellationToken cancellationToken = default)
        {
            var link = await _db.Set<InventoryLotLink>()
                .SingleOrDefaultAsync(
                    x => x.Id == inventoryLotLinkId,
                    cancellationToken);

            if (link is null)
            {
                return Result.Fail<InventoryLotResponse>(
                    new NotFoundError(
                        nameof(InventoryLotLink),
                        inventoryLotLinkId));
            }

            var lots = await _db.Set<InventoryLot>()
                .Where(
                    x => x.Id == link.ParentInventoryLotId
                        || x.Id == link.ChildInventoryLotId)
                .ToListAsync(cancellationToken);

            var parentLot = lots.SingleOrDefault(
                x => x.Id == link.ParentInventoryLotId);

            if (parentLot is null)
            {
                return Result.Fail<InventoryLotResponse>(
                    new NotFoundError(
                        nameof(InventoryLot),
                        link.ParentInventoryLotId));
            }

            var childLot = lots.SingleOrDefault(
                x => x.Id == link.ChildInventoryLotId);

            if (childLot is null)
            {
                return Result.Fail<InventoryLotResponse>(
                    new NotFoundError(
                        nameof(InventoryLot),
                        link.ChildInventoryLotId));
            }

            _db.Entry(link)
                .Property(x => x.RowVersion)
                .OriginalValue = rowVersion;

            _db.Entry(parentLot)
                .Property(x => x.RowVersion)
                .OriginalValue = parentInventoryLotRowVersion;

            _db.Entry(childLot)
                .Property(x => x.RowVersion)
                .OriginalValue = childInventoryLotRowVersion;

            _db.Set<InventoryLotLink>()
                .Remove(link);

            TouchLot(parentLot);
            TouchLot(childLot);

            var saveResult = await SaveChangesAsync(
                link.Id,
                parentLot.Id,
                childLot.Id,
                cancellationToken);

            if (saveResult.IsFailed)
            {
                return Result.Fail<InventoryLotResponse>(
                    saveResult.Errors);
            }

            return Result.Ok(
                await ProjectRequiredAsync(
                    childLot.Id,
                    cancellationToken));
        }

        /// <summary>
        /// Kontrollerar optional quantity unit och material transformation.
        /// </summary>
        private async Task<Result> ValidateReferencesAsync(int? quantityUnitId, Guid? materialTransformationId, CancellationToken cancellationToken)
        {
            if (quantityUnitId.HasValue)
            {
                var unitExists = await _db.Set<UnitOfMeasure>()
                    .AsNoTracking()
                    .AnyAsync(
                        x => x.Id == quantityUnitId.Value,
                        cancellationToken);

                if (!unitExists)
                {
                    return Result.Fail(
                        new NotFoundError(
                            nameof(UnitOfMeasure),
                            quantityUnitId.Value));
                }
            }

            if (materialTransformationId.HasValue)
            {
                var transformationExists = await _db.Set<MaterialTransformation>()
                    .AsNoTracking()
                    .AnyAsync(
                        x => x.Id == materialTransformationId.Value,
                        cancellationToken);

                if (!transformationExists)
                {
                    return Result.Fail(
                        new NotFoundError(
                            nameof(MaterialTransformation),
                            materialTransformationId.Value));
                }
            }

            return Result.Ok();
        }

        /// <summary>
        /// Förhindrar att den nya parent-till-child-länken skapar en cykel i provenance-grafen.
        /// </summary>
        private async Task<Result> ValidateAcyclicAsync(Guid parentInventoryLotId, Guid childInventoryLotId, CancellationToken cancellationToken)
        {
            var edges = await _db.Set<InventoryLotLink>()
                .AsNoTracking()
                .Select(
                    x => new
                    {
                        x.ParentInventoryLotId,
                        x.ChildInventoryLotId
                    })
                .ToListAsync(cancellationToken);

            var childrenByParent = edges
                .GroupBy(x => x.ParentInventoryLotId)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(x => x.ChildInventoryLotId).Distinct().ToArray());

            var visited = new HashSet<Guid>();
            var pending = new Stack<Guid>();
            pending.Push(childInventoryLotId);

            while (pending.Count > 0)
            {
                var current = pending.Pop();

                if (!visited.Add(current))
                {
                    continue;
                }

                if (current == parentInventoryLotId)
                {
                    return Result.Fail(
                        new ConflictError(
                            "The InventoryLotLink would create a cycle in lot provenance."));
                }

                if (!childrenByParent.TryGetValue(current, out var children))
                {
                    continue;
                }

                foreach (var child in children)
                {
                    pending.Push(child);
                }
            }

            return Result.Ok();
        }

        /// <summary>
        /// Markerar lot-roten så att provenance-writes konkurrerar med övriga aggregate-writes.
        /// </summary>
        private static void TouchLot(InventoryLot lot)
        {
            lot.ActionDateUtc = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Sparar provenance-ändringen och skiljer link-, parent- och child-concurrency åt.
        /// </summary>
        private async Task<Result> SaveChangesAsync(Guid inventoryLotLinkId, Guid parentInventoryLotId, Guid childInventoryLotId, CancellationToken cancellationToken)
        {
            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                return Result.Ok();
            }
            catch (DbUpdateConcurrencyException exception)
            {
                if (exception.Entries.Any(x => x.Entity is InventoryLotLink))
                {
                    return Result.Fail(
                        new ConcurrencyError(
                            nameof(InventoryLotLink),
                            inventoryLotLinkId));
                }

                if (exception.Entries.Any(
                        x => x.Entity is InventoryLot lot
                            && lot.Id == parentInventoryLotId))
                {
                    return Result.Fail(
                        new ConcurrencyError(
                            nameof(InventoryLot),
                            parentInventoryLotId));
                }

                return Result.Fail(
                    new ConcurrencyError(
                        nameof(InventoryLot),
                        childInventoryLotId));
            }
            catch (DbUpdateException exception)
                when (IsUniqueViolation(exception))
            {
                return Result.Fail(
                    new ConflictError(
                        "The same parent, child and provenance kind already exists."));
            }
        }

        /// <summary>
        /// Projekterar child-loten med origins och provenance-länkar.
        /// </summary>
        private async Task<InventoryLotResponse?> ProjectAsync(Guid inventoryLotId, CancellationToken cancellationToken)
        {
            return await _db.Set<InventoryLot>()
                .AsNoTracking()
                .Where(x => x.Id == inventoryLotId)
                .Select(InventoryLotProjections.Details)
                .SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Projekterar en sparad child-lot och kastar om den oväntat saknas.
        /// </summary>
        private async Task<InventoryLotResponse> ProjectRequiredAsync(Guid inventoryLotId, CancellationToken cancellationToken)
        {
            return await ProjectAsync(
                    inventoryLotId,
                    cancellationToken)
                ?? throw new InvalidOperationException(
                    $"InventoryLot '{inventoryLotId}' was saved but could not be projected.");
        }

        /// <summary>
        /// Kontrollerar om databasfelet kommer från en unik constraint.
        /// </summary>
        private static bool IsUniqueViolation(DbUpdateException exception)
        {
            return exception.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation
            };
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
}
