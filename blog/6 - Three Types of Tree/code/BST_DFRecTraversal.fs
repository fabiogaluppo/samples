//Sample provided by Fabio Galuppo
//February 2013

module BST_DFRecTraversal

//binary (b) search tree
module btree =
    type Node<'a> = { value : 'a; left : Node<'a> option; right : Node<'a> option }

    let isNull  (node : Node<'a> option) = node.IsNone
    let value   (node : Node<'a> option) = node.Value.value
    let left    (node : Node<'a> option) = node.Value.left
    let right   (node : Node<'a> option) = node.Value.right
    let newNode (value, left, right)     = Some { value = value; left = left; right = right }

//depth-first (df) recursive traversal
module df_traversal =
    let rec preOrder (node : btree.Node<'a> option, visit : 'a -> unit) =
        if not (btree.isNull(node)) then
            visit (btree.value(node))
            preOrder (btree.left(node), visit)
            preOrder (btree.right(node), visit)

    let rec inOrder (node : btree.Node<'a> option, visit : 'a -> unit) =
        if not (btree.isNull(node)) then
            inOrder (btree.left(node), visit)
            visit (btree.value(node))
            inOrder (btree.right(node), visit)

    let rec postOrder (node : btree.Node<'a> option, visit : 'a -> unit) =
        if not (btree.isNull(node)) then
            postOrder (btree.left(node), visit)
            postOrder (btree.right(node), visit)
            visit (btree.value(node))

let display (title, node : btree.Node<char> option, traversalFun : btree.Node<char> option * (char -> unit) -> unit) =
    printf title
    traversalFun (node, fun x -> printf "%c " x)
    printfn ""

let entryPoint =     
    let c = btree.newNode('C', None, None)
    let e = btree.newNode('E', None, None)
    let h = btree.newNode('H', None, None)
    let a = btree.newNode('A', None, None)
    let d = btree.newNode('D', c, e)
    let i = btree.newNode('I', h, None)
    let b = btree.newNode('B', a, d)
    let g = btree.newNode('G', None, i)
    let f = btree.newNode('F', b, g)
    
    display("Pre-order  traversal sequence: ", f, df_traversal.preOrder)
    display("In-order   traversal sequence: ", f, df_traversal.inOrder)
    display("Post-order traversal sequence: ", f, df_traversal.postOrder)