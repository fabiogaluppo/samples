//Sample provided by Fabio Galuppo
//April 2014

//Adaptation of http://algs4.cs.princeton.edu/44sp/DirectedEdge.java.html

namespace Collections

open System

type DirectedEdge(v:int, w:int, weight:double) =
    do
        if v < 0 then raise (ArgumentOutOfRangeException("Vertex names must be nonnegative integers"))
        if w < 0 then raise (ArgumentOutOfRangeException("Vertex names must be nonnegative integers"))
        if Double.IsNaN (weight) then raise (ArgumentException("Weight is NaN"))
    
    member this.From () = v
    member this.To () = w
    member this.Weight () = weight
    override this.ToString () = sprintf "%d->%d %5.2f" v w weight
