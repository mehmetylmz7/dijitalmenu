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

            string str = RemoveDiacritics(phrase).ToLowerInvariant();
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", " ").Trim();
            str = str.Substring(0, str.Length <= 100 ? str.Length : 100).Trim();
            str = Regex.Replace(str, @"\s", "-");
            return str;
        }

        private static string RemoveDiacritics(string text)
        {
            var sb = new StringBuilder(text.Length);
            foreach (var c in text)
            {
                switch (c)
                {
                    case 'ı':
                    case 'I':
                    case 'İ':
                        sb.Append('i');
                        break;
                    case 'ğ':
                    case 'Ğ':
                        sb.Append('g');
                        break;
                    case 'ü':
                    case 'Ü':
                        sb.Append('u');
                        break;
                    case 'ş':
                    case 'Ş':
                        sb.Append('s');
                        break;
                    case 'ö':
                    case 'Ö':
                        sb.Append('o');
                        break;
                    case 'ç':
                    case 'Ç':
                        sb.Append('c');
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }

            var normalizedString = sb.ToString().Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

            for (int i = 0; i < normalizedString.Length; i++)
            {
                char c = normalizedString[i];
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder
                .ToString()
                .Normalize(NormalizationForm.FormC);
        }
    }
}
