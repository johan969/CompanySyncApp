using System.Text.Json;
using FluentValidation;

namespace Arb_Int.Domain.Trading.Validation;

/// <summary>
/// Validerar nya materialkrav.
/// </summary>
public sealed class CreateBuyerRfqMaterialRequirementRequestValidator : BuyerRfqRequirementRequestValidatorBase<CreateBuyerRfqMaterialRequirementRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för materialkrav.
    /// </summary>
    public CreateBuyerRfqMaterialRequirementRequestValidator()
    {
        RuleFor(x => x.MaterialId).NotEqual(Guid.Empty).When(x => x.MaterialId.HasValue);
        RuleFor(x => x.MaterialTypeId).GreaterThan(0).When(x => x.MaterialTypeId.HasValue);
        RuleFor(x => x.MatchMode).IsInEnum();
    }
}

/// <summary>
/// Validerar uppdateringar av materialkrav.
/// </summary>
public sealed class UpdateBuyerRfqMaterialRequirementRequestValidator : UpdateBuyerRfqRequirementRequestValidatorBase<UpdateBuyerRfqMaterialRequirementRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för materialkrav.
    /// </summary>
    public UpdateBuyerRfqMaterialRequirementRequestValidator()
    {
        RuleFor(x => x.MaterialId).NotEqual(Guid.Empty).When(x => x.MaterialId.HasValue);
        RuleFor(x => x.MaterialTypeId).GreaterThan(0).When(x => x.MaterialTypeId.HasValue);
        RuleFor(x => x.MatchMode).IsInEnum();
    }
}

/// <summary>
/// Validerar nya biological-source-krav.
/// </summary>
public sealed class CreateBuyerRfqBiologicalSourceRequirementRequestValidator : BuyerRfqRequirementRequestValidatorBase<CreateBuyerRfqBiologicalSourceRequirementRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för biological-source-krav.
    /// </summary>
    public CreateBuyerRfqBiologicalSourceRequirementRequestValidator()
    {
        RuleFor(x => x.BiologicalSourceId).NotEmpty();
        RuleFor(x => x.Operator).IsInEnum();
    }
}

/// <summary>
/// Validerar uppdateringar av biological-source-krav.
/// </summary>
public sealed class UpdateBuyerRfqBiologicalSourceRequirementRequestValidator : UpdateBuyerRfqRequirementRequestValidatorBase<UpdateBuyerRfqBiologicalSourceRequirementRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för biological-source-krav.
    /// </summary>
    public UpdateBuyerRfqBiologicalSourceRequirementRequestValidator()
    {
        RuleFor(x => x.BiologicalSourceId).NotEmpty();
        RuleFor(x => x.Operator).IsInEnum();
    }
}

/// <summary>
/// Validerar nya ursprungskrav.
/// </summary>
public sealed class CreateBuyerRfqOriginRequirementRequestValidator : BuyerRfqRequirementRequestValidatorBase<CreateBuyerRfqOriginRequirementRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för ursprungskrav.
    /// </summary>
    public CreateBuyerRfqOriginRequirementRequestValidator()
    {
        RuleFor(x => x.GeographicLocationId).NotEqual(Guid.Empty).When(x => x.GeographicLocationId.HasValue);
        RuleFor(x => x.CountryId).NotEqual(Guid.Empty).When(x => x.CountryId.HasValue);
        RuleFor(x => x.OriginRole).IsInEnum();
        RuleFor(x => x.Operator).IsInEnum();
    }
}

/// <summary>
/// Validerar uppdateringar av ursprungskrav.
/// </summary>
public sealed class UpdateBuyerRfqOriginRequirementRequestValidator : UpdateBuyerRfqRequirementRequestValidatorBase<UpdateBuyerRfqOriginRequirementRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för ursprungskrav.
    /// </summary>
    public UpdateBuyerRfqOriginRequirementRequestValidator()
    {
        RuleFor(x => x.GeographicLocationId).NotEqual(Guid.Empty).When(x => x.GeographicLocationId.HasValue);
        RuleFor(x => x.CountryId).NotEqual(Guid.Empty).When(x => x.CountryId.HasValue);
        RuleFor(x => x.OriginRole).IsInEnum();
        RuleFor(x => x.Operator).IsInEnum();
    }
}

/// <summary>
/// Validerar nya certifieringskrav.
/// </summary>
public sealed class CreateBuyerRfqCertificationRequirementRequestValidator : BuyerRfqRequirementRequestValidatorBase<CreateBuyerRfqCertificationRequirementRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för certifieringskrav.
    /// </summary>
    public CreateBuyerRfqCertificationRequirementRequestValidator()
    {
        RuleFor(x => x.CertificationSchemeId).GreaterThan(0);
        RuleFor(x => x.Operator).IsInEnum();
        RuleFor(x => x.RequiredScope).IsInEnum();
        RuleFor(x => x.CertificationSchemeJurisdictionId).GreaterThan(0).When(x => x.CertificationSchemeJurisdictionId.HasValue);
    }
}

