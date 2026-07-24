using BusinessLayer.Abstract;
using dijitalmenu.Filters;
using dijitalmenu.Helpers;
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
            user.Password = PasswordHelper.Hash(user.Password);
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
            var existing = _userService.TGetByID(user.Id);
            if (existing == null)
                return RedirectToAction("Index");

            existing.Username = user.Username;
            existing.RestaurantId = user.RestaurantId;

            if (!string.IsNullOrWhiteSpace(user.Password))
                existing.Password = PasswordHelper.Hash(user.Password);

            _userService.TUpdate(existing);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var user = _userService.TGetByID(id);
            if (user != null)
                _userService.TDelete(user);
            return RedirectToAction("Index");
        }
    }
}
