package app;

import app.models.RectangleModel;
import app.utilities.Toolbelt;
import javafx.application.Application;
import javafx.application.Platform;
import javafx.fxml.FXMLLoader;
import javafx.scene.Cursor;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.stage.Stage;
import javafx.stage.StageStyle;


public class App extends Application {

    public static void main(String[] args) throws Exception {

      Application.launch(args);
    }

    @Override
    public void start(Stage primaryStage) throws Exception {
        Toolbelt toolbelt = new Toolbelt();
        Parent root = FXMLLoader.load(toolbelt.getResourceUrlAtPath("views/vwSplash.fxml"));

        primaryStage.setTitle ("Jack The Video Ripper");
        primaryStage.setScene(new Scene(root, 550, 400));
        primaryStage.getScene().setCursor(Cursor.WAIT);
        primaryStage.initStyle(StageStyle.UNDECORATED);

        primaryStage.setAlwaysOnTop(true);
        primaryStage.setResizable(false);

        RectangleModel bounds = toolbelt.getScreenCenterForStage(primaryStage.getScene().getWidth(), primaryStage.getScene().getHeight());
        primaryStage.setX(bounds.getX());
        primaryStage.setY(bounds.getY());

        primaryStage.show();
    }

    public static void exitApplication() {
        Platform.exit();
        System.exit(0);
    }
}
