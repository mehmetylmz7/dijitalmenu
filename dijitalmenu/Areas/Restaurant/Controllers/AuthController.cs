using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace dijitalmenu.Areas.Restaurant.Controllers
{
    [Area("Restaurant")]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRestaurantService _restaurantService;
        private readonly IMenuService _menuService;
        private readonly IThemeService _themeService;

        public AuthController(IUserService userService, IRestaurantService restaurantService,
            IMenuService menuService, IThemeService themeService)
        {
            _userService = userService;
            _restaurantService = restaurantService;
            _menuService = menuService;
            _themeService = themeService;
        }

        // GET: /Restaurant/Auth/Login
        [HttpGet]
        public IActionResult Login()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("RestaurantUserId")))
                return RedirectToAction("Index", "Dashboard", new { area = "Restaurant" });

            return View();
        }


        // POST: /Restaurant/Auth/Login
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _userService.TGetListAll()
                .FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                HttpContext.Session.SetString("RestaurantUserId", user.Id.ToString());
                HttpContext.Session.SetString("RestaurantId", user.RestaurantId.ToString());
                HttpContext.Session.SetString("RestaurantUsername", user.Username);
                return RedirectToAction("Index", "Dashboard", new { area = "Restaurant" });
            }

            ViewBag.Error = "Kullanıcı adı veya şifre hatalı.";
            return View();
        }

        // GET: /Restaurant/Auth/Register
        [HttpGet]
        public IActionResult Register()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("RestaurantUserId")))
                return RedirectToAction("Index", "Dashboard", new { area = "Restaurant" });

            return View();
        }

        // POST: /Restaurant/Auth/Register
        [HttpPost]
        public IActionResult Register(string restaurantName, string username, string password, int themeId)
        {
            // Kullanıcı adı kontrolü
            var existing = _userService.TGetListAll().FirstOrDefault(u => u.Username == username);
            if (existing != null)
            {
                ViewBag.Error = "Bu kullanıcı adı zaten alınmış.";
                return View();
            }

            // 1. Restoran oluştur
            var restaurant = new EntityLayer.Concrete.Restaurant
            {
                Name = restaurantName,
                ThemeId = themeId
            };
            _restaurantService.TInsert(restaurant);

            // 2. Kullanıcı oluştur
            var user = new User
            {
                Username = username,
                Password = password,
                RestaurantId = restaurant.Id
            };
            _userService.TInsert(user);

            // 3. Boş menü oluştur
            var menu = new Menu { RestaurantId = restaurant.Id };
            _menuService.TInsert(menu);

            // 4. Otomatik giriş
            HttpContext.Session.SetString("RestaurantUserId", user.Id.ToString());
            HttpContext.Session.SetString("RestaurantId", restaurant.Id.ToString());
            HttpContext.Session.SetString("RestaurantUsername", user.Username);

            return RedirectToAction("Index", "Dashboard", new { area = "Restaurant" });
        }

        // GET: /Restaurant/Auth/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("RestaurantUserId");
            HttpContext.Session.Remove("RestaurantId");
            HttpContext.Session.Remove("RestaurantUsername");
            return RedirectToAction("Login", "Auth", new { area = "Restaurant" });
        }
    }
}
