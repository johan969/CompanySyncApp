using Arb_Int.Domain.Articles;

namespace Arb_Int.Domain.Sourcing
{
    public enum SupplierOfferingSpecificationKind
    {
        Constituent,
        Characteristic,
        PhysicalProperty,
        ChemicalProperty,
        MicrobiologicalProperty,
        Other
    }

    public enum SupplyAvailabilityStatus
    {
        Indicative,
        Available,
        Reserved,
        PartiallyReserved,
        Unavailable,
        Expired
    }

    public enum InventoryLotStatus
    {
        Planned,
        Available,
        Reserved,
        InProduction,
        Quarantine,
        Released,
        InTransit,
        Consumed,
        Exhausted,
        Rejected
    }

    public enum InventoryLotLinkKind
    {
        SplitFrom,
        MergedFrom,
        ProcessedFrom,
        RepackedFrom,
        RelabeledFrom
    }

    public enum AnalysisStatus
    {
        Draft,
        Received,
        UnderReview,
        Verified,
        Rejected,
        Superseded
    }

    public enum AnalysisResultQualifier
    {
        Exact,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        NotDetected,
        Detected,
        Range
    }

    public enum RequirementOperator
    {
        Equals,
        NotEquals,
        AtLeast,
        AtMost,
        Between,
        OneOf,
        NoneOf,
        Exists,
        DoesNotExist,
        Contains
    }

    public enum MeasurementBasis
    {
        AsIs,
        DryMatter,
        WetWeight,
        AsSold,
        DryAsh,
        Other
    }

    public enum OriginRole
    {
        Cultivated,
        Harvested,
        Mined,
        Manufactured,
        Processed,
        Packed,
        ShippedFrom
    }

    public enum CertificationSchemeKind
    {
        Organic,
        FoodSafety,
        Sustainability,
        QualityManagement,
        ChainOfCustody,
        Regulatory,
        Other
    }

    public enum CertificationStatus
    {
        Claimed,
        PendingVerification,
        Verified,
        Expired,
        Suspended,
        Revoked,
        Rejected
    }

    public enum CertificationScope
    {
        Organization,
        Site,
        Offering,
        Lot,
        Process,
        Shipment
    }

    public enum DocumentCategory
    {
        Certificate,
        Analysis,
        SafetyDataSheet,
        TechnicalDataSheet,
        Specification,
        Traceability,
        Regulatory,
        Logistics,
        Commercial,
        Other
    }

    public enum DocumentStatus
    {
        Requested,
        Received,
        UnderReview,
        Verified,
        Rejected,
        Expired,
        Superseded
    }

    public enum VerificationState
    {
        Unverified,
        RequiresVerification,
        Verified,
        Rejected
    }

    public enum OriginVerificationStatus
    {
        Declared,
        Supported,
        Verified,
        Rejected
    }

    public enum CertificationJurisdictionKind
    {
        Unknown = 0,
        Country = 1,
        EconomicArea = 2,
        Region = 3,
        Global = 4
    }
}
