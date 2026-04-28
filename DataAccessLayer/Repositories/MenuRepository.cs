using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer.Repositories
{
    public class MenuRepository : IMenuDal
    {
        private readonly Context _context;

        public MenuRepository(Context context)
        {
            _context = context;
        }

        public void Delete(Menu t)
        {
            _context.Remove(t);
            _context.SaveChanges();
        }

        public Menu GetByID(int id)
        {
            return _context.Menus.Find(id);
        }

        public List<Menu> GetListAll()
        {
            return _context.Menus.ToList();
        }

        public void Insert(Menu t)
        {
            _context.Add(t);
            _context.SaveChanges();
        }

        public void Update(Menu t)
        {
            _context.Update(t);
            _context.SaveChanges();
        }
    }
}
