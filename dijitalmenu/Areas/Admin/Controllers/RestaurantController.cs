using BusinessLayer.Abstract;
using dijitalmenu.Filters;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using RestaurantEntity = EntityLayer.Concrete.Restaurant;


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
            ViewBag.Themes = _themeService.TGetListAll().Where(t => t.IsActive).ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(RestaurantEntity restaurant)
        {
            _restaurantService.TInsert(restaurant);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.Themes = _themeService.TGetListAll().Where(t => t.IsActive).ToList();
            var restaurant = _restaurantService.TGetByID(id);
            return View(restaurant);
        }

        [HttpPost]
        public IActionResult Edit(RestaurantEntity restaurant)
        {
            _restaurantService.TUpdate(restaurant);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var restaurant = _restaurantService.TGetByID(id);
            if (restaurant != null)
                _restaurantService.TDelete(restaurant);
            return RedirectToAction("Index");
        }
    }
}
