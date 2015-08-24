//Sample provided by Fabio Galuppo 
//August 2015

//compile: javac -d bin -cp . CountOps.java
//run: java -cp ./bin CountOps

import java.util.function.Consumer;
import java.util.Arrays;
import java.util.Random;
import java.util.Comparator;

final class CountOps {
    
    private static void run0(int N) {
        
        long count = 0;
        for (int i = 0; i < N; ++i)
            ++count;
        
        System.out.format("N = %6d count = %10d %n", N, count);
    
    }
    
    private static void run1(int N) {
        
        long count = 0;
        for (int i = 1; i < N; i = 2 * i)
            ++count;
        
        System.out.format("N = %6d count = %10d %n", N, count);
    
    }
    
    private static void run2(int N) {
        
        long count = 0;
        for (int i = 1; i * i <= N; ++i)
            ++count;
        
        System.out.format("N = %6d count = %10d %n", N, count);
    
    } 
    
    private static void run3(int N) {
        
        long count = 0;
        for (int i = 0; i < N; ++i)
            for (int j = 0; j < N; ++j)
                ++count;
        
        System.out.format("N = %6d count = %10d %n", N, count);
    
    }
    
    private static void run4(int N) {
        
        long count = 0;
        for (int i = 0; i < N; ++i)
            for (int j = 0; j < N; ++j)
                for (int k = 0; k < N; ++k)
                    ++count;
        
        System.out.format("N = %6d count = %10d %n", N, count);
    
    }
    
    private static void run5(int N) {
        
        long count = 0;
        for (int i = 1; i * i <= N; i = 2 * i)
            for (int j = 0; j < i; ++j)
                ++count;
        
        System.out.format("N = %6d count = %10d %n", N, count);
    
    }
    
    private static void run6(int N) {
        
        long count = 0;
        for (int i = 1; i * i <= N; ++i)
            for (int j = 1; j * j <= N; ++j)
                for (int k = 1; k * k <= N; ++k)
                    ++count;
        
        System.out.format("N = %6d count = %10d %n", N, count);
    
    }
    
    private static void run7(int N) {
        
        long count = 0;
        for (int i = 1; i * i <= N; ++i)
            for (int j = 1; j * j <= N; ++j)
                for (int k = 1; k * k <= N; ++k)
                    for (int l = 1; l <= 4; ++l)
                        ++count;
        
        System.out.format("N = %6d count = %10d %n", N, count);
    
    }
    
    //https://en.wikipedia.org/wiki/Fisher–Yates_shuffle
    private static long shuffle(Integer[] xs) {
        
        long count = 0;
        Random rnd = new Random(System.nanoTime());
        for (int i = xs.length - 1; i > 1; --i) {
            int j = rnd.nextInt(i + 1);
            Integer x = xs[j];
            xs[j] = xs[i];
            xs[i] = x;
            ++count;
        }
        return count;
        
    }
    
    static final class Comp implements Comparator<Integer> {
        public long count;
        
        public int compare(Integer lhs, Integer rhs) {
                ++count;
                return lhs.compareTo(rhs);
        }
    }  
    
    //2 N - 1 + lg N
    private static void run8(int N) {
        
        long count = 0;
        
        //N
        Integer[] xs = new Integer[N];
        for (int i = 0; i < N; ++i) {
            xs[i] = i + 1;
            ++count;
        }
        
        count += shuffle(xs); //N - 1
        
        Comp comp = new Comp();
        Arrays.binarySearch(xs, N, comp); //lg N
        count += comp.count;
        
        System.out.format("N = %6d count = %10d %n", N, count);
        
    }
    
    private static void exec(int[] Ns, Consumer<Integer> f, String title) {
        System.out.println(title + ":");
        for (int N : Ns) f.accept(N);
        System.out.println();
    }
    
    private static void printTable(int[] Ns, boolean displayNP) {
        System.out.format("lg N       | N^1/2      | N          | N lg N     | N^2        | N^3 %n");
        for (double N : Ns) {
            System.out.format
            (
                "%10.0f | %10.0f | %10.0f | %10.0f | %10.0f | %10.0f %n", 
                Math.log(N) / Math.log(2),
                Math.sqrt(N),
                N,
                N * (Math.log(N) / Math.log(2)),
                N * N,
                N * N * N
            );
        }
        
        if (displayNP) {        
            System.out.println();
            
            System.out.format("N        | 2^N %n");
            for (double N : Ns) {
                System.out.format("%8.0f | %.0f %n", N, Math.pow(2, N));
            }
            
            System.out.println();
            
            System.out.format("N        | N! %n");
            for (double N : Ns) {            
                double n = 1.0;
                for (int i = 2; i <= N; ++i) n *= i;
                System.out.format("%8.0f | %.0f %n", N, n);
            }
        }
        
        System.out.println("----------------------------------------------------------------------------------------------------------------------------------");
    }
    
    public static void main(String[] args) {
        
        int Ns[] = { 16, 32, 64, 128, 256, 512, 1024, 2048 }; 
        
        //printTable(Ns, true);
        printTable(Ns, false);
        
        exec(Ns, (N) -> run0(N), "N");
        exec(Ns, (N) -> run1(N), "lg N");
        exec(Ns, (N) -> run2(N), "N^(1/2)");
        exec(Ns, (N) -> run3(N), "N^2");
        exec(Ns, (N) -> run4(N), "N^3");
        exec(Ns, (N) -> run5(N), "N^(1/2) -> ~2 N^(1/2)");
        exec(Ns, (N) -> run6(N), "N^(3/2)");
        exec(Ns, (N) -> run7(N), "N^(3/2) -> ~4 N^(3/2)");
        exec(Ns, (N) -> run8(N), "2 N - 1 + lg N -> O(N) -> ~2 N");
        
    }
}
