//state
type person = | M of int | C of int
type state = {top : Set<person>; bottom : Set<person>; isValid : bool} 

let initialState = {top = Set.ofList [C 1; C 2; C 3; M 1; M 2; M 3]; bottom = Set.ofList []; isValid = true}

let goalState = {top = Set.ofList []; bottom = Set.ofList [C 1; C 2; C 3; M 1; M 2; M 3]; isValid = true}

//operations on state
let isValid (xs:Set<person>) =
    let (c, m) = xs |> Set.fold (fun acc s ->
                                    let (c, m) = acc
                                    match s with
                                    | M _ -> (c, m + 1)
                                    | C _ -> (c + 1, m)) (0, 0)
    m = 0 || m >= c

let moveDown (s:state) =
    let rec moveDownRec (t:Set<person>, b:Set<person>, xs:person seq) =
        if Seq.isEmpty xs then 
            Seq.empty
        else
            let x = Seq.head xs
            let ys = xs |> Seq.skip 1
            Seq.append (seq{for y in ys do 
                                let bottom = Set.union b (Set.ofList [x; y])
                                let top = Set.difference t bottom
                                let isValid = isValid(top) && isValid(bottom)
                                yield {top = top; bottom = bottom; isValid = isValid}}) (moveDownRec (t, b, ys))
    moveDownRec (s.top, s.bottom, (Set.toList s.top))

let moveUp (s:state) =
    seq{for x in s.bottom do 
            let xs = Set.ofList [x]
            let bottom = Set.difference s.bottom xs
            let top = Set.union s.top xs
            let isValid = isValid(bottom)
            yield {top = top; bottom = bottom; isValid = isValid}}

//explore state space using depth-first search and extract the first solution
let findSolution (s:state) (maxDepth:int) =
    let rec dfs (xs:seq<state>, depth:int, acc : state list) =
        if depth < maxDepth then
            let retVal = ref (false, acc)
            xs |> Seq.find (fun x -> 
                                if x.isValid then
                                    retVal := dfs ((if depth % 2 = 0 then moveDown x else moveUp x),
                                                    depth + 1, List.append acc [x])
                                    true
                                else
                                    false) |> ignore
            !retVal
        else
            (true, acc)
    snd (dfs ([s], 0, []))

//main
let solution = findSolution initialState 10
printfn "%A" solution

(* who is/are in the boat?
   pseudo code:
   if down then boat = Set.intersection current.bottom previous.top
   else 
   if up then boat = Set.intersection current.top previous.bottom *)