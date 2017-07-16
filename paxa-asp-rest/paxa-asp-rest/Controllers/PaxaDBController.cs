using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using paxa.Models;
using Microsoft.Extensions.Logging;
using System.Numerics;

namespace paxa.Controllers
{
    public class PaxaDBController : Controller
    {
        private readonly ILogger logger;

        private string allResourcesQuery = "SELECT id, name FROM resources";
        private String bookingsAtDateQuery = "SELECT b.*, r.name AS resource_name, u.name AS user_name, u.email FROM bookings b JOIN resources r on r.id = b.resource_id JOIN users u on u.id = b.user_id WHERE DATE(startTime) <= DATE(@sTime) AND DATE(endTime) >= DATE(@eTime)";
        private String bookingsExistQuery = "SELECT b.* FROM bookings b JOIN resources r on r.id = b.resource_id WHERE b.resource_id = @rId AND (startTime < @sTime) AND (endTime > @eTime)";
        private String createNewBookingQuery = "INSERT INTO bookings (resource_id, user_id, startTime, endTime) VALUES (@rId, @uId, @sTime, @eTime)";
        private String deleteBookingQuery = "DELETE FROM bookings WHERE id = @bId";
        private String lookUpUser = "SELECT id, profileid, name, email FROM users WHERE profileid = @pId";
        private String createNewUser = "INSERT INTO users (profileid, name, email) VALUES (@pId, @name, @email)";

        public string ConnectionString { get; set; }

        public PaxaDBController(string connectionString)
        {
            this.ConnectionString = connectionString;
            logger = paxa.Utilities.ApplicationLogging.CreateLogger();
        }

        private MySqlConnection GetConnection()
        {
            MySqlConnection con = new MySqlConnection(ConnectionString);
            con.Open();
            return con;
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
                logger.LogError("Error occured in interaction towards DB: " + e.ToString());
            }
            finally
            {
                if (sdr != null) { sdr.Close(); sdr.Dispose(); }
                if (sqlCmd != null) { sqlCmd.Dispose(); }
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
                logger.LogError("Error occured in interaction towards DB: " + e.ToString());
            }
            finally
            {
                if (sdr != null) { sdr.Close(); sdr.Dispose(); }
                if (sqlCmd != null) { sqlCmd.Dispose(); }
            }

            return bookings;
        }

