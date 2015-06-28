//Sample provided by Fabio Galuppo 
//June 2015 

//compile: javac -d bin -cp .;jars/rxjava-1.0.11.jar;jars/rxjava-math-1.0.0.jar FileImporterSample.java
//run: java -cp ./bin;jars/rxjava-1.0.11.jar;jars/rxjava-math-1.0.0.jar FileImporterSample

import java.util.ArrayList;
import java.util.List;
import java.util.HashMap;
import java.util.Map;
import java.lang.Iterable;
import java.nio.file.*;
import java.util.stream.Stream;
import java.io.IOException;
import java.lang.RuntimeException;

import rx.Observable;
import rx.observables.GroupedObservable;
import rx.Subscription;
import rx.subjects.PublishSubject;
import rx.functions.*;
import rx.observables.MathObservable;
import static rx.observables.MathObservable.*;

final class FileImporterSample {    
    private final HashMap<String, ArrayList<Double>> Table = new HashMap<String, ArrayList<Double>>();
        
    private final class Pair<K, V> {
        private final K key;
        private final V value;

        public Pair(K key, V value) {
            this.key = key;
            this.value = value;
        }

        public K getKey() { return key; }

        public V getValue() { return value; }
    }

    private static void Parse(HashMap<String, ArrayList<Double>> t, String s) {
        String[] ss = s.split(" ");
        String symbol = ss[0];        
        ArrayList<Double> ds;
        if (t.containsKey(symbol)) {
            ds = t.get(symbol);
        }
        else {
            ds = new ArrayList<Double>();
            t.put(symbol, ds);
        }
        
        for (int i = 1; i < ss.length; ++i) {
            if (!ss[i].isEmpty()) {
                ds.add(Double.valueOf(ss[i]));
            }
        }
    }
    
    private final static class Item {
        private final String symbol;
        private final Iterable<Double> values;

        public Item(String symbol, Iterable<Double> values) {
            this.symbol = symbol;
            this.values = values;
        }

        public String getSymbol() { return symbol; }

        public Iterable<Double> getValues() { return values; }
    }
    
    private static Item Parse2(String s) {
        String[] ss = s.split(" ");
        String symbol = ss[0];        
        ArrayList<Double> ds = new ArrayList<Double>();
        for (int i = 1; i < ss.length; ++i) {
            if (!ss[i].isEmpty()) {
                ds.add(Double.valueOf(ss[i]));
            }
        }
        return new Item(symbol, ds);
    }
    
    private void _run() {        
        PublishSubject<String> pubSubject = PublishSubject.create();
        
        Action1<String> displayLine = line -> { 
            //System.out.println(line); 
        };        
        Subscription sub4 = pubSubject.subscribe(displayLine);
        
        //////////
        
        Action1<String> onNext = line -> { 
            Parse(Table, line); 
        };
        
        Action1<Throwable> onError = t -> {
            t.printStackTrace();
        };

        Action0 onCompleted = () -> {
            for (Map.Entry<String, ArrayList<Double>> entry : Table.entrySet()) {
                System.out.println(entry.getKey() + ":");
                for (Double d : entry.getValue()) {
                    System.out.format("%20.13f%n", d);
                }
            } 
        };
        
        Subscription sub3 = pubSubject.filter(line -> { return line.startsWith("ABC"); })
                                      .subscribe(onNext, onError, onCompleted);
                                      
        //////////
        
        Action1<GroupedObservable<String, Item>> groupAction = group -> {
            Observable<Double> values = group.flatMap(item -> Observable.from(item.getValues()));            
            Observable<String> key = Observable.just(group.getKey());
            
            Observable<Double> aggregation = averageDouble(values);
            //Observable<Double> aggregation = min(values);
            //Observable<Double> aggregation = max(values);
            Observable.zip(key, aggregation, (k, v) -> new Pair<String, Double>(k, v))
                      .subscribe(x -> { 
                            System.out.println(x.getKey()); 
                            System.out.format("%20.13f%n", x.getValue());
                      });
        };
        
        Subscription sub2 = pubSubject.map(line -> { return Parse2(line); })
                                      .groupBy(item -> { return item.getSymbol(); })
                                      .subscribe(groupAction);
                                      
        //////////
        
        Observable<GroupedObservable<String, Item>> g = pubSubject.map(line -> { return Parse2(line); })
                                                                  .groupBy(item -> { return item.getSymbol(); });           
                                  
        Func1<Func1<Observable<Double>, Observable<Double>>, Func1<GroupedObservable<String, Item>, Observable<Pair<String, Double>>>> 
                aggregateFunc = aggregator -> { 
                                    return group -> { 
                                        Observable<String> key = Observable.just(group.getKey());
                                        Observable<Double> values = group.flatMap(item -> Observable.from(item.getValues()));
                                        return Observable.zip(key, aggregator.call(values), (k, v) -> new Pair<String, Double>(k, v));
                                    }; };  
                                  
        Observable<Pair<String, Double>> maxStream = g.flatMap(aggregateFunc.call(v -> max(v)));
        Observable<Pair<String, Double>> minStream = g.flatMap(aggregateFunc.call(v -> min(v)));
        Observable<Pair<String, Double[]>> minMaxStream = Observable.zip(minStream, maxStream, (a, b) -> {
            return new Pair(a.getKey(), new Double[]{ a.getValue(), b.getValue() });
        });
           
        Subscription sub = minMaxStream.subscribe(x -> {
                                            System.out.println(x.getKey()); 
                                            for (Double d : x.getValue())
                                                System.out.format("%20.13f ", d);
                                            System.out.println();
                                        });        
        
        /*   
        Action1<Pair<String, Double>> display = x -> {
                                        System.out.println(x.getKey()); 
                                        System.out.format("%20.13f%n", x.getValue());
                                      };
                                     
        //Subscription sub = maxStream.subscribe(display);
        Subscription sub = minStream.subscribe(display);
        */
        
        //////////                                        
           
        Path path = Paths.get("..\\..\\rx\\net\\ExportedFile.txt");        
        try {
        
            //Stream<String> lines = Files.lines(path);
            //lines.forEach(line -> { System.out.println(line); });
            
            Stream<String> lines = Files.lines(path);
            lines.forEach(line -> { pubSubject.onNext(line); });
            pubSubject.onCompleted();
        
        } catch (IOException e) {
            e.printStackTrace();
        }
        
        sub4.unsubscribe();
        sub3.unsubscribe();
        sub2.unsubscribe();
        sub.unsubscribe();
    }
    
    public static void main(String[] args) {
        new FileImporterSample()._run();
    }
}