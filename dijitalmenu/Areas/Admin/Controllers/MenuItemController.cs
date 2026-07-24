using BusinessLayer.Abstract;
using dijitalmenu.Filters;
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

        public MenuItemController(IMenuItemService menuItemService, ICategoryService categoryService)
        {
            _menuItemService = menuItemService;
            _categoryService = categoryService;
        }

        public IActionResult Index()
        {
            var list = _menuItemService.TGetListAll();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = _categoryService.TGetListAll();
            return View();
        }

        [HttpPost]
        public IActionResult Create(MenuItem menuItem)
        {
            _menuItemService.TInsert(menuItem);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.Categories = _categoryService.TGetListAll();
            var menuItem = _menuItemService.TGetByID(id);
            return View(menuItem);
        }

        [HttpPost]
        public IActionResult Edit(MenuItem menuItem)
        {
            _menuItemService.TUpdate(menuItem);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var menuItem = _menuItemService.TGetByID(id);
            if (menuItem != null)
                _menuItemService.TDelete(menuItem);
            return RedirectToAction("Index");
        }
    }
}
