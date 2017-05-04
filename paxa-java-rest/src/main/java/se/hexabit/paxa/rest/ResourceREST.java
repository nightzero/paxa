package se.hexabit.paxa.rest;


import se.hexabit.paxa.db.ResourcesDAO;
import se.hexabit.paxa.rest.filter.Secured;
import se.hexabit.paxa.rest.types.Booking;
import se.hexabit.paxa.rest.types.Resource;

import javax.servlet.http.HttpServletRequest;
import javax.ws.rs.*;
import javax.ws.rs.core.Context;
import javax.ws.rs.core.MediaType;
import java.text.SimpleDateFormat;
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

    @POST
    @Path("/createNewBooking")
    @Consumes(MediaType.APPLICATION_JSON)
    @Secured
    public void createBooking(Booking booking,@Context HttpServletRequest request) {
        String profileId = (String)request.getAttribute("profileId");
        String profileName = (String)request.getAttribute("profileName");
        String profileEmail = (String)request.getAttribute("profileEmail");
        booking.setUserName(profileName);
        booking.setEmail(profileEmail);
        resourcesDao.createBooking(booking, profileId);
    }

    @DELETE
    @Path("/deleteBooking")
    @Consumes(MediaType.APPLICATION_JSON)
    @Secured
    public void deleteBooking(long bookingId) {
        //TODO: Only allowed to delete your own bookings!
        resourcesDao.deleteBooking(bookingId);
    }

    //TODO: Update booking
}
