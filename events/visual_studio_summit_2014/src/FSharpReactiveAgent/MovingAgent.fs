//Sample provided by Fabio Galuppo
//April 2014

open System

type CellState =
    | Opened
    | Blocked
    | Agent of string * int
    | VisitedByAgent of int

let EnvironmentState = 
    let N = 9
    //let N = 20
    Array2D.init<CellState> N N (fun _ _ -> CellState.Opened)
    
let DisplayEnvironment () = 
    let N = Array2D.length1 EnvironmentState

    let stringFromCellState (x) =
        match x with
        | CellState.Opened -> "_"
        | CellState.Blocked -> "H"
        | CellState.Agent (representation, _) -> representation
        //| CellState.VisitedByAgent i -> if i > 10 then  "." else string i
        | CellState.VisitedByAgent _ -> "."
   
    Console.Clear()

    EnvironmentState |> Array2D.iteri (fun i j x -> 
        let isAgent = match x with | Agent _ -> true | _ -> false
        let isWall = match x with | Blocked -> true | _ -> false
        let s = stringFromCellState (x)
        let oldColor = Console.ForegroundColor
        if isAgent then Console.ForegroundColor <- ConsoleColor.Green
        else if isWall then Console.ForegroundColor <- ConsoleColor.DarkRed
        if j = N - 1 then printfn "%s " s else printf "%s " s
        Console.ForegroundColor <- oldColor)
    printfn ""

let BuildWalls () =
    let N = Array2D.length1 EnvironmentState

    if N >= 9 then
        //EnvironmentState.[2, 2..5] <- [| CellState.Blocked; CellState.Blocked; CellState.Blocked; CellState.Blocked |]
        //or
        EnvironmentState.[2, 2..5] <- Array.create 4 CellState.Blocked

        EnvironmentState.[1..4, 7] <- Array.create 4 CellState.Blocked
        EnvironmentState.[7, 0..2] <- Array.create 3 CellState.Blocked

    if N >= 20 then
        EnvironmentState.[17, 0..3] <- Array.create 4 CellState.Blocked
        EnvironmentState.[11..19, 15] <- Array.create 9 CellState.Blocked
    
        EnvironmentState.[12, 17] <- CellState.Blocked
        EnvironmentState.[13, 18] <- CellState.Blocked
        EnvironmentState.[14, 19] <- CellState.Blocked

        EnvironmentState.[9, 9..12] <- Array.create 4 CellState.Blocked
        EnvironmentState.[14, 9..12] <- Array.create 4 CellState.Blocked
        EnvironmentState.[11..13, 9] <- Array.create 3 CellState.Blocked
        EnvironmentState.[10..12, 12] <- Array.create 3 CellState.Blocked
        //or
        //EnvironmentState.[10..13, 9] <- Array.create 4 CellState.Blocked        
        //EnvironmentState.[10..13, 12] <- Array.create 4 CellState.Blocked

type internal perception = 
      NorthIsOpened
    | EastIsOpened
    | SouthIsOpened
    | WestIsOpened
    | AgentIsBlocked

type internal action = 
      GoNorth 
    | GoEast 
    | GoSouth 
    | GoWest 
    | Stop

type MovingAgent(x, y, s:string) =
    let N = Array2D.length1 EnvironmentState
    let mutable x = x
    let mutable y = y    
    let representation = s 
    do
        EnvironmentState.[y,x] <- CellState.Agent (representation, 0)

    member this.Next() = 
        let sensors () =
            let front = if y - 1 >= 0 then EnvironmentState.[y - 1, x] else CellState.Blocked
            let right = if x + 1 < N then EnvironmentState.[y, x + 1] else CellState.Blocked
            let back = if y + 1 < N then EnvironmentState.[y + 1, x] else CellState.Blocked
            let left = if x - 1 >= 0 then EnvironmentState.[y, x - 1] else CellState.Blocked
            (front, right, back, left)
        
        let updateAgent (new_x, new_y) =
            let visited () =
                match EnvironmentState.[y,x] with
                | Agent (_, i) -> i + 1
                | VisitedByAgent i -> i
                | _ -> 0
            
            printfn "%s: y = %2d x = %2d" representation y x
            EnvironmentState.[y,x] <- VisitedByAgent (visited ())
            x <- new_x; y <- new_y
            EnvironmentState.[y,x] <- CellState.Agent (representation, visited ())

        let performAction (action) =
            match action with
            | GoNorth -> updateAgent(x, y - 1)
            | GoEast -> updateAgent(x + 1, y)
            | GoSouth -> updateAgent(x, y + 1)
            | GoWest -> updateAgent(x - 1, y)            
            | _ -> ()

        let actionFromAgent (perception) =
            match perception with
            | NorthIsOpened -> GoNorth
            | EastIsOpened -> GoEast
            | SouthIsOpened -> GoSouth
            | WestIsOpened -> GoWest
            | AgentIsBlocked -> Stop

        let perceptionFromAgent () =
            let (front, right, back, left) = sensors()
            let perception = 
                let p = if front = CellState.Opened then NorthIsOpened
                        else if right = CellState.Opened then EastIsOpened
                        else if back = CellState.Opened then SouthIsOpened
                        else if left = CellState.Opened then WestIsOpened
                        else AgentIsBlocked                
                
                if p = AgentIsBlocked then
                    let visitCount(v) = match v with | VisitedByAgent i -> i | _ -> 0
                    let a = visitCount left
                    let b = visitCount back
                    let c = visitCount right
                    let d = visitCount front                    
                    let xs = [(a, WestIsOpened); (b, SouthIsOpened); (c, EastIsOpened); (d, NorthIsOpened)]
                             |> List.filter (fun (i, _) -> 0 < i && i < 9)
                    match xs with
                    | [] -> AgentIsBlocked
                    | _ -> 
                        snd (xs |> List.sortBy (fun (i, action) -> i) |> List.head)
                else p
                
            perception
        
        let perception = perceptionFromAgent ()
        let action = actionFromAgent(perception)
        if action <> Stop then
            performAction(action) 
            true
        else
            false

[<EntryPoint>]
let main argv =    
    Console.Title <- "Moving Agent by Fabio Galuppo (fabiogaluppo.com)"
    
    BuildWalls ()
    
    let N = Array2D.length1 EnvironmentState

    let a = new MovingAgent(N / 2, N / 2, "A")
    //let b = new MovingAgent(0, 0, "B")
    //let c = new MovingAgent(0, N - 1, "C")

    let fps_to_ms n = 1000 / n

    while not (List.isEmpty ([a.Next()] |> List.filter (fun x -> x))) do
    //while not (List.isEmpty ([a.Next(); b.Next()] |> List.filter (fun x -> x))) do
    //while not (List.isEmpty ([a.Next(); b.Next(); c.Next()] |> List.filter (fun x -> x))) do
        System.Threading.Thread.Sleep (fps_to_ms 12)
        DisplayEnvironment ()

    printfn "Stopped..."
    0