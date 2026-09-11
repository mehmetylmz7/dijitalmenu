using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EntityLayer.Concrete
{
    public class AuditLog
    {
        [Key]
        [StringLength(50)]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public int? RestaurantId { get; set; }

        public int? UserId { get; set; }

        public int? AdminId { get; set; }

        [Required, StringLength(100)]
        public string Action { get; set; } = string.Empty;

        [StringLength(100)]
        public string? EntityType { get; set; }

        public int? EntityId { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? Username { get; set; }

        [StringLength(100)]
        public string? IpAddress { get; set; }

        [StringLength(500)]
        public string? UserAgent { get; set; }

        [StringLength(500)]
        public string? RequestPath { get; set; }

        [Column(TypeName = "jsonb")]
        public string? OldValues { get; set; }

        [Column(TypeName = "jsonb")]
        public string? NewValues { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Denormalized name for fast and decoupled querying
        [StringLength(150)]
        public string? RestaurantName { get; set; }

        // Navigation properties (optional/nullable - ignored by MongoDB serializer)
        [BsonIgnore]
        [ForeignKey("RestaurantId")]
        public virtual Restaurant? Restaurant { get; set; }

        [BsonIgnore]
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [BsonIgnore]
        [ForeignKey("AdminId")]
        public virtual Admin? Admin { get; set; }
    }
}

