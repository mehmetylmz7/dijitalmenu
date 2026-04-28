using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;

namespace BusinessLayer.Concrete
{
    public class ThemeManager : IThemeService
    {
        private readonly IThemeDal _themeDal;

        public ThemeManager(IThemeDal themeDal)
        {
            _themeDal = themeDal;
        }

        public void TDelete(Theme t)
        {
            _themeDal.Delete(t);
        }

        public Theme TGetByID(int id)
        {
            return _themeDal.GetByID(id);
        }

        public List<Theme> TGetListAll()
        {
            return _themeDal.GetListAll();
        }

        public void TInsert(Theme t)
        {
            _themeDal.Insert(t);
        }

        public void TUpdate(Theme t)
        {
            _themeDal.Update(t);
        }
    }
}
