using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace dijitalmenu.Helpers
{
    public static class StringHelper
    {
        public static string GenerateSlug(string phrase)
        {
            if (string.IsNullOrWhiteSpace(phrase))
                return string.Empty;

            string str = RemoveDiacritics(phrase).ToLower();
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", " ").Trim();
            str = str.Substring(0, str.Length <= 100 ? str.Length : 100).Trim();
            str = Regex.Replace(str, @"\s", "-");
            return str;
        }

        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

            for (int i = 0; i < normalizedString.Length; i++)
            {
                char c = normalizedString[i];
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    if (c == 'ı') stringBuilder.Append('i');
                    else if (c == 'ğ') stringBuilder.Append('g');
                    else if (c == 'ü') stringBuilder.Append('u');
                    else if (c == 'ş') stringBuilder.Append('s');
                    else if (c == 'ö') stringBuilder.Append('o');
                    else if (c == 'ç') stringBuilder.Append('c');
                    else stringBuilder.Append(c);
                }
            }

            return stringBuilder
                .ToString()
                .Normalize(NormalizationForm.FormC);
        }
    }
}
