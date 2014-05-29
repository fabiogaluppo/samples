//Sample provided by Fabio Galuppo
//April 2014

(* to know more about Missionaries and Cannibals Problem and State Space Search:
  http://en.wikipedia.org/wiki/Missionaries_and_cannibals_problem
  http://en.wikipedia.org/wiki/State_space_search
  http://www-formal.stanford.edu/jmc/circumscription/node3.html *)

//324000 possibilities with 7965 solutions

//state
type person = | M of int | C of int
type state = {top : Set<person>; bottom : Set<person>; isValid : bool} 

let initialState = {top = Set.ofList [C 1; C 2; C 3; M 1; M 2; M 3]; bottom = Set.ofList []; isValid = true}

let goalState = {top = Set.ofList []; bottom = Set.ofList [C 1; C 2; C 3; M 1; M 2; M 3]; isValid = true}

//functions to problem solving
let isValid (xs:Set<person>) =
    let (c, m) = xs |> Set.fold (fun acc s ->
                                    let (c, m) = acc
                                    match s with
                                    | M _ -> (c, m + 1)
                                    | C _ -> (c + 1, m)) (0, 0)
    m = 0 || m >= c

let moveDown (s:state) =
    let rec moveDownRec (t:Set<person>, b:Set<person>, xs:person list) =
        match xs with
        | x :: ys -> 
            List.append [for y in ys -> 
                            let bottom = Set.union b (Set.ofList [x; y])
                            let top = Set.difference t bottom
                            let isValid = isValid(top) && isValid(bottom)
                            {top = top; bottom = bottom; isValid = isValid}] (moveDownRec (t, b, ys))
        | _ -> []    
    moveDownRec (s.top, s.bottom, (Set.toList s.top))

let moveUp (s:state) =
    [for x in s.bottom -> 
        let xs = Set.ofList [x]
        let bottom = Set.difference s.bottom xs
        let top = Set.union s.top xs
        let isValid = isValid(bottom)
        {top = top; bottom = bottom; isValid = isValid}]

//state space tree
type Node<'a> = {value : 'a; mutable children : Node<'a> list}

let makeNode (s) = {value = s; children = []}

let makeTree (state) = 
    let rec makeTreeRec (n:Node<state>, isDown) : Node<state> =
        let x = n.value
        let ss = if isDown then (moveDown x) else (moveUp x)        
        if List.length ss > 1 then
            let children = ss |> List.map (fun s -> makeNode s)
            let isUp = not isDown            
            for child in children do makeTreeRec (child, isUp) |> ignore
            n.children <- children
        n
    makeTreeRec (makeNode state, true)

//explore state space tree using depth-first search
let dfs (node:Node<state>) =
    let c = ref 1
    let display (xs:int list, ys:state list, depth:int) = 
        let toString (s:state) =
            let p (x:person) =
                match x with
                | M i -> sprintf "M%d" i
                | C i -> sprintf "C%d" i        
            let sb = new System.Text.StringBuilder()
            sb.Append("{") |> ignore
            for x in s.top do sb.Append(p x).Append(" ") |> ignore
            sb.Append("|~~| ") |> ignore
            for x in s.bottom do sb.Append(p x).Append(" ") |> ignore
            sb.Remove(sb.Length - 1, 1) |> ignore
            //sb.Append(" -> ").Append(s.isValid).Append("}") |> ignore
            //or (*)
            sb.Append("}") |> ignore
            sb.ToString()
        let zs = List.zip xs ys |> Array.ofList
        printfn "-----------------------------------------"
        printfn "{depth = %d} %d" depth !c
        c := !c + 1
        printfn "%2d:%s initial" (fst zs.[0]) (toString(snd zs.[0]))
        let mutable isDown = true
        for i = 1 to depth do 
            printfn "%2d:%s %s" (fst zs.[i]) (toString(snd zs.[i])) (if isDown then "down" else "up")
            isDown <- not isDown

    let rec dfsRec (node:Node<state>, xs:int list, ys:state list, isDown:bool, depth:int) =
        if not (List.isEmpty node.children) then
            let mutable i = 0
            let isUp = not isDown
            for child in node.children do
                //dfsRec(child, List.append xs [i], List.append ys [child.value], isUp, depth + 1)
                //or (*)
                if (child.value.isValid) then                
                    dfsRec(child, List.append xs [i], List.append ys [child.value], isUp, depth + 1)
                else
                    display (List.append xs [i], List.append ys [child.value], depth + 1)
                i <- i + 1
        else
            display (List.append xs [0], List.append ys [goalState], depth + 1)

    dfsRec (node, [0], [node.value], true, 0)

//main
System.Console.Title <- "Missionaries and Cannibals Problem and State Space Search by Fabio Galuppo (fabiogaluppo.com)"

let root = makeTree initialState
dfs root

(* who is/are in the boat?
   pseudo code:
   if down then boat = Set.intersection current.bottom previous.top
   else 
   if up then boat = Set.intersection current.top previous.bottom *)