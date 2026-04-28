using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer.Repositories
{
    public class MenuItemRepository : IMenuItemDal
    {
        private readonly Context _context;

        public MenuItemRepository(Context context)
        {
            _context = context;
        }

        public void Delete(MenuItem t)
        {
            _context.Remove(t);
            _context.SaveChanges();
        }

        public MenuItem GetByID(int id)
        {
            return _context.MenuItems.Find(id);
        }

        public List<MenuItem> GetListAll()
        {
            return _context.MenuItems.ToList();
        }

        public void Insert(MenuItem t)
        {
            _context.Add(t);
            _context.SaveChanges();
        }

        public void Update(MenuItem t)
        {
            _context.Update(t);
            _context.SaveChanges();
        }
    }
}
