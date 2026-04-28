using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Menu
    {
        public int Id { get; set; }

        public int RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; }

        public ICollection<Category> Categories { get; set; }
    }
}
