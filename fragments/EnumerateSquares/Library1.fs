//compile: fsc --optimize+ --target:library /out:bin\Lib.dll Library1.fs

namespace Lib

type Class1() = 
    let rec enumerateSquaresImpl1 bitboard = seq {
        if bitboard <> 0UL then
            let lb = int64 bitboard
            let square = lb &&& (-lb)           // isolate the rightmost bit
            yield (uint64 square)
            let remaining = lb &&& (lb - 1L)    // turn off the rightmost bit
            if remaining <> 0L then yield! enumerateSquaresImpl1 (uint64 remaining)
    }

    let enumerateSquaresImpl2 (bitboard: uint64) = 
        int64 bitboard
            |> Seq.unfold(fun x -> if x = 0L then None else Some(x, x &&& (x - 1L)))
            |> Seq.map(fun x -> uint64 (x &&& (-x)))

    //original
    let enumerateSquaresImpl3 (bits: uint64) =
        let bitIsOn x = (bits >>> x &&& 1UL) <> 0UL 
        [| for i in 0..63 -> if (bitIsOn i) then (1UL <<< i) else 0UL |] |> Array.filter (fun x -> x > 0UL)
//    //better alternative
//    let enumerateSquaresImpl3 (bits: uint64) =
//        let bitIsOn x = (bits >>> x &&& 1UL) <> 0UL 
//        [| for i in 0..63 do if (bitIsOn i) then yield (1UL <<< i) |]

    let enumerateSquaresImpl4 (bits: uint64) =
        match bits with
        | 0UL -> [||]
        | _ -> let y = ref (int64 bits)
               [| while (!y <> 0L) do let x = !y
                                      yield uint64 (x &&& -x) 
                                      y := x &&& (x - 1L) |]

    member this.EnumerateSquaresImpl1 x = enumerateSquaresImpl1 x

    member this.EnumerateSquaresImpl2 x = enumerateSquaresImpl2 x
    
    member this.EnumerateSquaresImpl3 x = enumerateSquaresImpl3 x

    member this.EnumerateSquaresImpl4 x = enumerateSquaresImpl4 x