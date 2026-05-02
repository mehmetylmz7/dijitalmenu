using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Auth Filters
builder.Services.AddScoped<dijitalmenu.Filters.AdminAuthFilter>();
builder.Services.AddScoped<dijitalmenu.Filters.RestaurantAuthFilter>();

builder.Services.AddDbContext<DataAccessLayer.Concrete.Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Data Access Layer DI Registration
builder.Services.AddScoped<DataAccessLayer.Abstract.IAdminDal, DataAccessLayer.Repositories.AdminRepository>();
builder.Services.AddScoped<DataAccessLayer.Abstract.ICategoryDal, DataAccessLayer.Repositories.CategoryRepository>();
builder.Services.AddScoped<DataAccessLayer.Abstract.IMenuDal, DataAccessLayer.Repositories.MenuRepository>();
builder.Services.AddScoped<DataAccessLayer.Abstract.IMenuItemDal, DataAccessLayer.Repositories.MenuItemRepository>();
builder.Services.AddScoped<DataAccessLayer.Abstract.IRestaurantDal, DataAccessLayer.Repositories.RestaurantRepository>();
builder.Services.AddScoped<DataAccessLayer.Abstract.IThemeDal, DataAccessLayer.Repositories.ThemeRepository>();
builder.Services.AddScoped<DataAccessLayer.Abstract.IUserDal, DataAccessLayer.Repositories.UserRepository>();

// Business Layer DI Registration
builder.Services.AddScoped<BusinessLayer.Abstract.IAdminService, BusinessLayer.Concrete.AdminManager>();
builder.Services.AddScoped<BusinessLayer.Abstract.ICategoryService, BusinessLayer.Concrete.CategoryManager>();
builder.Services.AddScoped<BusinessLayer.Abstract.IMenuService, BusinessLayer.Concrete.MenuManager>();
builder.Services.AddScoped<BusinessLayer.Abstract.IMenuItemService, BusinessLayer.Concrete.MenuItemManager>();
builder.Services.AddScoped<BusinessLayer.Abstract.IRestaurantService, BusinessLayer.Concrete.RestaurantManager>();
builder.Services.AddScoped<BusinessLayer.Abstract.IThemeService, BusinessLayer.Concrete.ThemeManager>();
builder.Services.AddScoped<BusinessLayer.Abstract.IUserService, BusinessLayer.Concrete.UserManager>();

var app = builder.Build();

// ─── Seed: Eksik temaları ekle (isim bazlı kontrol) ───
using (var scope = app.Services.CreateScope())
{
    var themeService = scope.ServiceProvider.GetRequiredService<BusinessLayer.Abstract.IThemeService>();
    var existing = themeService.TGetListAll();

    if (!existing.Any(t => t.Name == "Doğal Yeşil"))
        themeService.TInsert(new EntityLayer.Concrete.Theme { Name = "Doğal Yeşil" });
    if (!existing.Any(t => t.Name == "Ateşli Turuncu"))
        themeService.TInsert(new EntityLayer.Concrete.Theme { Name = "Ateşli Turuncu" });
    if (!existing.Any(t => t.Name == "Okyanus Mavisi"))
        themeService.TInsert(new EntityLayer.Concrete.Theme { Name = "Okyanus Mavisi" });
    if (!existing.Any(t => t.Name == "Kocaoğlu Klasik"))
        themeService.TInsert(new EntityLayer.Concrete.Theme { Name = "Kocaoğlu Klasik" });
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

// Admin Area Route
app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

// Default Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

