using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int MenuId { get; set; }
        public Menu Menu { get; set; }

        public ICollection<MenuItem> MenuItems { get; set; }
    }
}
