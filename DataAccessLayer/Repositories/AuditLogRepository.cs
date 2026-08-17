using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer.Repositories
{
    public class AuditLogRepository : IAuditLogDal
    {
        private readonly Context _context;

        public AuditLogRepository(Context context)
        {
            _context = context;
        }

        public void Delete(AuditLog t)
        {
            _context.AuditLogs.Remove(t);
            _context.SaveChanges();
        }

        public AuditLog? GetByID(int id)
        {
            return _context.AuditLogs
                .Include(a => a.Restaurant)
                .Include(a => a.User)
                .Include(a => a.Admin)
                .FirstOrDefault(a => a.Id == id);
        }

        public List<AuditLog> GetListAll()
        {
            return _context.AuditLogs.OrderByDescending(a => a.CreatedAt).ToList();
        }

        public void Insert(AuditLog t)
        {
            _context.AuditLogs.Add(t);
            _context.SaveChanges();
        }

        public void Update(AuditLog t)
        {
            _context.AuditLogs.Update(t);
            _context.SaveChanges();
        }

        public (List<AuditLog> Items, int TotalCount) GetPagedLogs(
            DateTime? dateFrom,
            DateTime? dateTo,
            int? restaurantId,
            int? userId,
            int? adminId,
            string? action,
            string? entityType,
            string? keyword,
            int page,
            int pageSize)
        {
            IQueryable<AuditLog> query = _context.AuditLogs
                .AsNoTracking()
                .Include(a => a.Restaurant)
                .Include(a => a.User)
                .Include(a => a.Admin);

            if (dateFrom.HasValue)
            {
                var utcFrom = DateTime.SpecifyKind(dateFrom.Value, DateTimeKind.Utc);
                query = query.Where(a => a.CreatedAt >= utcFrom);
            }

            if (dateTo.HasValue)
            {
                var utcTo = DateTime.SpecifyKind(dateTo.Value, DateTimeKind.Utc);
                query = query.Where(a => a.CreatedAt <= utcTo);
            }

            if (restaurantId.HasValue)
            {
                query = query.Where(a => a.RestaurantId == restaurantId.Value);
            }

            if (userId.HasValue)
            {
                query = query.Where(a => a.UserId == userId.Value);
            }

            if (adminId.HasValue)
            {
                query = query.Where(a => a.AdminId == adminId.Value);
            }

            if (!string.IsNullOrWhiteSpace(action))
            {
                var normAction = action.Trim();
                query = query.Where(a => a.Action == normAction);
            }

            if (!string.IsNullOrWhiteSpace(entityType))
            {
                var normEntityType = entityType.Trim();
                query = query.Where(a => a.EntityType == normEntityType);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var normKeyword = keyword.Trim().ToLower();
                query = query.Where(a =>
                    (a.Description != null && a.Description.ToLower().Contains(normKeyword)) ||
                    (a.Username != null && a.Username.ToLower().Contains(normKeyword)) ||
                    (a.IpAddress != null && a.IpAddress.ToLower().Contains(normKeyword)) ||
                    (a.Action != null && a.Action.ToLower().Contains(normKeyword)) ||
                    (a.EntityType != null && a.EntityType.ToLower().Contains(normKeyword))
                );
            }

            int totalCount = query.Count();

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            var items = query
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (items, totalCount);
        }

        public int GetFailedLoginCount(string username, string? ipAddress, TimeSpan duration)
        {
            var cutoff = DateTime.UtcNow.Subtract(duration);
            var query = _context.AuditLogs
                .AsNoTracking()
                .Where(a => a.Action == "LOGIN_FAILED" && a.CreatedAt >= cutoff);

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(ipAddress))
            {
                query = query.Where(a => a.Username == username || a.IpAddress == ipAddress);
            }
            else if (!string.IsNullOrEmpty(username))
            {
                query = query.Where(a => a.Username == username);
            }
            else if (!string.IsNullOrEmpty(ipAddress))
            {
                query = query.Where(a => a.IpAddress == ipAddress);
            }

            return query.Count();
        }
    }
}
