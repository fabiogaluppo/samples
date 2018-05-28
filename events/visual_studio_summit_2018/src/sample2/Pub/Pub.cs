//Sample provided by Fabio Galuppo  
//May 2018

using System;
using System.Threading;

sealed class Publisher
{
    static void PublishLoop(SWIGTYPE_p_void pubSocket)
    {
        int count = 0;
        string last = String.Empty; //, title = Console.Title;
        
        var throttle = TimeSpan.FromMilliseconds(100); //~10 msgs/s
        while (true)
        {
            String msg = "MSG #" + (++count);
            using(var buffer = PinnedBuffer.ASCII(msg.PadRight(16, ' '))) 
            {
                if (-1 != ZeroMQ.zmq_send(pubSocket, buffer, buffer.Length, 0))
                {
                    last = String.Format("Success: Message [{0}] sent", msg);                    
                }
                else
                {
                    last = String.Format("Warning: Message [{0}] not sent", msg);                    
                }
                //Console.Title = String.Format("{0}[{1}]", title, last);
                Console.WriteLine(last);
                Thread.Sleep(throttle);
            }
        }
    }

    static void Main(String[] args) 
    {
        Console.WriteLine("ZMQ version = {0}", Utils.GetZmqVersion());

        var ctx = ZeroMQ.zmq_ctx_new();
        if (ctx != null)
        {
            var pubSocket = ZeroMQ.zmq_socket(ctx, ZeroMQ.ZMQ_PUB);
            if (pubSocket != null)
            {
                if (0 == ZeroMQ.zmq_bind(pubSocket, "tcp://*:60001"))
                {
                    Console.WriteLine("Publishing...");
                    Console.WriteLine("[CTRL + C] to finish...\n");
                    PublishLoop(pubSocket);
                }
                else
                {
                    Console.WriteLine("bind failed");
                }
                ZeroMQ.zmq_close(pubSocket);
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