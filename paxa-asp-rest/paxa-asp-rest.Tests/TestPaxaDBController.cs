using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using paxa.Controllers;
using paxa.Models;
using MySql.Data.MySqlClient;


namespace paxa.Tests
{
    [TestClass]
    public class TestPaxaDBController
    {
        [TestMethod]
        public void TestCreateBooking()
        {
            PaxaDBController dao = new PaxaDBController("Server=localhost;User Id=paxa;Password=paxa;Database=paxa");
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
            PaxaDBController dao = new PaxaDBController("Server=localhost;User Id=paxa;Password=paxa;Database=paxa");
            dao.DeleteBooking(26, "112233");
        }

        [TestMethod]
        public void TestCreateUser()
        {
            MySqlConnection con = null;
            try
            {
                PaxaDBController dao = new PaxaDBController("Server=localhost;User Id=paxa;Password=paxa;Database=paxa");
                con = new MySqlConnection("Server=localhost;User Id=root;Password=zodiac;Database=paxa");
                con.Open();
                dao.createUser(con, "666", "Beast", "beast@hell.com");
            }
            finally
            {
                if (con != null) { con.Close(); }
            }
        }

        [TestMethod]
        public void testGetUser()
        {
            MySqlConnection con = null;
            try
            {
                PaxaDBController dao = new PaxaDBController("Server=localhost;User Id=paxa;Password=paxa;Database=paxa");
                con = new MySqlConnection("Server=localhost;User Id=root;Password=zodiac;Database=paxa");
                con.Open();
                User u = dao.getUser(con, "666");
                Assert.AreEqual(u.Name, "Beast");
            }
            finally
            {
                if (con != null) { con.Close(); }
            }
        }

    }
}
