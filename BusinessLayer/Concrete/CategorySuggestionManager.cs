using BusinessLayer.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.Concrete
{
    public class CategorySuggestionManager : ICategorySuggestionService
    {
        public List<string> GetSuggestions(List<string> existingCategories, string restaurantName)
        {
            if (restaurantName == null) restaurantName = string.Empty;
            
            // Normalize inputs for comparison
            var existingSet = new HashSet<string>(
                existingCategories.Select(c => c.Trim().ToLowerInvariant()), 
                StringComparer.OrdinalIgnoreCase
            );

            // Mutfak Türleri için Hazır Şablonlar
            var turkishGrillPool = new List<string> { "Çorbalar", "Salatalar", "Başlangıçlar", "Mezeler", "Dönerler", "Kebaplar", "Pideler & Lahmacunlar", "Tatlılar", "Soğuk İçecekler", "Sıcak İçecekler" };
            var cafeDessertPool = new List<string> { "Sıcak Kahveler", "Soğuk Kahveler", "Bitki Çayları", "Pastalar & Kekler", "Waffle & Krep", "Tuzlu Atıştırmalıklar", "Sandviçler", "Soğuk İçecekler" };
            var fastFoodPool = new List<string> { "Burgerler", "Pizzalar", "Dürümler", "Yan Ürünler (Patates vb.)", "Soslar", "Tavuk Kovaları", "Tatlılar", "Soğuk İçecekler" };
            var generalPool = new List<string> { "Günün Çorbası", "Başlangıçlar", "Salatalar", "Ara Sıcaklar", "Ana Yemekler", "Makarnalar", "Tatlılar", "İçecekler" };

            // Restoran ismine göre mutfak türü tespiti
            var nameLower = restaurantName.ToLowerInvariant();
            List<string> selectedPool;

            if (nameLower.Contains("kebap") || nameLower.Contains("kebab") || nameLower.Contains("döner") || 
                nameLower.Contains("doner") || nameLower.Contains("lahmacun") || nameLower.Contains("pide") || 
                nameLower.Contains("ocakbaşı") || nameLower.Contains("ocakbasi") || nameLower.Contains("ızgara") || 
                nameLower.Contains("izgara") || nameLower.Contains("et"))
            {
                selectedPool = turkishGrillPool;
            }
            else if (nameLower.Contains("cafe") || nameLower.Contains("kahve") || nameLower.Contains("pastane") || 
                     nameLower.Contains("tatlı") || nameLower.Contains("tatli") || nameLower.Contains("fırın") || 
                     nameLower.Contains("firin") || nameLower.Contains("bistro") || nameLower.Contains("coffee") || 
                     nameLower.Contains("tea"))
            {
                selectedPool = cafeDessertPool;
            }
            else if (nameLower.Contains("burger") || nameLower.Contains("pizza") || nameLower.Contains("fast") || 
                     nameLower.Contains("sandviç") || nameLower.Contains("sandvic") || nameLower.Contains("kızartma") || 
                     nameLower.Contains("katık") || nameLower.Contains("katik") || nameLower.Contains("dönerci"))
            {
                selectedPool = fastFoodPool;
            }
            else
            {
                selectedPool = generalPool;
            }

            // Zaten eklenmiş olanları filtrele ve kalanları döndür
            return selectedPool
                .Where(s => !existingSet.Contains(s.Trim().ToLowerInvariant()))
                .ToList();
        }
    }
}
