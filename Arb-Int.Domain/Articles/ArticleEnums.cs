namespace Arb_Int.Domain.Articles
{
    public enum BiologicalSourceRecordKind
    {
        Unknown = 0,

        /// <summary>
        /// Profilen representerar ett identifierat taxon.
        /// </summary>
        CanonicalTaxonProfile = 1,

        /// <summary>
        /// Exempelvis "Alaria sp.", där den exakta arten är okänd.
        /// </summary>
        UnspecifiedWithinParent = 2,

        /// <summary>
        /// En kommersiell eller importerad grupp som kan omfatta
        /// flera taxon.
        /// </summary>
        CommercialGroup = 3,

        /// <summary>
        /// Ett äldre namn, synonymt marknadsnamn eller legacyetikett.
        /// </summary>
        LegacyLabel = 4
    }

    public enum BiologicalSourceNameType
    {
        Unknown = 0,
        CommonName = 1,
        CommercialName = 2,
        LegacyName = 3,
        LocalName = 4
    }

    public enum RegulatoryRiskLevel
    {
        Unknown = 0,
        Low = 1,
        LowToMedium = 2,
        Medium = 3,
        MediumToHigh = 4,
        High = 5
    }

    public enum ContentEvidenceLevel
    {
        None = 0,
        Low = 1,
        Medium = 2,
        High = 3
    }

    public enum GeographicScope
    {
        CountryOrTerritory = 0,
        Global = 1
    }

    public enum MaterialBiologicalSourceRole
    {
        Unknown = 0,

        /// <summary>
        /// Den huvudsakliga biologiska källan.
        /// </summary>
        Primary = 1,

        /// <summary>
        /// Ytterligare biologisk källa i ett blandat material.
        /// </summary>
        Secondary = 2,

        /// <summary>
        /// Källan anges på etikett, specifikation eller motsvarande.
        /// </summary>
        Declared = 3
    }

    public enum MaterialOriginRole
    {
        Unknown = 0,
        Harvested = 1,
        Cultivated = 2,
        Mined = 3,
        Manufactured = 4,
        Recycled = 5,
        DeclaredSource = 6,
        Other = 7
    }

    public enum TransformationInputRole
    {
        Unknown = 0,
        PrimaryMaterial = 1,
        SecondaryMaterial = 2,
        Additive = 3,
        Carrier = 4,
        ProcessingAid = 5,
        Packaging = 6,
        Other = 7
    }

    public enum TransformationOutputRole
    {
        Unknown = 0,
        PrimaryProduct = 1,
        CoProduct = 2,
        ByProduct = 3,
        Waste = 4
    }
}