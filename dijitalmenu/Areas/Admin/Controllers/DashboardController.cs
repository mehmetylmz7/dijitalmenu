using BusinessLayer.Abstract;
using dijitalmenu.Filters;
using Microsoft.AspNetCore.Mvc;

namespace dijitalmenu.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ServiceFilter(typeof(AdminAuthFilter))]
    public class DashboardController : Controller
    {
        private readonly IRestaurantService _restaurantService;
        private readonly IMenuService _menuService;
        private readonly ICategoryService _categoryService;
        private readonly IMenuItemService _menuItemService;

        public DashboardController(IRestaurantService restaurantService, IMenuService menuService,
            ICategoryService categoryService, IMenuItemService menuItemService)
        {
            _restaurantService = restaurantService;
            _menuService = menuService;
            _categoryService = categoryService;
            _menuItemService = menuItemService;
        }

        public IActionResult Index()
        {
            ViewBag.RestaurantCount = _restaurantService.TGetListAll().Count;
            ViewBag.MenuCount = _menuService.TGetListAll().Count;
            ViewBag.CategoryCount = _categoryService.TGetListAll().Count;
            ViewBag.MenuItemCount = _menuItemService.TGetListAll().Count;
            ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");
            return View();
        }
    }
}
