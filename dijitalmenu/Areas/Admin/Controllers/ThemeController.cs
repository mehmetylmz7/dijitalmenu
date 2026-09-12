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
    public class ThemeController : Controller
    {
        private readonly IThemeService _themeService;
        private readonly IAuditContextService _auditContextService;

        public ThemeController(IThemeService themeService, IAuditContextService auditContextService)
        {
            _themeService = themeService;
            _auditContextService = auditContextService;
        }

        public IActionResult Index()
        {
            var list = _themeService.TGetListAll();
            ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        public IActionResult Create(ThemeInputModel model)
        {
            var theme = new Theme
            {
                Name = model.Name.Trim(),
                PrimaryColor = model.PrimaryColor.Trim(),
                SecondaryColor = model.SecondaryColor.Trim(),
                BackgroundColor = model.BackgroundColor?.Trim() ?? "#ffffff",
                FontFamily = model.FontFamily?.Trim() ?? "Inter, sans-serif",
                Layout = model.Layout,
                IsActive = model.IsActive
            };

            _themeService.TInsert(theme);

            _auditContextService.Log(
                action: "THEME_CREATED",
                entityType: "Theme",
                entityId: theme.Id,
                description: $"Admin tarafından yeni tema eklendi: '{theme.Name}'",
                newEntity: new { theme.Id, theme.Name, theme.PrimaryColor, theme.SecondaryColor, theme.IsActive }
            );

            return RedirectToAction("Index");
        }

        [NonAction]
        public IActionResult Create(Theme theme) =>
            Create(new ThemeInputModel
            {
                Id = theme.Id,
                Name = theme.Name,
                PrimaryColor = theme.PrimaryColor,
                SecondaryColor = theme.SecondaryColor,
                BackgroundColor = theme.BackgroundColor,
                FontFamily = theme.FontFamily,
                Layout = theme.Layout,
                IsActive = theme.IsActive
            });

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");
            var theme = _themeService.TGetByID(id);
            return View(theme);
        }

        [HttpPost]
        [ActionName("Edit")]
        public IActionResult Edit(ThemeInputModel model)
        {
            var existing = _themeService.TGetByID(model.Id);
            if (existing != null)
            {
                var oldValues = new { existing.Id, existing.Name, existing.PrimaryColor, existing.SecondaryColor, existing.BackgroundColor, existing.FontFamily, existing.Layout, existing.IsActive };

                existing.Name = model.Name.Trim();
                existing.PrimaryColor = model.PrimaryColor.Trim();
                existing.SecondaryColor = model.SecondaryColor.Trim();
                existing.BackgroundColor = model.BackgroundColor?.Trim() ?? "#ffffff";
                existing.FontFamily = model.FontFamily?.Trim() ?? "Inter, sans-serif";
                existing.Layout = model.Layout;
                existing.IsActive = model.IsActive;

                _themeService.TUpdate(existing);

                var newValues = new { existing.Id, existing.Name, existing.PrimaryColor, existing.SecondaryColor, existing.BackgroundColor, existing.FontFamily, existing.Layout, existing.IsActive };

                _auditContextService.Log(
                    action: "THEME_UPDATED",
                    entityType: "Theme",
                    entityId: existing.Id,
                    description: $"Admin tarafından tema güncellendi: '{existing.Name}'",
                    oldEntity: oldValues,
                    newEntity: newValues
                );
            }

            return RedirectToAction("Index");
        }

        [NonAction]
        public IActionResult Edit(Theme theme) =>
            Edit(new ThemeInputModel
            {
                Id = theme.Id,
                Name = theme.Name,
                PrimaryColor = theme.PrimaryColor,
                SecondaryColor = theme.SecondaryColor,
                BackgroundColor = theme.BackgroundColor,
                FontFamily = theme.FontFamily,
                Layout = theme.Layout,
                IsActive = theme.IsActive
            });

        [HttpPost]
        public IActionResult ToggleStatus(int id)
        {
            var theme = _themeService.TGetByID(id);
            if (theme != null)
            {
                var oldStatus = theme.IsActive;
                theme.IsActive = !theme.IsActive;
                _themeService.TUpdate(theme);

                _auditContextService.Log(
                    action: "THEME_STATUS_TOGGLED",
                    entityType: "Theme",
                    entityId: theme.Id,
                    description: $"Tema durumu değiştirildi: '{theme.Name}' ({oldStatus} -> {theme.IsActive})",
                    oldEntity: new { IsActive = oldStatus },
                    newEntity: new { IsActive = theme.IsActive }
                );
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var theme = _themeService.TGetByID(id);
            if (theme != null)
            {
                _auditContextService.Log(
                    action: "THEME_DELETED",
                    entityType: "Theme",
                    entityId: theme.Id,
                    description: $"Admin tarafından tema silindi: '{theme.Name}'",
                    oldEntity: new { theme.Id, theme.Name }
                );

                _themeService.TDelete(theme);
            }

            return RedirectToAction("Index");
        }
    }
}
