using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using paxa.Models;
using System.Configuration;
using Newtonsoft.Json;

namespace paxa.Controllers
{
    [Route("/[controller]")]
    public class PaxaController : Controller
    {
        private PaxaDBController dao = new PaxaDBController(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

        [HttpGet("checkStatus")]
        public string status()
        {
            return "OK!";
        }

        // GET paxa/allResources
        [HttpGet("allResources")]
        public IEnumerable<Resource> GetAllResources()
        {
            return dao.ReadAllResources();
        }

        // GET paxa/bookingsAtDate/{date}
        [HttpGet("bookingsAtDate")]
        public IEnumerable<Booking> BookingsAtDate(DateTime date)
        {
            return dao.ReadBookings(date);
        }

        // POST paxa/createNewBooking
        [HttpPost("createNewBooking")]
        [Produces("application/json")]
        public string createBooking([FromBody] object request)
        {
            var booking = JsonConvert.DeserializeObject<Booking>(request.ToString());
            //TODO: Fix profileid when we have authentication in place.
            dao.CreateBooking(booking, "112233");

            // This return is needed to be backwards compatible with the old java backend.
            // The ajax-call expect a valid json, due to dataType: "json"
            return null;
        }
    }
}
