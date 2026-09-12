using BusinessLayer.Abstract;
using dijitalmenu.Filters;
using dijitalmenu.Helpers;
using dijitalmenu.Services;
using dijitalmenu.Models;
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
        private readonly IAuditContextService _auditContextService;

        public UserController(
            IUserService userService,
            IRestaurantService restaurantService,
            IAuditContextService auditContextService)
        {
            _userService = userService;
            _restaurantService = restaurantService;
            _auditContextService = auditContextService;
        }

        public IActionResult Index()
        {
            var list = _userService.TGetListAll();
            ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Restaurants = _restaurantService.TGetListAll();
            ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");
            return View();
        }

        [HttpPost]
        public IActionResult Create(UserViewModel userViewModel)
        {
            var restaurant = _restaurantService.TGetByID(userViewModel.RestaurantId);
            if (restaurant == null)
            {
                ModelState.AddModelError("RestaurantId", "Geçersiz restoran seçildi.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Restaurants = _restaurantService.TGetListAll();
                ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");
                return View();
            }

            var user = new User
            {
                Username = userViewModel.Username,
                RestaurantId = userViewModel.RestaurantId,
                Password = PasswordHelper.Hash(userViewModel.Password)
            };

            _userService.TInsert(user);

            // Audit Log
            var newValues = new { user.Id, user.Username, user.RestaurantId };

            _auditContextService.Log(
                action: "USER_CREATED",
                entityType: "User",
                entityId: user.Id,
                restaurantId: user.RestaurantId,
                description: $"Admin tarafından yeni kullanıcı oluşturuldu: '{user.Username}'",
                newEntity: newValues
            );

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.Restaurants = _restaurantService.TGetListAll();
            ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");
            var user = _userService.TGetByID(id);
            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(UserViewModel userViewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Restaurants = _restaurantService.TGetListAll();
                ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");
                return View();
            }

            var existing = _userService.TGetByID(userViewModel.Id);
            if (existing == null)
                return RedirectToAction("Index");

            var oldValues = new { existing.Id, existing.Username, existing.RestaurantId };

            // Only allow username update; RestaurantId is preserved from existing record
            // to prevent mass assignment privilege escalation. Admin must use dedicated
            // restaurant management workflow to reassign users to different restaurants.
            existing.Username = userViewModel.Username;

            if (!string.IsNullOrWhiteSpace(userViewModel.Password))
                existing.Password = PasswordHelper.Hash(userViewModel.Password);

            _userService.TUpdate(existing);

            var newValues = new { existing.Id, existing.Username, existing.RestaurantId };

            _auditContextService.Log(
                action: "USER_UPDATED",
                entityType: "User",
                entityId: existing.Id,
                restaurantId: existing.RestaurantId,
                description: $"Admin tarafından kullanıcı güncellendi: '{existing.Username}'",
                oldEntity: oldValues,
                newEntity: newValues
            );

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var user = _userService.TGetByID(id);
            if (user != null)
            {
                var oldValues = new { user.Id, user.Username, user.RestaurantId };

                _auditContextService.Log(
                    action: "USER_DELETED",
                    entityType: "User",
                    entityId: user.Id,
                    restaurantId: user.RestaurantId,
                    description: $"Admin tarafından kullanıcı silindi: '{user.Username}'",
                    oldEntity: oldValues
                );

                _userService.TDelete(user);
            }

            return RedirectToAction("Index");
        }
    }
}
