using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DataAccessLayer.Concrete.Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
