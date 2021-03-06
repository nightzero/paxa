﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using paxa.Models;
using Microsoft.Extensions.Logging;
using System.Numerics;
using System.Net.Http;
using System.Net;
using System.Web.Http;
using paxa_asp_rest.Utilities;
using System.Data;

namespace paxa.Controllers
{
    public class PaxaDBController : ApiController
    {
        private readonly ILogger logger;

        private string allResourcesQuery = "SELECT id, name FROM resources";
        private String bookingsAtDateQuery = "SELECT b.*, r.name AS resource_name, u.name AS user_name, u.email FROM bookings b JOIN resources r on r.id = b.resource_id JOIN users u on u.id = b.user_id WHERE DATE(startTime) <= DATE(@sTime) AND DATE(endTime) >= DATE(@eTime)";
        private String bookingsExistQuery = "SELECT b.* FROM bookings b JOIN resources r on r.id = b.resource_id WHERE b.resource_id = @rId AND (startTime < @sTime) AND (endTime > @eTime)";
        private String bookingIdExistQuery = "SELECT b.* FROM bookings b Where b.id = @bId";
        private String createNewBookingQuery = "INSERT INTO bookings (resource_id, user_id, startTime, endTime) VALUES (@rId, @uId, @sTime, @eTime)";
        private String deleteBookingQuery = "DELETE FROM bookings WHERE id = @bId";
        private String bookingOnProfileIdQuery = "SELECT b.id, u.profileid FROM bookings b JOIN users u on u.id = b.user_id WHERE b.id = @bId AND u.profileid = @pId";
        private String lookUpUser = "SELECT id, profileid, name, email FROM users WHERE profileid = @pId";
        private String createNewUser = "INSERT INTO users (profileid, name, email) VALUES (@pId, @name, @email)";
        private String deleteUserQuery = "DELETE from users WHERE id = @uId";

        private ConnectionManager connectionManager;

        public PaxaDBController(string connectionString)
        {
            this.connectionManager = new ConnectionManager(connectionString);
            logger = paxa.Utilities.ApplicationLogging.CreateLogger();
        }

