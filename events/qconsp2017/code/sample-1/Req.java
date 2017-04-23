//Sample provided by Fabio Galuppo  
//April 2017

//compile:
//sudo javac -d bin -cp .:../refs/zmq.jar Req.java

//run:
//java -Djava.library.path="/Users/fabiogaluppo/zmq/samples/refs" -cp .:./bin:../refs/zmq.jar Req

import org.zeromq.ZMQ;
import org.zeromq.ZMQ.Socket;
import org.zeromq.ZMQ.Context;

public final class Req {
    @SuppressWarnings("unchecked")
    public static void main(String[] args) {
        System.out.println("ZMQ version = " + ZMQ.getVersionString());

        Context zmqContext = ZMQ.context(1);

        Socket requestSocket = zmqContext.socket(ZMQ.REQ);
        requestSocket.connect("tcp://localhost:60000");

        String msg = "Hello, World!";
        if (args.length > 0)
            msg = args[0];
        
        requestSocket.send(msg);
        byte[] data = requestSocket.recv(0);
        String text = new String(data);
        System.out.println(text);

        requestSocket.close();
        zmqContext.close();
    }
} 