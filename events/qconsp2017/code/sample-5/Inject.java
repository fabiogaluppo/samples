//Sample provided by Fabio Galuppo  
//April 2017

//compile:
//sudo javac -d bin -cp .:../refs/zmq.jar Inject.java

//run:
//java -Djava.library.path="/Users/fabiogaluppo/zmq/samples/refs" -cp .:./bin:../refs/zmq.jar Inject "tcp://localhost:60000" "tcp://localhost:60001" 10 "DL1"

import java.nio.ByteBuffer;
import java.nio.ByteOrder;

import org.zeromq.ZMQ;
import org.zeromq.ZMQ.Socket;
import org.zeromq.ZMQ.Context;
import org.zeromq.ZMQ.Poller;

public final class Inject {
    public static void pause() {
        java.io.BufferedReader bufferedReader = new java.io.BufferedReader(new java.io.InputStreamReader(System.in));
        try {
            bufferedReader.readLine(); //pause
        } catch(java.io.IOException e) {
            //swallow exception...
        }
    }

    private synchronized static void displayBytes(ByteBuffer bb, int size, String label) {
        System.out.format("[%05d] ", java.lang.Thread.currentThread().getId());
        if (label != null) System.out.print(label + ": ");
        for (int i = 0; i < size; ++i) {
            System.out.format("%02X ", bb.get(i));
        }
        System.out.println();
    }

    private static void displayBytes(ByteBuffer bb, int size) {
        displayBytes(bb, size, null);
    }

    private static ByteBuffer payload(int i) {
        ByteBuffer bbPayload = ByteBuffer.allocateDirect(512).order(ByteOrder.nativeOrder());
        bbPayload.putInt(0, i);
        bbPayload.putInt(4, 0xFFFFFFFF);
        bbPayload.putInt(8, 0xDEADC0DE);
        bbPayload.putInt(12, 0xFFFFFFFF);
        return bbPayload;
    }

    public static final class ReceiverRunner implements Runnable {
        private final SocketPair dealer;
        private final int N;
        private final Poller poller;
        private ByteBuffer bbDecoder, bbPayload;

        public ReceiverRunner(SocketPair dealer, int N) {
            this.dealer = dealer;
            this.N = N;
            this.poller = new Poller(1);
            this.poller.register(dealer.getReceiverSocket(), Poller.POLLIN);

            bbDecoder  = ByteBuffer.allocateDirect(1).order(ByteOrder.nativeOrder());            
            bbPayload  = ByteBuffer.allocateDirect(256).order(ByteOrder.nativeOrder());
        }

        @Override
        public void run() {
            int counter = 0;
            while(counter < N) {
                poller.poll(0);
                if (poller.pollin(0)) {
                    while(!SocketPairOps.tryBulkReceive(dealer, bbDecoder, bbPayload));
                    displayBytes(bbPayload, 16, "payload back " + (counter + 1));
                    ++counter;
                }
            }
            poller.unregister(dealer.getReceiverSocket());
        }
    }

    @SuppressWarnings("unchecked")
    public static void main(String[] args) {
        if (args.length != 4 || args[3].length() != 3) {
            System.out.println ("usage: inject <connect to send> <connect to receive> <number of messages> <dealer id (length must be 3)>");
            return;
        }

        String routerToSendAddress = args[0]; //connect to send
        String routerToReceivAddress = args[1]; //connect to receive
        int N = Integer.parseInt(args[2]); //number of messages
        String dealerId = args[3]; //dealer id

        System.out.println("ZMQ version = " + ZMQ.getVersionString());
        System.out.println("DealerId = " + dealerId);
        System.out.println("Enter to finish...");

        Context zmqContext = ZMQ.context(2);
        
        ByteBuffer bbDecoder = ByteBuffer.allocateDirect(1).order(ByteOrder.nativeOrder());
        
        SocketPair dealer = SocketPair.newDealer(zmqContext, dealerId, routerToSendAddress, routerToReceivAddress, socketPair -> {
            Socket senderSocket = socketPair.getSenderSocket();
            Socket receiverSocket = socketPair.getReceiverSocket();

            senderSocket.setLinger(0);
            senderSocket.setTCPKeepAlive(-1);
            senderSocket.setSndHWM(1000);            
            senderSocket.setRcvHWM(1000);
            senderSocket.setAffinity(1);
                        
            receiverSocket.setLinger(0);
            receiverSocket.setTCPKeepAlive(-1);
            receiverSocket.setSndHWM(1000);
            receiverSocket.setRcvHWM(1000);
            receiverSocket.setAffinity(2);
        });

        Thread receiverThread = new Thread(new ReceiverRunner(dealer, N));
        receiverThread.start();
        
        bbDecoder.put((byte)0);
        for (int i = 0; i < N; ++i) {
            ByteBuffer bbPayload = payload(i + 1);
            boolean status = false;
            int retries = -1;
            while (!status) {
                ++retries;
                status = SocketPairOps.tryBulkSend(dealer, bbDecoder, bbPayload);
            }
            
            System.out.println("Number of retries = " + retries);
            displayBytes(bbDecoder, 1,  "decoder");
            displayBytes(bbPayload, 16, "payload");
            System.out.println("------------------------------------------------");
        }
        
        try {
            receiverThread.join();
        } catch(InterruptedException e) {
            //swallow exception...
        }

        pause();

        dealer.close();
        zmqContext.close();
    }
}