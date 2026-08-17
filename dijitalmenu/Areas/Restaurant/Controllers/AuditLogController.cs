using BusinessLayer.Abstract;
using BusinessLayer.Models;
using dijitalmenu.Filters;
using Microsoft.AspNetCore.Mvc;
using System;

namespace dijitalmenu.Areas.Restaurant.Controllers
{
    [Area("Restaurant")]
    [ServiceFilter(typeof(RestaurantAuthFilter))]
    public class AuditLogController : Controller
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        private int GetRestaurantId() =>
            int.Parse(HttpContext.Session.GetString("RestaurantId")!);

        public IActionResult Index(
            DateTime? dateFrom,
            DateTime? dateTo,
            string? action,
            string? entityType,
            string? keyword,
            int page = 1,
            int pageSize = 20)
        {
            int restaurantId = GetRestaurantId();
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            // Multi-tenant security: RestaurantId is ALWAYS strictly bound to session
            var filter = new AuditLogFilterDto
            {
                DateFrom = dateFrom,
                DateTo = dateTo,
                RestaurantId = restaurantId,
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
            ViewBag.RestaurantUsername = HttpContext.Session.GetString("RestaurantUsername");

            return View(logs);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            int currentRestaurantId = GetRestaurantId();
            var log = _auditLogService.TGetByID(id);

            // Multi-tenant check
            if (log == null || log.RestaurantId != currentRestaurantId)
            {
                return NotFound();
            }

            return Json(new
            {
                id = log.Id,
                action = log.Action,
                entityType = log.EntityType,
                entityId = log.EntityId,
                username = log.Username,
                description = log.Description,
                ipAddress = log.IpAddress,
                requestPath = log.RequestPath,
                oldValues = log.OldValues,
                newValues = log.NewValues,
                createdAt = log.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss UTC")
            });
        }
    }
}
