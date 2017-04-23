//Sample provided by Fabio Galuppo  
//April 2017

//compile: 
//javac -d bin -cp .;./refs/zmq.jar;./refs/HdrHistogram-2.1.9.jar SocketPairOps.java

//set PATH before run (***): 
//set PATH=%PATH%;%CD%\refs

//run:
//java -cp .;./bin;./refs/zmq.jar;./refs/HdrHistogram-2.1.9.jar SocketPairOps

import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.util.concurrent.atomic.AtomicInteger;
import java.util.concurrent.CountDownLatch;
import java.util.concurrent.locks.LockSupport;

import org.zeromq.ZMQ;
import org.zeromq.ZMQ.Socket;
import org.zeromq.ZMQException;
import org.zeromq.ZMQ.Context;
import org.zeromq.ZMQ.Poller;

public final class SocketPairOps {
    private synchronized static void log(String msg, Exception e) {
        System.out.format("[%05d] ", java.lang.Thread.currentThread().getId());
        System.out.println("**** LOG: " + msg + " ****");
        if (e != null)
            e.printStackTrace();
    }

    private static void log(String msg) {
        log(msg, null);
    }

    public static int send(SocketPair socketPair, ByteBuffer bb) {
        return SocketOps.send(socketPair.getSenderSocket(), bb);
    }

    public static int sendMore(SocketPair socketPair, ByteBuffer bb) {
        return SocketOps.sendMore(socketPair.getSenderSocket(), bb);
    }

    public static boolean trySend(SocketPair socketPair, ByteBuffer bb) {
        return SocketOps.trySend(socketPair.getSenderSocket(), bb);
    }

    public static boolean trySendMore(SocketPair socketPair, ByteBuffer bb) {
        return SocketOps.trySendMore(socketPair.getSenderSocket(), bb);
    }    

    public static int bulkSend(SocketPair socketPair, ByteBuffer... bbs) {
        return SocketOps.bulkSend(socketPair.getSenderSocket(), bbs);
    }

    public static long bulkSendWithResult(SocketPair socketPair, ByteBuffer... bbs) {
        return SocketOps.bulkSendWithResult(socketPair.getSenderSocket(), bbs);
    }

    public static boolean tryBulkSend(SocketPair socketPair, ByteBuffer... bbs) {
        return SocketOps.tryBulkSend(socketPair.getSenderSocket(), bbs);
    }

    public static int receive(SocketPair socketPair, ByteBuffer bb) {
        return SocketOps.receive(socketPair.getReceiverSocket(), bb);
    }

    public static boolean tryReceive(SocketPair socketPair, ByteBuffer bb) {
        return SocketOps.tryReceive(socketPair.getReceiverSocket(), bb);
    }

    public static int bulkReceive(SocketPair socketPair, ByteBuffer... bbs) {
        return SocketOps.bulkReceive(socketPair.getReceiverSocket(), bbs);
    }

    public static long bulkReceiveWithResult(SocketPair socketPair, ByteBuffer... bbs) {
        return SocketOps.bulkReceiveWithResult(socketPair.getReceiverSocket(), bbs);
    }

