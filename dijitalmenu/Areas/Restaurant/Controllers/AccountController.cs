using BusinessLayer.Abstract;
using dijitalmenu.Filters;
using dijitalmenu.Helpers;
using dijitalmenu.Models;
using dijitalmenu.Services;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace dijitalmenu.Areas.Restaurant.Controllers
{
    [Area("Restaurant")]
    [ServiceFilter(typeof(RestaurantAuthFilter))]
    public class AccountController : Controller
    {
        private const int MaxAddressLength = 500;
        private const int MaxPhoneLength = 25;

        private readonly IUserService _userService;
        private readonly IRestaurantService _restaurantService;
        private readonly IAuditContextService _auditContextService;

        public AccountController(
            IUserService userService,
            IRestaurantService restaurantService,
            IAuditContextService auditContextService)
        {
            _userService = userService;
            _restaurantService = restaurantService;
            _auditContextService = auditContextService;
        }

        private int GetCurrentUserId() =>
            int.Parse(HttpContext.Session.GetString("RestaurantUserId")!);

        private int GetCurrentRestaurantId() =>
            int.Parse(HttpContext.Session.GetString("RestaurantId")!);

        [HttpGet]
        public IActionResult Index()
        {
            var userId = GetCurrentUserId();
            var restaurantId = GetCurrentRestaurantId();

            var user = _userService.TGetByID(userId);
            var restaurant = _restaurantService.TGetByID(restaurantId);

            if (user == null || restaurant == null)
            {
                return RedirectToAction("Login", "Auth", new { area = "Restaurant" });
            }

            var model = new RestaurantAccountViewModel
            {
                Username = user.Username,
                RestaurantName = restaurant.Name,
                Slug = restaurant.Slug,
                Phone = restaurant.Phone,
                Address = restaurant.Address,
                GoogleMapsUrl = restaurant.GoogleMapsUrl,
                ImportantNotice = restaurant.ImportantNotice,
                WorkingHours = restaurant.WorkingHours
            };

            ViewBag.RestaurantUsername = user.Username;
            return View(model);
        }

        [HttpPost]
        public IActionResult UpdateProfile(string username)
        {
            var userId = GetCurrentUserId();
            var user = _userService.TGetByID(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Auth", new { area = "Restaurant" });
            }

            username = username?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(username) || username.Length < 3 || username.Length > 50)
            {
                TempData["Error"] = "Kullanıcı adı 3 ile 50 karakter arasında olmalıdır.";
                return RedirectToAction("Index");
            }

            if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_.-]+$"))
            {
                TempData["Error"] = "Kullanıcı adı sadece harf, rakam, nokta, tire ve alt çizgi içerebilir.";
                return RedirectToAction("Index");
            }

            var existingUser = _userService.TGetListAll()
                .FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && u.Id != userId);

            if (existingUser != null)
            {
                TempData["Error"] = "Bu kullanıcı adı zaten başka bir hesap tarafından kullanılıyor.";
                return RedirectToAction("Index");
            }

            var oldValues = new { user.Id, user.Username, user.RestaurantId };

            user.Username = username;
            _userService.TUpdate(user);

            var newValues = new { user.Id, user.Username, user.RestaurantId };

            _auditContextService.Log(
                action: "USER_UPDATED",
                entityType: "User",
                entityId: user.Id,
                description: $"Kullanıcı adı güncellendi: '{username}'",
                oldEntity: oldValues,
                newEntity: newValues
            );

            HttpContext.Session.SetString("RestaurantUsername", username);
            TempData["Success"] = "Profil bilgileriniz başarıyla güncellendi.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateBusiness(string restaurantName, string? phone, string? address, string? googleMapsUrl, string? importantNotice = null, string? workingHours = null)
        {
            var restaurantId = GetCurrentRestaurantId();
            var restaurant = _restaurantService.TGetByID(restaurantId);
            if (restaurant == null)
            {
                return RedirectToAction("Login", "Auth", new { area = "Restaurant" });
            }

            restaurantName = restaurantName?.Trim() ?? string.Empty;
            if (restaurantName.Length is < 2 or > 100)
            {
                TempData["Error"] = "Restoran adı 2 ile 100 karakter arasında olmalıdır.";
                return RedirectToAction("Index");
            }

            if (!string.IsNullOrWhiteSpace(googleMapsUrl) && googleMapsUrl.Contains("<iframe"))
            {
                var match = Regex.Match(googleMapsUrl, @"src=[""']([^""']+)[""']");
                if (match.Success)
                {
                    googleMapsUrl = match.Groups[1].Value;
                }
            }

            if (!TryNormalizeGoogleMapsUrl(googleMapsUrl, out var normalizedMapsUrl) ||
                (address?.Length ?? 0) > MaxAddressLength ||
                (phone?.Length ?? 0) > MaxPhoneLength ||
                (importantNotice?.Length ?? 0) > 1000 ||
                (workingHours?.Length ?? 0) > 200 ||
                (!string.IsNullOrWhiteSpace(phone) && !Regex.IsMatch(phone, @"^[0-9+()\-\s]{7,25}$")))
            {
                TempData["Error"] = "Konum, adres, çalışma saatleri veya telefon bilgisi geçersiz.";
                return RedirectToAction("Index");
            }

            var oldValues = new
            {
                restaurant.Id,
                restaurant.Name,
                restaurant.Phone,
                restaurant.Address,
                restaurant.GoogleMapsUrl,
                restaurant.ImportantNotice,
                restaurant.WorkingHours
            };

            restaurant.Name = restaurantName;
            restaurant.Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
            restaurant.Address = string.IsNullOrWhiteSpace(address) ? null : address.Trim();
            restaurant.GoogleMapsUrl = normalizedMapsUrl;
            restaurant.ImportantNotice = string.IsNullOrWhiteSpace(importantNotice) ? null : importantNotice.Trim();
            restaurant.WorkingHours = string.IsNullOrWhiteSpace(workingHours) ? null : workingHours.Trim();

            _restaurantService.TUpdate(restaurant);

            var newValues = new
            {
                restaurant.Id,
                restaurant.Name,
                restaurant.Phone,
                restaurant.Address,
                restaurant.GoogleMapsUrl,
                restaurant.ImportantNotice,
                restaurant.WorkingHours
            };

            _auditContextService.Log(
                action: "RESTAURANT_UPDATED",
                entityType: "Restaurant",
                entityId: restaurant.Id,
                description: $"Restoran işletme bilgileri güncellendi: '{restaurant.Name}'",
                oldEntity: oldValues,
                newEntity: newValues
            );

            TempData["Success"] = "İşletme bilgileriniz başarıyla güncellendi.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            var userId = GetCurrentUserId();
            var user = _userService.TGetByID(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Auth", new { area = "Restaurant" });
            }

            ViewBag.RestaurantUsername = user.Username;
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var userId = GetCurrentUserId();
            var user = _userService.TGetByID(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Auth", new { area = "Restaurant" });
            }

            if (string.IsNullOrEmpty(currentPassword) || !PasswordHelper.Verify(currentPassword, user.Password))
            {
                TempData["Error"] = "Mevcut şifreniz hatalı.";
                return RedirectToAction("ChangePassword");
            }

            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "Yeni şifre ile şifre tekrarı eşleşmiyor.";
                return RedirectToAction("ChangePassword");
            }

            if (newPassword == currentPassword)
            {
                TempData["Error"] = "Yeni şifre mevcut şifrenizle aynı olamaz.";
                return RedirectToAction("ChangePassword");
            }

            if (!IsValidPassword(newPassword, out var passwordError))
            {
                TempData["Error"] = passwordError;
                return RedirectToAction("ChangePassword");
            }

            user.Password = PasswordHelper.Hash(newPassword);
            _userService.TUpdate(user);

            // Audit Log: Password Changed (Never logging actual password values)
            _auditContextService.Log(
                action: "PASSWORD_CHANGED",
                entityType: "User",
                entityId: user.Id,
                description: $"Kullanıcı şifresini başarıyla değiştirdi: '{user.Username}'"
            );

            TempData["Success"] = "Şifreniz başarıyla değiştirildi.";
            return RedirectToAction("Index");
        }

        private static bool IsValidPassword(string password, out string error)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 12 ||
                !password.Any(char.IsUpper) || !password.Any(char.IsLower) ||
                !password.Any(char.IsDigit) || !password.Any(c => !char.IsLetterOrDigit(c)))
            {
                error = "Yeni şifre en az 12 karakter olmalı; büyük harf, küçük harf, rakam ve özel karakter içermelidir.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        private static bool TryNormalizeGoogleMapsUrl(string? value, out string? normalizedUrl)
        {
            normalizedUrl = null;
            if (string.IsNullOrWhiteSpace(value))
                return true;

            if (!Uri.TryCreate(value.Trim(), UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
                return false;

            var host = uri.Host.ToLowerInvariant();
            var isGoogleMapsHost = host == "maps.google.com" || host == "www.google.com" ||
                                   host.EndsWith(".google.com", StringComparison.Ordinal);
            if (!isGoogleMapsHost)
                return false;

            normalizedUrl = uri.AbsoluteUri;
            return true;
        }
    }
}
