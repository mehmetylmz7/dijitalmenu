using BusinessLayer.Abstract;
using dijitalmenu.Filters;
using Microsoft.AspNetCore.Mvc;

namespace dijitalmenu.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ServiceFilter(typeof(AdminAuthFilter))]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public IActionResult Index(bool? unreadOnly)
        {
            var notifications = _notificationService.GetNotifications(restaurantId: null, unreadOnly: unreadOnly, limit: 100);
            ViewBag.UnreadCount = _notificationService.GetUnreadCount(null);
            ViewBag.UnreadOnly = unreadOnly ?? false;
            ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");

            return View(notifications);
        }

        [HttpPost]
        public IActionResult MarkAsRead(int id)
        {
            _notificationService.MarkAsRead(id);
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult MarkAllAsRead()
        {
            _notificationService.MarkAllAsRead(null);
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetUnreadCount()
        {
            int count = _notificationService.GetUnreadCount(null);
            return Json(new { count });
        }
    }
}
