using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace paxa.Models
{
    public class Booking
    {
        public Booking() { }

        public Booking(long id, Resource resource, string userName, String email, DateTime startTime, DateTime endTime)
        {
            this.Id = id;
            this.Resource = resource;
            this.UserName = userName;
            this.Email = email;
            this.StartTime = startTime;
            this.EndTime = endTime;
        }

        public long Id { get; set; }
        public Resource Resource { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
