using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityLayer.Concrete
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

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

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties (optional/nullable)
        [ForeignKey("RestaurantId")]
        public virtual Restaurant? Restaurant { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("AdminId")]
        public virtual Admin? Admin { get; set; }
    }
}
