//Sample provided by Fabio Galuppo 
//August 2015

//compile: fsc -o:.\bin\CountOps.exe CountOps.fs
//run: .\bin\CountOps.exe

open System

let run0 (xs: int list) =    
    let N = List.length xs
    let count = ref 0
    xs |> List.map (fun x -> count := !count + 1;  x * 2) |> ignore    
    printfn "N = %6d count = %10d" N !count

let run1 (xs: int list) =
    let N = List.length xs
    let count = ref 0
    xs |> List.collect (fun i -> [for j = 1 to N do count := !count + 1; 
                                                    yield (i, j)]) |> ignore
    printfn "N = %6d count = %10d" N !count
    
let run2 (xs: int list) =
    let N = List.length xs
    let count = ref 0
    xs |> List.tryFind (fun x -> count := !count + 1; x = N + 1) |> ignore
    xs |> List.tryFind (fun x -> count := !count + 1; x = N + 2) |> ignore
    printfn "N = %6d count = %10d" N !count
    
let run3 (xs: int list) =
    let N = List.length xs
    let count = ref 0
    xs |> List.toArray |> Array.tryFind (fun x -> count := !count + 1; x = N + 1) |> ignore
    printfn "N = %6d count = %10d" N !count
   
let run4 (xs: int list) =
    let N = List.length xs
    let count = ref 0
    xs |> List.toArray |> Array.sortWith (fun x y -> count := !count + 1; x - y) |> ignore
    let M = float N
    printfn "N = %6d count = %10d [N lg N = %10d]" N !count (int (M * Math.Log(M, 2.)))
    
let run5 (xs: int list) =
    let N = List.length xs
    let count = ref 0
    let f (x) = count := !count + 1; x * 2
    let g (x) = count := !count + 1; x + 1
    let h = f >> g //g . f
    xs |> List.map (h) |> ignore
    printfn "N = %6d count = %10d [2 N = %10d]" N !count (2 * N)

let exec_ (Ns: int list) (f: int list -> unit, title: String) =
    printfn "%s:" title
    Ns |> List.iter (fun N -> f([1..N]))
    printfn ""

let Ns = [ 16; 32; 64; 128; 256; 512; 1024; 2048 ] 

let exec = exec_ Ns

exec (run0, "N")
exec (run1, "N^2")
exec (run2, "N -> ~2 N")
exec (run3, "N")
exec (run4, "N lg N -> ~1.203125 N lg N")
exec (run5, "N -> ~2 N")
