using BusinessLayer.Abstract;
using dijitalmenu.Filters;
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

        public CategoryController(ICategoryService categoryService, IMenuService menuService, IWebHostEnvironment environment)
        {
            _categoryService = categoryService;
            _menuService = menuService;
            _environment = environment;
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

            _categoryService.TInsert(new Category { Name = normalizedName, MenuId = menu.Id, ImageUrl = uploadedImageUrl });
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

            category.Name = normalizedName;
            category.ImageUrl = uploadedImageUrl ?? category.ImageUrl;
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
            var turkishCulture = CultureInfo.GetCultureInfo("tr-TR");
            var alreadyExists = _categoryService.TGetListAll().Any(category =>
                category.MenuId == menuId && category.Id != currentCategoryId &&
                string.Compare(category.Name, categoryNameToCheck, ignoreCase: true, turkishCulture) == 0);
            if (alreadyExists)
            {
                error = "Bu kategori zaten mevcut.";
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

            var extension = Path.GetExtension(photoFile.FileName);
            if (photoFile.Length > MaxImageFileSize || !AllowedImageExtensions.Contains(extension) ||
                !AllowedImageContentTypes.Contains(photoFile.ContentType) || !HasValidImageSignature(photoFile))
            {
                error = "Görsel JPG, PNG, GIF veya WebP türünde ve en fazla 5 MB olmalıdır.";
                return false;
            }

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolder);

            var newFileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
            var filePath = Path.Combine(uploadsFolder, newFileName);
            using var stream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            photoFile.CopyTo(stream);
            imageUrl = $"/uploads/{newFileName}";
            return true;
        }

        private static bool HasValidImageSignature(IFormFile photoFile)
        {
            using var stream = photoFile.OpenReadStream();
            Span<byte> header = stackalloc byte[12];
            var read = stream.Read(header);

            return (read >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF) ||
                   (read >= 8 && header[..8].SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A })) ||
                   (read >= 6 && (header[..6].SequenceEqual("GIF87a"u8) || header[..6].SequenceEqual("GIF89a"u8))) ||
                   (read >= 12 && header[..4].SequenceEqual("RIFF"u8) && header[8..12].SequenceEqual("WEBP"u8));
        }
    }
}
