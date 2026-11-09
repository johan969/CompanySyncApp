using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Location
{
    public class Continent : IEntity<int>, INamed
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string ContinentCode { get; set; }
        public int? Id_Old { get; set; }

        public List<Country> Countries { get; set; } = [];

    }
}