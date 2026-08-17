using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;

namespace BusinessLayer.Concrete
{
    public class NotificationManager : INotificationService
    {
        private readonly INotificationDal _notificationDal;

        public NotificationManager(INotificationDal notificationDal)
        {
            _notificationDal = notificationDal;
        }

        public void TDelete(Notification t)
        {
            _notificationDal.Delete(t);
        }

        public Notification TGetByID(int id)
        {
            return _notificationDal.GetByID(id)!;
        }

        public List<Notification> TGetListAll()
        {
            return _notificationDal.GetListAll();
        }

        public void TInsert(Notification t)
        {
            _notificationDal.Insert(t);
        }

        public void TUpdate(Notification t)
        {
            _notificationDal.Update(t);
        }

        public void CreateNotification(
            string title,
            string message,
            string type = "Info",
            int? restaurantId = null,
            int? userId = null)
        {
            var notification = new Notification
            {
                Title = title,
                Message = message,
                Type = type,
                RestaurantId = restaurantId,
                UserId = userId,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _notificationDal.Insert(notification);
        }

        public List<Notification> GetNotifications(int? restaurantId = null, bool? unreadOnly = null, int limit = 50)
        {
            return _notificationDal.GetListByRestaurant(restaurantId, unreadOnly, limit);
        }

        public int GetUnreadCount(int? restaurantId = null)
        {
            return _notificationDal.GetUnreadCount(restaurantId);
        }

        public void MarkAsRead(int notificationId)
        {
            _notificationDal.MarkAsRead(notificationId);
        }

        public void MarkAllAsRead(int? restaurantId = null)
        {
            _notificationDal.MarkAllAsRead(restaurantId);
        }
    }
}
