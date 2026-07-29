using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Category
    {
        [Key]

        public int Id { get; set; }
        [Required, StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        public int MenuId { get; set; }
        public Menu Menu { get; set; }

        public ICollection<MenuItem> MenuItems { get; set; }
    }
}
