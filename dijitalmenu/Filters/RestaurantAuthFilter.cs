using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace dijitalmenu.Filters
{
    public class RestaurantAuthFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = context.HttpContext.Session.GetString("RestaurantUserId");
            var restaurantId = context.HttpContext.Session.GetString("RestaurantId");

            if (!int.TryParse(userId, out var parsedUserId) || parsedUserId <= 0 ||
                !int.TryParse(restaurantId, out var parsedRestaurantId) || parsedRestaurantId <= 0)
            {
                context.HttpContext.Session.Clear();
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
