package se.hexabit.paxa.rest.types;

import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import se.hexabit.paxa.rest.util.InstantToMillisDeserializer;
import se.hexabit.paxa.rest.util.InstantToMillisSerializer;

import java.time.Instant;


/**
 * Created by nightzero on 2017-03-18.
 */
public class Booking {
    private Resource resource;

    @JsonSerialize(using = InstantToMillisSerializer.class)
    @JsonDeserialize(using = InstantToMillisDeserializer.class)
    private Instant startTime;

    @JsonSerialize(using = InstantToMillisSerializer.class)
    @JsonDeserialize(using = InstantToMillisDeserializer.class)
    private Instant endTime;

    //TODO: Booked by

    public Booking() {
    }

    public Booking(Resource resource, Instant startTime, Instant endTime) {
        this.resource = resource;
        this.startTime = startTime;
        this.endTime = endTime;
    }

    public Resource getResource() {
        return resource;
    }

    public void setResource(Resource resource) {
        this.resource = resource;
    }

    public Instant getStartTime() {
        return startTime;
    }

    public void setStartTime(Instant startTime) {
        this.startTime = startTime;
    }

    public Instant getEndTime() {
        return endTime;
    }

    public void setEndTime(Instant endTime) {
        this.endTime = endTime;
    }
}
