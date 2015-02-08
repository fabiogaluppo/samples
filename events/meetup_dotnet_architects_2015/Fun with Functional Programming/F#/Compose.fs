//Sample provided by Fabio Galuppo 
//February 2015 

module ComposeProgram

open System

let compose = fun g f -> fun x -> g (f x)

let f x = 2 * x
let g x = x + 1
let h0 = compose g f
let h1 = compose f g
let h2 = g << f
let h3 = g >> f

let lg x = Math.Log(x, 2.)
let powerof2 x = 2. ** x

let test1 () =
    //printfn "%d" (h0 1)
    //printfn "%d" (h0 2)
    //printfn "%d" (h1 1)
    //printfn "%d" (h1 2)
    //printfn "%d" (h2 1)
    //printfn "%d" (h2 2)
    //printfn "%d" (h3 1)
    //printfn "%d" (h3 2)
    //or
    [h0; h1; h2; h3] 
      |> List.collect (fun f -> [for i in [1; 2] -> (f, i)]) 
      |> List.iter (fun x -> printfn "%d" (fst x (snd x)))

    printfn "%f" (lg 8.)
    printfn "%f" (powerof2 3.)

    printfn "%f" (((lg) << (powerof2)) 3.)
    printfn "%f" (((lg) << (powerof2)) 8.)
    printfn "%f" (((lg) >> (powerof2)) 3.)
    printfn "%f" (((lg) >> (powerof2)) 8.)
