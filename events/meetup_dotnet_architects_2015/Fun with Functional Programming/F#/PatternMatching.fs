//Sample provided by Fabio Galuppo 
//February 2015 

module PatternMatchingProgram

open System

//Tuples:

let r = (1, int64 2, bigint 3, 4.)
let (a, b) = (1, "2")
let (c, d, e) = (3., bigint 4, (true, false))
let e0 = fst e
let e1 = snd e

let get0 (x, _, _, _) = x
let get1 (_, x, _, _) = x
let get2 (_, _, x, _) = x
let get3 (_, _, _, x) = x

//Pattern matching as switch-case:

//let chooseANumber (x : int) : string = //annotations
let chooseANumber x = //inference
    match x with 
    | 1 | 2 | 3 -> "Yes, correct number :-)"
    | _ -> "No, wrong number :-("

//Discriminated Unions:

type Nullable<'a> = 
    | Value of 'a 
    | Null

let hasValue (x) = 
    match x with
    | Value(_) -> true
    | _ -> false

let hasSomeValue (x) =
    match x with
    | Some(_) -> true
    | None -> false

let getSomeValue (x) =
    match x with
    | Some(value) -> value
    | _ -> failwith "None"

type Color =
    | RGB of r : float * g : float * b : float
    | CMYK of c : float * m : float * y : float * k : float //http://en.wikipedia.org/wiki/CMYK_color_model

let blue = RGB(0., 0., 1.)
let yellow = CMYK(0., 0., 90., 90.)

//Records:

type File = { Name : string; Directory : string }

let file0 = { Name = "calc.exe"; Directory = @"c:\windows\system32" }

let isFileInDirectory file (directory : string) = 
    match file with
    | { Name = _; Directory = dir } when dir = directory -> true
    | _ -> false 

//Lists:

let rec cardinality lst = 
    match lst with
    | [] -> 0
    | [_] -> 1
    | [_; _] -> 2
    | [_; _; _] -> 3
    | _ :: xs -> 1 + (cardinality xs)

let test1 () = 
    printfn "%A" (get0 r)
    printfn "%A" (get1 r)
    printfn "%A" (get2 r)
    printfn "%A" (get3 r)
    printfn "%s" (chooseANumber 1)
    printfn "%s" (chooseANumber 10)
    printfn "%b" (hasValue (Value "Hello"))
    printfn "%b" (hasValue Null)
    printfn "%b" (hasSomeValue (Some "Hello"))
    printfn "%b" (hasSomeValue None)
    printfn "%s" (getSomeValue (Some "Hello"))
    printfn "%A" blue
    printfn "%A" yellow
    printfn "%b" (isFileInDirectory file0 "c:\windows")
    printfn "%b" (isFileInDirectory file0 "c:\windows\system32")
    printfn "%d" (cardinality [1..10])
    printfn "%d" (cardinality [1])
    printfn "%d" (cardinality [1; 2])


