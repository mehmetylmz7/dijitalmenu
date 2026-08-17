using System;

namespace BusinessLayer.Models
{
    public class AuditLogFilterDto
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int? RestaurantId { get; set; }
        public int? UserId { get; set; }
        public int? AdminId { get; set; }
        public string? Action { get; set; }
        public string? EntityType { get; set; }
        public string? Keyword { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
