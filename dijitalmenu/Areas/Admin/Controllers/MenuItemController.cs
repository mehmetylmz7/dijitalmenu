using BusinessLayer.Abstract;
using dijitalmenu.Filters;
using dijitalmenu.Models;
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
            ViewBag.Allergens = dijitalmenu.Helpers.AllergenHelper.DefaultAllergens;
            ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        public IActionResult Create(MenuItemInputModel model)
        {
            var category = _categoryService.TGetByID(model.CategoryId);
            if (category == null)
            {
                ModelState.AddModelError("CategoryId", "Geçersiz kategori seçildi.");
                ViewBag.Categories = _categoryService.TGetListAll();
                ViewBag.Allergens = dijitalmenu.Helpers.AllergenHelper.DefaultAllergens;
                ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");
                return View();
            }

            var allergens = dijitalmenu.Helpers.AllergenHelper.FormatAllergens(model.SelectedAllergens) ?? model.Allergens?.Trim();

            var menuItem = new MenuItem
            {
                Name = model.Name.Trim(),
                Price = model.Price,
                CategoryId = model.CategoryId,
                Description = model.Description?.Trim() ?? string.Empty,
                ImageUrl = model.ImageUrl?.Trim(),
                Calories = model.Calories,
                Allergens = allergens
            };

            _menuItemService.TInsert(menuItem);

            _auditContextService.Log(
                action: "MENU_ITEM_CREATED",
                entityType: "MenuItem",
                entityId: menuItem.Id,
                description: $"Admin tarafından ürün eklendi: '{menuItem.Name}' ({menuItem.Price:C})",
                newEntity: new { menuItem.Id, menuItem.Name, menuItem.Price, menuItem.CategoryId, menuItem.Description, menuItem.ImageUrl, menuItem.Calories, menuItem.Allergens }
            );

            return RedirectToAction("Index");
        }

        [NonAction]
        public IActionResult Create(MenuItem menuItem) =>
            Create(new MenuItemInputModel
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Price = menuItem.Price,
                CategoryId = menuItem.CategoryId,
                Description = menuItem.Description,
                ImageUrl = menuItem.ImageUrl,
                Calories = menuItem.Calories,
                Allergens = menuItem.Allergens
            });

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var menuItem = _menuItemService.TGetByID(id);
            if (menuItem == null)
                return NotFound();

            ViewBag.Categories = _categoryService.TGetListAll();
            ViewBag.Allergens = dijitalmenu.Helpers.AllergenHelper.DefaultAllergens;
            ViewBag.SelectedAllergens = dijitalmenu.Helpers.AllergenHelper.ParseAllergens(menuItem.Allergens);
            ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");
            return View(menuItem);
        }

        [HttpPost]
        [ActionName("Edit")]
        public IActionResult Edit(MenuItemInputModel model)
        {
            var existing = _menuItemService.TGetByID(model.Id);
            if (existing != null)
            {
                var category = _categoryService.TGetByID(model.CategoryId);
                if (category == null)
                {
                    ModelState.AddModelError("CategoryId", "Geçersiz kategori seçildi.");
                    ViewBag.Categories = _categoryService.TGetListAll();
                    ViewBag.Allergens = dijitalmenu.Helpers.AllergenHelper.DefaultAllergens;
                    ViewBag.SelectedAllergens = dijitalmenu.Helpers.AllergenHelper.ParseAllergens(existing.Allergens);
                    ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");
                    return View(existing);
                }

                var allergens = dijitalmenu.Helpers.AllergenHelper.FormatAllergens(model.SelectedAllergens) ?? model.Allergens?.Trim();

                var oldValues = new { existing.Id, existing.Name, existing.Price, existing.CategoryId, existing.Description, existing.ImageUrl, existing.Calories, existing.Allergens };

                existing.Name = model.Name.Trim();
                existing.Description = model.Description?.Trim() ?? string.Empty;
                existing.Price = model.Price;
                existing.CategoryId = model.CategoryId;
                existing.ImageUrl = model.ImageUrl?.Trim();
                existing.Calories = model.Calories;
                existing.Allergens = allergens;

                _menuItemService.TUpdate(existing);

                var newValues = new { existing.Id, existing.Name, existing.Price, existing.CategoryId, existing.Description, existing.ImageUrl, existing.Calories, existing.Allergens };

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

        [NonAction]
        public IActionResult Edit(MenuItem menuItem) =>
            Edit(new MenuItemInputModel
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Price = menuItem.Price,
                CategoryId = menuItem.CategoryId,
                Description = menuItem.Description,
                ImageUrl = menuItem.ImageUrl,
                Calories = menuItem.Calories,
                Allergens = menuItem.Allergens
            });

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var menuItem = _menuItemService.TGetByID(id);
            if (menuItem != null)
            {
                _auditContextService.Log(
                    action: "MENU_ITEM_DELETED",
                    entityType: "MenuItem",
                    entityId: menuItem.Id,
                    description: $"Admin tarafından ürün silindi: '{menuItem.Name}'",
                    oldEntity: new { menuItem.Id, menuItem.Name, menuItem.Price, menuItem.CategoryId }
                );

                _menuItemService.TDelete(menuItem);
            }

            return RedirectToAction("Index");
        }
    }
}
