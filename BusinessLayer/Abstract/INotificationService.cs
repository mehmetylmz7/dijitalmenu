using EntityLayer.Concrete;
using System.Collections.Generic;

namespace BusinessLayer.Abstract
{
    public interface INotificationService : IGenericService<Notification>
    {
        void CreateNotification(
            string title,
            string message,
            string type = "Info",
            int? restaurantId = null,
            int? userId = null);

        List<Notification> GetNotifications(int? restaurantId = null, bool? unreadOnly = null, int limit = 50);

        int GetUnreadCount(int? restaurantId = null);

        void MarkAsRead(int notificationId);

        void MarkAllAsRead(int? restaurantId = null);
    }
}
