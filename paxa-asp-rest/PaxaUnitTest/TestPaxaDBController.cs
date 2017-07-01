using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using Moq;
using paxa.Controllers;
using paxa.Models;

namespace paxa.Test
{
    [TestClass]
    public class TestPaxaDBController
    {
        [TestMethod]
        public void TestCreateBooking()
        {
            PaxaDBController dao = new PaxaDBController("Server=localhost;User Id=root;Password=zodiac;Database=paxa");
            Resource r = new Resource();
            r.Id = 5;
            Booking b = new Booking();
            b.Resource = r;
            b.StartTime = new DateTime(2099, 1, 1, 12, 0, 0);
            b.EndTime = new DateTime(2099, 1, 1, 19, 0, 0);
            
            dao.CreateBooking(b, "112233");
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestCreateBookingIfAlreadyExist()
        {
            TestCreateBooking();
        }

        [TestMethod]
        public void TestDeleteBooking()
        {
            PaxaDBController dao = new PaxaDBController("Server=localhost;User Id=root;Password=zodiac;Database=paxa");
            dao.DeleteBooking(26, "112233");
        }
    }
}
