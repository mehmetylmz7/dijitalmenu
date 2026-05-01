using BusinessLayer.Abstract;
using dijitalmenu.Filters;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace dijitalmenu.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ServiceFilter(typeof(AdminAuthFilter))]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRestaurantService _restaurantService;

        public UserController(IUserService userService, IRestaurantService restaurantService)
        {
            _userService = userService;
            _restaurantService = restaurantService;
        }

        public IActionResult Index()
        {
            var list = _userService.TGetListAll();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Restaurants = _restaurantService.TGetListAll();
            return View();
        }

        [HttpPost]
        public IActionResult Create(User user)
        {
            _userService.TInsert(user);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.Restaurants = _restaurantService.TGetListAll();
            var user = _userService.TGetByID(id);
            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(User user)
        {
            _userService.TUpdate(user);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var user = _userService.TGetByID(id);
            _userService.TDelete(user);
            return RedirectToAction("Index");
        }
    }
}
