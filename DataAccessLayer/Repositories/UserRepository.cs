using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer.Repositories
{
    public class UserRepository : IUserDal
    {
        private readonly Context _context;

        public UserRepository(Context context)
        {
            _context = context;
        }

        public void Delete(User t)
        {
            _context.Remove(t);
            _context.SaveChanges();
        }

        public User GetByID(int id)
        {
            return _context.Users.Find(id);
        }

        public List<User> GetListAll()
        {
            return _context.Users.ToList();
        }

        public void Insert(User t)
        {
            _context.Add(t);
            _context.SaveChanges();
        }

        public void Update(User t)
        {
            _context.Update(t);
            _context.SaveChanges();
        }
    }
}
