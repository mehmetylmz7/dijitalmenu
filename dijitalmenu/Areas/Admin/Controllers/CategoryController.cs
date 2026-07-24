using BusinessLayer.Abstract;
using dijitalmenu.Filters;
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

        public CategoryController(ICategoryService categoryService, IMenuService menuService)
        {
            _categoryService = categoryService;
            _menuService = menuService;
        }

        public IActionResult Index()
        {
            var list = _categoryService.TGetListAll();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Menus = _menuService.TGetListAll();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            _categoryService.TInsert(category);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.Menus = _menuService.TGetListAll();
            var category = _categoryService.TGetByID(id);
            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            _categoryService.TUpdate(category);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var category = _categoryService.TGetByID(id);
            if (category != null)
                _categoryService.TDelete(category);
            return RedirectToAction("Index");
        }
    }
}
