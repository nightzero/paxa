package se.hexabit.paxa.rest.util;

import com.fasterxml.jackson.core.JsonGenerator;
import com.fasterxml.jackson.databind.SerializerProvider;
import com.fasterxml.jackson.databind.ser.std.StdSerializer;

import java.io.IOException;
import java.time.Instant;

/**
 * Created by nightzero on 2017-03-19.
 */
public class InstantToMillisSerializer extends StdSerializer<Instant> {
    public InstantToMillisSerializer() {
        this(null);
    }

    public InstantToMillisSerializer(Class<Instant> t) {
        super(t);
    }

    @Override
    public void serialize(Instant tmpInst,
                          JsonGenerator jsonGenerator,
                          SerializerProvider serializerProvider)
            throws IOException {
        jsonGenerator.writeObject(tmpInst.toEpochMilli());
    }
}
