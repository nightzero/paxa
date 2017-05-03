package se.hexabit.paxa.rest.types;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import se.hexabit.paxa.rest.util.InstantToMillisSerializer;
import se.hexabit.paxa.rest.util.LongToInstantDeserializer;

import java.time.Instant;


/**
 * Created by nightzero on 2017-03-18.
 */
public class Booking {
    private long id = 0;

    private Resource resource;

    private String userName;

    private String email;

    @JsonSerialize(using = InstantToMillisSerializer.class)
    @JsonDeserialize(using = LongToInstantDeserializer.class)
    private Instant startTime;

    @JsonSerialize(using = InstantToMillisSerializer.class)
    @JsonDeserialize(using = LongToInstantDeserializer.class)
    private Instant endTime;

    public Booking() {
    }

    public Booking(long id, Resource resource, String userName, String email, Instant startTime, Instant endTime) {
        this.id = id;
        this.resource = resource;
        this.userName = userName;
        this.email = email;
        this.startTime = startTime;
        this.endTime = endTime;
    }

    @JsonProperty("resource")
    public Resource getResource() {
        return resource;
    }

    @JsonProperty("resource")
    public void setResource(Resource resource) {
        this.resource = resource;
    }

    @JsonProperty("userName")
    public String getUserName() {
        return userName;
    }

    @JsonProperty("userName")
    public void setUserName(String userName) {
        this.userName = userName;
    }

    @JsonProperty("email")
    public String getEmail() {
        return email;
    }

    @JsonProperty("email")
    public void setEmail(String email) {
        this.email = email;
    }

    @JsonProperty("startTime")
    public Instant getStartTime() {
        return startTime;
    }

    @JsonProperty("startTime")
    public void setStartTime(Instant startTime) {
        this.startTime = startTime;
    }

    @JsonProperty("endTime")
    public Instant getEndTime() {
        return endTime;
    }

    @JsonProperty("endTime")
    public void setEndTime(Instant endTime) {
        this.endTime = endTime;
    }

    @JsonProperty("id")
    public long getId() {
        return id;
    }

    @JsonProperty("id")
    public void setId(long id) {
        this.id = id;
    }
}
