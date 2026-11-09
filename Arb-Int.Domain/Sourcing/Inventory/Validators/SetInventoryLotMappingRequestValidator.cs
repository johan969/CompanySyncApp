using FluentValidation;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Validerar shape för offering- och materialmapping av en inventory lot.
    /// </summary>
    public sealed class SetInventoryLotMappingRequestValidator : AbstractValidator<SetInventoryLotMappingRequest>
    {
        /// <summary>
        /// Skapar validatorn; båda mapping-referenserna får vara null för att rensa mappingen.
        /// </summary>
        public SetInventoryLotMappingRequestValidator()
        {
        }
    }
}
