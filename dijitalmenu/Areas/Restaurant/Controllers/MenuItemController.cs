using BusinessLayer.Abstract;
using dijitalmenu.Filters;
using dijitalmenu.Services;
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
        private readonly IAuditContextService _auditContextService;

        public MenuItemController(
            IMenuItemService menuItemService,
            ICategoryService categoryService,
            IMenuService menuService,
            IWebHostEnvironment environment,
            IAuditContextService auditContextService)
        {
            _menuItemService = menuItemService;
            _categoryService = categoryService;
            _menuService = menuService;
            _environment = environment;
            _auditContextService = auditContextService;
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

            // Audit Log
            var newValues = new
            {
                menuItem.Id,
                menuItem.Name,
                menuItem.Price,
                menuItem.CategoryId,
                menuItem.Description,
                menuItem.ImageUrl,
                menuItem.DisplayOrder
            };

            _auditContextService.Log(
                action: "MENU_ITEM_CREATED",
                entityType: "MenuItem",
                entityId: menuItem.Id,
                description: $"Yeni ürün eklendi: '{menuItem.Name}' ({menuItem.Price:C})",
                newEntity: newValues
            );

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

            var oldValues = new
            {
                existingItem.Id,
                existingItem.Name,
                existingItem.Price,
                existingItem.CategoryId,
                existingItem.Description,
                existingItem.ImageUrl,
                existingItem.DisplayOrder
            };

            existingItem.Name = menuItem.Name.Trim();
            existingItem.Description = menuItem.Description?.Trim() ?? string.Empty;
            existingItem.Price = menuItem.Price;
            existingItem.CategoryId = menuItem.CategoryId;
            existingItem.ImageUrl = uploadedImageUrl ?? menuItem.ImageUrl?.Trim() ?? existingItem.ImageUrl;

            _menuItemService.TUpdate(existingItem);

            var newValues = new
            {
                existingItem.Id,
                existingItem.Name,
                existingItem.Price,
                existingItem.CategoryId,
                existingItem.Description,
                existingItem.ImageUrl,
                existingItem.DisplayOrder
            };

            _auditContextService.Log(
                action: "MENU_ITEM_UPDATED",
                entityType: "MenuItem",
                entityId: existingItem.Id,
                description: $"Menü ürünü güncellendi: '{existingItem.Name}'",
                oldEntity: oldValues,
                newEntity: newValues
            );

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var item = _menuItemService.TGetByID(id);
            var categoryIds = GetMyCategories().Select(category => category.Id).ToHashSet();

            if (item != null && categoryIds.Contains(item.CategoryId))
            {
                var oldValues = new
                {
                    item.Id,
                    item.Name,
                    item.Price,
                    item.CategoryId,
                    item.Description
                };

                _auditContextService.Log(
                    action: "MENU_ITEM_DELETED",
                    entityType: "MenuItem",
                    entityId: item.Id,
                    description: $"Menü ürünü silindi: '{item.Name}'",
                    oldEntity: oldValues
                );

                _menuItemService.TDelete(item);
            }

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

        private static bool IsValidImageUrl(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return true;

            if (imageUrl.StartsWith("/images/menu-items/", StringComparison.OrdinalIgnoreCase))
                return true;

            if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri))
                return false;

            return uri.Scheme is "http" or "https";
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

            var folderPath = Path.Combine(_environment.WebRootPath, "images", "menu-items");
            Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
            var fullPath = Path.Combine(folderPath, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            photoFile.CopyTo(stream);

            imageUrl = $"/images/menu-items/{fileName}";
            return true;
        }
    }
}
