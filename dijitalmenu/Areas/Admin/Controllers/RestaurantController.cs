using BusinessLayer.Abstract;
using dijitalmenu.Filters;
using dijitalmenu.Helpers;
using dijitalmenu.Models;
using dijitalmenu.Services;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using RestaurantEntity = EntityLayer.Concrete.Restaurant;

namespace dijitalmenu.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ServiceFilter(typeof(AdminAuthFilter))]
    public class RestaurantController : Controller
    {
        private readonly IRestaurantService _restaurantService;
        private readonly IThemeService _themeService;
        private readonly IAuditContextService _auditContextService;
        private readonly INotificationService _notificationService;

        public RestaurantController(
            IRestaurantService restaurantService,
            IThemeService themeService,
            IAuditContextService auditContextService,
            INotificationService notificationService)
        {
            _restaurantService = restaurantService;
            _themeService = themeService;
            _auditContextService = auditContextService;
            _notificationService = notificationService;
        }

        public IActionResult Index()
        {
            var list = _restaurantService.TGetListAll();
            ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Themes = _themeService.TGetListAll().Where(t => t.IsActive).ToList();
            ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        public IActionResult Create(RestaurantInputModel model)
        {
            var theme = _themeService.TGetByID(model.ThemeId);
            if (theme == null)
            {
                ModelState.AddModelError("ThemeId", "Geçersiz tema seçildi.");
                ViewBag.Themes = _themeService.TGetListAll().Where(t => t.IsActive).ToList();
                ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");
                return View();
            }

            var slug = !string.IsNullOrWhiteSpace(model.Slug)
                ? StringHelper.GenerateSlug(model.Slug)
                : StringHelper.GenerateSlug(model.Name);

            var restaurant = new RestaurantEntity
            {
                Name = model.Name.Trim(),
                Slug = slug,
                ThemeId = model.ThemeId,
                Phone = model.Phone?.Trim(),
                Address = model.Address?.Trim(),
                GoogleMapsUrl = model.GoogleMapsUrl?.Trim(),
                ImportantNotice = model.ImportantNotice?.Trim(),
                WorkingHours = model.WorkingHours?.Trim(),
                InstagramUrl = model.InstagramUrl?.Trim()
            };

            _restaurantService.TInsert(restaurant);

            _auditContextService.Log(
                action: "RESTAURANT_CREATED",
                entityType: "Restaurant",
                entityId: restaurant.Id,
                restaurantId: restaurant.Id,
                description: $"Admin tarafından yeni restoran eklendi: '{restaurant.Name}'",
                newEntity: new { restaurant.Id, restaurant.Name, restaurant.Slug, restaurant.ThemeId, restaurant.Phone, restaurant.Address }
            );

            _notificationService.CreateNotification(
                title: "Yeni Restoran Oluşturuldu",
                message: $"Admin paneli üzerinden '{restaurant.Name}' adlı yeni restoran oluşturuldu.",
                type: "Info",
                restaurantId: restaurant.Id
            );

            return RedirectToAction("Index");
        }

        [NonAction]
        public IActionResult Create(RestaurantEntity restaurant) =>
            Create(new RestaurantInputModel
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Slug = restaurant.Slug,
                ThemeId = restaurant.ThemeId,
                Phone = restaurant.Phone,
                Address = restaurant.Address,
                GoogleMapsUrl = restaurant.GoogleMapsUrl,
                ImportantNotice = restaurant.ImportantNotice,
                WorkingHours = restaurant.WorkingHours,
                InstagramUrl = restaurant.InstagramUrl
            });

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.Themes = _themeService.TGetListAll().Where(t => t.IsActive).ToList();
            ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");
            var restaurant = _restaurantService.TGetByID(id);
            return View(restaurant);
        }

        [HttpPost]
        [ActionName("Edit")]
        public IActionResult Edit(RestaurantInputModel model)
        {
            var existing = _restaurantService.TGetByID(model.Id);
            if (existing != null)
            {
                var theme = _themeService.TGetByID(model.ThemeId);
                if (theme == null)
                {
                    ModelState.AddModelError("ThemeId", "Geçersiz tema seçildi.");
                    ViewBag.Themes = _themeService.TGetListAll().Where(t => t.IsActive).ToList();
                    ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");
                    return View(existing);
                }

                var oldValues = new { existing.Id, existing.Name, existing.Slug, existing.ThemeId, existing.Phone, existing.Address };

                existing.Name = model.Name.Trim();
                if (!string.IsNullOrWhiteSpace(model.Slug))
                {
                    existing.Slug = StringHelper.GenerateSlug(model.Slug);
                }
                existing.ThemeId = model.ThemeId;
                existing.Phone = model.Phone?.Trim();
                existing.Address = model.Address?.Trim();
                existing.GoogleMapsUrl = model.GoogleMapsUrl?.Trim();
                existing.ImportantNotice = model.ImportantNotice?.Trim();
                existing.WorkingHours = model.WorkingHours?.Trim();
                existing.InstagramUrl = model.InstagramUrl?.Trim();

                _restaurantService.TUpdate(existing);

                var newValues = new { existing.Id, existing.Name, existing.Slug, existing.ThemeId, existing.Phone, existing.Address };

                _auditContextService.Log(
                    action: "RESTAURANT_UPDATED",
                    entityType: "Restaurant",
                    entityId: existing.Id,
                    restaurantId: existing.Id,
                    description: $"Admin tarafından restoran güncellendi: '{existing.Name}'",
                    oldEntity: oldValues,
                    newEntity: newValues
                );
            }

            return RedirectToAction("Index");
        }

        [NonAction]
        public IActionResult Edit(RestaurantEntity restaurant) =>
            Edit(new RestaurantInputModel
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Slug = restaurant.Slug,
                ThemeId = restaurant.ThemeId,
                Phone = restaurant.Phone,
                Address = restaurant.Address,
                GoogleMapsUrl = restaurant.GoogleMapsUrl,
                ImportantNotice = restaurant.ImportantNotice,
                WorkingHours = restaurant.WorkingHours,
                InstagramUrl = restaurant.InstagramUrl
            });

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var restaurant = _restaurantService.TGetByID(id);
            if (restaurant != null)
            {
                _auditContextService.Log(
                    action: "RESTAURANT_DELETED",
                    entityType: "Restaurant",
                    entityId: restaurant.Id,
                    restaurantId: restaurant.Id,
                    description: $"Admin tarafından restoran silindi: '{restaurant.Name}'",
                    oldEntity: new { restaurant.Id, restaurant.Name, restaurant.Slug }
                );

                _restaurantService.TDelete(restaurant);
            }

            return RedirectToAction("Index");
        }
    }
}
