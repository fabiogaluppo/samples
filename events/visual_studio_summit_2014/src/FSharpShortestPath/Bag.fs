//Sample provided by Fabio Galuppo
//April 2014

//Adaptation of http://algs4.cs.princeton.edu/13stacks/Bag.java.html

namespace Collections

open System
open System.Collections
open System.Collections.Generic

type Bag<'T>() =
    let mutable N = 0
    let mutable first : Node<'T> option = None
    
    member this.isEmpty () = Option.isNone first

    member this.size () = N

    member this.add (item:'T) =
        let oldfirst = first
        first <- Some { item = item; next = oldfirst }
        N <- N + 1
    
    ////
    // Enumerable interfaces
    ////
    interface IEnumerable<'T> with
        member this.GetEnumerator () : IEnumerator<'T> = 
            upcast new ListEnumerator<'T> (first)
        member this.GetEnumerator () : IEnumerator =
            upcast (this :> IEnumerable<'T>).GetEnumerator ()

and internal Node<'T> = { item:'T; next:Node<'T> option }

and internal ListEnumerator<'T>(first:Node<'T> option) =
    let mutable current = first
    let mutable currentValue = Unchecked.defaultof<'T>
    interface IEnumerator<'T> with
        member this.Current with get () = currentValue

    interface IEnumerator with
        member this.Current with get () = box currentValue
        member this.MoveNext() = 
            if Option.isSome current then
                let x = current.Value
                currentValue <- x.item
                current <- x.next
                true
            else
                currentValue <- Unchecked.defaultof<'T>
                current <- None
                false
        member this.Reset() = raise (NotSupportedException())
        
    interface IDisposable with
        member this.Dispose () = ()
