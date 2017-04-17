package se.hexabit.paxa.rest;


import se.hexabit.paxa.db.ResourcesDAO;
import se.hexabit.paxa.rest.types.Booking;
import se.hexabit.paxa.rest.types.Resource;

import javax.ws.rs.*;
import javax.ws.rs.core.MediaType;
import java.sql.Timestamp;
import java.text.SimpleDateFormat;
import java.time.Instant;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.List;

@Path("paxa")
public class ResourceREST {

    private ResourcesDAO resourcesDao = new ResourcesDAO();

    @GET
    @Path("/allResources")
    @Produces(MediaType.APPLICATION_JSON)
    public Resource[] getAllResources() {
        List<Resource> resp = resourcesDao.readAllResources();
        return resp.toArray(new Resource[resp.size()]);
    }

    /**
     * Dummy implementation. Should call DB in future
     * @return
     */
    private List<Resource> readAllResourcesDummy() {
        String[] hardCoded = new String[] {"Brum-Brum", "Svarten", "Oxybox"};
        List<Resource> resp = new ArrayList<Resource>();
        for (int i = 0; i < hardCoded.length; i++){
            resp.add(new Resource(hardCoded[i], i));
        }
        return resp;
    }

    @GET
    @Path("/bookingsAtDate")
    @Produces(MediaType.APPLICATION_JSON)
    public Booking[] bookingsAtDate(
            @QueryParam("date") String date) throws Exception {

        SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd");
        Date dateObj = sdf.parse(date);

        List<Booking> resp = resourcesDao.readBookings(dateObj);
        return resp.toArray(new Booking[resp.size()]);
    }

    /**
     * Dummy implementation. Should call DB in future
     * @param date
     * @return
     */
    private List<Booking> readBookingsDummy(Date date) {
        Timestamp timestamp = new Timestamp(date.getTime());
        List<Resource> res = readAllResourcesDummy();
        List<Booking> resp = new ArrayList<Booking>();
        for (int i=0; i < res.size(); i++){
            resp.add(new Booking(res.get(i), Instant.now(), Instant.now()));
        }
        return resp;
    }

    @POST
    @Path("/createNewBooking")
    @Consumes(MediaType.APPLICATION_JSON)
    public void createBooking(Booking booking) {
        resourcesDao.createBooking(booking);
    }

    //TODO: Remove booking

    //TODO: Update booking
}
