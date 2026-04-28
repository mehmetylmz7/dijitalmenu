using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer.Repositories
{
    public class AdminRepository : IAdminDal
    {
        private readonly Context _context;

        public AdminRepository(Context context)
        {
            _context = context;
        }

        public void Delete(Admin t)
        {
            _context.Remove(t);
            _context.SaveChanges();
        }

        public Admin GetByID(int id)
        {
            return _context.Admins.Find(id);
        }

        public List<Admin> GetListAll()
        {
            return _context.Admins.ToList();
        }

        public void Insert(Admin t)
        {
            _context.Add(t);
            _context.SaveChanges();
        }

        public void Update(Admin t)
        {
            _context.Update(t);
            _context.SaveChanges();
        }
    }
}
