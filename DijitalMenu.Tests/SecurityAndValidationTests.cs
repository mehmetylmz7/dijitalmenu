using BusinessLayer.Concrete;
using dijitalmenu.Filters;
using dijitalmenu.Helpers;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using RestaurantArea = dijitalmenu.Areas.Restaurant.Controllers;
using Xunit;

namespace DijitalMenu.Tests;

public class SecurityAndValidationTests
{
    [Fact]
    public void AdminAuthFilter_RedirectsUnauthenticatedUserToLogin()
    {
        var context = TestSupport.ControllerContext(); // empty session
        var actionContext = new ActionContext(context.HttpContext, new RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
        var executing = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object?>(), new object());

        new AdminAuthFilter().OnActionExecuting(executing);

        var redirect = Assert.IsType<RedirectToRouteResult>(executing.Result);
        Assert.Equal("Admin", redirect.RouteValues?["area"]);
        Assert.Equal("Auth", redirect.RouteValues?["controller"]);
        Assert.Equal("Login", redirect.RouteValues?["action"]);
    }

    [Fact]
    public void AdminAuthFilter_AllowsAuthenticatedAdmin()
    {
        var context = TestSupport.ControllerContext(new() { ["AdminUser"] = "superadmin" });
        var actionContext = new ActionContext(context.HttpContext, new RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
        var executing = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object?>(), new object());

        new AdminAuthFilter().OnActionExecuting(executing);

        Assert.Null(executing.Result);
    }

    [Fact]
    public void PasswordHelper_RejectsPlainTextPasswords()
    {
        // Stored password is plain text, not a BCrypt hash ($2...)
        var plainStored = "PlainPassword123!";
        
        // Plain text should be rejected for authentication
        Assert.False(PasswordHelper.Verify(plainStored, plainStored));
        Assert.True(PasswordHelper.NeedsRehash(plainStored));
    }

    [Fact]
    public void PasswordHelper_UpgradesLegacyHashOnSuccessfulLogin()
    {
        // Simulate a legacy hash with work factor 10 (valid bcrypt format)
        var legacyHash = BCrypt.Net.BCrypt.HashPassword("TestPass123!", 10);
        
        // Should verify successfully
        Assert.True(PasswordHelper.Verify("TestPass123!", legacyHash));
        
        // Should identify that it needs rehashing to work factor 12
        Assert.True(PasswordHelper.NeedsRehash(legacyHash));
        
        // New hash with default work factor 12
        var upgradedHash = PasswordHelper.Hash("TestPass123!");
        Assert.False(PasswordHelper.NeedsRehash(upgradedHash));
    }

    [Fact]
    public void MenuItemDelete_CannotDeleteAnotherRestaurantsItem()
    {
        using var context = TestSupport.CreateContext();
        var services = TestSupport.Services(context);
        context.Themes.Add(new Theme { Name = "Test", PrimaryColor = "#000", SecondaryColor = "#111", BackgroundColor = "#fff", FontFamily = "Arial" });
        var owner = new Restaurant { Name = "Owner", ThemeId = 1 };
        var attacker = new Restaurant { Name = "Attacker", ThemeId = 1 };
        context.Restaurants.AddRange(owner, attacker);
        context.SaveChanges();

        var ownerMenu = new Menu { RestaurantId = owner.Id };
        var attackerMenu = new Menu { RestaurantId = attacker.Id };
        context.Menus.AddRange(ownerMenu, attackerMenu);
        context.SaveChanges();

        var ownerCat = new Category { Name = "Owner Cat", MenuId = ownerMenu.Id };
        var attackerCat = new Category { Name = "Attacker Cat", MenuId = attackerMenu.Id };
        context.Categories.AddRange(ownerCat, attackerCat);
        context.SaveChanges();

        var targetItem = new MenuItem { Name = "Owner Dish", Description = "Secret Recipe", Price = 100, CategoryId = ownerCat.Id };
        context.MenuItems.Add(targetItem);
        context.SaveChanges();

        var httpContext = TestSupport.ControllerContext(new() { ["RestaurantId"] = attacker.Id.ToString() });
        var auditContext = TestSupport.CreateAuditContext(context, httpContext.HttpContext);
        var controller = new RestaurantArea.MenuItemController(services.Items, services.Categories, services.Menus, new TestWebHostEnvironment(), auditContext)
        {
            ControllerContext = httpContext
        };

        controller.Delete(targetItem.Id);

        // Target item should NOT be deleted because attacker does not own the category/menu
        Assert.Single(context.MenuItems);
        Assert.Equal("Owner Dish", context.MenuItems.Single().Name);
    }

    [Fact]
    public void CategoryDelete_CannotDeleteAnotherRestaurantsCategory()
    {
        using var context = TestSupport.CreateContext();
        var services = TestSupport.Services(context);
        context.Themes.Add(new Theme { Name = "Test", PrimaryColor = "#000", SecondaryColor = "#111", BackgroundColor = "#fff", FontFamily = "Arial" });
        var owner = new Restaurant { Name = "Owner", ThemeId = 1 };
        var attacker = new Restaurant { Name = "Attacker", ThemeId = 1 };
        context.Restaurants.AddRange(owner, attacker);
        context.SaveChanges();

        var ownerMenu = new Menu { RestaurantId = owner.Id };
        var attackerMenu = new Menu { RestaurantId = attacker.Id };
        context.Menus.AddRange(ownerMenu, attackerMenu);
        context.SaveChanges();

        var ownerCat = new Category { Name = "Owner Cat", MenuId = ownerMenu.Id };
        context.Categories.Add(ownerCat);
        context.SaveChanges();

        var httpContext = TestSupport.ControllerContext(new() { ["RestaurantId"] = attacker.Id.ToString() });
        var auditContext = TestSupport.CreateAuditContext(context, httpContext.HttpContext);
        var controller = new RestaurantArea.CategoryController(services.Categories, services.Menus, new TestWebHostEnvironment(), auditContext)
        {
            ControllerContext = httpContext
        };

        controller.Delete(ownerCat.Id);

        // Owner category must still exist
        Assert.Single(context.Categories);
        Assert.Equal("Owner Cat", context.Categories.Single().Name);
    }

    [Fact]
    public void RestaurantModel_HasUniqueIndexOnSlug()
    {
        using var context = TestSupport.CreateContext();
        var entityType = context.Model.FindEntityType(typeof(Restaurant));
        Assert.NotNull(entityType);

        var index = entityType.GetIndexes().FirstOrDefault(i => i.Properties.Any(p => p.Name == "Slug"));
        Assert.NotNull(index);
        Assert.True(index.IsUnique);
    }
}