/// <summary>
/// Validerar uppdateringar av certifieringskrav.
/// </summary>
public sealed class UpdateBuyerRfqCertificationRequirementRequestValidator : UpdateBuyerRfqRequirementRequestValidatorBase<UpdateBuyerRfqCertificationRequirementRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för certifieringskrav.
    /// </summary>
    public UpdateBuyerRfqCertificationRequirementRequestValidator()
    {
        RuleFor(x => x.CertificationSchemeId).GreaterThan(0);
        RuleFor(x => x.Operator).IsInEnum();
        RuleFor(x => x.RequiredScope).IsInEnum();
        RuleFor(x => x.CertificationSchemeJurisdictionId).GreaterThan(0).When(x => x.CertificationSchemeJurisdictionId.HasValue);
    }
}

/// <summary>
/// Validerar nya dokumentkrav.
/// </summary>
public sealed class CreateBuyerRfqDocumentRequirementRequestValidator : BuyerRfqRequirementRequestValidatorBase<CreateBuyerRfqDocumentRequirementRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för dokumentkrav.
    /// </summary>
    public CreateBuyerRfqDocumentRequirementRequestValidator()
    {
        RuleFor(x => x.DocumentTypeId).GreaterThan(0);
        RuleFor(x => x.Operator).IsInEnum();
    }
}

/// <summary>
/// Validerar uppdateringar av dokumentkrav.
/// </summary>
public sealed class UpdateBuyerRfqDocumentRequirementRequestValidator : UpdateBuyerRfqRequirementRequestValidatorBase<UpdateBuyerRfqDocumentRequirementRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för dokumentkrav.
    /// </summary>
    public UpdateBuyerRfqDocumentRequirementRequestValidator()
    {
        RuleFor(x => x.DocumentTypeId).GreaterThan(0);
        RuleFor(x => x.Operator).IsInEnum();
    }
}

/// <summary>
/// Validerar nya constituent-krav.
/// </summary>
public sealed class CreateBuyerRfqConstituentRequirementRequestValidator : BuyerRfqRequirementRequestValidatorBase<CreateBuyerRfqConstituentRequirementRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för constituent-krav.
    /// </summary>
    public CreateBuyerRfqConstituentRequirementRequestValidator()
    {
        RuleFor(x => x.ConstituentId).NotEmpty();
        RuleFor(x => x.Operator).IsInEnum();
        RuleFor(x => x.UnitOfMeasureId).GreaterThan(0).When(x => x.UnitOfMeasureId.HasValue);
        RuleFor(x => x.Basis).IsInEnum();
        RuleFor(x => x.AnalysisMethod).MaximumLength(1000);
    }
}

/// <summary>
/// Validerar uppdateringar av constituent-krav.
/// </summary>
public sealed class UpdateBuyerRfqConstituentRequirementRequestValidator : UpdateBuyerRfqRequirementRequestValidatorBase<UpdateBuyerRfqConstituentRequirementRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för constituent-krav.
    /// </summary>
    public UpdateBuyerRfqConstituentRequirementRequestValidator()
    {
        RuleFor(x => x.ConstituentId).NotEmpty();
        RuleFor(x => x.Operator).IsInEnum();
        RuleFor(x => x.UnitOfMeasureId).GreaterThan(0).When(x => x.UnitOfMeasureId.HasValue);
        RuleFor(x => x.Basis).IsInEnum();
        RuleFor(x => x.AnalysisMethod).MaximumLength(1000);
    }
}

/// <summary>
/// Validerar nya characteristic-krav.
/// </summary>
public sealed class CreateBuyerRfqCharacteristicRequirementRequestValidator : BuyerRfqRequirementRequestValidatorBase<CreateBuyerRfqCharacteristicRequirementRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för characteristic-krav.
    /// </summary>
    public CreateBuyerRfqCharacteristicRequirementRequestValidator()
    {
        RuleFor(x => x.CharacteristicId).NotEmpty();
        RuleFor(x => x.Operator).IsInEnum();
        RuleFor(x => x.ExpectedTextValue).MaximumLength(2048);
        RuleFor(x => x.UnitOfMeasureId).GreaterThan(0).When(x => x.UnitOfMeasureId.HasValue);
    }
}

/// <summary>
/// Validerar uppdateringar av characteristic-krav.
/// </summary>
public sealed class UpdateBuyerRfqCharacteristicRequirementRequestValidator : UpdateBuyerRfqRequirementRequestValidatorBase<UpdateBuyerRfqCharacteristicRequirementRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för characteristic-krav.
    /// </summary>
    public UpdateBuyerRfqCharacteristicRequirementRequestValidator()
    {
        RuleFor(x => x.CharacteristicId).NotEmpty();
        RuleFor(x => x.Operator).IsInEnum();
        RuleFor(x => x.ExpectedTextValue).MaximumLength(2048);
        RuleFor(x => x.UnitOfMeasureId).GreaterThan(0).When(x => x.UnitOfMeasureId.HasValue);
    }
}

