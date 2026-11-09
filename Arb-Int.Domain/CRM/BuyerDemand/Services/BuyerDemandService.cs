using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Parties;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Crm;

/// <summary>
/// Hanterar BuyerDemand som ett aggregat med scopes, evidence och explicit state history.
/// </summary>
public sealed class BuyerDemandService : IBuyerDemandService
{
    private readonly Arb_IntDbContext _db;
    private readonly IValidationService _validationService;

    /// <summary>
    /// Skapar BuyerDemand-servicen med databas- och valideringsberoenden.
    /// </summary>
    public BuyerDemandService(Arb_IntDbContext db, IValidationService validationService)
    {
        _db = db;
        _validationService = validationService;
    }

    /// <summary>
    /// Hämtar en BuyerDemand direkt som ett projekterat response-objekt.
    /// </summary>
    public async Task<Result<BuyerDemandResponse>> GetByIdAsync(Guid buyerDemandId, CancellationToken cancellationToken = default)
    {
        var response = await ProjectAsync(buyerDemandId, cancellationToken);

        if (response is null)
        {
            return Result.Fail<BuyerDemandResponse>(
                new NotFoundError(nameof(BuyerDemand), buyerDemandId));
        }

        return Result.Ok(response);
    }

    /// <summary>
    /// Söker, filtrerar, sorterar och paginerar BuyerDemand för en intern organisation.
    /// </summary>
    public async Task<Result<PagedResult<BuyerDemandListResponse>>> QueryAsync(
        Guid owningOrganizationId,
        BuyerDemandQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(
            request,
            cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<PagedResult<BuyerDemandListResponse>>(
                validationResult.Errors);
        }

        var organizationExists = await _db.Set<Organization>()
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == owningOrganizationId,
                cancellationToken);

        if (!organizationExists)
        {
            return Result.Fail<PagedResult<BuyerDemandListResponse>>(
                new NotFoundError(
                    nameof(Organization),
                    owningOrganizationId));
        }

        IQueryable<BuyerDemand> query = _db.Set<BuyerDemand>()
            .AsNoTracking()
            .Where(x =>
                x.BusinessRelationship.OwningOrganizationId == owningOrganizationId);

        if (request.BusinessRelationshipId.HasValue)
        {
            query = query.Where(x =>
                x.BusinessRelationshipId == request.BusinessRelationshipId.Value);
        }

        if (request.CounterpartyId.HasValue)
        {
            query = query.Where(x =>
                x.BusinessRelationship.CounterpartyId == request.CounterpartyId.Value);
        }

