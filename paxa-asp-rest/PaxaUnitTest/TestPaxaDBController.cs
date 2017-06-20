using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using Moq;
using paxa.Controllers;

namespace paxa.Test
{
    [TestClass]
    public class TestPaxaDBController
    {
        [TestMethod]
        public void TestCheckIfBookingExist()
        {
            PaxaDBController dao = new PaxaDBController("Server=localhost;User Id=root;Password=zodiac;Database=paxa");
            Assert.IsTrue(dao.CheckIfBookingExist(7, new DateTime(2017, 5, 4, 12, 0, 0), new DateTime(2017, 5, 4, 19, 0, 0)));
        }
    }
}
