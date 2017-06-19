using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using paxa.Models;
using Microsoft.Extensions.Logging;

namespace paxa.Controllers
{
    public class PaxaDBController : Controller
    {
        public PaxaDBController(ILogger<PaxaDBController> logger)
        {
            _logger = logger;
        }

        private readonly ILogger _logger;

        private string allResourcesQuery = "SELECT id, name FROM resources";
        private String bookingsAtDateQuery = "SELECT b.*, r.name AS resource_name, u.name AS user_name, u.email FROM bookings b JOIN resources r on r.id = b.resource_id JOIN users u on u.id = b.user_id WHERE DATE(startTime) <= DATE(@sTime) AND DATE(endTime) >= DATE(@eTime)";

        public string ConnectionString { get; set; }

        public PaxaDBController(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public List<Resource> ReadAllResources()
        {
            MySqlConnection con = GetConnection();
            try
            {
                return ReadAllResources(con);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }

        private List<Resource> ReadAllResources(MySqlConnection con)
        {
            List<Resource> resources = new List<Resource>();
            MySqlCommand sqlCmd = null;
            MySqlDataReader sdr = null;
            try
            {
                sqlCmd = new MySqlCommand(allResourcesQuery, con);
                con.Open();
                sdr = sqlCmd.ExecuteReader();
                while (sdr.Read())
                {
                    resources.Add(new Resource
                    {
                        Id = Convert.ToInt32(sdr["id"]),
                        Name = sdr["name"].ToString()
                    });
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error occured in interaction towards DB: " + e.ToString());
            }
            finally
            {
                if (sdr != null) { sdr.Close(); sdr.Dispose(); }
                if(sqlCmd != null) { sqlCmd.Dispose(); } 
            }

            return resources;
        }

        public List<Booking> ReadBookings(DateTime date)
        {
            MySqlConnection con = GetConnection();
            try
            {
                return ReadBookings(date, con);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }

        private List<Booking> ReadBookings(DateTime date, MySqlConnection con)
        {
            List<Booking> bookings = new List<Booking>();
            MySqlCommand sqlCmd = null;
            MySqlDataReader sdr = null;
            try
            {
                con.Open();
                sqlCmd = new MySqlCommand(bookingsAtDateQuery, con);
                sqlCmd.Prepare();
                sqlCmd.Parameters.AddWithValue("@sTime", date);
                sqlCmd.Parameters.AddWithValue("@eTime", date);
                sdr = sqlCmd.ExecuteReader();
                while (sdr.Read())
                {
                    int resId = Convert.ToInt32(sdr["resource_id"]);
                    String resName = sdr["resource_name"].ToString();
                    Resource res = new Resource(resName, resId);

                    bookings.Add(new Booking
                    {
                        Id = Convert.ToInt32(sdr["id"]),
                        Resource = res,
                        UserName = sdr["user_name"].ToString(),
                        Email = sdr["email"].ToString(),
                        StartTime = Convert.ToDateTime(sdr["startTime"]),
                        EndTime = Convert.ToDateTime(sdr["endTime"]),
                    });
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error occured in interaction towards DB: " + e.ToString());
            }
            finally
            {
                if (sdr != null) { sdr.Close(); sdr.Dispose(); }
                if (sqlCmd != null) { sqlCmd.Dispose(); }
            }

            return bookings;
        }
    }
}