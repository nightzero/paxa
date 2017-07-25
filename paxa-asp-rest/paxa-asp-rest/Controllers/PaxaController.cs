using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using paxa.Models;
using System.Configuration;
using Newtonsoft.Json;
using paxa.Utilities;

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
        [AuthenticationFilter]
        public string createBooking([FromBody] object request)
        {
            var booking = JsonConvert.DeserializeObject<Booking>(request.ToString());
            string profileId = ViewBag.ProfileId;
            String profileName = ViewBag.ProfileName;
            String profileEmail = ViewBag.ProfileEmail;

            booking.UserName = profileName;
            booking.Email = profileEmail;

            dao.CreateBooking(booking, profileId);

            // This return is needed to be backwards compatible with the old java backend.
            // The ajax-call expect a valid json, due to dataType: "json"
            return null;
        }

        [HttpDelete("deleteBooking")]
        [Produces("application/json")]
        [AuthenticationFilter]
        public string deleteBooking([FromBody] long bookingId)
        {
            string profileId = ViewBag.ProfileId;
            dao.DeleteBooking(bookingId, profileId);

            // This return is needed to be backwards compatible with the old java backend.
            // The ajax-call expect a valid json, due to dataType: "json"
            return null;
        }
    }
}
