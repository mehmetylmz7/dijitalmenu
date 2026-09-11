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
        private readonly IMenuItemService _menuItemService;
        private readonly ICategoryService _categoryService;
        private readonly IMenuService _menuService;
        private readonly IStorageService _storageService;
        private readonly IAuditContextService _auditContextService;

        public MenuItemController(
            IMenuItemService menuItemService,
            ICategoryService categoryService,
            IMenuService menuService,
            IStorageService storageService,
            IAuditContextService auditContextService)
        {
            _menuItemService = menuItemService;
            _categoryService = categoryService;
            _menuService = menuService;
            _storageService = storageService;
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

            if (!_storageService.TrySaveImage(photoFile, "menu-items", out var uploadedImageUrl, out var uploadError))
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

            if (!_storageService.TrySaveImage(photoFile, "menu-items", out var uploadedImageUrl, out var uploadError))
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

            if (uploadedImageUrl != null && !string.IsNullOrEmpty(existingItem.ImageUrl) && existingItem.ImageUrl.StartsWith("/images/menu-items/", StringComparison.OrdinalIgnoreCase))
            {
                _storageService.DeleteImage(existingItem.ImageUrl);
            }

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

                if (!string.IsNullOrEmpty(item.ImageUrl) && item.ImageUrl.StartsWith("/images/menu-items/", StringComparison.OrdinalIgnoreCase))
                {
                    _storageService.DeleteImage(item.ImageUrl);
                }

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
    }
}
