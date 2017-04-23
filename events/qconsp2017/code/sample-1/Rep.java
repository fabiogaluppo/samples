//Sample provided by Fabio Galuppo  
//April 2017

//compile:
//sudo javac -d bin -cp .:../refs/zmq.jar Rep.java

//run:
//java -Djava.library.path="/Users/fabiogaluppo/zmq/samples/refs" -cp .:./bin:../refs/zmq.jar Rep

import org.zeromq.ZMQ;
import org.zeromq.ZMQ.Socket;
import org.zeromq.ZMQ.Context;

public final class Rep {
    @SuppressWarnings("unchecked")
    public static void main(String[] args) {
        System.out.println("ZMQ version = " + ZMQ.getVersionString());
        
        Context zmqContext = ZMQ.context(1);

        Socket replySocket = zmqContext.socket(ZMQ.REP);
        
        replySocket.bind("tcp://*:60000");

        byte[] data = replySocket.recv(0);
        String text = new String(data);
        System.out.println(text);
        replySocket.send(text.toUpperCase());

        replySocket.close();
        zmqContext.close();
    }
} 