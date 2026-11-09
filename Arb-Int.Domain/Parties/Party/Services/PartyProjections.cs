using System.Linq.Expressions;

namespace Arb_Int.Domain.Parties
{
    /// <summary>
    /// Innehåller återanvändbara EF Core-projections för Party-domänen.
    /// </summary>
    public static class PartyProjections
    {
        /// <summary>
        /// Projekterar en Party till ett kompakt listobjekt.
        /// </summary>
        public static Expression<Func<Party, PartyListResponse>> List => party => new PartyListResponse
        {
            Id = party.Id,
            Name = party.Name,
            IsActive = party.IsActive,
            CreateDateUtc = party.CreateDateUtc,

            PartyType =
                party is Organization
                    ? PartyType.Organization
                    : party is Person
                        ? PartyType.Person
                        : PartyType.Unknown,

            LegalName =
                party is Organization
                    ? ((Organization)party).LegalName
                    : null,

            TradingName =
                party is Organization
                    ? ((Organization)party).TradingName
                    : null,

            OrganizationKind =
                party is Organization
                    ? ((Organization)party).OrganizationKind
                    : null,

            GivenName =
                party is Person
                    ? ((Person)party).GivenName
                    : null,

            FamilyName =
                party is Person
                    ? ((Person)party).FamilyName
                    : null
        };

        /// <summary>
        /// Projekterar en organisation till en detaljerad Party-Response.
        /// </summary>
        public static Expression<Func<Organization, PartyDetailsResponse>> OrganizationDetails => organization => new PartyDetailsResponse
        {
            Id = organization.Id,
            Name = organization.Name,
            Description = organization.Description,
            Notes = organization.Notes,
            IsActive = organization.IsActive,
            PartyType = PartyType.Organization,
            CreateDateUtc = organization.CreateDateUtc,
            AddressCount = organization.AddressLinks.Count(x => x.ValidToUtc == null),
            ContactPointCount = organization.ContactPoints.Count(x => x.ValidToUtc == null),
            ExternalReferenceCount = organization.ExternalReferences.Count,

            Organization = new OrganizationPartyResponse
            {
                LegalName = organization.LegalName,
                TradingName = organization.TradingName,
                OrganizationKind = organization.OrganizationKind,
                IsInternal = organization.IsInternal,
                RegistrationCountryId = organization.RegistrationCountryId,
                ParentOrganizationId = organization.ParentOrganizationId,
                ChildOrganizationCount = organization.ChildOrganizations.Count,
                IdentifierCount = organization.Identifiers.Count,
                PersonLinkCount = organization.PersonLinks.Count,

                ParentOrganization = organization.ParentOrganization == null
                    ? null
                    : new PartyReferenceResponse
                    {
                        Id = organization.ParentOrganization.Id,
                        Name = organization.ParentOrganization.Name,
                        PartyType = PartyType.Organization,
                        IsActive = organization.ParentOrganization.IsActive
                    }
            },

            Person = null
        };

        /// <summary>
        /// Projekterar en person till en detaljerad Party-Response.
        /// </summary>
        public static Expression<Func<Person, PartyDetailsResponse>> PersonDetails => person => new PartyDetailsResponse
        {
            Id = person.Id,
            Name = person.Name,
            Description = person.Description,
            Notes = person.Notes,
            IsActive = person.IsActive,
            PartyType = PartyType.Person,
            CreateDateUtc = person.CreateDateUtc,
            AddressCount = person.AddressLinks.Count(x => x.ValidToUtc == null),
            ContactPointCount = person.ContactPoints.Count(x => x.ValidToUtc == null),
            ExternalReferenceCount = person.ExternalReferences.Count,

            Organization = null,

            Person = new PersonPartyResponse
            {
                GivenName = person.GivenName,
                MiddleName = person.MiddleName,
                FamilyName = person.FamilyName,
                PreferredName = person.PreferredName,
                Honorific = person.Honorific,
                PreferredLanguageCode = person.PreferredLanguageCode,
                OrganizationLinkCount = person.OrganizationLinks.Count,
                HasUserAccount = person.UserAccount != null
            }
        };
    }
}