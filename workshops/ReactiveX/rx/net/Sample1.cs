//Sample provided by Fabio Galuppo 
//June 2015 

//compile: csc /r:System.Core.dll /r:Microsoft.CSharp.dll /r:System.dll /r:bin\refs\System.Reactive.Core.dll /r:bin\refs\System.Reactive.Interfaces.dll /r:bin\refs\System.Reactive.Linq.dll /r:bin\refs\System.Reactive.PlatformServices.dll /t:exe /out:bin\Sample1.exe Sample1.cs
//run: .\bin\Sample1.exe

using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Disposables;

sealed class Sample1 {
    public static void Main(string[] args) {
        //Using Observable factories:
        //IObservable<Int32> observable0 = Observable.Return(10);
        //IObservable<Int32> observable0 = Observable.Range(100, 10);
        IObservable<Int32> observable0 = new Int32[] { 10, 20, 30 }.ToObservable();
        //IObservable<Int32> observable0 = Observable.Throw<Int32>(new ArgumentException());

        IDisposable subscriber0 = observable0.Subscribe(
            (Int32 i) => { 
                Console.WriteLine("Value = " + i); 
            },
        
            (Exception e) => {
                Console.WriteLine("Exception = " + e);
            }
        );
        
        subscriber0.Dispose();
        
        //Observable create:
        //Publisher/Observable
        IObservable<String> observable1 = Observable.Create<String>(observer => {
            observer.OnNext("one");
            observer.OnNext("two");
            observer.OnNext("three");
            observer.OnCompleted();
            return Disposable.Empty;
        });

        //Subscriber/Observer
        IDisposable subscriber1 = observable1.Subscribe(s => {
            Console.WriteLine("Value = " + s);
        });
        
        subscriber1.Dispose();
    }
}