using BusinessLayer.Abstract;
using dijitalmenu.Filters;
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
        public IActionResult Create(Theme theme)
        {
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

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");
            var theme = _themeService.TGetByID(id);
            return View(theme);
        }

        [HttpPost]
        public IActionResult Edit(Theme theme)
        {
            var existing = _themeService.TGetByID(theme.Id);
            if (existing != null)
            {
                var oldValues = new { existing.Id, existing.Name, existing.PrimaryColor, existing.SecondaryColor, existing.BackgroundColor, existing.FontFamily, existing.Layout, existing.IsActive };

                existing.Name = theme.Name;
                existing.PrimaryColor = theme.PrimaryColor;
                existing.SecondaryColor = theme.SecondaryColor;
                existing.BackgroundColor = theme.BackgroundColor;
                existing.FontFamily = theme.FontFamily;
                existing.Layout = theme.Layout;
                existing.IsActive = theme.IsActive;

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
                    action: "THEME_UPDATED",
                    entityType: "Theme",
                    entityId: theme.Id,
                    description: $"Tema durumu değiştirildi: '{theme.Name}' -> {(theme.IsActive ? "Aktif" : "Pasif")}",
                    oldEntity: new { IsActive = oldStatus },
                    newEntity: new { IsActive = theme.IsActive }
                );

                TempData["Success"] = $"\"{theme.Name}\" teması {(theme.IsActive ? "aktifleştirildi" : "pasife alındı")}.";
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
