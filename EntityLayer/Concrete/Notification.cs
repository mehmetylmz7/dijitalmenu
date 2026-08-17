using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityLayer.Concrete
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        public int? RestaurantId { get; set; }

        public int? UserId { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(1000)]
        public string Message { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Type { get; set; } = "Info"; // "Info", "Warning", "Error", "Security", "Success"

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties (optional/nullable)
        [ForeignKey("RestaurantId")]
        public virtual Restaurant? Restaurant { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
