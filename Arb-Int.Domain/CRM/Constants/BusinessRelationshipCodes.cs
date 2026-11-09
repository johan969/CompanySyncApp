namespace Arb_Int.Domain.Crm
{
    public static class BusinessRelationshipTypeCodes
    {
        public const string Supplier = "supplier";
        public const string Buyer = "buyer";
        public const string Principal = "principal";
        public const string Processor = "processor";
        public const string LogisticsProvider = "logistics-provider";
        public const string Distributor = "distributor";
        public const string Partner = "partner";
        public const string ResearchPartner = "research-partner";
    }

    public static class BusinessRelationshipStageCodes
    {
        public const string Prospect = "prospect";
        public const string Contacted = "contacted";
        public const string UnderEvaluation = "under-evaluation";
        public const string Qualified = "qualified";
        public const string Approved = "approved";
        public const string Active = "active";
        public const string Dormant = "dormant";
        public const string Rejected = "rejected";
        public const string Lost = "lost";
        public const string Archived = "archived";
    }
}