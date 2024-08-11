using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Data.Models
{
    public class Address
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string TownId { get; set; }

        public virtual Town Town { get; set; }

        public virtual ICollection<User> Users { get; set; } = new List<User>();

        public virtual Project Project { get; set; }
    }
}
