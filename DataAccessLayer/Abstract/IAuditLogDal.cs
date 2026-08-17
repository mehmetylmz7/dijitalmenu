using EntityLayer.Concrete;
using System;
using System.Collections.Generic;

namespace DataAccessLayer.Abstract
{
    public interface IAuditLogDal : IGenericDal<AuditLog>
    {
        (List<AuditLog> Items, int TotalCount) GetPagedLogs(
            DateTime? dateFrom,
            DateTime? dateTo,
            int? restaurantId,
            int? userId,
            int? adminId,
            string? action,
            string? entityType,
            string? keyword,
            int page,
            int pageSize);

        int GetFailedLoginCount(string username, string? ipAddress, TimeSpan duration);
    }
}
