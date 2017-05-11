package se.hexabit.paxa.db;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import se.hexabit.paxa.rest.GenericException;
import se.hexabit.paxa.rest.types.Booking;
import se.hexabit.paxa.rest.types.Resource;
import se.hexabit.paxa.rest.types.User;

import java.math.BigInteger;
import java.sql.*;
import java.time.Instant;
import java.util.ArrayList;
import java.util.List;
import java.util.Optional;

/**
 * Created by night on 2017-04-05.
 */
public class ResourcesDAO {
    private Logger logger = LoggerFactory.getLogger(ResourcesDAO.class);

    private String allResourcesQuery = "SELECT id, name FROM resources";
    private String bookingsAtDateQuery = "SELECT b.*, r.name AS resource_name, u.name AS user_name, u.email FROM bookings b JOIN resources r on r.id = b.resource_id JOIN users u on u.id = b.user_id WHERE DATE(startTime) <= DATE(?) AND DATE(endTime) >= DATE(?)";
    private String bookingsExistQuery = "SELECT b.* FROM bookings b JOIN resources r on r.id = b.resource_id WHERE b.resource_id = ? AND (startTime < ?) AND (endTime > ?)";
    private String createNewBookingQuery = "INSERT INTO bookings (resource_id, user_id, startTime, endTime) VALUES (?, ?, ?, ?)";
    private String deleteBookingQuery = "DELETE FROM bookings WHERE id = ?";
    private String bookingOnProfileIdQuery = "SELECT b.id, u.profileid FROM bookings b JOIN users u on u.id = b.user_id WHERE b.id = ? AND u.profileid = ?";
    private String lookUpUser = "SELECT id, profileid, name, email FROM users WHERE profileid = ?";
    private String createNewUser = "INSERT INTO users (profileid, name, email) VALUES (?, ?, ?)";

