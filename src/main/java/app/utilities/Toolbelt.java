package app.utilities;

import java.net.URL;

public class Toolbelt {

    public URL getResourceUrlAtPath(String path) {
        return Toolbelt.class.getClassLoader().getResource(path);
    }
}
