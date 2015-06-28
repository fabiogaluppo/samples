//Sample provided by Fabio Galuppo 
//June 2015 

//compile: javac -d bin -cp .;jars/rxjava-1.0.11.jar Sample3.java
//run: java -cp ./bin;jars/rxjava-1.0.11.jar Sample3

import rx.Observable;
import rx.Subscription;
import rx.functions.Action0;
import rx.functions.Action1;

final class Sample3 {

    private static String getCurrentThreadName() {
        return Thread.currentThread().getName();
    }

    private static boolean isEven(Integer i) {
        return (i.intValue() & 0x1) == 0x0;
    }

    private static boolean isOdd(Integer i) { return (i.intValue() & 0x1) == 0x1; }

    private void _run() {
        //Using rx operators:
        final int COUNT = 200;
        Observable<Integer> observable = Observable.range(1, COUNT);

        Action1<Integer> onNext = (i) -> {
            System.out.printf("[%s] onNext: %d %n", getCurrentThreadName(), i);
        };

        Action1<Throwable> onError = (t) -> {
            t.printStackTrace();
        };

        Action0 onCompleted = () -> {
            System.out.printf("[%s] onCompleted%n", getCurrentThreadName());
        };

        Subscription sub0 = observable
                .filter((i) -> {
                    return isEven(i);
                })
                .subscribe(onNext, onError, onCompleted);

        Subscription sub1 = observable
                .filter((i) -> {
                    return isOdd(i);
                })
                .buffer(10)
                .map(xs -> xs.get(0))
                .subscribe(onNext, onError, onCompleted);

        sub1.unsubscribe();
        sub0.unsubscribe();
    }

    public static void main(String[] args) {
        new Sample3()._run();
    }

}
