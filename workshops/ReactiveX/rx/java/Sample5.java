//Sample provided by Fabio Galuppo 
//June 2015 

//compile: javac -d bin -cp .;jars/rxjava-1.0.11.jar Sample5.java
//run: java -cp ./bin;jars/rxjava-1.0.11.jar Sample5

import rx.Observable;
import rx.functions.Action1;
import rx.functions.Func0;
import rx.functions.Func1;
import rx.observables.ConnectableObservable;
import rx.subjects.PublishSubject;

import java.util.Comparator;
import java.util.HashMap;
import java.util.Random;
import java.util.concurrent.TimeUnit;
import java.util.List;

final class Sample5 {
    private class StockInfo {
        private final String symbol;
        private final Float value;

        public StockInfo(String symbol, Float value) {
            this.symbol = symbol;
            this.value = value;
        }

        public String getSymbol() { return symbol; }

        public Float getValue() { return value; }
    }

    private class TimedStream<T> implements AutoCloseable {
        private final PublishSubject<T> thisSubject;
        private final long interval;
        private final Func0<T> thisGetData;
        private final Object thisLock;

        private long lastTime;
        private Thread timerThread;

        public TimedStream(long intervalMillis, Func0<T> getData) {
            thisSubject = PublishSubject.create();
            interval = intervalMillis;
            thisGetData = getData;
            thisLock = new Object();
            start();
        }

        private void start() {
            synchronized (thisLock) {
                lastTime = System.currentTimeMillis();
                timerThread = new Thread(() -> {
                    try {
                        while (!Thread.interrupted()) {
                            Thread.yield();

                            long currentTick = System.currentTimeMillis();
                            if (currentTick - lastTime > interval) {
                                lastTime = currentTick;
                                thisSubject.onNext(thisGetData.call());
                            }
                        }
                    }
                    catch (RuntimeException t) {
                        thisSubject.onError(t);
                    }

                    thisSubject.onCompleted();

                }, "TimerThread");
                timerThread.start();
            }
        }

        public Observable<T> asObservable() {
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

    private StockInfo rndUpdate(HashMap<String, Float> dic, Random rnd, String key) {
        Float x = dic.get(key);
        Float delta = rnd.nextFloat();
        x += (0.001f * (0 == rnd.nextInt(2) ? delta : -delta));
        dic.replace(key, x);
        return new StockInfo(key, x);
    }

    private void _run() {
        final HashMap<String, Float> dic = new HashMap<String, Float>();
        dic.put("ABC", 10.f);
        dic.put("DEF", 100.f);
        dic.put("GHI", 8.75f);
        dic.put("JKL", 15.5f);

        final Random rnd = new Random();
        Func0<StockInfo> f = () -> {
            switch (rnd.nextInt(4))
            {
                case 0:
                    return rndUpdate(dic, rnd, "ABC");
                case 1:
                   return rndUpdate(dic, rnd, "DEF");
                case 2:
                    return rndUpdate(dic, rnd, "GHI");
                default:
                    break;
            }
            return rndUpdate(dic, rnd, "JKL");
        };

        Func1<String, Action1<StockInfo>> onNext = (s) -> {
            return (stockInfo) -> {
                String symbol = stockInfo.getSymbol();
                Float value = stockInfo.getValue();
                System.out.printf("[%s] [%s] {Symbol: %s; Value: %f} %n", getCurrentThreadName(), s, symbol, value);
            };
        };

        Func1<String, Action1<List<StockInfo>>> onNextRange = (s) -> {
            Comparator<StockInfo> comparator = (lhs, rhs) -> {
                    if (lhs.getValue() < rhs.getValue()) return -1;
                    if (lhs.getValue() > rhs.getValue()) return  1;
                    return 0;
            };

            return (xs) -> {
                StockInfo min = xs.stream().min(comparator).get();
                StockInfo max = xs.stream().max(comparator).get();
                String minSymbol = min.getSymbol();
                Float minValue = min.getValue();
                String maxSymbol = max.getSymbol();
                Float maxValue = max.getValue();
                System.out.printf("[%s] [%s] {Min: {Symbol: %s; Value: %f} Max: {Symbol: %s; Value: %f}} %n",
                        getCurrentThreadName(), s, minSymbol, minValue, maxSymbol, maxValue);
            };
        };

        try(TimedStream<StockInfo> ts = new TimedStream<>(300, f)) {

            ConnectableObservable<StockInfo> connectableObservable = ts.asObservable().publish();

            connectableObservable.subscribe(onNext.call("Subscriber #1"));

            connectableObservable
                    .filter((stockInfo) -> stockInfo.getSymbol() == "ABC")
                    .buffer(5, TimeUnit.SECONDS)
                    .filter((xs) -> xs.size() > 0)
                    .subscribe(onNextRange.call("Subscriber #2"));

            connectableObservable.connect();

            System.in.read();

        }
        catch (Exception e) {
            e.printStackTrace();
        }

    }

    public static void main(String[] args) {
        new Sample5()._run();
    }
}
