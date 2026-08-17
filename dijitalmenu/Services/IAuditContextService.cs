using System;

namespace dijitalmenu.Services
{
    public interface IAuditContextService
    {
        string? GetClientIpAddress();
        string? GetUserAgent();
        string? GetRequestPath();
        (int? RestaurantId, int? UserId, int? AdminId, string? Username) GetCurrentUserContext();

        string? SerializeClean(object? entity);

        void Log(
            string action,
            string? entityType = null,
            int? entityId = null,
            string? description = null,
            object? oldEntity = null,
            object? newEntity = null,
            int? restaurantId = null,
            int? userId = null,
            int? adminId = null,
            string? username = null);

        void CheckAndTriggerFailedLoginAlert(string username);
    }
}
