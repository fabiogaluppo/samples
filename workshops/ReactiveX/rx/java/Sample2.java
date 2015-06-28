//Sample provided by Fabio Galuppo 
//June 2015 

//compile: javac -d bin -cp .;jars/rxjava-1.0.11.jar Sample2.java
//run: java -cp ./bin;jars/rxjava-1.0.11.jar Sample2

import rx.Observer;
import rx.Observable;
import rx.Subscription;
import rx.schedulers.Schedulers;

import java.util.concurrent.CountDownLatch;

final class Sample2 {

    private static String getCurrentThreadName() {
        return Thread.currentThread().getName();
    }

    private static CountDownLatch latch = new CountDownLatch(2);

    private class MySubscriber implements Observer<Integer> {

        @Override
        public void onCompleted() {
            System.out.printf("[%s] onCompleted%n", getCurrentThreadName());
            latch.countDown();
        }

        @Override
        public void onError(Throwable t) {
            t.printStackTrace();
        }

        @Override
        public void onNext(Integer i) {
            System.out.printf("[%s] onNext: %d %n", getCurrentThreadName(), i);
        }
    }

    private void _run() {
        //Using Schedulers:
        final int COUNT = 1000;
        Observable<Integer> observable = Observable.range(1, COUNT);

        System.out.printf("[%s] run%n", getCurrentThreadName());

        Subscription sub0 = observable
                //.observeOn(Schedulers.computation())
                .subscribe(new Sample2.MySubscriber());

        Subscription sub1 = observable
                //.observeOn(Schedulers.computation())
                .subscribe(
                        //onNext
                        (i) -> {
                            System.out.printf("[%s] onNext: %d %n", getCurrentThreadName(), i);
                        },
                        // onError
                        (t) -> {
                            t.printStackTrace();
                        },
                        // onCompleted
                        () -> {
                            System.out.printf("[%s] onCompleted%n", getCurrentThreadName());
                            latch.countDown();
                        }
                );

        try { latch.await(); }
        catch (InterruptedException e) { e.printStackTrace(); }

        sub1.unsubscribe();
        sub0.unsubscribe();
    }

    public static void main(String[] args) {
        new Sample2()._run();
    }

}
