using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;

namespace BusinessLayer.Concrete
{
    public class MenuItemManager : IMenuItemService
    {
        private readonly IMenuItemDal _menuItemDal;

        public MenuItemManager(IMenuItemDal menuItemDal)
        {
            _menuItemDal = menuItemDal;
        }

        public void TDelete(MenuItem t)
        {
            _menuItemDal.Delete(t);
        }

        public MenuItem TGetByID(int id)
        {
            return _menuItemDal.GetByID(id);
        }

        public List<MenuItem> TGetListAll()
        {
            return _menuItemDal.GetListAll();
        }

        public void TInsert(MenuItem t)
        {
            _menuItemDal.Insert(t);
        }

        public void TUpdate(MenuItem t)
        {
            _menuItemDal.Update(t);
        }
    }
}
