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
            var categories = _categoryService.TGetListAll().Where(c => c.MenuId == menu.Id).ToList();
            var catIds = categories.Select(c => c.Id).ToHashSet();
            var menuItems = _menuItemService.TGetListAll().Where(mi => catIds.Contains(mi.CategoryId)).ToList();

            var existingNames = categories.Select(c => c.Name).ToList();
            var suggestions = _categorySuggestionService.GetSuggestions(existingNames, restaurant.Name);

            ViewBag.Restaurant = restaurant;
            ViewBag.Categories = categories;
            ViewBag.MenuItems = menuItems;
            ViewBag.Themes = _themeService.TGetListAll();
            ViewBag.Suggestions = suggestions;
            ViewBag.RestaurantUsername = HttpContext.Session.GetString("RestaurantUsername");

            return View();
        }

        [HttpPost]
        public IActionResult SelectTheme(int themeId)
        {
            var rid = GetRestaurantId();
            var restaurant = _restaurantService.TGetByID(rid);
            if (restaurant == null) return Json(new { success = false, message = "Restoran bulunamadı." });

            var theme = _themeService.TGetByID(themeId);
            if (theme == null) return Json(new { success = false, message = "Tema bulunamadı." });

            restaurant.ThemeId = themeId;
            _restaurantService.TUpdate(restaurant);

            return Json(new { success = true, themeName = theme.Name });
        }

        [HttpPost]
        public IActionResult AddCategory(string name)
        {
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

            var category = new Category { Name = name, MenuId = menu.Id };
            _categoryService.TInsert(category);

            // Yeni önerileri hesapla
            var categories = _categoryService.TGetListAll().Where(c => c.MenuId == menu.Id).ToList();
            var existingNames = categories.Select(c => c.Name).ToList();
            var suggestions = _categorySuggestionService.GetSuggestions(existingNames, restaurant.Name);

            return Json(new { 
                success = true, 
                category = new { id = category.Id, name = category.Name },
                suggestions = suggestions
            });
        }

        [HttpPost]
        public IActionResult AddMenuItem(string name, string description, decimal price, int categoryId)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Json(new { success = false, message = "Ürün adı boş olamaz." });

            if (price < 0)
                return Json(new { success = false, message = "Fiyat negatif olamaz." });

            var rid = GetRestaurantId();
            var menu = GetOrCreateMyMenu(rid);
            var category = _categoryService.TGetByID(categoryId);

            // Güvenlik kontrolü: Kategori bu restorana mı ait?
            if (category == null || category.MenuId != menu.Id)
                return Json(new { success = false, message = "Geçersiz kategori." });

            var item = new MenuItem
            {
                Name = name,
                Description = description ?? string.Empty,
                Price = price,
                CategoryId = categoryId
            };
            _menuItemService.TInsert(item);

            return Json(new { 
                success = true, 
                item = new { id = item.Id, name = item.Name, description = item.Description, price = item.Price, categoryId = item.CategoryId }
            });
        }
    }
}
