using BusinessLayer.Concrete;
using dijitalmenu.Areas.Restaurant.Controllers;
using dijitalmenu.Helpers;
using dijitalmenu.Models;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DijitalMenu.Tests;

public class RestaurantAccountTests
{
    [Fact]
    public void Account_Index_ReturnsOwnProfileAndBusinessData()
    {
        using var context = TestSupport.CreateContext();
        var services = TestSupport.Services(context);
        context.Themes.Add(new Theme { Name = "Test", PrimaryColor = "#000", SecondaryColor = "#111", BackgroundColor = "#fff", FontFamily = "Arial" });
        
        var restaurant = new Restaurant { Name = "Gaziantep Sofrası", Slug = "gaziantep-sofrasi", Phone = "05551234567", Address = "Kadıköy", ThemeId = 1 };
        context.Restaurants.Add(restaurant);
        context.SaveChanges();

        var user = new User { Username = "gaziantep_admin", Password = PasswordHelper.Hash("StrongPassword123!"), RestaurantId = restaurant.Id };
        context.Users.Add(user);
        context.SaveChanges();

        var controller = new AccountController(services.Users, services.Restaurants)
        {
            ControllerContext = TestSupport.ControllerContext(new()
            {
                ["RestaurantUserId"] = user.Id.ToString(),
                ["RestaurantId"] = restaurant.Id.ToString(),
                ["RestaurantUsername"] = user.Username
            })
        };

        var result = controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<RestaurantAccountViewModel>(viewResult.Model);
        Assert.Equal("gaziantep_admin", model.Username);
        Assert.Equal("Gaziantep Sofrası", model.RestaurantName);
        Assert.Equal("gaziantep-sofrasi", model.Slug);
        Assert.Equal("05551234567", model.Phone);
        Assert.Equal("Kadıköy", model.Address);
    }

    [Fact]
    public void Account_UpdateProfile_UpdatesUsernameAndSession()
    {
        using var context = TestSupport.CreateContext();
        var services = TestSupport.Services(context);
        context.Themes.Add(new Theme { Name = "Test", PrimaryColor = "#000", SecondaryColor = "#111", BackgroundColor = "#fff", FontFamily = "Arial" });
        
        var restaurant = new Restaurant { Name = "Eski Ad", Slug = "eski-ad", ThemeId = 1 };
        context.Restaurants.Add(restaurant);
        context.SaveChanges();

        var user = new User { Username = "old_user", Password = PasswordHelper.Hash("StrongPassword123!"), RestaurantId = restaurant.Id };
        context.Users.Add(user);
        context.SaveChanges();

        var controllerContext = TestSupport.ControllerContext(new()
        {
            ["RestaurantUserId"] = user.Id.ToString(),
            ["RestaurantId"] = restaurant.Id.ToString(),
            ["RestaurantUsername"] = "old_user"
        });

        var controller = new AccountController(services.Users, services.Restaurants)
        {
            ControllerContext = controllerContext,
            TempData = TestSupport.TempData(controllerContext.HttpContext)
        };

        var result = controller.UpdateProfile("new_user_name");

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("new_user_name", context.Users.Find(user.Id)!.Username);
        Assert.Equal("new_user_name", controllerContext.HttpContext.Session.GetString("RestaurantUsername"));
        Assert.NotNull(controller.TempData["Success"]);
    }

    [Fact]
    public void Account_UpdateProfile_RejectsDuplicateUsername()
    {
        using var context = TestSupport.CreateContext();
        var services = TestSupport.Services(context);
        context.Themes.Add(new Theme { Name = "Test", PrimaryColor = "#000", SecondaryColor = "#111", BackgroundColor = "#fff", FontFamily = "Arial" });
        
        var r1 = new Restaurant { Name = "R1", ThemeId = 1 };
        var r2 = new Restaurant { Name = "R2", ThemeId = 1 };
        context.Restaurants.AddRange(r1, r2);
        context.SaveChanges();

        var user1 = new User { Username = "existing_user", Password = PasswordHelper.Hash("StrongPassword123!"), RestaurantId = r1.Id };
        var user2 = new User { Username = "my_user", Password = PasswordHelper.Hash("StrongPassword123!"), RestaurantId = r2.Id };
        context.Users.AddRange(user1, user2);
        context.SaveChanges();

        var controllerContext = TestSupport.ControllerContext(new()
        {
            ["RestaurantUserId"] = user2.Id.ToString(),
            ["RestaurantId"] = r2.Id.ToString()
        });

        var controller = new AccountController(services.Users, services.Restaurants)
        {
            ControllerContext = controllerContext,
            TempData = TestSupport.TempData(controllerContext.HttpContext)
        };

        var result = controller.UpdateProfile("existing_user");

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("my_user", context.Users.Find(user2.Id)!.Username);
        Assert.NotNull(controller.TempData["Error"]);
    }

