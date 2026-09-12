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
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IMenuService _menuService;
        private readonly IAuditContextService _auditContextService;

        public CategoryController(
            ICategoryService categoryService,
            IMenuService menuService,
            IAuditContextService auditContextService)
        {
            _categoryService = categoryService;
            _menuService = menuService;
            _auditContextService = auditContextService;
        }

        public IActionResult Index()
        {
            var list = _categoryService.TGetListAll();
            ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Menus = _menuService.TGetListAll();
            ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        public IActionResult Create(CategoryInputModel model)
        {
            var menu = _menuService.TGetByID(model.MenuId);
            if (menu == null)
            {
                ModelState.AddModelError("MenuId", "Geçersiz menü seçildi.");
                ViewBag.Menus = _menuService.TGetListAll();
                ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");
                return View();
            }

            var category = new Category
            {
                Name = model.Name.Trim(),
                MenuId = model.MenuId,
                ImageUrl = model.ImageUrl?.Trim()
            };

            _categoryService.TInsert(category);

            _auditContextService.Log(
                action: "CATEGORY_CREATED",
                entityType: "Category",
                entityId: category.Id,
                description: $"Admin tarafından kategori eklendi: '{category.Name}'",
                newEntity: new { category.Id, category.Name, category.MenuId, category.ImageUrl }
            );

            return RedirectToAction("Index");
        }

        [NonAction]
        public IActionResult Create(Category category) =>
            Create(new CategoryInputModel { Id = category.Id, Name = category.Name, MenuId = category.MenuId, ImageUrl = category.ImageUrl });

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.Menus = _menuService.TGetListAll();
            ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");
            var category = _categoryService.TGetByID(id);
            return View(category);
        }

        [HttpPost]
        [ActionName("Edit")]
        public IActionResult Edit(CategoryInputModel model)
        {
            var existing = _categoryService.TGetByID(model.Id);
            if (existing != null)
            {
                var menu = _menuService.TGetByID(model.MenuId);
                if (menu == null)
                {
                    ModelState.AddModelError("MenuId", "Geçersiz menü seçildi.");
                    ViewBag.Menus = _menuService.TGetListAll();
                    ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");
                    return View(existing);
                }

                var oldValues = new { existing.Id, existing.Name, existing.MenuId, existing.ImageUrl };

                existing.Name = model.Name.Trim();
                existing.MenuId = model.MenuId;
                existing.ImageUrl = model.ImageUrl?.Trim();

                _categoryService.TUpdate(existing);

                var newValues = new { existing.Id, existing.Name, existing.MenuId, existing.ImageUrl };

                _auditContextService.Log(
                    action: "CATEGORY_UPDATED",
                    entityType: "Category",
                    entityId: existing.Id,
                    description: $"Admin tarafından kategori güncellendi: '{existing.Name}'",
                    oldEntity: oldValues,
                    newEntity: newValues
                );
            }

            return RedirectToAction("Index");
        }

        [NonAction]
        public IActionResult Edit(Category category) =>
            Edit(new CategoryInputModel { Id = category.Id, Name = category.Name, MenuId = category.MenuId, ImageUrl = category.ImageUrl });

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var category = _categoryService.TGetByID(id);
            if (category != null)
            {
                _auditContextService.Log(
                    action: "CATEGORY_DELETED",
                    entityType: "Category",
                    entityId: category.Id,
                    description: $"Admin tarafından kategori silindi: '{category.Name}'",
                    oldEntity: new { category.Id, category.Name, category.MenuId }
                );

                _categoryService.TDelete(category);
            }

            return RedirectToAction("Index");
        }
    }
}
