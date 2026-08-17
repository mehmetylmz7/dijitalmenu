using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using BusinessLayer.Models;
using DataAccessLayer.Repositories;
using dijitalmenu.Areas.Admin.Controllers;
using dijitalmenu.Areas.Restaurant.Controllers;
using dijitalmenu.Helpers;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Xunit;
using AdminAuthController = dijitalmenu.Areas.Admin.Controllers.AuthController;
using RestaurantAuthController = dijitalmenu.Areas.Restaurant.Controllers.AuthController;

namespace DijitalMenu.Tests;

public class AuditLogAndNotificationTests
{
    [Fact]
    public void Scenario1_LoginSuccess_Creates_AuditLog()
    {
        using var context = TestSupport.CreateContext();
        var allServices = TestSupport.AllServices(context);

        var admin = new Admin { Username = "adminuser", Password = PasswordHelper.Hash("SuperPass123!") };
        context.Admins.Add(admin);
        context.SaveChanges();

        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
        httpContext.Request.Headers["User-Agent"] = "Mozilla/5.0 TestBrowser";
        httpContext.Request.Path = "/Admin/Auth/Login";
        var controllerContext = new ControllerContext { HttpContext = httpContext };
        controllerContext.HttpContext.Session = new TestSession();

        var auditContext = TestSupport.CreateAuditContext(context, httpContext);
        var controller = new AdminAuthController(allServices.Admins, auditContext)
        {
            ControllerContext = controllerContext
        };

        var result = controller.Login("adminuser", "SuperPass123!");

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("adminuser", httpContext.Session.GetString("AdminUser"));

        var log = context.AuditLogs.FirstOrDefault(l => l.Action == "LOGIN_SUCCESS");
        Assert.NotNull(log);
        Assert.Equal("adminuser", log.Username);
        Assert.Equal(admin.Id, log.AdminId);
        Assert.Equal("127.0.0.1", log.IpAddress);
        Assert.Equal("Mozilla/5.0 TestBrowser", log.UserAgent);
        Assert.Equal("/Admin/Auth/Login", log.RequestPath);
    }

    [Fact]
    public void Scenario2_LoginFailed_Creates_AuditLog_And_Increments_Failed_Count()
    {
        using var context = TestSupport.CreateContext();
        var allServices = TestSupport.AllServices(context);

        var admin = new Admin { Username = "adminuser", Password = PasswordHelper.Hash("SuperPass123!") };
        context.Admins.Add(admin);
        context.SaveChanges();

        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("192.168.1.50");
        httpContext.Request.Path = "/Admin/Auth/Login";
        var controllerContext = new ControllerContext { HttpContext = httpContext };
        controllerContext.HttpContext.Session = new TestSession();

        var auditContext = TestSupport.CreateAuditContext(context, httpContext);
        var controller = new AdminAuthController(allServices.Admins, auditContext)
        {
            ControllerContext = controllerContext
        };

        // 1st failed attempt
        controller.Login("adminuser", "WrongPassword1");
        // 2nd failed attempt
        controller.Login("adminuser", "WrongPassword2");
        // 3rd failed attempt
        controller.Login("adminuser", "WrongPassword3");

        var failedLogs = context.AuditLogs.Where(l => l.Action == "LOGIN_FAILED" && l.Username == "adminuser").ToList();
        Assert.Equal(3, failedLogs.Count);

        // Verify no passwords in logs
        foreach (var l in failedLogs)
        {
            Assert.DoesNotContain("WrongPassword", l.Description);
            Assert.Null(l.OldValues);
            Assert.Null(l.NewValues);
        }

        // Verify security notification created on 3rd failed attempt
        var notification = context.Notifications.FirstOrDefault(n => n.Type == "Danger" || n.Type == "Warning");
        Assert.NotNull(notification);
        Assert.Contains("Şüpheli Giriş Denemesi", notification.Title);
        Assert.Contains("adminuser", notification.Message);
    }

    [Fact]
    public void Scenario3_Logout_Creates_AuditLog()
    {
        using var context = TestSupport.CreateContext();
        var allServices = TestSupport.AllServices(context);

        var httpContext = new DefaultHttpContext();
        httpContext.Session = new TestSession(new() { ["AdminUser"] = "adminuser" });
        var controllerContext = new ControllerContext { HttpContext = httpContext };

        var auditContext = TestSupport.CreateAuditContext(context, httpContext);
        var controller = new AdminAuthController(allServices.Admins, auditContext)
        {
            ControllerContext = controllerContext
        };

        var result = controller.Logout();

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Null(httpContext.Session.GetString("AdminUser"));

        var log = context.AuditLogs.FirstOrDefault(l => l.Action == "LOGOUT");
        Assert.NotNull(log);
        Assert.Equal("adminuser", log.Username);
    }

