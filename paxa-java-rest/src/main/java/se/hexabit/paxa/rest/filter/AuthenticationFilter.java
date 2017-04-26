package se.hexabit.paxa.rest.filter;

import com.google.api.client.googleapis.auth.oauth2.GoogleIdToken;
import com.google.api.client.googleapis.auth.oauth2.GoogleIdTokenVerifier;
import com.google.api.client.http.javanet.NetHttpTransport;
import com.google.api.client.json.jackson.JacksonFactory;

import javax.annotation.Priority;
import javax.ws.rs.NotAuthorizedException;
import javax.ws.rs.Priorities;
import javax.ws.rs.container.ContainerRequestContext;
import javax.ws.rs.container.ContainerRequestFilter;
import javax.ws.rs.core.HttpHeaders;
import javax.ws.rs.ext.Provider;
import java.io.IOException;
import java.util.Arrays;

/**
 * Created by night on 2017-04-23.
 */
@Secured
@Provider
@Priority(Priorities.AUTHENTICATION)
public class AuthenticationFilter implements ContainerRequestFilter {

    private String CLIENT_ID = "208762726005-snfujhcqdcu40gkla949jlakd1pphmpk.apps.googleusercontent.com";

    @Override
    public void filter(ContainerRequestContext requestContext) throws IOException {
        // Get the token header from the HTTP Authorization request header, Bearer
        String authHeader = requestContext.getHeaderString(HttpHeaders.AUTHORIZATION);
        String[] split = authHeader.trim().split("\\s+");
        if(split == null || split.length != 2 || !split[0].equalsIgnoreCase("Bearer")) {
            throw new NotAuthorizedException("Autentiseringsproblem. Felaktigt angivet token.");
        }
        String token = split[1];
        // Check if the token is present
        if (token == null || token.isEmpty()) {
            throw new NotAuthorizedException("Autentiseringsproblem. Token saknas.");
        }

        // Validate the token
        validateToken(token);
    }

    private void validateToken(String token) {
        GoogleIdTokenVerifier verifier = new GoogleIdTokenVerifier
                .Builder(new NetHttpTransport(), new JacksonFactory())
                .setAudience(Arrays.asList(CLIENT_ID))
                .build();

        GoogleIdToken idToken = null;
        try {
            idToken = verifier.verify(token);
        } catch (Exception e) {
            throw new NotAuthorizedException("Autentiseringsproblem: " + e.getMessage());
        }
        if (idToken != null) {
            GoogleIdToken.Payload payload = idToken.getPayload();
        } else {
            throw new NotAuthorizedException("Felaktigt logintoken. Har du loggat in?");
        }
    }
}