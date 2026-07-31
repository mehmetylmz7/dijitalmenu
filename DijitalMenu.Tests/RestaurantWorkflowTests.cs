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
        var controller = new CategoryController(services.Categories, services.Menus) { ControllerContext = TestSupport.ControllerContext(new() { ["RestaurantId"] = restaurant.Id.ToString() }) };

        var result = controller.Create("içecek");

        Assert.IsType<ViewResult>(result);
        Assert.Single(context.Categories);
    }
}
