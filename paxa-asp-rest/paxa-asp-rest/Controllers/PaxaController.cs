using System;
using System.Collections.Generic;
using System.Linq;
using paxa.Models;
using System.Configuration;
using Newtonsoft.Json;
using paxa.Utilities;
using System.Web.Http;
using System.Web.Http.Cors;

namespace paxa.Controllers
{
    [RoutePrefix("paxa")]
    [EnableCors(origins: "*", headers: "X-Requested-With, Content-Type, X-Codingpedia, Access-Control-Allow-Headers, Authorization", methods: "GET, POST, DELETE, PUT")]
    public class PaxaController : ApiController
    {
        private PaxaDBController dao = new PaxaDBController(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

        [HttpGet]
        [Route("checkStatus")]
        public string status()
        {
            return "OK!";
        }

        // GET paxa/allResources
        [HttpGet]
        [Route("allResources")]
        public IEnumerable<Resource> GetAllResources()
        {
            return dao.ReadAllResources();
        }

        // GET paxa/bookingsAtDate/{date}
        [HttpGet]
        [Route("bookingsAtDate")]
        public IEnumerable<Booking> BookingsAtDate(DateTime date)
        {
            return dao.ReadBookings(date);
        }

        // POST paxa/createNewBooking
        [HttpPost]
        [Route("createNewBooking")]
        [AuthenticationFilter]
        public string createBooking([FromBody] object request)
        {
            var booking = JsonConvert.DeserializeObject<Booking>(request.ToString());
            object profileId = null;
            object profileName = null;
            object profileEmail = null;

            this.Request.Properties.TryGetValue("ProfileId", out profileId);
            this.Request.Properties.TryGetValue("ProfileName", out profileName);
            this.Request.Properties.TryGetValue("ProfileEmail", out profileEmail);

            booking.UserName = (string)profileName;
            booking.Email = (string)profileEmail;

            dao.CreateBooking(booking, (string)profileId);

            // This return is needed to be backwards compatible with the old java backend.
            // The ajax-call expect a valid json, due to dataType: "json"
            return null;
        }

        [HttpDelete]
        [Route("deleteBooking")]
        [AuthenticationFilter]
        public string deleteBooking([FromBody] long bookingId)
        {
            object profileId = null;
            this.Request.Properties.TryGetValue("ProfileId", out profileId);

            dao.DeleteBooking(bookingId, (string)profileId);

            // This return is needed to be backwards compatible with the old java backend.
            // The ajax-call expect a valid json, due to dataType: "json"
            return null;
        }
    }
}
