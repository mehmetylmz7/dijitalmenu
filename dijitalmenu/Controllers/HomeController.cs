using System.Diagnostics;
using BusinessLayer.Abstract;
using Microsoft.AspNetCore.Mvc;
using dijitalmenu.Models;

namespace dijitalmenu.Controllers;

public class HomeController : Controller
{
    private readonly IRestaurantService _restaurantService;
    private readonly IMenuService _menuService;
    private readonly ICategoryService _categoryService;
    private readonly IMenuItemService _menuItemService;

    public HomeController(IRestaurantService restaurantService, IMenuService menuService,
        ICategoryService categoryService, IMenuItemService menuItemService)
    {
        _restaurantService = restaurantService;
        _menuService = menuService;
        _categoryService = categoryService;
        _menuItemService = menuItemService;
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

        var menu = _menuService.TGetListAll().FirstOrDefault(m => m.RestaurantId == id);
        if (menu == null)
        {
            ViewBag.Restaurant = restaurant;
            ViewBag.Categories = new List<EntityLayer.Concrete.Category>();
            ViewBag.MenuItems = new List<EntityLayer.Concrete.MenuItem>();
            return View();
        }

        var categories = _categoryService.TGetListAll()
            .Where(c => c.MenuId == menu.Id).ToList();

        var categoryIds = categories.Select(c => c.Id).ToHashSet();
        var menuItems = _menuItemService.TGetListAll()
            .Where(mi => categoryIds.Contains(mi.CategoryId)).ToList();

        ViewBag.Restaurant = restaurant;
        ViewBag.Categories = categories;
        ViewBag.MenuItems = menuItems;

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

