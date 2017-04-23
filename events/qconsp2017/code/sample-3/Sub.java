//Sample provided by Fabio Galuppo  
//April 2017

//compile:
//sudo javac -d bin -cp .:../refs/zmq.jar Sub.java

//run:
//java -Djava.library.path="/Users/fabiogaluppo/zmq/samples/refs" -cp .:./bin:../refs/zmq.jar Sub 5 "GOOG " "MSFT "

import org.zeromq.ZMQ;
import org.zeromq.ZMQ.Socket;
import org.zeromq.ZMQ.Context;

public final class Sub {
    private static int toInt(String value) {  
        try {  
            return Integer.parseInt(value);   
        } catch (NumberFormatException e) {  
            return 0;  
        }  
    }
    
    @SuppressWarnings("unchecked")
    public static void main(String[] args) {
        int N = 0;
        if (args.length < 2 || (N = toInt(args[0])) == 0) {
            System.out.println("usage: Sub <N messages> <topic list>");
            System.out.println("example: Sub 10 \"APPL \" \"GOOG \" \"MSFT \"");
            return;
	    }

        System.out.println("ZMQ version = " + ZMQ.getVersionString());

        Context zmqContext = ZMQ.context(1);

        Socket subscriberSocket = zmqContext.socket(ZMQ.SUB);
        //subscriberSocket.connect("tcp://localhost:60001");
        subscriberSocket.connect("ipc://pub.ipc");

        System.out.println("Listening...");
        System.out.println("[CTRL + C] to finish...");

        for (int i = 1; i < args.length; ++i) {
            subscriberSocket.subscribe(args[i].getBytes());
        }

        for (int i = 0; i < N; ++i) {
            byte[] data = subscriberSocket.recv(0);
            String text = new String(data);
            System.out.println(text);
        }

        subscriberSocket.close();
        zmqContext.close();
    }
} 