        public List<Resource> ReadAllResources()
        {
            MySqlConnection con = connectionManager.GetConnection();
            try
            {
                return ReadAllResources(con);
            }
            finally
            {
                connectionManager.CloseConnection(con);
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
            MySqlConnection con = connectionManager.GetConnection();
            try
            {
                return ReadBookings(date, con);
            }
            finally
            {
                connectionManager.CloseConnection(con);
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
                sqlCmd.Parameters.AddWithValue("@sTime", date);
                sqlCmd.Parameters.AddWithValue("@eTime", date);
                sqlCmd.Prepare();
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
            MySqlConnection con = connectionManager.GetConnection();
            try
            {
                return CheckIfBookingExist(resourceId, startTime, endTime, con);
            }
            finally
            {
                connectionManager.CloseConnection(con);
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
                sqlCmd.Parameters.AddWithValue("@rId", resourceId);
                sqlCmd.Parameters.AddWithValue("@sTime", endTime);
                sqlCmd.Parameters.AddWithValue("@eTime", startTime);
                sqlCmd.Prepare();
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

        private bool CheckIfBookingIdExist(long bookingId, MySqlConnection con)
        {
            List<Booking> bookings = new List<Booking>();
            MySqlCommand sqlCmd = null;
            MySqlDataReader sdr = null;
            try
            {
                sqlCmd = new MySqlCommand(bookingIdExistQuery, con);
                sqlCmd.Parameters.AddWithValue("@bId", bookingId);
                sqlCmd.Prepare();
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

        public Tuple<long, long> CreateBooking(Booking booking, String profileId)
        {
            MySqlConnection con = connectionManager.GetConnection();
            long bookingId = -1;
            long userId = -1;
            Tuple<long, long> response = Tuple.Create(-1L, -1L);
            try
            {
                if (CheckIfBookingExist(booking.Resource.Id, booking.StartTime, booking.EndTime, con))
                {
                    //Booking for the resource already exist in the specified time range. Raise error!
                    var resp = new HttpResponseMessage(HttpStatusCode.Forbidden)
                    {
                        Content = new StringContent("Resursen är redan bokad i angivet tidsintervall!"),
                        ReasonPhrase = "Resurs redan bokad"
                    };
                    throw new HttpResponseException(resp);
                }
                //Is the user already in DB, else create it first and get the generated ID.
                User user = getUser(profileId);
                if(user == null)
                {
                    createUser(profileId, booking.UserName, booking.Email);
                    user = getUser(profileId);
                    userId = user.Id;
                }
                bookingId = CreateBooking(booking, user.Id, con);
            }
            finally
            {
                connectionManager.CloseConnection(con);
            }
            return Tuple.Create(bookingId, userId);
        }

        private long CreateBooking(Booking booking, BigInteger userId, MySqlConnection con)
        {
            List<Booking> bookings = new List<Booking>();
            MySqlCommand sqlCmd = null;
            long createdId = -1;
            try
            {
                sqlCmd = new MySqlCommand(createNewBookingQuery, con);
                sqlCmd.Parameters.AddWithValue("@rId", booking.Resource.Id);
                sqlCmd.Parameters.AddWithValue("@uId", userId);
                sqlCmd.Parameters.AddWithValue("@sTime", booking.StartTime);
                sqlCmd.Parameters.AddWithValue("@eTime", booking.EndTime);
                sqlCmd.Prepare();
                sqlCmd.ExecuteNonQuery();

                createdId = sqlCmd.LastInsertedId;
            }
            catch (Exception e)
            {
                logger.LogError("Error occured in interaction towards DB: " + e.ToString());
            }
            finally
            {
                if (sqlCmd != null) { sqlCmd.Dispose(); }
            }
            return createdId;
        }

        public User getUser(String profileID)
        {
            MySqlConnection con = connectionManager.GetConnection();
            MySqlCommand sqlCmd = null;
            MySqlDataReader sdr = null;
            User resp = null;

            try
            {
                sqlCmd = new MySqlCommand(lookUpUser, con);
                sqlCmd.Parameters.AddWithValue("@pId", profileID);
                sqlCmd.Prepare();
                sdr = sqlCmd.ExecuteReader();

                while (sdr.Read())
                {
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
                if (sdr != null) { sdr.Close(); sdr.Dispose(); }
                connectionManager.CloseConnection(con);
            }
            return resp;
        }

        public void createUser(String profileId, String name, String email)
        {
            MySqlConnection con = connectionManager.GetConnection();
            MySqlCommand sqlCmd = null;
            try
            {
                sqlCmd = new MySqlCommand(createNewUser, con);
                sqlCmd.Parameters.AddWithValue("@pId", profileId);
                sqlCmd.Parameters.AddWithValue("@name", name);
                sqlCmd.Parameters.AddWithValue("@email", email);
                sqlCmd.Prepare();
                sqlCmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                logger.LogError("Error occured in interaction towards DB: " + e.ToString());
            }
            finally
            {
                connectionManager.CloseConnection(con);
            }
        }

        public void deleteUser(long uId)
        {
            MySqlCommand sqlCmd = null;
            try
            {
                sqlCmd = new MySqlCommand(deleteUserQuery, connectionManager.GetConnection());
                sqlCmd.Parameters.AddWithValue("@uId", uId);
                sqlCmd.Prepare();
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

        public void deleteBooking(long bookingId, String profileId)
        {
            MySqlConnection con = connectionManager.GetConnection();

            if (CheckIfBookingIdExist(bookingId, con))
            {
                if (!CheckIfOwningBooking(bookingId, profileId, con))
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.Forbidden)
                    {
                        Content = new StringContent("Du kan bara ta bort dina egna bokningar!"),
                        ReasonPhrase = "Kunde inte ta bort bokning"
                    };
                    throw new HttpResponseException(resp);
                }

                try
                {
                    DeleteBooking(bookingId, con);
                }
                finally
                {
                    connectionManager.CloseConnection(con);
                }
            }
        }

        bool CheckIfOwningBooking(long bookingId, String profileId, MySqlConnection con)
        {
            List<Booking> bookings = new List<Booking>();
            MySqlCommand sqlCmd = null;
            MySqlDataReader sdr = null;

            try
            {
                sqlCmd = new MySqlCommand(bookingOnProfileIdQuery, con);
                sqlCmd.Parameters.AddWithValue("@bId", bookingId);
                sqlCmd.Parameters.AddWithValue("@pId", profileId);
                sqlCmd.Prepare();
                sdr = sqlCmd.ExecuteReader();

                if(sdr.HasRows)
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
                if (sqlCmd != null) { sqlCmd.Dispose(); }
                if (sdr != null) { sdr.Close(); sdr.Dispose(); }
            }
            return false;
        }

        private void DeleteBooking(long bookingId, MySqlConnection con)
        {
            MySqlCommand sqlCmd = null;
            try
            {
                sqlCmd = new MySqlCommand(deleteBookingQuery, con);
                sqlCmd.Parameters.AddWithValue("@bId", bookingId);
                sqlCmd.Prepare();
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