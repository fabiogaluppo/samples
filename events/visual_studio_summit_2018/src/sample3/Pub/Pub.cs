//Sample provided by Fabio Galuppo  
//May 2018

using System;
using System.Threading;

sealed class Publisher
{
    static void PublishLoop(SWIGTYPE_p_void pubSocket)
    {
        String last = String.Empty;
        int seed = (int)DateTime.Now.Ticks; //narrowing (discard MSBs)
        var rnd = new Random(seed);
        var symbols = new String[] { "MSFT", "GOOG", "TSLA", "AMZN", "NVDA", "FB", "PS" };
        var lastBidPrice = new double[] { 96.71, 1065.13, 277.76, 1571.05, 240.28, 182.50, 19.94 };
        var throttle = TimeSpan.FromMilliseconds(100); //~10 msgs/s
        while (true)
        {
            int i = rnd.Uniform(0, symbols.Length);
            var symbol = symbols[i];
            //var bidPrice = lastBidPrice[i] + (lastBidPrice[i] * rnd.Uniform(-0.0125, 0.0125));
            var bidPrice = lastBidPrice[i] + (lastBidPrice[i] * (rnd.Gaussian() / 100.0));                
            lastBidPrice[i] = bidPrice;
            var quantity = rnd.Uniform(50, 1000 + 1);
            var tradeType = (MarketOrder.TypeOfTrade)rnd.Uniform(1, 5 + 1);
            var timestamp = DateTime.Now;
            var order = new MarketOrder(symbol, bidPrice, quantity, tradeType, timestamp);
            using(var buffer = new PinnedBuffer(MarketOrder.ToBytes(order))) 
            {
                if (-1 != ZeroMQ.zmq_send(pubSocket, buffer, buffer.Length, 0))
                {
                    last = String.Format("Success: Message [{0}] sent", order);                    
                }
                else
                {
                    last = String.Format("Warning: Message [{0}] not sent", order);                    
                }
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