

using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Economy
{
    public class CurrencyExchangeRateChange : IEntity<long>
    {
        public long Id { get; set; }

        public required int CurrencyId { get; set; }

        public Currency Currency { get; set; } = null!;

        /// <summary>
        /// Datum för kursen enligt ECB (Cube time='YYYY-MM-DD')
        /// </summary>
        public required DateOnly RateDate { get; set; }

        /// <summary>
        /// Ny kurs mot EUR
        /// </summary>
        public required decimal ExchangeRate { get; set; }

        /// <summary>
        /// Föregående kurs som Currency hade precis innan uppdateringen
        /// </summary>
        public decimal? PreviousExchangeRate { get; set; }

        /// <summary>
        /// När vi hämtade datat (systemtid)
        /// </summary>
        public required DateTimeOffset RetrievedAtUtc { get; set; }

        /// <summary>
        /// Källa, t.ex. "ECB:eurofxref-daily"
        /// </summary>
        public required string Source { get; set; } = "ECB:eurofxref-daily";
    }
}