//Sample provided by Fabio Galuppo
//May 2013

let order_of_growth(N : uint64) = 
    assert(0uL <= N && N <= 20uL)
    
    let _0, _1, _2 = 0uL, 1uL, 2uL
    
    let constant = _1

    let logarithmic =
        let mutable sum, i = _0, _2
        while (i <= N) do
            sum <- sum + _1; i <- i * _2
        sum

    let linear =
        let mutable sum, i = _0, _1
        while (i <= N) do
            sum <- sum + _1; i <- i + _1
        sum

    let linear_logarithmic = 
        let mutable sum, i = _0, _1
        while (i <= N) do
            let mutable j = _2
            while (j <= N) do
                sum <- sum + _1; j <- j * _2
            i <- i + _1
        sum

    let quadratic = 
        let mutable sum, i = _0, _1
        while (i <= N) do
            let mutable j = _1
            while (j <= N) do
                sum <- sum + _1; j <- j + _1
            i <- i + _1
        sum

    let cubic = 
        let mutable sum, i = _0, _1
        while (i <= N) do
            let mutable j = _1
            while (j <= N) do
                let mutable k = _1
                while (k <= N) do
                    sum <- sum + _1; k <- k + _1
                j <- j + _1
            i <- i + _1
        sum

    let exponential =
        let rec factorial n = 
            if (n < _1) then _1 else n * factorial(n - _1)
        let binomial_coefficient(n, k) =
            (factorial n) / ((factorial k) * (factorial (n - k)))
        let mutable sum, i = _0, _0
        while (i <= N) do
            let k = binomial_coefficient(N, i)
            let mutable j = _1
            while (j <= k) do
                sum <- sum + _1;  j <- j + _1
            i <- i + _1
        sum
         
    printfn "Ordem de Crescimento N = %d" N
    List.zip 
        ["constante"; "logarítmico"; "linear"; "linear-logarítmico"; "quadrático"; "cúbico"; "exponencial"]
        [ constant;    logarithmic;   linear;   linear_logarithmic;   quadratic;    cubic;    exponential]
        |> List.iter (fun (x, y) -> printfn "%-18s = %10d" x y)
    printfn ""

for i = 2 to 20 do order_of_growth(uint64 i)