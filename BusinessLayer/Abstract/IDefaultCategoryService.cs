using EntityLayer.Concrete;

namespace BusinessLayer.Abstract;

public interface IDefaultCategoryService : IGenericService<DefaultCategory>
{
    void TApplyToMenu(int menuId);
    void TApplyToAllMenus();
}
