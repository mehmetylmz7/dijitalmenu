using BusinessLayer.Abstract;
using dijitalmenu.Filters;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace dijitalmenu.Areas.Restaurant.Controllers
{
    [Area("Restaurant")]
    [ServiceFilter(typeof(RestaurantAuthFilter))]
    public class MenuItemController : Controller
    {
        private readonly IMenuItemService _menuItemService;
        private readonly ICategoryService _categoryService;
        private readonly IMenuService _menuService;

        public MenuItemController(IMenuItemService menuItemService, ICategoryService categoryService,
            IMenuService menuService)
        {
            _menuItemService = menuItemService;
            _categoryService = categoryService;
            _menuService = menuService;
        }

        private int GetRestaurantId() =>
            int.Parse(HttpContext.Session.GetString("RestaurantId")!);

        private List<Category> GetMyCategories()
        {
            var rid = GetRestaurantId();
            var menu = _menuService.TGetListAll().FirstOrDefault(m => m.RestaurantId == rid);
            if (menu == null) return new List<Category>();
            return _categoryService.TGetListAll().Where(c => c.MenuId == menu.Id).ToList();
        }

        public IActionResult Index()
        {
            var myCats = GetMyCategories();
            var catIds = myCats.Select(c => c.Id).ToHashSet();
            var items = _menuItemService.TGetListAll()
                .Where(mi => catIds.Contains(mi.CategoryId)).ToList();

            ViewBag.Categories = myCats;
            ViewBag.RestaurantUsername = HttpContext.Session.GetString("RestaurantUsername");
            return View(items);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var cats = GetMyCategories();
            if (!cats.Any())
            {
                TempData["Error"] = "Önce en az bir kategori eklemelisiniz.";
                return RedirectToAction("Index");
            }
            ViewBag.Categories = cats;
            ViewBag.RestaurantUsername = HttpContext.Session.GetString("RestaurantUsername");
            return View();
        }

        [HttpPost]
        public IActionResult Create(MenuItem menuItem)
        {
            var myCats = GetMyCategories();
            var catIds = myCats.Select(c => c.Id).ToHashSet();

            // Güvenlik: Kendi kategorisine mi ekliyor?
            if (!catIds.Contains(menuItem.CategoryId))
                return RedirectToAction("Index");

            _menuItemService.TInsert(menuItem);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var item = _menuItemService.TGetByID(id);
            var myCats = GetMyCategories();
            var catIds = myCats.Select(c => c.Id).ToHashSet();

            if (item == null || !catIds.Contains(item.CategoryId))
                return RedirectToAction("Index");

            ViewBag.Categories = myCats;
            ViewBag.RestaurantUsername = HttpContext.Session.GetString("RestaurantUsername");
            return View(item);
        }

        [HttpPost]
        public IActionResult Edit(MenuItem menuItem)
        {
            var myCats = GetMyCategories();
            var catIds = myCats.Select(c => c.Id).ToHashSet();

            if (!catIds.Contains(menuItem.CategoryId))
                return RedirectToAction("Index");

            _menuItemService.TUpdate(menuItem);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var item = _menuItemService.TGetByID(id);
            var myCats = GetMyCategories();
            var catIds = myCats.Select(c => c.Id).ToHashSet();

            if (item != null && catIds.Contains(item.CategoryId))
                _menuItemService.TDelete(item);

            return RedirectToAction("Index");
        }
    }
}
