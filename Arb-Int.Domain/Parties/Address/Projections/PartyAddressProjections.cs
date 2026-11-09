using System.Linq.Expressions;

namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller EF Core-projections för PartyAddress.
    /// </summary>
    public static class PartyAddressProjections
    {
        /// <summary>
        /// Projekterar en PartyAddress och dess Address direkt till DTO.
        /// </summary>
        public static Expression<Func<PartyAddress, PartyAddressResponse>> Details => link => new PartyAddressResponse
        {
            Id = link.Id,
            PartyId = link.PartyId,
            UsageType = link.UsageType,
            Label = link.Label,
            Attention = link.Attention,
            IsPrimary = link.IsPrimary,
            ValidFromUtc = link.ValidFromUtc,
            ValidToUtc = link.ValidToUtc,

            Address = new AddressResponse
            {
                Id = link.Address.Id,
                CareOf = link.Address.CareOf,
                PostOfficeBox = link.Address.PostOfficeBox,
                AddressLine1 = link.Address.AddressLine1,
                AddressLine2 = link.Address.AddressLine2,
                AddressLine3 = link.Address.AddressLine3,
                PostalCode = link.Address.PostalCode,
                Locality = link.Address.Locality,
                AdministrativeArea = link.Address.AdministrativeArea,
                CountryId = link.Address.CountryId
            }
        };
    }
}