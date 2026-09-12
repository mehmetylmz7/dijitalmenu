using BusinessLayer.Concrete;
using DataAccessLayer.Repositories;
using dijitalmenu.Filters;
using dijitalmenu.Models;
using dijitalmenu.Services;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Xunit;
using RestaurantArea = dijitalmenu.Areas.Restaurant.Controllers;
using AdminArea = dijitalmenu.Areas.Admin.Controllers;

namespace DijitalMenu.Tests
{
    public class SecurityRegressionTests
    {
        // ─── 1. TENANT ISOLATION (IDOR / BOLA) ───

        [Fact]
        public void TenantIsolation_CategoryEdit_ReturnsForbid_WhenAccessingOtherRestaurantCategory()
        {
            using var context = TestSupport.CreateContext();
            var services = TestSupport.Services(context);
            context.Themes.Add(new Theme { Name = "Test", PrimaryColor = "#000", SecondaryColor = "#111", BackgroundColor = "#fff", FontFamily = "Arial" });
            var restA = new Restaurant { Name = "Restaurant A", ThemeId = 1 };
            var restB = new Restaurant { Name = "Restaurant B", ThemeId = 1 };
            context.Restaurants.AddRange(restA, restB);
            context.SaveChanges();

            var menuA = new Menu { RestaurantId = restA.Id };
            var menuB = new Menu { RestaurantId = restB.Id };
            context.Menus.AddRange(menuA, menuB);
            context.SaveChanges();

            var catA = new Category { Name = "Cat A", MenuId = menuA.Id };
            var catB = new Category { Name = "Cat B", MenuId = menuB.Id };
            context.Categories.AddRange(catA, catB);
            context.SaveChanges();

            // Logged in as Restaurant A
            var httpContext = TestSupport.ControllerContext(new() { ["RestaurantId"] = restA.Id.ToString() });
            var auditContext = TestSupport.CreateAuditContext(context, httpContext.HttpContext);
            var controller = new RestaurantArea.CategoryController(services.Categories, services.Menus, new TestStorageService(), auditContext)
            {
                ControllerContext = httpContext
            };

            // Attempt GET edit on Cat B (belongs to Restaurant B)
            var getResult = controller.Edit(catB.Id);
            Assert.IsType<ForbidResult>(getResult);

            // Attempt POST edit on Cat B
            var postResult = controller.Edit(catB.Id, "Hacked Cat", null);
            Assert.IsType<ForbidResult>(postResult);

            // Verify Cat B was not modified
            var untouched = services.Categories.TGetByID(catB.Id);
            Assert.Equal("Cat B", untouched.Name);
        }

        [Fact]
        public void TenantIsolation_CategoryDelete_ReturnsForbid_WhenDeletingOtherRestaurantCategory()
        {
            using var context = TestSupport.CreateContext();
            var services = TestSupport.Services(context);
            context.Themes.Add(new Theme { Name = "Test", PrimaryColor = "#000", SecondaryColor = "#111", BackgroundColor = "#fff", FontFamily = "Arial" });
            var restA = new Restaurant { Name = "Restaurant A", ThemeId = 1 };
            var restB = new Restaurant { Name = "Restaurant B", ThemeId = 1 };
            context.Restaurants.AddRange(restA, restB);
            context.SaveChanges();

            var menuA = new Menu { RestaurantId = restA.Id };
            var menuB = new Menu { RestaurantId = restB.Id };
            context.Menus.AddRange(menuA, menuB);
            context.SaveChanges();

            var catB = new Category { Name = "Cat B", MenuId = menuB.Id };
            context.Categories.Add(catB);
            context.SaveChanges();

            // Logged in as Restaurant A
            var httpContext = TestSupport.ControllerContext(new() { ["RestaurantId"] = restA.Id.ToString() });
            var auditContext = TestSupport.CreateAuditContext(context, httpContext.HttpContext);
            var controller = new RestaurantArea.CategoryController(services.Categories, services.Menus, new TestStorageService(), auditContext)
            {
                ControllerContext = httpContext
            };

            var result = controller.Delete(catB.Id);
            Assert.IsType<ForbidResult>(result);

            // Verify Category B still exists
            Assert.NotNull(services.Categories.TGetByID(catB.Id));
        }

