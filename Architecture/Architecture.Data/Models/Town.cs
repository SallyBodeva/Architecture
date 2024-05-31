using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Data.Models
{
    public  class Town
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
    }
}
