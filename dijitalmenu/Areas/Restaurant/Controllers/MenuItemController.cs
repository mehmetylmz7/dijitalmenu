using BusinessLayer.Abstract;
using dijitalmenu.Filters;
using dijitalmenu.Models;
using dijitalmenu.Services;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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
            ViewBag.Allergens = dijitalmenu.Helpers.AllergenHelper.DefaultAllergens;
            ViewBag.RestaurantUsername = HttpContext.Session.GetString("RestaurantUsername");
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        public IActionResult Create(MenuItemInputModel model, IFormFile? photoFile)
        {
            var categoryIds = GetMyCategories().Select(category => category.Id).ToHashSet();
            if (!categoryIds.Contains(model.CategoryId))
                return Forbid();

            if (!TryValidateMenuItem(model, out var validationError))
            {
                TempData["Error"] = validationError;
                return RedirectToAction("Create");
            }

            if (!_storageService.TrySaveImage(photoFile, "menu-items", out var uploadedImageUrl, out var uploadError))
            {
                TempData["Error"] = uploadError;
                return RedirectToAction("Create");
            }

            var allergens = dijitalmenu.Helpers.AllergenHelper.FormatAllergens(model.SelectedAllergens) ?? model.Allergens?.Trim();

            var menuItem = new MenuItem
            {
                Name = model.Name.Trim(),
                Description = model.Description?.Trim() ?? string.Empty,
                Price = model.Price,
                CategoryId = model.CategoryId,
                ImageUrl = uploadedImageUrl ?? model.ImageUrl?.Trim(),
                Calories = model.Calories,
                Allergens = allergens
            };

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
                menuItem.DisplayOrder,
                menuItem.Calories,
                menuItem.Allergens
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

        // Backward compatibility overload for unit tests
        [NonAction]
        public IActionResult Create(MenuItem menuItem, IFormFile? photoFile) =>
            Create(new MenuItemInputModel
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                CategoryId = menuItem.CategoryId,
                ImageUrl = menuItem.ImageUrl,
                DisplayOrder = menuItem.DisplayOrder,
                Calories = menuItem.Calories,
                Allergens = menuItem.Allergens
            }, photoFile);

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var item = _menuItemService.TGetByID(id);
            if (item == null)
                return NotFound();

            var categories = GetMyCategories();
            var categoryIds = categories.Select(category => category.Id).ToHashSet();

            if (!categoryIds.Contains(item.CategoryId))
                return Forbid();

            ViewBag.Categories = categories;
            ViewBag.Allergens = dijitalmenu.Helpers.AllergenHelper.DefaultAllergens;
            ViewBag.SelectedAllergens = dijitalmenu.Helpers.AllergenHelper.ParseAllergens(item.Allergens);
            ViewBag.RestaurantUsername = HttpContext.Session.GetString("RestaurantUsername");
            return View(item);
        }

        [HttpPost]
        [ActionName("Edit")]
        public IActionResult Edit(MenuItemInputModel model, IFormFile? photoFile)
        {
            var categoryIds = GetMyCategories().Select(category => category.Id).ToHashSet();
            var existingItem = _menuItemService.TGetByID(model.Id);

            if (existingItem == null)
                return NotFound();

            // Tenant isolation: Both the existing item and target category must belong to the tenant's menu
            if (!categoryIds.Contains(existingItem.CategoryId) || !categoryIds.Contains(model.CategoryId))
                return Forbid();

            if (!TryValidateMenuItem(model, out var validationError))
            {
                TempData["Error"] = validationError;
                return RedirectToAction("Edit", new { id = model.Id });
            }

            if (!_storageService.TrySaveImage(photoFile, "menu-items", out var uploadedImageUrl, out var uploadError))
            {
                TempData["Error"] = uploadError;
                return RedirectToAction("Edit", new { id = model.Id });
            }

            var allergens = dijitalmenu.Helpers.AllergenHelper.FormatAllergens(model.SelectedAllergens) ?? model.Allergens?.Trim();

            var oldValues = new
            {
                existingItem.Id,
                existingItem.Name,
                existingItem.Price,
                existingItem.CategoryId,
                existingItem.Description,
                existingItem.ImageUrl,
                existingItem.DisplayOrder,
                existingItem.Calories,
                existingItem.Allergens
            };

            if (uploadedImageUrl != null && !string.IsNullOrEmpty(existingItem.ImageUrl) && existingItem.ImageUrl.StartsWith("/images/menu-items/", StringComparison.OrdinalIgnoreCase))
            {
                _storageService.DeleteImage(existingItem.ImageUrl);
            }

            // Safe explicit mapping (prevents mass assignment)
            existingItem.Name = model.Name.Trim();
            existingItem.Description = model.Description?.Trim() ?? string.Empty;
            existingItem.Price = model.Price;
            existingItem.CategoryId = model.CategoryId;
            existingItem.ImageUrl = uploadedImageUrl ?? model.ImageUrl?.Trim() ?? existingItem.ImageUrl;
            existingItem.Calories = model.Calories;
            existingItem.Allergens = allergens;

            _menuItemService.TUpdate(existingItem);

            var newValues = new
            {
                existingItem.Id,
                existingItem.Name,
                existingItem.Price,
                existingItem.CategoryId,
                existingItem.Description,
                existingItem.ImageUrl,
                existingItem.DisplayOrder,
                existingItem.Calories,
                existingItem.Allergens
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

        // Backward compatibility overload for unit tests
        [NonAction]
        public IActionResult Edit(MenuItem menuItem, IFormFile? photoFile) =>
            Edit(new MenuItemInputModel
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                CategoryId = menuItem.CategoryId,
                ImageUrl = menuItem.ImageUrl,
                DisplayOrder = menuItem.DisplayOrder,
                Calories = menuItem.Calories,
                Allergens = menuItem.Allergens
            }, photoFile);

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var item = _menuItemService.TGetByID(id);
            if (item == null)
                return NotFound();

            var categoryIds = GetMyCategories().Select(category => category.Id).ToHashSet();
            if (!categoryIds.Contains(item.CategoryId))
                return Forbid();

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

            return RedirectToAction("Index");
        }

        private bool TryValidateMenuItem(MenuItemInputModel model, out string error)
        {
            model.Name = model.Name?.Trim() ?? string.Empty;
            model.Description = model.Description?.Trim() ?? string.Empty;
            model.ImageUrl = model.ImageUrl?.Trim();

            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(model, new ValidationContext(model), validationResults, validateAllProperties: true))
            {
                error = validationResults
                    .Select(item => item.ErrorMessage)
                    .FirstOrDefault(message => !string.IsNullOrWhiteSpace(message)) ?? "Ürün bilgileri geçersiz.";
                return false;
            }

            if (!IsValidImageUrl(model.ImageUrl))
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
