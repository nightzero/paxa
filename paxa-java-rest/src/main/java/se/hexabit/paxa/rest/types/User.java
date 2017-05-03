package se.hexabit.paxa.rest.types;

/**
 * Created by nightzero on 2017-03-20.
 */
public class User {
    private long id;
    private String profileId;
    private String name;
    private String email;

    public User(long id, String profileId, String name, String email) {
        this.id = id;
        this.profileId = profileId;
        this.name = name;
        this.email = email;
    }

    public long getId() {
        return id;
    }

    public void setId(long id) {
        this.id = id;
    }

    public String getProfileId() {
        return profileId;
    }

    public void setProfileId(String profileId) {
        this.profileId = profileId;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }
}
