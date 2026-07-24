using System.Diagnostics;
using System.Text.Json;
using BusinessLayer.Abstract;
using Microsoft.AspNetCore.Hosting;
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
    private readonly IWebHostEnvironment _env;

    public HomeController(IRestaurantService restaurantService, IMenuService menuService,
        ICategoryService categoryService, IMenuItemService menuItemService,
        IThemeService themeService, IWebHostEnvironment env)
    {
        _restaurantService = restaurantService;
        _menuService       = menuService;
        _categoryService   = categoryService;
        _menuItemService   = menuItemService;
        _themeService      = themeService;
        _env               = env;
    }

    // GET: / — Landing page with demo themes
    public IActionResult Index()
    {
        var themes = _themeService.TGetListAll().ToList();
        ViewBag.Themes = themes;
        return View();
    }

    // GET: /Home/Preview?themeId=1 — Canlı tema önizleme ve cihaz simülatörü
    public IActionResult Preview(int themeId = 1)
    {
        var themes = _themeService.TGetListAll().ToList();
        ViewBag.Themes = themes;
        ViewBag.SelectedThemeId = themeId;
        return View();
    }

    // GET: /Home/Demo?themeId=1 — Statik JSON menü ile tema önizleme
    public IActionResult Demo(int themeId = 1)
    {
        // Temayı DB'den al
        var theme = _themeService.TGetByID(themeId)
                    ?? _themeService.TGetListAll().FirstOrDefault();
        ViewBag.Theme = theme;

        // JSON dosyasını oku
        var jsonPath = Path.Combine(_env.WebRootPath, "demo", "menu.json");
        if (!System.IO.File.Exists(jsonPath))
            return NotFound("Demo menü dosyası bulunamadı.");

        var jsonText = System.IO.File.ReadAllText(jsonPath);
        using var doc  = JsonDocument.Parse(jsonText);
        var root        = doc.RootElement;

        // Fake Restaurant
        var restaurant = new EntityLayer.Concrete.Restaurant
        {
            Id   = 0,
            Name = root.GetProperty("restaurantName").GetString() ?? "Demo Restoran"
        };
        ViewBag.Restaurant = restaurant;

        // Fake Categories & MenuItems
        var categories = new List<EntityLayer.Concrete.Category>();
        var menuItems  = new List<EntityLayer.Concrete.MenuItem>();

        int catId  = 1;
        int itemId = 1;

        foreach (var catEl in root.GetProperty("categories").EnumerateArray())
        {
            var cat = new EntityLayer.Concrete.Category
            {
                Id     = catId,
                Name   = catEl.GetProperty("name").GetString() ?? "",
                MenuId = 0
            };
            categories.Add(cat);

            foreach (var itemEl in catEl.GetProperty("items").EnumerateArray())
            {
                menuItems.Add(new EntityLayer.Concrete.MenuItem
                {
                    Id          = itemId++,
                    CategoryId  = catId,
                    Name        = itemEl.GetProperty("name").GetString() ?? "",
                    Description = itemEl.GetProperty("description").GetString() ?? "",
                    Price       = itemEl.GetProperty("price").GetDecimal()
                });
            }

            catId++;
        }

        ViewBag.Categories = categories;
        ViewBag.MenuItems  = menuItems;

        return View("Menu");
    }

    // GET: /Home/Menu/1 — Restoranın dijital menüsü
    public IActionResult Menu(int id, int? previewThemeId = null)
    {
        var restaurant = _restaurantService.TGetByID(id);
        if (restaurant == null) return NotFound();

        var menu  = _menuService.TGetListAll().FirstOrDefault(m => m.RestaurantId == id);
        
        EntityLayer.Concrete.Theme? theme = null;
        if (previewThemeId.HasValue)
        {
            theme = _themeService.TGetByID(previewThemeId.Value);
        }
        
        if (theme == null)
        {
            theme = _themeService.TGetByID(restaurant.ThemeId);
        }

        if (theme == null)
        {
            theme = _themeService.TGetListAll().FirstOrDefault();
        }

        ViewBag.Theme = theme;

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