        if (request.KnowledgeLevel.HasValue)
        {
            query = query.Where(x =>
                x.KnowledgeLevel == request.KnowledgeLevel.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(x =>
                x.Status == request.Status.Value);
        }

        if (request.OwnerUserId.HasValue)
        {
            query = query.Where(x =>
                x.OwnerUserId == request.OwnerUserId.Value);
        }

        if (request.MinConfidence.HasValue)
        {
            query = query.Where(x =>
                x.Confidence.HasValue
                && x.Confidence.Value >= request.MinConfidence.Value);
        }

        if (request.MaxConfidence.HasValue)
        {
            query = query.Where(x =>
                x.Confidence.HasValue
                && x.Confidence.Value <= request.MaxConfidence.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchPattern =
                $"%{request.Search.Trim()}%";

            query = query.Where(x =>
                EF.Functions.ILike(
                    x.Name,
                    searchPattern)
                || EF.Functions.ILike(
                    x.BusinessRelationship.Counterparty.Name,
                    searchPattern));
        }

        var totalCount = await query.CountAsync(
            cancellationToken);

        query = ApplySorting(
            query,
            request.SortBy,
            request.SortDirection);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(BuyerDemandProjections.List)
            .ToListAsync(cancellationToken);

        return Result.Ok(
            new PagedResult<BuyerDemandListResponse>
            {
                Items = items,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            });
    }

    /// <summary>
    /// Hämtar den affärsmässiga state-historiken i kronologisk ordning.
    /// </summary>
    public async Task<Result<IReadOnlyList<BuyerDemandStateHistoryResponse>>> GetStateHistoryAsync(
        Guid buyerDemandId,
        CancellationToken cancellationToken = default)
    {
        var exists = await _db.Set<BuyerDemand>()
            .AsNoTracking()
            .AnyAsync(x => x.Id == buyerDemandId, cancellationToken);

        if (!exists)
        {
            return Result.Fail<IReadOnlyList<BuyerDemandStateHistoryResponse>>(
                new NotFoundError(nameof(BuyerDemand), buyerDemandId));
        }

        var history = await _db.Set<BuyerDemandStateHistory>()
            .AsNoTracking()
            .Where(x => x.BuyerDemandId == buyerDemandId)
            .OrderBy(x => x.ChangedAtUtc)
            .ThenBy(x => x.Id)
            .Select(BuyerDemandProjections.StateHistory)
            .ToListAsync(cancellationToken);

        return Result.Ok<IReadOnlyList<BuyerDemandStateHistoryResponse>>(history);
    }

    /// <summary>
    /// Skapar en BuyerDemand och dess initiala state history atomiskt.
    /// </summary>
    public async Task<Result<BuyerDemandResponse>> CreateAsync(
        Guid businessRelationshipId,
        CreateBuyerDemandRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<BuyerDemandResponse>(validationResult.Errors);
        }

        var relationship = await _db.Set<BusinessRelationship>()
            .AsNoTracking()
            .Where(x => x.Id == businessRelationshipId)
            .Select(x => new
            {
                x.Id,
                RelationshipTypeCode = x.BusinessRelationshipType.Code
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (relationship is null)
        {
            return Result.Fail<BuyerDemandResponse>(
                new NotFoundError(nameof(BusinessRelationship), businessRelationshipId));
        }

        if (relationship.RelationshipTypeCode != BusinessRelationshipTypeCodes.Buyer)
        {
            return Result.Fail<BuyerDemandResponse>(
                new ConflictError(
                    "BuyerDemand can only be created on a buyer business relationship."));
        }

        var changedAtUtc = DateTimeOffset.UtcNow;

        var demand = new BuyerDemand
        {
            Id = Guid.CreateVersion7(),
            BusinessRelationshipId = businessRelationshipId,
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            KnowledgeLevel = request.KnowledgeLevel,
            Status = request.Status,
            Confidence = request.Confidence,
            ValidFromUtc = request.ValidFromUtc,
            ValidToUtc = request.ValidToUtc,
            LastConfirmedAtUtc = request.KnowledgeLevel == BuyerDemandKnowledgeLevel.Confirmed ? changedAtUtc : null,
            OwnerUserId = request.OwnerUserId,
            InternalNotes = TrimToNull(request.InternalNotes)
        };

        var history = new BuyerDemandStateHistory
        {
            Id = Guid.CreateVersion7(),
            BuyerDemandId = demand.Id,
            FromKnowledgeLevel = null,
            ToKnowledgeLevel = request.KnowledgeLevel,
            FromStatus = null,
            ToStatus = request.Status,
            Confidence = request.Confidence,
            ChangedAtUtc = changedAtUtc,
            ChangedByUserId = request.InitialStateChangedByUserId,
            Notes = TrimToNull(request.InitialStateNotes)
        };

        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

        _db.Set<BuyerDemand>().Add(demand);
        _db.Set<BuyerDemandStateHistory>().Add(history);

        await _db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return Result.Ok(await ProjectRequiredAsync(demand.Id, cancellationToken));
    }

    /// <summary>
    /// Uppdaterar metadata med optimistic concurrency utan att ändra demandens state.
    /// </summary>
    public async Task<Result<BuyerDemandResponse>> UpdateAsync(
        Guid buyerDemandId,
        UpdateBuyerDemandRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<BuyerDemandResponse>(validationResult.Errors);
        }

        var demand = await _db.Set<BuyerDemand>()
            .SingleOrDefaultAsync(x => x.Id == buyerDemandId, cancellationToken);

        if (demand is null)
        {
            return Result.Fail<BuyerDemandResponse>(
                new NotFoundError(nameof(BuyerDemand), buyerDemandId));
        }

        demand.Name = request.Name.Trim();
        demand.Description = request.Description.Trim();
        demand.Confidence = request.Confidence;
        demand.ValidFromUtc = request.ValidFromUtc;
        demand.ValidToUtc = request.ValidToUtc;
        demand.OwnerUserId = request.OwnerUserId;
        demand.InternalNotes = TrimToNull(request.InternalNotes);

        var saveResult = await SaveWithConcurrencyAsync(
            demand,
            demand.Id,
            request.RowVersion,
            cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<BuyerDemandResponse>(saveResult.Errors);
        }

        return Result.Ok(await ProjectRequiredAsync(demand.Id, cancellationToken));
    }

    /// <summary>
    /// Ändrar KnowledgeLevel och Status och skriver motsvarande state history i samma transaktion.
    /// </summary>
    public async Task<Result<BuyerDemandResponse>> ChangeStateAsync(
        Guid buyerDemandId,
        ChangeBuyerDemandStateRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<BuyerDemandResponse>(validationResult.Errors);
        }

        var demand = await _db.Set<BuyerDemand>()
            .SingleOrDefaultAsync(x => x.Id == buyerDemandId, cancellationToken);

        if (demand is null)
        {
            return Result.Fail<BuyerDemandResponse>(
                new NotFoundError(nameof(BuyerDemand), buyerDemandId));
        }

        if (request.BuyerDemandEvidenceId.HasValue)
        {
            var evidenceBelongsToDemand = await _db.Set<BuyerDemandEvidence>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == request.BuyerDemandEvidenceId.Value
                        && x.BuyerDemandId == demand.Id,
                    cancellationToken);

            if (!evidenceBelongsToDemand)
            {
                return Result.Fail<BuyerDemandResponse>(
                    new ConflictError(
                        "The selected evidence does not belong to this BuyerDemand."));
            }
        }

