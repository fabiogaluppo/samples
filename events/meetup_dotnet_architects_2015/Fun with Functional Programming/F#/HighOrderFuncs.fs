//Sample provided by Fabio Galuppo 
//February 2015 

module HighOrderFuncsProgram

let fibs = Seq.unfold (fun state -> 
                         let i, j = fst state, snd state
                         Some(j, (j, i + j))) (bigint 0, bigint 1)

let xs = Seq.take 100 fibs |> Seq.toList |> List.map (fun x -> string x)

let ys = [1..10] |> List.map (fun x -> 10. * float x)

let multiplyBy x y = x * y
let duplicate = multiplyBy 2
let quadruplicate = multiplyBy 4

let a = duplicate 10 
let b = quadruplicate 10
let c (f: unit -> int) = duplicate (f())
let d (f: unit -> int) = fun () -> quadruplicate (f())

let test1 () =
    printfn "%A" xs
    printfn "%A" ys
    printfn "%d" a
    printfn "%d" b
    printfn "%d" (c (fun () -> 10))
    printfn "%A" (d (fun () -> 10))
    printfn "%d" ((d (fun () -> 10))())