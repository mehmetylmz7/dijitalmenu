using BusinessLayer.Abstract;
using dijitalmenu.Filters;
using dijitalmenu.Services;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace dijitalmenu.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ServiceFilter(typeof(AdminAuthFilter))]
    public class MenuItemController : Controller
    {
        private readonly IMenuItemService _menuItemService;
        private readonly ICategoryService _categoryService;
        private readonly IAuditContextService _auditContextService;

        public MenuItemController(
            IMenuItemService menuItemService,
            ICategoryService categoryService,
            IAuditContextService auditContextService)
        {
            _menuItemService = menuItemService;
            _categoryService = categoryService;
            _auditContextService = auditContextService;
        }

        public IActionResult Index()
        {
            var list = _menuItemService.TGetListAll();
            ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = _categoryService.TGetListAll();
            ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");
            return View();
        }

        [HttpPost]
        public IActionResult Create(MenuItem menuItem)
        {
            _menuItemService.TInsert(menuItem);

            _auditContextService.Log(
                action: "MENU_ITEM_CREATED",
                entityType: "MenuItem",
                entityId: menuItem.Id,
                description: $"Admin tarafından ürün eklendi: '{menuItem.Name}' ({menuItem.Price:C})",
                newEntity: new { menuItem.Id, menuItem.Name, menuItem.Price, menuItem.CategoryId, menuItem.Description, menuItem.ImageUrl }
            );

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.Categories = _categoryService.TGetListAll();
            ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");
            var menuItem = _menuItemService.TGetByID(id);
            return View(menuItem);
        }

        [HttpPost]
        public IActionResult Edit(MenuItem menuItem)
        {
            var existing = _menuItemService.TGetByID(menuItem.Id);
            if (existing != null)
            {
                var oldValues = new { existing.Id, existing.Name, existing.Price, existing.CategoryId, existing.Description, existing.ImageUrl };

                existing.Name = menuItem.Name;
                existing.Description = menuItem.Description;
                existing.Price = menuItem.Price;
                existing.CategoryId = menuItem.CategoryId;
                existing.ImageUrl = menuItem.ImageUrl;

                _menuItemService.TUpdate(existing);

                var newValues = new { existing.Id, existing.Name, existing.Price, existing.CategoryId, existing.Description, existing.ImageUrl };

                _auditContextService.Log(
                    action: "MENU_ITEM_UPDATED",
                    entityType: "MenuItem",
                    entityId: existing.Id,
                    description: $"Admin tarafından ürün güncellendi: '{existing.Name}'",
                    oldEntity: oldValues,
                    newEntity: newValues
                );
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var menuItem = _menuItemService.TGetByID(id);
            if (menuItem != null)
            {
                var oldValues = new { menuItem.Id, menuItem.Name, menuItem.Price, menuItem.CategoryId };

                _auditContextService.Log(
                    action: "MENU_ITEM_DELETED",
                    entityType: "MenuItem",
                    entityId: menuItem.Id,
                    description: $"Admin tarafından ürün silindi: '{menuItem.Name}'",
                    oldEntity: oldValues
                );

                _menuItemService.TDelete(menuItem);
            }

            return RedirectToAction("Index");
        }
    }
}
