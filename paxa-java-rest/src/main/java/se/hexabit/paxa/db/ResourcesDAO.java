package se.hexabit.paxa.db;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import se.hexabit.paxa.rest.types.Booking;
import se.hexabit.paxa.rest.types.Resource;

import java.sql.*;
import java.time.Instant;
import java.time.LocalDateTime;
import java.util.*;
import java.util.stream.Collectors;


/**
 * Created by night on 2017-04-05.
 */
public class ResourcesDAO {
    private Logger logger = LoggerFactory.getLogger(ResourcesDAO.class);

    String allResourcesQuery = "SELECT id, name FROM resources";
    String bookingsAtDateQuery = "SELECT * FROM bookings where startTime >= ?";

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
        List<Resource> resources = readAllResources(connection);
        Map<Integer, String> resMap = resources.stream().collect(
                Collectors.toMap(Resource::getId, Resource::getName));

        PreparedStatement ps = null;
        ResultSet rs = null;
        try
        {
            ps = connection.prepareStatement(bookingsAtDateQuery);
            ps.setDate(1, new java.sql.Date(date.getTime()));
            rs = ps.executeQuery();
            while ( rs.next() )
            {
                int resId = rs.getInt("resource_id");
                Resource res = new Resource(resMap.get(resId), resId);
                Timestamp startDate =  rs.getTimestamp("startTime");
                Timestamp endDate =  rs.getTimestamp("endTime");
                resp.add(new Booking(res, startDate.toInstant(), endDate.toInstant()));
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