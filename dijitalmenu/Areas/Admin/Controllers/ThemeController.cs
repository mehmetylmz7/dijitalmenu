using BusinessLayer.Abstract;
using dijitalmenu.Filters;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace dijitalmenu.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ServiceFilter(typeof(AdminAuthFilter))]
    public class ThemeController : Controller
    {
        private readonly IThemeService _themeService;

        public ThemeController(IThemeService themeService)
        {
            _themeService = themeService;
        }

        public IActionResult Index()
        {
            var list = _themeService.TGetListAll();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Theme theme)
        {
            _themeService.TInsert(theme);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var theme = _themeService.TGetByID(id);
            return View(theme);
        }

        [HttpPost]
        public IActionResult Edit(Theme theme)
        {
            _themeService.TUpdate(theme);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var theme = _themeService.TGetByID(id);
            _themeService.TDelete(theme);
            return RedirectToAction("Index");
        }
    }
}
