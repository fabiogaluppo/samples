//Sample provided by Fabio Galuppo 
//June 2015 

//compile: csc /r:System.Core.dll /r:Microsoft.CSharp.dll /r:System.dll /r:System.Net.Http.dll /r:bin\refs\System.Reactive.Core.dll /r:bin\refs\System.Reactive.Interfaces.dll /r:bin\refs\System.Reactive.Linq.dll /r:bin\refs\System.Reactive.PlatformServices.dll /t:exe /out:bin\FileExporterSample.exe FileExporterSample.cs
//run: .\bin\FileExporterSample.exe

using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;

using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;

sealed class FileExporterSample {
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
    
    private static void SetupPushBasedStreams() {
        RequestStream = Observable.FromEventPattern<EventArgs<string>>(ev => RequestEvent += ev, ev => RequestEvent -= ev);        
        ResponseStream = RequestStream
            .Select(url => {
                var client = new HttpClient();
                var response = client.GetAsync(url.EventArgs.Value).Result;            
                return Parse(response.Content.ReadAsStringAsync().Result).ToObservable();
            }).SelectMany(d => d);
        
        var readySubject = new Subject<string>();        
        ReadyStream  = (IObservable<string>) readySubject;
        ReadyStreamW = (IObserver<string>) readySubject;
    }
    
    private static IObservable<EventPattern<EventArgs<string>>> RequestStream;
    private static IObservable<double> ResponseStream;
    
    private static IObserver<string> ReadyStreamW;
    private static IObservable<string> ReadyStream;
    
    sealed class FileExporter : IObserver<string>
    {
        private StreamWriter sw;
        private int counting;
        private System.Threading.ManualResetEvent mre = new System.Threading.ManualResetEvent(false);
        private System.Diagnostics.Stopwatch watch;
        
        public void Wait() {
            mre.WaitOne();
        }
        
        public FileExporter(string path) {
            watch = System.Diagnostics.Stopwatch.StartNew();
            
            if (File.Exists(path)) File.Delete(path);
            sw = File.CreateText(path);
        }
        
        private void FreeResources() {
            mre.Set();
            sw.Dispose();
        }
        
        public void OnCompleted() {
            watch.Stop();
            var elapsed = watch.Elapsed.TotalMilliseconds;
            
            Console.WriteLine("File exported successfully with {0} line(s) in {1:N0} ms.", counting, elapsed);
            FreeResources();
        }

        public void OnError(Exception e) {
            Console.WriteLine("Error during file exporting. Reason: {0}", e);
            FreeResources();
        }
        
        public void OnNext(string s) {
            sw.WriteLine(s);
            ++counting;
        }
    }

    private static void Export(int linesCount = 30) {
        var rnd = new Random();
        var symbols = new string[] { "ABC", "DEF", "GHI", "JKL" }; 

        int itemsTotal = linesCount;
        int itemsCountByLine = 3;
        if (itemsTotal < itemsCountByLine) {
            Console.WriteLine("Sorry :-(");
            return;
        }

        int counting = itemsTotal / itemsCountByLine;
        
        SetupPushBasedStreams();
                
        var sub1 = ResponseStream
            .Buffer(itemsCountByLine)
            .Subscribe(xs => {
                
                StringBuilder sb = new StringBuilder();
                sb.Append(symbols[rnd.Next(symbols.Length)]);
                foreach (var x in xs) 
                    sb.AppendFormat(" {0,18:0.000000000000000}", x);
                
                ReadyStreamW.OnNext(sb.ToString());
                if (--counting == 0) 
                    ReadyStreamW.OnCompleted();
                    
            });
            
            //var sub2 = ReadyStream.Subscribe(s => Console.WriteLine(s));
            
            string path = @".\ExportedFile.txt";
            var fileExporter = new FileExporter(path);                
            var sub2 = ReadyStream.Subscribe(fileExporter);
            
            Send(itemsTotal, 10, 2);
            
            fileExporter.Wait();
            
            sub2.Dispose();
            sub1.Dispose();
    }
    
    public static void Main(string[] args) {        
        
        if (args.Length > 0) {
            int N;
            if (Int32.TryParse(args[0], out N)) {
                Export(N);
                return;        
            }
        }
        
        Export();
    }
}