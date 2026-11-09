using System.Linq.Expressions;

namespace Arb_Int.Domain.Parties.Projections
{
    /// <summary>
    /// Innehåller EF Core-projections för PersonUserAccount.
    /// </summary>
    public static class PersonUserAccountProjections
    {
        /// <summary>
        /// Projekterar en kontokoppling direkt till dess detail-response.
        /// </summary>
        public static Expression<Func<PersonUserAccount, PersonUserAccountResponse>> Details => account => new PersonUserAccountResponse
        {
            Id = account.Id,
            PersonId = account.PersonId,
            PersonName = account.Person.Name,
            ApplicationUserId = account.ApplicationUserId,
            AuthEmailContactPointId = account.AuthEmailContactPointId,
            AuthEmail = account.AuthEmailContactPoint != null ? account.AuthEmailContactPoint.Value : null,
            Status = account.Status,
            IsExternalUser = account.IsExternalUser,
            InvitedAtUtc = account.InvitedAtUtc,
            ActivatedAtUtc = account.ActivatedAtUtc,
            RevokedAtUtc = account.RevokedAtUtc,
            InvitedByUserId = account.InvitedByUserId,
            Notes = account.Notes,
            CreateDateUtc = account.CreateDateUtc,
            RowVersion = account.RowVersion
        };
    }
}
