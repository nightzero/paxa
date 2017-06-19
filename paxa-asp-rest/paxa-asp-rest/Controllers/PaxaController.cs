using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using paxa.Models;
using System.Configuration;

namespace paxa.Controllers
{
    [Route("/[controller]")]
    public class PaxaController : Controller
    {
        private PaxaDBController dao = new PaxaDBController(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

        // GET paxa/allResources
        [HttpGet("allResources")]
        public IEnumerable<Resource> GetAllResources()
        {
            return dao.ReadAllResources();
        }

        [HttpGet("bookingsAtDate/{date}")]
        public IEnumerable<Booking> BookingsAtDate(DateTime date)
        {
            return dao.ReadBookings(date);
        }
    }
}
