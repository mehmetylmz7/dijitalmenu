using BusinessLayer.Abstract;
using DataAccessLayer.Concrete;
using dijitalmenu.Helpers;
using dijitalmenu.Services;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace dijitalmenu.Areas.Restaurant.Controllers
{
    [Area("Restaurant")]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRestaurantService _restaurantService;
        private readonly IMenuService _menuService;
        private readonly IThemeService _themeService;
        private readonly IDefaultCategoryService _defaultCategoryService;
        private readonly IAuditContextService _auditContextService;
        private readonly INotificationService _notificationService;
        private readonly Context _context;

        public AuthController(
            IUserService userService,
            IRestaurantService restaurantService,
            IMenuService menuService,
            IThemeService themeService,
            IDefaultCategoryService defaultCategoryService,
            IAuditContextService auditContextService,
            INotificationService notificationService,
            Context context)
        {
            _userService = userService;
            _restaurantService = restaurantService;
            _menuService = menuService;
            _themeService = themeService;
            _defaultCategoryService = defaultCategoryService;
            _auditContextService = auditContextService;
            _notificationService = notificationService;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("RestaurantUserId")))
                return RedirectToAction("Index", "Dashboard", new { area = "Restaurant" });

            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var normalizedUsername = username?.Trim() ?? string.Empty;
            var user = _userService.TGetListAll()
                .FirstOrDefault(item => item.Username.Equals(normalizedUsername, StringComparison.OrdinalIgnoreCase));

            if (user != null && PasswordHelper.Verify(password, user.Password))
            {
                if (PasswordHelper.NeedsRehash(user.Password))
                {
                    user.Password = PasswordHelper.Hash(password);
                    _userService.TUpdate(user);
                }

                SignIn(user);

                // Audit Log: Success
                _auditContextService.Log(
                    action: "LOGIN_SUCCESS",
                    entityType: "User",
                    entityId: user.Id,
                    restaurantId: user.RestaurantId,
                    userId: user.Id,
                    username: user.Username,
                    description: $"Restoran kullanıcısı başarılı giriş yaptı: '{user.Username}'"
                );

                return RedirectToAction("Index", "Dashboard", new { area = "Restaurant" });
            }

            // Audit Log: Failed (never logging password)
            _auditContextService.Log(
                action: "LOGIN_FAILED",
                entityType: "User",
                entityId: user?.Id,
                restaurantId: user?.RestaurantId,
                userId: user?.Id,
                username: normalizedUsername,
                description: $"Restoran paneline hatalı şifre veya kullanıcı adı ile giriş denemesi: '{normalizedUsername}'"
            );

            _auditContextService.CheckAndTriggerFailedLoginAlert(normalizedUsername);

            ViewBag.Error = "Kullanıcı adı veya şifre hatalı.";
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("RestaurantUserId")))
                return RedirectToAction("Index", "Dashboard", new { area = "Restaurant" });

            PopulateThemes();
            return View();
        }

        [HttpPost]
        public IActionResult Register(string restaurantName, string username, string password, int themeId = 0)
        {
            restaurantName = restaurantName?.Trim() ?? string.Empty;
            username = username?.Trim() ?? string.Empty;

            if (!IsValidRegistration(restaurantName, username, password, out var validationError))
            {
                ViewBag.Error = validationError;
                PopulateThemes();
                return View();
            }

            var existingUser = _userService.TGetListAll()
                .FirstOrDefault(item => item.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (existingUser != null)
            {
                ViewBag.Error = "Bu kullanıcı adı zaten alınmış.";
                PopulateThemes();
                return View();
            }

            var themes = _themeService.TGetListAll().Where(t => t.IsActive).OrderBy(theme => theme.Id).ToList();
            var selectedTheme = themeId > 0
                ? themes.FirstOrDefault(theme => theme.Id == themeId)
                : themes.FirstOrDefault();
            if (selectedTheme == null)
            {
                ViewBag.Error = "Geçerli bir menü teması seçmelisiniz.";
                PopulateThemes(themes);
                return View();
            }

            try
            {
                var restaurant = new EntityLayer.Concrete.Restaurant
                {
                    Name = restaurantName,
                    ThemeId = selectedTheme.Id
                };
                _restaurantService.TInsert(restaurant);

                var user = new User
                {
                    Username = username,
                    Password = PasswordHelper.Hash(password),
                    RestaurantId = restaurant.Id
                };
                _userService.TInsert(user);

                var menu = new Menu { RestaurantId = restaurant.Id };
                _menuService.TInsert(menu);
                _defaultCategoryService.TApplyToMenu(menu.Id);

                SignIn(user);

                // Audit Log: Restaurant Registration
                _auditContextService.Log(
                    action: "RESTAURANT_CREATED",
                    entityType: "Restaurant",
                    entityId: restaurant.Id,
                    restaurantId: restaurant.Id,
                    userId: user.Id,
                    username: user.Username,
                    description: $"Yeni restoran ve kullanıcı kaydı oluşturuldu: '{restaurant.Name}' (Kullanıcı: {user.Username})"
                );

                // Notification for Admin
                _notificationService.CreateNotification(
                    title: "Yeni Restoran Kaydı",
                    message: $"'{restaurant.Name}' isimli yeni bir restoran sisteme kaydoldu.",
                    type: "Info",
                    restaurantId: restaurant.Id,
                    userId: user.Id
                );

                return RedirectToAction("Index", "Dashboard", new { area = "Restaurant" });
            }
            catch (DbUpdateException)
            {
                ViewBag.Error = "Kayıt tamamlanamadı. Kullanıcı adı zaten alınmış olabilir; lütfen tekrar deneyin.";
                PopulateThemes(themes);
                return View();
            }
        }

        [HttpPost]
        [HttpGet]
        public IActionResult Logout()
        {
            var userIdStr = HttpContext.Session.GetString("RestaurantUserId");
            var restIdStr = HttpContext.Session.GetString("RestaurantId");
            var username = HttpContext.Session.GetString("RestaurantUsername");

            int? userId = int.TryParse(userIdStr, out var u) ? u : null;
            int? restId = int.TryParse(restIdStr, out var r) ? r : null;

            if (userId.HasValue || !string.IsNullOrEmpty(username))
            {
                _auditContextService.Log(
                    action: "LOGOUT",
                    entityType: "User",
                    entityId: userId,
                    restaurantId: restId,
                    userId: userId,
                    username: username,
                    description: $"Restoran kullanıcısı oturumu kapattı: '{username}'"
                );
            }

            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth", new { area = "Restaurant" });
        }

        private void SignIn(User user)
        {
            HttpContext.Session.SetString("RestaurantUserId", user.Id.ToString());
            HttpContext.Session.SetString("RestaurantId", user.RestaurantId.ToString());
            HttpContext.Session.SetString("RestaurantUsername", user.Username);
        }

        private void PopulateThemes(List<Theme>? themes = null) =>
            ViewBag.Themes = themes ?? _themeService.TGetListAll().Where(t => t.IsActive).OrderBy(theme => theme.Id).ToList();

        private static bool IsValidRegistration(string restaurantName, string username, string password, out string error)
        {
            if (restaurantName.Length is < 2 or > 100)
            {
                error = "Restoran adı 2 ile 100 karakter arasında olmalıdır.";
                return false;
            }

            if (username.Length is < 3 or > 50 || !username.All(character => char.IsLetterOrDigit(character) || character is '.' or '_' or '-'))
            {
                error = "Kullanıcı adı 3 ile 50 karakter arasında olmalı; yalnızca harf, rakam, nokta, alt çizgi ve tire içermelidir.";
                return false;
            }

            if (password.Length < 12 || !password.Any(char.IsUpper) || !password.Any(char.IsLower) ||
                !password.Any(char.IsDigit) || !password.Any(character => !char.IsLetterOrDigit(character)))
            {
                error = "Şifre en az 12 karakter olmalı; büyük harf, küçük harf, rakam ve özel karakter içermelidir.";
                return false;
            }

            error = string.Empty;
            return true;
        }
    }
}
