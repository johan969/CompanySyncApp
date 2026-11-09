using FluentResults;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar supplier-quote-lines medan deras parent-revision är Draft.
/// </summary>
public interface ISupplierQuoteLineService
{
    /// <summary>
    /// Skapar en line i en Draft-revision.
    /// </summary>
    Task<Result<SupplierQuoteRevisionResponse>> CreateAsync(Guid supplierQuoteRevisionId, CreateSupplierQuoteLineRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar en line i en Draft-revision.
    /// </summary>
    Task<Result<SupplierQuoteRevisionResponse>> UpdateAsync(Guid supplierQuoteLineId, UpdateSupplierQuoteLineRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tar bort en line från en Draft-revision.
    /// </summary>
    Task<Result<SupplierQuoteRevisionResponse>> DeleteAsync(Guid supplierQuoteLineId, uint rowVersion, CancellationToken cancellationToken = default);
}