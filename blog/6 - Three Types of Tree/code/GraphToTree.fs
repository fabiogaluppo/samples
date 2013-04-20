//Sample provided by Fabio Galuppo
//February 2013

module GraphToTree

type Node<'a when 'a : equality> = { value : 'a; mutable connections : (Node<'a> * int) list }

let toTree (x : Node<'a>) =
    let rec recToTree (x : Node<'a>, acc : (Node<'a> * int) list) =
        seq { let children = x.connections |> //essentially this is a set diff in the hard way
                                List.filter (fun (a, _) -> 
                                    not (acc |> List.exists (fun (b, _) -> b.value = a.value)))
              if not (List.isEmpty children) then
                for child in children do
                    for ys in recToTree(fst child, acc @ [child]) do yield ys
              else yield acc }
    recToTree(x, [(x, 0)])

let connect (a: Node<'a>, b: Node<'a>, distance : int) =
     a.connections <- (b, distance) :: a.connections
     b.connections <- (a, distance) :: b.connections

let greedyApproach(x : Node<'a>) =
    toTree(x) |> Seq.map (fun xs -> (xs, xs |> List.fold (fun acc (_, distance) -> acc + distance) 0))
              |> Seq.sortBy (fun (_, distance) -> distance)
              |> Seq.map (fun (xs, _) -> xs)

let getDistance(source: Node<'a>, destination: Node<'a>) =
    let (_, distance) = source.connections |> List.find (fun (x, _) -> x.value = destination.value)
    distance

let print (xs: (Node<char> * int) list) =
    let mutable count = 0
    for (node, distance) in xs do
        printf " -> (%02d)%c" distance node.value
        count <- count + distance
    printf " = %d" count
    printfn ""

let entryPoint =
    let a = { value = 'A'; connections = [] }
    let b = { value = 'B'; connections = [] }
    let c = { value = 'C'; connections = [] }
    let d = { value = 'D'; connections = [] }
    let e = { value = 'E'; connections = [] }

    connect(a, b, 10); connect(a, c, 7)
    connect(a, d, 3);  connect(a, e, 8)
    connect(b, c, 5);  connect(b, d, 12)
    connect(b, e, 9);  connect(c, d, 1)
    connect(c, e, 2);  connect(d, e, 4)
    
    for path in greedyApproach(a) do print(path)