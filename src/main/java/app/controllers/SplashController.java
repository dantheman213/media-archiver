package app.controllers;

import app.models.RectangleModel;
import app.utilities.Toolbelt;
import javafx.application.Platform;
import javafx.concurrent.Task;
import javafx.fxml.FXML;
import javafx.fxml.FXMLLoader;
import javafx.fxml.Initializable;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.scene.layout.AnchorPane;
import javafx.stage.Stage;
import javafx.stage.StageStyle;

import java.net.URL;
import java.util.ResourceBundle;

public class SplashController implements Initializable  {
    @FXML
    private AnchorPane anchorPane;

    @Override
    public void initialize(URL location, ResourceBundle resources) {
        new Thread(() -> {
            try {
                Thread.sleep(5000);
            } catch(Exception ex) {
                ex.printStackTrace();
            }

            Platform.runLater(new Runnable() {
                @Override
                public void run() {
                    try {
                        ((Stage)anchorPane.getScene().getWindow()).close();
                        loadEncoderWindow();
                    } catch (Exception e) {
                        e.printStackTrace();
                    }
                }
            });
        }).start();
    }

    private void loadEncoderWindow() throws Exception {
        Toolbelt toolbelt = new Toolbelt();
        Parent root = FXMLLoader.load(toolbelt.getResourceUrlAtPath("views/vwEncoder.fxml"));

        Stage stage = new Stage();

        stage.setTitle ("Jack The Video Ripper");
        stage.setScene(new Scene(root, 800, 600));

        RectangleModel bounds = toolbelt.getCenterForStageInWindow(stage.getScene().getWidth(), stage.getScene().getHeight());
        stage.setX(bounds.getX());
        stage.setY(bounds.getY());

        stage.show();
    }
}