using BusinessLayer.Abstract;
using BusinessLayer.Models;
using dijitalmenu.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace dijitalmenu.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ServiceFilter(typeof(AdminAuthFilter))]
    public class AuditLogController : Controller
    {
        private readonly IAuditLogService _auditLogService;
        private readonly IRestaurantService _restaurantService;
        private readonly IUserService _userService;
        private readonly IAdminService _adminService;

        public AuditLogController(
            IAuditLogService auditLogService,
            IRestaurantService restaurantService,
            IUserService userService,
            IAdminService adminService)
        {
            _auditLogService = auditLogService;
            _restaurantService = restaurantService;
            _userService = userService;
            _adminService = adminService;
        }

        public IActionResult Index(
            DateTime? dateFrom,
            DateTime? dateTo,
            int? restaurantId,
            int? userId,
            int? adminId,
            string? action,
            string? entityType,
            string? keyword,
            int page = 1,
            int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var filter = new AuditLogFilterDto
            {
                DateFrom = dateFrom,
                DateTo = dateTo,
                RestaurantId = restaurantId,
                UserId = userId,
                AdminId = adminId,
                Action = action,
                EntityType = entityType,
                Keyword = keyword,
                Page = page,
                PageSize = pageSize
            };

            var (logs, totalCount) = _auditLogService.GetFilteredLogs(filter);

            ViewBag.Filter = filter;
            ViewBag.TotalCount = totalCount;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            ViewBag.Restaurants = _restaurantService.TGetListAll().OrderBy(r => r.Name).ToList();
            ViewBag.Users = _userService.TGetListAll().OrderBy(u => u.Username).ToList();
            ViewBag.Admins = _adminService.TGetListAll().OrderBy(a => a.Username).ToList();

            ViewBag.AdminUser = HttpContext.Session.GetString("AdminUser");

            return View(logs);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var log = _auditLogService.TGetByID(id);
            if (log == null)
            {
                return NotFound();
            }

            return Json(new
            {
                id = log.Id,
                action = log.Action,
                entityType = log.EntityType,
                entityId = log.EntityId,
                restaurant = log.Restaurant?.Name,
                restaurantId = log.RestaurantId,
                user = log.User?.Username,
                userId = log.UserId,
                admin = log.Admin?.Username,
                adminId = log.AdminId,
                username = log.Username,
                description = log.Description,
                ipAddress = log.IpAddress,
                userAgent = log.UserAgent,
                requestPath = log.RequestPath,
                oldValues = log.OldValues,
                newValues = log.NewValues,
                createdAt = log.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss UTC")
            });
        }
    }
}
