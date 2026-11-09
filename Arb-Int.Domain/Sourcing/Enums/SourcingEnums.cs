namespace Arb_Int.Domain.Sourcing
{
    public enum SourcingMappingStatus
    {
        Unmapped = 0,
        Suggested = 1,
        Confirmed = 2,
        Rejected = 3
    }

    public enum SourcingVerificationStatus
    {
        Unverified = 0,
        ObservedInPublicSource = 1,
        PartyDeclared = 2,
        ConfirmedByContact = 3,
        IndependentlyVerified = 4,
        Outdated = 5,
        Rejected = 6
    }

    public enum SupplierOfferingStatus
    {
        Unknown = 0,

        /// <summary>
        /// Erbjudandet har observerats men är ännu inte verifierat
        /// som kommersiellt tillgängligt.
        /// </summary>
        Observed = 1,

        Available = 2,

        /// <summary>
        /// Leverantören kan eventuellt leverera materialet efter
        /// kontakt, offert eller särskild beställning.
        /// </summary>
        AvailableOnRequest = 3,

        TemporarilyUnavailable = 4,
        Discontinued = 5
    }

    public enum SupplierOfferingBiologicalSourceRole
    {
        Unknown = 0,
        Primary = 1,
        Secondary = 2,
        Declared = 3
    }

    public enum SupplierOfferingOriginRole
    {
        Unknown = 0,
        CultivatedAt = 1,
        WildHarvestedAt = 2,
        CollectedAt = 3,
        ManufacturedAt = 4,
        ShippedFrom = 5,
        DeclaredOrigin = 6,
        Other = 99
    }

    public enum SupplierProcessCapabilityStatus
    {
        Unknown = 0,
        Observed = 1,
        Available = 2,
        AvailableOnRequest = 3,
        TemporarilyUnavailable = 4,
        Inactive = 5
    }

    public enum SourcingEvidenceType
    {
        Unknown = 0,
        PublicWebsite = 1,
        ProductCatalog = 2,
        PublicMarketplace = 3,
        Database = 4,
        Report = 5,
        Email = 6,
        Meeting = 7,
        SupplierStatement = 8,
        Quote = 9,
        Contract = 10,
        Certification = 11,
        Audit = 12,
        Other = 99
    }
}