package se.hexabit.paxa.rest.util;

import com.fasterxml.jackson.core.JsonParser;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.core.JsonToken;
import com.fasterxml.jackson.databind.DeserializationContext;
import com.fasterxml.jackson.databind.deser.std.StdDeserializer;

import java.io.IOException;
import java.time.Instant;

/**
 * Created by nightzero on 2017-03-19.
 */
public class InstantToMillisDeserializer extends StdDeserializer<Instant> {

    public InstantToMillisDeserializer() {
        this(null);
    }

    public InstantToMillisDeserializer(Class<Instant> t) {
        super(t);
    }

    public Instant deserialize(JsonParser jp, DeserializationContext dc)
            throws IOException, JsonProcessingException {
        long timestamp = 0;
        JsonToken currentToken = null;
        while ((currentToken = jp.nextValue()) != null) {
            switch (currentToken) {
                case VALUE_NUMBER_INT:
                    if (jp.getCurrentName().equals("startTime")) {
                        timestamp = jp.getLongValue();
                        return Instant.ofEpochMilli(timestamp);
                    }
                    else if (jp.getCurrentName().equals("endTime")) {
                        timestamp = jp.getLongValue();
                        return Instant.ofEpochMilli(timestamp);
                    }
                    break;
                default:
                    break;
            }
        }
        return Instant.ofEpochMilli(timestamp);
    }
}