using BusinessLayer.Abstract;
using BusinessLayer.Models;
using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;

namespace BusinessLayer.Concrete
{
    public class AuditLogManager : IAuditLogService
    {
        private readonly IAuditLogDal _auditLogDal;

        public AuditLogManager(IAuditLogDal auditLogDal)
        {
            _auditLogDal = auditLogDal;
        }

        public void TDelete(AuditLog t)
        {
            _auditLogDal.Delete(t);
        }

        public AuditLog TGetByID(int id)
        {
            return _auditLogDal.GetByID(id)!;
        }

        public List<AuditLog> TGetListAll()
        {
            return _auditLogDal.GetListAll();
        }

        public void TInsert(AuditLog t)
        {
            _auditLogDal.Insert(t);
        }

        public void TUpdate(AuditLog t)
        {
            _auditLogDal.Update(t);
        }

        public (List<AuditLog> Logs, int TotalCount) GetFilteredLogs(AuditLogFilterDto filter)
        {
            if (filter == null) filter = new AuditLogFilterDto();

            return _auditLogDal.GetPagedLogs(
                filter.DateFrom,
                filter.DateTo,
                filter.RestaurantId,
                filter.UserId,
                filter.AdminId,
                filter.Action,
                filter.EntityType,
                filter.Keyword,
                filter.Page,
                filter.PageSize
            );
        }

        public void Log(
            string action,
            string? entityType = null,
            int? entityId = null,
            string? description = null,
            int? restaurantId = null,
            int? userId = null,
            int? adminId = null,
            string? username = null,
            string? ipAddress = null,
            string? userAgent = null,
            string? requestPath = null,
            string? oldValues = null,
            string? newValues = null)
        {
            var log = new AuditLog
            {
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                Description = description,
                RestaurantId = restaurantId,
                UserId = userId,
                AdminId = adminId,
                Username = username,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                RequestPath = requestPath,
                OldValues = oldValues,
                NewValues = newValues,
                CreatedAt = DateTime.UtcNow
            };

            _auditLogDal.Insert(log);
        }

        public int GetFailedLoginCount(string username, string? ipAddress, TimeSpan duration)
        {
            return _auditLogDal.GetFailedLoginCount(username, ipAddress, duration);
        }
    }
}
