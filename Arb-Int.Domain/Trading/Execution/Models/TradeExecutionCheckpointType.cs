using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Trading
{
    public class TradeExecutionCheckpointType : NamedReferenceEntity, IEntity<int>, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public int Id { get; set; }
        public ExecutionCheckpointCompletionMode DefaultCompletionMode { get; set; }
        public bool DefaultIsRequired { get; set; } = true;
        public int DefaultSortOrder { get; set; }
        public string? DefaultAutomationHandlerKey { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public ICollection<TradeExecutionCheckpoint> Checkpoints { get; set; } = [];
    }
}
