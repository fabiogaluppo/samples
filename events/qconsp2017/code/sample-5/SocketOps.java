//Sample provided by Fabio Galuppo  
//April 2017

//compile: 
//javac -d bin -cp .;./refs/zmq.jar SocketOps.java

//set PATH before run (***): 
//set PATH=%PATH%;%CD%\refs

//run:
//java -cp .;./bin;./refs/zmq.jar SocketOps

import org.zeromq.ZMQ;
import org.zeromq.ZMQ.Socket;
import org.zeromq.ZMQException;

import java.nio.ByteBuffer;
import java.util.concurrent.locks.LockSupport;

public final class SocketOps {
    private synchronized static void log(String msg, Exception e) {
        System.out.format("[%05d] ", java.lang.Thread.currentThread().getId());
        System.out.println("**** LOG: " + msg + " ****");
        if (e != null)
            e.printStackTrace();
    }

    private static void log(String msg) {
        log(msg, null);
    }

    private static boolean canRetry(ZMQException e) {
        //org.zeromq.ZMQException: Unknown error(0x6e) - client not ready to receive data from server
        //org.zeromq.ZMQException: Resource temporarily unavailable(0xb) - high water mark exceeded
        final int errorCode = e.getErrorCode(); 
        return errorCode == 0x6e || errorCode == 0xb; 
    }

    private static int internalSend(Socket senderSocket, ByteBuffer bb, int flags) {
        final int MAX_RETRIES = 20; //TODO: Config
        boolean retry;
        int retryCounter = 0;
        do {
            retry = false;
            bb.rewind();
            try {
                int n = senderSocket.sendByteBuffer(bb, flags);                
                return n;
            } catch (ZMQException e) {
                if (canRetry(e) && retryCounter++ < MAX_RETRIES) {
                    retry = true;
                    LockSupport.parkNanos(100 * 1000); //100 microsecond //TODO: Config
                } else {
                    log("internalSend error", e);
                    //System.exit(0); //DBG
                    //throw e;
                }
            }
        } while(retry);
        return 0;
    }

    private static int internalRecv(Socket receiverSocket, ByteBuffer bb, int flags) {
        bb.rewind();
        int n = receiverSocket.recvZeroCopy(bb, bb.capacity(), flags);
        //TODO: Log if n == 0
        //if (n == 0) log("internalRecv error");
        return n;
    }

    public static class Result {
        private Result() {}

        public static boolean failed(long result) {
            return (int)(result >> 32) == 0xFFFFFFFF;
        }

        public static boolean succeeded(long result) {
            return (int)(result >> 32) == 0x00000000;
        }

        public static int value(long result) {
            return (int)(result & 0x00000000FFFFFFFFL);
        }

        private static long fail(int value) {
            return 0xFFFFFFFF00000000L | (long)value;
        }

        private static long success(int value) {
            return 0x0000000000000000L | (long)value;
        }
    }

    public static int send(Socket senderSocket, ByteBuffer bb) {
        return internalSend(senderSocket, bb, ZMQ.DONTWAIT);
    }

    public static int sendMore(Socket senderSocket, ByteBuffer bb) {
        return internalSend(senderSocket, bb, ZMQ.DONTWAIT | ZMQ.SNDMORE);
    }

    public static boolean trySend(Socket senderSocket, ByteBuffer bb) {
        boolean sent = send(senderSocket, bb) > 0;
        return sent;
    }

    public static boolean trySendMore(Socket senderSocket, ByteBuffer bb) {
        boolean sent = sendMore(senderSocket, bb) > 0;
        return sent;
    }

    public static int bulkSend(Socket senderSocket, ByteBuffer... bbs) {
        //this method ignores partial failure
        long result = bulkSendWithResult(senderSocket, bbs);
        return Result.value(result);
    }

    public static long bulkSendWithResult(Socket senderSocket, ByteBuffer... bbs) {
        int total = 0;
        int length = bbs.length;
        if (length > 0) {
            for (int i = 0; i < length - 1; ++i) {
                int n = sendMore(senderSocket, bbs[i]);
                if (n > 0)
                    total += n;
                else                    
                    return Result.fail(total); //partial
            }
            total += send(senderSocket, bbs[length - 1]);
        }
        return Result.success(total); //complete
    }

    public static boolean tryBulkSend(Socket senderSocket, ByteBuffer... bbs) {
        long result = bulkSendWithResult(senderSocket, bbs);
        boolean sent = Result.succeeded(result);
        return sent;
    }

    public static int receive(Socket receiverSocket, ByteBuffer bb) {
        return internalRecv(receiverSocket, bb, ZMQ.DONTWAIT);
    }

    public static boolean tryReceive(Socket receiverSocket, ByteBuffer bb) {
        boolean received = internalRecv(receiverSocket, bb, ZMQ.DONTWAIT) > 0;        
        return received;
    }

    public static int bulkReceive(Socket receiverSocket, ByteBuffer... bbs) {
        //this method ignores partial failure
        long result = bulkReceiveWithResult(receiverSocket, bbs);
        return Result.value(result);
    }

    public static long bulkReceiveWithResult(Socket receiverSocket, ByteBuffer... bbs) {
        int total = 0;
        int length = bbs.length;
        if (length > 0) {
            for (int i = 0; i < length; ++i) {                
                int n = receive(receiverSocket, bbs[i]);
                if (n > 0)
                    total += n;
                else
                    return Result.fail(total); //partial
            }
        }        
        return Result.success(total); //complete
    }

    public static boolean tryBulkReceive(Socket receiverSocket, ByteBuffer... bbs) {
        long result = bulkReceiveWithResult(receiverSocket, bbs);
        boolean received = Result.succeeded(result);
        return received;
    }
}
