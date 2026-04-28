using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer.Repositories
{
    public class ThemeRepository : IThemeDal
    {
        private readonly Context _context;

        public ThemeRepository(Context context)
        {
            _context = context;
        }

        public void Delete(Theme t)
        {
            _context.Remove(t);
            _context.SaveChanges();
        }

        public Theme GetByID(int id)
        {
            return _context.Themes.Find(id);
        }

        public List<Theme> GetListAll()
        {
            return _context.Themes.ToList();
        }

        public void Insert(Theme t)
        {
            _context.Add(t);
            _context.SaveChanges();
        }

        public void Update(Theme t)
        {
            _context.Update(t);
            _context.SaveChanges();
        }
    }
}
