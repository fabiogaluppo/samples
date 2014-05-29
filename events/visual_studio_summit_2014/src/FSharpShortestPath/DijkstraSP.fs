//Sample provided by Fabio Galuppo
//April 2014

//Adaptation of http://algs4.cs.princeton.edu/44sp/DijkstraSP.java.html

namespace Algorithms

open System
open System.Collections.Generic

type DijkstraSP(G:Collections.EdgeWeightedDigraph, s:int) as this =
    let mutable distTo:double array = null
    let mutable edgeTo:Collections.DirectedEdge array = null
    let mutable pq:Collections.IndexMinPQ<double> = Unchecked.defaultof<Collections.IndexMinPQ<double>>
    do
        G.edges() |> Seq.iter (fun e -> if e.Weight() < 0. then 
                                            raise (ArgumentException("edge " + e.ToString() + " has negative weight")))
        distTo <- Array.init<double> (G.CountOfV()) (fun _ -> Double.PositiveInfinity)
        edgeTo <- Array.zeroCreate<Collections.DirectedEdge> (G.CountOfV())
        distTo.[s] <- 0.

        pq <- new Collections.IndexMinPQ<double>(G.CountOfV())
        pq.insert(s, distTo.[s])
        while not (pq.isEmpty()) do
            let v = pq.delMin()
            G.adj(v) |> Seq.iter (fun e -> this.relax(e))

        //assert this.check(G, s)

    member private this.relax (e:Collections.DirectedEdge) =
        let v = e.From()
        let w = e.To()
        if distTo.[w] > distTo.[v] + e.Weight() then
            distTo.[w] <- distTo.[v] + e.Weight()
            edgeTo.[w] <- e
            if pq.contains(w) then pq.decreaseKey(w, distTo.[w])
            else pq.insert(w, distTo.[w])

    member this.DistTo(v:int) = distTo.[v]

    member this.hasPathTo(v:int) = distTo.[v] < Double.PositiveInfinity

    member this.pathTo(v:int) : IEnumerable<Collections.DirectedEdge> =
        if this.hasPathTo(v) then
            let path = new Stack<Collections.DirectedEdge>()
            let mutable e = edgeTo.[v]
            while e <> Unchecked.defaultof<Collections.DirectedEdge> do
                path.Push (e)
                e <- edgeTo.[e.From()]
            upcast path
        else
            null

    member private this.check(G:Collections.EdgeWeightedDigraph, s:int) = (* TODO *) true

