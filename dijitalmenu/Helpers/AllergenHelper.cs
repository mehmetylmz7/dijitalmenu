using System;
using System.Collections.Generic;
using System.Linq;

namespace dijitalmenu.Helpers
{
    public static class AllergenHelper
    {
        public static readonly IReadOnlyList<string> DefaultAllergens = new[]
        {
            "Gluten",
            "Süt (Laktoz)",
            "Yumurta",
            "Yer Fıstığı",
            "Sert Kabuklu Meyveler (Fındık/Ceviz)",
            "Soya",
            "Balık",
            "Kabuklular (Deniz Ürünleri)",
            "Kereviz",
            "Hardal",
            "Susam",
            "Kükürt Dioksit / Sülfitler"
        };

        public static string? FormatAllergens(IEnumerable<string>? allergens)
        {
            if (allergens == null) return null;
            var clean = allergens
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .Select(a => a.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            return clean.Count > 0 ? string.Join(", ", clean) : null;
        }

        public static HashSet<string> ParseAllergens(string? allergensString)
        {
            if (string.IsNullOrWhiteSpace(allergensString))
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            return allergensString
                .Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(a => a.Trim())
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }
    }
}
