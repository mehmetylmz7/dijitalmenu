using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace dijitalmenu.Filters
{
    public class RestaurantAuthFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = context.HttpContext.Session.GetString("RestaurantUserId");
            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "area", "Restaurant" },
                    { "controller", "Auth" },
                    { "action", "Login" }
                });
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
