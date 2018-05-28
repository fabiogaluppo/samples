//Sample provided by Fabio Galuppo  
//May 2018

using System;

sealed class Subscriber
{
    static void ListenLoop(SWIGTYPE_p_void subSocket)
    {
        using(var buffer = new PinnedBuffer(new byte[64]))
        {
            while (true)
            {
                if (-1 != ZeroMQ.zmq_recv(subSocket, buffer.Pointer, buffer.Length, 0))
                {
                    Console.WriteLine("Success: [{0}] received", MarketOrder.FromBytes(buffer));
                }
                else
                {
                    Console.WriteLine("Warning: zmq_recv failed");
                }
            }
        }
    }

    static void Main(String[] args) 
    {
        Console.WriteLine("ZMQ version = {0}", Utils.GetZmqVersion());
        
        var ctx = ZeroMQ.zmq_ctx_new();        
        if (ctx != null)
        {
            var subSocket = ZeroMQ.zmq_socket(ctx, ZeroMQ.ZMQ_SUB);            
            if (subSocket != null)
            {
                if (0 == ZeroMQ.zmq_connect(subSocket, "tcp://localhost:60001"))
                {
                    int numberOfSubscriptions = 0;
                    for (int i = 0; i < args.Length; ++i)
                    {
                        using(var filter = PinnedBuffer.ASCII(args[i]))
                        {
                            if (-1 != ZeroMQ.zmq_setsockopt_2(subSocket, ZeroMQ.ZMQ_SUBSCRIBE, filter.Pointer, filter.Length))
                            {
                                ++numberOfSubscriptions;                            
                            }
                            else
                            {
                                Console.WriteLine("subscription failed");
                                break;
                            }
                        }
                    }

                    if (numberOfSubscriptions > 0 && numberOfSubscriptions == args.Length)
                    {
                        Console.WriteLine("Listening...");
                        Console.WriteLine("[CTRL + C] to finish...\n");
                        ListenLoop(subSocket);
                    }
                }
                else
                {
                    Console.WriteLine("connection failed");
                }
                ZeroMQ.zmq_close(subSocket);
            }
            else
            {
                Console.WriteLine("socket failed");
            }
            ZeroMQ.zmq_ctx_term(ctx);
        }
        else
        {
            Console.WriteLine("context failed");
        }
    }
}