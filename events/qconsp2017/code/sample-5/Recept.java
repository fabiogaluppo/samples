//Sample provided by Fabio Galuppo  
//April 2017

//compile:
//sudo javac -d bin -cp .:../refs/zmq.jar Recept.java

//run:
//java -Djava.library.path="/Users/fabiogaluppo/zmq/samples/refs" -cp .:./bin:../refs/zmq.jar Recept "tcp://*:60001" "tcp://*:60000"

import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.util.concurrent.ConcurrentLinkedQueue;

import org.zeromq.ZMQ;
import org.zeromq.ZMQ.Socket;
import org.zeromq.ZMQ.Context;
import org.zeromq.ZMQ.Poller;
import org.zeromq.ZMQException;

public final class Recept {
    private static ByteBuffer clone(ByteBuffer original) {
        ByteBuffer clone = ByteBuffer.allocateDirect(original.capacity());
        original.rewind(); //copy from the beginning
        clone.put(original);
        original.rewind();
        clone.flip();
        return clone;
    }

    private static class Buffers {
        public Buffers(ByteBuffer bbClientId, ByteBuffer bbDecoder, ByteBuffer bbPayload) {
            this.bbClientId = bbClientId;
            this.bbDecoder = bbDecoder;
            this.bbPayload = bbPayload;
        }
        public ByteBuffer bbClientId, bbDecoder, bbPayload;
    }

    public static final class SenderRunner implements Runnable {
        private final SocketPair router;
        private ConcurrentLinkedQueue<Buffers> Q;
        
        public SenderRunner(SocketPair router) {
            this.router = router;
            Q = new ConcurrentLinkedQueue<Buffers>();
        }

        public void sendBack(ByteBuffer id, ByteBuffer decoder, ByteBuffer payload) {
            Q.add(new Buffers(id, decoder, payload));
        }
        
        @Override
        public void run() {
            while (!Thread.currentThread().isInterrupted()) {
                if (!Q.isEmpty()) {
                    Buffers buffers = Q.poll();
                    while(!SocketPairOps.tryBulkSend(router, buffers.bbClientId, buffers.bbDecoder, buffers.bbPayload));
                }
            }
        }
    }

    private final static int ZMQ_ROUTER_HANDOVER = 56;

    @SuppressWarnings("unchecked")
    public static void main(String[] args) {
        if (args.length != 2) {
            System.out.println ("usage: Recept <bind to sender> <bind to receiver>");
            return;
        }

        String routerSenderBindAddress = args[0]; //bind to sender
        String routerReceiverBindAddress = args[1]; //bind to receiver        

        Context zmqContext = ZMQ.context(2);       

        SocketPair router = SocketPair.newRouter(zmqContext, routerSenderBindAddress, routerReceiverBindAddress, socketPair -> {
            Socket senderSocket = socketPair.getSenderSocket();
            Socket receiverSocket = socketPair.getReceiverSocket();

            senderSocket.setRouterMandatory(true);
            senderSocket.setLinger(0);
            senderSocket.setTCPKeepAlive(-1);
            senderSocket.setSndHWM(1000);
            senderSocket.setRcvHWM(1000);
            senderSocket.setAffinity(1);
            senderSocket.setLongSockoptUnsafe(ZMQ_ROUTER_HANDOVER, 1);

            receiverSocket.setLinger(0);
            receiverSocket.setTCPKeepAlive(-1);
            receiverSocket.setSndHWM(1000);
            receiverSocket.setRcvHWM(1000);
            receiverSocket.setAffinity(2);
            receiverSocket.setLongSockoptUnsafe(ZMQ_ROUTER_HANDOVER, 1);
        });

        ByteBuffer bbDecoder  = ByteBuffer.allocateDirect(1).order(ByteOrder.nativeOrder());
        ByteBuffer bbDealerId = ByteBuffer.allocateDirect(3).order(ByteOrder.nativeOrder());
        ByteBuffer bbPayload  = ByteBuffer.allocateDirect(512).order(ByteOrder.nativeOrder());

        Poller poller = new Poller(1);
        poller.register(router.getReceiverSocket(), Poller.POLLIN); //0

        System.out.println("ZMQ version = " + ZMQ.getVersionString());
        System.out.println("Listening...");
        System.out.println("[CTRL + C] to finish...");

        SenderRunner senderRunner = new SenderRunner(router);
        Thread senderThread = new Thread(senderRunner);
        senderThread.start();

        int count = 0;
        while(!Thread.currentThread().isInterrupted()) {
            poller.poll(0);
            if (poller.pollin(0)) {
                while(!SocketPairOps.tryBulkReceive(router, bbDealerId, bbDecoder, bbPayload));
                senderRunner.sendBack(clone(bbDealerId), clone(bbDecoder), clone(bbPayload));
            }
        }
        poller.unregister(router.getReceiverSocket());        

        try {
            senderThread.join();
        } catch(InterruptedException e) {
            //swallow exception...
        }

        router.close();
        zmqContext.close();
    }
}