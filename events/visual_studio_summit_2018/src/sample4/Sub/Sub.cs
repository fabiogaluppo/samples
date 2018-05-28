//Sample provided by Fabio Galuppo  
//May 2018

using System;
using System.Globalization;
using System.Reactive.Subjects;
using System.Reactive.Linq;

sealed class Subscriber
{
    static void ListenLoop(SWIGTYPE_p_void subSocket, Subject<MarketOrder> marketOrderSubject)
    {
        using(var buffer = new PinnedBuffer(new byte[64]))
        {
            while (true)
            {
                if (-1 != ZeroMQ.zmq_recv(subSocket, buffer.Pointer, buffer.Length, 0))
                {
                    //Console.WriteLine("Success: [{0}] received", MarketOrder.FromBytes(buffer));
                    marketOrderSubject.OnNext(MarketOrder.FromBytes(buffer));
                }
                else
                {
                    Console.WriteLine("Warning: zmq_recv failed");
                }
            }
        }
    }

    static Subject<MarketOrder> Subscriptions()
    {
        var subject = new Subject<MarketOrder>();
        //subject.Subscribe(order => Console.WriteLine("Success: [{0}] received", order));
        
        var groups = subject.GroupBy(order => order.Symbol);
        /*
        groups.Subscribe(group =>
            group.Window(TimeSpan.FromSeconds(15))
            .Subscribe(window => { 
                window.MinBy(order => order.BidPrice)
                      .Subscribe(orders => { foreach (var order in orders) Console.WriteLine("Min: " + order); });
                window.MaxBy(order => order.BidPrice)
                      .Subscribe(orders => { foreach (var order in orders) Console.WriteLine("Max: " + order); });
        }));
        */
        /*
        groups.Subscribe(group =>
            group.Window(TimeSpan.FromSeconds(15))
            .Subscribe(window => { 
                window.MinBy(order => order.BidPrice)
                      .SelectMany(orders => orders)
                      .Subscribe(order => Console.WriteLine("Min: " + order));
                window.MaxBy(order => order.BidPrice)
                      .SelectMany(orders => orders)
                      .Subscribe(order => Console.WriteLine("Max: " + order));
        }));
        */
        /*
        groups.Subscribe(group =>
            group.Window(50)
            .Subscribe(window => { 
                window.MinBy(order => order.BidPrice)
                      .SelectMany(orders => orders)
                      .Subscribe(order => Console.WriteLine("Min: " + order));
                window.MaxBy(order => order.BidPrice)
                      .SelectMany(orders => orders)
                      .Subscribe(order => Console.WriteLine("Max: " + order));
        }));
        */
        /*
        groups.Subscribe(group =>
            group.Window(50)
            .Subscribe(window => { 
                var filteredWindow = window.Where(order => order.TradeType == MarketOrder.TypeOfTrade.Day);
                filteredWindow.MinBy(order => order.BidPrice)
                              .SelectMany(orders => orders)
                              .Subscribe(order => Console.WriteLine("Min: " + order));
                filteredWindow.MaxBy(order => order.BidPrice)
                              .SelectMany(orders => orders)
                              .Subscribe(order => Console.WriteLine("Max: " + order));
        }));
        */
        
        groups.Subscribe(group =>
            group.Window(50)
            .Subscribe(window => {
                var filteredWindow = window.Where(order => order.TradeType == MarketOrder.TypeOfTrade.Day);
                filteredWindow.MinBy(order => order.BidPrice)
                              .SelectMany(orders => orders)
                              .Select(order => new { Symbol = order.Symbol, TransactTime = order.Timestamp.ToString("HH:mm:ss.ffff"), Price = order.BidPrice })
                              .Subscribe(order => Console.WriteLine("Min: {0} {1,6} {2,8:F2}", order.TransactTime, order.Symbol, order.Price));
                filteredWindow.MaxBy(order => order.BidPrice)
                              .SelectMany(orders => orders)
                              .Select(order => new { Symbol = order.Symbol, TransactTime = order.Timestamp.ToString("HH:mm:ss.ffff"), Price = order.BidPrice })
                              .Subscribe(order => Console.WriteLine("Max: {0} {1,6} {2,8:F2}", order.TransactTime, order.Symbol, order.Price));
        }));
        
        return subject;
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
                        ListenLoop(subSocket, Subscriptions());
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