//http://elemarjr.net/2015/06/25/o-que-e-mais-funcional
let enumerateSquares (bits: uint64) =
	let bitIsOn x = (bits >>> x &&& 1UL) <> 0UL 
	[| for i in 0..63 -> if (bitIsOn i) then (1UL <<< i) else 0UL |]
	|> Array.filter (fun x -> x > 0UL)
