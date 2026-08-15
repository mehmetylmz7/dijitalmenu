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
    private readonly IConfiguration     _configuration;

    public HomeController(IRestaurantService restaurantService, IMenuService menuService,
        ICategoryService categoryService, IMenuItemService menuItemService,
        IThemeService themeService, IWebHostEnvironment env, IConfiguration configuration)
    {
        _restaurantService = restaurantService;
        _menuService       = menuService;
        _categoryService   = categoryService;
        _menuItemService   = menuItemService;
        _themeService      = themeService;
        _env               = env;
        _configuration     = configuration;
    }

    // GET: / — Landing page with demo themes
    public IActionResult Index()
    {
        var themes = _themeService.TGetListAll().Where(t => t.IsActive).OrderBy(t => t.Id).ToList();
        ViewBag.Themes = themes;
        return View();
    }

    // GET: /Home/Preview?themeId=1 — Canlı tema önizleme ve cihaz simülatörü
    public IActionResult Preview(int themeId = 1)
    {
        var themes = _themeService.TGetListAll().Where(t => t.IsActive).OrderBy(t => t.Id).ToList();
        var selectedTheme = themes.FirstOrDefault(t => t.Id == themeId) ?? themes.FirstOrDefault();
        ViewBag.Themes = themes;
        ViewBag.SelectedThemeId = selectedTheme?.Id ?? themeId;
        return View();
    }

    // GET: /Home/Demo?themeId=1 — Statik JSON menü ile tema önizleme
    public IActionResult Demo(int themeId = 1)
    {
        // Temayı DB'den al (yalnızca aktif temalar)
        var theme = _themeService.TGetByID(themeId);
        if (theme == null || !theme.IsActive)
        {
            theme = _themeService.TGetListAll().FirstOrDefault(t => t.IsActive);
        }
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

    // GET: /Menu/{slug} — Restoranın dijital menüsü
    public IActionResult Menu(string id, int? previewThemeId = null)
    {
        // id parametresi routing'den geliyor ama aslında slug'ı temsil ediyor
        var restaurant = _restaurantService.TGetListAll().FirstOrDefault(r => r.Slug == id);
        
        // Geriye dönük uyumluluk (eğer ID girilmişse)
        if (restaurant == null && int.TryParse(id, out int numericId))
        {
            restaurant = _restaurantService.TGetByID(numericId);
        }

        if (restaurant == null) return NotFound();

        var menu  = _menuService.TGetListAll().FirstOrDefault(m => m.RestaurantId == restaurant.Id);
        
        EntityLayer.Concrete.Theme? theme = null;
        if (previewThemeId.HasValue)
        {
            var pt = _themeService.TGetByID(previewThemeId.Value);
            if (pt != null && pt.IsActive)
            {
                theme = pt;
            }
        }
        
        if (theme == null)
        {
            var restTheme = _themeService.TGetByID(restaurant.ThemeId);
            if (restTheme != null && restTheme.IsActive)
            {
                theme = restTheme;
            }
        }

        if (theme == null)
        {
            theme = _themeService.TGetListAll().FirstOrDefault(t => t.IsActive);
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

        string appUrl = _configuration["AppUrl"];
        string baseUrl = !string.IsNullOrWhiteSpace(appUrl) ? appUrl : $"{Request.Scheme}://{Request.Host}";
        string qrUrl = $"{baseUrl.TrimEnd('/')}/Menu/{restaurant.Slug}";

        ViewBag.Restaurant = restaurant;
        ViewBag.Categories = cats;
        ViewBag.MenuItems  = items;
        ViewBag.MenuUrl    = qrUrl;
        ViewBag.QrCodeUrl  = $"{baseUrl.TrimEnd('/')}/Menu/{restaurant.Slug}/qr";

        return View();
    }

    // GET: /Menu/{id}/qr or /QR/{id} — Public QR Code PNG Generator
    [HttpGet("Menu/{id}/qr")]
    [HttpGet("QR/{id}")]
    public IActionResult GetQrCode(string id)
    {
        var restaurant = _restaurantService.TGetListAll().FirstOrDefault(r => r.Slug == id);
        if (restaurant == null && int.TryParse(id, out int numericId))
        {
            restaurant = _restaurantService.TGetByID(numericId);
        }

        if (restaurant == null) return NotFound();

        string appUrl = _configuration["AppUrl"];
        string baseUrl = !string.IsNullOrWhiteSpace(appUrl) ? appUrl : $"{Request.Scheme}://{Request.Host}";
        string qrUrl = $"{baseUrl.TrimEnd('/')}/Menu/{restaurant.Slug}";

        using var qrGenerator = new QRCoder.QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(qrUrl, QRCoder.QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new QRCoder.PngByteQRCode(qrCodeData);
        byte[] qrCodeBytes = qrCode.GetGraphic(20);

        return File(qrCodeBytes, "image/png");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
