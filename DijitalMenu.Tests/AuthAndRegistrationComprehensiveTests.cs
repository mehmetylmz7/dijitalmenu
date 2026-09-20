using BusinessLayer.Concrete;
using DataAccessLayer.Repositories;
using dijitalmenu.Areas.Admin.Controllers;
using dijitalmenu.Helpers;
using dijitalmenu.Services;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Xunit;
using AdminAuthController = dijitalmenu.Areas.Admin.Controllers.AuthController;
using RestaurantAuthController = dijitalmenu.Areas.Restaurant.Controllers.AuthController;

namespace DijitalMenu.Tests;

public class AuthAndRegistrationComprehensiveTests
{
    private const string StrongPassword = "StrongPassword123!";

    private static (RestaurantAuthController controller, TestSession session, DataAccessLayer.Concrete.Context context) CreateRestaurantAuthController(
        DataAccessLayer.Concrete.Context context,
        LoginAttemptService? loginAttemptService = null)
    {
        var services = TestSupport.AllServices(context);
        var defaultCategoryService = new DefaultCategoryManager(
            new DefaultCategoryRepository(context),
            new CategoryRepository(context),
            new MenuRepository(context)
        );

        var session = new TestSession();
        var httpContext = new DefaultHttpContext { Session = session };
        var auditContext = TestSupport.CreateAuditContext(context, httpContext);
        loginAttemptService ??= new LoginAttemptService();

        var controller = new RestaurantAuthController(
            services.Users,
            services.Restaurants,
            services.Menus,
            services.Themes,
            defaultCategoryService,
            auditContext,
            services.Notifications,
            context,
            loginAttemptService
        )
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext }
        };

        return (controller, session, context);
    }

    private static (AdminAuthController controller, TestSession session, DataAccessLayer.Concrete.Context context) CreateAdminAuthController(
        DataAccessLayer.Concrete.Context context,
        LoginAttemptService? loginAttemptService = null)
    {
        var services = TestSupport.AllServices(context);
        var session = new TestSession();
        var httpContext = new DefaultHttpContext { Session = session };
        var auditContext = TestSupport.CreateAuditContext(context, httpContext);
        loginAttemptService ??= new LoginAttemptService();

        var controller = new AdminAuthController(
            services.Admins,
            auditContext,
            loginAttemptService
        )
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext }
        };

        return (controller, session, context);
    }

    private static Theme SeedDefaultTheme(DataAccessLayer.Concrete.Context context)
    {
        var theme = new Theme
        {
            Name = "Modern Minimalist",
            PrimaryColor = "#18181b",
            SecondaryColor = "#71717a",
            BackgroundColor = "#ffffff",
            FontFamily = "Inter, sans-serif",
            Layout = LayoutType.List,
            IsActive = true
        };
        context.Themes.Add(theme);
        context.SaveChanges();
        return theme;
    }

    // ==========================================
    // RESTAURANT REGISTER TESTS
    // ==========================================

    [Fact]
    public void Register_Success_GeneratesUniqueSlug_HashesPassword_SetsUpMenu_AndSignsIn()
    {
        using var context = TestSupport.CreateContext();
        var defaultTheme = SeedDefaultTheme(context);
        context.DefaultCategories.Add(new DefaultCategory { Name = "Ana Yemekler" });
        context.DefaultCategories.Add(new DefaultCategory { Name = "İçecekler" });
        context.SaveChanges();

        var (controller, session, _) = CreateRestaurantAuthController(context);

        var result = controller.Register(
            restaurantName: "  Boğaziçi Balık & Meze Evi  ",
            username: "  bogazici_admin  ",
            password: StrongPassword,
            themeId: defaultTheme.Id
        );

        // 1. Redirection to Dashboard
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Dashboard", redirect.ControllerName);
        Assert.Equal("Restaurant", redirect.RouteValues?["area"]);

        // 2. Restaurant persisted with cleaned, unique slug
        var restaurant = context.Restaurants.FirstOrDefault(r => r.Name == "Boğaziçi Balık & Meze Evi");
        Assert.NotNull(restaurant);
        Assert.Equal("bogazici-balik-meze-evi", restaurant.Slug);
        Assert.Equal(defaultTheme.Id, restaurant.ThemeId);

        // 3. User persisted with hashed password
        var user = context.Users.FirstOrDefault(u => u.Username == "bogazici_admin");
        Assert.NotNull(user);
        Assert.Equal(restaurant.Id, user.RestaurantId);
        Assert.True(PasswordHelper.Verify(StrongPassword, user.Password));

        // 4. Menu & default categories automatically created
        var menu = context.Menus.FirstOrDefault(m => m.RestaurantId == restaurant.Id);
        Assert.NotNull(menu);
        var categories = context.Categories.Where(c => c.MenuId == menu.Id).ToList();
        Assert.Contains(categories, c => c.Name == "Ana Yemekler");
        Assert.Contains(categories, c => c.Name == "İçecekler");

        // 5. Automatic Session Sign-in
        Assert.True(session.TryGetValue("RestaurantUserId", out var uidBytes));
        Assert.Equal(user.Id.ToString(), System.Text.Encoding.UTF8.GetString(uidBytes));
        Assert.True(session.TryGetValue("RestaurantId", out var ridBytes));
        Assert.Equal(restaurant.Id.ToString(), System.Text.Encoding.UTF8.GetString(ridBytes));
        Assert.True(session.TryGetValue("RestaurantUsername", out var uNameBytes));
        Assert.Equal("bogazici_admin", System.Text.Encoding.UTF8.GetString(uNameBytes));

        // 6. Admin Notification created
        var notification = context.Notifications.FirstOrDefault(n => n.RestaurantId == restaurant.Id);
        Assert.NotNull(notification);
        Assert.Contains("Boğaziçi Balık & Meze Evi", notification.Message);
    }

    [Fact]
    public void Register_ConsecutiveRegistrationsWithSameRestaurantName_AssignsIncrementedSlugs()
    {
        using var context = TestSupport.CreateContext();
        var theme = SeedDefaultTheme(context);

        var (controller1, _, _) = CreateRestaurantAuthController(context);
        var res1 = controller1.Register("Kocaoğlu Kebap", "kocaoglu1", StrongPassword, theme.Id);
        Assert.IsType<RedirectToActionResult>(res1);

        var (controller2, _, _) = CreateRestaurantAuthController(context);
        var res2 = controller2.Register("Kocaoğlu Kebap", "kocaoglu2", StrongPassword, theme.Id);
        Assert.IsType<RedirectToActionResult>(res2);

        var (controller3, _, _) = CreateRestaurantAuthController(context);
        var res3 = controller3.Register("Kocaoğlu Kebap", "kocaoglu3", StrongPassword, theme.Id);
        Assert.IsType<RedirectToActionResult>(res3);

        var r1 = context.Restaurants.FirstOrDefault(r => r.Users.Any(u => u.Username == "kocaoglu1"));
        var r2 = context.Restaurants.FirstOrDefault(r => r.Users.Any(u => u.Username == "kocaoglu2"));
        var r3 = context.Restaurants.FirstOrDefault(r => r.Users.Any(u => u.Username == "kocaoglu3"));

        Assert.NotNull(r1);
        Assert.NotNull(r2);
        Assert.NotNull(r3);

        Assert.Equal("kocaoglu-kebap", r1.Slug);
        Assert.Equal("kocaoglu-kebap-1", r2.Slug);
        Assert.Equal("kocaoglu-kebap-2", r3.Slug);
    }

    [Fact]
    public void Register_DuplicateUsername_CaseInsensitive_FailsWithErrorMessage()
    {
        using var context = TestSupport.CreateContext();
        var theme = SeedDefaultTheme(context);

        var (controller1, _, _) = CreateRestaurantAuthController(context);
        controller1.Register("Restoran A", "MehmetAdmin", StrongPassword, theme.Id);

        var (controller2, _, _) = CreateRestaurantAuthController(context);
        var result = controller2.Register("Restoran B", "mehmetadmin", StrongPassword, theme.Id);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Bu kullanıcı adı zaten alınmış.", viewResult.ViewData["Error"]);

        // Verify second restaurant was NOT created
        Assert.Null(context.Restaurants.FirstOrDefault(r => r.Name == "Restoran B"));
    }

    [Theory]
    [InlineData("", "validuser", StrongPassword, "Restoran adı 2 ile 100 karakter arasında olmalıdır.")]
    [InlineData("Valid Rest", "", StrongPassword, "Kullanıcı adı 3 ile 50 karakter arasında olmalı; yalnızca harf, rakam, nokta, alt çizgi ve tire içermelidir.")]
    [InlineData("Valid Rest", "ab", StrongPassword, "Kullanıcı adı 3 ile 50 karakter arasında olmalı; yalnızca harf, rakam, nokta, alt çizgi ve tire içermelidir.")]
    [InlineData("Valid Rest", "valid user", StrongPassword, "Kullanıcı adı 3 ile 50 karakter arasında olmalı; yalnızca harf, rakam, nokta, alt çizgi ve tire içermelidir.")]
    [InlineData("Valid Rest", "validuser", "", "Şifre en az 12 karakter olmalı; büyük harf, küçük harf, rakam ve özel karakter içermelidir.")]
    [InlineData("Valid Rest", "validuser", "Short1!", "Şifre en az 12 karakter olmalı; büyük harf, küçük harf, rakam ve özel karakter içermelidir.")]
    [InlineData("Valid Rest", "validuser", "nocapitalletter123!", "Şifre en az 12 karakter olmalı; büyük harf, küçük harf, rakam ve özel karakter içermelidir.")]
    [InlineData("Valid Rest", "validuser", "NOSMALLLETTERS123!", "Şifre en az 12 karakter olmalı; büyük harf, küçük harf, rakam ve özel karakter içermelidir.")]
    [InlineData("Valid Rest", "validuser", "NoSpecialCharacters123", "Şifre en az 12 karakter olmalı; büyük harf, küçük harf, rakam ve özel karakter içermelidir.")]
    public void Register_InvalidInputs_ReturnsValidationError(string restName, string username, string password, string expectedError)
    {
        using var context = TestSupport.CreateContext();
        SeedDefaultTheme(context);

        var (controller, _, _) = CreateRestaurantAuthController(context);

        var result = controller.Register(restName, username, password, themeId: 1);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(expectedError, viewResult.ViewData["Error"]);
    }

    // ==========================================
    // RESTAURANT LOGIN TESTS
    // ==========================================

    [Fact]
    public void Login_ValidCredentials_SignsInAndRedirectsToDashboard()
    {
        using var context = TestSupport.CreateContext();
        var theme = SeedDefaultTheme(context);
        var restaurant = new Restaurant { Name = "Test Restoran", Slug = "test-restoran", ThemeId = theme.Id };
        context.Restaurants.Add(restaurant);
        context.SaveChanges();

        var user = new User
        {
            Username = "test_owner",
            Password = PasswordHelper.Hash(StrongPassword),
            RestaurantId = restaurant.Id
        };
        context.Users.Add(user);
        context.SaveChanges();

        var (controller, session, _) = CreateRestaurantAuthController(context);

        var result = controller.Login("test_owner", StrongPassword);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Dashboard", redirect.ControllerName);

        // Session values populated
        Assert.True(session.TryGetValue("RestaurantUserId", out var uidBytes));
        Assert.Equal(user.Id.ToString(), System.Text.Encoding.UTF8.GetString(uidBytes));
        Assert.True(session.TryGetValue("RestaurantId", out var ridBytes));
        Assert.Equal(restaurant.Id.ToString(), System.Text.Encoding.UTF8.GetString(ridBytes));
        Assert.True(session.TryGetValue("RestaurantUsername", out var uNameBytes));
        Assert.Equal("test_owner", System.Text.Encoding.UTF8.GetString(uNameBytes));
    }

    [Fact]
    public void Login_WrongPassword_FailsWithErrorMessage()
    {
        using var context = TestSupport.CreateContext();
        var theme = SeedDefaultTheme(context);
        var restaurant = new Restaurant { Name = "Test Restoran", Slug = "test-restoran", ThemeId = theme.Id };
        context.Restaurants.Add(restaurant);
        context.SaveChanges();

        var user = new User
        {
            Username = "test_owner",
            Password = PasswordHelper.Hash(StrongPassword),
            RestaurantId = restaurant.Id
        };
        context.Users.Add(user);
        context.SaveChanges();

        var (controller, session, _) = CreateRestaurantAuthController(context);

        var result = controller.Login("test_owner", "WrongPassword123!");

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Kullanıcı adı veya şifre hatalı.", viewResult.ViewData["Error"]);
        Assert.False(session.TryGetValue("RestaurantUserId", out _));
    }

    [Fact]
    public void Login_BruteForceLockout_TriggersAfterMultipleFailedAttempts()
    {
        using var context = TestSupport.CreateContext();
        var theme = SeedDefaultTheme(context);
        var restaurant = new Restaurant { Name = "Test Restoran", Slug = "test-restoran", ThemeId = theme.Id };
        context.Restaurants.Add(restaurant);
        context.SaveChanges();

        var user = new User
        {
            Username = "brute_target",
            Password = PasswordHelper.Hash(StrongPassword),
            RestaurantId = restaurant.Id
        };
        context.Users.Add(user);
        context.SaveChanges();

        var attemptService = new LoginAttemptService();
        var (controller, _, _) = CreateRestaurantAuthController(context, attemptService);

        // 5 consecutive failed attempts
        for (int i = 0; i < 5; i++)
        {
            controller.Login("brute_target", "WrongPass1234!");
        }

        // 6th attempt should be locked out
        var result = controller.Login("brute_target", StrongPassword);
        var viewResult = Assert.IsType<ViewResult>(result);
        var error = viewResult.ViewData["Error"]?.ToString();
        Assert.NotNull(error);
        Assert.Contains("Çok fazla başarısız giriş denemesi yapıldı", error);
    }

    [Fact]
    public void Logout_ClearsSession_AndRedirectsToLogin()
    {
        using var context = TestSupport.CreateContext();
        var (controller, session, _) = CreateRestaurantAuthController(context);

        session.Set("RestaurantUserId", System.Text.Encoding.UTF8.GetBytes("42"));
        session.Set("RestaurantUsername", System.Text.Encoding.UTF8.GetBytes("test_user"));

        var result = controller.Logout();

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirect.ActionName);
        Assert.False(session.TryGetValue("RestaurantUserId", out _));
        Assert.False(session.TryGetValue("RestaurantUsername", out _));
    }

    // ==========================================
    // ADMIN LOGIN & LOGOUT TESTS
    // ==========================================

    [Fact]
    public void AdminLogin_ValidCredentials_SignsInAndRedirectsToAdminDashboard()
    {
        using var context = TestSupport.CreateContext();
        var admin = new Admin
        {
            Username = "superadmin",
            Password = PasswordHelper.Hash(StrongPassword)
        };
        context.Admins.Add(admin);
        context.SaveChanges();

        var (controller, session, _) = CreateAdminAuthController(context);

        var result = controller.Login("superadmin", StrongPassword);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Dashboard", redirect.ControllerName);
        Assert.Equal("Admin", redirect.RouteValues?["area"]);

        Assert.True(session.TryGetValue("AdminUser", out var adminUserBytes));
        Assert.Equal("superadmin", System.Text.Encoding.UTF8.GetString(adminUserBytes));
    }

    [Fact]
    public void AdminLogin_InvalidCredentials_FailsWithErrorMessage()
    {
        using var context = TestSupport.CreateContext();
        var admin = new Admin
        {
            Username = "superadmin",
            Password = PasswordHelper.Hash(StrongPassword)
        };
        context.Admins.Add(admin);
        context.SaveChanges();

        var (controller, session, _) = CreateAdminAuthController(context);

        var result = controller.Login("superadmin", "WrongPass1234!");

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Kullanıcı adı veya şifre hatalı.", viewResult.ViewData["Error"]);
        Assert.False(session.TryGetValue("AdminUser", out _));
    }

    [Fact]
    public void AdminLogout_ClearsAdminSession_AndRedirectsToLogin()
    {
        using var context = TestSupport.CreateContext();
        var (controller, session, _) = CreateAdminAuthController(context);

        session.Set("AdminUser", System.Text.Encoding.UTF8.GetBytes("superadmin"));

        var result = controller.Logout();

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirect.ActionName);
        Assert.False(session.TryGetValue("AdminUser", out _));
    }
}
