using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using paxa.Controllers;
using paxa.Models;
using MySql.Data.MySqlClient;
using System.Web.Http;
using System.Collections.Generic;

namespace paxa.Tests
{
    [TestClass]
    public class TestPaxaDBController
    {
        [TestMethod]
        public void TestCreateBooking()
        {
            PaxaDBController dao = new PaxaDBController("Server=localhost;User Id=paxa;Password=paxa;Database=paxa");
            Tuple<long, long> resp = null;
            try
            {
                Resource r = new Resource();
                r.Id = 5;
                Booking b = new Booking();
                b.Resource = r;
                b.StartTime = new DateTime(2099, 1, 1, 12, 0, 0);
                b.EndTime = new DateTime(2099, 1, 1, 19, 0, 0);
                b.UserName = "kalle";
                b.Email = "kalle@sverige.se";

                resp = dao.CreateBooking(b, "112233");
                List<Booking> bookings = dao.ReadBookings(b.StartTime);
                Assert.AreEqual(1, bookings.Count);
            }
            finally
            {
                if(resp != null)
                {
                    dao.deleteBooking(resp.Item1, "112233");
                    dao.deleteUser(resp.Item2);
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void TestCreateBookingIfAlreadyExist()
        {
            PaxaDBController dao = new PaxaDBController("Server=localhost;User Id=paxa;Password=paxa;Database=paxa");
            Tuple<long, long> resp = null;
            try
            {
                Resource r = new Resource();
                r.Id = 5;
                Booking b = new Booking();
                b.Resource = r;
                b.StartTime = new DateTime(2099, 1, 1, 12, 0, 0);
                b.EndTime = new DateTime(2099, 1, 1, 19, 0, 0);
                b.UserName = "kalle";
                b.Email = "kalle@sverige.se";

                resp = dao.CreateBooking(b, "112233");
                resp = dao.CreateBooking(b, "112233");
            }
            finally
            {
                if (resp != null)
                {
                    dao.deleteBooking(resp.Item1, "112233");
                    dao.deleteUser(resp.Item2);
                }
            }
        }

        [TestMethod]
        public void TestDeleteBooking()
        {
            PaxaDBController dao = new PaxaDBController("Server=localhost;User Id=paxa;Password=paxa;Database=paxa");
            Tuple<long, long> resp = null;
            try
            {
                Resource r = new Resource();
                r.Id = 5;
                Booking b = new Booking();
                b.Resource = r;
                b.StartTime = new DateTime(2099, 1, 1, 12, 0, 0);
                b.EndTime = new DateTime(2099, 1, 1, 19, 0, 0);
                b.UserName = "kalle";
                b.Email = "kalle@sverige.se";

                resp = dao.CreateBooking(b, "112233");
                List<Booking> bookings = dao.ReadBookings(b.StartTime);
                Assert.AreEqual(1, bookings.Count);
                dao.deleteBooking(resp.Item1, "112233");
                bookings = dao.ReadBookings(b.StartTime);
                Assert.AreEqual(0, bookings.Count);
            }
            finally
            {
                if (resp != null)
                {
                    dao.deleteBooking(resp.Item1, "112233");
                    dao.deleteUser(resp.Item2);
                }
            }
        }

        [TestMethod]
        public void TestCreateUser()
        {
            PaxaDBController dao = new PaxaDBController("Server=localhost;User Id=paxa;Password=paxa;Database=paxa");
            User user = null;
            try
            {
                dao.createUser("666", "Beast", "beast@hell.com");
                user = dao.getUser("666");
                Assert.IsNotNull(user);
            }
            finally
            {
                if(user != null)
                {
                    dao.deleteUser(user.Id);
                }
            }
        }

        [TestMethod]
        public void TestGetUser()
        {
            PaxaDBController dao = new PaxaDBController("Server=localhost;User Id=paxa;Password=paxa;Database=paxa");
            User user = null;
            try
            {
                dao.createUser("666", "Beast", "beast@hell.com");
                user = dao.getUser("666");
                Assert.AreEqual(user.Name, "Beast");
            }
            finally
            {
                if (user != null)
                {
                    dao.deleteUser(user.Id);
                }
            }
        }
    }
}
