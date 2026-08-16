using BusinessLayer.Concrete;
using dijitalmenu.Areas.Restaurant.Controllers;
using dijitalmenu.Filters;
using dijitalmenu.Helpers;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Xunit;

namespace DijitalMenu.Tests;

public class RestaurantWorkflowTests
{
    [Fact]
    public void PasswordHelper_HashesAndVerifiesPassword()
    {
        var hash = PasswordHelper.Hash("StrongPassword1!");
        Assert.True(PasswordHelper.Verify("StrongPassword1!", hash));
        Assert.False(PasswordHelper.Verify("wrong", hash));
        Assert.False(PasswordHelper.NeedsRehash(hash));
        Assert.True(PasswordHelper.NeedsRehash("legacy"));
    }

    [Fact]
    public void RestaurantAuthFilter_RedirectsAndClearsInvalidSession()
    {
        var context = TestSupport.ControllerContext(new() { ["RestaurantUserId"] = "abc", ["RestaurantId"] = "5" });
        var actionContext = new ActionContext(context.HttpContext, new RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
        var executing = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object?>(), new object());

        new RestaurantAuthFilter().OnActionExecuting(executing);

        Assert.IsType<RedirectToRouteResult>(executing.Result);
        Assert.Empty(context.HttpContext.Session.Keys);
    }

    [Fact]
    public void MenuItemCreate_AcceptsSelectedOwnedCategory()
    {
        using var context = TestSupport.CreateContext();
        var services = TestSupport.Services(context);
        var restaurant = new Restaurant { Name = "Test Restaurant", ThemeId = 1 };
        context.Themes.Add(new Theme { Name = "Test", PrimaryColor = "#000", SecondaryColor = "#111", BackgroundColor = "#fff", FontFamily = "Arial" });
        context.Restaurants.Add(restaurant); context.SaveChanges();
        var menu = new Menu { RestaurantId = restaurant.Id }; context.Menus.Add(menu); context.SaveChanges();
        var category = new Category { Name = "Ana Yemek", MenuId = menu.Id }; context.Categories.Add(category); context.SaveChanges();
        var controller = new MenuItemController(services.Items, services.Categories, services.Menus, new TestWebHostEnvironment())
        {
            ControllerContext = TestSupport.ControllerContext(new() { ["RestaurantId"] = restaurant.Id.ToString() })
        };
        controller.TempData = TestSupport.TempData(controller.HttpContext);

        var result = controller.Create(new MenuItem { Name = "Kebap", Description = "Test", Price = 250m, CategoryId = category.Id }, null);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Single(context.MenuItems);
        Assert.Equal(category.Id, context.MenuItems.Single().CategoryId);
    }

    [Fact]
    public void MenuItemEdit_CannotModifyAnotherRestaurantsItem()
    {
        using var context = TestSupport.CreateContext();
        var services = TestSupport.Services(context);
        context.Themes.Add(new Theme { Name = "Test", PrimaryColor = "#000", SecondaryColor = "#111", BackgroundColor = "#fff", FontFamily = "Arial" });
        var owner = new Restaurant { Name = "Owner", ThemeId = 1 }; var attacker = new Restaurant { Name = "Attacker", ThemeId = 1 };
        context.Restaurants.AddRange(owner, attacker); context.SaveChanges();
        var ownerMenu = new Menu { RestaurantId = owner.Id }; var attackerMenu = new Menu { RestaurantId = attacker.Id };
        context.Menus.AddRange(ownerMenu, attackerMenu); context.SaveChanges();
        var ownerCategory = new Category { Name = "Owner", MenuId = ownerMenu.Id }; var attackerCategory = new Category { Name = "Attacker", MenuId = attackerMenu.Id };
        context.Categories.AddRange(ownerCategory, attackerCategory); context.SaveChanges();
        var target = new MenuItem { Name = "Original", Description = "", Price = 10, CategoryId = ownerCategory.Id };
        context.MenuItems.Add(target); context.SaveChanges();
        var controller = new MenuItemController(services.Items, services.Categories, services.Menus, new TestWebHostEnvironment()) { ControllerContext = TestSupport.ControllerContext(new() { ["RestaurantId"] = attacker.Id.ToString() }) };
        controller.TempData = TestSupport.TempData(controller.HttpContext);

        controller.Edit(new MenuItem { Id = target.Id, Name = "Hijacked", Price = 50, CategoryId = attackerCategory.Id }, null);

        Assert.Equal("Original", context.MenuItems.Single().Name);
        Assert.Equal(ownerCategory.Id, context.MenuItems.Single().CategoryId);
    }

    [Fact]
    public void CategoryCreate_RejectsDuplicateNameIgnoringCase()
    {
        using var context = TestSupport.CreateContext();
        var services = TestSupport.Services(context);
        context.Themes.Add(new Theme { Name = "Test", PrimaryColor = "#000", SecondaryColor = "#111", BackgroundColor = "#fff", FontFamily = "Arial" });
        var restaurant = new Restaurant { Name = "Test Restaurant", ThemeId = 1 }; context.Restaurants.Add(restaurant); context.SaveChanges();
        var menu = new Menu { RestaurantId = restaurant.Id }; context.Menus.Add(menu); context.SaveChanges();
        context.Categories.Add(new Category { Name = "İçecek", MenuId = menu.Id }); context.SaveChanges();
        var controller = new CategoryController(services.Categories, services.Menus, new TestWebHostEnvironment()) { ControllerContext = TestSupport.ControllerContext(new() { ["RestaurantId"] = restaurant.Id.ToString() }) };

        var result = controller.Create("içecek", null);

        Assert.IsType<ViewResult>(result);
        Assert.Single(context.Categories);
    }

