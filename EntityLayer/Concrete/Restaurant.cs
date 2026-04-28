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
        public string Name { get; set; }

        public int ThemeId { get; set; }
        public Theme Theme { get; set; }

        public Menu Menu { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
