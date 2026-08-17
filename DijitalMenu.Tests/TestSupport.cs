using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace DijitalMenu.Tests;

internal static class TestSupport
{
    public static Context CreateContext()
    {
        var options = new DbContextOptionsBuilder<Context>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new Context(options);
    }

    public static (IUserService Users, IRestaurantService Restaurants, IMenuService Menus, ICategoryService Categories, IMenuItemService Items, IThemeService Themes) Services(Context context) =>
        (new UserManager(new UserRepository(context)), new RestaurantManager(new RestaurantRepository(context)),
         new MenuManager(new MenuRepository(context)), new CategoryManager(new CategoryRepository(context)),
         new MenuItemManager(new MenuItemRepository(context)), new ThemeManager(new ThemeRepository(context)));

    public static (IUserService Users, IRestaurantService Restaurants, IMenuService Menus, ICategoryService Categories, IMenuItemService Items, IThemeService Themes, IAdminService Admins, IAuditLogService AuditLogs, INotificationService Notifications) AllServices(Context context) =>
        (new UserManager(new UserRepository(context)),
         new RestaurantManager(new RestaurantRepository(context)),
         new MenuManager(new MenuRepository(context)),
         new CategoryManager(new CategoryRepository(context)),
         new MenuItemManager(new MenuItemRepository(context)),
         new ThemeManager(new ThemeRepository(context)),
         new AdminManager(new AdminRepository(context)),
         new AuditLogManager(new AuditLogRepository(context)),
         new NotificationManager(new NotificationRepository(context)));

    public static dijitalmenu.Services.IAuditContextService CreateAuditContext(Context context, HttpContext? httpContext = null)
    {
        var accessor = new HttpContextAccessor { HttpContext = httpContext ?? new DefaultHttpContext() };
        var auditLogs = new AuditLogManager(new AuditLogRepository(context));
        var notifications = new NotificationManager(new NotificationRepository(context));
        var admins = new AdminManager(new AdminRepository(context));
        return new dijitalmenu.Services.AuditContextService(accessor, auditLogs, notifications, admins);
    }

    public static ControllerContext ControllerContext(Dictionary<string, string>? values = null)
    {
        var httpContext = new DefaultHttpContext { Session = new TestSession(values) };
        return new ControllerContext { HttpContext = httpContext };
    }

    public static ITempDataDictionary TempData(HttpContext httpContext) =>
        new TempDataDictionary(httpContext, new NullTempDataProvider());
}

internal sealed class TestSession : ISession
{
    private readonly Dictionary<string, byte[]> _values = new();
    public TestSession(Dictionary<string, string>? values = null)
    {
        foreach (var pair in values ?? new()) _values[pair.Key] = System.Text.Encoding.UTF8.GetBytes(pair.Value);
    }
    public IEnumerable<string> Keys => _values.Keys;
    public string Id => "test";
    public bool IsAvailable => true;
    public void Clear() => _values.Clear();
    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public void Remove(string key) => _values.Remove(key);
    public void Set(string key, byte[] value) => _values[key] = value;
    public bool TryGetValue(string key, out byte[] value) => _values.TryGetValue(key, out value!);
}

internal sealed class NullTempDataProvider : ITempDataProvider
{
    public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();
    public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
}

internal sealed class TestWebHostEnvironment : IWebHostEnvironment
{
    public string ApplicationName { get; set; } = "DijitalMenu.Tests";
    public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    public string WebRootPath { get; set; } = Path.GetTempPath();
    public string EnvironmentName { get; set; } = Environments.Development;
    public string ContentRootPath { get; set; } = Path.GetTempPath();
    public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
}