    [Fact]
    public void BuilderController_UpdateCategoryOrder_ReordersCategoriesCorrectly()
    {
        using var context = TestSupport.CreateContext();
        var services = TestSupport.Services(context);
        context.Themes.Add(new Theme { Name = "Test", PrimaryColor = "#000", SecondaryColor = "#111", BackgroundColor = "#fff", FontFamily = "Arial" });
        var restaurant = new Restaurant { Name = "Test Restaurant", ThemeId = 1 };
        context.Restaurants.Add(restaurant); context.SaveChanges();
        var menu = new Menu { RestaurantId = restaurant.Id }; context.Menus.Add(menu); context.SaveChanges();
        var cat1 = new Category { Name = "Cat 1", MenuId = menu.Id, DisplayOrder = 0 };
        var cat2 = new Category { Name = "Cat 2", MenuId = menu.Id, DisplayOrder = 1 };
        var cat3 = new Category { Name = "Cat 3", MenuId = menu.Id, DisplayOrder = 2 };
        context.Categories.AddRange(cat1, cat2, cat3); context.SaveChanges();

        var suggestionService = new CategorySuggestionManager();
        var controller = new BuilderController(services.Restaurants, services.Menus, services.Categories, services.Items, services.Themes, suggestionService)
        {
            ControllerContext = TestSupport.ControllerContext(new() { ["RestaurantId"] = restaurant.Id.ToString() })
        };

        var result = controller.UpdateCategoryOrder(new List<int> { cat3.Id, cat1.Id, cat2.Id });

        Assert.IsType<JsonResult>(result);
        Assert.Equal(0, context.Categories.Find(cat3.Id)!.DisplayOrder);
        Assert.Equal(1, context.Categories.Find(cat1.Id)!.DisplayOrder);
        Assert.Equal(2, context.Categories.Find(cat2.Id)!.DisplayOrder);
    }

    [Fact]
    public void BuilderController_UpdateMenuItemOrder_ReordersMenuItemsCorrectly()
    {
        using var context = TestSupport.CreateContext();
        var services = TestSupport.Services(context);
        context.Themes.Add(new Theme { Name = "Test", PrimaryColor = "#000", SecondaryColor = "#111", BackgroundColor = "#fff", FontFamily = "Arial" });
        var restaurant = new Restaurant { Name = "Test Restaurant", ThemeId = 1 };
        context.Restaurants.Add(restaurant); context.SaveChanges();
        var menu = new Menu { RestaurantId = restaurant.Id }; context.Menus.Add(menu); context.SaveChanges();
        var cat = new Category { Name = "Cat 1", MenuId = menu.Id };
        context.Categories.Add(cat); context.SaveChanges();
        var item1 = new MenuItem { Name = "Item 1", CategoryId = cat.Id, Price = 10, DisplayOrder = 0 };
        var item2 = new MenuItem { Name = "Item 2", CategoryId = cat.Id, Price = 20, DisplayOrder = 1 };
        context.MenuItems.AddRange(item1, item2); context.SaveChanges();

        var suggestionService = new CategorySuggestionManager();
        var controller = new BuilderController(services.Restaurants, services.Menus, services.Categories, services.Items, services.Themes, suggestionService)
        {
            ControllerContext = TestSupport.ControllerContext(new() { ["RestaurantId"] = restaurant.Id.ToString() })
        };

        var result = controller.UpdateMenuItemOrder(new List<int> { item2.Id, item1.Id });

        Assert.IsType<JsonResult>(result);
        Assert.Equal(0, context.MenuItems.Find(item2.Id)!.DisplayOrder);
        Assert.Equal(1, context.MenuItems.Find(item1.Id)!.DisplayOrder);
    }

    [Fact]
    public void BuilderController_UpdateLocation_NormalizesAndSavesInstagramUrl()
    {
        using var context = TestSupport.CreateContext();
        var services = TestSupport.Services(context);
        context.Themes.Add(new Theme { Name = "Test", PrimaryColor = "#000", SecondaryColor = "#111", BackgroundColor = "#fff", FontFamily = "Arial" });
        var restaurant = new Restaurant { Name = "Test Restaurant", ThemeId = 1 };
        context.Restaurants.Add(restaurant); context.SaveChanges();

        var suggestionService = new CategorySuggestionManager();
        var controller = new BuilderController(services.Restaurants, services.Menus, services.Categories, services.Items, services.Themes, suggestionService)
        {
            ControllerContext = TestSupport.ControllerContext(new() { ["RestaurantId"] = restaurant.Id.ToString() })
        };

        var result = controller.UpdateLocation(null, "Test Adres", "+90 555 123 45 67", "09:00 - 22:00", "@kocaoglurestoran");

        Assert.IsType<JsonResult>(result);
        var updated = context.Restaurants.Find(restaurant.Id);
        Assert.NotNull(updated);
        Assert.Equal("https://instagram.com/kocaoglurestoran", updated.InstagramUrl);
    }
}
