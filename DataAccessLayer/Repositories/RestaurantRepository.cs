using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer.Repositories
{
    public class RestaurantRepository : IRestaurantDal
    {
        private readonly Context _context;

        public RestaurantRepository(Context context)
        {
            _context = context;
        }

        public void Delete(Restaurant t)
        {
            _context.Remove(t);
            _context.SaveChanges();
        }

        public Restaurant GetByID(int id)
        {
            return _context.Restaurants.Find(id);
        }

        public List<Restaurant> GetListAll()
        {
            return _context.Restaurants.ToList();
        }

        public void Insert(Restaurant t)
        {
            _context.Add(t);
            _context.SaveChanges();
        }

        public void Update(Restaurant t)
        {
            _context.Update(t);
            _context.SaveChanges();
        }
    }
}
