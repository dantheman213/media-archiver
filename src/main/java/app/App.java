package app;

import app.utilities.Toolbelt;
import javafx.application.Application;
import javafx.application.Platform;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.scene.control.Alert;
import javafx.stage.Stage;

import java.security.Provider;


public class App extends Application {

    public static void main(String[] args) throws Exception {

      Application.launch(args);
    }

    @Override
    public void start(Stage primaryStage) throws Exception {
        Toolbelt toolbelt = new Toolbelt();
        Parent root = FXMLLoader.load(toolbelt.getResourceUrlAtPath("views/vwAppMain.fxml"));

        primaryStage.setTitle ("Jack The Video Ripper");
        primaryStage.setScene(new Scene(root, 800, 600));

        primaryStage.show();
    }

    public static void exitApplication() {
        Platform.exit();
        System.exit(0);
    }
}
