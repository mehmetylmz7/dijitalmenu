using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Theme
    {
        [Key]
        public int Id { get; set; }
        [Required, StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        public string PrimaryColor { get; set; } = string.Empty;
        public string SecondaryColor { get; set; } = string.Empty;
        public string BackgroundColor { get; set; } = string.Empty;
        public string FontFamily { get; set; } = string.Empty;
        public LayoutType Layout { get; set; }

        public ICollection<Restaurant> Restaurants { get; set; }
    }
}

