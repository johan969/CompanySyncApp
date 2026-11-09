namespace Arb_Int.Domain.Crm
{
    public enum RelationshipPriority
    {
        Low = 0,
        Normal = 1,
        High = 2,
        Critical = 3
    }

    public enum BusinessRelationshipContactRole
    {
        Unknown = 0,
        Primary = 1,
        DecisionMaker = 2,
        Purchasing = 3,
        Sales = 4,
        Technical = 5,
        Quality = 6,
        Regulatory = 7,
        Finance = 8,
        Logistics = 9,
        ExecutiveSponsor = 10,
        Other = 99
    }

    public enum CrmActivityType
    {
        Unknown = 0,
        Note = 1,
        Email = 2,
        PhoneCall = 3,
        Meeting = 4,
        Task = 5,
        WebsiteReview = 6,
        DocumentReview = 7,
        Other = 99
    }

    public enum CrmActivityStatus
    {
        Planned = 0,
        InProgress = 1,
        Completed = 2,
        Cancelled = 3
    }

    public enum CrmActivityDirection
    {
        None = 0,
        Inbound = 1,
        Outbound = 2
    }

    public enum CrmActivityParticipantRole
    {
        Unknown = 0,
        Organizer = 1,
        Attendee = 2,
        Sender = 3,
        Recipient = 4,
        CopiedRecipient = 5,
        Regarding = 6,
        Other = 99
    }

    public enum BiologicalSourceRelationshipKind
    {
        Unknown = 0,

        Cultivates = 1,
        WildHarvests = 2,
        Collects = 3,
        ProducesBiomass = 4,
        Processes = 5,
        Supplies = 6,
        Distributes = 7,

        SeeksToBuy = 20,
        Buys = 21,
        Uses = 22,

        Researches = 30,
        AdvisesOn = 31
    }

    public enum ClaimMappingStatus
    {
        Unmapped = 0,
        Suggested = 1,
        Confirmed = 2,
        Rejected = 3
    }

    public enum ClaimVerificationStatus
    {
        Unverified = 0,
        ObservedInPublicSource = 1,
        PartyDeclared = 2,
        ConfirmedByContact = 3,
        IndependentlyVerified = 4,
        Outdated = 5,
        Rejected = 6
    }

    public enum ClaimEvidenceType
    {
        Unknown = 0,
        PublicWebsite = 1,
        PublicDirectory = 2,
        Database = 3,
        Report = 4,
        Email = 5,
        Meeting = 6,
        PhoneCall = 7,
        Certification = 8,
        Contract = 9,
        Audit = 10,
        Other = 99
    }


    public enum BiologicalSourceClaimLocationRole
    {
        Unknown = 0,

        /// <summary>
        /// Plats där den biologiska källan odlas.
        /// </summary>
        CultivationSite = 1,

        /// <summary>
        /// Område där den biologiska källan skördas eller samlas.
        /// </summary>
        HarvestArea = 2,

        /// <summary>
        /// Ett bredare geografiskt verksamhetsområde.
        /// </summary>
        OperatingRegion = 3,

        /// <summary>
        /// En efterfrågad eller prioriterad marknad.
        /// Kan exempelvis användas för köparrelationer.
        /// </summary>
        TargetMarket = 4,

        /// <summary>
        /// Källan anger en plats men den exakta rollen är okänd.
        /// </summary>
        DeclaredRegion = 5
    }
}