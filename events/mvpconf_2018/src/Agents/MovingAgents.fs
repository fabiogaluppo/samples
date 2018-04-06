//Sample provided by Fabio Galuppo
//March 2018

open System

type CellState =
    | Goal
    | Opened
    | Blocked
    | Agent of string * int
    | VisitedByAgent of int

let EnvironmentState = 
    let N = 9
    //let N = 20
    Array2D.init<CellState> N N (fun _ _ -> CellState.Opened)

type PQ<'a>(comparer : 'a * 'a -> bool) =
    let comp (x,y) = not (comparer(x,y))
    let heap = new System.Collections.Generic.List<'a>(16)
    let swap (i, j) = 
        let x = heap.[i]
        heap.[i] <- heap.[j]
        heap.[j] <- x
    let isEmpty () = heap.Count <= 1
    do
        heap.Add(Unchecked.defaultof<'a>) //slot 0 unavailable    
    member this.IsEmpty() = isEmpty()        
    member this.Push(x: 'a) =
        let up x =
            let mutable i = x
            while (i > 1 && comp(heap.[i / 2], heap.[i])) do
                let j = i / 2
                swap(i, j)
                i <- j
        heap.Add(x)
        up(heap.Count - 1)
    member this.Top() : 'a =
        if isEmpty() then
            failwith "PQ is empty"
        heap.[1]
    member this.Pop() =
        let down x = 
            let N = heap.Count - 2
            let mutable i = x
            let mutable _break = false
            while (not _break && 2 * i <= N) do
                let mutable j = 2 * i
                if (j < N && comp(heap.[j], heap.[j + 1])) then j <- j + 1
                if (comp(heap.[i], heap.[j])) then
                    swap(i, j)
                    i <- j
                else
                    _break <- true
        let N = heap.Count - 1
        swap(1, N)
        down(1)
        heap.RemoveAt(N)

let euclideanDistance (px: int, py: int) (qx: int, qy: int) =
    int (System.Math.Sqrt(float (px - qx) ** 2. + float (py - qy) ** 2.))

type XY = int * int

let AStar (start: XY, heuristic: XY -> int, allowedMoves: XY -> XY list) =
    let fringe = new PQ<int * XY>(fun ((c1, _), (c2, _)) -> c1 < c2) //minpq
    let previous = new System.Collections.Generic.Dictionary<XY, XY option>()
    let pathCost = new System.Collections.Generic.Dictionary<XY, int>()
    fringe.Push(heuristic start, start)
    previous.Add(start, None)
    pathCost.Add(start, 0)
    let mutable goal = None
    while ((goal |> Option.isNone) && not (fringe.IsEmpty())) do
        let (c, x) = fringe.Top()
        if (heuristic(x) = 0) then
            goal <- Some x
        else    
            fringe.Pop()
            for y in allowedMoves(x) do
                let newCost = pathCost.[x] + 1
                if not (pathCost.ContainsKey(y)) || newCost < pathCost.[y] then
                    fringe.Push(newCost + heuristic(y), y)
                    pathCost.[y] <- newCost
                    previous.[y] <- Some x
    match goal with
    | Some _ -> 
        let rec pathRec p acc =
            match p with
            | Some x -> pathRec previous.[x] (x::acc)
            | None -> acc            
        pathRec goal []
    | None -> []

let DisplayEnvironment () = 
    let N = Array2D.length1 EnvironmentState

    let stringFromCellState (x) =
        match x with
        | CellState.Goal -> "O"
        | CellState.Opened -> "_"
        | CellState.Blocked -> "H"
        | CellState.Agent (representation, _) -> representation
        //| CellState.VisitedByAgent i -> if i > 10 then  "." else string i
        | CellState.VisitedByAgent _ -> "."
   
    Console.Clear()

    EnvironmentState |> Array2D.iteri (fun i j x -> 
        let isAgent = match x with | Agent _ -> true | _ -> false
        let isWall = match x with | Blocked -> true | _ -> false
        let isGoal = match x with | Goal -> true | _ -> false
        let s = stringFromCellState (x)
        let oldColor = Console.ForegroundColor
        if isAgent then Console.ForegroundColor <- ConsoleColor.Green
        else if isWall then Console.ForegroundColor <- ConsoleColor.DarkRed
        else if isGoal then Console.ForegroundColor <- ConsoleColor.Yellow 
        if j = N - 1 then printfn "%s " s else printf "%s " s
        Console.ForegroundColor <- oldColor)
    printfn ""

let BuildWalls() =
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

let SetGoal(goal_x, goal_y) = 
    EnvironmentState.[goal_y, goal_x] <- CellState.Goal
    (goal_x, goal_y)

type perception = 
      NorthIsOpened
    | EastIsOpened
    | SouthIsOpened
    | WestIsOpened
    | AgentIsBlocked
    | GoalReached

type action = 
      GoNorth 
    | GoEast 
    | GoSouth 
    | GoWest 
    | Stop

[<AbstractClass>]
type AbstractMovingAgent() =
    abstract member MoveNext : unit -> bool

let isAgent x = match x with | CellState.Agent(_, _) -> true | _ -> false

let isGoalOrOpened x = x = CellState.Goal || x = CellState.Opened

let visited(x, y) =
    match EnvironmentState.[y,x] with
    | Agent (_, i) -> i + 1
    | VisitedByAgent i -> i
    | _ -> 0

let updateEnvironment(x, y, new_x, new_y, representation) =
    printfn "%s: y = %2d x = %2d" representation y x
    EnvironmentState.[y, x] <- VisitedByAgent (visited(x, y))
    EnvironmentState.[new_y, new_x] <- CellState.Agent (representation, visited(new_x, new_y))

type AStarMovingAgent(x, y, goal_x, goal_y, representation: string) =
    inherit AbstractMovingAgent()
    let N = Array2D.length1 EnvironmentState
    let mutable x = x
    let mutable y = y
    let allowedMoves(x, y) =
        let sensors() =
            let blocked = (CellState.Blocked, (-1, -1))
            let envState (y, x) = (EnvironmentState.[y, x], (x, y))
            let front = if y - 1 >= 0 then envState(y - 1, x) else blocked
            let right = if x + 1 < N  then envState(y, x + 1) else blocked
            let back = if y + 1 < N   then envState(y + 1, x) else blocked
            let left = if x - 1 >= 0  then envState(y, x - 1) else blocked
            (front, right, back, left)
        let (front, right, back, left) = sensors()                
        [front; right; back; left] |> List.filter(fun (cellState, _) -> cellState |> isGoalOrOpened)
                                   |> List.map (fun (_, xy) -> xy)
    let mutable shortestPath = AStar((x,y), euclideanDistance(goal_x, goal_y), allowedMoves)
    do
        EnvironmentState.[y, x] <- CellState.Agent (representation, 0)
        
    override this.MoveNext() =
        let updateAgent(new_x, new_y) =
            updateEnvironment(x, y, new_x, new_y, representation)
            x <- new_x; y <- new_y

        let performAction(x, y) = updateAgent(x, y)

        let actionFromAgent(perception) = perception
        
        let perceptionFromAgent() =
            match shortestPath with
            | h :: t -> shortestPath <- t; Some h
            | [] -> None

        let perception = perceptionFromAgent()
        let action = actionFromAgent(perception)
        match action with
        | Some (new_x, new_y) ->
            if EnvironmentState.[y, x] <> CellState.Goal then
                performAction(new_x, new_y)
                true
            else
                false
        | None -> false

type CasualMovingAgent(x, y, goal_x, goal_y, s:string) =
    inherit AbstractMovingAgent()
    let N = Array2D.length1 EnvironmentState
    let mutable x = x
    let mutable y = y
    let representation = s    
    do
        EnvironmentState.[y, x] <- CellState.Agent (representation, 0)

    override this.MoveNext() = 
        let sensors() =
            let envState (y, x) = EnvironmentState.[y, x]
            let blocked = CellState.Blocked
            let front = if y - 1 >= 0 then envState(y - 1, x) else blocked
            let right = if x + 1 < N  then envState(y, x + 1) else blocked
            let back = if y + 1 < N   then envState(y + 1, x) else blocked
            let left = if x - 1 >= 0  then envState(y, x - 1) else blocked
            (front, right, back, left)

        let updateAgent(new_x, new_y) =
            updateEnvironment(x, y, new_x, new_y, representation)
            x <- new_x; y <- new_y

        let performAction(action) =
            match action with
            | GoNorth -> updateAgent(x, y - 1)
            | GoEast -> updateAgent(x + 1, y)
            | GoSouth -> updateAgent(x, y + 1)
            | GoWest -> updateAgent(x - 1, y)
            | _ -> ()

        let actionFromAgent(perception) =
            if (isAgent EnvironmentState.[goal_y, goal_x]) then
                Stop
            else
                match perception with
                | NorthIsOpened -> GoNorth
                | EastIsOpened -> GoEast
                | SouthIsOpened -> GoSouth
                | WestIsOpened -> GoWest
                | AgentIsBlocked | _ -> Stop

        let perceptionFromAgent() =
            let (front, right, back, left) = sensors()
            let perception = 
                let p = 
                    if      isGoalOrOpened front then NorthIsOpened
                    else if isGoalOrOpened right then EastIsOpened
                    else if isGoalOrOpened back  then SouthIsOpened
                    else if isGoalOrOpened left  then WestIsOpened
                    else AgentIsBlocked
                if p = AgentIsBlocked then
                    let visitCount(v) = match v with | VisitedByAgent i -> i | _ -> 0
                    let a = visitCount(left)
                    let b = visitCount(back)
                    let c = visitCount(right)
                    let d = visitCount(front)
                    let xs = [(a, WestIsOpened); (b, SouthIsOpened); (c, EastIsOpened); (d, NorthIsOpened)]
                             |> List.filter (fun (i, _) -> 0 < i && i < 9)
                    match xs with
                    | [] -> AgentIsBlocked
                    | _ -> 
                        snd (xs |> List.sortBy (fun (i, action) -> i) |> List.head)
                else p                
            perception

        let perception = perceptionFromAgent()
        let action = actionFromAgent(perception)        
        if action <> Stop then
            performAction(action)
            true
        else
            false

type HeuristicMovingAgent(x, y, goal_x, goal_y, s:string) =
    inherit AbstractMovingAgent()
    let N = Array2D.length1 EnvironmentState
    let mutable x = x
    let mutable y = y
    let representation = s    
    do
        EnvironmentState.[y, x] <- CellState.Agent (representation, 0)

    override this.MoveNext() = 
        let sensors() =
            let INF = 1000000000
            let envState(y, x) = (EnvironmentState.[y, x], euclideanDistance (goal_x, goal_y) (x, y))
            let blocked = (CellState.Blocked, INF)
            let front = if y - 1 >= 0 then envState(y - 1, x) else blocked
            let right = if x + 1 < N  then envState(y, x + 1) else blocked
            let back = if y + 1 < N   then envState(y + 1, x) else blocked
            let left = if x - 1 >= 0  then envState(y, x - 1) else blocked
            (front, right, back, left)

        let updateAgent(new_x, new_y) =
            updateEnvironment(x, y, new_x, new_y, representation)
            x <- new_x; y <- new_y

        let performAction(action) =
            match action with
            | GoNorth -> updateAgent(x, y - 1)
            | GoEast -> updateAgent(x + 1, y)
            | GoSouth -> updateAgent(x, y + 1)
            | GoWest -> updateAgent(x - 1, y)
            | _ -> ()

        let actionFromAgent(perception) =
            if (isAgent EnvironmentState.[goal_y, goal_x]) then
                Stop
            else
                match perception with
                | NorthIsOpened -> GoNorth
                | EastIsOpened -> GoEast
                | SouthIsOpened -> GoSouth
                | WestIsOpened -> GoWest
                | AgentIsBlocked | _ -> Stop
        
        let perceptionFromAgent() =
            let (front, right, back, left) = sensors()
            let perception =
                let p = 
                    let moves = [(front, NorthIsOpened); (right, EastIsOpened); (back, SouthIsOpened); (left, WestIsOpened)] 
                                |> List.sortWith(fun ((_, x), _) ((_, y), _) -> x - y) 
                                |> List.filter (fun ((x, _), _) -> isGoalOrOpened x)
                                |> List.map (fun (_, x) -> x)
                    if (List.isEmpty moves) then AgentIsBlocked else List.head moves                
                if p = AgentIsBlocked then
                    let visitCount(v) = match v with | VisitedByAgent i -> i | _ -> 0
                    let a = visitCount(fst left)
                    let b = visitCount(fst back)
                    let c = visitCount(fst right)
                    let d = visitCount(fst front)
                    let xs = [(a, WestIsOpened); (b, SouthIsOpened); (c, EastIsOpened); (d, NorthIsOpened)]
                             |> List.filter (fun (i, _) -> 0 < i && i < 9)
                    match xs with
                    | [] -> AgentIsBlocked
                    | _ -> 
                        snd (xs |> List.sortBy (fun (i, action) -> i) |> List.head)
                else p                
            perception

        let perception = perceptionFromAgent()
        let action = actionFromAgent(perception)
        if action <> Stop then
            performAction(action)
            true
        else
            false

[<EntryPoint>]
let main argv =    
    let title = "Moving Agents by Fabio Galuppo (fabiogaluppo.com)  "
    Console.Title <- title
    
    let N = Array2D.length1 EnvironmentState
    let goal_x, goal_y = SetGoal(N - 1, N - 1)
    BuildWalls()
    
    let a = new CasualMovingAgent(N / 2, N / 2, goal_x, goal_y, "A")
    //let a = new HeuristicMovingAgent(N / 2, N / 2, goal_x, goal_y, "A")
    //let a = new AStarMovingAgent(N / 2, N / 2, goal_x, goal_y, "A")
    
    //let b = new HeuristicMovingAgent(0, 0, goal_x, goal_y, "B")
    //let b = new AStarMovingAgent(0, 0, goal_x, goal_y, "B")    
    
    //let c = new CasualMovingAgent(0, N - 1, goal_x, goal_y, "C")

    let fps_to_ms n = 1000 / n
    let sw = System.Diagnostics.Stopwatch.StartNew()

    while not (List.isEmpty ([a.MoveNext()] |> List.filter (fun x -> x))) do
    //while not (List.isEmpty ([a.MoveNext(); b.MoveNext()] |> List.filter (fun x -> x))) do
    //while not (List.isEmpty ([a.MoveNext(); b.MoveNext(); c.MoveNext()] |> List.filter (fun x -> x))) do
        Console.Title <- title + sw.ElapsedMilliseconds.ToString() + " ms"
        //System.Threading.Thread.Sleep(fps_to_ms 12)  
        System.Threading.Thread.Sleep(fps_to_ms 4)      
        DisplayEnvironment()

    printfn "Stopped..."
    0