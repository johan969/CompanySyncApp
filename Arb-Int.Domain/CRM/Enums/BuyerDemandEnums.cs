namespace Arb_Int.Domain.Crm
{
    public enum BuyerDemandKnowledgeLevel
    {
        Inferred,
        Indicated,
        Confirmed,
        Invalidated
    }

    public enum BuyerDemandStatus
    {
        Active,
        Dormant,
        ConvertedToRfq,
        Invalidated,
        Closed
    }

    public enum EvidencePosition
    {
        Supports,
        Contradicts,
        Neutral
    }

    public enum EvidenceSourceKind
    {
        Website,
        Email,
        Meeting,
        Call,
        Document,
        Registry,
        Marketplace,
        Import,
        Manual
    }
}
