//Sample provided by Fabio Galuppo
//April 2014

//Adaptation of http://algs4.cs.princeton.edu/44sp/IndexMinPQ.java.html

namespace Collections

open System
open System.Collections
open System.Collections.Generic

type IndexMinPQ<'T when 'T : comparison>(NMAX : int) =
    let mutable N = 0
    let mutable keys:'T array = null
    let mutable pq:int array = null
    let mutable qp:int array = null
    do
        if NMAX < 0 then raise (ArgumentException("NMAX"))
        keys <- Array.zeroCreate<'T> (NMAX + 1)
        pq <- Array.zeroCreate<int> (NMAX + 1)
        qp <- Array.create<int> (NMAX + 1) -1
    
    member this.isEmpty () = N = 0

    member this.contains (i:int) =
        if i < 0 || i >= NMAX then raise (IndexOutOfRangeException())
        qp.[i] <> -1

    member this.size () = N

    member this.insert (i:int, key:'T) =
        if i < 0 || i >= NMAX then raise (IndexOutOfRangeException())
        if this.contains(i) then raise (ArgumentException("index is already in the priority queue"))
        N <- N + 1
        qp.[i] <- N        
        pq.[N] <- i
        keys.[i] <- key
        this.swin (N)

    member this.minIndex() =
        if N = 0 then raise(InvalidOperationException("Priority queue underflow"))
        pq.[1]

    member this.minKey() =
        if N = 0 then raise(InvalidOperationException("Priority queue underflow"))
        keys.[pq.[1]]

    member this.delMin() =
        if N = 0 then raise(InvalidOperationException("Priority queue underflow"))
        let min = pq.[1]
        this.exch (1, N); N <- N - 1
        this.sink (1)
        qp.[min] <- -1
        keys.[pq.[N + 1]] <- Unchecked.defaultof<'T>
        pq.[N + 1] <- -1
        min

    member this.keyOf (i:int) =
        if i < 0 || i >= NMAX then raise (IndexOutOfRangeException())
        if not (this.contains (i)) then raise (ArgumentException("index is not in the priority queue"))
        keys.[i]

    member this.changeKey (i:int, key: 'T) =
        if i < 0 || i >= NMAX then raise (IndexOutOfRangeException())
        if not (this.contains (i)) then raise (ArgumentException("index is not in the priority queue"))
        keys.[i] <- key
        this.swin (qp.[i])
        this.sink (qp.[i])

    member this.decreaseKey (i:int, key: 'T) =
        if i < 0 || i >= NMAX then raise (IndexOutOfRangeException())
        if not (this.contains (i)) then raise (ArgumentException("index is not in the priority queue"))
        if (compare keys.[i] key) <= 0 then raise (ArgumentOutOfRangeException("Calling decreaseKey() with given argument would not strictly decrease the key"))
        keys.[i] <- key
        this.swin (qp.[i])

    member this.increaseKey (i:int, key: 'T) =
        if i < 0 || i >= NMAX then raise (IndexOutOfRangeException())
        if not (this.contains (i)) then raise (ArgumentException("index is not in the priority queue"))
        if (compare keys.[i] key) >= 0 then raise (ArgumentOutOfRangeException("Calling increaseKey() with given argument would not strictly increase the key"))
        keys.[i] <- key
        this.sink (qp.[i])

    member this.delete (i:int) =
        if i < 0 || i >= NMAX then raise (IndexOutOfRangeException())
        if not (this.contains (i)) then raise (ArgumentException("index is not in the priority queue"))
        let index = qp.[i]
        this.exch (index, N); N <- N - 1
        this.swin (index)
        this.sink (index)
        keys.[i] <- Unchecked.defaultof<'T>
        qp.[i] <- -1

    ////
    // General helper functions
    ////
    member private this.greater (i:int, j:int) =
        (compare keys.[pq.[i]] keys.[pq.[j]]) > 0

    member private this.exch (i:int, j:int) =
        let swap = pq.[i] 
        pq.[i] <- pq.[j]; pq.[j] <- swap
        qp.[pq.[i]] <- i; qp.[pq.[j]] <- j

    ////
    // Heap helper functions
    ////
    member private this.swin (k_:int) =
        let mutable k = k_
        while (k > 1 && this.greater (k / 2, k)) do
            this.exch (k, k / 2)
            k <- k / 2

    member private this.sink (k_:int) =
        let mutable k = k_
        let mutable canBreak = false
        while (not canBreak) && 2 * k <= N do
            let mutable j = 2 * k
            if j < N && this.greater (j, j + 1) then j <- j + 1
            if not (this.greater (k, j)) then 
                canBreak <- true
            else
                this.exch (k, j)
                k <- j

    ////
    // Enumerable interfaces
    ////
    interface IEnumerable<int> with
        member this.GetEnumerator () : IEnumerator<int> = 
            upcast new HeapEnumerator<'T> (N, pq, keys)
        member this.GetEnumerator () : IEnumerator =
            upcast (this :> IEnumerable<int>).GetEnumerator ()

and internal HeapEnumerator<'T when 'T : comparison>(N: int, pq:int array, keys:'T array) =
        let copy = new IndexMinPQ<'T>(Array.length pq - 1)
        let mutable current = Unchecked.defaultof<int>
        do
            for i = 1 to N do copy.insert (pq.[i], keys.[pq.[i]])

        interface IEnumerator<int> with
            member this.Current with get () = current

        interface IEnumerator with
            member this.Current with get () = box current
            member this.MoveNext() = 
                if not (copy.isEmpty ()) then
                    current <- copy.delMin ()
                    true
                else
                    current <- Unchecked.defaultof<int>
                    false
            member this.Reset() = raise (NotSupportedException())
        
        interface IDisposable with
            member this.Dispose () = ()
        