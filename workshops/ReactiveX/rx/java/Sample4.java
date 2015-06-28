//Sample provided by Fabio Galuppo 
//June 2015 

//compile: javac -d bin -cp .;jars/rxjava-1.0.11.jar Sample4.java
//run: java -cp ./bin;jars/rxjava-1.0.11.jar Sample4

import rx.Observable;
import rx.functions.Action1;
import rx.functions.Func1;
import rx.observables.ConnectableObservable;
import rx.schedulers.Schedulers;
import rx.subjects.PublishSubject;

import java.util.concurrent.TimeUnit;

final class Sample4 {

    private class Timer implements AutoCloseable {
        private final PublishSubject<Long> thisSubject;
        private final long interval;
        private final Object thisLock;

        private long lastTime;
        private Thread timerThread;

        public Timer(long intervalMillis) {
            thisSubject = PublishSubject.create();
            interval = intervalMillis;
            thisLock = new Object();
            start();
        }

        private void start() {
            synchronized (thisLock) {
                lastTime = System.currentTimeMillis();
                timerThread = new Thread(() -> {
                    try {
                        while (!Thread.interrupted()) {
                            sleep(1);

                            long currentTick = System.currentTimeMillis();
                            if (currentTick - lastTime > interval) {
                                lastTime = currentTick;
                                thisSubject.onNext(lastTime);
                            }
                        }
                    } catch (InterruptedException e) {
                    } catch (RuntimeException t) {
                        thisSubject.onError(t);
                    }

                    thisSubject.onCompleted();

                }, "TimerThread");
                timerThread.start();
            }
        }

        public Observable<Long> asObservable() {
            return thisSubject;
        }

        @Override
        public void close() throws Exception {
            synchronized (thisLock) {
                if (timerThread != null) {
                    timerThread.interrupt();
                    try {
                        timerThread.join();
                    } catch (InterruptedException ex) {
                    }
                    timerThread = null;
                }
            }
        }
    }

    private static String getCurrentThreadName() {
        return Thread.currentThread().getName();
    }

    private static void sleep(long millis) throws InterruptedException {
        Thread.sleep(millis);
    }

    private void _run() {

        // Create a ticker that goes off every 1/2 second.
        try(Timer timer = new Timer(500)) {
            ConnectableObservable<Long> connectableObservable = timer
                    .asObservable()
                    .publish();

            Func1<String, Action1<Long>> onNext = (s) -> {
                return (t) -> {
                    System.out.printf("[%s] [%s] ticks: %d %n", getCurrentThreadName(), s, t);
                };
            };

            connectableObservable
                    .subscribe(onNext.call("Subscriber #1"));

            connectableObservable
                    .observeOn(Schedulers.newThread())
                    .sample(1000, TimeUnit.MILLISECONDS)
                    .subscribe(onNext.call("Subscriber #2"));

            connectableObservable.connect();

            sleep(10000);

        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public static void main(String[] args) {
        new Sample4()._run();
    }

}
