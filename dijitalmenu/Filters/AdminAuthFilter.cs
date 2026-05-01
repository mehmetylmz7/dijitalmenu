using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace dijitalmenu.Filters
{
    public class AdminAuthFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session.GetString("AdminUser");
            if (string.IsNullOrEmpty(session))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "area", "Admin" },
                    { "controller", "Auth" },
                    { "action", "Login" }
                });
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
