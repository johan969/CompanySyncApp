using System.Linq.Expressions;

namespace Arb_Int.Domain.Sourcing.Projections
{
    /// <summary>
    /// Innehåller återanvändbara EF-projektioner för certification schemes.
    /// </summary>
    public static class CertificationSchemeProjections
    {
        /// <summary>
        /// Projekterar en scheme-version till listmodellen.
        /// </summary>
        public static Expression<Func<CertificationScheme, CertificationSchemeListResponse>> List => scheme => new CertificationSchemeListResponse
        {
            Id = scheme.Id,
            Code = scheme.Code,
            Name = scheme.Name,
            Kind = scheme.Kind,
            StandardOwner = scheme.StandardOwner,
            StandardVersion = scheme.StandardVersion,
            ValidFromUtc = scheme.ValidFromUtc,
            ValidToUtc = scheme.ValidToUtc,
            IsActive = scheme.IsActive,
            RowVersion = scheme.RowVersion
        };

        /// <summary>
        /// Projekterar en scheme-version med jurisdictions.
        /// </summary>
        public static Expression<Func<CertificationScheme, CertificationSchemeResponse>> Details => scheme => new CertificationSchemeResponse
        {
            Id = scheme.Id,
            Code = scheme.Code,
            Name = scheme.Name,
            Description = scheme.Description,
            Kind = scheme.Kind,
            StandardOwner = scheme.StandardOwner,
            StandardVersion = scheme.StandardVersion,
            PublicUrl = scheme.PublicUrl,
            ValidFromUtc = scheme.ValidFromUtc,
            ValidToUtc = scheme.ValidToUtc,
            IsActive = scheme.IsActive,
            Jurisdictions = scheme.Jurisdictions
                .OrderBy(x => x.Kind)
                .ThenBy(x => x.Code)
                .ThenBy(x => x.Id)
                .Select(x => new CertificationSchemeJurisdictionResponse
                {
                    Id = x.Id,
                    CertificationSchemeId = x.CertificationSchemeId,
                    Kind = x.Kind,
                    Code = x.Code,
                    Name = x.Name,
                    CountryId = x.CountryId,
                    Notes = x.Notes,
                    RowVersion = x.RowVersion
                })
                .ToList(),
            RowVersion = scheme.RowVersion
        };
    }
}
