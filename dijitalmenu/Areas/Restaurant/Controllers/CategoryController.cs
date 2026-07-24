using BusinessLayer.Abstract;
using dijitalmenu.Filters;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace dijitalmenu.Areas.Restaurant.Controllers
{
    [Area("Restaurant")]
    [ServiceFilter(typeof(RestaurantAuthFilter))]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IMenuService _menuService;

        public CategoryController(ICategoryService categoryService, IMenuService menuService)
        {
            _categoryService = categoryService;
            _menuService = menuService;
        }

        private int GetRestaurantId() =>
            int.Parse(HttpContext.Session.GetString("RestaurantId")!);

        private Menu? GetMyMenu()
        {
            var rid = GetRestaurantId();
            return _menuService.TGetListAll().FirstOrDefault(m => m.RestaurantId == rid);
        }

        public IActionResult Index()
        {
            var menu = GetMyMenu();
            var list = menu == null
                ? new List<Category>()
                : _categoryService.TGetListAll().Where(c => c.MenuId == menu.Id).ToList();

            ViewBag.RestaurantUsername = HttpContext.Session.GetString("RestaurantUsername");
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.RestaurantUsername = HttpContext.Session.GetString("RestaurantUsername");
            return View();
        }

        [HttpPost]
        public IActionResult Create(string name)
        {
            var menu = GetMyMenu();
            if (menu == null)
            {
                ViewBag.Error = "Menünüz bulunamadı.";
                return View();
            }

            _categoryService.TInsert(new Category { Name = name, MenuId = menu.Id });
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var menu = GetMyMenu();
            var category = _categoryService.TGetByID(id);

            // Güvenlik: Başkasının kategorisi değilse engelle
            if (category == null || menu == null || category.MenuId != menu.Id)
                return RedirectToAction("Index");

            ViewBag.RestaurantUsername = HttpContext.Session.GetString("RestaurantUsername");
            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(int id, string name)
        {
            var menu = GetMyMenu();
            var category = _categoryService.TGetByID(id);

            if (category == null || menu == null || category.MenuId != menu.Id)
                return RedirectToAction("Index");

            category.Name = name;
            _categoryService.TUpdate(category);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var menu = GetMyMenu();
            var category = _categoryService.TGetByID(id);

            if (category != null && menu != null && category.MenuId == menu.Id)
                _categoryService.TDelete(category);

            return RedirectToAction("Index");
        }
    }
}
