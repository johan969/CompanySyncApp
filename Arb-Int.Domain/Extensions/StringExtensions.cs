using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Arb_Int.Domain.Extensions
{
    public static class StringExtensions
    {
        private static readonly Regex _invalidCharsInTranslationKeyRegex = new("[^a-zA-Z0-9\\s.]", RegexOptions.Compiled);
        private static readonly Regex _whitespaceOrDotInTranslationKeyRegex = new("[\\s.]+", RegexOptions.Compiled);
        private static readonly Regex _multiUnderscoreRegex = new("_+", RegexOptions.Compiled);
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }



        /// <summary>
        /// Used to normalize a person name so that capitalisation is correct
        /// </summary>
        /// <param name="name">Un-normalized name</param>
        /// <returns>Normalized name</returns>
        public static string NormalizeName(this string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;

            var result = new StringBuilder(name.Length);
            bool newWord = true;

            foreach (var ch in name.Trim())
            {
                if (char.IsWhiteSpace(ch) || ch == '-' || ch == '\'' || ch == '´')
                {
                    // Behåll separatorn och markera att nästa bokstav är ny
                    result.Append(ch);
                    newWord = true;
                }
                else
                {
                    if (newWord)
                    {
                        result.Append(char.ToUpperInvariant(ch));
                        newWord = false;
                    }
                    else
                    {
                        result.Append(char.ToLowerInvariant(ch));
                    }
                }
            }

            return result.ToString();
        }

        public static string RemoveDiacritics(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var normalized = input.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder(normalized.Length);

            foreach (var ch in normalized)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(ch);

                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                    builder.Append(ch);
            }

            return builder
                .ToString()
                .Normalize(NormalizationForm.FormC);
        }

        public static string FilterCharacters(this string? input, bool includeLetters = true, bool includeDigits = false, bool includeSymbols = false, bool includePunctuations = false, bool includeWhiteSpace = false, bool includeSurogates = false)
        {
            if (input == null)
                return string.Empty;
            StringBuilder output = new StringBuilder();
            var chars = input.ToCharArray();
            foreach (char c in chars)
            {
                if (char.IsLetter(c) && includeLetters)
                    output.Append(c);
                if (char.IsDigit(c) && includeDigits)
                    output.Append(c);
                if (char.IsSymbol(c) && includeSymbols)
                    output.Append(c);
                if (char.IsWhiteSpace(c) && includeWhiteSpace)
                    output.Append(c);
                if (char.IsSurrogate(c) && includeSurogates)
                    output.Append(c);
                if (char.IsPunctuation(c) && includePunctuations)
                    output.Append(c);
            }
            return output.ToString();
        }


        public static bool ContainsAny(this string source, IEnumerable<string> values, StringComparison comparison = StringComparison.Ordinal)
        {
            if (string.IsNullOrEmpty(source))
                return false;

            foreach (var value in values)
            {
                if (source.Contains(value, comparison))
                    return true;
            }

            return false;
        }
    }
}