package se.hexabit.paxa.rest;

import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.datatype.jsr310.JavaTimeModule;
import com.fasterxml.jackson.jaxrs.json.JacksonJsonProvider;
import org.glassfish.jersey.server.ResourceConfig;
import org.glassfish.jersey.test.JerseyTest;
import org.junit.Test;
import se.hexabit.paxa.rest.types.Booking;
import se.hexabit.paxa.rest.types.Resource;

import javax.ws.rs.core.Application;
import javax.ws.rs.core.Response;

import static junit.framework.Assert.assertEquals;

/**
 * Unit test for simple ResourceREST.
 */
public class ResourceRESTTest extends JerseyTest {

    @Override
    protected Application configure() {
        return new ResourceConfig(ResourceREST.class);
    }

    @Test
    public void testAllResources() {
        Response response = target("paxa/allResources").request().get();
        assertEquals("Should return status 200", 200, response.getStatus());
        Resource[] value = response.readEntity(Resource[].class);
        assertEquals(3, value.length);
    }

    @Test
    public void testBookingsAtDate() throws Exception {
        Response response = target("paxa/bookingsAtDate").queryParam("date", "2017-01-01").request().get();
        assertEquals("Should return status 200", 200, response.getStatus());

        Booking[] value = response.readEntity(Booking[].class);
        assertEquals(3, value.length);
    }
}