/// <summary>
/// Validerar nya processkrav.
/// </summary>
public sealed class CreateBuyerRfqProcessRequirementRequestValidator : BuyerRfqRequirementRequestValidatorBase<CreateBuyerRfqProcessRequirementRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för processkrav.
    /// </summary>
    public CreateBuyerRfqProcessRequirementRequestValidator()
    {
        RuleFor(x => x.ProcessTypeId).GreaterThan(0);
        RuleFor(x => x.Mode).IsInEnum();
        RuleFor(x => x.ProcessParametersJson).Must(BeValidJson).When(x => !string.IsNullOrWhiteSpace(x.ProcessParametersJson)).WithMessage("ProcessParametersJson must contain valid JSON.");
    }

    /// <summary>
    /// Kontrollerar att texten innehåller giltig JSON.
    /// </summary>
    private static bool BeValidJson(string? value)
    {
        try
        {
            using var document = JsonDocument.Parse(value!);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}

/// <summary>
/// Validerar uppdateringar av processkrav.
/// </summary>
public sealed class UpdateBuyerRfqProcessRequirementRequestValidator : UpdateBuyerRfqRequirementRequestValidatorBase<UpdateBuyerRfqProcessRequirementRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för processkrav.
    /// </summary>
    public UpdateBuyerRfqProcessRequirementRequestValidator()
    {
        RuleFor(x => x.ProcessTypeId).GreaterThan(0);
        RuleFor(x => x.Mode).IsInEnum();
        RuleFor(x => x.ProcessParametersJson).Must(BeValidJson).When(x => !string.IsNullOrWhiteSpace(x.ProcessParametersJson)).WithMessage("ProcessParametersJson must contain valid JSON.");
    }

    /// <summary>
    /// Kontrollerar att texten innehåller giltig JSON.
    /// </summary>
    private static bool BeValidJson(string? value)
    {
        try
        {
            using var document = JsonDocument.Parse(value!);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}

/// <summary>
/// Validerar nya kommersiella krav.
/// </summary>
public sealed class CreateBuyerRfqCommercialRequirementRequestValidator : BuyerRfqRequirementRequestValidatorBase<CreateBuyerRfqCommercialRequirementRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för kommersiella krav.
    /// </summary>
    public CreateBuyerRfqCommercialRequirementRequestValidator()
    {
        RuleFor(x => x.Kind).IsInEnum();
        RuleFor(x => x.Operator).IsInEnum();
        RuleFor(x => x.TextValue).MaximumLength(2048);
        RuleFor(x => x.UnitOfMeasureId).GreaterThan(0).When(x => x.UnitOfMeasureId.HasValue);
        RuleFor(x => x.CurrencyId).GreaterThan(0).When(x => x.CurrencyId.HasValue);
    }
}

/// <summary>
/// Validerar uppdateringar av kommersiella krav.
/// </summary>
public sealed class UpdateBuyerRfqCommercialRequirementRequestValidator : UpdateBuyerRfqRequirementRequestValidatorBase<UpdateBuyerRfqCommercialRequirementRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för kommersiella krav.
    /// </summary>
    public UpdateBuyerRfqCommercialRequirementRequestValidator()
    {
        RuleFor(x => x.Kind).IsInEnum();
        RuleFor(x => x.Operator).IsInEnum();
        RuleFor(x => x.TextValue).MaximumLength(2048);
        RuleFor(x => x.UnitOfMeasureId).GreaterThan(0).When(x => x.UnitOfMeasureId.HasValue);
        RuleFor(x => x.CurrencyId).GreaterThan(0).When(x => x.CurrencyId.HasValue);
    }
}

/// <summary>
/// Validerar nya logistikkrav.
/// </summary>
public sealed class CreateBuyerRfqLogisticsRequirementRequestValidator : BuyerRfqRequirementRequestValidatorBase<CreateBuyerRfqLogisticsRequirementRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för logistikkrav.
    /// </summary>
    public CreateBuyerRfqLogisticsRequirementRequestValidator()
    {
        RuleFor(x => x.Kind).IsInEnum();
        RuleFor(x => x.Operator).IsInEnum();
        RuleFor(x => x.TextValue).MaximumLength(2048);
        RuleFor(x => x.UnitOfMeasureId).GreaterThan(0).When(x => x.UnitOfMeasureId.HasValue);
        RuleFor(x => x.IncotermRuleId).GreaterThan(0).When(x => x.IncotermRuleId.HasValue);
    }
}

/// <summary>
/// Validerar uppdateringar av logistikkrav.
/// </summary>
public sealed class UpdateBuyerRfqLogisticsRequirementRequestValidator : UpdateBuyerRfqRequirementRequestValidatorBase<UpdateBuyerRfqLogisticsRequirementRequest>
{
    /// <summary>
    /// Konfigurerar reglerna för logistikkrav.
    /// </summary>
    public UpdateBuyerRfqLogisticsRequirementRequestValidator()
    {
        RuleFor(x => x.Kind).IsInEnum();
        RuleFor(x => x.Operator).IsInEnum();
        RuleFor(x => x.TextValue).MaximumLength(2048);
        RuleFor(x => x.UnitOfMeasureId).GreaterThan(0).When(x => x.UnitOfMeasureId.HasValue);
        RuleFor(x => x.IncotermRuleId).GreaterThan(0).When(x => x.IncotermRuleId.HasValue);
    }
}