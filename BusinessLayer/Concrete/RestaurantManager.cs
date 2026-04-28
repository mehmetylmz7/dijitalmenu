using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;

namespace BusinessLayer.Concrete
{
    public class RestaurantManager : IRestaurantService
    {
        private readonly IRestaurantDal _restaurantDal;

        public RestaurantManager(IRestaurantDal restaurantDal)
        {
            _restaurantDal = restaurantDal;
        }

        public void TDelete(Restaurant t)
        {
            _restaurantDal.Delete(t);
        }

        public Restaurant TGetByID(int id)
        {
            return _restaurantDal.GetByID(id);
        }

        public List<Restaurant> TGetListAll()
        {
            return _restaurantDal.GetListAll();
        }

        public void TInsert(Restaurant t)
        {
            _restaurantDal.Insert(t);
        }

        public void TUpdate(Restaurant t)
        {
            _restaurantDal.Update(t);
        }
    }
}
