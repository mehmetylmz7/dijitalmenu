using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace dijitalmenu.Services
{
    public class AuditContextService : IAuditContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditLogService _auditLogService;
        private readonly INotificationService _notificationService;
        private readonly IAdminService _adminService;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = false,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        private static readonly HashSet<string> SensitivePropertyNames = new(StringComparer.OrdinalIgnoreCase)
        {
            "Password",
            "PasswordHash",
            "SecurityStamp",
            "ConcurrencyStamp",
            "Token",
            "SecurityToken",
            "Secret",
            "SecretKey",
            "SessionId"
        };

        public AuditContextService(
            IHttpContextAccessor httpContextAccessor,
            IAuditLogService auditLogService,
            INotificationService notificationService,
            IAdminService adminService)
        {
            _httpContextAccessor = httpContextAccessor;
            _auditLogService = auditLogService;
            _notificationService = notificationService;
            _adminService = adminService;
        }

        public string? GetClientIpAddress()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return null;

            // Check X-Forwarded-For header in case behind reverse proxy
            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
            {
                var ips = forwardedFor.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (ips.Length > 0)
                {
                    return ips[0].Trim();
                }
            }

            return context.Connection.RemoteIpAddress?.ToString();
        }

        public string? GetUserAgent()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return null;

            return context.Request.Headers["User-Agent"].ToString();
        }

        public string? GetRequestPath()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return null;

            return context.Request.Path.Value;
        }

        public (int? RestaurantId, int? UserId, int? AdminId, string? Username) GetCurrentUserContext()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return (null, null, null, null);

            ISession? session = null;
            try
            {
                session = context.Session;
            }
            catch
            {
                session = null;
            }

            if (session == null || !session.IsAvailable)
            {
                return (null, null, null, null);
            }

            // Check Admin session
            var adminUsername = session.GetString("AdminUser");
            if (!string.IsNullOrEmpty(adminUsername))
            {
                var admin = _adminService.TGetListAll().FirstOrDefault(a => a.Username == adminUsername);
                return (null, null, admin?.Id, adminUsername);
            }

            // Check Restaurant session
            var restaurantIdStr = session.GetString("RestaurantId");
            var userIdStr = session.GetString("RestaurantUserId");
            var restUsername = session.GetString("RestaurantUsername");

            int? restaurantId = int.TryParse(restaurantIdStr, out var rId) ? rId : null;
            int? userId = int.TryParse(userIdStr, out var uId) ? uId : null;

            return (restaurantId, userId, null, restUsername);
        }

        public string? SerializeClean(object? entity)
        {
            if (entity == null) return null;

            try
            {
                var dict = CleanEntityToDictionary(entity);
                return JsonSerializer.Serialize(dict, JsonOptions);
            }
            catch
            {
                return null;
            }
        }

        private Dictionary<string, object?> CleanEntityToDictionary(object entity)
        {
            var result = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            var type = entity.GetType();

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanRead) continue;

                // Mask or exclude sensitive fields
                if (SensitivePropertyNames.Contains(prop.Name))
                {
                    continue; // exclude completely
                }

                // Exclude complex navigation collections / EF proxies to keep JSON clean
                var propType = prop.PropertyType;
                if (typeof(System.Collections.IEnumerable).IsAssignableFrom(propType) && propType != typeof(string) && !propType.IsArray)
                {
                    continue;
                }

                if (propType.Namespace != null && propType.Namespace.StartsWith("EntityLayer"))
                {
                    continue;
                }

                try
                {
                    var val = prop.GetValue(entity);
                    result[prop.Name] = val;
                }
                catch
                {
                    // Ignore property evaluation errors
                }
            }

            return result;
        }

        public void Log(
            string action,
            string? entityType = null,
            int? entityId = null,
            string? description = null,
            object? oldEntity = null,
            object? newEntity = null,
            int? restaurantId = null,
            int? userId = null,
            int? adminId = null,
            string? username = null)
        {
            var (ctxRestId, ctxUserId, ctxAdminId, ctxUsername) = GetCurrentUserContext();

            var finalRestId = restaurantId ?? ctxRestId;
            var finalUserId = userId ?? ctxUserId;
            var finalAdminId = adminId ?? ctxAdminId;
            var finalUsername = username ?? ctxUsername;

            var ip = GetClientIpAddress();
            var ua = GetUserAgent();
            var path = GetRequestPath();

            var oldJson = oldEntity != null ? SerializeClean(oldEntity) : null;
            var newJson = newEntity != null ? SerializeClean(newEntity) : null;

            _auditLogService.Log(
                action: action,
                entityType: entityType,
                entityId: entityId,
                description: description,
                restaurantId: finalRestId,
                userId: finalUserId,
                adminId: finalAdminId,
                username: finalUsername,
                ipAddress: ip,
                userAgent: ua,
                requestPath: path,
                oldValues: oldJson,
                newValues: newJson
            );
        }

        public void CheckAndTriggerFailedLoginAlert(string username)
        {
            try
            {
                var ip = GetClientIpAddress();
                int failedCount = _auditLogService.GetFailedLoginCount(username, ip, TimeSpan.FromMinutes(15));
                if (failedCount >= 3)
                {
                    _notificationService.CreateNotification(
                        title: "Şüpheli Giriş Denemesi: Çok Sayıda Başarısız Giriş",
                        message: $"'{username}' kullanıcısı için ({ip ?? "Bilinmeyen IP"}) üzerinden son 15 dakikada {failedCount} kez başarısız giriş denendi.",
                        type: "Danger"
                    );
                }
            }
            catch
            {
                // Silent fail for notification triggering so login flow is not interrupted
            }
        }
    }
}
