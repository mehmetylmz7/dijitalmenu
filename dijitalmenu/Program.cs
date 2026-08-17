using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var port = Environment.GetEnvironmentVariable("PORT");
if (int.TryParse(port, out var parsedPort) && parsedPort > 0)
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{parsedPort}");
}

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
});

// Auth Filters
builder.Services.AddScoped<dijitalmenu.Filters.AdminAuthFilter>();
builder.Services.AddScoped<dijitalmenu.Filters.RestaurantAuthFilter>();

builder.Services.AddDbContext<DataAccessLayer.Concrete.Context>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("PostgresConnection")
                           ?? builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString);
});

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
builder.Services.AddScoped<DataAccessLayer.Abstract.IDefaultCategoryDal, DataAccessLayer.Repositories.DefaultCategoryRepository>();
builder.Services.AddScoped<DataAccessLayer.Abstract.IMenuDal, DataAccessLayer.Repositories.MenuRepository>();
builder.Services.AddScoped<DataAccessLayer.Abstract.IMenuItemDal, DataAccessLayer.Repositories.MenuItemRepository>();
builder.Services.AddScoped<DataAccessLayer.Abstract.IRestaurantDal, DataAccessLayer.Repositories.RestaurantRepository>();
builder.Services.AddScoped<DataAccessLayer.Abstract.IThemeDal, DataAccessLayer.Repositories.ThemeRepository>();
builder.Services.AddScoped<DataAccessLayer.Abstract.IUserDal, DataAccessLayer.Repositories.UserRepository>();

// Business Layer DI Registration
builder.Services.AddScoped<BusinessLayer.Abstract.IAdminService, BusinessLayer.Concrete.AdminManager>();
builder.Services.AddScoped<BusinessLayer.Abstract.ICategoryService, BusinessLayer.Concrete.CategoryManager>();
builder.Services.AddScoped<BusinessLayer.Abstract.IDefaultCategoryService, BusinessLayer.Concrete.DefaultCategoryManager>();
builder.Services.AddScoped<BusinessLayer.Abstract.IMenuService, BusinessLayer.Concrete.MenuManager>();
builder.Services.AddScoped<BusinessLayer.Abstract.IMenuItemService, BusinessLayer.Concrete.MenuItemManager>();
builder.Services.AddScoped<BusinessLayer.Abstract.IRestaurantService, BusinessLayer.Concrete.RestaurantManager>();
builder.Services.AddScoped<BusinessLayer.Abstract.IThemeService, BusinessLayer.Concrete.ThemeManager>();
builder.Services.AddScoped<BusinessLayer.Abstract.IUserService, BusinessLayer.Concrete.UserManager>();
builder.Services.AddScoped<BusinessLayer.Abstract.ICategorySuggestionService, BusinessLayer.Concrete.CategorySuggestionManager>();

var app = builder.Build();