        if (demand.KnowledgeLevel == request.KnowledgeLevel
            && demand.Status == request.Status
            && demand.Confidence == request.Confidence)
        {
            return Result.Ok(await ProjectRequiredAsync(demand.Id, cancellationToken));
        }

        var changedAtUtc = request.ChangedAtUtc ?? DateTimeOffset.UtcNow;

        var history = new BuyerDemandStateHistory
        {
            Id = Guid.CreateVersion7(),
            BuyerDemandId = demand.Id,
            FromKnowledgeLevel = demand.KnowledgeLevel,
            ToKnowledgeLevel = request.KnowledgeLevel,
            FromStatus = demand.Status,
            ToStatus = request.Status,
            BuyerDemandEvidenceId = request.BuyerDemandEvidenceId,
            Confidence = request.Confidence,
            ChangedAtUtc = changedAtUtc,
            ChangedByUserId = request.ChangedByUserId,
            Notes = TrimToNull(request.Notes)
        };

        demand.KnowledgeLevel = request.KnowledgeLevel;
        demand.Status = request.Status;
        demand.Confidence = request.Confidence;

        if (request.KnowledgeLevel == BuyerDemandKnowledgeLevel.Confirmed)
        {
            demand.LastConfirmedAtUtc = changedAtUtc;
        }

        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

        _db.Set<BuyerDemandStateHistory>().Add(history);

        var saveResult = await SaveWithConcurrencyAsync(
            demand,
            demand.Id,
            request.RowVersion,
            cancellationToken);

