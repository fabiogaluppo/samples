//Sample provided by Fabio Galuppo  
//May 2018

using System;

sealed class Subscriber
{
    static void ListenLoop(SWIGTYPE_p_void subSocket)
    {
        string last = String.Empty; //, title = Console.Title;
        using(var buffer = new PinnedBuffer(new byte[16]))
        {
            while (true)
            {
                if (-1 != ZeroMQ.zmq_recv(subSocket, buffer.Pointer, buffer.Length, 0))
                {
                    String msg = PinnedBuffer.ASCII(buffer).Replace('\0', ' ').TrimEnd();
                    last = String.Format("Success: [{0}] received", msg);
                }
                else
                {
                    Console.WriteLine("Warning: zmq_recv failed");
                }
                //Console.Title = String.Format("{0}[{1}]", title, last);
                Console.WriteLine(last);
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
                    using(var filter = PinnedBuffer.ASCII("MSG"))
                    {
                        if (-1 != ZeroMQ.zmq_setsockopt_2(subSocket, ZeroMQ.ZMQ_SUBSCRIBE, filter.Pointer, filter.Length))
                        {
                            Console.WriteLine("Listening...");
                            Console.WriteLine("[CTRL + C] to finish...\n");
                            ListenLoop(subSocket);                            
                        }
                        else
                        {
                            Console.WriteLine("subscription failed");
                        }
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