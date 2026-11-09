using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Economy;
using Arb_Int.Domain.Errors;
using Arb_Int.Domain.Location;
using Arb_Int.Domain.Sourcing;
using Arb_Int.Domain.Trading.Projections;
using Arb_Int.Domain.Validation;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar den polymorfa requirement-hierarkin inom RFQ-draftrevisioner.
/// </summary>
public sealed class BuyerRfqRequirementService : IBuyerRfqRequirementService
{
    private readonly Arb_IntDbContext _db;
    private readonly IValidationService _validationService;

    /// <summary>
    /// Skapar requirement-servicen med databas- och valideringsberoenden.
    /// </summary>
    public BuyerRfqRequirementService(Arb_IntDbContext db, IValidationService validationService)
    {
        _db = db;
        _validationService = validationService;
    }

    /// <summary>
    /// Hämtar samtliga eller endast aktiva krav för en RFQ-revision.
    /// </summary>
    public async Task<Result<IReadOnlyList<BuyerRfqRequirementResponse>>> GetByRevisionAsync(
        Guid buyerRfqRevisionId,
        bool includeInactive = true,
        CancellationToken cancellationToken = default)
    {
        var revisionExists = await _db.Set<BuyerRfqRevision>()
            .AsNoTracking()
            .AnyAsync(x => x.Id == buyerRfqRevisionId, cancellationToken);

        if (!revisionExists)
        {
            return Result.Fail<IReadOnlyList<BuyerRfqRequirementResponse>>(
                new NotFoundError(nameof(BuyerRfqRevision), buyerRfqRevisionId));
        }

        IQueryable<BuyerRfqRequirement> query = _db.Set<BuyerRfqRequirement>()
            .AsNoTracking()
            .Where(x => x.BuyerRfqRevisionId == buyerRfqRevisionId);

        if (!includeInactive)
        {
            query = query.Where(x => x.IsActive);
        }

        var requirements = await query
            .OrderBy(x => x.BuyerRfqLineId)
            .ThenBy(x => x.SortOrder)
            .ThenBy(x => x.Id)
            .Select(BuyerRfqRequirementProjections.Details)
            .ToListAsync(cancellationToken);

        return Result.Ok<IReadOnlyList<BuyerRfqRequirementResponse>>(requirements);
    }

    /// <summary>
    /// Hämtar samtliga eller endast aktiva krav för en specifik RFQ-rad.
    /// </summary>
    public async Task<Result<IReadOnlyList<BuyerRfqRequirementResponse>>> GetByLineAsync(
        Guid buyerRfqLineId,
        bool includeInactive = true,
        CancellationToken cancellationToken = default)
    {
        var lineExists = await _db.Set<BuyerRfqLine>()
            .AsNoTracking()
            .AnyAsync(x => x.Id == buyerRfqLineId, cancellationToken);

        if (!lineExists)
        {
            return Result.Fail<IReadOnlyList<BuyerRfqRequirementResponse>>(
                new NotFoundError(nameof(BuyerRfqLine), buyerRfqLineId));
        }

        IQueryable<BuyerRfqRequirement> query = _db.Set<BuyerRfqRequirement>()
            .AsNoTracking()
            .Where(x => x.BuyerRfqLineId == buyerRfqLineId);

        if (!includeInactive)
        {
            query = query.Where(x => x.IsActive);
        }

        var requirements = await query
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Id)
            .Select(BuyerRfqRequirementProjections.Details)
            .ToListAsync(cancellationToken);

