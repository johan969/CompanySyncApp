using System.Linq.Expressions;

namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller EF Core-projections för externa Party-referenser.
    /// </summary>
    public static class PartyExternalReferenceProjections
    {
        /// <summary>
        /// Projekterar en extern Party-referens direkt till response-objektet.
        /// </summary>
        public static Expression<Func<PartyExternalReference, PartyExternalReferenceResponse>> Details => reference => new PartyExternalReferenceResponse
        {
            Id = reference.Id,
            PartyId = reference.PartyId,
            SourceSystem = reference.SourceSystem,
            SourceRecordKey = reference.SourceRecordKey,
            SourceUrl = reference.SourceUrl,
            FirstSeenUtc = reference.FirstSeenUtc,
            LastSeenUtc = reference.LastSeenUtc,
            ContentHash = reference.ContentHash
        };
    }
}