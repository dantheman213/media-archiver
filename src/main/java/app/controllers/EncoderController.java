package app.controllers;

import app.App;
import app.models.RectangleModel;
import app.utilities.Toolbelt;
import javafx.fxml.FXML;
import javafx.fxml.FXMLLoader;
import javafx.fxml.Initializable;

import javafx.event.ActionEvent;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.scene.control.*;
import javafx.stage.Modality;
import javafx.stage.Stage;

import java.net.URL;
import java.util.ResourceBundle;

public class EncoderController implements Initializable {
    @FXML
    TreeView treeMenu;

    @FXML
    TableView tableEncodeList;

    @Override
    public void initialize(URL location, ResourceBundle resources) {
        buildTreeMenu();
    }

    private void buildTreeMenu() {
        TreeItem<String> item1 = new TreeItem<String>("All (0)");
        TreeItem<String> item2 = new TreeItem<String>("Pending (0)");
        TreeItem<String> item3 = new TreeItem<String>("In Progress (0)");
        TreeItem<String> item4 = new TreeItem<String>("Completed (0)");
        TreeItem<String> item5 = new TreeItem<String>("Error (0)");

        TreeItem<String> root = new TreeItem<>("");
        root.getChildren().addAll(item1, item2, item3, item4, item5);

        treeMenu.setShowRoot(false);
        treeMenu.setRoot(root);
        treeMenu.getSelectionModel().select(treeMenu.getRow(item1));
    }

    @FXML
    public void handleMenuFileExitButtonAction(ActionEvent event) throws Exception {
        App.exitApplication();
    }

    @FXML
    public void handleMenuHelpAboutButtonAction(ActionEvent event) throws Exception {
        Toolbelt toolbelt = new Toolbelt();
        Parent root = FXMLLoader.load(toolbelt.getResourceUrlAtPath("views/vwAbout.fxml"));

        Stage stage = new Stage();
        stage.setTitle ("About");
        stage.setScene(new Scene(root, 550, 400));
        stage.initModality(Modality.APPLICATION_MODAL);
        stage.setResizable(false);

        RectangleModel bounds = toolbelt.getScreenCenterForStage(stage.getScene().getWidth(), stage.getScene().getHeight());
        stage.setX(bounds.getX());
        stage.setY(bounds.getY());

        stage.show();
    }

    @FXML
    public void handleAddVideoButtonAction(ActionEvent event) throws Exception {

    }

    @FXML
    public void handleRemoveVideoButtonAction(ActionEvent event) throws Exception {

    }

    @FXML
    public void handleStartQueueButtonAction(ActionEvent event) throws Exception {
        Alert alert = new Alert(Alert.AlertType.INFORMATION);
        alert.setContentText("This feature will be implemented soon!");
        alert.showAndWait();
    }

    @FXML
    public void handleStopQueueButtonAction(ActionEvent event) throws Exception {
        Alert alert = new Alert(Alert.AlertType.INFORMATION);
        alert.setContentText("This feature will be implemented soon!");
        alert.showAndWait();
    }
}