    public static boolean tryBulkReceive(SocketPair socketPair, ByteBuffer... bbs) {
        return SocketOps.tryBulkReceive(socketPair.getReceiverSocket(), bbs);
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

    private synchronized static void display(String text, String label) {
        System.out.format("[%05d] ", java.lang.Thread.currentThread().getId());
        if (label != null) System.out.print(label + ": ");
        System.out.println(text);
    }

    private static void display(String text) {
        display(text, null);
    }

    private static void test1(SocketPair dealer, SocketPair router) {
        Poller poller = new Poller(2);
        poller.register(router.getReceiverSocket(), Poller.POLLIN); //0
        poller.register(dealer.getReceiverSocket(), Poller.POLLIN); //1

        ByteBuffer bb1 = ByteBuffer.allocateDirect(256).order(ByteOrder.nativeOrder());
        ByteBuffer bb2 = ByteBuffer.allocateDirect(256).order(ByteOrder.nativeOrder());
        ByteBuffer bb3 = ByteBuffer.allocateDirect(256).order(ByteOrder.nativeOrder());
        bb1.putInt(0, 1234);
        bb2.putInt(0, 5678);
        bb3.putInt(0, 9999);

        int n = SocketPairOps.bulkSend(dealer, bb1, bb2, bb3);
        
        ByteBuffer bbId = ByteBuffer.allocateDirect(4).order(ByteOrder.nativeOrder());
        bb1.putInt(0, 0);
        bb2.putInt(0, 0);
        bb3.putInt(0, 0);

        while(true) {
            poller.poll(0);
            if (poller.pollin(0)) {
                n = SocketPairOps.bulkReceive(router, bbId, bb1, bb2, bb3);
                System.out.println("router received:");
                displayBytes(bbId, bbId.capacity());
                displayBytes(bb1,  bb1.capacity());
                displayBytes(bb2,  bb2.capacity());
                displayBytes(bb3,  bb3.capacity());
                break;
            }
        }

        SocketPairOps.bulkSend(router, bbId, bb1, bb2, bb3);

        bb1.putInt(0, 0);
        bb2.putInt(0, 0);
        bb3.putInt(0, 0);

        while(true) {
            poller.poll(0);
            if (poller.pollin(1)) {
                n = SocketPairOps.bulkReceive(dealer, bb1, bb2, bb3);
                System.out.println("dealer received:");
                displayBytes(bb1,  bb1.capacity());
                displayBytes(bb2,  bb2.capacity());
                displayBytes(bb3,  bb3.capacity());
                break;
            }
        }

        poller.unregister(router.getReceiverSocket());
        poller.unregister(dealer.getReceiverSocket());
    }

    private static void Await(CountDownLatch latch) {
        try {
            latch.await();
        } catch (InterruptedException e) {
            log("InterruptedException in latch");
            Thread.currentThread().interrupt();
        }
    }

    private static void test2() {
        Context zmqContext = ZMQ.context(4);
        String dealerId = "DEAL";
        SocketPair dealer = SocketPair.newDealer(zmqContext, dealerId, "tcp://localhost:6000", "tcp://localhost:6001", socketPair -> { //specializing config
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
        });
        SocketPair router = SocketPair.newRouter(zmqContext, "tcp://*:6001", "tcp://*:6000", socketPair -> { //specializing config
            Socket senderSocket = socketPair.getSenderSocket();
            Socket receiverSocket = socketPair.getReceiverSocket();

            senderSocket.setRouterMandatory(true); //Enable exception throwing when router sends to a unavailable dealer
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

        final int BUFFER_SIZE = 1024;
        final int N = 1000000;
        CountDownLatch latch = new CountDownLatch(4);
        CountDownLatch latchForAllSocketsConnected = new CountDownLatch(1);

        int [] counters = new int[4];

        Thread dealerSenderThread = new Thread() {
            public void run() {
                ByteBuffer bb = ByteBuffer.allocateDirect(BUFFER_SIZE).order(ByteOrder.BIG_ENDIAN);
                bb.putInt(0, 1234);
                latch.countDown();
                Await(latch);
                Await(latchForAllSocketsConnected);
                //display("dealerSenderThread running"); //DBG
                for (int counter = 0; counter < N; ++counter, counters[0] = counter) {
                    bb.putInt(4, counter + 1);
                    while(!SocketPairOps.trySend(dealer, bb));
                    //displayBytes(bb, 8, "dealer sent"); //DBG
                }
            }
        };
        
        AtomicInteger received = new AtomicInteger(0);

        Thread routerReceiverThread = new Thread() {
            public void run() {
                Poller poller = new Poller(1);
                poller.register(router.getReceiverSocket(), Poller.POLLIN); //0
                ByteBuffer bb = ByteBuffer.allocateDirect(BUFFER_SIZE).order(ByteOrder.BIG_ENDIAN);
                ByteBuffer bbId = ByteBuffer.allocateDirect(4).order(ByteOrder.nativeOrder());
                latch.countDown();
                Await(latch);
                Await(latchForAllSocketsConnected);
                //display("routerReceiverThread running"); //DBG
                int counter = 0;
                while(counter < N) {
                    poller.poll(0);
                    if (poller.pollin(0)) {
                        while(!SocketPairOps.tryBulkReceive(router, bbId, bb));
                        received.incrementAndGet();
                        //displayBytes(bb, 8, "router received"); //DBG
                        ++counter;
                        counters[1] = counter;
                    }
                }
                poller.unregister(router.getReceiverSocket());
            }
        };

        Thread routerSenderThread = new Thread() {
            public void run() {
                ByteBuffer bb = ByteBuffer.allocateDirect(BUFFER_SIZE).order(ByteOrder.BIG_ENDIAN);
                ByteBuffer bbId = ByteBuffer.allocateDirect(4).order(ByteOrder.nativeOrder());
                bbId.put(dealerId.getBytes());
                bb.putInt(0, 1234);
                latch.countDown();
                Await(latch);
                Await(latchForAllSocketsConnected);
                //display("routerSenderThread running"); //DBG
                int counter = 0;
                while(counter < N) {
                    if (counter < received.get()) {                        
                        bb.putInt(4, counter + 1);
                        while(!SocketPairOps.tryBulkSend(router, bbId, bb));
                        //displayBytes(bb, 8, "router sent"); //DBG
                        ++counter;
                        counters[2] = counter;
                    }
                }
            }
        };
        
        Thread dealerReceiverThread = new Thread() {
            public void run() {
                Poller poller = new Poller(1);
                poller.register(dealer.getReceiverSocket(), Poller.POLLIN); //0
                ByteBuffer bb = ByteBuffer.allocateDirect(BUFFER_SIZE).order(ByteOrder.nativeOrder());
                latch.countDown();
                Await(latch);
                Await(latchForAllSocketsConnected);
                //display("dealerReceiverThread running"); //DBG
                int counter = 0;
                while(counter < N) {
                    poller.poll(0);
                    if (poller.pollin(0)) {
                        while(!SocketPairOps.tryReceive(dealer, bb));
                        //displayBytes(bb, 8, "dealer received"); //DBG
                        ++counter;
                        counters[3] = counter;
                    }
                }
                poller.unregister(dealer.getReceiverSocket());
            }
        };

        //display("" + dealer.getReceiverSocket().getAffinity(), "dealer receiver socket affinity"); //DBG
        //display("" + dealer.getSenderSocket().getAffinity(), "dealer sender socket affinity"); //DBG
        //display("" + router.getReceiverSocket().getAffinity(), "router receiver socket affinity"); //DBG
        //display("" + router.getSenderSocket().getAffinity(), "router sender socket affinity"); //DBG

        dealerSenderThread.start();
        dealerReceiverThread.start();
        routerReceiverThread.start();
        routerSenderThread.start();

        Await(latch);

        //warm up
        ByteBuffer oneByte = ByteBuffer.allocateDirect(1).order(ByteOrder.BIG_ENDIAN);
        oneByte.put((byte)0x1);
        while(!SocketPairOps.trySend(dealer, oneByte));        
        //display("dealer sent"); //DBG
        oneByte.rewind();
        oneByte.put((byte)0x0);
        ByteBuffer id = ByteBuffer.allocateDirect(4).order(ByteOrder.nativeOrder());
        while(!SocketPairOps.tryBulkReceive(router, id, oneByte));
        //display("router received"); //DBG
        //displayBytes(id, 4); //DBG
        //displayBytes(oneByte, 1); //DBG
        while(!SocketPairOps.tryBulkSend(router, id, oneByte));
        //display("router sent"); //DBG
        oneByte.rewind();
        oneByte.put((byte)0x0);
        while(!SocketPairOps.tryReceive(dealer, oneByte));
        //display("dealer received"); //DBG
        //displayBytes(id, 4); //DBG
        //displayBytes(oneByte, 1); //DBG
        latchForAllSocketsConnected.countDown();        
        
        try {
            routerSenderThread.join();
            routerReceiverThread.join();
            dealerReceiverThread.join();
            dealerSenderThread.join();
        } catch(InterruptedException e) {
            //swallow exception...
        }

        System.out.format("[dealer] Sent     = %d\tReceivedBack = %d\r\n[router] Received = %d\tSendBack     = %d", 
            counters[0], counters[3], counters[1], counters[2]);

        router.close();
        dealer.close();
        zmqContext.close();
    }

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
        Context zmqContext = ZMQ.context(2); //ZMQ.context(4);
        String dealerId = "DEAL";
        SocketPair dealer = SocketPair.newDealer(zmqContext, dealerId, "tcp://localhost:5558", "tcp://localhost:5559", s -> defaultSocketConfig(s));
        SocketPair router = SocketPair.newRouter(zmqContext, "tcp://*:5559", "tcp://*:5558", s -> defaultSocketConfig(s)); //sharing config
        // SocketPair router = SocketPair.newRouter(zmqContext, "tcp://*:5559", "tcp://*:5558", socketPair -> { //specializing config
        //     Socket senderSocket = socketPair.getSenderSocket();
        //     Socket receiverSocket = socketPair.getReceiverSocket();

        //     senderSocket.setLinger(0);
        //     senderSocket.setTCPKeepAlive(-1);
        //     senderSocket.setSndHWM(0);            
        //     senderSocket.setRcvHWM(0);
        //     senderSocket.setAffinity(4);

        //     receiverSocket.setLinger(0);
        //     receiverSocket.setTCPKeepAlive(-1);
        //     receiverSocket.setSndHWM(0);
        //     receiverSocket.setRcvHWM(0);
        //     receiverSocket.setAffinity(8);
        // });

        //test1(dealer, router);
        test2();
        
        router.close();
        dealer.close();
        zmqContext.close();
    }
}