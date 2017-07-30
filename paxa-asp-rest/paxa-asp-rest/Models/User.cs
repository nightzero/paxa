using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace paxa.Models
{ 
    public class User
    {
        public User() { }

        public User(Int64 id, String profileId, String name, String email)
        {
            this.Id = id;
            this.ProfileId = profileId;
            this.Name = name;
            this.Email = email;
        }

        public Int64 Id { get; set; }
        public String ProfileId { get; set; }
        public String Name { get; set; }
        public String Email { get; set; }
    }
}
