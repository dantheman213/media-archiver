package app.library;

import app.models.RectangleModel;
import javafx.geometry.Rectangle2D;
import javafx.stage.Screen;

import java.net.URL;

public class Toolbelt {

    public URL getResourceUrlAtPath(String path) {
        return Toolbelt.class.getClassLoader().getResource(path);
    }

    public RectangleModel getScreenCenterForStage(double stageWidth, double stageHeight) {
        Rectangle2D screen = Screen.getPrimary().getVisualBounds();

        int stageX = (int)Math.floor((screen.getWidth() - stageWidth) / 2);
        int stageY = (int)Math.floor((screen.getHeight() - stageHeight) / 2);

        return new RectangleModel(stageX, stageY, (int)stageWidth, (int)stageHeight);
    }
}
