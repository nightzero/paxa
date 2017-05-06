package se.hexabit.paxa.db;

import org.junit.Test;
import se.hexabit.paxa.rest.types.Booking;
import se.hexabit.paxa.rest.types.Resource;
import se.hexabit.paxa.rest.types.User;

import java.text.SimpleDateFormat;
import java.time.*;
import java.time.temporal.ChronoUnit;
import java.time.temporal.TemporalUnit;
import java.util.Date;
import java.util.List;
import java.util.Optional;

import static org.junit.Assert.*;

//TODO: Requires a DB loaded with resources. This could be done by mocking DB.
/**
 * Created by night on 2017-04-05.
 */
public class ResourcesDAOTest {
    @Test
    public void testReadAllResources() {
        ResourcesDAO dao = new ResourcesDAO();
        List<Resource> resp = dao.readAllResources();
    }

    @Test
    public void testReadBookings() throws Exception {
        ResourcesDAO dao = new ResourcesDAO();
        SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd");
        Date dateObj = sdf.parse("2017-04-05");
        List<Booking> resp = dao.readBookings(dateObj);
    }

    @Test
    public void testCreateBooking() throws Exception {
        ResourcesDAO dao = new ResourcesDAO();
        Resource r = new Resource();
        r.setId(5);
        Booking b = new Booking();
        b.setResource(r);
        b.setStartTime(Instant.now());
        b.setEndTime(Instant.now().plus(3, ChronoUnit.HOURS));
        dao.createBooking(b, "112233");
    }

    @Test
    public void testCreateBookingAlreadyBooked() throws Exception {
        ResourcesDAO dao = new ResourcesDAO();
        Resource r = new Resource();
        r.setId(1);
        Booking b = new Booking();
        b.setResource(r);
        LocalDate localDate = LocalDate.parse("2017-05-05");

        LocalDateTime localDateTime = localDate.atTime(13,0,0);
        Instant start = localDateTime.toInstant(ZoneId.of("Europe/Stockholm").getRules().getOffset(localDateTime));
        localDateTime = localDate.atTime(14,0,0);
        Instant end = localDateTime.toInstant(ZoneId.of("Europe/Stockholm").getRules().getOffset(localDateTime));

        b.setStartTime(start);
        b.setEndTime(end);
        dao.createBooking(b, "112233");
    }

    @Test
    public void testDeleteBooking() throws Exception {
        ResourcesDAO dao = new ResourcesDAO();
        dao.deleteBooking(5L);
    }

    @Test
    public void testCreateUser() throws Exception {
        ResourcesDAO dao = new ResourcesDAO();
        dao.createUser(dao.getConnection(),"112233", "Kalle2", "kalle2@ssdk.se");
    }

    @Test
    public void testGetUser() {
        ResourcesDAO dao = new ResourcesDAO();
        Optional<User> user = dao.getUser(dao.getConnection(),"112233");
        assertEquals(user.get().getName(), "Kalle");
    }

    @Test
    public void testCheckIfBookingExist() {
        ResourcesDAO dao = new ResourcesDAO();

        LocalDate localDate = LocalDate.parse("2017-05-05");

        LocalDateTime localDateTime = localDate.atTime(13,0,0);
        Instant start = localDateTime.toInstant(ZoneId.of("Europe/Stockholm").getRules().getOffset(localDateTime));
        localDateTime = localDate.atTime(14,0,0);
        Instant end = localDateTime.toInstant(ZoneId.of("Europe/Stockholm").getRules().getOffset(localDateTime));

        boolean resp = dao.checkIfBookingExist(1, start, end, dao.getConnection());
        assertTrue(resp);

        localDateTime = localDate.atTime(14,0,0);
        start = localDateTime.toInstant(ZoneId.of("Europe/Stockholm").getRules().getOffset(localDateTime));
        localDateTime = localDate.atTime(15,0,0);
        end = localDateTime.toInstant(ZoneId.of("Europe/Stockholm").getRules().getOffset(localDateTime));

        resp = dao.checkIfBookingExist(1, start, end, dao.getConnection());
        assertFalse(resp);
    }
}
