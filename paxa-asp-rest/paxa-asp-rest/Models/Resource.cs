using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace paxa.Models
{
    public class Resource
    {
        public Resource() { }

        public Resource(string name, int id)
        {
            this.Name = name;
            this.Id = id;
        }

        public String Name { get; set; }
        public int Id { get; set; }
    }
}
