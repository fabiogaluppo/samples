open System

//Sample provided by Fabio Galuppo

[<Sealed>] type X() = member this.M(value : String) = value.ToUpper()

[<Sealed>] type Y() = member this.M(value : String) = new String(Array.rev(value.ToCharArray()))

let inline selector (id : ^T) (value : String) = (^T : (member M : String -> String) (id, value))

let main = //Static Duck Typing
    (fun v -> printfn "%s -> %s" v (selector (X()) v)) "Hello"
    (fun v -> printfn "%s -> %s" v (selector (Y()) v)) "World"

    Console.ReadLine() |> ignore