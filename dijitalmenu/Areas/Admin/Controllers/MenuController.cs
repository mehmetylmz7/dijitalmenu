using BusinessLayer.Abstract;
using dijitalmenu.Filters;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace dijitalmenu.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ServiceFilter(typeof(AdminAuthFilter))]
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly IRestaurantService _restaurantService;

        public MenuController(IMenuService menuService, IRestaurantService restaurantService)
        {
            _menuService = menuService;
            _restaurantService = restaurantService;
        }

        public IActionResult Index()
        {
            var list = _menuService.TGetListAll();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Restaurants = _restaurantService.TGetListAll();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Menu menu)
        {
            _menuService.TInsert(menu);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.Restaurants = _restaurantService.TGetListAll();
            var menu = _menuService.TGetByID(id);
            return View(menu);
        }

        [HttpPost]
        public IActionResult Edit(Menu menu)
        {
            _menuService.TUpdate(menu);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var menu = _menuService.TGetByID(id);
            _menuService.TDelete(menu);
            return RedirectToAction("Index");
        }
    }
}
