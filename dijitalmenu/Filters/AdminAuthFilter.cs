using BusinessLayer.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace dijitalmenu.Filters
{
    public class AdminAuthFilter : IActionFilter
    {
        private readonly IAdminService? _adminService;

        public AdminAuthFilter(IAdminService? adminService = null)
        {
            _adminService = adminService;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var adminUser = context.HttpContext.Session.GetString("AdminUser");
            if (string.IsNullOrWhiteSpace(adminUser))
            {
                context.HttpContext.Session.Clear();
                context.Result = new RedirectToRouteResult(new Microsoft.AspNetCore.Routing.RouteValueDictionary
                {
                    { "area", "Admin" },
                    { "controller", "Auth" },
                    { "action", "Login" }
                });
                return;
            }

            var adminService = _adminService ?? (context.HttpContext.RequestServices != null ? context.HttpContext.RequestServices.GetService<IAdminService>() : null);
            if (adminService != null)
            {
                var exists = adminService.TGetListAll().Any(a => a.Username == adminUser);
                if (!exists)
                {
                    context.HttpContext.Session.Clear();
                    context.Result = new RedirectToRouteResult(new Microsoft.AspNetCore.Routing.RouteValueDictionary
                    {
                        { "area", "Admin" },
                        { "controller", "Auth" },
                        { "action", "Login" }
                    });
                    return;
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
