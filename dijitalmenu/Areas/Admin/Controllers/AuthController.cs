using BusinessLayer.Abstract;
using dijitalmenu.Helpers;
using dijitalmenu.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace dijitalmenu.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IAuditContextService _auditContextService;
        private readonly ILoginAttemptService? _loginAttemptService;

        public AuthController(
            IAdminService adminService,
            IAuditContextService auditContextService,
            ILoginAttemptService? loginAttemptService = null)
        {
            _adminService = adminService;
            _auditContextService = auditContextService;
            _loginAttemptService = loginAttemptService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("AdminUser")))
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

            return View();
        }

        [HttpPost]
        [EnableRateLimiting("login-policy")]
        public IActionResult Login(string username, string password)
        {
            var normalizedUsername = username?.Trim() ?? string.Empty;

            // Account-level progressive throttling (abuse & brute-force defense without permanent lockout)
            var attemptService = _loginAttemptService ?? (HttpContext?.RequestServices != null ? HttpContext.RequestServices.GetService<ILoginAttemptService>() : null);
            if (attemptService != null && attemptService.IsLockedOut(normalizedUsername))
            {
                var remaining = attemptService.GetLockoutRemaining(normalizedUsername);
                var seconds = remaining.HasValue ? (int)Math.Ceiling(remaining.Value.TotalSeconds) : 60;
                ViewBag.Error = $"Çok fazla başarısız giriş denemesi yapıldı. Lütfen {seconds} saniye sonra tekrar deneyiniz.";
                return View();
            }

            var admin = _adminService.TGetListAll()
                .FirstOrDefault(a => a.Username == normalizedUsername);

            if (admin != null && PasswordHelper.Verify(password, admin.Password))
            {
                attemptService?.ResetAttempts(normalizedUsername);

                if (PasswordHelper.NeedsRehash(admin.Password))
                {
                    admin.Password = PasswordHelper.Hash(password);
                    _adminService.TUpdate(admin);
                }

                // Session Fixation Protection: Clear existing unauthenticated session completely
                HttpContext.Session.Clear();
                HttpContext.Session.SetString("AdminUser", admin.Username);

                // Audit Log: Success
                _auditContextService.Log(
                    action: "LOGIN_SUCCESS",
                    entityType: "Admin",
                    entityId: admin.Id,
                    adminId: admin.Id,
                    username: admin.Username,
                    description: $"Admin paneline başarılı giriş yapıldı: '{admin.Username}'"
                );

                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }

            attemptService?.RecordFailedAttempt(normalizedUsername);

            // Audit Log: Failed (never logging password)
            _auditContextService.Log(
                action: "LOGIN_FAILED",
                entityType: "Admin",
                entityId: admin?.Id,
                adminId: admin?.Id,
                username: normalizedUsername,
                description: $"Admin paneline hatalı şifre veya kullanıcı adı ile giriş denemesi: '{normalizedUsername}'"
            );

            _auditContextService.CheckAndTriggerFailedLoginAlert(normalizedUsername);

            ViewBag.Error = "Kullanıcı adı veya şifre hatalı.";
            return View();
        }

        [HttpPost]
        public IActionResult Logout()
        {
            var adminUser = HttpContext.Session.GetString("AdminUser");

            _auditContextService.Log(
                action: "LOGOUT",
                entityType: "Admin",
                username: adminUser,
                description: $"Admin oturumu sonlandırıldı: '{adminUser}'"
            );

            // Clear authentication state completely
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth", new { area = "Admin" });
        }
    }
}
