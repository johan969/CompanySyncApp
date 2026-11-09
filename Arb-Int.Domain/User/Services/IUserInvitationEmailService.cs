using FluentResults;

namespace Arb_Int.Domain.User
{
    /// <summary>
    /// Skapar och schemalägger e-post för initial kontosättning av en inviterad användare.
    /// </summary>
    public interface IUserInvitationEmailService
    {
        /// <summary>
        /// Skapar en engångslänk för lösenordssättning och schemalägger invitationen till vald kontaktpunkt.
        /// </summary>
        Task<Result> QueueInvitationAsync(ApplicationUser user, Guid contactPointId, string clientUrl, CancellationToken cancellationToken = default);
    }
}
