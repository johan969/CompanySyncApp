namespace Arb_Int.Domain.Parties
{
    public enum OrganizationKind
    {
        Unknown = 0,
        Company = 1,
        SoleTrader = 2,
        Cooperative = 3,
        NonProfit = 4,
        GovernmentOrganization = 5,
        ResearchInstitution = 6,
        EducationalInstitution = 7,
        IndustryAssociation = 8,
        Other = 99
    }

    public enum OrganizationPersonRelationshipType
    {
        Unknown = 0,
        Employee = 1,
        Owner = 2,
        Founder = 3,
        BoardMember = 4,
        Contractor = 5,
        Consultant = 6,
        Advisor = 7,
        ExternalContact = 8,
        Other = 99
    }

    public enum AddressUsageType
    {
        Unknown = 0,
        RegisteredOffice = 1,
        Visiting = 2,
        Postal = 3,
        Billing = 4,
        Shipping = 5,
        ProductionSite = 6,
        Warehouse = 7,
        CultivationSite = 8,
        HarvestSite = 9,
        Laboratory = 10,
        Home = 11,
        Other = 99
    }

    public enum ContactPointKind
    {
        Unknown = 0,
        Email = 1,
        Phone = 2,
        MobilePhone = 3,
        Website = 4,
        LinkedIn = 5,
        SocialProfile = 6,
        InstantMessaging = 7,
        Other = 99
    }

    public enum ContactPermissionStatus
    {
        Unknown = 0,

        /// <summary>
        /// Kontaktvägen får användas enligt verksamhetens bedömning.
        /// Detta ersätter inte eventuell separat samtyckes- eller
        /// laglig-grund-hantering.
        /// </summary>
        Allowed = 1,

        DoNotContact = 2
    }

    public enum PersonUserAccountStatus
    {
        Invited,
        Active,
        Suspended,
        Revoked
    }
    public enum PartyType
    {
        Unknown = 0,
        Organization = 1,
        Person = 2
    }
}