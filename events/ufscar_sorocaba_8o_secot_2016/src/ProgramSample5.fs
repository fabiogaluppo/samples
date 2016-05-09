//Sample provided by Fabio Galuppo 
//May 2016 

let bellman_fordSP source weightedDirectedEdges (* [(from, to, weight)] *) =
    //BELLMAN-FORD algorithm to the shortest path
    //from pseudo code in Algorithms Unlocked, Thomas H. Cormen
    let vertices = weightedDirectedEdges |> List.collect (fun (u, v, _) -> [u; v]) 
                                         |> List.distinct
    let n = vertices |> List.length
    let shortest = Array.zeroCreate<int> n
    let pred = Array.zeroCreate<int> n
    //relax procedure
    let relax u v w =
         //w == weight u v
         let dist = shortest.[u] + w
         if dist < shortest.[v] then
            shortest.[v] <- dist
            pred.[v] <- u
    let inf = System.Int32.MaxValue
    let nil = -1
    //BELLMAN-FORD algorithm
    for i = 0 to n - 1 do
        shortest.[i] <- inf
        pred.[i] <- nil
    shortest.[source] <- 0
    for i = 1 to n - 1 do
        for (u, v, w) in weightedDirectedEdges do
            relax u v w
    pred |> Array.mapi (fun i p -> (p, i, shortest.[i]))
         |> Array.filter (fun (x, _, _) -> x <> nil)
         |> Array.toList

//usage example:
let s, t, x, y, z = 0, 1, 2, 3, 4
let translate i = 
        match i with 
        | 0 -> 's' | 1 -> 't' | 2 -> 'x' | 3 -> 'y' | 4 -> 'z' 
        | _ -> failwith "invalid" 
let directedEdges =
    //u -> v : w == u, v, w
    [
        (s, t, 6); (s, y, 4);
        (t, x, 3); (t, y, 2);
        (x, z, 4);
        (y, t, 1); (y, z, 3); (y, x, 9);
        (z, x, 5); (z, s, 7)
    ]

let sp = directedEdges |> bellman_fordSP s
sp |> printfn "%A"

let f (u, v, t) = (translate u), (translate v)
sp |> List.map f |> printfn "%A"
 
let g (u, v, t) = string (translate u) + " -> " + string (translate v)
sp |> List.map g |> printfn "%A" 
sp |> List.map g |> List.fold (fun acc x -> acc + x + "\n") "" |> printfn "%s"

(*
    [(3, 1, 5); (1, 2, 8); (0, 3, 4); (3, 4, 7)]
    [('y', 't'); ('t', 'x'); ('s', 'y'); ('y', 'z')]
    ["y -> t"; "t -> x"; "s -> y"; "y -> z"]
    y -> t
    t -> x
    s -> y
    y -> z
*)