//Sample provided by Fabio Galuppo  
//March 2016
//http://www.fssnip.net/7OM 

open Microsoft.FSharp.Core 
open System

//Microsoft.FSharp.Core.OptimizedClosures
//https://msdn.microsoft.com/en-us/library/ee340450.aspx

//https://github.com/fsharp/fsharp/blob/master/src/fsharp/FSharp.Core/array.fs#L25
let inline checkNonNull argName arg =  
    match box arg with  
    | null -> nullArg argName  
    | _ -> ()

let inline checkArrayNonZeroLen argName arg =
    match (Array.length arg) with
    | 0 -> failwith (argName + " can't be zero length")
    | _ -> ()

//https://github.com/fsharp/fsharp/blob/master/src/fsharp/FSharp.Core/array.fs#L698
[<CompiledName("MyFold")>] 
let myfold<'T,'State> (f : 'State -> 'T -> 'State) (acc: 'State) (array:'T[]) = 
    checkNonNull "array" array 
    let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f) 
    let mutable state = acc  
    let len = array.Length 
    for i = 0 to len - 1 do  
        state <- f.Invoke(state,array.[i]) 
    state 

let xs = [|10; 3; 17; 8; 2; 5; 1; 20; 9|]

printfn "min(%A) = %d" xs (xs |> myfold (fun acc x -> Math.Min(acc, x)) Int32.MaxValue)
printfn "sum(%A) = %d" xs (xs |> myfold (fun acc x -> acc + x) 0)

[<CompiledName("MyFoldAssociative")>]
//f : SemigroupOperation
let myfoldAssociative<'T> (f : 'T -> 'T -> 'T) (array:'T[]) = 
    checkNonNull "array" array
    checkArrayNonZeroLen "array" array
    let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)
    let THRESHOLD = 2 //for demonstration purpose    
    let rec myfoldAssociativeRec (array2:'T[]) =
        let n = Array.length array2
        if (n >= THRESHOLD) then
            let lhs, rhs = array2 |> Array.splitAt (n / 2)
            //divide
            let a = myfoldAssociativeRec lhs //potential parallelism here (*)
            let b = myfoldAssociativeRec rhs //(*)
            //combine
            f.Invoke(a, b)
        else
            let mutable acc = array2.[0]
            for i = 1 to n - 1 do  
                acc <- f.Invoke(acc,array2.[i]) 
            acc
    myfoldAssociativeRec array

printfn "min(%A) = %d" xs (xs |> myfoldAssociative (fun x y -> Math.Min(x, y)))
printfn "sum(%A) = %d" xs (xs |> myfoldAssociative (fun acc x -> acc + x))

(*
    min([|10; 3; 17; 8; 2; 5; 1; 20; 9|]) = 1
    sum([|10; 3; 17; 8; 2; 5; 1; 20; 9|]) = 75
    min([|10; 3; 17; 8; 2; 5; 1; 20; 9|]) = 1
    sum([|10; 3; 17; 8; 2; 5; 1; 20; 9|]) = 75
*)