    [Fact]
    public void Scenario4_Restaurant_User_Can_Query_Own_Logs()
    {
        using var context = TestSupport.CreateContext();
        var allServices = TestSupport.AllServices(context);

        var r1 = new Restaurant { Name = "Restoran 1", ThemeId = 1 };
        var r2 = new Restaurant { Name = "Restoran 2", ThemeId = 1 };
        context.Restaurants.AddRange(r1, r2);
        context.SaveChanges();

        // Logs for R1
        context.AuditLogs.Add(new AuditLog { RestaurantId = r1.Id, Action = "CATEGORY_CREATED", Description = "R1 Log 1" });
        context.AuditLogs.Add(new AuditLog { RestaurantId = r1.Id, Action = "MENU_ITEM_CREATED", Description = "R1 Log 2" });

        // Logs for R2
        context.AuditLogs.Add(new AuditLog { RestaurantId = r2.Id, Action = "CATEGORY_CREATED", Description = "R2 Log 1" });
        context.SaveChanges();

        var filter = new AuditLogFilterDto { RestaurantId = r1.Id, Page = 1, PageSize = 10 };
        var (logs, totalCount) = allServices.AuditLogs.GetFilteredLogs(filter);

        Assert.Equal(2, totalCount);
        Assert.All(logs, item => Assert.Equal(r1.Id, item.RestaurantId));
    }

    [Fact]
    public void Scenario5_MultiTenant_Restaurant_Cannot_Query_Other_Restaurant_Logs()
    {
        using var context = TestSupport.CreateContext();
        var allServices = TestSupport.AllServices(context);

        var rOwner = new Restaurant { Name = "Owner Restoran", ThemeId = 1 };
        var rAttacker = new Restaurant { Name = "Attacker Restoran", ThemeId = 1 };
        context.Restaurants.AddRange(rOwner, rAttacker);
        context.SaveChanges();

        context.AuditLogs.Add(new AuditLog { RestaurantId = rOwner.Id, Action = "SECRET_ACTION", Description = "Owner Secret Log" });
        context.AuditLogs.Add(new AuditLog { RestaurantId = rAttacker.Id, Action = "NORMAL_ACTION", Description = "Attacker Log" });
        context.SaveChanges();

        // Restaurant controller strictly uses session RestaurantId
        var controllerContext = TestSupport.ControllerContext(new()
        {
            ["RestaurantId"] = rAttacker.Id.ToString(),
            ["RestaurantUserId"] = "99",
            ["RestaurantUsername"] = "attacker"
        });

        var restaurantAuditController = new dijitalmenu.Areas.Restaurant.Controllers.AuditLogController(allServices.AuditLogs)
        {
            ControllerContext = controllerContext
        };

        var viewResult = Assert.IsType<ViewResult>(restaurantAuditController.Index(null, null, null, null, null, 1, 20));
        var logs = Assert.IsAssignableFrom<IEnumerable<AuditLog>>(viewResult.Model);

        Assert.Single(logs);
        Assert.Equal(rAttacker.Id, logs.First().RestaurantId);
        Assert.DoesNotContain(logs, l => l.RestaurantId == rOwner.Id);
    }

    [Fact]
    public void Scenario6_SuperAdmin_Can_Query_All_Logs()
    {
        using var context = TestSupport.CreateContext();
        var allServices = TestSupport.AllServices(context);

        var r1 = new Restaurant { Name = "R1", ThemeId = 1 };
        var r2 = new Restaurant { Name = "R2", ThemeId = 1 };
        context.Restaurants.AddRange(r1, r2);
        context.SaveChanges();

        context.AuditLogs.Add(new AuditLog { RestaurantId = r1.Id, Action = "LOGIN_SUCCESS", Description = "R1 User Login" });
        context.AuditLogs.Add(new AuditLog { RestaurantId = r2.Id, Action = "LOGIN_SUCCESS", Description = "R2 User Login" });
        context.AuditLogs.Add(new AuditLog { RestaurantId = null, Action = "ADMIN_ACTION", Description = "System Admin Action" });
        context.SaveChanges();

        var adminControllerContext = TestSupport.ControllerContext(new() { ["AdminUser"] = "superadmin" });
        var adminAuditController = new dijitalmenu.Areas.Admin.Controllers.AuditLogController(
            allServices.AuditLogs,
            allServices.Restaurants,
            allServices.Users,
            allServices.Admins)
        {
            ControllerContext = adminControllerContext
        };

        var viewResult = Assert.IsType<ViewResult>(adminAuditController.Index(null, null, null, null, null, null, null, null, 1, 50));
        var logs = Assert.IsAssignableFrom<IEnumerable<AuditLog>>(viewResult.Model);

        Assert.Equal(3, logs.Count());
    }

