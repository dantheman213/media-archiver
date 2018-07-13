package app.comm;


import java.util.ArrayList;
import java.util.List;

public class ProcessWrapper {
    private ProcessBuilder procBuilder;
    private Thread threadStdOut;
    private Thread threadStdErr;

    private String command;
    private List<String> args;
    private String workingDirectory;
    private String consoleBuffer;

    public ProcessWrapper() {
        args = new ArrayList<String>();
    }

    public void setCommand(String command) {
        this.command = command;
    }

    public void setWorkingDirectory(String workingDirectory) {
        this.workingDirectory = workingDirectory;
    }

    public void addArgument(String arg) {
        args.add(arg);
    }

    public void execute() {

    }

    public String getLatestConsoleOutput() {
        String buffer = consoleBuffer;
        consoleBuffer = ""; // clear main buffer for new output

        return buffer;
    }
}
