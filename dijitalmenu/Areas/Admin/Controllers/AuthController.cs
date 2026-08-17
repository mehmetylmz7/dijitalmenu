using BusinessLayer.Abstract;
using dijitalmenu.Helpers;
using dijitalmenu.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace dijitalmenu.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IAuditContextService _auditContextService;

        public AuthController(IAdminService adminService, IAuditContextService auditContextService)
        {
            _adminService = adminService;
            _auditContextService = auditContextService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("AdminUser")))
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var normalizedUsername = username?.Trim() ?? string.Empty;
            var admin = _adminService.TGetListAll()
                .FirstOrDefault(a => a.Username == normalizedUsername);

            if (admin != null && PasswordHelper.Verify(password, admin.Password))
            {
                if (PasswordHelper.NeedsRehash(admin.Password))
                {
                    admin.Password = PasswordHelper.Hash(password);
                    _adminService.TUpdate(admin);
                }

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

            HttpContext.Session.Remove("AdminUser");
            return RedirectToAction("Login", "Auth", new { area = "Admin" });
        }
    }
}
