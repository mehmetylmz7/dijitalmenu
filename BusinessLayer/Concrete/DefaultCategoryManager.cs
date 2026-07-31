using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Concrete;

namespace BusinessLayer.Concrete;

public class DefaultCategoryManager : IDefaultCategoryService
{
    private readonly IDefaultCategoryDal _defaultCategoryDal;
    private readonly ICategoryDal _categoryDal;
    private readonly IMenuDal _menuDal;

    public DefaultCategoryManager(IDefaultCategoryDal defaultCategoryDal, ICategoryDal categoryDal, IMenuDal menuDal)
    {
        _defaultCategoryDal = defaultCategoryDal;
        _categoryDal = categoryDal;
        _menuDal = menuDal;
    }

    public void TApplyToMenu(int menuId)
    {
        var existingNames = _categoryDal.GetListAll()
            .Where(category => category.MenuId == menuId)
            .Select(category => category.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var defaultCategory in _defaultCategoryDal.GetListAll())
        {
            if (!existingNames.Contains(defaultCategory.Name))
            {
                _categoryDal.Insert(new Category { Name = defaultCategory.Name, MenuId = menuId });
                existingNames.Add(defaultCategory.Name);
            }
        }
    }

    public void TApplyToAllMenus()
    {
        foreach (var menu in _menuDal.GetListAll())
            TApplyToMenu(menu.Id);
    }

    public void TDelete(DefaultCategory category) => _defaultCategoryDal.Delete(category);
    public DefaultCategory TGetByID(int id) => _defaultCategoryDal.GetByID(id);
    public List<DefaultCategory> TGetListAll() => _defaultCategoryDal.GetListAll();
    public void TInsert(DefaultCategory category) => _defaultCategoryDal.Insert(category);
    public void TUpdate(DefaultCategory category) => _defaultCategoryDal.Update(category);
}
