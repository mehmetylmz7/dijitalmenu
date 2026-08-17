using BusinessLayer.Abstract;
using dijitalmenu.Filters;
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
        public IActionResult Create(Category category)
        {
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

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.Menus = _menuService.TGetListAll();
            ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");
            var category = _categoryService.TGetByID(id);
            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            var existing = _categoryService.TGetByID(category.Id);
            if (existing != null)
            {
                var oldValues = new { existing.Id, existing.Name, existing.MenuId, existing.ImageUrl };

                existing.Name = category.Name;
                existing.MenuId = category.MenuId;
                existing.ImageUrl = category.ImageUrl;

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
