using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int ThemeId { get; set; }
        public Theme Theme { get; set; }

        public Menu Menu { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