    [Fact]
    public void Account_UpdateBusiness_UpdatesRestaurantDetails()
    {
        using var context = TestSupport.CreateContext();
        var services = TestSupport.Services(context);
        context.Themes.Add(new Theme { Name = "Test", PrimaryColor = "#000", SecondaryColor = "#111", BackgroundColor = "#fff", FontFamily = "Arial" });
        
        var restaurant = new Restaurant { Name = "Eski Restoran", Phone = "02120000000", ThemeId = 1 };
        context.Restaurants.Add(restaurant);
        context.SaveChanges();

        var user = new User { Username = "rest_admin", Password = PasswordHelper.Hash("StrongPassword123!"), RestaurantId = restaurant.Id };
        context.Users.Add(user);
        context.SaveChanges();

        var controllerContext = TestSupport.ControllerContext(new()
        {
            ["RestaurantUserId"] = user.Id.ToString(),
            ["RestaurantId"] = restaurant.Id.ToString()
        });

        var controller = new AccountController(services.Users, services.Restaurants)
        {
            ControllerContext = controllerContext,
            TempData = TestSupport.TempData(controllerContext.HttpContext)
        };

        var result = controller.UpdateBusiness("Yeni Lezzet Restoranı", "05321112233", "Beşiktaş Çarşı No:10", "https://maps.google.com/maps?q=test");

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);