    [Fact]
    public void Scenario7_Entity_Update_Creates_Old_And_New_Values_In_Jsonb()
    {
        using var context = TestSupport.CreateContext();
        var httpContext = new DefaultHttpContext();
        var auditContext = TestSupport.CreateAuditContext(context, httpContext);

        var oldItem = new { Name = "Eski Pizza", Price = 150m, CategoryId = 5 };
        var newItem = new { Name = "Yeni Pizza", Price = 180m, CategoryId = 5 };

        auditContext.Log(
            action: "MENU_ITEM_UPDATED",
            entityType: "MenuItem",
            entityId: 42,
            description: "Pizza fiyatı güncellendi",
            oldEntity: oldItem,
            newEntity: newItem
        );

        var log = context.AuditLogs.FirstOrDefault(l => l.EntityId == 42 && l.Action == "MENU_ITEM_UPDATED");
        Assert.NotNull(log);
        Assert.NotNull(log.OldValues);
        Assert.NotNull(log.NewValues);

        using var oldDoc = JsonDocument.Parse(log.OldValues);
        Assert.Equal("Eski Pizza", oldDoc.RootElement.GetProperty("Name").GetString());
        Assert.Equal(150m, oldDoc.RootElement.GetProperty("Price").GetDecimal());

        using var newDoc = JsonDocument.Parse(log.NewValues);
        Assert.Equal("Yeni Pizza", newDoc.RootElement.GetProperty("Name").GetString());
        Assert.Equal(180m, newDoc.RootElement.GetProperty("Price").GetDecimal());
    }

    [Fact]
    public void Scenario8_Sensitive_Fields_Are_Never_Logged()
    {
        using var context = TestSupport.CreateContext();
        var httpContext = new DefaultHttpContext();
        var auditContext = TestSupport.CreateAuditContext(context, httpContext);

        var userEntity = new
        {
            Id = 1,
            Username = "supersecretuser",
            Password = "PlainTextPassword123!",
            PasswordHash = "$2a$12$eX4mpL3H4shV4lu3",
            SecurityToken = "token_abc_123",
            SecretKey = "secret_xyz_999",
            Email = "user@test.com"
        };

        auditContext.Log(
            action: "USER_UPDATED",
            entityType: "User",
            entityId: 1,
            description: "User details updated",
            newEntity: userEntity
        );

        var log = context.AuditLogs.FirstOrDefault(l => l.EntityId == 1 && l.Action == "USER_UPDATED");
        Assert.NotNull(log);
        Assert.NotNull(log.NewValues);

        Assert.DoesNotContain("PlainTextPassword123!", log.NewValues);
        Assert.DoesNotContain("eX4mpL3H4shV4lu3", log.NewValues);
        Assert.DoesNotContain("token_abc_123", log.NewValues);
        Assert.DoesNotContain("secret_xyz_999", log.NewValues);

        using var doc = JsonDocument.Parse(log.NewValues);
        Assert.False(doc.RootElement.TryGetProperty("Password", out _));
        Assert.False(doc.RootElement.TryGetProperty("PasswordHash", out _));
        Assert.False(doc.RootElement.TryGetProperty("SecurityToken", out _));
        Assert.False(doc.RootElement.TryGetProperty("SecretKey", out _));
        Assert.True(doc.RootElement.TryGetProperty("Username", out var usernameProp));
        Assert.Equal("supersecretuser", usernameProp.GetString());
    }

    [Fact]
    public void Scenario9_AuditLog_Pagination_Works_Correctly()
    {
        using var context = TestSupport.CreateContext();
        var allServices = TestSupport.AllServices(context);

        for (int i = 1; i <= 25; i++)
        {
            context.AuditLogs.Add(new AuditLog
            {
                Action = $"ACTION_{i}",
                Description = $"Description {i}",
                CreatedAt = DateTime.UtcNow.AddMinutes(-i)
            });
        }
        context.SaveChanges();

        // Page 1 (10 items)
        var (page1Logs, page1Total) = allServices.AuditLogs.GetFilteredLogs(new AuditLogFilterDto { Page = 1, PageSize = 10 });
        Assert.Equal(25, page1Total);
        Assert.Equal(10, page1Logs.Count);

        // Page 3 (5 items)
        var (page3Logs, page3Total) = allServices.AuditLogs.GetFilteredLogs(new AuditLogFilterDto { Page = 3, PageSize = 10 });
        Assert.Equal(25, page3Total);
        Assert.Equal(5, page3Logs.Count);
    }

    [Fact]
    public void Scenario10_Notification_Creation_And_MarkAsRead_Works()
    {
        using var context = TestSupport.CreateContext();
        var allServices = TestSupport.AllServices(context);

        // Create notifications
        allServices.Notifications.CreateNotification("Başlık 1", "Mesaj 1", "Info");
        allServices.Notifications.CreateNotification("Başlık 2", "Mesaj 2", "Warning");

        var notifications = context.Notifications.OrderBy(n => n.Id).ToList();
        Assert.Equal(2, notifications.Count);
        var n1 = notifications[0];
        var n2 = notifications[1];

        Assert.Equal(2, allServices.Notifications.GetUnreadCount());

        // Mark single as read
        allServices.Notifications.MarkAsRead(n1.Id);
        Assert.Equal(1, allServices.Notifications.GetUnreadCount());
        Assert.True(context.Notifications.Find(n1.Id)!.IsRead);
        Assert.False(context.Notifications.Find(n2.Id)!.IsRead);

        // Mark all as read
        allServices.Notifications.MarkAllAsRead();
        Assert.Equal(0, allServices.Notifications.GetUnreadCount());
        Assert.True(context.Notifications.Find(n2.Id)!.IsRead);
    }
}
