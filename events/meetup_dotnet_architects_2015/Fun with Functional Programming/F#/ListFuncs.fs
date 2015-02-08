//Sample provided by Fabio Galuppo 
//February 2015 

module ListFuncsProgram

open System

let xs = [1; 1; 2; 3; 5; 8; 13]
let ys = [1..10]
let zs = [for i in 1..10 -> i * i]
let ws = [for (i, j) in (List.zip [1..10] [2.. 2 ..20]) -> i * j]

let head (xs : seq<'a>) : 'a = Seq.head xs

let tail (xs : seq<'a>) : seq<'a> = Seq.skip 1 xs

let init (xs : seq<'a>) : seq<'a> = Seq.take (Math.Max(0, Seq.length xs - 1)) xs

let last (xs : seq<'a>) : 'a = Seq.last xs

let test1() = 
    printfn "xs: %A" xs
    printfn "head of xs: %d" (List.head xs)
    printfn "head of xs: %d" (xs |> List.head)
    printfn "tail of xs: %A" (List.tail xs)
    printfn "tail of xs: %A" (xs |> List.tail)
    printfn "tail of xs: %A" (xs |> Seq.skip 1 |> Seq.toList)
    printfn "last of xs: %d" (xs |> Seq.last)
    printfn "init of xs: %A" (xs |> Seq.take (Math.Max(0, List.length xs - 1)) |> Seq.toList)

let test2() = 
    printfn "xs: %A" xs
    printfn "head of xs: %d" (head xs)
    printfn "tail of xs: %A" (tail xs)
    printfn "last of xs: %d" (last xs)
    printfn "init of xs: %A" (init xs)


