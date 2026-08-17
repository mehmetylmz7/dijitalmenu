using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer.Repositories
{
    public class NotificationRepository : INotificationDal
    {
        private readonly Context _context;

        public NotificationRepository(Context context)
        {
            _context = context;
        }

        public void Delete(Notification t)
        {
            _context.Notifications.Remove(t);
            _context.SaveChanges();
        }

        public Notification? GetByID(int id)
        {
            return _context.Notifications
                .Include(n => n.Restaurant)
                .Include(n => n.User)
                .FirstOrDefault(n => n.Id == id);
        }

        public List<Notification> GetListAll()
        {
            return _context.Notifications
                .Include(n => n.Restaurant)
                .Include(n => n.User)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
        }

        public void Insert(Notification t)
        {
            _context.Notifications.Add(t);
            _context.SaveChanges();
        }

        public void Update(Notification t)
        {
            _context.Notifications.Update(t);
            _context.SaveChanges();
        }

        public List<Notification> GetListByRestaurant(int? restaurantId, bool? unreadOnly, int limit = 50)
        {
            IQueryable<Notification> query = _context.Notifications
                .AsNoTracking()
                .Include(n => n.Restaurant)
                .Include(n => n.User);

            if (restaurantId.HasValue)
            {
                query = query.Where(n => n.RestaurantId == restaurantId.Value);
            }

            if (unreadOnly.HasValue && unreadOnly.Value)
            {
                query = query.Where(n => !n.IsRead);
            }

            return query
                .OrderByDescending(n => n.CreatedAt)
                .Take(limit)
                .ToList();
        }

        public int GetUnreadCount(int? restaurantId)
        {
            IQueryable<Notification> query = _context.Notifications
                .AsNoTracking()
                .Where(n => !n.IsRead);

            if (restaurantId.HasValue)
            {
                query = query.Where(n => n.RestaurantId == restaurantId.Value);
            }

            return query.Count();
        }

        public void MarkAsRead(int id)
        {
            var notification = _context.Notifications.Find(id);
            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                _context.SaveChanges();
            }
        }

        public void MarkAllAsRead(int? restaurantId)
        {
            IQueryable<Notification> query = _context.Notifications.Where(n => !n.IsRead);

            if (restaurantId.HasValue)
            {
                query = query.Where(n => n.RestaurantId == restaurantId.Value);
            }

            var unreadNotifications = query.ToList();
            foreach (var item in unreadNotifications)
            {
                item.IsRead = true;
            }

            _context.SaveChanges();
        }
    }
}