    public List<Resource> readAllResources() {
        Connection connection = getConnection();
        try {
            return readAllResources(connection);
        }
        finally {
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
        finally {
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
        finally {
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
                String resName = rs.getString("resource_name");
                Resource res = new Resource(resName, resId);
                String userName = rs.getString("user_name");
                String email = rs.getString("email");
                Timestamp startDate =  rs.getTimestamp("startTime");
                Timestamp endDate =  rs.getTimestamp("endTime");
                resp.add(new Booking(id, res, userName, email, startDate.toInstant(), endDate.toInstant()));
            }
        }
        catch (Exception e) {
            logger.error("Error occured in interaction towards DB: ", e);
        }
        finally {
            try {
                if (rs != null) rs.close();
                if (ps != null) ps.close();
            }
            catch (Exception e) {}
        }
        return resp;
    }

    boolean checkIfBookingExist(int resourceId, Instant startTime, Instant endTime, Connection connection) {
        List<Booking> resp = new ArrayList<Booking>();
        PreparedStatement ps = null;
        ResultSet rs = null;
        try {
            ps = connection.prepareStatement(bookingsExistQuery);
            ps.setInt(1, resourceId);
            ps.setTimestamp(2, Timestamp.from(endTime));
            ps.setTimestamp(3, Timestamp.from(startTime));
            rs = ps.executeQuery();
            if(rs.first()) {
                return true;
            }
        }
        catch (Exception e) {
            logger.error("Error occured in interaction towards DB: ", e);
        }
        finally {
            try {
                if (rs != null) rs.close();
                if (ps != null) ps.close();
            }
            catch (Exception e) {}
        }

        return false;
    }

    public void createBooking(Booking booking, String profileId) throws GenericException {
        Connection connection = getConnection();
        try {
            if(checkIfBookingExist(booking.getResource().getId(), booking.getStartTime(), booking.getEndTime(), connection)) {
                //Booking for the resource already exist in the specified time range. Raise error!
                throw new GenericException("Resursen är redan bokad i angivet tidsintervall!");
            }
            //Is the user already in DB, else create it first and get the generated ID.
            Optional<User> user = getUser(connection, profileId);
            if(!user.isPresent()) {
                createUser(connection, profileId, booking.getUserName(), booking.getEmail());
                user = getUser(connection, profileId);
            }
            createBooking(booking, user.get().getId(), connection);
        }
        finally {
            try {if (connection != null) connection.close();}
            catch (Exception e) {}
        }
    }

    private void createBooking(Booking booking, BigInteger userId, Connection connection) {
        PreparedStatement ps = null;
        try
        {
            ps = connection.prepareStatement(createNewBookingQuery);
            ps.setInt(1, booking.getResource().getId());
            ps.setString(2, userId.toString());
            ps.setTimestamp(3, Timestamp.from(booking.getStartTime()));
            ps.setTimestamp(4, Timestamp.from(booking.getEndTime()));
            ps.executeUpdate();

        }
        catch (Exception e) {
            logger.error("Error occured in interaction towards DB: ", e);
        }
        finally {
            try {
                if (ps != null) ps.close();
            }
            catch (Exception e) {}
        }
    }

    Optional<User> getUser(Connection connection, String profileID) {
        PreparedStatement ps = null;
        ResultSet rs = null;
        User resp = null;

        try {
            ps = connection.prepareStatement(lookUpUser);
            ps.setString(1, profileID);
            rs = ps.executeQuery();
            while ( rs.next() )
            {
                BigInteger id = new BigInteger(rs.getString("id"));
                String profileId =  rs.getString("profileid");
                String name =  rs.getString("name");
                String email =  rs.getString("email");
                resp = new User(id, profileId, name, email);
            }
        }
        catch (Exception e) {
            logger.error("Error occured in interaction towards DB: ", e);
        }
        finally {
            try {
                if (rs != null) rs.close();
                if (ps != null) ps.close();
            }
            catch (Exception e) {}
        }
        return Optional.ofNullable(resp);
    }

    void createUser(Connection connection, String profileId, String name, String email) {
        PreparedStatement ps = null;
        try {
            ps = connection.prepareStatement(createNewUser);
            ps.setString(1, profileId);
            ps.setString(2, name);
            ps.setString(3, email);
            ps.executeUpdate();
        }
        catch (SQLIntegrityConstraintViolationException e) {
            if(e.getErrorCode() == 1062) {
                logger.debug("Trying to insert duplicate: " + e.getMessage());
            }
            else {
                logger.error("Error occured in interaction towards DB: ", e);
            }
        }
        catch (Exception e) {
            logger.error("Error occured in interaction towards DB: ", e);
        }
        finally {
            try {
                if (ps != null) ps.close();
            }
            catch (Exception e) {}
        }
    }


    public void deleteBooking(long bookingId, String profileId) throws GenericException {
        Connection connection = getConnection();

        if(!checkIfOwningBooking(bookingId, profileId, connection)) {
            //User is trying to delete another users booking
            throw new GenericException("Du kan bara ta bort dina egna bokningar!");
        }

        try {
            deleteBooking(bookingId, connection);
        }
        finally {
            try {if (connection != null) connection.close();}
            catch (Exception e) {}
        }
    }

    boolean checkIfOwningBooking(long bookingId, String profileId, Connection connection) {
        List<Booking> resp = new ArrayList<Booking>();
        PreparedStatement ps = null;
        ResultSet rs = null;
        try {
            ps = connection.prepareStatement(bookingOnProfileIdQuery);
            ps.setLong(1, bookingId);
            ps.setString(2, profileId);
            rs = ps.executeQuery();
            if(rs.first()) {
                return true;
            }
        }
        catch (Exception e) {
            logger.error("Error occured in interaction towards DB: ", e);
        }
        finally {
            try {
                if (rs != null) rs.close();
                if (ps != null) ps.close();
            }
            catch (Exception e) {}
        }

        return false;
    }

    private void deleteBooking(long bookingId, Connection connection) {
        PreparedStatement ps = null;
        try
        {
            ps = connection.prepareStatement(deleteBookingQuery);
            ps.setLong(1, bookingId);
            ps.executeUpdate();

        }
        catch (Exception e) {
            logger.error("Error occured in interaction towards DB: ", e);
        }
        finally {
            try {
                if (ps != null) ps.close();
            }
            catch (Exception e) {}
        }
    }

    Connection getConnection() {
        Connection conn = null;
        try {
            /*
            String hostName = context.getInitParameter("mysql.host");
            String port = context.getInitParameter("mysql.port");
            String database = context.getInitParameter("mysql.database");
            String userName = context.getInitParameter("mysql.username");
            String password = context.getInitParameter("mysql.password");
            */
            String hostName = "mysql34.unoeuro.com";
            String port = "3306";
            String database = "ssdk_karlshamn_se_db";
            String userName = "ssdk_karlsh_se";
            String password = "flippe05";

            //conn = DriverManager.getConnection("jdbc:mysql://localhost/paxa?user=root&password=zodiac&useSSL=false&useJDBCCompliantTimezoneShift=true&useLegacyDatetimeCode=false&serverTimezone=Europe/Stockholm");
            conn = DriverManager.getConnection("jdbc:mysql://" + hostName + ":" + port + "/" + database +"?user=" + userName + "&password=" + password + "&useSSL=false&useJDBCCompliantTimezoneShift=true&useLegacyDatetimeCode=false&serverTimezone=Europe/Stockholm");
        } catch (SQLException e) {
            logger.error("Error occured retreiving connection towards DB: ", e);
        }
        return conn;
    }
}
