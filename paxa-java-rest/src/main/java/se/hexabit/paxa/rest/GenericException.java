package se.hexabit.paxa.rest;

import javax.ws.rs.WebApplicationException;
import javax.ws.rs.core.Response;


/**
 * Created by night on 2017-05-07.
 */
public class GenericException extends WebApplicationException {
    public GenericException() {
        super(Response.status(Response.Status.FORBIDDEN).build());
    }

    public GenericException(String message) {
        super(Response.status(Response.Status.FORBIDDEN).entity(message).type("application/json").build());
    }
}
