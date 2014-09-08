//N
//X Y
//...

open System
open System.IO

let isKeyValue(key : string, value : string) : bool =
    match key with
    | "DIMENSION" -> true
    | _ -> false
 
let mutable m = Map.empty<string, string>
let mutable l = List.empty<string * string>
type f_delegate = delegate of string -> (string * string) option

let node_coord (line:string) : (string * string) option = //("", "")
    let a = line.Split(' ') 
    Some(a.[1], a.[2]) 

let mutable f : f_delegate = null

let parse (line : string) : unit =
    let colon = line.IndexOf(':')
    if (colon > -1) then
        let key, value = line.Substring(0, colon).Trim(), line.Substring(colon + 1).Trim() //line.Substring(0, colon - 1), line.Substring(colon + 2)
        if (isKeyValue(key, value)) then m <- m.Add(key, value)
    else
        match line with
        | "NODE_COORD_SECTION" -> f <- new f_delegate (node_coord)
        | "EOF" -> ()
        | _ -> 
            let x = if (f <> null) then f.Invoke(line) else None
            if (Option.isSome x) then l <- [x.Value] |> List.append l   //l <- x.Value :: l

[<EntryPoint>]
let main argv = 
    let path = argv.[0]

    for line in File.ReadLines(path) do        
        parse line
    
    printfn "%s" (m |> Map.find "DIMENSION")
    l |> List.iter (fun (x, y) -> printfn "%s %s" x y)

    0 // return an integer exit code
