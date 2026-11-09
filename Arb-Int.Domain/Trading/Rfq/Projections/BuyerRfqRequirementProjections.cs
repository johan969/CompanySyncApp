using System.Linq.Expressions;

namespace Arb_Int.Domain.Trading.Projections;

/// <summary>
/// Innehåller EF Core-projections för den polymorfa RFQ-requirement-hierarkin.
/// </summary>
public static class BuyerRfqRequirementProjections
{
    /// <summary>
    /// Projekterar ett konkret RFQ-krav till ett gemensamt response-objekt med typspecifika detaljer.
    /// </summary>
    public static Expression<Func<BuyerRfqRequirement, BuyerRfqRequirementResponse>> Details => requirement => new BuyerRfqRequirementResponse
    {
        Id = requirement.Id,
        BuyerRfqRevisionId = requirement.BuyerRfqRevisionId,
        BuyerRfqLineId = requirement.BuyerRfqLineId,
        BuyerRfqLineNumber = requirement.BuyerRfqLine == null ? null : requirement.BuyerRfqLine.LineNumber,
        RequirementType =
            requirement is BuyerRfqMaterialRequirement ? BuyerRfqRequirementType.Material :
            requirement is BuyerRfqBiologicalSourceRequirement ? BuyerRfqRequirementType.BiologicalSource :
            requirement is BuyerRfqOriginRequirement ? BuyerRfqRequirementType.Origin :
            requirement is BuyerRfqCertificationRequirement ? BuyerRfqRequirementType.Certification :
            requirement is BuyerRfqDocumentRequirement ? BuyerRfqRequirementType.Document :
            requirement is BuyerRfqConstituentRequirement ? BuyerRfqRequirementType.Constituent :
            requirement is BuyerRfqCharacteristicRequirement ? BuyerRfqRequirementType.Characteristic :
            requirement is BuyerRfqProcessRequirement ? BuyerRfqRequirementType.Process :
            requirement is BuyerRfqCommercialRequirement ? BuyerRfqRequirementType.Commercial :
            BuyerRfqRequirementType.Logistics,
        Name = requirement.Name,
        Description = requirement.Description,
        Level = requirement.Level,
        Weight = requirement.Weight,
        AlternativeGroupKey = requirement.AlternativeGroupKey,
        IsActive = requirement.IsActive,
        SortOrder = requirement.SortOrder,
        BuyerOriginalText = requirement.BuyerOriginalText,
        InternalNotes = requirement.InternalNotes,
        CreateDateUtc = requirement.CreateDateUtc,
        RowVersion = requirement.RowVersion,

        Material = requirement is BuyerRfqMaterialRequirement
            ? new BuyerRfqMaterialRequirementDetailsResponse
            {
                MaterialId = ((BuyerRfqMaterialRequirement)requirement).MaterialId,
                MaterialTypeId = ((BuyerRfqMaterialRequirement)requirement).MaterialTypeId,
                MatchMode = ((BuyerRfqMaterialRequirement)requirement).MatchMode,
                AllowSubstitutes = ((BuyerRfqMaterialRequirement)requirement).AllowSubstitutes
            }
            : null,

        BiologicalSource = requirement is BuyerRfqBiologicalSourceRequirement
            ? new BuyerRfqBiologicalSourceRequirementDetailsResponse
            {
                BiologicalSourceId = ((BuyerRfqBiologicalSourceRequirement)requirement).BiologicalSourceId,
                Operator = ((BuyerRfqBiologicalSourceRequirement)requirement).Operator
            }
            : null,

        Origin = requirement is BuyerRfqOriginRequirement
            ? new BuyerRfqOriginRequirementDetailsResponse
            {
                GeographicLocationId = ((BuyerRfqOriginRequirement)requirement).GeographicLocationId,
                CountryId = ((BuyerRfqOriginRequirement)requirement).CountryId,
                OriginRole = ((BuyerRfqOriginRequirement)requirement).OriginRole,
                Operator = ((BuyerRfqOriginRequirement)requirement).Operator
            }
            : null,

        Certification = requirement is BuyerRfqCertificationRequirement
            ? new BuyerRfqCertificationRequirementDetailsResponse
            {
                CertificationSchemeId = ((BuyerRfqCertificationRequirement)requirement).CertificationSchemeId,
                Operator = ((BuyerRfqCertificationRequirement)requirement).Operator,
                RequiredScope = ((BuyerRfqCertificationRequirement)requirement).RequiredScope,
                CertificationSchemeJurisdictionId = ((BuyerRfqCertificationRequirement)requirement).CertificationSchemeJurisdictionId,
                MustBeValidAtDelivery = ((BuyerRfqCertificationRequirement)requirement).MustBeValidAtDelivery
            }
            : null,

        Document = requirement is BuyerRfqDocumentRequirement
            ? new BuyerRfqDocumentRequirementDetailsResponse
            {
                DocumentTypeId = ((BuyerRfqDocumentRequirement)requirement).DocumentTypeId,
                Operator = ((BuyerRfqDocumentRequirement)requirement).Operator,
                MustBeLotSpecific = ((BuyerRfqDocumentRequirement)requirement).MustBeLotSpecific,
                MustBeIssuedByAccreditedParty = ((BuyerRfqDocumentRequirement)requirement).MustBeIssuedByAccreditedParty,
                MaximumAgeDays = ((BuyerRfqDocumentRequirement)requirement).MaximumAgeDays
            }
            : null,

        Constituent = requirement is BuyerRfqConstituentRequirement
            ? new BuyerRfqConstituentRequirementDetailsResponse
            {
                ConstituentId = ((BuyerRfqConstituentRequirement)requirement).ConstituentId,
                Operator = ((BuyerRfqConstituentRequirement)requirement).Operator,
                MinimumValue = ((BuyerRfqConstituentRequirement)requirement).MinimumValue,
                MaximumValue = ((BuyerRfqConstituentRequirement)requirement).MaximumValue,
                TargetValue = ((BuyerRfqConstituentRequirement)requirement).TargetValue,
                UnitOfMeasureId = ((BuyerRfqConstituentRequirement)requirement).UnitOfMeasureId,
                Basis = ((BuyerRfqConstituentRequirement)requirement).Basis,
                AnalysisMethod = ((BuyerRfqConstituentRequirement)requirement).AnalysisMethod,
                MustBeVerifiedForEveryLot = ((BuyerRfqConstituentRequirement)requirement).MustBeVerifiedForEveryLot,
                AccreditedLaboratoryRequired = ((BuyerRfqConstituentRequirement)requirement).AccreditedLaboratoryRequired
            }
            : null,

        Characteristic = requirement is BuyerRfqCharacteristicRequirement
            ? new BuyerRfqCharacteristicRequirementDetailsResponse
            {
                CharacteristicId = ((BuyerRfqCharacteristicRequirement)requirement).CharacteristicId,
                Operator = ((BuyerRfqCharacteristicRequirement)requirement).Operator,
                ExpectedTextValue = ((BuyerRfqCharacteristicRequirement)requirement).ExpectedTextValue,
                ExpectedNumericValue = ((BuyerRfqCharacteristicRequirement)requirement).ExpectedNumericValue,
                UnitOfMeasureId = ((BuyerRfqCharacteristicRequirement)requirement).UnitOfMeasureId
            }
            : null,

        Process = requirement is BuyerRfqProcessRequirement
            ? new BuyerRfqProcessRequirementDetailsResponse
            {
                ProcessTypeId = ((BuyerRfqProcessRequirement)requirement).ProcessTypeId,
                Mode = ((BuyerRfqProcessRequirement)requirement).Mode,
                ProcessParametersJson = ((BuyerRfqProcessRequirement)requirement).ProcessParametersJson
            }
            : null,

        Commercial = requirement is BuyerRfqCommercialRequirement
            ? new BuyerRfqCommercialRequirementDetailsResponse
            {
                Kind = ((BuyerRfqCommercialRequirement)requirement).Kind,
                Operator = ((BuyerRfqCommercialRequirement)requirement).Operator,
                NumericValue = ((BuyerRfqCommercialRequirement)requirement).NumericValue,
                TextValue = ((BuyerRfqCommercialRequirement)requirement).TextValue,
                UnitOfMeasureId = ((BuyerRfqCommercialRequirement)requirement).UnitOfMeasureId,
                CurrencyId = ((BuyerRfqCommercialRequirement)requirement).CurrencyId
            }
            : null,

        Logistics = requirement is BuyerRfqLogisticsRequirement
            ? new BuyerRfqLogisticsRequirementDetailsResponse
            {
                Kind = ((BuyerRfqLogisticsRequirement)requirement).Kind,
                Operator = ((BuyerRfqLogisticsRequirement)requirement).Operator,
                NumericValue = ((BuyerRfqLogisticsRequirement)requirement).NumericValue,
                TextValue = ((BuyerRfqLogisticsRequirement)requirement).TextValue,
                UnitOfMeasureId = ((BuyerRfqLogisticsRequirement)requirement).UnitOfMeasureId,
                IncotermRuleId = ((BuyerRfqLogisticsRequirement)requirement).IncotermRuleId
            }
            : null
    };
}