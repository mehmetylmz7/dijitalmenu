using BusinessLayer.Abstract;
using dijitalmenu.Filters;
using dijitalmenu.Services;
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
        private readonly IAuditContextService _auditContextService;

        public MenuController(
            IMenuService menuService,
            IRestaurantService restaurantService,
            IAuditContextService auditContextService)
        {
            _menuService = menuService;
            _restaurantService = restaurantService;
            _auditContextService = auditContextService;
        }

        public IActionResult Index()
        {
            var list = _menuService.TGetListAll();
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
        public IActionResult Create(Menu menu)
        {
            _menuService.TInsert(menu);

            _auditContextService.Log(
                action: "MENU_CREATED",
                entityType: "Menu",
                entityId: menu.Id,
                restaurantId: menu.RestaurantId,
                description: $"Admin tarafından menü oluşturuldu (Restoran #{menu.RestaurantId})",
                newEntity: new { menu.Id, menu.RestaurantId }
            );

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.Restaurants = _restaurantService.TGetListAll();
            ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");
            var menu = _menuService.TGetByID(id);
            return View(menu);
        }

        [HttpPost]
        public IActionResult Edit(Menu menu)
        {
            var existing = _menuService.TGetByID(menu.Id);
            if (existing != null)
            {
                var oldValues = new { existing.Id, existing.RestaurantId };
                existing.RestaurantId = menu.RestaurantId;
                _menuService.TUpdate(existing);
                var newValues = new { existing.Id, existing.RestaurantId };

                _auditContextService.Log(
                    action: "MENU_UPDATED",
                    entityType: "Menu",
                    entityId: existing.Id,
                    restaurantId: existing.RestaurantId,
                    description: $"Admin tarafından menü güncellendi #{existing.Id}",
                    oldEntity: oldValues,
                    newEntity: newValues
                );
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var menu = _menuService.TGetByID(id);
            if (menu != null)
            {
                _auditContextService.Log(
                    action: "MENU_DELETED",
                    entityType: "Menu",
                    entityId: menu.Id,
                    restaurantId: menu.RestaurantId,
                    description: $"Admin tarafından menü silindi #{menu.Id}",
                    oldEntity: new { menu.Id, menu.RestaurantId }
                );

                _menuService.TDelete(menu);
            }

            return RedirectToAction("Index");
        }
    }
}
