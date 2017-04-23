//Sample provided by Fabio Galuppo  
//April 2017

using System;
using System.Text;

static class Program
{
    private static PinnedBuffer ASCII(String str)
    {
        return new PinnedBuffer(Encoding.ASCII.GetBytes(str));
    }

    static void Main(string[] args)
    {
        Console.WriteLine("ZMQ version = " + ZMQ.GetVersionString());

        IntPtr zmqContext = ZMQ.zmq_ctx_new();
        IntPtr requestSocket = ZMQ.zmq_socket(zmqContext, ZMQ.ZMQ_REQ);
        using (var endpoint = ASCII("tcp://localhost:60000"))
        {
            ZMQ.zmq_connect(requestSocket, endpoint);

            String msg = "Hello, World!";
            if (args.Length > 0)
                msg = args[0];

            using(var buffer = ASCII(msg))
                ZMQ.zmq_send(requestSocket, buffer, buffer.Length, 0);
            
            String text;
            using(var buffer = new PinnedBuffer(new byte[64]))
            {
                ZMQ.zmq_recv(requestSocket, buffer, buffer.Length, 0);
                text = Encoding.ASCII.GetString(buffer);
            }
            Console.WriteLine(text);
        }

        ZMQ.zmq_close(requestSocket);
        ZMQ.zmq_ctx_term(zmqContext);
    }
}
