//Sample provided by Fabio Galuppo 
//June 2015 

//compile: csc /r:System.Core.dll /r:Microsoft.CSharp.dll /r:System.dll /r:System.Net.Http.dll /r:bin\refs\System.Reactive.Core.dll /r:bin\refs\System.Reactive.Interfaces.dll /r:bin\refs\System.Reactive.Linq.dll /r:bin\refs\System.Reactive.PlatformServices.dll /t:exe /out:bin\ReqRespAsStreamSample.exe ReqRespAsStreamSample.cs
//run: .\bin\ReqRespAsStreamSample.exe

using System;
using System.Collections.Generic;
using System.Net.Http;

using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Disposables;

sealed class ReqRespAsStreamSample {
    private static void TestPull1() {
        string baseAddress = "http://localhost:8090/";
        var client = new HttpClient();
        var response = client.GetAsync(baseAddress + "api/rnd?size=3&mean=10.0&stddev=2.0").Result;
        Console.WriteLine(response);
        Console.WriteLine(response.Content.ReadAsStringAsync().Result);       
    }
    
    sealed class EventArgs<T> : EventArgs
    {
        public readonly T Value;
        public EventArgs(T value) { Value = value; }
        public static EventArgs<T> New(T value) { return new EventArgs<T>(value); } 
    }
    
    private static event EventHandler<EventArgs<string>> RequestEvent;
    
    private static void SendHttpRequest(string url) {
        if (null != RequestEvent) 
            RequestEvent(null, EventArgs<string>.New(url));
    }
    
    private static readonly char[] ParsePattern = new char[]{'[', ',', ']'};
    
    private static IEnumerable<double> Parse(string doubles) {
        return doubles.Split(ParsePattern, System.StringSplitOptions.RemoveEmptyEntries)
                      .Select(d => Double.Parse(d, System.Globalization.CultureInfo.InvariantCulture));
    }
    
    private static void Send(int size, double mean, double stddev) {
        string baseAddress = "http://localhost:8090/";
        string path = String.Format(System.Globalization.CultureInfo.InvariantCulture, 
            "api/rnd?size={0}&mean={1:0.00}&stddev={2:0.00}", size, mean, stddev);
        SendHttpRequest(baseAddress + path);
    }
    
    private static void TestPush1() {        
        var requestStream = Observable.FromEventPattern<EventArgs<string>>(ev => RequestEvent += ev, ev => RequestEvent -= ev);
            
        IObservable<double> responseStream = requestStream
            .Select(url => {
                var client = new HttpClient();
                var response = client.GetAsync(url.EventArgs.Value).Result;            
                return Parse(response.Content.ReadAsStringAsync().Result).ToObservable();
            })
            .SelectMany(d => d);
        
        /*
          requestStream:  --1-----3------------2----------->
          responseStream: -----d-------d-d-d-------d-d----->
        */
        IDisposable subscriber1 = responseStream.Subscribe(d => {
            //Console.WriteLine("{0:0.000000000000000}", d);
        });
        
        /*
          requestStream:  --1-----3---------------2---------------------->
          responseStream: -----d1-------d2-d3-d4-------d5-d6------------->
                          vvvvvvvvvvvvvvvvvv Buffer(5) vvvvvvvvvvvvvvvvvv      
                          ----------------------------[d1,d2,d3,d4,d5]--->
        */
        IDisposable subscriber2 = responseStream.Buffer(5).Subscribe(ds => {
            //foreach(var d in ds) Console.WriteLine("{0:0.000000000000000}", d);
        });
        
        /*
          requestStream:  --1-----3---------------2---------------------->
          responseStream: -----d1-------d2-d3-d4-------d5-d6------------->
                          vvvvvvvvvvvvvvvvvv Buffer(5) vvvvvvvvvvvvvvvvvv      
                          ----------------------------[d1,d2,d3,d4,d5]--->
                          vvvvvvvvvvvvvvvvvv SelectMany vvvvvvvvvvvvvvvvv
                          -----------------------------d1-d2-d3-d4-d5---->
                          vvvvvvvvvvvvvvvvv Where(> 10) vvvvvvvvvvvvvvvvv
                          -----------------------------d1----d3----d5---->
        */
        IDisposable subscriber3 = responseStream
                                    .Buffer(5) //buffering
                                    .SelectMany(x => x) //flattening
                                    .Where(x => x >= 10.0 ) // filtering
                                    .Subscribe(d => { // observing
                                        //Console.WriteLine("{0:0.000000000000000}", d); 
                                    });
                                    
        /*
          requestStream:  --1-----3---------------2---------------------->
          responseStream: -----d1-------d2-d3-d4-------d5-d6------------->
                          vvvvvvvvvvvvvvvvv Where(> 10) vvvvvvvvvvvvvvvvv
                          -----------------------------d1----d3----d5---->
                          vvvvvvvvvvvvvvvvvv Buffer(3) vvvvvvvvvvvvvvvvvv      
                          ----------------------------[d1,d2,d3]--------->
                          vvvvvvvvvvvvvvvvvv SelectMany vvvvvvvvvvvvvvvvv
                          -----------------------------d1-d2-d3---------->
        */
        IDisposable subscriber4 = responseStream
                                    .Where(x => x >= 10.0 ) // filtering
                                    .Buffer(3) //buffering
                                    .SelectMany(x => x) //flattening
                                    .Subscribe(d => { // observing
                                        Console.WriteLine("{0:0.000000000000000}", d); 
                                    });
        
        while (true) {
            Console.Write("How many numbers to fetch? ");
            int N;
            if (Int32.TryParse(Console.ReadLine(), out N))            
                Send(N, 10, 2);    
            else break;
        };
        
        subscriber4.Dispose();
        subscriber3.Dispose();
        subscriber2.Dispose();
        subscriber1.Dispose();
    }
    
    public static void Main(string[] args) {        
        //TestPull1();
        TestPush1();
    }
}