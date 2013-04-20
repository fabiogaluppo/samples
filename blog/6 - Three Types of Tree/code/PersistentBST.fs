//Sample provided by Fabio Galuppo
//February 2013

module PersistentBST

type Node<'a> = Empty | Node of Node<'a> * 'a * Node<'a>

let rec insert<'a when 'a : comparison>(node : Node<'a>, value : 'a)  = 
    match node with 
    | Empty -> 
        Node(Empty, value, Empty)
    | Node(left, nodeValue, right) ->
        if (value < nodeValue) then 
            Node(insert(left, value), nodeValue, right)
        else if (value > nodeValue) then 
            Node(left, nodeValue, insert(right, value))
        else
            node

let bf_traversal<'a when 'a : comparison>(n : Node<'a>, visit : 'a -> unit) =
    let expandNode (n : Node<'a>) =
        match n with
        | Empty -> failwith "Empty not supported"
        | Node(left, value, right) -> (left, value, right)
                    
    let q = new System.Collections.Generic.Queue<Node<'a>>()
    q.Enqueue(n)
    while q.Count > 0 do
        let (left, value, right) = expandNode(q.Dequeue())
        visit(value)
        if Empty <> left then q.Enqueue(left)
        if Empty <> right then q.Enqueue(right)

let display (title, node : Node<'a>, traversalFun : Node<'a> * (char -> unit) -> unit) =
    printf title
    traversalFun (node, fun x -> printf "%c " x)
    printfn ""

let entryPoint = 
   
    let level0 = insert(Empty,'F')
    let level1 = insert(insert(level0, 'B'), 'G')
    let level2 = insert(insert(insert(level1, 'A'), 'D'), 'I')
    let level3 = insert(insert(insert(level2, 'C'), 'E'), 'H')
    
    display("Level order traversal sequence: ", level0, bf_traversal)
    display("Level order traversal sequence: ", level1, bf_traversal)
    display("Level order traversal sequence: ", level2, bf_traversal)
    display("Level order traversal sequence: ", level3, bf_traversal)