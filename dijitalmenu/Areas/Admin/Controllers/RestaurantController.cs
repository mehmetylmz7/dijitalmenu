using BusinessLayer.Abstract;
using dijitalmenu.Filters;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace dijitalmenu.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ServiceFilter(typeof(AdminAuthFilter))]
    public class RestaurantController : Controller
    {
        private readonly IRestaurantService _restaurantService;
        private readonly IThemeService _themeService;

        public RestaurantController(IRestaurantService restaurantService, IThemeService themeService)
        {
            _restaurantService = restaurantService;
            _themeService = themeService;
        }

        public IActionResult Index()
        {
            var list = _restaurantService.TGetListAll();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Themes = _themeService.TGetListAll();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Restaurant restaurant)
        {
            _restaurantService.TInsert(restaurant);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.Themes = _themeService.TGetListAll();
            var restaurant = _restaurantService.TGetByID(id);
            return View(restaurant);
        }

        [HttpPost]
        public IActionResult Edit(Restaurant restaurant)
        {
            _restaurantService.TUpdate(restaurant);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var restaurant = _restaurantService.TGetByID(id);
            _restaurantService.TDelete(restaurant);
            return RedirectToAction("Index");
        }
    }
}
