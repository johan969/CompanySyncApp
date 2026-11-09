namespace Arb_Int.Domain.Location
{
    public class CountryWater
    {
        public Guid CountryId { get; set; }
        public Country Country { get; set; } = null!;

        public Guid WaterId { get; set; }
        public Water Water { get; set; } = null!;
    }
}