//Sample provided by Fabio Galuppo 
//February 2015 

module FoldProgram

let xs = [1..10]

let rec sum acc xs =
    match xs with
    | [] -> acc
    | x :: ys -> sum (acc + x) ys

let rec fold (f : 'a -> 'b -> 'a) (acc : 'a) (xs : 'b list) : 'a =
    match xs with
    | [] -> acc
    | x :: ys -> fold f (f acc x) ys        

let a = xs |> fold (fun acc x -> acc + x) 0
let b = xs |> List.fold (fun acc x -> acc + x) 0
let c = xs |> List.sum
let d = List.foldBack (fun x acc -> x + acc) xs 0

let i = List.reduce     (fun acc x -> acc + x) xs
let j = List.reduceBack (fun x acc -> x + acc) xs

let concatLeft  (xs : string list) = List.fold     (fun acc x -> acc + x) "" xs
let concatRight (xs : string list) = List.foldBack (fun x acc -> x + acc) xs ""

let ys = ["Hello"; " "; "World"]

let test1 () =
    printfn "%d" a
    printfn "%b" (a = (sum 0 xs))
    printfn "%d" b
    printfn "%d" c
    xs |> List.fold (fun acc x -> printf "(acc: %A x: %A) " acc x; acc + x) 0 |> ignore
    printfn ""
    List.foldBack (fun x acc -> printf "(x: %A acc: %A) " acc x; x + acc) xs 0 |> ignore
    printfn ""
    printfn "%d" i
    printfn "%d" j
    printfn "%s" (concatLeft ys)
    printfn "%s" (concatRight ys)
    