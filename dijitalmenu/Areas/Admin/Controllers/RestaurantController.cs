using BusinessLayer.Abstract;
using dijitalmenu.Filters;
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
        public IActionResult Create(RestaurantEntity restaurant)
        {
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

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.Themes = _themeService.TGetListAll().Where(t => t.IsActive).ToList();
            ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");
            var restaurant = _restaurantService.TGetByID(id);
            return View(restaurant);
        }

        [HttpPost]
        public IActionResult Edit(RestaurantEntity restaurant)
        {
            var existing = _restaurantService.TGetByID(restaurant.Id);
            if (existing != null)
            {
                var oldValues = new { existing.Id, existing.Name, existing.Slug, existing.ThemeId, existing.Phone, existing.Address };

                existing.Name = restaurant.Name;
                existing.Slug = restaurant.Slug;
                existing.ThemeId = restaurant.ThemeId;
                existing.Phone = restaurant.Phone;
                existing.Address = restaurant.Address;
                existing.GoogleMapsUrl = restaurant.GoogleMapsUrl;
                existing.ImportantNotice = restaurant.ImportantNotice;
                existing.WorkingHours = restaurant.WorkingHours;
                existing.InstagramUrl = restaurant.InstagramUrl;

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

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var restaurant = _restaurantService.TGetByID(id);
            if (restaurant != null)
            {
                var oldValues = new { restaurant.Id, restaurant.Name, restaurant.Slug };

                _auditContextService.Log(
                    action: "RESTAURANT_DELETED",
                    entityType: "Restaurant",
                    entityId: restaurant.Id,
                    restaurantId: restaurant.Id,
                    description: $"Admin tarafından restoran ve ilişkili tüm verileri silindi: '{restaurant.Name}'",
                    oldEntity: oldValues
                );

                _notificationService.CreateNotification(
                    title: "Kritik İşlem: Restoran Silindi",
                    message: $"'{restaurant.Name}' adlı restoran ve ilişkili tüm menü/ürün kayıtları silindi.",
                    type: "Warning"
                );

                _restaurantService.TDelete(restaurant);
            }

            return RedirectToAction("Index");
        }
    }
}
