//Sample provided by Fabio Galuppo 
//September 2015

//compile: fsc -o:.\bin\FunctionsAndInverses.exe FunctionsAndInverses.fs
//run: .\bin\FunctionsAndInverses.exe

open System

//Functions and Inverses

let lg N = Math.Log(N, 2.)
let power_of_2 N = 2. ** N
let square_root N =  Math.Sqrt(float N)
let square N = N ** 2.
let n N = N
let n_lg_n N = N * lg N
let inverse_of_n_lg_n N = //http://math.stackexchange.com/questions/1301343/how-to-find-the-inverse-of-n-log-n
    let ITERS = 50
    let mutable n = N
    for i = 1 to ITERS do n <- N / lg n
    n
let cube_root N = N ** (1./3.)
let cube N = N ** 3.

let N = 8.

let composition_of f g = 
    let c = f >> g
    let a = c N
    let b = id N
    let aboutEquals (x : float) (y : float) =
        let epsilon = Math.Max(Math.Abs(x), Math.Abs(y)) * 1E-15 
        Math.Abs(x - y) <= epsilon
    printfn "%.0f => %A" a (aboutEquals a b)

composition_of lg power_of_2

composition_of square_root square

composition_of n n

//printfn "%.0f" (n_lg_n N)
//printfn "%.0f" (inverse_of_n_lg_n (n_lg_n N))
composition_of n_lg_n inverse_of_n_lg_n

composition_of square square_root

composition_of cube cube_root

composition_of power_of_2 lg