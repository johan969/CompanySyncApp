using Arb_Int.Domain.Base;
using Arb_Int.Domain.Parties;

namespace Arb_Int.Domain.Crm
{
    public class CrmActivityParticipant : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid CrmActivityId { get; set; }
        public CrmActivity CrmActivity { get; set; } = null!;

        public Guid PartyId { get; set; }
        public Party Party { get; set; } = null!;

        public CrmActivityParticipantRole Role { get; set; }
    }
}