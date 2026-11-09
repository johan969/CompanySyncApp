namespace Arb_Int.Domain.Worker
{
    public sealed class WorkerHeartbeat
    {
        public required string InstanceId { get; set; }     // t.ex. hostname/container id
        public required string ServiceName { get; set; }    // "Arb-Int.WorkerService"
        public required string Version { get; set; }        // asm version / image tag
        public DateTimeOffset StartedUtc { get; set; }
        public DateTimeOffset LastSeenUtc { get; set; }

        public string? Node { get; set; }                   // valfritt: machine/node
        public string? Notes { get; set; }                  // valfritt
    }
}