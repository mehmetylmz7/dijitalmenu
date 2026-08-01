using BusinessLayer.Abstract;
using dijitalmenu.Filters;
using Microsoft.AspNetCore.Mvc;

namespace dijitalmenu.Areas.Restaurant.Controllers
{
    [Area("Restaurant")]
    [ServiceFilter(typeof(RestaurantAuthFilter))]
    public class DashboardController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IMenuItemService _menuItemService;
        private readonly IMenuService _menuService;
        private readonly IRestaurantService _restaurantService;
        private readonly IConfiguration _configuration;

        public DashboardController(ICategoryService categoryService, IMenuItemService menuItemService,
            IMenuService menuService, IRestaurantService restaurantService, IConfiguration configuration)
        {
            _categoryService = categoryService;
            _menuItemService = menuItemService;
            _menuService = menuService;
            _restaurantService = restaurantService;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var restaurantId = int.Parse(HttpContext.Session.GetString("RestaurantId")!);
            var restaurant = _restaurantService.TGetByID(restaurantId);
            var menu = _menuService.TGetListAll().FirstOrDefault(m => m.RestaurantId == restaurantId);

            int catCount = 0, itemCount = 0;
            if (menu != null)
            {
                var cats = _categoryService.TGetListAll().Where(c => c.MenuId == menu.Id).ToList();
                catCount = cats.Count;
                var catIds = cats.Select(c => c.Id).ToHashSet();
                itemCount = _menuItemService.TGetListAll().Count(mi => catIds.Contains(mi.CategoryId));
            }

            ViewBag.RestaurantName = restaurant?.Name;
            ViewBag.RestaurantUsername = HttpContext.Session.GetString("RestaurantUsername");
            ViewBag.CategoryCount = catCount;
            ViewBag.MenuItemCount = itemCount;

            if (restaurant != null)
            {
                string appUrl = _configuration["AppUrl"];
                string baseUrl = !string.IsNullOrWhiteSpace(appUrl) ? appUrl : $"{Request.Scheme}://{Request.Host}";
                string qrUrl = $"{baseUrl.TrimEnd('/')}/Menu/{restaurant.Slug}";

                using var qrGenerator = new QRCoder.QRCodeGenerator();
                using var qrCodeData = qrGenerator.CreateQrCode(qrUrl, QRCoder.QRCodeGenerator.ECCLevel.Q);
                using var qrCode = new QRCoder.PngByteQRCode(qrCodeData);
                byte[] qrCodeImage = qrCode.GetGraphic(20);

                ViewBag.QrCodeImage = "data:image/png;base64," + Convert.ToBase64String(qrCodeImage);
                ViewBag.MenuUrl = qrUrl;
                ViewBag.PublicQrImageUrl = $"{baseUrl.TrimEnd('/')}/Menu/{restaurant.Slug}/qr";
            }

            return View();
        }
    }
}