        [Fact]
        public void TenantIsolation_MenuItemEdit_ReturnsForbid_WhenModifyingOtherRestaurantItem()
        {
            using var context = TestSupport.CreateContext();
            var services = TestSupport.Services(context);
            context.Themes.Add(new Theme { Name = "Test", PrimaryColor = "#000", SecondaryColor = "#111", BackgroundColor = "#fff", FontFamily = "Arial" });
            var restA = new Restaurant { Name = "Restaurant A", ThemeId = 1 };
            var restB = new Restaurant { Name = "Restaurant B", ThemeId = 1 };
            context.Restaurants.AddRange(restA, restB);
            context.SaveChanges();

            var menuA = new Menu { RestaurantId = restA.Id };
            var menuB = new Menu { RestaurantId = restB.Id };
            context.Menus.AddRange(menuA, menuB);
            context.SaveChanges();

            var catA = new Category { Name = "Cat A", MenuId = menuA.Id };
            var catB = new Category { Name = "Cat B", MenuId = menuB.Id };
            context.Categories.AddRange(catA, catB);
            context.SaveChanges();

            var itemB = new MenuItem { Name = "Dish B", Price = 50m, CategoryId = catB.Id };
            context.MenuItems.Add(itemB);
            context.SaveChanges();

            // Logged in as Restaurant A
            var httpContext = TestSupport.ControllerContext(new() { ["RestaurantId"] = restA.Id.ToString() });
            var auditContext = TestSupport.CreateAuditContext(context, httpContext.HttpContext);
            var controller = new RestaurantArea.MenuItemController(services.Items, services.Categories, services.Menus, new TestStorageService(), auditContext)
            {
                ControllerContext = httpContext
            };
            controller.TempData = TestSupport.TempData(httpContext.HttpContext);

            // Attempt GET edit on item B
            var getResult = controller.Edit(itemB.Id);
            Assert.IsType<ForbidResult>(getResult);

            // Attempt POST edit on item B with input model
            var postResult = controller.Edit(new MenuItemInputModel
            {
                Id = itemB.Id,
                Name = "Hacked Dish",
                Price = 100m,
                CategoryId = catA.Id // trying to move to Cat A
            }, null);

            Assert.IsType<ForbidResult>(postResult);

            // Verify item B was not modified
            var untouched = services.Items.TGetByID(itemB.Id);
            Assert.Equal("Dish B", untouched.Name);
            Assert.Equal(catB.Id, untouched.CategoryId);
        }

        [Fact]
        public void TenantIsolation_MenuItemDelete_ReturnsForbid_WhenDeletingOtherRestaurantItem()
        {
            using var context = TestSupport.CreateContext();
            var services = TestSupport.Services(context);
            context.Themes.Add(new Theme { Name = "Test", PrimaryColor = "#000", SecondaryColor = "#111", BackgroundColor = "#fff", FontFamily = "Arial" });
            var restA = new Restaurant { Name = "Restaurant A", ThemeId = 1 };
            var restB = new Restaurant { Name = "Restaurant B", ThemeId = 1 };
            context.Restaurants.AddRange(restA, restB);
            context.SaveChanges();

            var menuA = new Menu { RestaurantId = restA.Id };
            var menuB = new Menu { RestaurantId = restB.Id };
            context.Menus.AddRange(menuA, menuB);
            context.SaveChanges();

            var catB = new Category { Name = "Cat B", MenuId = menuB.Id };
            context.Categories.Add(catB);
            context.SaveChanges();

            var itemB = new MenuItem { Name = "Dish B", Price = 50m, CategoryId = catB.Id };
            context.MenuItems.Add(itemB);
            context.SaveChanges();

            // Logged in as Restaurant A
            var httpContext = TestSupport.ControllerContext(new() { ["RestaurantId"] = restA.Id.ToString() });
            var auditContext = TestSupport.CreateAuditContext(context, httpContext.HttpContext);
            var controller = new RestaurantArea.MenuItemController(services.Items, services.Categories, services.Menus, new TestStorageService(), auditContext)
            {
                ControllerContext = httpContext
            };

            var result = controller.Delete(itemB.Id);
            Assert.IsType<ForbidResult>(result);

            Assert.NotNull(services.Items.TGetByID(itemB.Id));
        }

        // ─── 2. SESSION SECURITY & TENANT VERIFICATION ───

