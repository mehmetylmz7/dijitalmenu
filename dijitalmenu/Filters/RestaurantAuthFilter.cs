using BusinessLayer.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace dijitalmenu.Filters
{
    public class RestaurantAuthFilter : IActionFilter
    {
        private readonly IUserService? _userService;

        public RestaurantAuthFilter(IUserService? userService = null)
        {
            _userService = userService;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = context.HttpContext.Session.GetString("RestaurantUserId");
            var restaurantId = context.HttpContext.Session.GetString("RestaurantId");

            if (!int.TryParse(userId, out var parsedUserId) || parsedUserId <= 0 ||
                !int.TryParse(restaurantId, out var parsedRestaurantId) || parsedRestaurantId <= 0)
            {
                context.HttpContext.Session.Clear();
                context.Result = new RedirectToRouteResult(new Microsoft.AspNetCore.Routing.RouteValueDictionary
                {
                    { "area", "Restaurant" },
                    { "controller", "Auth" },
                    { "action", "Login" }
                });
                return;
            }

            // Server-side tenant verification: Session RestaurantUserId -> DB User -> User.RestaurantId == Session RestaurantId
            var userService = _userService ?? (context.HttpContext.RequestServices != null ? context.HttpContext.RequestServices.GetService<IUserService>() : null);
            if (userService != null)
            {
                var user = userService.TGetByID(parsedUserId);
                if (user == null || user.RestaurantId != parsedRestaurantId)
                {
                    context.HttpContext.Session.Clear();
                    context.Result = new RedirectToRouteResult(new Microsoft.AspNetCore.Routing.RouteValueDictionary
                    {
                        { "area", "Restaurant" },
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
