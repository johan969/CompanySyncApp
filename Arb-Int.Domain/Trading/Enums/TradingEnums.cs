namespace Arb_Int.Domain.Trading
{
    public enum BuyerRfqStatus
    {
        Draft,
        Active,
        OnHold,
        Closed,
        Cancelled
    }

    public enum BuyerRfqRevisionStatus
    {
        Draft,
        ReleasedForMatching,
        Superseded,
        Cancelled
    }

    public enum BuyerRfqPriority
    {
        Low,
        Normal,
        High,
        Critical
    }

    public enum InformationVisibility
    {
        InternalOnly,
        Restricted,
        ShareAnonymized,
        ShareWithApprovedCounterparties
    }

    public enum MaterialOptionKind
    {
        Preferred,
        Allowed,
        Excluded
    }

    public enum RequirementLevel
    {
        Mandatory,
        Preferred,
        Informational
    }

    public enum RequirementEvaluationOutcome
    {
        Pass,
        Fail,
        Unknown,
        NotApplicable
    }

    public enum MaterialMatchMode
    {
        ExactMaterial,
        MaterialType,
        Equivalent,
        SubstituteAllowed
    }

    public enum MatchingRuleSetStatus
    {
        Draft,
        Active,
        Retired
    }

    public enum MatchingRunStatus
    {
        Queued,
        Running,
        Completed,
        Failed,
        Cancelled
    }

    public enum SupplySolutionKind
    {
        SingleSupplier,
        SupplierPool,
        TransformationChain,
        Hybrid
    }

    public enum SupplySolutionOutcome
    {
        Qualified,
        RequiresVerification,
        HardFail,
        Incomplete
    }

    public enum SupplySolutionStatus
    {
        Generated,
        UnderReview,
        Shortlisted,
        Rejected,
        ApprovedForProposal,
        Expired
    }

    public enum SupplySolutionStepType
    {
        Supply,
        Processing,
        QualityControl,
        Consolidation,
        Storage,
        Transport,
        Packaging,
        Customs
    }

    public enum SupplySolutionStepStatus
    {
        Proposed,
        Confirmed,
        Rejected,
        Completed,
        Cancelled
    }

    public enum SupplyAllocationStatus
    {
        Proposed,
        AvailabilityUnverified,
        Reserved,
        Confirmed,
        Rejected,
        Released
    }

    public enum CapacityEvaluationOutcome
    {
        Sufficient,
        PartiallySufficient,
        Insufficient,
        Unknown
    }

    public enum MatchingModerationDecisionKind
    {
        Shortlist,
        Reject,
        RequestVerification,
        ApproveForProposal,
        Reopen
    }

    public enum SupplierLineKind
    {
        Material,
        ProcessingService,
        QualityService,
        StorageService,
        LogisticsService,
        PackagingService,
        CustomsService,
        Other
    }

    public enum SupplierQuoteStatus
    {
        Draft,
        Received,
        UnderReview,
        AcceptedInternally,
        Rejected,
        Expired,
        Withdrawn
    }

    public enum SupplierQuoteRevisionStatus
    {
        Draft,
        Received,
        Superseded,
        AcceptedInternally,
        Rejected,
        Expired
    }

    public enum BuyerProposalStatus
    {
        Draft,
        Issued,
        PartiallyAccepted,
        Accepted,
        Rejected,
        Expired,
        Withdrawn
    }

    public enum BuyerProposalRevisionStatus
    {
        Draft,
        Issued,
        Superseded,
        Accepted,
        Rejected,
        Expired
    }

    public enum BuyerProposalOptionStatus
    {
        Draft,
        Offered,
        Accepted,
        Rejected,
        Expired
    }

    public enum BuyerProposalAcceptanceStatus
    {
        PendingInternalApproval,
        Approved,
        Rejected,
        Superseded
    }

    public enum DisclosurePackageStatus
    {
        Draft,
        UnderReview,
        Approved,
        Sent,
        Superseded,
        Withdrawn
    }

    public enum DisclosureItemClassification
    {
        Public,
        Anonymized,
        Confidential,
        InternalOnly
    }

    public enum DisclosureDirection
    {
        ToBuyer,
        ToSupplier,
        ToPartner
    }

    public enum ExternalConversationStatus
    {
        Open,
        WaitingForExternal,
        WaitingForInternal,
        Closed,
        Cancelled
    }

    public enum ExternalMessageDirection
    {
        Inbound,
        Outbound,
        Internal
    }

    public enum CommunicationChannel
    {
        Email,
        Portal,
        Phone,
        Meeting,
        Manual,
        Api
    }

    public enum SalesOrderStatus
    {
        Draft,
        Confirmed,
        OnHold,
        PartiallyFulfilled,
        Fulfilled,
        Cancelled,
        Closed
    }

    public enum PurchaseOrderStatus
    {
        Draft,
        Sent,
        Acknowledged,
        OnHold,
        PartiallyFulfilled,
        Fulfilled,
        Cancelled,
        Closed
    }

    public enum ResponsibilityTransferKind
    {
        Title,
        Risk,
        Custody
    }

    public enum ResponsibilityTransferStatus
    {
        Planned,
        Confirmed,
        Disputed,
        Cancelled
    }

    public enum ShipmentStatus
    {
        Planned,
        BookingPending,
        Booked,
        InTransit,
        Delayed,
        Delivered,
        Cancelled
    }

    public enum ShipmentMode
    {
        Road,
        Sea,
        Air,
        Rail,
        Multimodal,
        Courier,
        Other
    }

    public enum ShipmentLegStatus
    {
        Planned,
        Booked,
        InTransit,
        Delayed,
        Completed,
        Cancelled
    }

    public enum ShipmentMilestoneStatus
    {
        Planned,
        Completed,
        Missed,
        Waived
    }

    public enum CarrierBookingStatus
    {
        Draft,
        Requested,
        Confirmed,
        Rejected,
        Cancelled,
        Completed
    }

    public enum CustomsReferenceKind
    {
        ExportDeclaration,
        ImportDeclaration,
        TransitDeclaration,
        CommodityCode,
        HealthCertificate,
        PhytosanitaryCertificate,
        Other
    }

    public enum InvoiceDirection
    {
        AccountsReceivable,
        AccountsPayable
    }

    public enum CommercialInvoiceStatus
    {
        Draft,
        ReadyForExport,
        Exported,
        Booked,
        PartiallyPaid,
        Paid,
        Overdue,
        Credited,
        Cancelled,
        SyncFailed
    }

    public enum PaymentStatus
    {
        NotDue,
        Unpaid,
        PartiallyPaid,
        Paid,
        Overdue,
        Disputed,
        WrittenOff
    }

    public enum ExternalSystemOwnership
    {
        ArbInt,
        ExternalSystem
    }

    public enum ExternalSystemKind
    {
        Accounting,
        FreightForwarder,
        Carrier,
        Laboratory,
        DocumentStorage,
        Crm,
        Marketplace,
        Other
    }

    public enum SyncDirection
    {
        Push,
        Pull
    }

    public enum SyncStatus
    {
        Pending,
        Running,
        Succeeded,
        Failed,
        Ignored
    }

    public enum ExecutionCheckpointStatus
    {
        Pending,
        Ready,
        InProgress,
        Blocked,
        Completed,
        Waived,
        Failed,
        Cancelled
    }

    public enum ExecutionCheckpointCompletionMode
    {
        Manual,
        Automatic,
        Hybrid
    }

    public enum ReconciliationTaskStatus
    {
        Open,
        InProgress,
        Blocked,
        Completed,
        Cancelled
    }

    public enum ReconciliationTaskType
    {
        InvoiceOrder,
        InvoicePayment,
        ShipmentOrder,
        Quantity,
        Price,
        Tax,
        ExternalSync,
        Other
    }

    public enum CommercialRequirementKind
    {
        TargetPrice,
        MaximumPrice,
        MinimumOrderQuantity,
        MaximumLeadTime,
        PaymentTerms,
        Recurrence,
        ContractPeriod,
        SampleRequired,
        Other
    }

    public enum LogisticsRequirementKind
    {
        Incoterm,
        NamedPlace,
        Packaging,
        TemperatureMinimum,
        TemperatureMaximum,
        ShelfLifeAtDelivery,
        DeliveryWindow,
        CarrierConstraint,
        Other
    }

    public enum ProcessRequirementMode
    {
        Required,
        Preferred,
        Prohibited
    }
}