        [Fact]
        public void SessionSecurity_RestaurantAuthFilter_RejectsMismatchedUserAndRestaurant()
        {
            using var context = TestSupport.CreateContext();
            var users = new UserManager(new UserRepository(context));

            // User belongs to Restaurant 10
            var user = new User { Id = 1, Username = "user1", Password = "hash", RestaurantId = 10 };
            context.Users.Add(user);
            context.SaveChanges();

            // Attacker sets session RestaurantUserId = 1, but RestaurantId = 99 (spoofed)
            var session = new TestSession(new()
            {
                ["RestaurantUserId"] = "1",
                ["RestaurantId"] = "99"
            });

            var httpContext = new DefaultHttpContext { Session = session };
            var actionContext = new ActionContext(httpContext, new RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
            var executingContext = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object?>(), new object());

            var filter = new RestaurantAuthFilter(users);
            filter.OnActionExecuting(executingContext);

            // Mismatch must redirect to Login and clear session
            var redirect = Assert.IsType<RedirectToRouteResult>(executingContext.Result);
            Assert.Equal("Auth", redirect.RouteValues?["controller"]);
            Assert.Equal("Login", redirect.RouteValues?["action"]);
            Assert.False(session.TryGetValue("RestaurantUserId", out _));
            Assert.False(session.TryGetValue("RestaurantId", out _));
        }

        [Fact]
        public void SessionSecurity_RestaurantAuthFilter_RejectsNonExistentUser()
        {
            using var context = TestSupport.CreateContext();
            var users = new UserManager(new UserRepository(context));

            // Session has user ID 999 which does not exist in DB
            var session = new TestSession(new()
            {
                ["RestaurantUserId"] = "999",
                ["RestaurantId"] = "10"
            });

            var httpContext = new DefaultHttpContext { Session = session };
            var actionContext = new ActionContext(httpContext, new RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
            var executingContext = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object?>(), new object());

            var filter = new RestaurantAuthFilter(users);
            filter.OnActionExecuting(executingContext);

            var redirect = Assert.IsType<RedirectToRouteResult>(executingContext.Result);
            Assert.Equal("Login", redirect.RouteValues?["action"]);
            Assert.False(session.TryGetValue("RestaurantUserId", out _));
        }

        // ─── 3. ADMIN AUTHORIZATION & MASS ASSIGNMENT ───

        [Fact]
        public void AdminAuth_RejectsUnauthenticatedOrRestaurantUser()
        {
            using var context = TestSupport.CreateContext();
            var admins = new AdminManager(new AdminRepository(context));

            // Restaurant user session (no AdminUser key)
            var session = new TestSession(new()
            {
                ["RestaurantUserId"] = "1",
                ["RestaurantId"] = "10",
                ["RestaurantUsername"] = "restuser"
            });

            var httpContext = new DefaultHttpContext { Session = session };
            var actionContext = new ActionContext(httpContext, new RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
            var executingContext = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object?>(), new object());

            var filter = new AdminAuthFilter(admins);
            filter.OnActionExecuting(executingContext);

            var redirect = Assert.IsType<RedirectToRouteResult>(executingContext.Result);
            Assert.Equal("Admin", redirect.RouteValues?["area"]);
            Assert.Equal("Auth", redirect.RouteValues?["controller"]);
            Assert.Equal("Login", redirect.RouteValues?["action"]);
        }

        [Fact]
        public void MassAssignment_AdminUserController_RejectsNonExistentRestaurant()
        {
            using var context = TestSupport.CreateContext();
            var allServices = TestSupport.AllServices(context);
            var auditContext = TestSupport.CreateAuditContext(context);
            var controller = new AdminArea.UserController(allServices.Users, allServices.Restaurants, auditContext)
            {
                ControllerContext = TestSupport.ControllerContext(new() { ["AdminUser"] = "admin" })
            };

            // Attempt to create user with arbitrary non-existent RestaurantId = 9999
            var result = controller.Create(new UserViewModel
            {
                Username = "testuser",
                Password = "Password123!",
                RestaurantId = 9999
            });

            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ContainsKey("RestaurantId"));
            Assert.Empty(context.Users);
        }

        // ─── 4. FILE UPLOAD SECURITY (MAGIC BYTES & TRAVERSAL) ───

        [Fact]
        public void FileUpload_RejectsFakeImageWithInvalidSignature()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "dijitalmenu_test_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);