        var updated = context.Restaurants.Find(restaurant.Id)!;
        Assert.Equal("Yeni Lezzet Restoranı", updated.Name);
        Assert.Equal("05321112233", updated.Phone);
        Assert.Equal("Beşiktaş Çarşı No:10", updated.Address);
        Assert.Equal("https://maps.google.com/maps?q=test", updated.GoogleMapsUrl);
    }

    [Fact]
    public void Account_TenantIsolation_CannotModifyAnotherRestaurantsData()
    {
        using var context = TestSupport.CreateContext();
        var services = TestSupport.Services(context);
        context.Themes.Add(new Theme { Name = "Test", PrimaryColor = "#000", SecondaryColor = "#111", BackgroundColor = "#fff", FontFamily = "Arial" });
        
        var victimRestaurant = new Restaurant { Name = "Victim Restaurant", Phone = "05001112233", ThemeId = 1 };
        var attackerRestaurant = new Restaurant { Name = "Attacker Restaurant", Phone = "05009998877", ThemeId = 1 };
        context.Restaurants.AddRange(victimRestaurant, attackerRestaurant);
        context.SaveChanges();

        var victimUser = new User { Username = "victim", Password = PasswordHelper.Hash("VictimPass1234!"), RestaurantId = victimRestaurant.Id };
        var attackerUser = new User { Username = "attacker", Password = PasswordHelper.Hash("AttackerPass1234!"), RestaurantId = attackerRestaurant.Id };
        context.Users.AddRange(victimUser, attackerUser);
        context.SaveChanges();

        // Attacker is logged in with Attacker's session
        var controllerContext = TestSupport.ControllerContext(new()
        {
            ["RestaurantUserId"] = attackerUser.Id.ToString(),
            ["RestaurantId"] = attackerRestaurant.Id.ToString()
        });

        var controller = new AccountController(services.Users, services.Restaurants)
        {
            ControllerContext = controllerContext,
            TempData = TestSupport.TempData(controllerContext.HttpContext)
        };

        controller.UpdateBusiness("Hijacked By Attacker", "05000000000", "Hijacked Address", null);

        // Victim restaurant must remain completely untouched
        var victimDb = context.Restaurants.Find(victimRestaurant.Id)!;
        Assert.Equal("Victim Restaurant", victimDb.Name);
        Assert.Equal("05001112233", victimDb.Phone);

        // Only attacker's own restaurant is updated
        var attackerDb = context.Restaurants.Find(attackerRestaurant.Id)!;
        Assert.Equal("Hijacked By Attacker", attackerDb.Name);
    }

    [Fact]
    public void Account_ChangePassword_AcceptsCorrectCurrentPassword_AndHashesWithBCrypt()
    {
        using var context = TestSupport.CreateContext();
        var services = TestSupport.Services(context);
        context.Themes.Add(new Theme { Name = "Test", PrimaryColor = "#000", SecondaryColor = "#111", BackgroundColor = "#fff", FontFamily = "Arial" });
        
        var restaurant = new Restaurant { Name = "R", ThemeId = 1 };
        context.Restaurants.Add(restaurant);
        context.SaveChanges();

        var oldPassword = "OldPassword123!";
        var user = new User { Username = "user_pass", Password = PasswordHelper.Hash(oldPassword), RestaurantId = restaurant.Id };
        context.Users.Add(user);
        context.SaveChanges();

        var controllerContext = TestSupport.ControllerContext(new()
        {
            ["RestaurantUserId"] = user.Id.ToString(),
            ["RestaurantId"] = restaurant.Id.ToString()
        });

        var controller = new AccountController(services.Users, services.Restaurants)
        {
            ControllerContext = controllerContext,
            TempData = TestSupport.TempData(controllerContext.HttpContext)
        };

        var newPassword = "NewStrongPassword2026!#";
        var result = controller.ChangePassword(oldPassword, newPassword, newPassword);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);

        var updatedUser = context.Users.Find(user.Id)!;
        Assert.StartsWith("$2", updatedUser.Password);
        Assert.True(PasswordHelper.Verify(newPassword, updatedUser.Password));
        Assert.False(PasswordHelper.Verify(oldPassword, updatedUser.Password));
    }

    [Fact]
    public void Account_ChangePassword_RejectsWrongCurrentPassword()
    {
        using var context = TestSupport.CreateContext();
        var services = TestSupport.Services(context);
        context.Themes.Add(new Theme { Name = "Test", PrimaryColor = "#000", SecondaryColor = "#111", BackgroundColor = "#fff", FontFamily = "Arial" });
        
        var restaurant = new Restaurant { Name = "R", ThemeId = 1 };
        context.Restaurants.Add(restaurant);
        context.SaveChanges();

        var initialHash = PasswordHelper.Hash("CorrectOldPassword123!");
        var user = new User { Username = "user_pass2", Password = initialHash, RestaurantId = restaurant.Id };
        context.Users.Add(user);
        context.SaveChanges();

        var controllerContext = TestSupport.ControllerContext(new()
        {
            ["RestaurantUserId"] = user.Id.ToString(),
            ["RestaurantId"] = restaurant.Id.ToString()
        });

        var controller = new AccountController(services.Users, services.Restaurants)
        {
            ControllerContext = controllerContext,
            TempData = TestSupport.TempData(controllerContext.HttpContext)
        };

        var result = controller.ChangePassword("WrongPassword123!", "NewStrongPassword2026!#", "NewStrongPassword2026!#");

        Assert.IsType<RedirectToActionResult>(result);
        var notUpdatedUser = context.Users.Find(user.Id)!;
        Assert.Equal(initialHash, notUpdatedUser.Password);
        Assert.NotNull(controller.TempData["Error"]);
    }

    [Fact]
    public void Account_ChangePassword_RejectsWeakOrMismatchedPassword()
    {
        using var context = TestSupport.CreateContext();
        var services = TestSupport.Services(context);
        context.Themes.Add(new Theme { Name = "Test", PrimaryColor = "#000", SecondaryColor = "#111", BackgroundColor = "#fff", FontFamily = "Arial" });
        
        var restaurant = new Restaurant { Name = "R", ThemeId = 1 };
        context.Restaurants.Add(restaurant);
        context.SaveChanges();

        var initialHash = PasswordHelper.Hash("OldStrongPassword123!");
        var user = new User { Username = "user_pass3", Password = initialHash, RestaurantId = restaurant.Id };
        context.Users.Add(user);
        context.SaveChanges();

        var controllerContext = TestSupport.ControllerContext(new()
        {
            ["RestaurantUserId"] = user.Id.ToString(),
            ["RestaurantId"] = restaurant.Id.ToString()
        });

        var controller = new AccountController(services.Users, services.Restaurants)
        {
            ControllerContext = controllerContext,
            TempData = TestSupport.TempData(controllerContext.HttpContext)
        };

        // 1. Mismatched confirmation
        controller.ChangePassword("OldStrongPassword123!", "NewStrongPassword2026!#", "DifferentPassword123!");
        Assert.Equal(initialHash, context.Users.Find(user.Id)!.Password);

        // 2. Short password (< 12 chars)
        controller.ChangePassword("OldStrongPassword123!", "Short1!", "Short1!");
        Assert.Equal(initialHash, context.Users.Find(user.Id)!.Password);

        // 3. No special character
        controller.ChangePassword("OldStrongPassword123!", "NoSpecialChar12345", "NoSpecialChar12345");
        Assert.Equal(initialHash, context.Users.Find(user.Id)!.Password);

        // 4. Same as current password
        controller.ChangePassword("OldStrongPassword123!", "OldStrongPassword123!", "OldStrongPassword123!");
        Assert.Equal(initialHash, context.Users.Find(user.Id)!.Password);
    }
}
