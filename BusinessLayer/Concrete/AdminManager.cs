using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;

namespace BusinessLayer.Concrete
{
    public class AdminManager : IAdminService
    {
        private readonly IAdminDal _adminDal;

        public AdminManager(IAdminDal adminDal)
        {
            _adminDal = adminDal;
        }

        public void TDelete(Admin t)
        {
            _adminDal.Delete(t);
        }

        public Admin TGetByID(int id)
        {
            return _adminDal.GetByID(id);
        }

        public List<Admin> TGetListAll()
        {
            return _adminDal.GetListAll();
        }

        public void TInsert(Admin t)
        {
            _adminDal.Insert(t);
        }

        public void TUpdate(Admin t)
        {
            _adminDal.Update(t);
        }
    }
}
