//Sample provided by Fabio Galuppo
//April 2014

//Adaptation of http://algs4.cs.princeton.edu/44sp/EdgeWeightedDigraph.java.html

namespace Collections

open System
open System.Collections.Generic

type EdgeWeightedDigraph(V:int) =
    let mutable V = V
    let mutable E = 0
    let mutable adj_:Bag<DirectedEdge> array = null
    do
        if V < 0 then raise (ArgumentException("Number of vertices in a Digraph must be nonnegative"))
        adj_ <- Array.init<Bag<DirectedEdge>> V (fun _ -> new Bag<DirectedEdge> ())

    new(V:int, E:int) as this = 
        EdgeWeightedDigraph(V) then
            if E < 0 then raise (ArgumentException("Number of edges in a Digraph must be nonnegative"))
            let rnd = new Random()
            for i = 0 to E - 1 do
                let v = int (rnd.NextDouble() * double V)
                let w = int (rnd.NextDouble() * double V)
                let weight = Math.Round(100. * rnd.NextDouble()) / 100.
                let e = new DirectedEdge(v, w, weight)
                this.addEdge(e)

    new(G:EdgeWeightedDigraph) as this = 
        EdgeWeightedDigraph(int (G.CountOfV())) then
            this.SetE(G.CountOfE())
            for v = 0 to G.CountOfV() - 1 do
                let reverse = new Stack<DirectedEdge>()
                for e in G.adj(v) do reverse.Push(e)
                for e in reverse do this.Adj(v).add(e)

    member private this.SetE(e:int) = E <- e

    member this.CountOfV() = V

    member this.CountOfE() = E

    member this.addEdge (e:DirectedEdge) =
        let v = e.From()
        adj_.[v].add(e)
        E <- E + 1

    member private this.Adj(i:int) : Bag<DirectedEdge> = adj_.[i]

    member this.adj(v:int) : IEnumerable<DirectedEdge> = 
        if v < 0 || v >= V then raise (IndexOutOfRangeException(String.Format("vertex {0} is not between 0 and {1}", v, V - 1)))
        upcast adj_.[v]

    member this.edges() : IEnumerable<DirectedEdge> =
        let list = new Bag<DirectedEdge>()
        for v = 0 to V - 1 do adj_.[v] |> Seq.iter (fun e -> list.add (e))
        upcast list

    member this.outdegree(v:int) =
        if v < 0 || v >= V then raise (IndexOutOfRangeException(String.Format("vertex {0} is not between 0 and {1}", v, V - 1)))
        adj_.[v].size()

    override this.ToString () =
        let sb = new System.Text.StringBuilder()
        sb.Append(V).Append(" ").Append(E).AppendLine() |> ignore
        for v = 0 to V - 1 do
            sb.Append(v).Append(": ") |> ignore
            adj_.[v] |> Seq.iter (fun e -> sb.Append(e).Append(" ") |> ignore)
            sb.AppendLine() |> ignore
        sb.ToString()