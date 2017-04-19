package se.hexabit.paxa.db;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import se.hexabit.paxa.rest.types.Booking;
import se.hexabit.paxa.rest.types.Resource;

import java.sql.*;
import java.util.ArrayList;
import java.util.List;

//TODO: Show errors in REST interface?
/**
 * Created by night on 2017-04-05.
 */
public class ResourcesDAO {
    private Logger logger = LoggerFactory.getLogger(ResourcesDAO.class);

    String allResourcesQuery = "SELECT id, name FROM resources";
    String bookingsAtDateQuery = "SELECT b.*, r.name FROM bookings b JOIN resources r on r.id = b.resource_id where DATE(startTime) <= DATE(?) AND DATE(endTime) >= DATE(?)";
    String createNewBooking = "INSERT INTO bookings (resource_id, startTime, endTime) VALUES (?, ?, ?)";
    String deleteBooking = "DELETE FROM bookings WHERE id = ?";

    public List<Resource> readAllResources() {
        Connection connection = getConnection();
        try {
            return readAllResources(connection);
        }
        finally
        {
            try {if (connection != null) connection.close();}
            catch (Exception e) {}
        }
    }

    private List<Resource> readAllResources(Connection connection) {
        List<Resource> resp = new ArrayList<Resource>();
        PreparedStatement ps = null;
        ResultSet rs = null;
        try
        {
            ps = connection.prepareStatement(allResourcesQuery);
            rs = ps.executeQuery();
            while ( rs.next() )
            {
                int id = rs.getInt("id");
                String name =  rs.getString("name");
                resp.add(new Resource(name, id));
            }
        }
        catch (Exception e) {
            logger.error("Error occured in interaction towards DB: ", e);
        }
        finally
        {
            try {
                if (rs != null) rs.close();
                if (ps != null) ps.close();
            }
            catch (Exception e) {}
        }
        return resp;
    }

    public List<Booking> readBookings(java.util.Date date) {
        Connection connection = getConnection();
        try {
            return readBookings(date, connection);
        }
        finally
        {
            try {if (connection != null) connection.close();}
            catch (Exception e) {}
        }
    }

    private List<Booking> readBookings(java.util.Date date, Connection connection) {
        List<Booking> resp = new ArrayList<Booking>();
        PreparedStatement ps = null;
        ResultSet rs = null;
        try
        {
            ps = connection.prepareStatement(bookingsAtDateQuery);
            ps.setDate(1, new java.sql.Date(date.getTime()));
            ps.setDate(2, new java.sql.Date(date.getTime()));
            rs = ps.executeQuery();
            while ( rs.next() )
            {
                long id = rs.getLong("id");
                int resId = rs.getInt("resource_id");
                String resName = rs.getString("name");
                Resource res = new Resource(resName, resId);
                Timestamp startDate =  rs.getTimestamp("startTime");
                Timestamp endDate =  rs.getTimestamp("endTime");
                resp.add(new Booking(id, res, startDate.toInstant(), endDate.toInstant()));
            }
        }
        catch (Exception e) {
            logger.error("Error occured in interaction towards DB: ", e);
        }
        finally
        {
            try {
                if (rs != null) rs.close();
                if (ps != null) ps.close();
            }
            catch (Exception e) {}
        }
        return resp;
    }

    public void createBooking(Booking booking) {
        Connection connection = getConnection();
        try {
            createBooking(booking, connection);
        }
        finally
        {
            try {if (connection != null) connection.close();}
            catch (Exception e) {}
        }
    }

    private void createBooking(Booking booking, Connection connection) {
        PreparedStatement ps = null;
        try
        {
            ps = connection.prepareStatement(createNewBooking);
            ps.setInt(1, booking.getResource().getId());
            ps.setTimestamp(2, Timestamp.from(booking.getStartTime()));
            ps.setTimestamp(3, Timestamp.from(booking.getEndTime()));
            ps.executeUpdate();

        }
        catch (Exception e) {
            logger.error("Error occured in interaction towards DB: ", e);
        }
        finally
        {
            try {
                if (ps != null) ps.close();
            }
            catch (Exception e) {}
        }
    }

    public void deleteBooking(long bookingId) {
        Connection connection = getConnection();
        try {
            deleteBooking(bookingId, connection);
        }
        finally
        {
            try {if (connection != null) connection.close();}
            catch (Exception e) {}
        }
    }

    private void deleteBooking(long bookingId, Connection connection) {
        PreparedStatement ps = null;
        try
        {
            ps = connection.prepareStatement(deleteBooking);
            ps.setLong(1, bookingId);
            ps.executeUpdate();

        }
        catch (Exception e) {
            logger.error("Error occured in interaction towards DB: ", e);
        }
        finally
        {
            try {
                if (ps != null) ps.close();
            }
            catch (Exception e) {}
        }
    }

    private Connection getConnection() {
        Connection conn = null;
        try {
            //TODO: Do not hard code host, username, password
            //conn = DriverManager.getConnection("jdbc:mysql://localhost/paxa?user=root&password=zodiac&useJDBCCompliantTimezoneShift=true&useLegacyDatetimeCode=false&serverTimezone=UTC");
            conn = DriverManager.getConnection("jdbc:mysql://localhost/paxa?user=root&password=zodiac&useJDBCCompliantTimezoneShift=true&useLegacyDatetimeCode=false&serverTimezone=Europe/Stockholm");
            //conn = DriverManager.getConnection("jdbc:mysql://localhost/paxa?user=root&password=zodiac&useLegacyDatetimeCode=false&serverTimezone=UTC");
        } catch (SQLException e) {
            logger.error("Error occured retreiving connection towards DB: ", e);
        }
        return conn;
    }
}
