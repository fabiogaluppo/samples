//Sample provided by Fabio Galuppo  
//May 2018

using System;

class Req 
{
    static void Main(String[] args) 
    {
        Console.WriteLine("ZMQ version = {0}", Utils.GetZmqVersion());

        var ctx = ZeroMQ.zmq_ctx_new();
        if (ctx != null)
        {
            var requestSocket = ZeroMQ.zmq_socket(ctx, ZeroMQ.ZMQ_REQ);
            if (requestSocket != null)
            {
                if (-1 != ZeroMQ.zmq_connect(requestSocket, "tcp://localhost:60000")) 
                {
                    String msg = 0 == args.Length ? 
                        "Hello World" : 
                        args[0].Substring(0, Math.Min(args[0].Length, 64));
                    using(var buffer = PinnedBuffer.ASCII(msg)) 
                    {
                        if (-1 != ZeroMQ.zmq_send(requestSocket, buffer, buffer.Length, 0)) 
                        {
                            Console.WriteLine(msg + " sent");
                            if (-1 != ZeroMQ.zmq_recv(requestSocket, buffer, buffer.Length, 0))
                            {
                                Console.WriteLine(PinnedBuffer.ASCII(buffer) + " received back");
                            }
                            else
                            {
                                Console.WriteLine("receive back failed");
                            }
                        }
                        else
                        {
                            Console.WriteLine("send failed");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("connect failed");
                }

                ZeroMQ.zmq_close(requestSocket);
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
