//Sample provided by Fabio Galuppo 
//May 2016 

open System

type Rnd = { rnd : Random }

let createRnd () = { rnd = new Random() }
let createRndWithSeed (seed) = { rnd = new Random(seed) }

let inline precondition cond dueTo = 
    if (not cond) then 
        failwith ("precondition violation: " + dueTo)

//[0.0, 1.0)
let uniform (r : Rnd) = r.rnd.NextDouble()
//[0, n)
let uniformInt (r : Rnd) n = 
    precondition (n > 0) "less or equal than 0"
    r.rnd.Next(n)
//[n, m)
let uniformRange (r : Rnd) n m = 
    precondition (m > n) "invalid range"
    n + uniform r * (m - n)
//[n, m)
let uniformIntRange (r : Rnd) n m = 
    precondition (m > n) "invalid range"
    precondition (int64 (m - n) < int64 Int32.MaxValue) "invalid range"
    n + uniformInt r (m - n)

let gaussian (r : Rnd) mu (* mean *) sigma (* std dev *) =
    //Polar form of the Box-Muller transform
    let gaussian1 ()  =
        let rec loop x h =
            if (h >= 1. || h = 0.) then
                let x = uniformRange r -1. 1.
                let y = uniformRange r -1. 1.
                let h = x * x + y * y
                loop x h
            else
                x * Math.Sqrt(-2. * Math.Log(h) / h)
        loop 0. 0.
    mu + sigma * gaussian1 ()

let normalToInt x =
    //gaussian r1 0. 1. == ~[-3, 3]
    match x with
    | _ when x < -1.5              -> 1
    | _ when x >= -1.5 &&  x < 0.  -> 2
    | _ when x >=  0.  &&  x < 1.5 -> 3
    | _ (* when x >= 1.5 *)        -> 4

//let r1 = createRndWithSeed 100602069
//some display tests
(*
for i = 1 to 200 do
    //gaussian r1 9. 0.2 |> printfn "%f"
    //gaussian r1 0. 1. |> printfn "%f"
    gaussian r1 0. 1. |> normalToInt |> printfn "%d"
printfn "----------"    
for i = 1 to 20 do
    uniformIntRange r1 1 5 |> printfn "%d"
*)

let now = DateTime.Now
let r1 = createRndWithSeed (now.Millisecond + now.Second * 1000)
let summarize xs = 
    //xs |> List.countBy (fun x -> x) |> List.sortBy (fun x -> fst x)
    xs |> List.countBy id
       |> List.sortBy (fun x -> fst x)

[for i = 1 to 250 do yield gaussian r1 0. 1.] |> List.map normalToInt |> summarize |> printfn "gaussian: %A"
printfn "----------"
[for i = 1 to 250 do yield uniformIntRange r1 1 5] |> summarize |> printfn "uniform : %A"