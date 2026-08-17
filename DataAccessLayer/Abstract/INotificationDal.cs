using EntityLayer.Concrete;
using System.Collections.Generic;

namespace DataAccessLayer.Abstract
{
    public interface INotificationDal : IGenericDal<Notification>
    {
        List<Notification> GetListByRestaurant(int? restaurantId, bool? unreadOnly, int limit = 50);
        int GetUnreadCount(int? restaurantId);
        void MarkAsRead(int id);
        void MarkAllAsRead(int? restaurantId);
    }
}