        public bool CheckIfBookingExist(int resourceId, DateTime startTime, DateTime endTime)
        {
            MySqlConnection con = GetConnection();
            try
            {
                return CheckIfBookingExist(resourceId, startTime, endTime, con);
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

        private bool CheckIfBookingExist(int resourceId, DateTime startTime, DateTime endTime, MySqlConnection con)
        {
            List<Booking> bookings = new List<Booking>();
            MySqlCommand sqlCmd = null;
            MySqlDataReader sdr = null;
            try
            {
                sqlCmd = new MySqlCommand(bookingsExistQuery, con);
                sqlCmd.Prepare();
                sqlCmd.Parameters.AddWithValue("@rId", resourceId);
                sqlCmd.Parameters.AddWithValue("@sTime", endTime);
                sqlCmd.Parameters.AddWithValue("@eTime", startTime);
                sdr = sqlCmd.ExecuteReader();
                if (sdr.HasRows)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                logger.LogError("Error occured in interaction towards DB: " + e.ToString());
            }
            finally
            {
                if (sdr != null) { sdr.Close(); sdr.Dispose(); }
                if (sqlCmd != null) { sqlCmd.Dispose(); }
            }

            return false;
        }

        public void CreateBooking(Booking booking, String profileId)
        {
            MySqlConnection con = GetConnection();
            try
            {
                if (CheckIfBookingExist(booking.Resource.Id, booking.StartTime, booking.EndTime, con))
                {
                    //Booking for the resource already exist in the specified time range. Raise error!
                    throw new ApplicationException("Resursen är redan bokad i angivet tidsintervall!");
                }
                //Is the user already in DB, else create it first and get the generated ID.
                User user = getUser(con, profileId);
                if(user != null)
                {
                    createUser(con, profileId, booking.UserName, booking.Email);
                    user = getUser(con, profileId);
                }
                CreateBooking(booking, user.Id, con);
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

        private void CreateBooking(Booking booking, BigInteger userId, MySqlConnection con)
        {
            List<Booking> bookings = new List<Booking>();
            MySqlCommand sqlCmd = null;
            try
            {
                sqlCmd = new MySqlCommand(createNewBookingQuery, con);
                sqlCmd.Prepare();
                sqlCmd.Parameters.AddWithValue("@rId", booking.Resource.Id);
                sqlCmd.Parameters.AddWithValue("@uId", userId);
                sqlCmd.Parameters.AddWithValue("@sTime", booking.StartTime);
                sqlCmd.Parameters.AddWithValue("@eTime", booking.EndTime);
                sqlCmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                logger.LogError("Error occured in interaction towards DB: " + e.ToString());
            }
            finally
            {
                if (sqlCmd != null) { sqlCmd.Dispose(); }
            }
        }

        User getUser(MySqlConnection con, String profileID)
        {
            MySqlCommand sqlCmd = null;
            MySqlDataReader sdr = null;
            User resp = null;

            try
            {
                sqlCmd = new MySqlCommand(lookUpUser, con);
                sqlCmd.Prepare();
                sqlCmd.Parameters.AddWithValue("@pId", profileID);
                sdr = sqlCmd.ExecuteReader();

                while (sdr.Read())
                {
                    int resId = Convert.ToInt32(sdr["resource_id"]);
                    String resName = sdr["resource_name"].ToString();

                    Int64 id = Convert.ToInt64(sdr["id"]);
                    String profileId = sdr["profileid"].ToString();
                    String name = sdr["name"].ToString();
                    String email = sdr["email"].ToString();
                    resp = new User(id, profileId, name, email);
                }
            }
            catch (Exception e)
            {
                logger.LogError("Error occured in interaction towards DB: " + e.ToString());
            }
            finally
            {
                if (sqlCmd != null) { sqlCmd.Dispose(); }
                if (sqlCmd != null) { sqlCmd.Dispose(); }
            }
            return resp;
        }

        void createUser(MySqlConnection con, String profileId, String name, String email)
        {
            MySqlCommand sqlCmd = null;
            try
            {
                sqlCmd = new MySqlCommand(createNewUser, con);
                sqlCmd.Prepare();
                sqlCmd.Parameters.AddWithValue("@pId", profileId);
                sqlCmd.Parameters.AddWithValue("@name", name);
                sqlCmd.Parameters.AddWithValue("@email", email);
                sqlCmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                logger.LogError("Error occured in interaction towards DB: " + e.ToString());
            }
            finally
            {
                if (sqlCmd != null) { sqlCmd.Dispose(); }
            }
        }

        public void DeleteBooking(long bookingId, String profileId)
        {
            MySqlConnection con = GetConnection();

            // TODO: Implement when authentication is in place.
            //if (!CheckIfOwningBooking(bookingId, profileId, con))
            //{
            //    //User is trying to delete another users booking
            //    throw new ApplicationException("Du kan bara ta bort dina egna bokningar!");
            //}

            try
            {
                DeleteBooking(bookingId, con);
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

        private void DeleteBooking(long bookingId, MySqlConnection con)
        {
            MySqlCommand sqlCmd = null;
            try
            {
                sqlCmd = new MySqlCommand(deleteBookingQuery, con);
                sqlCmd.Prepare();
                sqlCmd.Parameters.AddWithValue("@bId", bookingId);
                sqlCmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                logger.LogError("Error occured in interaction towards DB: " + e.ToString());
            }
            finally
            {
                if (sqlCmd != null) { sqlCmd.Dispose(); }
            }
        }
    }
}