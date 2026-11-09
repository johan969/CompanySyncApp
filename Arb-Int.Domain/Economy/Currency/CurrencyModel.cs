using Arb_Int.Domain.Base;
using Arb_Int.Domain.Location;

namespace Arb_Int.Domain.Economy
{
    public class Currency : IEntity<int>, INamed, IActionTracker, ICode
    {
        public int Id { get; set; }
        /// <summary>
        /// Name of the currency
        /// </summary>
        public required string Name { get; set; }
        /// <summary>
        /// ISO 4217 alphabetic code
        /// </summary>
        public string Code { get; set; } = string.Empty;
        /// <summary>
        /// Symbol for the currency
        /// </summary>
        public string? Symbol { get; set; }
        /// <summary>
        /// ISO 4217 numeric code
        /// </summary>
        public string? Number { get; set; }
        /// <summary>
        /// Number of digits after the decimal separator
        /// </summary>
        public int? DecimalDigits { get; set; }
        /// <summary>
        /// Exchange rate against the Euro
        /// </summary>
        public decimal? ExchangeRate { get; set; }
        /// <summary>
        /// Pointer to latest currency exchange rate change ID (if any)
        /// </summary>
        public long? LatestExchangeRateChangeId { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        /// <summary>
        /// Pointer to latest currency exchange rate change (if any)
        /// </summary>
        //public CurrencyExchangeRateChange? LatestExchangeRateChange { get; set; }
        public List<Country> Countries { get; set; } = [];
    }
}