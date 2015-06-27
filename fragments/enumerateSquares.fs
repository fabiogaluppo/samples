//http://elemarjr.net/2015/06/25/o-que-e-mais-funcional

//was this ugly code
(* 
let enumerateSquares (bits: uint64) =
	let bitIsOn x = (bits >>> x &&& 1UL) <> 0UL 
	[| for i in 0..63 -> if (bitIsOn i) then (1UL <<< i) else 0UL |]
	|> Array.filter (fun x -> x > 0UL)
*)
//now the suggestion (more info: https://github.com/fabiogaluppo/samples/tree/master/fragments/EnumerateSquares)
let enumerateSquares (bits: uint64) =
    match bits with
    | 0UL -> [||]
    | _ -> let y = ref (int64 bits)
           [| while (!y <> 0L) do let x = !y
                                  yield uint64 (x &&& -x) 
                                  y := x &&& (x - 1L) |]