        return Result.Ok<IReadOnlyList<BuyerRfqRequirementResponse>>(requirements);
    }

    /// <summary>
    /// Hämtar ett enskilt krav oberoende av konkret requirement-typ.
    /// </summary>
    public async Task<Result<BuyerRfqRequirementResponse>> GetByIdAsync(
        Guid buyerRfqRequirementId,
        CancellationToken cancellationToken = default)
    {
        var response = await ProjectAsync(
            buyerRfqRequirementId,
            cancellationToken);

        if (response is null)
        {
            return Result.Fail<BuyerRfqRequirementResponse>(
                new NotFoundError(nameof(BuyerRfqRequirement), buyerRfqRequirementId));
        }

        return Result.Ok(response);
    }

    /// <summary>
    /// Skapar ett materialkrav i en draft-revision.
    /// </summary>
    public Task<Result<BuyerRfqRequirementResponse>> CreateMaterialAsync(
        Guid buyerRfqRevisionId,
        CreateBuyerRfqMaterialRequirementRequest request,
        CancellationToken cancellationToken = default)
    {
        return CreateRequirementAsync<BuyerRfqMaterialRequirement, CreateBuyerRfqMaterialRequirementRequest>(
        buyerRfqRevisionId,
        request,
        x => new BuyerRfqMaterialRequirement
        {
            Name = x.Name.Trim(),
            Description = x.Description.Trim()
        },
        (x, ct) => ValidateMaterialReferencesAsync(
            x.MaterialId,
            x.MaterialTypeId,
            ct),
        (entity, x) =>
        {
            entity.MaterialId = x.MaterialId;
            entity.MaterialTypeId = x.MaterialTypeId;
            entity.MatchMode = x.MatchMode;
            entity.AllowSubstitutes = x.AllowSubstitutes;
        },
        cancellationToken);
    }

    /// <summary>
    /// Uppdaterar ett materialkrav i en draft-revision.
    /// </summary>
    public Task<Result<BuyerRfqRequirementResponse>> UpdateMaterialAsync(
        Guid buyerRfqRequirementId,
        UpdateBuyerRfqMaterialRequirementRequest request,
        CancellationToken cancellationToken = default)
    {
        return UpdateRequirementAsync<BuyerRfqMaterialRequirement, UpdateBuyerRfqMaterialRequirementRequest>(
            buyerRfqRequirementId,
            request,
            (x, ct) => ValidateMaterialReferencesAsync(x.MaterialId, x.MaterialTypeId, ct),
            (entity, x) =>
            {
                entity.MaterialId = x.MaterialId;
                entity.MaterialTypeId = x.MaterialTypeId;
                entity.MatchMode = x.MatchMode;
                entity.AllowSubstitutes = x.AllowSubstitutes;
            },
            cancellationToken);
    }

    /// <summary>
    /// Skapar ett biological-source-krav i en draft-revision.
    /// </summary>
    public Task<Result<BuyerRfqRequirementResponse>> CreateBiologicalSourceAsync(
        Guid buyerRfqRevisionId,
        CreateBuyerRfqBiologicalSourceRequirementRequest request,
        CancellationToken cancellationToken = default)
    {
        return CreateRequirementAsync<BuyerRfqBiologicalSourceRequirement, CreateBuyerRfqBiologicalSourceRequirementRequest>(
        buyerRfqRevisionId,
        request,
        x => new BuyerRfqBiologicalSourceRequirement
        {
            Name = x.Name.Trim(),
            Description = x.Description.Trim()
        },
        (x, ct) => ValidateRequiredReferenceAsync<BiologicalSource>(
            x.BiologicalSourceId,
            ct),
        (entity, x) =>
        {
            entity.BiologicalSourceId = x.BiologicalSourceId;
            entity.Operator = x.Operator;
        },
        cancellationToken);
    }

    /// <summary>
    /// Uppdaterar ett biological-source-krav i en draft-revision.
    /// </summary>
    public Task<Result<BuyerRfqRequirementResponse>> UpdateBiologicalSourceAsync(
        Guid buyerRfqRequirementId,
        UpdateBuyerRfqBiologicalSourceRequirementRequest request,
        CancellationToken cancellationToken = default)
    {
        return UpdateRequirementAsync<BuyerRfqBiologicalSourceRequirement, UpdateBuyerRfqBiologicalSourceRequirementRequest>(
            buyerRfqRequirementId,
            request,
            (x, ct) => ValidateRequiredReferenceAsync<BiologicalSource>(x.BiologicalSourceId, ct),
            (entity, x) =>
            {
                entity.BiologicalSourceId = x.BiologicalSourceId;
                entity.Operator = x.Operator;
            },
            cancellationToken);
    }

    /// <summary>
    /// Skapar ett ursprungskrav i en draft-revision.
    /// </summary>
    public Task<Result<BuyerRfqRequirementResponse>> CreateOriginAsync(
        Guid buyerRfqRevisionId,
        CreateBuyerRfqOriginRequirementRequest request,
        CancellationToken cancellationToken = default)
    {
        return CreateRequirementAsync<BuyerRfqOriginRequirement, CreateBuyerRfqOriginRequirementRequest>(
        buyerRfqRevisionId,
        request,
        x => new BuyerRfqOriginRequirement
        {
            Name = x.Name.Trim(),
            Description = x.Description.Trim()
        },
        (x, ct) => ValidateOriginReferencesAsync(
            x.GeographicLocationId,
            x.CountryId,
            ct),
        (entity, x) =>
        {
            entity.GeographicLocationId = x.GeographicLocationId;
            entity.CountryId = x.CountryId;
            entity.OriginRole = x.OriginRole;
            entity.Operator = x.Operator;
        },
        cancellationToken);
    }

    /// <summary>
    /// Uppdaterar ett ursprungskrav i en draft-revision.
    /// </summary>
    public Task<Result<BuyerRfqRequirementResponse>> UpdateOriginAsync(
        Guid buyerRfqRequirementId,
        UpdateBuyerRfqOriginRequirementRequest request,
        CancellationToken cancellationToken = default)
    {
        return UpdateRequirementAsync<BuyerRfqOriginRequirement, UpdateBuyerRfqOriginRequirementRequest>(
            buyerRfqRequirementId,
            request,
            (x, ct) => ValidateOriginReferencesAsync(x.GeographicLocationId, x.CountryId, ct),
            (entity, x) =>
            {
                entity.GeographicLocationId = x.GeographicLocationId;
                entity.CountryId = x.CountryId;
                entity.OriginRole = x.OriginRole;
                entity.Operator = x.Operator;
            },
            cancellationToken);
    }

    /// <summary>
    /// Skapar ett certifieringskrav i en draft-revision.
    /// </summary>
    public Task<Result<BuyerRfqRequirementResponse>> CreateCertificationAsync(
        Guid buyerRfqRevisionId,
        CreateBuyerRfqCertificationRequirementRequest request,
        CancellationToken cancellationToken = default)
    {
        return CreateRequirementAsync<BuyerRfqCertificationRequirement, CreateBuyerRfqCertificationRequirementRequest>(
            buyerRfqRevisionId,
            request,
            x => new BuyerRfqCertificationRequirement
            {
                Name = x.Name.Trim(),
                Description = x.Description.Trim()
            },
            (x, ct) => ValidateCertificationReferencesAsync(x.CertificationSchemeId, x.CertificationSchemeJurisdictionId, ct),
            (entity, x) =>
            {
                entity.CertificationSchemeId = x.CertificationSchemeId;
                entity.Operator = x.Operator;
                entity.RequiredScope = x.RequiredScope;
                entity.CertificationSchemeJurisdictionId = x.CertificationSchemeJurisdictionId;
                entity.MustBeValidAtDelivery = x.MustBeValidAtDelivery;
            },
            cancellationToken);
    }

    /// <summary>
    /// Uppdaterar ett certifieringskrav i en draft-revision.
    /// </summary>
    public Task<Result<BuyerRfqRequirementResponse>> UpdateCertificationAsync(
        Guid buyerRfqRequirementId,
        UpdateBuyerRfqCertificationRequirementRequest request,
        CancellationToken cancellationToken = default)
    {
        return UpdateRequirementAsync<BuyerRfqCertificationRequirement, UpdateBuyerRfqCertificationRequirementRequest>(
            buyerRfqRequirementId,
            request,
            (x, ct) => ValidateCertificationReferencesAsync(x.CertificationSchemeId, x.CertificationSchemeJurisdictionId, ct),
            (entity, x) =>
            {
                entity.CertificationSchemeId = x.CertificationSchemeId;
                entity.Operator = x.Operator;
                entity.RequiredScope = x.RequiredScope;
                entity.CertificationSchemeJurisdictionId = x.CertificationSchemeJurisdictionId;
                entity.MustBeValidAtDelivery = x.MustBeValidAtDelivery;
            },
            cancellationToken);
    }

    /// <summary>
    /// Skapar ett dokumentkrav i en draft-revision.
    /// </summary>
    public Task<Result<BuyerRfqRequirementResponse>> CreateDocumentAsync(
        Guid buyerRfqRevisionId,
        CreateBuyerRfqDocumentRequirementRequest request,
        CancellationToken cancellationToken = default)
    {
        return CreateRequirementAsync<BuyerRfqDocumentRequirement, CreateBuyerRfqDocumentRequirementRequest>(
            buyerRfqRevisionId,
            request,
            x => new BuyerRfqDocumentRequirement
            {
                Name = x.Name.Trim(),
                Description = x.Description.Trim()
            },
            (x, ct) => ValidateRequiredReferenceAsync<DocumentType>(x.DocumentTypeId, ct),
            (entity, x) =>
            {
                entity.DocumentTypeId = x.DocumentTypeId;
                entity.Operator = x.Operator;
                entity.MustBeLotSpecific = x.MustBeLotSpecific;
                entity.MustBeIssuedByAccreditedParty = x.MustBeIssuedByAccreditedParty;
                entity.MaximumAgeDays = x.MaximumAgeDays;
            },
            cancellationToken);
    }

    /// <summary>
    /// Uppdaterar ett dokumentkrav i en draft-revision.
    /// </summary>
    public Task<Result<BuyerRfqRequirementResponse>> UpdateDocumentAsync(
        Guid buyerRfqRequirementId,
        UpdateBuyerRfqDocumentRequirementRequest request,
        CancellationToken cancellationToken = default)
    {
        return UpdateRequirementAsync<BuyerRfqDocumentRequirement, UpdateBuyerRfqDocumentRequirementRequest>(
            buyerRfqRequirementId,
            request,
            (x, ct) => ValidateRequiredReferenceAsync<DocumentType>(x.DocumentTypeId, ct),
            (entity, x) =>
            {
                entity.DocumentTypeId = x.DocumentTypeId;
                entity.Operator = x.Operator;
                entity.MustBeLotSpecific = x.MustBeLotSpecific;
                entity.MustBeIssuedByAccreditedParty = x.MustBeIssuedByAccreditedParty;
                entity.MaximumAgeDays = x.MaximumAgeDays;
            },
            cancellationToken);
    }

    /// <summary>
    /// Skapar ett constituent-krav i en draft-revision.
    /// </summary>
    public Task<Result<BuyerRfqRequirementResponse>> CreateConstituentAsync(
        Guid buyerRfqRevisionId,
        CreateBuyerRfqConstituentRequirementRequest request,
        CancellationToken cancellationToken = default)
    {
        return CreateRequirementAsync<BuyerRfqConstituentRequirement, CreateBuyerRfqConstituentRequirementRequest>(
            buyerRfqRevisionId,
            request,
            x => new BuyerRfqConstituentRequirement
            {
                Name = x.Name.Trim(),
                Description = x.Description.Trim()
            },
            (x, ct) => ValidateConstituentReferencesAsync(x.ConstituentId, x.UnitOfMeasureId, ct),
            (entity, x) =>
            {
                entity.ConstituentId = x.ConstituentId;
                entity.Operator = x.Operator;
                entity.MinimumValue = x.MinimumValue;
                entity.MaximumValue = x.MaximumValue;
                entity.TargetValue = x.TargetValue;
                entity.UnitOfMeasureId = x.UnitOfMeasureId;
                entity.Basis = x.Basis;
                entity.AnalysisMethod = TrimToNull(x.AnalysisMethod);
                entity.MustBeVerifiedForEveryLot = x.MustBeVerifiedForEveryLot;
                entity.AccreditedLaboratoryRequired = x.AccreditedLaboratoryRequired;
            },
            cancellationToken);
    }

    /// <summary>
    /// Uppdaterar ett constituent-krav i en draft-revision.
    /// </summary>
    public Task<Result<BuyerRfqRequirementResponse>> UpdateConstituentAsync(
        Guid buyerRfqRequirementId,
        UpdateBuyerRfqConstituentRequirementRequest request,
        CancellationToken cancellationToken = default)
    {
        return UpdateRequirementAsync<BuyerRfqConstituentRequirement, UpdateBuyerRfqConstituentRequirementRequest>(
            buyerRfqRequirementId,
            request,
            (x, ct) => ValidateConstituentReferencesAsync(x.ConstituentId, x.UnitOfMeasureId, ct),
            (entity, x) =>
            {
                entity.ConstituentId = x.ConstituentId;
                entity.Operator = x.Operator;
                entity.MinimumValue = x.MinimumValue;
                entity.MaximumValue = x.MaximumValue;
                entity.TargetValue = x.TargetValue;
                entity.UnitOfMeasureId = x.UnitOfMeasureId;
                entity.Basis = x.Basis;
                entity.AnalysisMethod = TrimToNull(x.AnalysisMethod);
                entity.MustBeVerifiedForEveryLot = x.MustBeVerifiedForEveryLot;
                entity.AccreditedLaboratoryRequired = x.AccreditedLaboratoryRequired;
            },
            cancellationToken);
    }

    /// <summary>
    /// Skapar ett characteristic-krav i en draft-revision.
    /// </summary>
    public Task<Result<BuyerRfqRequirementResponse>> CreateCharacteristicAsync(
        Guid buyerRfqRevisionId,
        CreateBuyerRfqCharacteristicRequirementRequest request,
        CancellationToken cancellationToken = default)
    {
        return CreateRequirementAsync<BuyerRfqCharacteristicRequirement, CreateBuyerRfqCharacteristicRequirementRequest>(
            buyerRfqRevisionId,
            request,
            x => new BuyerRfqCharacteristicRequirement
            {
                Name = x.Name.Trim(),
                Description = x.Description.Trim()
            },
            (x, ct) => ValidateCharacteristicReferencesAsync(x.CharacteristicId, x.UnitOfMeasureId, ct),
            (entity, x) =>
            {
                entity.CharacteristicId = x.CharacteristicId;
                entity.Operator = x.Operator;
                entity.ExpectedTextValue = TrimToNull(x.ExpectedTextValue);
                entity.ExpectedNumericValue = x.ExpectedNumericValue;
                entity.UnitOfMeasureId = x.UnitOfMeasureId;
            },
            cancellationToken);
    }

    /// <summary>
    /// Uppdaterar ett characteristic-krav i en draft-revision.
    /// </summary>
    public Task<Result<BuyerRfqRequirementResponse>> UpdateCharacteristicAsync(
        Guid buyerRfqRequirementId,
        UpdateBuyerRfqCharacteristicRequirementRequest request,
        CancellationToken cancellationToken = default)
    {
        return UpdateRequirementAsync<BuyerRfqCharacteristicRequirement, UpdateBuyerRfqCharacteristicRequirementRequest>(
            buyerRfqRequirementId,
            request,
            (x, ct) => ValidateCharacteristicReferencesAsync(x.CharacteristicId, x.UnitOfMeasureId, ct),
            (entity, x) =>
            {
                entity.CharacteristicId = x.CharacteristicId;
                entity.Operator = x.Operator;
                entity.ExpectedTextValue = TrimToNull(x.ExpectedTextValue);
                entity.ExpectedNumericValue = x.ExpectedNumericValue;
                entity.UnitOfMeasureId = x.UnitOfMeasureId;
            },
            cancellationToken);
    }

    /// <summary>
    /// Skapar ett processkrav i en draft-revision.
    /// </summary>
    public Task<Result<BuyerRfqRequirementResponse>> CreateProcessAsync(
        Guid buyerRfqRevisionId,
        CreateBuyerRfqProcessRequirementRequest request,
        CancellationToken cancellationToken = default)
    {
        return CreateRequirementAsync<BuyerRfqProcessRequirement, CreateBuyerRfqProcessRequirementRequest>(
            buyerRfqRevisionId,
            request,
            x => new BuyerRfqProcessRequirement
            {
                Name = x.Name.Trim(),
                Description = x.Description.Trim()
            },
            (x, ct) => ValidateRequiredReferenceAsync<ProcessType>(x.ProcessTypeId, ct),
            (entity, x) =>
            {
                entity.ProcessTypeId = x.ProcessTypeId;
                entity.Mode = x.Mode;
                entity.ProcessParametersJson = TrimToNull(x.ProcessParametersJson);
            },
            cancellationToken);
    }

    /// <summary>
    /// Uppdaterar ett processkrav i en draft-revision.
    /// </summary>
    public Task<Result<BuyerRfqRequirementResponse>> UpdateProcessAsync(
        Guid buyerRfqRequirementId,
        UpdateBuyerRfqProcessRequirementRequest request,
        CancellationToken cancellationToken = default)
    {
        return UpdateRequirementAsync<BuyerRfqProcessRequirement, UpdateBuyerRfqProcessRequirementRequest>(
            buyerRfqRequirementId,
            request,
            (x, ct) => ValidateRequiredReferenceAsync<ProcessType>(x.ProcessTypeId, ct),
            (entity, x) =>
            {
                entity.ProcessTypeId = x.ProcessTypeId;
                entity.Mode = x.Mode;
                entity.ProcessParametersJson = TrimToNull(x.ProcessParametersJson);
            },
            cancellationToken);
    }

    /// <summary>
    /// Skapar ett kommersiellt krav i en draft-revision.
    /// </summary>
    public Task<Result<BuyerRfqRequirementResponse>> CreateCommercialAsync(
        Guid buyerRfqRevisionId,
        CreateBuyerRfqCommercialRequirementRequest request,
        CancellationToken cancellationToken = default)
    {
        return CreateRequirementAsync<BuyerRfqCommercialRequirement, CreateBuyerRfqCommercialRequirementRequest>(
            buyerRfqRevisionId,
            request,
            x => new BuyerRfqCommercialRequirement
            {
                Name = x.Name.Trim(),
                Description = x.Description.Trim()
            },
            (x, ct) => ValidateCommercialReferencesAsync(x.UnitOfMeasureId, x.CurrencyId, ct),
            (entity, x) =>
            {
                entity.Kind = x.Kind;
                entity.Operator = x.Operator;
                entity.NumericValue = x.NumericValue;
                entity.TextValue = TrimToNull(x.TextValue);
                entity.UnitOfMeasureId = x.UnitOfMeasureId;
                entity.CurrencyId = x.CurrencyId;
            },
            cancellationToken);
    }

    /// <summary>
    /// Uppdaterar ett kommersiellt krav i en draft-revision.
    /// </summary>
    public Task<Result<BuyerRfqRequirementResponse>> UpdateCommercialAsync(
        Guid buyerRfqRequirementId,
        UpdateBuyerRfqCommercialRequirementRequest request,
        CancellationToken cancellationToken = default)
    {
        return UpdateRequirementAsync<BuyerRfqCommercialRequirement, UpdateBuyerRfqCommercialRequirementRequest>(
            buyerRfqRequirementId,
            request,
            (x, ct) => ValidateCommercialReferencesAsync(x.UnitOfMeasureId, x.CurrencyId, ct),
            (entity, x) =>
            {
                entity.Kind = x.Kind;
                entity.Operator = x.Operator;
                entity.NumericValue = x.NumericValue;
                entity.TextValue = TrimToNull(x.TextValue);
                entity.UnitOfMeasureId = x.UnitOfMeasureId;
                entity.CurrencyId = x.CurrencyId;
            },
            cancellationToken);
    }

    /// <summary>
    /// Skapar ett logistikkrav i en draft-revision.
    /// </summary>
    public Task<Result<BuyerRfqRequirementResponse>> CreateLogisticsAsync(
        Guid buyerRfqRevisionId,
        CreateBuyerRfqLogisticsRequirementRequest request,
        CancellationToken cancellationToken = default)
    {
        return CreateRequirementAsync<BuyerRfqLogisticsRequirement, CreateBuyerRfqLogisticsRequirementRequest>(
            buyerRfqRevisionId,
            request,
            x => new BuyerRfqLogisticsRequirement
            {
                Name = x.Name.Trim(),
                Description = x.Description.Trim()
            },
            (x, ct) => ValidateLogisticsReferencesAsync(x.UnitOfMeasureId, x.IncotermRuleId, ct),
            (entity, x) =>
            {
                entity.Kind = x.Kind;
                entity.Operator = x.Operator;
                entity.NumericValue = x.NumericValue;
                entity.TextValue = TrimToNull(x.TextValue);
                entity.UnitOfMeasureId = x.UnitOfMeasureId;
                entity.IncotermRuleId = x.IncotermRuleId;
            },
            cancellationToken);
    }

    /// <summary>
    /// Uppdaterar ett logistikkrav i en draft-revision.
    /// </summary>
    public Task<Result<BuyerRfqRequirementResponse>> UpdateLogisticsAsync(
        Guid buyerRfqRequirementId,
        UpdateBuyerRfqLogisticsRequirementRequest request,
        CancellationToken cancellationToken = default)
    {
        return UpdateRequirementAsync<BuyerRfqLogisticsRequirement, UpdateBuyerRfqLogisticsRequirementRequest>(
            buyerRfqRequirementId,
            request,
            (x, ct) => ValidateLogisticsReferencesAsync(x.UnitOfMeasureId, x.IncotermRuleId, ct),
            (entity, x) =>
            {
                entity.Kind = x.Kind;
                entity.Operator = x.Operator;
                entity.NumericValue = x.NumericValue;
                entity.TextValue = TrimToNull(x.TextValue);
                entity.UnitOfMeasureId = x.UnitOfMeasureId;
                entity.IncotermRuleId = x.IncotermRuleId;
            },
            cancellationToken);
    }

    /// <summary>
    /// Kör den gemensamma create-pipelinen för en konkret requirement-typ.
    /// </summary>
    private async Task<Result<BuyerRfqRequirementResponse>> CreateRequirementAsync<TEntity, TRequest>(
        Guid buyerRfqRevisionId,
        TRequest request,
        Func<TRequest, TEntity> createEntity,
        Func<TRequest, CancellationToken, Task<Result>> validateSpecificReferences,
        Action<TEntity, TRequest> applySpecificValues,
        CancellationToken cancellationToken)
        where TEntity : BuyerRfqRequirement
        where TRequest : BuyerRfqRequirementRequestBase
    {
        var validationResult = await _validationService.ValidateAsync(
            request,
            cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<BuyerRfqRequirementResponse>(
                validationResult.Errors);
        }

        var revision = await _db.Set<BuyerRfqRevision>()
            .SingleOrDefaultAsync(
                x => x.Id == buyerRfqRevisionId,
                cancellationToken);

        if (revision is null)
        {
            return Result.Fail<BuyerRfqRequirementResponse>(
                new NotFoundError(
                    nameof(BuyerRfqRevision),
                    buyerRfqRevisionId));
        }

        var editableResult = ValidateRevisionIsEditable(
            revision);

        if (editableResult.IsFailed)
        {
            return Result.Fail<BuyerRfqRequirementResponse>(
                editableResult.Errors);
        }

        var contextResult = await ValidateLineContextAsync(
            buyerRfqRevisionId,
            request.BuyerRfqLineId,
            cancellationToken);

        if (contextResult.IsFailed)
        {
            return Result.Fail<BuyerRfqRequirementResponse>(
                contextResult.Errors);
        }

        var referencesResult = await validateSpecificReferences(
            request,
            cancellationToken);

        if (referencesResult.IsFailed)
        {
            return Result.Fail<BuyerRfqRequirementResponse>(
                referencesResult.Errors);
        }

        var entity = createEntity(request);

        entity.Id = Guid.CreateVersion7();
        entity.BuyerRfqRevisionId = buyerRfqRevisionId;
        entity.IsActive = true;

        ApplyCommonValues(
            entity,
            request);

        applySpecificValues(
            entity,
            request);

        _db.Set<TEntity>()
            .Add(entity);

        TouchRevision(revision);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            var concurrencyError = CreateConcurrencyError(
                exception,
                typeof(TEntity).Name,
                entity.Id);

            return Result.Fail<BuyerRfqRequirementResponse>(concurrencyError);
        }

        return Result.Ok(
            await ProjectRequiredAsync(
                entity.Id,
                cancellationToken));
    }

    /// <summary>
    /// Kör den gemensamma update-pipelinen för en konkret requirement-typ.
    /// </summary>
    private async Task<Result<BuyerRfqRequirementResponse>> UpdateRequirementAsync<TEntity, TRequest>(
        Guid buyerRfqRequirementId,
        TRequest request,
        Func<TRequest, CancellationToken, Task<Result>> validateSpecificReferences,
        Action<TEntity, TRequest> applySpecificValues,
        CancellationToken cancellationToken)
        where TEntity : BuyerRfqRequirement
        where TRequest : UpdateBuyerRfqRequirementRequestBase
    {
        var validationResult = await _validationService.ValidateAsync(request, cancellationToken);

        if (validationResult.IsFailed)
        {
            return Result.Fail<BuyerRfqRequirementResponse>(validationResult.Errors);
        }

        var entity = await _db.Set<TEntity>()
            .Include(x => x.BuyerRfqRevision)
            .SingleOrDefaultAsync(x => x.Id == buyerRfqRequirementId, cancellationToken);

        if (entity is null)
        {
            return Result.Fail<BuyerRfqRequirementResponse>(
                new NotFoundError(typeof(TEntity).Name, buyerRfqRequirementId));
        }

        var editableResult = ValidateRevisionIsEditable(entity.BuyerRfqRevision);

        if (editableResult.IsFailed)
        {
            return Result.Fail<BuyerRfqRequirementResponse>(editableResult.Errors);
        }

        var contextResult = await ValidateLineContextAsync(
            entity.BuyerRfqRevisionId,
            request.BuyerRfqLineId,
            cancellationToken);

        if (contextResult.IsFailed)
        {
            return Result.Fail<BuyerRfqRequirementResponse>(contextResult.Errors);
        }

        var referencesResult = await validateSpecificReferences(
            request,
            cancellationToken);

        if (referencesResult.IsFailed)
        {
            return Result.Fail<BuyerRfqRequirementResponse>(referencesResult.Errors);
        }

        ApplyCommonUpdateValues(entity, request);
        entity.IsActive = request.IsActive;

        applySpecificValues(entity, request);

        TouchRevision(entity.BuyerRfqRevision);

        _db.Entry(entity)
            .Property(x => x.RowVersion)
            .OriginalValue = request.RowVersion;

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            return Result.Fail<BuyerRfqRequirementResponse>(
                CreateConcurrencyError(
                    exception,
                    typeof(TEntity).Name,
                    entity.Id));
        }

        return Result.Ok(
            await ProjectRequiredAsync(
                entity.Id,
                cancellationToken));
    }

    /// <summary>
    /// Kopierar gemensamma request-värden till requirement-entiteten.
    /// </summary>
    private static void ApplyCommonValues(
        BuyerRfqRequirement entity,
        BuyerRfqRequirementRequestBase request)
    {
        entity.BuyerRfqLineId = request.BuyerRfqLineId;
        entity.Level = request.Level;
        entity.Weight = request.Weight;
        entity.AlternativeGroupKey = TrimToNull(request.AlternativeGroupKey);
        entity.SortOrder = request.SortOrder;
        entity.BuyerOriginalText = TrimToNull(request.BuyerOriginalText);
        entity.InternalNotes = TrimToNull(request.InternalNotes);
    }

    /// <summary>
    /// Kopierar gemensamma fält som får ändras på ett befintligt requirement.
    /// </summary>
    private static void ApplyCommonUpdateValues(
        BuyerRfqRequirement entity,
        UpdateBuyerRfqRequirementRequestBase request)
    {
        entity.BuyerRfqLineId = request.BuyerRfqLineId;
        entity.Name = request.Name.Trim();
        entity.Description = request.Description.Trim();
        entity.Level = request.Level;
        entity.Weight = request.Weight;
        entity.AlternativeGroupKey = TrimToNull(request.AlternativeGroupKey);
        entity.IsActive = request.IsActive;
        entity.SortOrder = request.SortOrder;
        entity.BuyerOriginalText = TrimToNull(request.BuyerOriginalText);
        entity.InternalNotes = TrimToNull(request.InternalNotes);
    }

    /// <summary>
    /// Markerar parent-revisionen så att requirement-ändringar konkurrerar med release på samma RowVersion.
    /// </summary>
    private static void TouchRevision(BuyerRfqRevision revision)
    {
        revision.ActionDateUtc = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Identifierar om concurrency-konflikten gäller parent-revisionen eller requirementet.
    /// </summary>
    private static ConcurrencyError CreateConcurrencyError(
        DbUpdateConcurrencyException exception,
        string fallbackEntityName,
        Guid fallbackEntityId)
    {
        var revision = exception.Entries
            .Select(x => x.Entity)
            .OfType<BuyerRfqRevision>()
            .FirstOrDefault();

        return revision is not null
            ? new ConcurrencyError(nameof(BuyerRfqRevision), revision.Id)
            : new ConcurrencyError(fallbackEntityName, fallbackEntityId);
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
    /// Kontrollerar att eventuell rad tillhör samma revision som kravet.
    /// </summary>
    private async Task<Result> ValidateLineContextAsync(
        Guid buyerRfqRevisionId,
        Guid? buyerRfqLineId,
        CancellationToken cancellationToken)
    {
        if (!buyerRfqLineId.HasValue)
        {
            return Result.Ok();
        }

        var validLine = await _db.Set<BuyerRfqLine>()
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == buyerRfqLineId.Value
                    && x.BuyerRfqRevisionId == buyerRfqRevisionId,
                cancellationToken);

        if (!validLine)
        {
            return Result.Fail(
                new ConflictError(
                    "The selected BuyerRfqLine does not belong to the requirement's BuyerRfqRevision."));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Kontrollerar eventuella Material- och MaterialType-referenser.
    /// </summary>
    private async Task<Result> ValidateMaterialReferencesAsync(
        Guid? materialId,
        int? materialTypeId,
        CancellationToken cancellationToken)
    {
        if (materialId.HasValue
            && !await _db.Set<Material>().AsNoTracking().AnyAsync(x => x.Id == materialId.Value, cancellationToken))
        {
            return Result.Fail(
                new NotFoundError(nameof(Material), materialId.Value));
        }

        if (materialTypeId.HasValue
            && !await _db.Set<MaterialType>().AsNoTracking().AnyAsync(x => x.Id == materialTypeId.Value, cancellationToken))
        {
            return Result.Fail(
                new NotFoundError(nameof(MaterialType), materialTypeId.Value));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Kontrollerar eventuella geografiska referenser för ett ursprungskrav.
    /// </summary>
    private async Task<Result> ValidateOriginReferencesAsync(
        Guid? geographicLocationId,
        Guid? countryId,
        CancellationToken cancellationToken)
    {
        if (geographicLocationId.HasValue
            && !await _db.Set<GeographicLocation>().AsNoTracking().AnyAsync(x => x.Id == geographicLocationId.Value, cancellationToken))
        {
            return Result.Fail(
                new NotFoundError(nameof(GeographicLocation), geographicLocationId.Value));
        }

        if (countryId.HasValue
            && !await _db.Set<Country>().AsNoTracking().AnyAsync(x => x.Id == countryId.Value, cancellationToken))
        {
            return Result.Fail(
                new NotFoundError(nameof(Country), countryId.Value));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Kontrollerar certifieringssystem och att eventuell jurisdiction tillhör samma system.
    /// </summary>
    private async Task<Result> ValidateCertificationReferencesAsync(
        int certificationSchemeId,
        int? certificationSchemeJurisdictionId,
        CancellationToken cancellationToken)
    {
        var schemeExists = await _db.Set<CertificationScheme>()
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == certificationSchemeId,
                cancellationToken);

        if (!schemeExists)
        {
            return Result.Fail(
                new NotFoundError(
                    nameof(CertificationScheme),
                    certificationSchemeId));
        }

        if (!certificationSchemeJurisdictionId.HasValue)
        {
            return Result.Ok();
        }

        var jurisdictionExists = await _db.Set<CertificationSchemeJurisdiction>()
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == certificationSchemeJurisdictionId.Value
                    && x.CertificationSchemeId == certificationSchemeId,
                cancellationToken);

        if (!jurisdictionExists)
        {
            return Result.Fail(
                new ConflictError(
                    "The selected CertificationSchemeJurisdiction does not belong to the selected CertificationScheme."));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Kontrollerar Constituent och eventuell UnitOfMeasure.
    /// </summary>
    private async Task<Result> ValidateConstituentReferencesAsync(
        Guid constituentId,
        int? unitOfMeasureId,
        CancellationToken cancellationToken)
    {
        var constituentExists = await _db.Set<Constituent>()
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == constituentId,
                cancellationToken);

        if (!constituentExists)
        {
            return Result.Fail(
                new NotFoundError(
                    nameof(Constituent),
                    constituentId));
        }

        return await ValidateOptionalIntReferenceAsync<UnitOfMeasure>(
            unitOfMeasureId,
            cancellationToken);
    }

    /// <summary>
    /// Kontrollerar Characteristic och eventuell UnitOfMeasure.
    /// </summary>
    private async Task<Result> ValidateCharacteristicReferencesAsync(
        Guid characteristicId,
        int? unitOfMeasureId,
        CancellationToken cancellationToken)
    {
        var characteristicExists = await _db.Set<Characteristic>()
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == characteristicId,
                cancellationToken);

        if (!characteristicExists)
        {
            return Result.Fail(
                new NotFoundError(
                    nameof(Characteristic),
                    characteristicId));
        }

        return await ValidateOptionalIntReferenceAsync<UnitOfMeasure>(
            unitOfMeasureId,
            cancellationToken);
    }

    /// <summary>
    /// Kontrollerar eventuella UnitOfMeasure- och Currency-referenser.
    /// </summary>
    private async Task<Result> ValidateCommercialReferencesAsync(
        int? unitOfMeasureId,
        int? currencyId,
        CancellationToken cancellationToken)
    {
        var unitResult = await ValidateOptionalIntReferenceAsync<UnitOfMeasure>(
            unitOfMeasureId,
            cancellationToken);

        if (unitResult.IsFailed)
        {
            return unitResult;
        }

        return await ValidateOptionalIntReferenceAsync<Currency>(
            currencyId,
            cancellationToken);
    }

    /// <summary>
    /// Kontrollerar eventuella UnitOfMeasure- och IncotermRule-referenser.
    /// </summary>
    private async Task<Result> ValidateLogisticsReferencesAsync(
        int? unitOfMeasureId,
        int? incotermRuleId,
        CancellationToken cancellationToken)
    {
        var unitResult = await ValidateOptionalIntReferenceAsync<UnitOfMeasure>(
            unitOfMeasureId,
            cancellationToken);

        if (unitResult.IsFailed)
        {
            return unitResult;
        }

        return await ValidateOptionalIntReferenceAsync<IncotermRule>(
            incotermRuleId,
            cancellationToken);
    }

    /// <summary>
    /// Kontrollerar en obligatorisk Guid-referens.
    /// </summary>
    private async Task<Result> ValidateRequiredReferenceAsync<TEntity>(
        Guid id,
        CancellationToken cancellationToken)
        where TEntity : class
    {
        var exists = await _db.Set<TEntity>()
            .AsNoTracking()
            .AnyAsync(
                x => EF.Property<Guid>(x, "Id") == id,
                cancellationToken);

        if (!exists)
        {
            return Result.Fail(
                new NotFoundError(
                    typeof(TEntity).Name,
                    id));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Kontrollerar en obligatorisk int-referens.
    /// </summary>
    private async Task<Result> ValidateRequiredReferenceAsync<TEntity>(
        int id,
        CancellationToken cancellationToken)
        where TEntity : class
    {
        var exists = await _db.Set<TEntity>()
            .AsNoTracking()
            .AnyAsync(
                x => EF.Property<int>(x, "Id") == id,
                cancellationToken);

        if (!exists)
        {
            return Result.Fail(
                new NotFoundError(
                    typeof(TEntity).Name,
                    id));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Kontrollerar en valfri int-referens.
    /// </summary>
    private async Task<Result> ValidateOptionalIntReferenceAsync<TEntity>(
        int? id,
        CancellationToken cancellationToken)
        where TEntity : class
    {
        if (!id.HasValue)
        {
            return Result.Ok();
        }

        return await ValidateRequiredReferenceAsync<TEntity>(
            id.Value,
            cancellationToken);
    }

    /// <summary>
    /// Projekterar ett requirement direkt från TPH-tabellen.
    /// </summary>
    private async Task<BuyerRfqRequirementResponse?> ProjectAsync(
        Guid buyerRfqRequirementId,
        CancellationToken cancellationToken)
    {
        return await _db.Set<BuyerRfqRequirement>()
            .AsNoTracking()
            .Where(x => x.Id == buyerRfqRequirementId)
            .Select(BuyerRfqRequirementProjections.Details)
            .SingleOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Projekterar ett sparat requirement och kastar om det oväntat saknas.
    /// </summary>
    private async Task<BuyerRfqRequirementResponse> ProjectRequiredAsync(
        Guid buyerRfqRequirementId,
        CancellationToken cancellationToken)
    {
        return await ProjectAsync(
                buyerRfqRequirementId,
                cancellationToken)
            ?? throw new InvalidOperationException(
                $"BuyerRfqRequirement '{buyerRfqRequirementId}' was saved but could not be projected.");
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