        if (saveResult.IsFailed)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result.Fail<BuyerDemandResponse>(saveResult.Errors);
        }

        await transaction.CommitAsync(cancellationToken);

        return Result.Ok(await ProjectRequiredAsync(demand.Id, cancellationToken));
    }

    /// <summary>
    /// Validerar referenser och lägger till ett nytt scope på demanden.
    /// </summary>
    public async Task<Result<BuyerDemandResponse>> AddScopeAsync(
        Guid buyerDemandId,
        CreateBuyerDemandScopeRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<BuyerDemandResponse>(validationResult.Errors);
        }

        var demand = await _db.Set<BuyerDemand>()
            .AsNoTracking()
            .Where(x => x.Id == buyerDemandId)
            .Select(x => new
            {
                x.Id,
                x.BusinessRelationshipId
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (demand is null)
        {
            return Result.Fail<BuyerDemandResponse>(
                new NotFoundError(nameof(BuyerDemand), buyerDemandId));
        }

        var referenceResult = await ValidateScopeReferencesAsync(
            demand.BusinessRelationshipId,
            request.BiologicalSourceClaimId,
            request.MaterialId,
            request.MaterialTypeId,
            request.IntendedUseCharacteristicId,
            request.TypicalQuantityUnitId,
            cancellationToken);

        if (referenceResult.IsFailed)
        {
            return Result.Fail<BuyerDemandResponse>(referenceResult.Errors);
        }

        var scope = new BuyerDemandScope
        {
            Id = Guid.CreateVersion7(),
            BuyerDemandId = demand.Id,
            BusinessRelationshipId = demand.BusinessRelationshipId,
            BiologicalSourceClaimId = request.BiologicalSourceClaimId,
            MaterialId = request.MaterialId,
            MaterialTypeId = request.MaterialTypeId,
            IntendedUseCharacteristicId = request.IntendedUseCharacteristicId,
            DeclaredFormat = TrimToNull(request.DeclaredFormat),
            IntendedUseText = TrimToNull(request.IntendedUseText),
            TypicalQuantity = request.TypicalQuantity,
            TypicalQuantityUnitId = request.TypicalQuantityUnitId,
            RecurrenceDescription = TrimToNull(request.RecurrenceDescription),
            SortOrder = request.SortOrder
        };

        _db.Set<BuyerDemandScope>().Add(scope);

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Ok(await ProjectRequiredAsync(demand.Id, cancellationToken));
    }

    /// <summary>
    /// Uppdaterar ett scope med optimistic concurrency och behåller demandens BusinessRelationshipId.
    /// </summary>
    public async Task<Result<BuyerDemandResponse>> UpdateScopeAsync(
        Guid buyerDemandScopeId,
        UpdateBuyerDemandScopeRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<BuyerDemandResponse>(validationResult.Errors);
        }

        var scope = await _db.Set<BuyerDemandScope>()
            .SingleOrDefaultAsync(x => x.Id == buyerDemandScopeId, cancellationToken);

        if (scope is null)
        {
            return Result.Fail<BuyerDemandResponse>(
                new NotFoundError(nameof(BuyerDemandScope), buyerDemandScopeId));
        }

        var referenceResult = await ValidateScopeReferencesAsync(
            scope.BusinessRelationshipId,
            request.BiologicalSourceClaimId,
            request.MaterialId,
            request.MaterialTypeId,
            request.IntendedUseCharacteristicId,
            request.TypicalQuantityUnitId,
            cancellationToken);

        if (referenceResult.IsFailed)
        {
            return Result.Fail<BuyerDemandResponse>(referenceResult.Errors);
        }

        scope.BiologicalSourceClaimId = request.BiologicalSourceClaimId;
        scope.MaterialId = request.MaterialId;
        scope.MaterialTypeId = request.MaterialTypeId;
        scope.IntendedUseCharacteristicId = request.IntendedUseCharacteristicId;
        scope.DeclaredFormat = TrimToNull(request.DeclaredFormat);
        scope.IntendedUseText = TrimToNull(request.IntendedUseText);
        scope.TypicalQuantity = request.TypicalQuantity;
        scope.TypicalQuantityUnitId = request.TypicalQuantityUnitId;
        scope.RecurrenceDescription = TrimToNull(request.RecurrenceDescription);
        scope.SortOrder = request.SortOrder;

        var saveResult = await SaveWithConcurrencyAsync(
            scope,
            scope.Id,
            request.RowVersion,
            cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<BuyerDemandResponse>(saveResult.Errors);
        }

        return Result.Ok(await ProjectRequiredAsync(scope.BuyerDemandId, cancellationToken));
    }

    /// <summary>
    /// Tar bort ett felaktigt scope med optimistic concurrency.
    /// </summary>
    public async Task<Result<BuyerDemandResponse>> DeleteScopeAsync(
        Guid buyerDemandScopeId,
        uint rowVersion,
        CancellationToken cancellationToken = default)
    {
        var scope = await _db.Set<BuyerDemandScope>()
            .SingleOrDefaultAsync(x => x.Id == buyerDemandScopeId, cancellationToken);

        if (scope is null)
        {
            return Result.Fail<BuyerDemandResponse>(
                new NotFoundError(nameof(BuyerDemandScope), buyerDemandScopeId));
        }

        var demandId = scope.BuyerDemandId;

        _db.Entry(scope).Property(x => x.RowVersion).OriginalValue = rowVersion;
        _db.Set<BuyerDemandScope>().Remove(scope);

        var saveResult = await SaveChangesWithConcurrencyAsync(
            nameof(BuyerDemandScope),
            scope.Id,
            cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<BuyerDemandResponse>(saveResult.Errors);
        }

        return Result.Ok(await ProjectRequiredAsync(demandId, cancellationToken));
    }

    /// <summary>
    /// Validerar referenser och lägger till evidens på en BuyerDemand.
    /// </summary>
    public async Task<Result<BuyerDemandResponse>> AddEvidenceAsync(
        Guid buyerDemandId,
        CreateBuyerDemandEvidenceRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<BuyerDemandResponse>(validationResult.Errors);
        }

        var demandExists = await _db.Set<BuyerDemand>()
            .AsNoTracking()
            .AnyAsync(x => x.Id == buyerDemandId, cancellationToken);

        if (!demandExists)
        {
            return Result.Fail<BuyerDemandResponse>(
                new NotFoundError(nameof(BuyerDemand), buyerDemandId));
        }

        var referenceResult = await ValidateEvidenceReferencesAsync(
            request.ReferenceEntryId,
            request.CrmActivityId,
            cancellationToken);

        if (referenceResult.IsFailed)
        {
            return Result.Fail<BuyerDemandResponse>(referenceResult.Errors);
        }

        var evidence = new BuyerDemandEvidence
        {
            Id = Guid.CreateVersion7(),
            BuyerDemandId = buyerDemandId,
            SourceKind = request.SourceKind,
            Position = request.Position,
            ReferenceEntryId = request.ReferenceEntryId,
            CrmActivityId = request.CrmActivityId,
            SourceUri = TrimToNull(request.SourceUri),
            SourceExternalId = TrimToNull(request.SourceExternalId),
            Excerpt = TrimToNull(request.Excerpt),
            Notes = TrimToNull(request.Notes),
            Confidence = request.Confidence,
            ObservedAtUtc = request.ObservedAtUtc ?? DateTimeOffset.UtcNow,
            EvidenceHash = TrimToNull(request.EvidenceHash)
        };

        _db.Set<BuyerDemandEvidence>().Add(evidence);

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Ok(await ProjectRequiredAsync(buyerDemandId, cancellationToken));
    }

    /// <summary>
    /// Uppdaterar BuyerDemand-evidens med optimistic concurrency.
    /// </summary>
    public async Task<Result<BuyerDemandResponse>> UpdateEvidenceAsync(
        Guid buyerDemandEvidenceId,
        UpdateBuyerDemandEvidenceRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<BuyerDemandResponse>(validationResult.Errors);
        }

        var evidence = await _db.Set<BuyerDemandEvidence>()
            .SingleOrDefaultAsync(x => x.Id == buyerDemandEvidenceId, cancellationToken);

        if (evidence is null)
        {
            return Result.Fail<BuyerDemandResponse>(
                new NotFoundError(nameof(BuyerDemandEvidence), buyerDemandEvidenceId));
        }

        var referenceResult = await ValidateEvidenceReferencesAsync(
            request.ReferenceEntryId,
            request.CrmActivityId,
            cancellationToken);

        if (referenceResult.IsFailed)
        {
            return Result.Fail<BuyerDemandResponse>(referenceResult.Errors);
        }

        evidence.SourceKind = request.SourceKind;
        evidence.Position = request.Position;
        evidence.ReferenceEntryId = request.ReferenceEntryId;
        evidence.CrmActivityId = request.CrmActivityId;
        evidence.SourceUri = TrimToNull(request.SourceUri);
        evidence.SourceExternalId = TrimToNull(request.SourceExternalId);
        evidence.Excerpt = TrimToNull(request.Excerpt);
        evidence.Notes = TrimToNull(request.Notes);
        evidence.Confidence = request.Confidence;
        evidence.ObservedAtUtc = request.ObservedAtUtc;
        evidence.EvidenceHash = TrimToNull(request.EvidenceHash);

        var saveResult = await SaveWithConcurrencyAsync(
            evidence,
            evidence.Id,
            request.RowVersion,
            cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<BuyerDemandResponse>(saveResult.Errors);
        }

        return Result.Ok(await ProjectRequiredAsync(evidence.BuyerDemandId, cancellationToken));
    }

    /// <summary>
    /// Tar bort felaktig evidens om den inte används av state history.
    /// </summary>
    public async Task<Result<BuyerDemandResponse>> DeleteEvidenceAsync(
        Guid buyerDemandEvidenceId,
        uint rowVersion,
        CancellationToken cancellationToken = default)
    {
        var evidence = await _db.Set<BuyerDemandEvidence>()
            .SingleOrDefaultAsync(x => x.Id == buyerDemandEvidenceId, cancellationToken);

        if (evidence is null)
        {
            return Result.Fail<BuyerDemandResponse>(
                new NotFoundError(nameof(BuyerDemandEvidence), buyerDemandEvidenceId));
        }

        var usedByHistory = await _db.Set<BuyerDemandStateHistory>()
            .AsNoTracking()
            .AnyAsync(x => x.BuyerDemandEvidenceId == evidence.Id, cancellationToken);

        if (usedByHistory)
        {
            return Result.Fail<BuyerDemandResponse>(
                new ConflictError(
                    "Evidence referenced by BuyerDemand state history cannot be deleted."));
        }

        var demandId = evidence.BuyerDemandId;

        _db.Entry(evidence).Property(x => x.RowVersion).OriginalValue = rowVersion;
        _db.Set<BuyerDemandEvidence>().Remove(evidence);

        var saveResult = await SaveChangesWithConcurrencyAsync(
            nameof(BuyerDemandEvidence),
            evidence.Id,
            cancellationToken);

        if (saveResult.IsFailed)
        {
            return Result.Fail<BuyerDemandResponse>(saveResult.Errors);
        }

        return Result.Ok(await ProjectRequiredAsync(demandId, cancellationToken));
    }

    /// <summary>
    /// Kontrollerar scope-referenser och att eventuell claim hör till samma buyer-relation.
    /// </summary>
    private async Task<Result> ValidateScopeReferencesAsync(
        Guid businessRelationshipId,
        Guid? biologicalSourceClaimId,
        Guid? materialId,
        int? materialTypeId,
        Guid? intendedUseCharacteristicId,
        int? typicalQuantityUnitId,
        CancellationToken cancellationToken)
    {
        if (biologicalSourceClaimId.HasValue)
        {
            var validClaim = await _db.Set<BusinessRelationshipBiologicalSourceClaim>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == biologicalSourceClaimId.Value
                        && x.BusinessRelationshipId == businessRelationshipId,
                    cancellationToken);

            if (!validClaim)
            {
                return Result.Fail(
                    new ConflictError(
                        "The biological source claim does not belong to the BuyerDemand business relationship."));
            }
        }

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

        if (typicalQuantityUnitId.HasValue
            && !await _db.Set<UnitOfMeasure>().AsNoTracking().AnyAsync(x => x.Id == typicalQuantityUnitId.Value, cancellationToken))
        {
            return Result.Fail(new NotFoundError(nameof(UnitOfMeasure), typicalQuantityUnitId.Value));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Kontrollerar att eventuella evidence-referenser finns.
    /// </summary>
    private async Task<Result> ValidateEvidenceReferencesAsync(
        Guid? referenceEntryId,
        Guid? crmActivityId,
        CancellationToken cancellationToken)
    {
        if (referenceEntryId.HasValue
            && !await _db.Set<ReferenceEntry>().AsNoTracking().AnyAsync(x => x.Id == referenceEntryId.Value, cancellationToken))
        {
            return Result.Fail(new NotFoundError(nameof(ReferenceEntry), referenceEntryId.Value));
        }

        if (crmActivityId.HasValue
            && !await _db.Set<CrmActivity>().AsNoTracking().AnyAsync(x => x.Id == crmActivityId.Value, cancellationToken))
        {
            return Result.Fail(new NotFoundError(nameof(CrmActivity), crmActivityId.Value));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Sparar ett RowVersion-skyddat objekt med optimistic concurrency.
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
    /// Projekterar en BuyerDemand direkt från databasen.
    /// </summary>
    private async Task<BuyerDemandResponse?> ProjectAsync(
        Guid buyerDemandId,
        CancellationToken cancellationToken)
    {
        return await _db.Set<BuyerDemand>()
            .AsNoTracking()
            .Where(x => x.Id == buyerDemandId)
            .Select(BuyerDemandProjections.Details)
            .SingleOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Projekterar en sparad BuyerDemand och kastar om den oväntat saknas.
    /// </summary>
    private async Task<BuyerDemandResponse> ProjectRequiredAsync(
        Guid buyerDemandId,
        CancellationToken cancellationToken)
    {
        return await ProjectAsync(buyerDemandId, cancellationToken)
            ?? throw new InvalidOperationException(
                $"BuyerDemand '{buyerDemandId}' was saved but could not be projected.");
    }

    /// <summary>
    /// Lägger på stabil sortering innan paginering utförs.
    /// </summary>
    private static IQueryable<BuyerDemand> ApplySorting(
        IQueryable<BuyerDemand> query,
        BuyerDemandSortField sortBy,
        SortDirection sortDirection)
    {
        return (sortBy, sortDirection) switch
        {
            (BuyerDemandSortField.Name, SortDirection.Descending) =>
                query.OrderByDescending(x => x.Name)
                    .ThenByDescending(x => x.Id),

            (BuyerDemandSortField.CounterpartyName, SortDirection.Ascending) =>
                query.OrderBy(x => x.BusinessRelationship.Counterparty.Name)
                    .ThenBy(x => x.Id),

            (BuyerDemandSortField.CounterpartyName, SortDirection.Descending) =>
                query.OrderByDescending(x => x.BusinessRelationship.Counterparty.Name)
                    .ThenByDescending(x => x.Id),

            (BuyerDemandSortField.Confidence, SortDirection.Ascending) =>
                query.OrderBy(x => x.Confidence)
                    .ThenBy(x => x.Id),

            (BuyerDemandSortField.Confidence, SortDirection.Descending) =>
                query.OrderByDescending(x => x.Confidence)
                    .ThenByDescending(x => x.Id),

            (BuyerDemandSortField.LastConfirmedAtUtc, SortDirection.Ascending) =>
                query.OrderBy(x => x.LastConfirmedAtUtc)
                    .ThenBy(x => x.Id),

            (BuyerDemandSortField.LastConfirmedAtUtc, SortDirection.Descending) =>
                query.OrderByDescending(x => x.LastConfirmedAtUtc)
                    .ThenByDescending(x => x.Id),

            (BuyerDemandSortField.CreateDateUtc, SortDirection.Ascending) =>
                query.OrderBy(x => x.CreateDateUtc)
                    .ThenBy(x => x.Id),

            (BuyerDemandSortField.CreateDateUtc, SortDirection.Descending) =>
                query.OrderByDescending(x => x.CreateDateUtc)
                    .ThenByDescending(x => x.Id),

            _ =>
                query.OrderBy(x => x.Name)
                    .ThenBy(x => x.Id)
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