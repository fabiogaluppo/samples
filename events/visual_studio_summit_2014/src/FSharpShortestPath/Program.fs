//Sample provided by Fabio Galuppo
//April 2014

let testIndexMinPQ () =
    let strings = [| "it"; "was"; "the"; "best"; "of"; "times"; "it"; "was"; "the"; "worst" |]
    
    let pq = new Collections.IndexMinPQ<string>(Array.length strings)
    for i = 0 to Array.length strings - 1 do
        pq.insert (i, strings.[i])
    
    while not (pq.isEmpty ()) do
        let i = pq.delMin ()
        printfn "%d %s" i strings.[i]
    printfn ""

    for i = 0 to Array.length strings - 1 do
        pq.insert (i, strings.[i])

    for i in pq do
        printfn "%d %s" i strings.[i]
        
    while not (pq.isEmpty ()) do
        pq.delMin () |> ignore

let testBag () =
    let bag = new Collections.Bag<string> ()
    let strings = [| "it"; "was"; "the"; "best"; "of"; "times"; "it"; "was"; "the"; "worst" |]
    for s in strings do bag.add (s)

    printfn "size of bag = %d" (bag.size ())

    for s in bag do printfn "%s" s

let testDirectedEdge () =
    let e = new Collections.DirectedEdge(12, 23, System.Math.PI)
    printfn "%s" (e.ToString ())

let testEdgeWeightedDigraph () =
    let G = new Collections.EdgeWeightedDigraph(8, 16)
    printfn "%s" (G.ToString ())

let testDijkstraSP (s:int) =
    let data =
        let v = 8
        let e = 15
        let xs = [| 4, 5, 0.35;
                    5, 4, 0.35;
                    4, 7, 0.37;
                    5, 7, 0.28;
                    7, 5, 0.28;
                    5, 1, 0.32;
                    0, 4, 0.38;
                    0, 2, 0.26;
                    7, 3, 0.39;
                    1, 3, 0.29;
                    2, 7, 0.34;
                    6, 2, 0.40;
                    3, 6, 0.52;
                    6, 0, 0.58;
                    6, 4, 0.93 |]
        ((v, e), xs)

    let V = fst (fst data)
    let E = snd (fst data)
    let A = snd data

    let G = Collections.EdgeWeightedDigraph(V)
    for (v, w, weight) in A do G.addEdge(new Collections.DirectedEdge(v, w, weight))

    let sp = new Algorithms.DijkstraSP(G, s)

    for t = 0 to G.CountOfV() - 1 do
        if sp.hasPathTo(t) then
            printf "%d to %d (%.2f)  " s t (sp.DistTo(t))
            for e in sp.pathTo(t) do printf "%s " (e.ToString())
            printfn ""
        else
            printfn "%d to %d         no path" s t

let plotDijkstraSP (s:int, d:int) =
    let data =
        let v = 8
        let e = 15
        let xs = [| 4, 5, 0.35;
                    5, 4, 0.35;
                    4, 7, 0.37;
                    5, 7, 0.28;
                    7, 5, 0.28;
                    5, 1, 0.32;
                    0, 4, 0.38;
                    0, 2, 0.26;
                    7, 3, 0.39;
                    1, 3, 0.29;
                    2, 7, 0.34;
                    6, 2, 0.40;
                    3, 6, 0.52;
                    6, 0, 0.58;
                    6, 4, 0.93 |]
        ((v, e), xs)

    let V = fst (fst data)
    let E = snd (fst data)
    let A = snd data
    let G = Collections.EdgeWeightedDigraph(V)
    for (v, w, weight) in A do G.addEdge(new Collections.DirectedEdge(v, w, weight))

    let sp = new Algorithms.DijkstraSP(G, s)

    let path = 
        if sp.hasPathTo(d) then
            (s :: [for e in sp.pathTo(d) -> e.To()], true)
        else ([s], false)

    let sb = new System.Text.StringBuilder()
    
    //custom highlight Mathematica plot from here: http://stackoverflow.com/questions/3897479/creating-a-graph-with-edges-of-different-colours-in-mathematica    
    sb.Append("edges = {") |> ignore
    for (v, w, _) in A do sb.Append(sprintf "%d -> %d," v w) |> ignore
    sb.Remove(sb.Length - 1, 1) |> ignore
    sb.Append("};").AppendLine() |> ignore
    
    sb.Append("path = {") |> ignore
    for x in fst path do sb.Append(sprintf "%d," x) |> ignore
    sb.Remove(sb.Length - 1, 1) |> ignore
    sb.Append("};").AppendLine() |> ignore

    sb.Append("edgesToHighlight = Partition[path, 2, 1];").AppendLine()
        .Append("edgesToHighlight = Join[edgesToHighlight, Reverse /@ edgesToHighlight];").AppendLine()
        .Append("erf[pts_, edge_, ___] := If[MemberQ[edgesToHighlight, edge], {Thick, Black, {Arrowheads[Large], Arrow[pts, 0.1]}}, {Darker[Red], {Arrowheads[Medium], Arrow[pts, 0.1]}}];").AppendLine()
        .Append("GraphPlot[edges, ").AppendFormat("PlotLabel -> \"{2} from {0} to {1}\"", s, d, if snd path then "path" else "no path").Append(", DirectedEdges -> True, VertexLabeling -> True, Frame -> True, EdgeRenderingFunction -> erf, Method -> \"SpringEmbedding\"]").AppendLine() |> ignore

    sb.ToString()

[<EntryPoint>]
let main argv = 
    System.Console.Title <- "Shortest Path with Dijkstra's Algorithm by Fabio Galuppo (fabiogaluppo.com)"
    
    //testIndexMinPQ ()
    //testBag ()
    //testDirectedEdge ()
    //testEdgeWeightedDigraph ()
    
    for i = 0 to 7 do testDijkstraSP (i); printfn ""
    
    for i = 0 to 7 do 
        for j = 0 to 7 do
            let mathematicaPlot = plotDijkstraSP (i, j)
            printfn "%s" mathematicaPlot

    0