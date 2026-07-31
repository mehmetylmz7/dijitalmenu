using BusinessLayer.Abstract;
using dijitalmenu.Filters;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace dijitalmenu.Areas.Restaurant.Controllers
{
    [Area("Restaurant")]
    [ServiceFilter(typeof(RestaurantAuthFilter))]
    public class MenuItemController : Controller
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

        private readonly IMenuItemService _menuItemService;
        private readonly ICategoryService _categoryService;
        private readonly IMenuService _menuService;
        private readonly IWebHostEnvironment _environment;

        public MenuItemController(
            IMenuItemService menuItemService,
            ICategoryService categoryService,
            IMenuService menuService,
            IWebHostEnvironment environment)
        {
            _menuItemService = menuItemService;
            _categoryService = categoryService;
            _menuService = menuService;
            _environment = environment;
        }

        private int GetRestaurantId() =>
            int.Parse(HttpContext.Session.GetString("RestaurantId")!);

        private List<Category> GetMyCategories()
        {
            var restaurantId = GetRestaurantId();
            var menu = _menuService.TGetListAll().FirstOrDefault(item => item.RestaurantId == restaurantId);
            if (menu == null) return new List<Category>();

            return _categoryService.TGetListAll().Where(category => category.MenuId == menu.Id).ToList();
        }

        public IActionResult Index()
        {
            var categories = GetMyCategories();
            var categoryIds = categories.Select(category => category.Id).ToHashSet();
            var items = _menuItemService.TGetListAll()
                .Where(item => categoryIds.Contains(item.CategoryId)).ToList();

            ViewBag.Categories = categories;
            ViewBag.RestaurantUsername = HttpContext.Session.GetString("RestaurantUsername");
            return View(items);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var categories = GetMyCategories();
            if (!categories.Any())
            {
                TempData["Error"] = "Önce en az bir kategori eklemelisiniz.";
                return RedirectToAction("Index");
            }

            ViewBag.Categories = categories;
            ViewBag.RestaurantUsername = HttpContext.Session.GetString("RestaurantUsername");
            return View();
        }

        [HttpPost]
        public IActionResult Create(MenuItem menuItem, IFormFile? photoFile)
        {
            var categoryIds = GetMyCategories().Select(category => category.Id).ToHashSet();
            if (!categoryIds.Contains(menuItem.CategoryId))
                return RedirectToAction("Index");

            if (!TryValidateMenuItem(menuItem, out var validationError))
            {
                TempData["Error"] = validationError;
                return RedirectToAction("Create");
            }

            if (!TrySavePhoto(photoFile, out var uploadedImageUrl, out var uploadError))
            {
                TempData["Error"] = uploadError;
                return RedirectToAction("Create");
            }

            menuItem.ImageUrl = uploadedImageUrl ?? menuItem.ImageUrl?.Trim();
            _menuItemService.TInsert(menuItem);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var item = _menuItemService.TGetByID(id);
            var categories = GetMyCategories();
            var categoryIds = categories.Select(category => category.Id).ToHashSet();

            if (item == null || !categoryIds.Contains(item.CategoryId))
                return RedirectToAction("Index");

            ViewBag.Categories = categories;
            ViewBag.RestaurantUsername = HttpContext.Session.GetString("RestaurantUsername");
            return View(item);
        }

        [HttpPost]
        public IActionResult Edit(MenuItem menuItem, IFormFile? photoFile)
        {
            var categoryIds = GetMyCategories().Select(category => category.Id).ToHashSet();
            var existingItem = _menuItemService.TGetByID(menuItem.Id);

            if (existingItem == null || !categoryIds.Contains(existingItem.CategoryId) || !categoryIds.Contains(menuItem.CategoryId))
                return RedirectToAction("Index");

            if (!TryValidateMenuItem(menuItem, out var validationError))
            {
                TempData["Error"] = validationError;
                return RedirectToAction("Edit", new { id = menuItem.Id });
            }

            if (!TrySavePhoto(photoFile, out var uploadedImageUrl, out var uploadError))
            {
                TempData["Error"] = uploadError;
                return RedirectToAction("Edit", new { id = menuItem.Id });
            }

            existingItem.Name = menuItem.Name.Trim();
            existingItem.Description = menuItem.Description?.Trim() ?? string.Empty;
            existingItem.Price = menuItem.Price;
            existingItem.CategoryId = menuItem.CategoryId;
            existingItem.ImageUrl = uploadedImageUrl ?? menuItem.ImageUrl?.Trim() ?? existingItem.ImageUrl;

            _menuItemService.TUpdate(existingItem);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var item = _menuItemService.TGetByID(id);
            var categoryIds = GetMyCategories().Select(category => category.Id).ToHashSet();

            if (item != null && categoryIds.Contains(item.CategoryId))
                _menuItemService.TDelete(item);

            return RedirectToAction("Index");
        }

        private bool TryValidateMenuItem(MenuItem menuItem, out string error)
        {
            menuItem.Name = menuItem.Name?.Trim() ?? string.Empty;
            menuItem.Description = menuItem.Description?.Trim() ?? string.Empty;
            menuItem.ImageUrl = menuItem.ImageUrl?.Trim();

            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(menuItem, new ValidationContext(menuItem), validationResults, validateAllProperties: true))
            {
                error = validationResults
                    .Select(item => item.ErrorMessage)
                    .FirstOrDefault(message => !string.IsNullOrWhiteSpace(message)) ?? "Ürün bilgileri geçersiz.";
                return false;
            }

            if (!IsValidImageUrl(menuItem.ImageUrl))
            {
                error = "Görsel bağlantısı yalnızca HTTPS veya HTTP adresi olabilir.";
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

        private static bool IsValidImageUrl(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return true;

            if (imageUrl.StartsWith("/uploads/", StringComparison.Ordinal) && !imageUrl.Contains("..", StringComparison.Ordinal))
                return true;

            return Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri) &&
                   (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp);
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
