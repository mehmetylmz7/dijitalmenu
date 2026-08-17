using BusinessLayer.Abstract;
using dijitalmenu.Filters;
using dijitalmenu.Helpers;
using dijitalmenu.Services;
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
        public IActionResult Create(User user)
        {
            user.Password = PasswordHelper.Hash(user.Password);
            _userService.TInsert(user);

            // Audit Log (Passwords are masked/excluded)
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
        public IActionResult Edit(User user)
        {
            var existing = _userService.TGetByID(user.Id);
            if (existing == null)
                return RedirectToAction("Index");

            var oldValues = new { existing.Id, existing.Username, existing.RestaurantId };

            existing.Username = user.Username;
            existing.RestaurantId = user.RestaurantId;

            if (!string.IsNullOrWhiteSpace(user.Password))
                existing.Password = PasswordHelper.Hash(user.Password);

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
