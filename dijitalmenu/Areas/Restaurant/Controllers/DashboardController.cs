using BusinessLayer.Abstract;
using dijitalmenu.Filters;
using Microsoft.AspNetCore.Mvc;

namespace dijitalmenu.Areas.Restaurant.Controllers
{
    [Area("Restaurant")]
    [ServiceFilter(typeof(RestaurantAuthFilter))]
    public class DashboardController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IMenuItemService _menuItemService;
        private readonly IMenuService _menuService;
        private readonly IRestaurantService _restaurantService;

        public DashboardController(ICategoryService categoryService, IMenuItemService menuItemService,
            IMenuService menuService, IRestaurantService restaurantService)
        {
            _categoryService = categoryService;
            _menuItemService = menuItemService;
            _menuService = menuService;
            _restaurantService = restaurantService;
        }

        public IActionResult Index()
        {
            var restaurantId = int.Parse(HttpContext.Session.GetString("RestaurantId")!);
            var restaurant = _restaurantService.TGetByID(restaurantId);
            var menu = _menuService.TGetListAll().FirstOrDefault(m => m.RestaurantId == restaurantId);

            int catCount = 0, itemCount = 0;
            if (menu != null)
            {
                var cats = _categoryService.TGetListAll().Where(c => c.MenuId == menu.Id).ToList();
                catCount = cats.Count;
                var catIds = cats.Select(c => c.Id).ToHashSet();
                itemCount = _menuItemService.TGetListAll().Count(mi => catIds.Contains(mi.CategoryId));
            }

            ViewBag.RestaurantName = restaurant?.Name;
            ViewBag.RestaurantUsername = HttpContext.Session.GetString("RestaurantUsername");
            ViewBag.CategoryCount = catCount;
            ViewBag.MenuItemCount = itemCount;

            return View();
        }
    }
}
