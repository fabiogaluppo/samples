//Sample provided by Fabio Galuppo 
//June 2015 

//compile: javac -d bin -cp .;jars/rxjava-1.0.11.jar Sample1.java
//run: java -cp ./bin;jars/rxjava-1.0.11.jar Sample1

import rx.Observable;
import rx.Subscriber;
import rx.Observable.OnSubscribe;
import rx.Subscription;
import rx.functions.Action1;

final class Sample1 {
    public static void main(String[] args) {
        //Using Observable factories:
        //Observable<Integer> observable0  = Observable.just(10);
        //Observable<Integer> observable0  = Observable.range(100, 10);
        Observable<Integer> observable0  = Observable.from(new Integer[]{10, 20, 30});
        //Observable<Integer> observable0 = Observable.error(new IllegalArgumentException());

        Subscription subscriber0 = observable0.subscribe(new Action1<Integer>() {

            @Override
            public void call(Integer i) {
                System.out.println("Value = " + i);
            }

        }, new Action1<Throwable>() {

             @Override
             public void call(Throwable e) {
                 System.out.println("Exception = " + e);
             }

         });

        subscriber0.unsubscribe();

        //Observable create:
        //Publisher/Observable
        Observable<String> observable1 = Observable.create(new OnSubscribe<String>() {

            @Override
            public void call(Subscriber<? super String> observer) {
                observer.onNext("one");
                observer.onNext("two");
                observer.onNext("three");
                observer.onCompleted();
            }

        });

        //Subscriber/Observer
        Subscription subscriber1 = observable1.subscribe(new Action1<String>() {

            @Override
            public void call(String s) {
                System.out.println("Value = " + s);
            }

        });

        subscriber1.unsubscribe();
    }
}
