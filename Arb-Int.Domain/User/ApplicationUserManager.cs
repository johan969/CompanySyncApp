using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Arb_Int.Domain.User
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        private readonly UserStore<ApplicationUser, ApplicationRole, Arb_IntDbContext, Guid, ApplicationUserClaim,
                        ApplicationUserRole, ApplicationUserLogin, ApplicationUserToken, ApplicationRoleClaim, IdentityUserPasskey<Guid>> _store;
        public ApplicationUserManager(
            IUserStore<ApplicationUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IEnumerable<IUserValidator<ApplicationUser>> userValidators,
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors,
            IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _store = (UserStore<ApplicationUser, ApplicationRole, Arb_IntDbContext, Guid, ApplicationUserClaim,
                        ApplicationUserRole, ApplicationUserLogin, ApplicationUserToken, ApplicationRoleClaim, IdentityUserPasskey<Guid>>)store;
        }

        public virtual async Task<IdentityResult> AddToRoleByRoleId(ApplicationUser user, Guid roleId)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(roleId.ToString()) || roleId == Guid.Empty)
                throw new ArgumentNullException(nameof(roleId));

            if (await IsInRoleById(user, roleId, CancellationToken))
                return IdentityResult.Failed(ErrorDescriber.UserAlreadyInRole(roleId.ToString()));

            _store.Context.Set<ApplicationUserRole>().Add(new ApplicationUserRole { RoleId = roleId, UserId = user.Id });

            return await UpdateUserAsync(user);
        }
        public async Task<bool> IsInRoleById(ApplicationUser user, Guid roleId, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(roleId.ToString()) || roleId == Guid.Empty)
                throw new ArgumentNullException(nameof(roleId));

            var role = await _store.Context.Set<ApplicationRole>().FindAsync(roleId);
            if (role == null)
                return false;

            var userRole = await _store.Context.Set<ApplicationUserRole>().FindAsync(new object[] { user.Id, roleId }, cancellationToken);
            return userRole != null;
        }

        public static string GenerateRandomPassword(int length = 32)
        {
            return GenerateRandomPassword(totalLength: length);
            //const string chars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@$?_-";
            //var random = new Random();
            //return new string(Enumerable.Repeat(chars, length)
            //    .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string GenerateRandomPassword(
        int totalLength = 32,
        int requiredLower = 1,
        int requiredUpper = 1,
        int requiredDigits = 1,
        int requiredSymbols = 1,
        string? customSymbols = null)
        {
            if (totalLength < 0) throw new ArgumentOutOfRangeException(nameof(totalLength));
            if (requiredLower < 0 || requiredUpper < 0 || requiredDigits < 0 || requiredSymbols < 0)
                throw new ArgumentOutOfRangeException("Krav kan inte vara negativa.");

            const string lower = "abcdefghijkmnopqrstuvwxyz";
            const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
            const string digits = "0123456789";
            string symbols = customSymbols ?? "!@$?_-#¤%&+=";

            if (requiredSymbols > 0 && symbols.Length == 0)
                throw new ArgumentException("Symboluppsättningen är tom men symbols krävs.");

            int requiredSum = requiredLower + requiredUpper + requiredDigits + requiredSymbols;
            if (requiredSum > totalLength)
                throw new ArgumentException("Summan av kraven överstiger total längd.");

            var result = new List<char>(capacity: totalLength);

            // Väljer ett index i [0, n) med jämn fördelning, utan GetInt32
            static int NextIndexUniform(int n)
            {
                if (n <= 0) throw new ArgumentOutOfRangeException(nameof(n));
                Span<byte> bytes = stackalloc byte[4];
                while (true)
                {
                    System.Security.Cryptography.RandomNumberGenerator.Fill(bytes);
                    uint value = BitConverter.ToUInt32(bytes);    // 0..UInt32.MaxValue
                    uint limit = uint.MaxValue - (uint.MaxValue % (uint)n);
                    if (value < limit) return (int)(value % (uint)n);
                }
            }

            char Pick(string set) => set[NextIndexUniform(set.Length)];

            // Uppfyll minimikraven
            for (int i = 0; i < requiredLower; i++) result.Add(Pick(lower));
            for (int i = 0; i < requiredUpper; i++) result.Add(Pick(upper));
            for (int i = 0; i < requiredDigits; i++) result.Add(Pick(digits));
            for (int i = 0; i < requiredSymbols; i++) result.Add(Pick(symbols));

            // Fyll upp till total längd
            string all = lower + upper + digits + symbols;
            while (result.Count < totalLength)
                result.Add(Pick(all));

            // Fisher–Yates-shuffle (använder samma RNG-primitive)
            for (int i = result.Count - 1; i > 0; i--)
            {
                int j = NextIndexUniform(i + 1);
                (result[i], result[j]) = (result[j], result[i]);
            }

            return new string(result.ToArray());
        }
    }
}
