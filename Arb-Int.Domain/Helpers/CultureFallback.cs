namespace Arb_Int.Domain.Helpers
{
    public static class CultureFallback
    {
        public static IReadOnlyList<string> GetFallbackChain(string? culture)
        {
            var list = new List<string>();

            if (!string.IsNullOrWhiteSpace(culture))
            {
                list.Add(culture);

                var dash = culture.IndexOf('-');
                if (dash > 0)
                {
                    var neutral = culture[..dash];
                    list.Add(neutral);
                }
            }

            // sista fallback – ha en standard i systemet
            list.Add("en-GB");
            list.Add("en");

            return [.. list.Distinct(StringComparer.OrdinalIgnoreCase)];
        }
    }
}