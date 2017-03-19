package se.hexabit.paxa.rest.types;


/**
 * Created by nightzero on 2017-03-18.
 */
public class Resource {
    private String name;
    private int id;

    public Resource() {
    }

    public Resource(String name, int id) {
        this.name = name;
        this.id = id;
    }

    public String getName() {
        return name;
    }

    public int getId() {
        return id;
    }

    public void setName(String name) {
        this.name = name;
    }

    public void setId(int id) {
        this.id = id;
    }
}
