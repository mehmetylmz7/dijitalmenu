using System.ComponentModel.DataAnnotations;
using dijitalmenu.Helpers;
using dijitalmenu.Models;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DijitalMenu.Tests;

public class CaloriesAndAllergensTests
{
    [Fact]
    public void AllergenHelper_FormatAllergens_CorrectlyConcatenates()
    {
        // Null or empty
        Assert.Null(AllergenHelper.FormatAllergens(null));
        Assert.Null(AllergenHelper.FormatAllergens(Array.Empty<string>()));
        Assert.Null(AllergenHelper.FormatAllergens(new[] { "  ", "" }));

        // Valid items with whitespace
        var result = AllergenHelper.FormatAllergens(new[] { "Gluten", " Süt (Laktoz) ", "Yumurta" });
        Assert.Equal("Gluten, Süt (Laktoz), Yumurta", result);
    }

    [Fact]
    public void AllergenHelper_ParseAllergens_CorrectlyParsesToSet()
    {
        Assert.Empty(AllergenHelper.ParseAllergens(null));
        Assert.Empty(AllergenHelper.ParseAllergens("   "));

        var set = AllergenHelper.ParseAllergens("Gluten, Süt (Laktoz); Yumurta, Balık");
        Assert.Equal(4, set.Count);
        Assert.Contains("Gluten", set);
        Assert.Contains("Süt (Laktoz)", set);
        Assert.Contains("Yumurta", set);
        Assert.Contains("Balık", set);
    }

    [Fact]
    public void MenuItem_CaloriesAndAllergens_RangeAndLengthValidation()
    {
        // Valid item
        var validItem = new MenuItem
        {
            Name = "Köfte",
            Price = 250,
            CategoryId = 1,
            Calories = 650,
            Allergens = "Gluten, Süt"
        };
        var validResults = new List<ValidationResult>();
        Assert.True(Validator.TryValidateObject(validItem, new ValidationContext(validItem), validResults, true));

        // Negative calories
        var invalidCalItem = new MenuItem
        {
            Name = "Köfte",
            Price = 250,
            CategoryId = 1,
            Calories = -50
        };
        var invalidCalResults = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(invalidCalItem, new ValidationContext(invalidCalItem), invalidCalResults, true));

        // Calories over 10000
        var overCalItem = new MenuItem
        {
            Name = "Ziyafet",
            Price = 2500,
            CategoryId = 1,
            Calories = 10001
        };
        var overCalResults = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(overCalItem, new ValidationContext(overCalItem), overCalResults, true));

        // Allergens over 500 characters
        var longAllergensItem = new MenuItem
        {
            Name = "Salata",
            Price = 100,
            CategoryId = 1,
            Allergens = new string('A', 501)
        };
        var longAllergensResults = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(longAllergensItem, new ValidationContext(longAllergensItem), longAllergensResults, true));
    }

    [Fact]
    public void AdminMenuItemController_CreateAndEdit_PersistsCaloriesAndAllergens()
    {
        using var context = TestSupport.CreateContext();
        var services = TestSupport.Services(context);
        var auditContext = TestSupport.CreateAuditContext(context);

        var cat = new Category { Name = "Tatlılar", MenuId = 1 };
        context.Categories.Add(cat);
        context.SaveChanges();

        var controller = new dijitalmenu.Areas.Admin.Controllers.MenuItemController(services.Items, services.Categories, auditContext)
        {
            ControllerContext = TestSupport.ControllerContext(new() { ["AdminUsername"] = "admin" })
        };

        // Create
        var createModel = new MenuItemInputModel
        {
            Name = "Künefe",
            Description = "Hatay usulü çıtır peynirli künefe",
            Price = 180,
            CategoryId = cat.Id,
            Calories = 480,
            SelectedAllergens = new[] { "Gluten", "Süt" }
        };

        var createResult = controller.Create(createModel);
        var redirectResult = Assert.IsType<RedirectToActionResult>(createResult);
        Assert.Equal("Index", redirectResult.ActionName);

        var savedItem = context.MenuItems.FirstOrDefault(i => i.Name == "Künefe");
        Assert.NotNull(savedItem);
        Assert.Equal(480, savedItem.Calories);
        Assert.Equal("Gluten, Süt", savedItem.Allergens);

        // Edit
        var editModel = new MenuItemInputModel
        {
            Id = savedItem.Id,
            Name = "Künefe (Kaymaklı)",
            Description = "Özel kaymaklı",
            Price = 220,
            CategoryId = cat.Id,
            Calories = 620,
            SelectedAllergens = new[] { "Gluten", "Süt", "Fındık/Ceviz" }
        };

        var editResult = controller.Edit(editModel);
        Assert.IsType<RedirectToActionResult>(editResult);

        var updatedItem = context.MenuItems.Find(savedItem.Id);
        Assert.NotNull(updatedItem);
        Assert.Equal("Künefe (Kaymaklı)", updatedItem.Name);
        Assert.Equal(620, updatedItem.Calories);
        Assert.Equal("Gluten, Süt, Fındık/Ceviz", updatedItem.Allergens);
    }

    [Fact]
    public void RestaurantMenuItemController_CreateAndEdit_PersistsCaloriesAndAllergensWithTenantIsolation()
    {
        using var context = TestSupport.CreateContext();
        var services = TestSupport.Services(context);
        var auditContext = TestSupport.CreateAuditContext(context);
        var storage = new TestStorageService();

        var rest = new Restaurant { Name = "Antakya Sofrası", Slug = "antakya-sofrasi" };
        context.Restaurants.Add(rest);
        context.SaveChanges();

        var menu = new Menu { RestaurantId = rest.Id };
        context.Menus.Add(menu);
        context.SaveChanges();

        var cat = new Category { Name = "Mezeler", MenuId = menu.Id };
        context.Categories.Add(cat);
        context.SaveChanges();

        var controller = new dijitalmenu.Areas.Restaurant.Controllers.MenuItemController(
            services.Items, services.Categories, services.Menus, storage, auditContext)
        {
            ControllerContext = TestSupport.ControllerContext(new()
            {
                ["RestaurantId"] = rest.Id.ToString(),
                ["RestaurantUsername"] = "antakya"
            })
        };
        controller.TempData = TestSupport.TempData(controller.HttpContext);

        // Create
        var createModel = new MenuItemInputModel
        {
            Name = "Humus",
            Description = "Tahinli sıcak humus",
            Price = 140,
            CategoryId = cat.Id,
            Calories = 350,
            SelectedAllergens = new[] { "Susam" }
        };

        var createResult = controller.Create(createModel, null);
        var redirect = Assert.IsType<RedirectToActionResult>(createResult);
        Assert.Equal("Index", redirect.ActionName);

        var created = context.MenuItems.FirstOrDefault(i => i.Name == "Humus");
        Assert.NotNull(created);
        Assert.Equal(350, created.Calories);
        Assert.Equal("Susam", created.Allergens);

        // Edit
        var editModel = new MenuItemInputModel
        {
            Id = created.Id,
            Name = "Tereyağlı Humus",
            Description = "Kızgın tereyağı ile",
            Price = 175,
            CategoryId = cat.Id,
            Calories = 450,
            SelectedAllergens = new[] { "Susam", "Süt" }
        };

        var editResult = controller.Edit(editModel, null);
        Assert.IsType<RedirectToActionResult>(editResult);

        var updated = context.MenuItems.Find(created.Id);
        Assert.NotNull(updated);
        Assert.Equal(450, updated.Calories);
        Assert.Equal("Susam, Süt", updated.Allergens);
    }
}
