using BusinessLayer.Abstract;
using dijitalmenu.Filters;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace dijitalmenu.Areas.Restaurant.Controllers
{
    [Area("Restaurant")]
    [ServiceFilter(typeof(RestaurantAuthFilter))]
    public class BuilderController : Controller
    {
        private const int MaxAddressLength = 500;
        private const int MaxPhoneLength = 25;
        private readonly IRestaurantService _restaurantService;
        private readonly IMenuService _menuService;
        private readonly ICategoryService _categoryService;
        private readonly IMenuItemService _menuItemService;
        private readonly IThemeService _themeService;
        private readonly ICategorySuggestionService _categorySuggestionService;

        public BuilderController(IRestaurantService restaurantService, IMenuService menuService,
            ICategoryService categoryService, IMenuItemService menuItemService,
            IThemeService themeService, ICategorySuggestionService categorySuggestionService)
        {
            _restaurantService = restaurantService;
            _menuService = menuService;
            _categoryService = categoryService;
            _menuItemService = menuItemService;
            _themeService = themeService;
            _categorySuggestionService = categorySuggestionService;
        }

        private int GetRestaurantId() =>
            int.Parse(HttpContext.Session.GetString("RestaurantId")!);

        private Menu GetOrCreateMyMenu(int restaurantId)
        {
            var menu = _menuService.TGetListAll().FirstOrDefault(m => m.RestaurantId == restaurantId);
            if (menu == null)
            {
                menu = new Menu { RestaurantId = restaurantId };
                _menuService.TInsert(menu);
            }
            return menu;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var rid = GetRestaurantId();
            var restaurant = _restaurantService.TGetByID(rid);
            if (restaurant == null) return NotFound();

            var menu = GetOrCreateMyMenu(rid);
            var categories = _categoryService.TGetListAll().Where(c => c.MenuId == menu.Id).OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id).ToList();
            var catIds = categories.Select(c => c.Id).ToHashSet();
            var menuItems = _menuItemService.TGetListAll().Where(mi => catIds.Contains(mi.CategoryId)).OrderBy(mi => mi.DisplayOrder).ThenBy(mi => mi.Id).ToList();

            var existingNames = categories.Select(c => c.Name).ToList();
            var suggestions = _categorySuggestionService.GetSuggestions(existingNames, restaurant.Name);

            ViewBag.Restaurant = restaurant;
            ViewBag.Categories = categories;
            ViewBag.MenuItems = menuItems;
            ViewBag.Themes = _themeService.TGetListAll().Where(t => t.IsActive).OrderBy(t => t.Id).ToList();
            ViewBag.Suggestions = suggestions;
            ViewBag.RestaurantUsername = HttpContext.Session.GetString("RestaurantUsername");

            return View();
        }

        [HttpPost]
        public IActionResult UpdateCategoryOrder([FromBody] List<int> categoryIds)
        {
            if (categoryIds == null || !categoryIds.Any())
                return Json(new { success = false, message = "Kategori listesi boş olamaz." });

            var rid = GetRestaurantId();
            var menu = GetOrCreateMyMenu(rid);
            var myCategories = _categoryService.TGetListAll().Where(c => c.MenuId == menu.Id).ToDictionary(c => c.Id);

            for (int i = 0; i < categoryIds.Count; i++)
            {
                var id = categoryIds[i];
                if (myCategories.TryGetValue(id, out var cat))
                {
                    cat.DisplayOrder = i;
                    _categoryService.TUpdate(cat);
                }
            }

            return Json(new { success = true, message = "Kategori sıralaması güncellendi." });
        }

        [HttpPost]
        public IActionResult UpdateMenuItemOrder([FromBody] List<int> itemIds)
        {
            if (itemIds == null || !itemIds.Any())
                return Json(new { success = false, message = "Ürün listesi boş olamaz." });

            var rid = GetRestaurantId();
            var menu = GetOrCreateMyMenu(rid);
            var myCatIds = _categoryService.TGetListAll().Where(c => c.MenuId == menu.Id).Select(c => c.Id).ToHashSet();
            var myItems = _menuItemService.TGetListAll().Where(mi => myCatIds.Contains(mi.CategoryId)).ToDictionary(mi => mi.Id);

            for (int i = 0; i < itemIds.Count; i++)
            {
                var id = itemIds[i];
                if (myItems.TryGetValue(id, out var item))
                {
                    item.DisplayOrder = i;
                    _menuItemService.TUpdate(item);
                }
            }

            return Json(new { success = true, message = "Ürün sıralaması güncellendi." });
        }

        [HttpPost]
        public IActionResult DeleteCategory(int id)
        {
            var rid = GetRestaurantId();
            var menu = GetOrCreateMyMenu(rid);
            var category = _categoryService.TGetByID(id);

            if (category == null || category.MenuId != menu.Id)
                return Json(new { success = false, message = "Kategori bulunamadı veya yetkisiz işlem." });

            // İlgili ürünleri sil
            var items = _menuItemService.TGetListAll().Where(mi => mi.CategoryId == id).ToList();
            foreach (var item in items)
            {
                _menuItemService.TDelete(item);
            }

            _categoryService.TDelete(category);

            return Json(new { success = true, message = "Kategori ve ürünleri başarıyla silindi." });
        }

        [HttpPost]
        public IActionResult DeleteMenuItem(int id)
        {
            var rid = GetRestaurantId();
            var menu = GetOrCreateMyMenu(rid);
            var myCatIds = _categoryService.TGetListAll().Where(c => c.MenuId == menu.Id).Select(c => c.Id).ToHashSet();
            var item = _menuItemService.TGetByID(id);

            if (item == null || !myCatIds.Contains(item.CategoryId))
                return Json(new { success = false, message = "Ürün bulunamadı veya yetkisiz işlem." });

            _menuItemService.TDelete(item);

            return Json(new { success = true, message = "Ürün başarıyla silindi." });
        }

        [HttpPost]
        public IActionResult SelectTheme(int themeId)
        {
            var rid = GetRestaurantId();
            var restaurant = _restaurantService.TGetByID(rid);
            if (restaurant == null) return Json(new { success = false, message = "Restoran bulunamadı." });

            var theme = _themeService.TGetByID(themeId);
            if (theme == null || !theme.IsActive) return Json(new { success = false, message = "Bu tema şu anda kullanıma kapalıdır." });

            restaurant.ThemeId = themeId;
            _restaurantService.TUpdate(restaurant);

            return Json(new { success = true, themeName = theme.Name });
        }

        [HttpPost]
        public IActionResult AddCategory(string name)
        {
            name = name?.Trim() ?? string.Empty;
            if (name.Length > 100)
                return Json(new { success = false, message = "Kategori adı en fazla 100 karakter olabilir." });

            if (string.IsNullOrWhiteSpace(name))
                return Json(new { success = false, message = "Kategori adı boş olamaz." });

            var rid = GetRestaurantId();
            var restaurant = _restaurantService.TGetByID(rid);
            if (restaurant == null) return Json(new { success = false, message = "Restoran bulunamadı." });

            var menu = GetOrCreateMyMenu(rid);

            // Çift kategori kontrolü
            var exists = _categoryService.TGetListAll()
                .Any(c => c.MenuId == menu.Id && c.Name.Trim().Equals(name.Trim(), StringComparison.OrdinalIgnoreCase));
            
            if (exists)
                return Json(new { success = false, message = "Bu kategori zaten eklenmiş." });

            var currentMaxOrder = _categoryService.TGetListAll()
                .Where(c => c.MenuId == menu.Id)
                .Select(c => (int?)c.DisplayOrder)
                .Max() ?? -1;

            var category = new Category { Name = name, MenuId = menu.Id, DisplayOrder = currentMaxOrder + 1 };
            _categoryService.TInsert(category);

            // Yeni önerileri hesapla
            var categories = _categoryService.TGetListAll().Where(c => c.MenuId == menu.Id).OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id).ToList();
            var existingNames = categories.Select(c => c.Name).ToList();
            var suggestions = _categorySuggestionService.GetSuggestions(existingNames, restaurant.Name);

            return Json(new { 
                success = true, 
                category = new { id = category.Id, name = category.Name, displayOrder = category.DisplayOrder },
                suggestions = suggestions
            });
        }

        [HttpPost]
        public IActionResult AddMenuItem(string name, string description, decimal price, int categoryId)
        {
            name = name?.Trim() ?? string.Empty;
            description = description?.Trim() ?? string.Empty;
            if (name.Length > 150 || description.Length > 1000)
                return Json(new { success = false, message = "Ürün adı veya açıklaması izin verilen uzunluğu aşıyor." });

            if (string.IsNullOrWhiteSpace(name))
                return Json(new { success = false, message = "Ürün adı boş olamaz." });

            if (price < 0 || price > 999999.99m)
                return Json(new { success = false, message = "Fiyat negatif olamaz." });

            var rid = GetRestaurantId();
            var menu = GetOrCreateMyMenu(rid);
            var category = _categoryService.TGetByID(categoryId);

            // Güvenlik kontrolü: Kategori bu restorana mı ait?
            if (category == null || category.MenuId != menu.Id)
                return Json(new { success = false, message = "Geçersiz kategori." });

            var currentMaxOrder = _menuItemService.TGetListAll()
                .Where(mi => mi.CategoryId == categoryId)
                .Select(mi => (int?)mi.DisplayOrder)
                .Max() ?? -1;

            var item = new MenuItem
            {
                Name = name,
                Description = description ?? string.Empty,
                Price = price,
                CategoryId = categoryId,
                DisplayOrder = currentMaxOrder + 1
            };
            _menuItemService.TInsert(item);

            return Json(new { 
                success = true, 
                item = new { id = item.Id, name = item.Name, description = item.Description, price = item.Price, categoryId = item.CategoryId, displayOrder = item.DisplayOrder }
            });
        }

        [HttpPost]
        public IActionResult UpdateLocation(string? googleMapsUrl, string? address, string? phone, string? workingHours, string? instagramUrl)
        {
            var rid = GetRestaurantId();
            var restaurant = _restaurantService.TGetByID(rid);
            if (restaurant == null)
                return Json(new { success = false, message = "Restoran bulunamadı." });

            // Kullanıcı <iframe src="..."> yapıştırdıysa src URL'sini temizle
            if (!string.IsNullOrWhiteSpace(googleMapsUrl) && googleMapsUrl.Contains("<iframe"))
            {
                var match = System.Text.RegularExpressions.Regex.Match(googleMapsUrl, @"src=[""']([^""']+)[""']");
                if (match.Success)
                {
                    googleMapsUrl = match.Groups[1].Value;
                }
            }

            if (!TryNormalizeGoogleMapsUrl(googleMapsUrl, out var normalizedMapsUrl) ||
                (address?.Length ?? 0) > MaxAddressLength ||
                (phone?.Length ?? 0) > MaxPhoneLength ||
                (workingHours?.Length ?? 0) > 200 ||
                (!string.IsNullOrWhiteSpace(phone) && !System.Text.RegularExpressions.Regex.IsMatch(phone, @"^[0-9+()\-\s]{7,25}$")))
            {
                return Json(new { success = false, message = "Konum, adres veya telefon bilgisi geçersiz." });
            }

            // Normalize Instagram URL
            if (!string.IsNullOrWhiteSpace(instagramUrl))
            {
                var cleanIg = instagramUrl.Trim();
                if (!cleanIg.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && 
                    !cleanIg.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    cleanIg = "https://instagram.com/" + cleanIg.TrimStart('@');
                }
                instagramUrl = cleanIg;
            }

            restaurant.GoogleMapsUrl = normalizedMapsUrl;
            restaurant.Address = address?.Trim();
            restaurant.Phone = phone?.Trim();
            restaurant.WorkingHours = string.IsNullOrWhiteSpace(workingHours) ? null : workingHours.Trim();
            restaurant.InstagramUrl = string.IsNullOrWhiteSpace(instagramUrl) ? null : instagramUrl.Trim();

            _restaurantService.TUpdate(restaurant);

            return Json(new { success = true, message = "Konum, saat, telefon ve Instagram bilgileri kaydedildi." });
        }

        [HttpPost]
        public IActionResult UpdateNotice(string? notice)
        {
            var rid = GetRestaurantId();
            var restaurant = _restaurantService.TGetByID(rid);
            if (restaurant == null) return Json(new { success = false, message = "Restoran bulunamadı." });

            notice = notice?.Trim();
            if (notice != null && notice.Length > 1000)
                return Json(new { success = false, message = "Bilgilendirme notu en fazla 1000 karakter olabilir." });

            restaurant.ImportantNotice = string.IsNullOrWhiteSpace(notice) ? null : notice;
            _restaurantService.TUpdate(restaurant);

            return Json(new { success = true, message = "Bilgilendirme notu güncellendi." });
        }

        private static bool TryNormalizeGoogleMapsUrl(string? value, out string? normalizedUrl)
        {
            normalizedUrl = null;
            if (string.IsNullOrWhiteSpace(value))
                return true;

            if (!Uri.TryCreate(value.Trim(), UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
                return false;

            var host = uri.Host.ToLowerInvariant();
            var isGoogleMapsHost = host == "maps.google.com" || host == "www.google.com" ||
                                   host.EndsWith(".google.com", StringComparison.Ordinal);
            if (!isGoogleMapsHost)
                return false;

            normalizedUrl = uri.AbsoluteUri;
            return true;
        }
    }
}
