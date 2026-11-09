namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller data för att ändra RFQ:ns livscykelstatus.
/// </summary>
public sealed class ChangeBuyerRfqStatusRequest
{
    /// <summary>
    /// RFQ:ns nya status.
    /// </summary>
    public BuyerRfqStatus Status { get; init; }

    /// <summary>
    /// Tidpunkt då statusändringen sker. Aktuell UTC-tid används om värdet saknas.
    /// </summary>
    public DateTimeOffset? ChangedAtUtc { get; init; }

    /// <summary>
    /// Concurrency-token från senast lästa version.
    /// </summary>
    public uint RowVersion { get; init; }
}