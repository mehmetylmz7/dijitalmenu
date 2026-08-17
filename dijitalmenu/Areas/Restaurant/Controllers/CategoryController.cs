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
        private const long MaxImageFileSize = 5 * 1024 * 1024;
        private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp"
        };
        private static readonly HashSet<string> AllowedImageContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg", "image/png", "image/gif", "image/webp"
        };

        private readonly ICategoryService _categoryService;
        private readonly IMenuService _menuService;
        private readonly IWebHostEnvironment _environment;
        private readonly IAuditContextService _auditContextService;

        public CategoryController(
            ICategoryService categoryService,
            IMenuService menuService,
            IWebHostEnvironment environment,
            IAuditContextService auditContextService)
        {
            _categoryService = categoryService;
            _menuService = menuService;
            _environment = environment;
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

            if (!TrySavePhoto(photoFile, out var uploadedImageUrl, out var uploadError))
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

            if (!TrySavePhoto(photoFile, out var uploadedImageUrl, out var uploadError))
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

        private bool TrySavePhoto(IFormFile? photoFile, out string? imageUrl, out string error)
        {
            imageUrl = null;
            error = string.Empty;

            if (photoFile == null || photoFile.Length == 0)
                return true;

            if (photoFile.Length > MaxImageFileSize)
            {
                error = "Yüklenen görsel en fazla 5 MB olabilir.";
                return false;
            }

            var extension = Path.GetExtension(photoFile.FileName);
            if (!AllowedImageExtensions.Contains(extension) ||
                !AllowedImageContentTypes.Contains(photoFile.ContentType))
            {
                error = "Sadece JPG, JPEG, PNG, GIF ve WEBP formatları desteklenmektedir.";
                return false;
            }

            var folderPath = Path.Combine(_environment.WebRootPath, "images", "categories");
            Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
            var fullPath = Path.Combine(folderPath, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            photoFile.CopyTo(stream);

            imageUrl = $"/images/categories/{fileName}";
            return true;
        }
    }
}
