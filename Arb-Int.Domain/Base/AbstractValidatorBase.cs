using System.Globalization;
using FluentValidation;

namespace Arb_Int.Domain.Base
{
    public class AbstractValidatorBase<T> : AbstractValidator<T>
        where T : class
    {
        protected static bool ContainsNoWhiteSpace(string input)
        {
            return !ContainsWhiteSpace(input);
        }
        protected static bool ContainsWhiteSpace(string input)
        {
            return input.Any(char.IsWhiteSpace);
        }
        protected static bool HasDigits(string input)
        {
            return input.Any(char.IsDigit);
        }
        protected static bool HasNoDigits(string input)
        {
            return !HasDigits(input);
        }
        protected static bool BeValidWebsiteUrl(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return true;

            var candidate = NormalizeUrl(input);

            // Kräver absolut URI + host + http/https
            if (!Uri.TryCreate(candidate, UriKind.Absolute, out var uri)) return false;
            if (uri.Scheme is not ("http" or "https")) return false;

            // Minimalt: host måste finnas och ha minst en punkt (ex. example.com)
            // (Vill du tillåta localhost/intranät-domäner, ta bort denna.)
            if (string.IsNullOrWhiteSpace(uri.Host)) return false;
            if (!uri.Host.Contains('.')) return false;

            return true;
        }

        protected static bool BeValidSocialUrl(string? input, string[] allowedHosts)
        {
            if (string.IsNullOrWhiteSpace(input)) return true;

            var candidate = NormalizeUrl(input);

            if (!Uri.TryCreate(candidate, UriKind.Absolute, out var uri)) return false;
            if (uri.Scheme is not ("http" or "https")) return false;

            // Host match
            var host = uri.Host.ToLowerInvariant();
            if (!allowedHosts.Contains(host)) return false;

            // Kräver att det finns någon path (profil, company, etc.)
            // "/" eller tom path räknas som ogiltigt.
            var path = (uri.AbsolutePath ?? "").Trim();
            if (string.IsNullOrWhiteSpace(path) || path == "/") return false;

            return true;
        }

        protected static string NormalizeUrl(string input)
        {
            var s = input.Trim();

            // Om användaren inte skrev schema, lägg på https://
            if (!s.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !s.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                s = "https://" + s;
            }

            return s;
        }
        protected static bool HaveNoDuplicates(List<string> strings)
        {
            return strings
                .Select(e => e.ToLowerInvariant())
                .Distinct()
                .Count() == strings.Count;
        }
        protected static bool BeValidAmount(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return true;

            var s = input.Trim();

            // Vanliga "skräptecken" att ta bort: mellanrum (inkl. non-breaking), kr, etc.
            s = s.Replace("\u00A0", " "); // NBSP
            s = s.Replace("kr", "", StringComparison.OrdinalIgnoreCase);
            s = s.Replace("sek", "", StringComparison.OrdinalIgnoreCase);
            s = s.Replace("euro", "", StringComparison.OrdinalIgnoreCase);
            s = s.Replace("eur", "", StringComparison.OrdinalIgnoreCase);
            s = s.Replace("pounds", "", StringComparison.OrdinalIgnoreCase);
            s = s.Replace("pound", "", StringComparison.OrdinalIgnoreCase);
            s = s.Replace("dollar", "", StringComparison.OrdinalIgnoreCase);
            s = s.Replace("usd", "", StringComparison.OrdinalIgnoreCase);
            s = s.Replace("$", "");
            s = s.Replace("£", "");
            s = s.Replace("€", "");

            // Försök parse med ett par kulturer och NumberStyles
            var styles = NumberStyles.AllowLeadingSign |
                         NumberStyles.AllowThousands |
                         NumberStyles.AllowDecimalPoint |
                         NumberStyles.AllowLeadingWhite |
                         NumberStyles.AllowTrailingWhite;

            // Prova olika varianter
            return decimal.TryParse(s, styles, new CultureInfo("sv-SE"), out _) ||
                   decimal.TryParse(s, styles, CultureInfo.InvariantCulture, out _) ||
                   decimal.TryParse(s, styles, new CultureInfo("en-GB"), out _) ||
                   decimal.TryParse(s, styles, new CultureInfo("fr-FR"), out _) ||
                   decimal.TryParse(s, styles, new CultureInfo("de-DE"), out _) ||
                   decimal.TryParse(s, styles, new CultureInfo("es-ES"), out _);
        }
        protected static bool BeNullOrAllPositiveDistinct(ICollection<int>? ids)
        {
            if (ids is null) return true;
            if (ids.Count == 0) return true;

            return ids.All(x => x > 0) && ids.Distinct().Count() == ids.Count;
        }
        protected static bool BeAllPositiveDistinct(ICollection<int>? ids)
        {
            if (ids is null) return false;
            if (ids.Count == 0) return false;
            return ids.All(x => x > 0) && ids.Distinct().Count() == ids.Count;
        }
    }
}
