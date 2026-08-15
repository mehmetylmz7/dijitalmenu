using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Restaurant
    {
        [Key]

        public int Id { get; set; }
        [Required, StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Slug { get; set; }

        [StringLength(2048)]
        public string? GoogleMapsUrl { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(1000)]
        public string? ImportantNotice { get; set; }

        [StringLength(200)]
        public string? WorkingHours { get; set; }

        [RegularExpression(@"^$|^[0-9+()\-\s]{7,25}$")]
        public string? Phone { get; set; }

        public int ThemeId { get; set; }
        public Theme Theme { get; set; }

        public Menu Menu { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
