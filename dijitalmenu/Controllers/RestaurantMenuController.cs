using dijitalmenu.Models;
using Microsoft.AspNetCore.Mvc;

namespace dijitalmenu.Controllers
{
    /// <summary>
    /// Kocaoğlu Restoran teması — constants.ts verisi buraya taşındı.
    /// Erişim: /RestaurantMenu veya /RestaurantMenu/Index
    /// İleride DB'ye bağlamak için GetMenuItems() metodunu IMenuItemService ile değiştirin.
    /// </summary>
    public class RestaurantMenuController : Controller
    {
        public IActionResult Index()
        {
            ViewData["RestaurantName"]   = "Kocaoğlu Restoran";
            ViewData["RestaurantSlogan"] = "Geleneksel Lezzet Durağı";

            var vm = new RestaurantMenuViewModel
            {
                MenuItems = GetMenuItems()
            };

            return View("~/Views/Themes/RestaurantMenu/Index.cshtml", vm);
        }

        // ──────────────────────────────────────────────
        //  constants.ts → C# hardcoded data
        // ──────────────────────────────────────────────
        private static List<MenuItemVM> GetMenuItems() => new()
        {
            // Mezeler
            new() { Id = "1",  Category = "Mezeler",      Name = "Humus",             IsSpecial = false,
                    Description = "Tahin ve limon ile zenginleştirilmiş geleneksel nohut ezmesi.",
                    Price = "120 TL",  ImageUrl = "https://images.unsplash.com/photo-1585937421612-70a008356fbe?auto=format&fit=crop&q=80&w=400" },

            new() { Id = "2",  Category = "Mezeler",      Name = "Süzme",             IsSpecial = false,
                    Description = "Taze süzme yoğurt ve özel baharatlarla.",
                    Price = "120 TL",  ImageUrl = "https://images.unsplash.com/photo-1467003909585-2f8a72700288?auto=format&fit=crop&q=80&w=400" },

            new() { Id = "3",  Category = "Mezeler",      Name = "Abugannuş",         IsSpecial = false,
                    Description = "Közlenmiş patlıcan, domates ve biberden oluşan meze.",
                    Price = "120 TL",  ImageUrl = "https://images.unsplash.com/photo-1541518763669-27fef04b14ea?auto=format&fit=crop&q=80&w=400" },

            new() { Id = "4",  Category = "Mezeler",      Name = "Biber Yoğurtlama",  IsSpecial = false,
                    Description = "Közlenmiş kırmızı biber ve sarımsaklı yoğurt.",
                    Price = "120 TL",  ImageUrl = "https://images.unsplash.com/photo-1626074313790-9759275037ca?auto=format&fit=crop&q=80&w=400" },

            new() { Id = "5",  Category = "Mezeler",      Name = "Patlıcan Yoğurtlama", IsSpecial = false,
                    Description = "Közlenmiş patlıcan ve sarımsaklı yoğurt sosu.",
                    Price = "120 TL",  ImageUrl = "https://images.unsplash.com/photo-1541014741259-df529411b96a?auto=format&fit=crop&q=80&w=400" },

            new() { Id = "8",  Category = "Mezeler",      Name = "Ali Nazik",         IsSpecial = true,
                    Description = "Közlenmiş patlıcanlı yoğurt yatağında baharatlı kuşbaşı et.",
                    Price = "250 TL",  ImageUrl = "https://images.unsplash.com/photo-1565557623262-b51c2513a641?auto=format&fit=crop&q=80&w=400" },

            // Ana Yemekler
            new() { Id = "10", Category = "Ana Yemekler", Name = "Tepsi Kebabı",      IsSpecial = false,
                    Description = "Fırında özel domates sosuyla pişirilmiş, kilo ile servis edilir.",
                    Price = "1100 TL / KG", ImageUrl = "https://images.unsplash.com/photo-1529006557810-274b9b2fc783?auto=format&fit=crop&q=80&w=400" },

            new() { Id = "11", Category = "Ana Yemekler", Name = "Kağıt Kebabı",      IsSpecial = false,
                    Description = "Özel kağıtta, sebzelerle fırınlanmış lezzet (200 gr).",
                    Price = "200 TL",  ImageUrl = "https://images.unsplash.com/photo-1555939594-58d7cb561ad1?auto=format&fit=crop&q=80&w=400" },

            new() { Id = "12", Category = "Ana Yemekler", Name = "Kıyma Porsiyon",    IsSpecial = false,
                    Description = "Özel baharatlarla hazırlanmış ızgara kıyma.",
                    Price = "250 TL",  ImageUrl = "https://images.unsplash.com/photo-1514326640560-7d063ef2aed5?auto=format&fit=crop&q=80&w=400" },

            new() { Id = "13", Category = "Ana Yemekler", Name = "Kuşbaşı Porsiyon",  IsSpecial = false,
                    Description = "Izgarada pişirilmiş dana veya kuzu kuşbaşı şiş.",
                    Price = "250 TL",  ImageUrl = "https://images.unsplash.com/photo-1544025162-d76694265947?auto=format&fit=crop&q=80&w=400" },

            new() { Id = "18", Category = "Ana Yemekler", Name = "Beyti Kebabı",      IsSpecial = false,
                    Description = "Şiş kebap olarak servis edilir.",
                    Price = "250 TL",  ImageUrl = "https://images.unsplash.com/photo-1551183053-bf91a1d81141?auto=format&fit=crop&q=80&w=400" },

            new() { Id = "19", Category = "Ana Yemekler", Name = "Lahmacun",          IsSpecial = false,
                    Description = "Tam ekmek veya iki adet orta boy. Yanında maydanoz ve limon.",
                    Price = "150 TL",  ImageUrl = "https://images.unsplash.com/photo-1541745537411-b8046dc6d66c?auto=format&fit=crop&q=80&w=400" },

            // Salatalar
            new() { Id = "20", Category = "Salatalar",    Name = "Mevsim Salata",     IsSpecial = false,
                    Description = "Mevsimin en taze yeşillikleri ve sebzeleri ile hazırlanan salata.",
                    Price = "120 TL",  ImageUrl = "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?auto=format&fit=crop&q=80&w=400" },

            new() { Id = "21", Category = "Salatalar",    Name = "Çoban Salata",      IsSpecial = false,
                    Description = "Küp doğranmış domates, salatalık, biber, soğan, zeytinyağı ve limon.",
                    Price = "120 TL",  ImageUrl = "https://images.unsplash.com/photo-1540420773420-3366772f4999?auto=format&fit=crop&q=80&w=400" },

            // Tatlılar
            new() { Id = "23", Category = "Tatlılar",     Name = "Kabak Tatlısı",     IsSpecial = false,
                    Description = "Tahin ve ceviz ile servis edilen, geleneksel kabak tatlısı.",
                    Price = "150 TL",  ImageUrl = "https://images.unsplash.com/photo-1589119908995-c6837fa14848?auto=format&fit=crop&q=80&w=400" },

            new() { Id = "24", Category = "Tatlılar",     Name = "Künefe",            IsSpecial = false,
                    Description = "Özel Hatay Künefesi, Antep fıstığı ile sıcak servis edilir.",
                    Price = "150 TL",  ImageUrl = "https://images.unsplash.com/photo-1628189870503-4905d6801934?auto=format&fit=crop&q=80&w=400" },

            // İçecekler
            new() { Id = "26", Category = "İçecekler",    Name = "Ayran",             IsSpecial = false,
                    Description = "Geleneksel, ferahlatıcı yoğurt içeceği.",
                    Price = "30 TL",   ImageUrl = "https://images.unsplash.com/photo-1523362628242-f513a30efcae?auto=format&fit=crop&q=80&w=400" },

            new() { Id = "27", Category = "İçecekler",    Name = "Şalgam Suyu",       IsSpecial = false,
                    Description = "Acılı veya acısız seçenekleriyle (330 ml).",
                    Price = "50 TL",   ImageUrl = "https://images.unsplash.com/photo-1621361510340-0255c26b911a?auto=format&fit=crop&q=80&w=400" },
        };
    }
}
