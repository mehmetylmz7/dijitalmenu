using System.Diagnostics;
using BusinessLayer.Abstract;
using Microsoft.AspNetCore.Mvc;
using dijitalmenu.Models;

namespace dijitalmenu.Controllers;

public class HomeController : Controller
{
    private readonly IRestaurantService _restaurantService;
    private readonly IMenuService       _menuService;
    private readonly ICategoryService   _categoryService;
    private readonly IMenuItemService   _menuItemService;
    private readonly IThemeService      _themeService;

    public HomeController(IRestaurantService restaurantService, IMenuService menuService,
        ICategoryService categoryService, IMenuItemService menuItemService,
        IThemeService themeService)
    {
        _restaurantService = restaurantService;
        _menuService       = menuService;
        _categoryService   = categoryService;
        _menuItemService   = menuItemService;
        _themeService      = themeService;
    }

    // GET: / — Restoran listesi
    public IActionResult Index()
    {
        var restaurants = _restaurantService.TGetListAll();
        return View(restaurants);
    }

    // GET: /Home/Menu/1 — Restoranın dijital menüsü
    public IActionResult Menu(int id)
    {
        var restaurant = _restaurantService.TGetByID(id);
        if (restaurant == null) return NotFound();

        var menu  = _menuService.TGetListAll().FirstOrDefault(m => m.RestaurantId == id);
        var theme = _themeService.TGetByID(restaurant.ThemeId);

        // ── Kocaoğlu Klasik tema ─────────────────────────────────────────
        if (theme?.Name == "Kocaoğlu Klasik")
        {
            var categories = menu == null
                ? new List<EntityLayer.Concrete.Category>()
                : _categoryService.TGetListAll().Where(c => c.MenuId == menu.Id).ToList();

            var catIds   = categories.Select(c => c.Id).ToHashSet();
            var rawItems = _menuItemService.TGetListAll()
                .Where(mi => catIds.Contains(mi.CategoryId)).ToList();

            var vmItems = rawItems.Select(mi => new MenuItemVM
            {
                Id          = mi.Id.ToString(),
                Name        = mi.Name,
                Description = mi.Description ?? "",
                Price       = mi.Price.ToString("N2") + " ₺",
                Category    = categories.FirstOrDefault(c => c.Id == mi.CategoryId)?.Name ?? "",
                // ImageUrl entity'sinde henüz yok → genel placeholder
                ImageUrl    = "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&q=80&w=400",
                IsSpecial   = false
            }).ToList();

            ViewData["RestaurantName"]   = restaurant.Name;
            ViewData["RestaurantSlogan"] = "Dijital Menü";

            return View("~/Views/Themes/RestaurantMenu/Index.cshtml",
                new RestaurantMenuViewModel { MenuItems = vmItems });
        }

        // ── Varsayılan tema ───────────────────────────────────────────────
        if (menu == null)
        {
            ViewBag.Restaurant = restaurant;
            ViewBag.Categories = new List<EntityLayer.Concrete.Category>();
            ViewBag.MenuItems  = new List<EntityLayer.Concrete.MenuItem>();
            return View();
        }

        var cats    = _categoryService.TGetListAll().Where(c => c.MenuId == menu.Id).ToList();
        var catIds2 = cats.Select(c => c.Id).ToHashSet();
        var items   = _menuItemService.TGetListAll()
                        .Where(mi => catIds2.Contains(mi.CategoryId)).ToList();

        ViewBag.Restaurant = restaurant;
        ViewBag.Categories = cats;
        ViewBag.MenuItems  = items;
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
