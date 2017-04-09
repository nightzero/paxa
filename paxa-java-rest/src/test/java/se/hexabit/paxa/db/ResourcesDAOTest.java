package se.hexabit.paxa.db;

import org.junit.Test;
import se.hexabit.paxa.rest.types.Booking;
import se.hexabit.paxa.rest.types.Resource;

import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.List;

import static org.junit.Assert.*;

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
}
