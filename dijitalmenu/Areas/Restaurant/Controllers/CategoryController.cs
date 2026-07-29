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

        private Menu? GetMyMenu() =>
            _menuService.TGetListAll().FirstOrDefault(menu => menu.RestaurantId == GetRestaurantId());

        public IActionResult Index()
        {
            var menu = GetMyMenu();
            var categories = menu == null
                ? new List<Category>()
                : _categoryService.TGetListAll().Where(category => category.MenuId == menu.Id).ToList();

            ViewBag.RestaurantUsername = HttpContext.Session.GetString("RestaurantUsername");
            return View(categories);
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

            if (!TryNormalizeCategoryName(name, menu.Id, null, out var normalizedName, out var error))
            {
                ViewBag.Error = error;
                ViewBag.RestaurantUsername = HttpContext.Session.GetString("RestaurantUsername");
                return View();
            }

            _categoryService.TInsert(new Category { Name = normalizedName, MenuId = menu.Id });
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var menu = GetMyMenu();
            var category = _categoryService.TGetByID(id);

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

            if (!TryNormalizeCategoryName(name, menu.Id, category.Id, out var normalizedName, out var error))
            {
                TempData["Error"] = error;
                return RedirectToAction("Edit", new { id });
            }

            category.Name = normalizedName;
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

        private bool TryNormalizeCategoryName(string? name, int menuId, int? currentCategoryId, out string normalizedName, out string error)
        {
            normalizedName = name?.Trim() ?? string.Empty;
            if (normalizedName.Length is < 1 or > 100)
            {
                error = "Kategori adı 1 ile 100 karakter arasında olmalıdır.";
                return false;
            }

            var categoryNameToCheck = normalizedName;
            var alreadyExists = _categoryService.TGetListAll().Any(category =>
                category.MenuId == menuId && category.Id != currentCategoryId &&
                category.Name.Equals(categoryNameToCheck, StringComparison.OrdinalIgnoreCase));
            if (alreadyExists)
            {
                error = "Bu kategori zaten mevcut.";
                return false;
            }

            error = string.Empty;
            return true;
        }
    }
}
