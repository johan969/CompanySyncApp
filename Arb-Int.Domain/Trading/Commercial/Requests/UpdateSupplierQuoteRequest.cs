namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller redigerbar metadata för supplier-quote-roten.
/// </summary>
public sealed class UpdateSupplierQuoteRequest
{
    /// <summary>
    /// Anger quotens namn.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Beskriver quoten.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Anger supplierns egen referens.
    /// </summary>
    public string? SupplierReference { get; init; }

    /// <summary>
    /// Identifierar eventuell intern ägare.
    /// </summary>
    public Guid? OwnerUserId { get; init; }

    /// <summary>
    /// Innehåller interna anteckningar.
    /// </summary>
    public string? InternalNotes { get; init; }

    /// <summary>
    /// Concurrency-token från senast lästa quote.
    /// </summary>
    public uint RowVersion { get; init; }
}