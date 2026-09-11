using BusinessLayer.Abstract;
using dijitalmenu.Filters;
using dijitalmenu.Services;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace dijitalmenu.Areas.Restaurant.Controllers
{
    [Area("Restaurant")]
    [ServiceFilter(typeof(RestaurantAuthFilter))]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IMenuService _menuService;
        private readonly IStorageService _storageService;
        private readonly IAuditContextService _auditContextService;

        public CategoryController(
            ICategoryService categoryService,
            IMenuService menuService,
            IStorageService storageService,
            IAuditContextService auditContextService)
        {
            _categoryService = categoryService;
            _menuService = menuService;
            _storageService = storageService;
            _auditContextService = auditContextService;
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
        public IActionResult Create(string name, IFormFile? photoFile)
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

            if (!_storageService.TrySaveImage(photoFile, "categories", out var uploadedImageUrl, out var uploadError))
            {
                ViewBag.Error = uploadError;
                ViewBag.RestaurantUsername = HttpContext.Session.GetString("RestaurantUsername");
                return View();
            }

            var category = new Category { Name = normalizedName, MenuId = menu.Id, ImageUrl = uploadedImageUrl };
            _categoryService.TInsert(category);

            _auditContextService.Log(
                action: "CATEGORY_CREATED",
                entityType: "Category",
                entityId: category.Id,
                description: $"Yeni kategori eklendi: '{category.Name}'",
                newEntity: new { category.Id, category.Name, category.MenuId, category.ImageUrl }
            );

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
        public IActionResult Edit(int id, string name, IFormFile? photoFile)
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

            if (!_storageService.TrySaveImage(photoFile, "categories", out var uploadedImageUrl, out var uploadError))
            {
                TempData["Error"] = uploadError;
                return RedirectToAction("Edit", new { id });
            }

            var oldValues = new
            {
                category.Id,
                category.Name,
                category.MenuId,
                category.ImageUrl
            };

            if (uploadedImageUrl != null && !string.IsNullOrEmpty(category.ImageUrl))
            {
                _storageService.DeleteImage(category.ImageUrl);
            }

            category.Name = normalizedName;
            category.ImageUrl = uploadedImageUrl ?? category.ImageUrl;
            _categoryService.TUpdate(category);

            var newValues = new
            {
                category.Id,
                category.Name,
                category.MenuId,
                category.ImageUrl
            };

            _auditContextService.Log(
                action: "CATEGORY_UPDATED",
                entityType: "Category",
                entityId: category.Id,
                description: $"Kategori güncellendi: '{category.Name}'",
                oldEntity: oldValues,
                newEntity: newValues
            );

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var menu = GetMyMenu();
            var category = _categoryService.TGetByID(id);

            if (category != null && menu != null && category.MenuId == menu.Id)
            {
                var oldValues = new
                {
                    category.Id,
                    category.Name,
                    category.MenuId
                };

                _auditContextService.Log(
                    action: "CATEGORY_DELETED",
                    entityType: "Category",
                    entityId: category.Id,
                    description: $"Kategori silindi: '{category.Name}'",
                    oldEntity: oldValues
                );

                if (!string.IsNullOrEmpty(category.ImageUrl))
                {
                    _storageService.DeleteImage(category.ImageUrl);
                }

                _categoryService.TDelete(category);
            }

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

            var existingCategories = _categoryService.TGetListAll()
                .Where(category => category.MenuId == menuId && (currentCategoryId == null || category.Id != currentCategoryId.Value))
                .ToList();

            var trCulture = CultureInfo.GetCultureInfo("tr-TR");
            var targetToCompare = normalizedName;
            var isDuplicate = existingCategories.Any(category =>
                string.Equals(category.Name.Trim(), targetToCompare, StringComparison.CurrentCultureIgnoreCase) ||
                string.Equals(category.Name.Trim(), targetToCompare, StringComparison.OrdinalIgnoreCase) ||
                trCulture.CompareInfo.Compare(category.Name.Trim(), targetToCompare, CompareOptions.IgnoreCase) == 0);

            if (isDuplicate)
            {
                error = "Bu menüde aynı isimde başka bir kategori zaten var.";
                return false;
            }

            error = string.Empty;
            return true;
        }
    }
}
