using BusinessLayer.Models;
using DataAccessLayer.Repositories;
using EntityLayer.Concrete;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DijitalMenu.Tests;

public class MongoAuditLogTests
{
    private static (IMongoDatabase Db, bool Available) GetTestMongoDb()
    {
        try
        {
            var client = new MongoClient("mongodb://admin:ChangeMe_12345!@localhost:27017/?authSource=admin&serverSelectionTimeoutMS=2000");
            var db = client.GetDatabase("DijitalMenuAudit_Test_" + Guid.NewGuid().ToString("N")[..8]);
            // Ping to verify connection
            db.RunCommand<BsonDocument>(new BsonDocument("ping", 1));
            return (db, true);
        }
        catch
        {
            return (null!, false);
        }
    }

    [Fact]
    public void AuditLog_Model_Generates_Valid_Bson_ObjectId()
    {
        var log = new AuditLog
        {
            Action = "LOGIN_SUCCESS",
            Username = "admin",
            CreatedAt = DateTime.UtcNow
        };

        Assert.NotNull(log.Id);
        Assert.True(ObjectId.TryParse(log.Id, out _), "AuditLog.Id must be a valid Bson ObjectId string.");
    }

    [Fact]
    public void MongoAuditLogRepository_Works_When_Mongo_Available()
    {
        var (db, available) = GetTestMongoDb();
        if (!available)
        {
            // If local Mongo container is not yet ready during test execution, skip live assert
            return;
        }

        var repo = new MongoAuditLogRepository(db);

        // 1. Insert
        var log1 = new AuditLog
        {
            Action = "LOGIN_SUCCESS",
            Username = "mongo_user",
            RestaurantId = 1,
            Description = "User logged in successfully",
            IpAddress = "127.0.0.1",
            CreatedAt = DateTime.UtcNow
        };
        repo.Insert(log1);

        Assert.NotNull(log1.Id);

        // 2. GetByID
        var fetched = repo.GetByID(log1.Id);
        Assert.NotNull(fetched);
        Assert.Equal("LOGIN_SUCCESS", fetched.Action);
        Assert.Equal("mongo_user", fetched.Username);

        // 3. Failed Login Count
        repo.Insert(new AuditLog
        {
            Action = "LOGIN_FAILED",
            Username = "attacker_user",
            IpAddress = "192.168.1.100",
            CreatedAt = DateTime.UtcNow
        });
        repo.Insert(new AuditLog
        {
            Action = "LOGIN_FAILED",
            Username = "attacker_user",
            IpAddress = "192.168.1.100",
            CreatedAt = DateTime.UtcNow
        });

        int failedCount = repo.GetFailedLoginCount("attacker_user", "192.168.1.100", TimeSpan.FromMinutes(15));
        Assert.Equal(2, failedCount);

        // 4. Filter and Pagination
        var (items, total) = repo.GetPagedLogs(
            dateFrom: null,
            dateTo: null,
            restaurantId: 1,
            userId: null,
            adminId: null,
            action: "LOGIN_SUCCESS",
            entityType: null,
            keyword: "mongo_user",
            page: 1,
            pageSize: 10
        );

        Assert.Equal(1, total);
        Assert.Single(items);
        Assert.Equal(log1.Id, items.First().Id);

        // Clean up test DB
        try
        {
            db.Client.DropDatabase(db.DatabaseNamespace.DatabaseName);
        }
        catch
        {
        }
    }
}