// ─── Migration + Seed ───
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataAccessLayer.Concrete.Context>();
    try
    {
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Veritabanı migration adımı atlandı veya tablolar zaten mevcut.");
    }

    try
    {
        context.Database.ExecuteSqlRaw(@"ALTER TABLE ""Restaurants"" ADD COLUMN IF NOT EXISTS ""ImportantNotice"" character varying(1000);");
        context.Database.ExecuteSqlRaw(@"ALTER TABLE ""Restaurants"" ADD COLUMN IF NOT EXISTS ""WorkingHours"" character varying(200);");
        context.Database.ExecuteSqlRaw(@"ALTER TABLE ""Restaurants"" ADD COLUMN IF NOT EXISTS ""InstagramUrl"" character varying(2048);");
        context.Database.ExecuteSqlRaw(@"ALTER TABLE ""Themes"" ADD COLUMN IF NOT EXISTS ""IsActive"" boolean NOT NULL DEFAULT true;");
        context.Database.ExecuteSqlRaw(@"ALTER TABLE ""Categories"" ADD COLUMN IF NOT EXISTS ""DisplayOrder"" integer NOT NULL DEFAULT 0;");
        context.Database.ExecuteSqlRaw(@"ALTER TABLE ""MenuItems"" ADD COLUMN IF NOT EXISTS ""DisplayOrder"" integer NOT NULL DEFAULT 0;");
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "ImportantNotice / WorkingHours / IsActive sütun kontrolü atlandı veya zaten mevcut.");
    }

    // Populate missing slugs for existing restaurants
    try
    {
        var restaurantsWithoutSlug = context.Restaurants.Where(r => string.IsNullOrEmpty(r.Slug)).ToList();
        if (restaurantsWithoutSlug.Any())
        {
            foreach (var rest in restaurantsWithoutSlug)
            {
                string baseSlug = dijitalmenu.Helpers.StringHelper.GenerateSlug(rest.Name);
                if (string.IsNullOrEmpty(baseSlug)) baseSlug = "restoran";

                string candidate = baseSlug;
                int counter = 1;
                while (context.Restaurants.Any(r => r.Id != rest.Id && r.Slug == candidate))
                {
                    candidate = $"{baseSlug}-{counter++}";
                }
                rest.Slug = candidate;
            }
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Restoran Slug değerleri güncellenirken bir uyarı oluştu.");
    }

    // Admin bootstrap seed
    var adminService = scope.ServiceProvider.GetRequiredService<BusinessLayer.Abstract.IAdminService>();
    if (!adminService.TGetListAll().Any())
    {
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var username = config["Seed:AdminUsername"];
        var password = config["Seed:AdminPassword"];

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            if (app.Environment.IsDevelopment())
            {
                username ??= "admin";
                password ??= "DevPassword123!";
                var devLogger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                devLogger.LogWarning("Seed:AdminUsername/Password yapılandırılmamış. Development ortamı için varsayılan credential kullanılıyor.");
            }
            else
            {
                var prodLogger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                prodLogger.LogCritical("Production ortamında Seed:AdminUsername ve Seed:AdminPassword yapılandırılmalıdır!");
                throw new InvalidOperationException("Production ortamında admin seed credential'ları yapılandırılmalıdır. 'Seed:AdminUsername' ve 'Seed:AdminPassword' environment variable'larını ayarlayın.");
            }
        }

        adminService.TInsert(new EntityLayer.Concrete.Admin
        {
            Username = username,
            Password = dijitalmenu.Helpers.PasswordHelper.Hash(password)
        });
    }

    var themeService = scope.ServiceProvider.GetRequiredService<BusinessLayer.Abstract.IThemeService>();
    var restaurantService = scope.ServiceProvider.GetRequiredService<BusinessLayer.Abstract.IRestaurantService>();
    var existing = themeService.TGetListAll();

    var premiumThemes = new List<EntityLayer.Concrete.Theme>
    {
        new EntityLayer.Concrete.Theme 
        { 
            Name = "Modern Minimalist", 
            PrimaryColor = "#18181b", 
            SecondaryColor = "#71717a", 
            BackgroundColor = "#ffffff", 
            FontFamily = "Inter, sans-serif", 
            Layout = EntityLayer.Concrete.LayoutType.List 
        },
        new EntityLayer.Concrete.Theme 
        { 
            Name = "Sıcak & Rustik", 
            PrimaryColor = "#78350f", 
            SecondaryColor = "#b45309", 
            BackgroundColor = "#fdf8f6", 
            FontFamily = "Playfair Display, serif", 
            Layout = EntityLayer.Concrete.LayoutType.CardWithImage 
        },
        new EntityLayer.Concrete.Theme 
        { 
            Name = "Koyu & Lüks", 
            PrimaryColor = "#d4af37", 
            SecondaryColor = "#aa7c11", 
            BackgroundColor = "#121212", 
            FontFamily = "Cinzel, serif", 
            Layout = EntityLayer.Concrete.LayoutType.Grid 
        },
        new EntityLayer.Concrete.Theme 
        { 
            Name = "Renkli & Eğlenceli", 
            PrimaryColor = "#ec4899", 
            SecondaryColor = "#f59e0b", 
            BackgroundColor = "#fffbeb", 
            FontFamily = "Outfit, sans-serif", 
            Layout = EntityLayer.Concrete.LayoutType.CardWithImage 
        },
        new EntityLayer.Concrete.Theme 
        { 
            Name = "Editöryal & Kalın Tipografi", 
            PrimaryColor = "#000000", 
            SecondaryColor = "#ef4444", 
            BackgroundColor = "#fafafa", 
            FontFamily = "DM Serif Display, serif", 
            Layout = EntityLayer.Concrete.LayoutType.Grid 
        },
        new EntityLayer.Concrete.Theme 
        { 
            Name = "Mobil Bistro", 
            PrimaryColor = "#ef233c", 
            SecondaryColor = "#2b2d42", 
            BackgroundColor = "#f8f9fa", 
            FontFamily = "-apple-system, BlinkMacSystemFont, Segoe UI, sans-serif", 
            Layout = EntityLayer.Concrete.LayoutType.List 
        },
        new EntityLayer.Concrete.Theme 
        { 
            Name = "Ay Cafe", 
            PrimaryColor = "#ff6b6b", 
            SecondaryColor = "#2d3436", 
            BackgroundColor = "#f8f9fa", 
            FontFamily = "-apple-system, BlinkMacSystemFont, Segoe UI, sans-serif", 
            Layout = EntityLayer.Concrete.LayoutType.List 
        },
        new EntityLayer.Concrete.Theme 
        { 
            Name = "Klasik Elegance", 
            PrimaryColor = "#37463b", 
            SecondaryColor = "#a9824c", 
            BackgroundColor = "#faf9f6", 
            FontFamily = "Fraunces, serif", 
            Layout = EntityLayer.Concrete.LayoutType.List 
        },
        new EntityLayer.Concrete.Theme 
        { 
            Name = "Anadolu Klasik", 
            PrimaryColor = "#78350f", 
            SecondaryColor = "#d97706", 
            BackgroundColor = "#fafaf9", 
            FontFamily = "Georgia, serif", 
            Layout = EntityLayer.Concrete.LayoutType.Grid 
        },
        new EntityLayer.Concrete.Theme 
        { 
            Name = "Eski Gazete Kültürü", 
            PrimaryColor = "#2d1e10", 
            SecondaryColor = "#5c4033", 
            BackgroundColor = "#f4eedb", 
            FontFamily = "Georgia, serif", 
            Layout = EntityLayer.Concrete.LayoutType.List 
        },
        new EntityLayer.Concrete.Theme 
        { 
            Name = "Uzak Doğu Esintisi", 
            PrimaryColor = "#d4af37", 
            SecondaryColor = "#022312", 
            BackgroundColor = "#022312", 
            FontFamily = "Georgia, serif", 
            Layout = EntityLayer.Concrete.LayoutType.List 
        },
        new EntityLayer.Concrete.Theme 
        { 
            Name = "Pastel Günbatımı", 
            PrimaryColor = "#ff793f", 
            SecondaryColor = "#e15f41", 
            BackgroundColor = "#fffdfa", 
            FontFamily = "system-ui, sans-serif", 
            Layout = EntityLayer.Concrete.LayoutType.Grid 
        },
        new EntityLayer.Concrete.Theme 
        { 
            Name = "Bento Düzeni", 
            PrimaryColor = "#b45309", 
            SecondaryColor = "#78350f", 
            BackgroundColor = "#f8fafc", 
            FontFamily = "system-ui, sans-serif", 
            Layout = EntityLayer.Concrete.LayoutType.Grid 
        },
        new EntityLayer.Concrete.Theme 
        { 
            Name = "Ege Rüzgarı", 
            PrimaryColor = "#0284c7", 
            SecondaryColor = "#0369a1", 
            BackgroundColor = "#fcfcff", 
            FontFamily = "Georgia, serif", 
            Layout = EntityLayer.Concrete.LayoutType.Grid 
        },
        new EntityLayer.Concrete.Theme 
        { 
            Name = "Fütüristik Neon Diner", 
            PrimaryColor = "#00f0ff", 
            SecondaryColor = "#ff007f", 
            BackgroundColor = "#030008", 
            FontFamily = "monospace", 
            Layout = EntityLayer.Concrete.LayoutType.Grid 
        },
        new EntityLayer.Concrete.Theme 
        { 
            Name = "Sıcak Odun Ateşi", 
            PrimaryColor = "#c0603d", 
            SecondaryColor = "#5c3e31", 
            BackgroundColor = "#f9f3eb", 
            FontFamily = "Georgia, serif", 
            Layout = EntityLayer.Concrete.LayoutType.Grid 
        },
        new EntityLayer.Concrete.Theme 
        { 
            Name = "Lüks Akşam Yemeği", 
            PrimaryColor = "#f59e0b", 
            SecondaryColor = "#c29b5a", 
            BackgroundColor = "#0b0908", 
            FontFamily = "Georgia, serif", 
            Layout = EntityLayer.Concrete.LayoutType.Grid 
        },
        new EntityLayer.Concrete.Theme 
        { 
            Name = "Endüstriyel Minimalist", 
            PrimaryColor = "#111111", 
            SecondaryColor = "#78350f", 
            BackgroundColor = "#fcfcfc", 
            FontFamily = "system-ui, sans-serif", 
            Layout = EntityLayer.Concrete.LayoutType.Grid 
        },
        new EntityLayer.Concrete.Theme 
        { 
            Name = "Zümrüt Gurme", 
            PrimaryColor = "#006c49", 
            SecondaryColor = "#575e70", 
            BackgroundColor = "#f8f9fa", 
            FontFamily = "Sora, Inter, sans-serif", 
            Layout = EntityLayer.Concrete.LayoutType.Grid 
        },
        new EntityLayer.Concrete.Theme 
        { 
            Name = "Modern Asimetrik", 
            PrimaryColor = "#006c49", 
            SecondaryColor = "#10b981", 
            BackgroundColor = "#f8f9fa", 
            FontFamily = "Sora, Inter, sans-serif", 
            Layout = EntityLayer.Concrete.LayoutType.Grid 
        },
        new EntityLayer.Concrete.Theme 
        { 
            Name = "Zarif Noktalı", 
            PrimaryColor = "#006c49", 
            SecondaryColor = "#575e70", 
            BackgroundColor = "#f8f9fa", 
            FontFamily = "Sora, Inter, sans-serif", 
            Layout = EntityLayer.Concrete.LayoutType.List 
        }
    };

    foreach (var pt in premiumThemes)
    {
        var existingTheme = existing.FirstOrDefault(t => t.Name == pt.Name);
        if (existingTheme == null)
        {
            themeService.TInsert(pt);
        }
        else
        {
            existingTheme.PrimaryColor = pt.PrimaryColor;
            existingTheme.SecondaryColor = pt.SecondaryColor;
            existingTheme.BackgroundColor = pt.BackgroundColor;
            existingTheme.FontFamily = pt.FontFamily;
            existingTheme.Layout = pt.Layout;
            themeService.TUpdate(existingTheme);
        }
    }

    var updatedThemes = themeService.TGetListAll();
    var defaultTheme = updatedThemes.FirstOrDefault(t => t.Name == "Modern Minimalist") ?? updatedThemes.FirstOrDefault();
    var oldThemeNames = new HashSet<string> { "Doğal Yeşil", "Ateşli Turuncu", "Okyanus Mavisi", "Kocaoğlu Klasik" };
    var oldThemes = existing.Where(t => oldThemeNames.Contains(t.Name)).ToList();

    if (oldThemes.Any() && defaultTheme != null)
    {
        var oldThemeIds = oldThemes.Select(t => t.Id).ToHashSet();
        var restaurants = restaurantService.TGetListAll();
        
        foreach (var r in restaurants)
        {
            bool rUpdated = false;
            if (string.IsNullOrWhiteSpace(r.Slug))
            {
                r.Slug = dijitalmenu.Helpers.StringHelper.GenerateSlug(r.Name);
                if (string.IsNullOrWhiteSpace(r.Slug)) r.Slug = "restoran-" + r.Id;
                rUpdated = true;
            }

            if (oldThemeIds.Contains(r.ThemeId))
            {
                var oldTheme = oldThemes.First(ot => ot.Id == r.ThemeId);
                var targetThemeName = "Modern Minimalist";
                if (oldTheme.Name == "Ateşli Turuncu") targetThemeName = "Renkli & Eğlenceli";
                if (oldTheme.Name == "Okyanus Mavisi") targetThemeName = "Sıcak & Rustik";
                if (oldTheme.Name == "Kocaoğlu Klasik") targetThemeName = "Koyu & Lüks";

                var targetTheme = updatedThemes.FirstOrDefault(ut => ut.Name == targetThemeName) ?? defaultTheme;
                r.ThemeId = targetTheme.Id;
                rUpdated = true;
            }

            if (rUpdated)
            {
                restaurantService.TUpdate(r);
            }
        }
    }

    // Seed deniz1234 user & menu (Development only)
    if (app.Environment.IsDevelopment())
    {
    var denizUser = context.Users.FirstOrDefault(u => u.Username == "deniz1234");
    EntityLayer.Concrete.Menu denizMenu = null;

    if (denizUser == null)
    {
        var firstTheme = context.Themes.FirstOrDefault();
        var themeId = firstTheme?.Id ?? 1;

        var restaurant = new EntityLayer.Concrete.Restaurant
        {
            Name = "Deniz Restaurant",
            ThemeId = themeId
        };
        context.Restaurants.Add(restaurant);
        context.SaveChanges();

        denizUser = new EntityLayer.Concrete.User
        {
            Username = "deniz1234",
            Password = dijitalmenu.Helpers.PasswordHelper.Hash("deniz1234"),
            RestaurantId = restaurant.Id
        };
        context.Users.Add(denizUser);

        denizMenu = new EntityLayer.Concrete.Menu
        {
            RestaurantId = restaurant.Id
        };
        context.Menus.Add(denizMenu);
        context.SaveChanges();
    }
    else
    {
        denizMenu = context.Menus.FirstOrDefault(m => m.RestaurantId == denizUser.RestaurantId);
        if (denizMenu == null)
        {
            denizMenu = new EntityLayer.Concrete.Menu
            {
                RestaurantId = denizUser.RestaurantId
            };
            context.Menus.Add(denizMenu);
            context.SaveChanges();
        }
    }

    if (denizMenu != null && !context.Categories.Any(c => c.MenuId == denizMenu.Id))
    {
        var jsonPath = Path.Combine(app.Environment.WebRootPath, "demo", "deniz1234_menu.json");
        if (File.Exists(jsonPath))
        {
            var jsonStr = File.ReadAllText(jsonPath);
            using var doc = System.Text.Json.JsonDocument.Parse(jsonStr);
            var root = doc.RootElement;
            if (root.TryGetProperty("categories", out var categoriesElement))
            {
                foreach (var catElem in categoriesElement.EnumerateArray())
                {
                    var catName = catElem.GetProperty("name").GetString();
                    var category = new EntityLayer.Concrete.Category
                    {
                        Name = catName,
                        MenuId = denizMenu.Id
                    };
                    context.Categories.Add(category);
                    context.SaveChanges();

                    if (catElem.TryGetProperty("items", out var itemsElement))
                    {
                        foreach (var itemElem in itemsElement.EnumerateArray())
                        {
                            var itemName = itemElem.GetProperty("name").GetString();
                            var price = itemElem.GetProperty("price").GetDecimal();
                            var desc = itemElem.GetProperty("description").GetString() ?? "";

                            var menuItem = new EntityLayer.Concrete.MenuItem
                            {
                                Name = itemName,
                                Price = price,
                                Description = desc,
                                CategoryId = category.Id
                            };
                            context.MenuItems.Add(menuItem);
                        }
                        context.SaveChanges();
                    }
                }
            }
        }
    }
    } // end if (IsDevelopment) — deniz1234 seed
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

// Menu Route
app.MapControllerRoute(
    name: "menu",
    pattern: "Menu/{id?}",
    defaults: new { controller = "Home", action = "Menu" });

// Admin Area Route
app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

// Default Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

