using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DataAccessLayer.Repositories
{
    public class MongoAuditLogRepository : IAuditLogDal
    {
        private readonly IMongoCollection<AuditLog> _collection;
        private readonly Context? _context;
        private static bool _indexesCreated = false;
        private static readonly object _indexLock = new();

        public MongoAuditLogRepository(IMongoDatabase database, Context? context = null)
        {
            _collection = database.GetCollection<AuditLog>("AuditLogs");
            _context = context;

            EnsureIndexes();
        }

        private void EnsureIndexes()
        {
            if (_indexesCreated) return;

            lock (_indexLock)
            {
                if (_indexesCreated) return;

                try
                {
                    var indexKeys = Builders<AuditLog>.IndexKeys;
                    var indexModels = new List<CreateIndexModel<AuditLog>>
                    {
                        new CreateIndexModel<AuditLog>(indexKeys.Descending(x => x.CreatedAt), new CreateIndexOptions { Background = true, Name = "ix_audit_created_at" }),
                        new CreateIndexModel<AuditLog>(indexKeys.Ascending(x => x.RestaurantId).Descending(x => x.CreatedAt), new CreateIndexOptions { Background = true, Name = "ix_audit_restaurant_created" }),
                        new CreateIndexModel<AuditLog>(indexKeys.Ascending(x => x.Action), new CreateIndexOptions { Background = true, Name = "ix_audit_action" }),
                        new CreateIndexModel<AuditLog>(indexKeys.Ascending(x => x.Action).Descending(x => x.CreatedAt).Ascending(x => x.Username).Ascending(x => x.IpAddress), new CreateIndexOptions { Background = true, Name = "ix_audit_security_login" }),
                        new CreateIndexModel<AuditLog>(indexKeys.Ascending(x => x.UserId), new CreateIndexOptions { Background = true, Name = "ix_audit_user_id" }),
                        new CreateIndexModel<AuditLog>(indexKeys.Ascending(x => x.AdminId), new CreateIndexOptions { Background = true, Name = "ix_audit_admin_id" })
                    };

                    _collection.Indexes.CreateMany(indexModels);
                    _indexesCreated = true;
                }
                catch
                {
                    // Ignore index creation failure on initial run or restricted permissions
                }
            }
        }

        public void Insert(AuditLog t)
        {
            if (string.IsNullOrEmpty(t.Id))
            {
                t.Id = ObjectId.GenerateNewId().ToString();
            }

            if (t.CreatedAt == default)
            {
                t.CreatedAt = DateTime.UtcNow;
            }
            else
            {
                t.CreatedAt = DateTime.SpecifyKind(t.CreatedAt, DateTimeKind.Utc);
            }

            // Denormalize RestaurantName for resilience if available
            if (_context != null && t.RestaurantId.HasValue && string.IsNullOrEmpty(t.RestaurantName))
            {
                try
                {
                    var rest = _context.Restaurants.AsNoTracking().FirstOrDefault(r => r.Id == t.RestaurantId.Value);
                    if (rest != null)
                    {
                        t.RestaurantName = rest.Name;
                    }
                }
                catch
                {
                    // Ignore error in lookup
                }
            }

            _collection.InsertOne(t);
        }

        public void Delete(AuditLog t)
        {
            _collection.DeleteOne(x => x.Id == t.Id);
        }

        public void Update(AuditLog t)
        {
            _collection.ReplaceOne(x => x.Id == t.Id, t);
        }

        public List<AuditLog> GetListAll()
        {
            var logs = _collection.Find(FilterDefinition<AuditLog>.Empty)
                .SortByDescending(x => x.CreatedAt)
                .ToList();

            PopulateNavigations(logs);
            return logs;
        }

        public AuditLog? GetByID(string id)
        {
            var item = _collection.Find(x => x.Id == id).FirstOrDefault();
            if (item != null)
            {
                PopulateNavigations(new List<AuditLog> { item });
            }
            return item;
        }

        public AuditLog? GetByID(int id)
        {
            return GetByID(id.ToString());
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
            var builder = Builders<AuditLog>.Filter;
            var filter = builder.Empty;

            if (dateFrom.HasValue)
            {
                var utcFrom = DateTime.SpecifyKind(dateFrom.Value, DateTimeKind.Utc);
                filter &= builder.Gte(x => x.CreatedAt, utcFrom);
            }

            if (dateTo.HasValue)
            {
                var utcTo = DateTime.SpecifyKind(dateTo.Value, DateTimeKind.Utc);
                filter &= builder.Lte(x => x.CreatedAt, utcTo);
            }

            if (restaurantId.HasValue)
            {
                filter &= builder.Eq(x => x.RestaurantId, restaurantId.Value);
            }

            if (userId.HasValue)
            {
                filter &= builder.Eq(x => x.UserId, userId.Value);
            }

            if (adminId.HasValue)
            {
                filter &= builder.Eq(x => x.AdminId, adminId.Value);
            }

            if (!string.IsNullOrWhiteSpace(action))
            {
                filter &= builder.Eq(x => x.Action, action.Trim());
            }

            if (!string.IsNullOrWhiteSpace(entityType))
            {
                filter &= builder.Eq(x => x.EntityType, entityType.Trim());
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var pattern = new BsonRegularExpression(Regex.Escape(keyword.Trim()), "i");
                var keywordFilter = builder.Or(
                    builder.Regex(x => x.Description, pattern),
                    builder.Regex(x => x.Username, pattern),
                    builder.Regex(x => x.IpAddress, pattern),
                    builder.Regex(x => x.Action, pattern),
                    builder.Regex(x => x.EntityType, pattern),
                    builder.Regex(x => x.RestaurantName, pattern)
                );
                filter &= keywordFilter;
            }

            int totalCount = (int)_collection.CountDocuments(filter);

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            var items = _collection.Find(filter)
                .SortByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToList();

            PopulateNavigations(items);

            return (items, totalCount);
        }

        public int GetFailedLoginCount(string username, string? ipAddress, TimeSpan duration)
        {
            var cutoff = DateTime.UtcNow.Subtract(duration);
            var builder = Builders<AuditLog>.Filter;
            var filter = builder.Eq(x => x.Action, "LOGIN_FAILED") & builder.Gte(x => x.CreatedAt, cutoff);

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(ipAddress))
            {
                filter &= builder.Or(builder.Eq(x => x.Username, username), builder.Eq(x => x.IpAddress, ipAddress));
            }
            else if (!string.IsNullOrEmpty(username))
            {
                filter &= builder.Eq(x => x.Username, username);
            }
            else if (!string.IsNullOrEmpty(ipAddress))
            {
                filter &= builder.Eq(x => x.IpAddress, ipAddress);
            }

            return (int)_collection.CountDocuments(filter);
        }

        private void PopulateNavigations(List<AuditLog> logs)
        {
            if (logs == null || !logs.Any() || _context == null) return;

            try
            {
                var restIds = logs.Where(l => l.RestaurantId.HasValue).Select(l => l.RestaurantId!.Value).Distinct().ToList();
                var userIds = logs.Where(l => l.UserId.HasValue).Select(l => l.UserId!.Value).Distinct().ToList();
                var adminIds = logs.Where(l => l.AdminId.HasValue).Select(l => l.AdminId!.Value).Distinct().ToList();

                var restaurants = restIds.Any()
                    ? _context.Restaurants.AsNoTracking().Where(r => restIds.Contains(r.Id)).ToDictionary(r => r.Id)
                    : new Dictionary<int, Restaurant>();

                var users = userIds.Any()
                    ? _context.Users.AsNoTracking().Where(u => userIds.Contains(u.Id)).ToDictionary(u => u.Id)
                    : new Dictionary<int, User>();

                var admins = adminIds.Any()
                    ? _context.Admins.AsNoTracking().Where(a => adminIds.Contains(a.Id)).ToDictionary(a => a.Id)
                    : new Dictionary<int, Admin>();

                foreach (var log in logs)
                {
                    if (log.RestaurantId.HasValue && restaurants.TryGetValue(log.RestaurantId.Value, out var rest))
                    {
                        log.Restaurant = rest;
                    }
                    if (log.UserId.HasValue && users.TryGetValue(log.UserId.Value, out var user))
                    {
                        log.User = user;
                    }
                    if (log.AdminId.HasValue && admins.TryGetValue(log.AdminId.Value, out var admin))
                    {
                        log.Admin = admin;
                    }
                }
            }
            catch
            {
                // Gracefully continue without navigation props if EF context is not connected
            }
        }
    }
}
