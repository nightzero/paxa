using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using paxa.Models;
using System.Configuration;
using Microsoft.AspNetCore.Cors;

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
        public void createBooking([FromBody] Booking booking)
        {
        }
    }
}
