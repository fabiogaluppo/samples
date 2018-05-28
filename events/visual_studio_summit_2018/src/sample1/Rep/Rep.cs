//Sample provided by Fabio Galuppo  
//May 2018

using System;

class Rep 
{
    static void Main(String[] args) 
    {
        Console.WriteLine("ZMQ version = {0}", Utils.GetZmqVersion());

        var ctx = ZeroMQ.zmq_ctx_new();
        if (ctx != null)
        {
            var replySocket = ZeroMQ.zmq_socket(ctx, ZeroMQ.ZMQ_REP);
            if (replySocket != null)
            {
                if (0 == ZeroMQ.zmq_bind(replySocket, "tcp://*:60000")) 
                {
                    using (var buffer = new PinnedBuffer(new byte[64])) 
                    {
                        if (-1 != ZeroMQ.zmq_recv(replySocket, buffer, buffer.Length, 0)) 
                        {
                            String msg = PinnedBuffer.ASCII(buffer).Replace('\0', ' ').TrimEnd();
                            Console.WriteLine(msg + " received");

                            if (-1 != ZeroMQ.zmq_send(replySocket, buffer, buffer.Length, 0))
                            {
                                Console.WriteLine(msg + " sent back");
                            }
                            else
                            {
                                Console.WriteLine("send failed");
                            }
                        }
                        else
                        {
                            Console.WriteLine("receive failed");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("bind failed");
                }

                ZeroMQ.zmq_close(replySocket);
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
