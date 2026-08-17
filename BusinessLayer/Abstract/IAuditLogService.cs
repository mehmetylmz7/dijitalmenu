using BusinessLayer.Models;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;

namespace BusinessLayer.Abstract
{
    public interface IAuditLogService : IGenericService<AuditLog>
    {
        (List<AuditLog> Logs, int TotalCount) GetFilteredLogs(AuditLogFilterDto filter);

        void Log(
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
            string? newValues = null);

        int GetFailedLoginCount(string username, string? ipAddress, TimeSpan duration);
    }
}
