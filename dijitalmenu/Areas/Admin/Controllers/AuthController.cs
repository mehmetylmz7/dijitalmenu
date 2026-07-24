using BusinessLayer.Abstract;
using dijitalmenu.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace dijitalmenu.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthController : Controller
    {
        private readonly IAdminService _adminService;

        public AuthController(IAdminService adminService)
        {
            _adminService = adminService;
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
            var admin = _adminService.TGetListAll()
                .FirstOrDefault(a => a.Username == username);

            if (admin != null && PasswordHelper.Verify(password, admin.Password))
            {
                if (PasswordHelper.NeedsRehash(admin.Password))
                {
                    admin.Password = PasswordHelper.Hash(password);
                    _adminService.TUpdate(admin);
                }

                HttpContext.Session.SetString("AdminUser", admin.Username);
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }

            ViewBag.Error = "Kullanıcı adı veya şifre hatalı.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminUser");
            return RedirectToAction("Login", "Auth", new { area = "Admin" });
        }
    }
}
