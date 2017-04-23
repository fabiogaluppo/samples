//Sample provided by Fabio Galuppo  
//April 2017

//compile: 
//javac -d bin -cp .;./refs/zmq.jar SocketPair.java

//set PATH before run (***): 
//set PATH=%PATH%;%CD%\refs

//run:
//java -cp .;./bin;./refs/zmq.jar SocketPair

import java.util.function.Consumer;

import org.zeromq.ZMQ;
import org.zeromq.ZMQ.Context;
import org.zeromq.ZMQ.Socket;

public final class SocketPair implements AutoCloseable {
    public SocketPair(Socket senderSocket, Socket receiverSocket, Consumer<SocketPair> configSockets) {
        //TOOD: assert
        this.senderSocket = senderSocket;
        this.receiverSocket = receiverSocket;

        if (null != configSockets)
            configSockets.accept(this);
    }

    public final void close() {
        senderSocket.close();
        receiverSocket.close();
    }

    public final Socket getSenderSocket()   { return senderSocket;   }

    public final Socket getReceiverSocket() { return receiverSocket; }

    private static SocketPair newInstance(Socket senderSocket, Socket receiverSocket, Consumer<SocketPair> configSockets) {
        return new SocketPair(senderSocket, receiverSocket, configSockets);
    }

    public static SocketPair newDealer(Context zmqContext, String id, String routerToSendAddress, String routerToReceivAddress, Consumer<SocketPair> configSockets) {
        Socket dealerSenderSocket = zmqContext.socket(ZMQ.DEALER);
        Socket dealerReceiverSocket = zmqContext.socket(ZMQ.DEALER);
        byte[] idBytes = id.getBytes();
        dealerSenderSocket.setIdentity(idBytes);
        dealerReceiverSocket.setIdentity(idBytes);
        SocketPair dealer = newInstance(dealerSenderSocket, dealerReceiverSocket, configSockets);
        dealer.getSenderSocket().connect(routerToSendAddress); //dealer sender connects to router receiver
        dealer.getReceiverSocket().connect(routerToReceivAddress); //dealer receiver connects to router sender
        return dealer;
    }

    public static SocketPair newRouter(Context zmqContext, String routerSenderBindAddress, String routerReceiverBindAddress, Consumer<SocketPair> configSockets) {
        Socket routerSenderSocket = zmqContext.socket(ZMQ.ROUTER);
        Socket routerReceiverSocket = zmqContext.socket(ZMQ.ROUTER);        
        SocketPair router = newInstance(routerSenderSocket, routerReceiverSocket, configSockets);
        router.getSenderSocket().bind(routerSenderBindAddress); //dealer receiver connects to router sender
        router.getReceiverSocket().bind(routerReceiverBindAddress); //dealer sender connects to router receiver
        return router;
    }
 
    private Socket senderSocket;
    private Socket receiverSocket;

    private static void defaultSocketConfig(SocketPair socketPair) {
        //  Add your socket options here.
        //  For example ZMQ_RATE, ZMQ_RECOVERY_IVL and ZMQ_MCAST_LOOP for PGM.
        //http://api.zeromq.org/4-1:zmq-setsockopt
        
        Socket senderSocket = socketPair.getSenderSocket();
        Socket receiverSocket = socketPair.getReceiverSocket();

        senderSocket.setLinger(0);
        senderSocket.setTCPKeepAlive(-1);
        senderSocket.setSndHWM(0);            
        senderSocket.setRcvHWM(0);
        senderSocket.setAffinity(1);

        receiverSocket.setLinger(0);
        receiverSocket.setTCPKeepAlive(-1);
        receiverSocket.setSndHWM(0);
        receiverSocket.setRcvHWM(0);
        receiverSocket.setAffinity(2);
    }

    @SuppressWarnings("unchecked")
    public static void main(String[] args) {
        //Test case
        Context zmqContext = ZMQ.context(4); //ZMQ.context(2)
        String dealerId = "DEAL";
        SocketPair dealer = SocketPair.newDealer(zmqContext, dealerId, "tcp://localhost:5558", "tcp://localhost:5559", s -> defaultSocketConfig(s));
        //SocketPair router = SocketPair.newRouter(zmqContext, "tcp://*:5559", "tcp://*:5558", s -> defaultSocketConfig(s)); //sharing config
        SocketPair router = SocketPair.newRouter(zmqContext, "tcp://*:5559", "tcp://*:5558", socketPair -> { //specializing config
            Socket senderSocket = socketPair.getSenderSocket();
            Socket receiverSocket = socketPair.getReceiverSocket();

            senderSocket.setLinger(0);
            senderSocket.setTCPKeepAlive(-1);
            senderSocket.setSndHWM(0);            
            senderSocket.setRcvHWM(0);
            senderSocket.setAffinity(4);

            receiverSocket.setLinger(0);
            receiverSocket.setTCPKeepAlive(-1);
            receiverSocket.setSndHWM(0);
            receiverSocket.setRcvHWM(0);
            receiverSocket.setAffinity(8);
        });
        byte data[] = new byte[1]; 
        data[0] = 127;
        dealer.getSenderSocket().send(data, 0);
        data = null;
        byte id[] = router.getReceiverSocket().recv(0);
        data = router.getReceiverSocket().recv(0);
        System.out.println(String.format("router received = 0x%X", data[0]));
        router.getSenderSocket().send(dealerId.getBytes(), ZMQ.SNDMORE);
        router.getSenderSocket().send(data, 0);
        data = null;
        data = dealer.getReceiverSocket().recv(0);
        System.out.println(String.format("dealer received = 0x%X", data[0]));        
        router.close();
        dealer.close();
        zmqContext.close();
    }
}