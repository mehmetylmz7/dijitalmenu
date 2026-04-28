using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;

namespace BusinessLayer.Concrete
{
    public class MenuManager : IMenuService
    {
        private readonly IMenuDal _menuDal;

        public MenuManager(IMenuDal menuDal)
        {
            _menuDal = menuDal;
        }

        public void TDelete(Menu t)
        {
            _menuDal.Delete(t);
        }

        public Menu TGetByID(int id)
        {
            return _menuDal.GetByID(id);
        }

        public List<Menu> TGetListAll()
        {
            return _menuDal.GetListAll();
        }

        public void TInsert(Menu t)
        {
            _menuDal.Insert(t);
        }

        public void TUpdate(Menu t)
        {
            _menuDal.Update(t);
        }
    }
}
