namespace Arb_Int.Domain.Parties
{
    public class PartyContactPointNormalizer : IPartyContactPointNormalizer
    {
        public string Normalize(ContactPointKind kind, string value)
        {
            return kind switch
            {
                ContactPointKind.Email => value.Trim().ToLowerInvariant(),
                ContactPointKind.Phone => NormalizePhone(value),
                ContactPointKind.Website => NormalizeUrl(value),
                _ => value
            };
        }
        private static string NormalizePhone(string value)
        {
            // Remove all non-digit characters
            var digitsOnly = new string(value.Where(char.IsDigit).ToArray());

            // If the number starts with '00', replace it with '+'
            if (digitsOnly.StartsWith("00"))
            {
                digitsOnly = "+" + digitsOnly.Substring(2);
            }
            // If the number starts with '0', replace it with '+46' (assuming Swedish numbers)
            else if (digitsOnly.StartsWith("0"))
            {
                digitsOnly = "+46" + digitsOnly.Substring(1);
            }
            // If the number doesn't start with '+', assume it's a local number and prepend '+46'
            else if (!digitsOnly.StartsWith("+"))
            {
                digitsOnly = "+46" + digitsOnly;
            }

            return digitsOnly;
        }
        private static string NormalizeUrl(string value)
        {
            if (Uri.TryCreate(value, UriKind.Absolute, out var uri))
            {
                return uri.GetComponents(UriComponents.HttpRequestUrl, UriFormat.SafeUnescaped);
            }
            return value;
        }
    }
}