//Sample provided by Fabio Galuppo 
//February 2015 

module LiftingProgram

let lift (f : 'a -> 'b) : ('a option -> 'b option) =
    fun x ->
        match x with
        | Some(value) -> Some (f(value))
        | None -> None

let xs = [1..10]
let ys = List.map (fun x -> Some x) xs

let int2float (x : int32) : float32 = float32 x

let test1() =
    let zs = xs |> List.map (int2float)
    printfn "%A" zs
    let ws = ys |> List.map (lift int2float)
    printfn "%A" ws
