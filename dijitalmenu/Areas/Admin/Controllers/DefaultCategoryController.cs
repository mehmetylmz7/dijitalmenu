using BusinessLayer.Abstract;
using dijitalmenu.Filters;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace dijitalmenu.Areas.Admin.Controllers;

[Area("Admin")]
[ServiceFilter(typeof(AdminAuthFilter))]
public class DefaultCategoryController : Controller
{
    private readonly IDefaultCategoryService _defaultCategoryService;

    public DefaultCategoryController(IDefaultCategoryService defaultCategoryService) =>
        _defaultCategoryService = defaultCategoryService;

    public IActionResult Index() => View(_defaultCategoryService.TGetListAll());

    [HttpPost]
    public IActionResult Create(string name)
    {
        name = name?.Trim() ?? string.Empty;
        if (name.Length is < 1 or > 100 || _defaultCategoryService.TGetListAll()
                .Any(category => category.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            TempData["Error"] = "Kategori adı geçersiz veya zaten mevcut.";
            return RedirectToAction(nameof(Index));
        }

        _defaultCategoryService.TInsert(new DefaultCategory { Name = name });
        _defaultCategoryService.TApplyToAllMenus();
        TempData["Success"] = "Varsayılan kategori tüm mevcut restoran menülerine eklendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var category = _defaultCategoryService.TGetByID(id);
        if (category != null)
            _defaultCategoryService.TDelete(category);

        TempData["Success"] = "Varsayılan kategori kaldırıldı. Mevcut restoranlardaki kopyalar korunur.";
        return RedirectToAction(nameof(Index));
    }
}
