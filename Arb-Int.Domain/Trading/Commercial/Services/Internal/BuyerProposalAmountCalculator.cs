namespace Arb_Int.Domain.Trading;

/// <summary>
/// Beräknar monetära buyer-proposal-belopp från kvantitet, pris och eventuell skattesats.
/// </summary>
internal static class BuyerProposalAmountCalculator
{
    /// <summary>
    /// Beräknar line-belopp utan att införa en ännu odefinierad currency-rounding-policy.
    /// <para>
    /// Net = Quantity × UnitPrice.<br />
    /// Tax = Net × TaxPercent / 100 när TaxPercent finns.<br />
    /// Gross = Net + Tax.
    /// </para>
    /// </summary>
    public static BuyerProposalLineAmounts Calculate(
        decimal quantity,
        decimal unitPrice,
        decimal? taxPercent)
    {
        var netAmount = quantity * unitPrice;

        var taxAmount = taxPercent.HasValue
            ? netAmount * taxPercent.Value / 100m
            : 0m;

        var grossAmount = netAmount + taxAmount;

        return new BuyerProposalLineAmounts
        {
            NetAmount = netAmount,
            TaxAmount = taxAmount,
            GrossAmount = grossAmount
        };
    }
}

/// <summary>
/// Innehåller de tre belopp som lagras på en buyer-proposal-line.
/// </summary>
internal sealed class BuyerProposalLineAmounts
{
    /// <summary>
    /// Anger line-belopp före skatt.
    /// </summary>
    public decimal NetAmount { get; init; }

    /// <summary>
    /// Anger beräknat skattebelopp när skatt används.
    /// </summary>
    public decimal? TaxAmount { get; init; }

    /// <summary>
    /// Anger line-belopp inklusive skatt.
    /// </summary>
    public decimal GrossAmount { get; init; }
}