            try
            {
                var env = new TestWebHostEnvironment { WebRootPath = tempDir };
                var storage = new LocalStorageService(env, NullLogger<LocalStorageService>.Instance);

                // Executable content renamed to .jpg
                var fakeJpgBytes = Encoding.UTF8.GetBytes("MZ\x90\x00\x03\x00\x00\x00This is a fake PE executable disguised as JPG");
                using var stream = new MemoryStream(fakeJpgBytes);
                var fakeFile = new FormFile(stream, 0, fakeJpgBytes.Length, "photoFile", "test.jpg")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/jpeg"
                };

                var saved = storage.TrySaveImage(fakeFile, "menu-items", out var fileUrl, out var error);

                Assert.False(saved);
                Assert.Null(fileUrl);
                Assert.Contains("Dosya içeriği geçerli bir görsel formatı ile eşleşmiyor", error);
            }
            finally
            {
                if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
            }
        }

        [Fact]
        public void FileUpload_AcceptsValidJpegSignature()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "dijitalmenu_test_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);

            try
            {
                var env = new TestWebHostEnvironment { WebRootPath = tempDir };
                var storage = new LocalStorageService(env, NullLogger<LocalStorageService>.Instance);

                // Valid JPEG header: FF D8 FF E0 00 10 4A 46 49 46 00 01
                var validJpgBytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01, 0x00, 0x00 };
                using var stream = new MemoryStream(validJpgBytes);
                var file = new FormFile(stream, 0, validJpgBytes.Length, "photoFile", "photo.jpg")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/jpeg"
                };

                var saved = storage.TrySaveImage(file, "menu-items", out var fileUrl, out var error);

                Assert.True(saved);
                Assert.Null(error);
                Assert.NotNull(fileUrl);
                Assert.StartsWith("/images/menu-items/", fileUrl);
            }
            finally
            {
                if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
            }
        }

        [Fact]
        public void FileUpload_DeleteImage_BlocksPathTraversal()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "dijitalmenu_test_" + Guid.NewGuid().ToString("N"));
            var imagesDir = Path.Combine(tempDir, "images");
            Directory.CreateDirectory(imagesDir);

            // Create a sensitive file outside images
            var sensitiveFile = Path.Combine(tempDir, "appsettings.json");
            File.WriteAllText(sensitiveFile, "{\"Secret\":\"sensitive\"}");

            try
            {
                var env = new TestWebHostEnvironment { WebRootPath = tempDir };
                var storage = new LocalStorageService(env, NullLogger<LocalStorageService>.Instance);

                // Attempt path traversal deletion
                var deleted = storage.DeleteImage("../../appsettings.json");

                Assert.False(deleted);
                Assert.True(File.Exists(sensitiveFile)); // Sensitive file must remain untouched
            }
            finally
            {
                if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
            }
        }

        // ─── 5. LOGIN ABUSE PROTECTION & THROTTLING ───

        [Fact]
        public void LoginAbuse_ThrottlesAccountAfterRepeatedFailures()
        {
            var service = new LoginAttemptService();
            var username = "target_account";

            Assert.False(service.IsLockedOut(username));

            // Record 4 failed attempts -> should not be locked out yet
            for (int i = 0; i < 4; i++)
            {
                service.RecordFailedAttempt(username);
                Assert.False(service.IsLockedOut(username));
            }

            // 5th failed attempt triggers temporary throttle
            service.RecordFailedAttempt(username);
            Assert.True(service.IsLockedOut(username));
            Assert.NotNull(service.GetLockoutRemaining(username));

            // Successful login resets the counter
            service.ResetAttempts(username);
            Assert.False(service.IsLockedOut(username));
            Assert.Equal(0, service.GetFailedAttempts(username));
        }

        // ─── 6. XSS PROTECTION TEST ───

        [Fact]
        public void XssProtection_JsonSerializer_EscapesScriptTags()
        {
            var maliciousCategory = new
            {
                id = 1,
                name = "</script><script>alert('xss')</script>",
                displayOrder = 0
            };

            var serialized = JsonSerializer.Serialize(maliciousCategory);

            // Default .NET serializer must escape '<' and '>' to unicode to prevent script breakouts in inline JSON
            Assert.DoesNotContain("<script>", serialized);
            Assert.DoesNotContain("</script>", serialized);
            Assert.Contains(@"\u003Cscript\u003E", serialized);
            Assert.Contains(@"\u003C/script\u003E", serialized);
        }
    }
}
