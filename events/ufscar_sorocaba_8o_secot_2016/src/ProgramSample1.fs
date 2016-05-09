//https://msdn.microsoft.com/en-us/library/dd233212.aspx
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open Microsoft.FSharp.Quotations.DerivedPatterns

//Limit of f when x approaches x0
let Limit f x0 =
    let threshold = 0.0001
    let left  = f (x0 - threshold)
    let right = f (x0 + threshold)
    let estimate = (left + right) / 2.
    let threshold = 0.01
    if (abs (left - right) < threshold) then
        Some estimate
    else 
        None

//Code Quotations 
let printExpression expr =
    let translateValue (v : obj) (t : System.Type) =
        if (t = typeof<System.Double>) then 
            sprintf "%0.4f" (System.Convert.ToDouble(v))
        else t.ToString()
    let translateType (t : System.Type) =
        if (t = typeof<System.Double>) then "float"
        else t.ToString()
    let isVarOrValue x =
        match x with
        | Var(_) -> true
        | Value(_, _) -> true
        | _ -> false
    let rec printExpressionRec expr =
        let printBinOp op (exprList : Expr list) =
            let parenthesis x =
                if isVarOrValue(x) then
                    printExpressionRec x
                else
                    printf "("
                    printExpressionRec x
                    printf ")"
            parenthesis (exprList |> List.head)
            printf "%s" op
            parenthesis (exprList |> List.tail |> List.head)            
        match expr with
        | SpecificCall <@@ (+) @@> (_, _, exprList) -> printBinOp " + " exprList
        | SpecificCall <@@ (-) @@> (_, _, exprList) -> printBinOp " - " exprList
        | SpecificCall <@@ (*) @@> (_, _, exprList) -> printBinOp " * " exprList
        | SpecificCall <@@ (/) @@> (_, _, exprList) -> printBinOp " / " exprList
        | SpecificCall <@@ (%) @@> (_, _, exprList) -> printBinOp " % " exprList
        | SpecificCall <@@ ( ** ) @@> (_, _, exprList) -> printBinOp "**" exprList
        | Call(exprOpt, methodInfo, exprList) ->
            match exprOpt with
                | Some expr -> printExpressionRec expr
                | None -> printf "%s" methodInfo.DeclaringType.Name
            printf ".%s " methodInfo.Name
            printExpressionRec (exprList |> List.head)
            exprList |> List.tail 
                        |> List.iter (fun expr -> printf " "
                                                  printExpressionRec expr)
        | Lambda(x, body) ->
            printf "(fun (%s:%s) -> " x.Name (translateType x.Type)
            printExpressionRec body
            printf ")"
        | Var(v) -> printf "%s" v.Name
        | Value(v, t) -> printf "%s" (translateValue v t)
        | _ -> printf "%s" (expr.ToString()) //DBG
    printExpressionRec expr
    printfn ""

let displayLimit f x0 = 
    let result = Limit f x0
    match result with
    | Some (value) -> printfn "The limit of f when x approaches %f is %f" x0 value
    | None -> printfn "The limit does not exist"    

printf "f: "
printExpression <@@ Limit (fun x -> 3. * (x ** 2.)) 2. @@>
displayLimit (fun x -> 3. * (x ** 2.)) 2.
printfn ""

printf "f: "
printExpression <@@ Limit (fun x -> (2. * x + 1.) / (x ** 2.)) 3. @@>
displayLimit (fun x -> (2. * x + 1.) / (x ** 2.)) 3.
printfn ""

printf "f: "
printExpression <@@ Limit (fun x -> (x ** 2. - 1.) / (x - 1.)) 1. @@>
displayLimit (fun x -> (x ** 2. - 1.) / (x - 1.)) 1.
printfn ""

printf "f: "
printExpression <@@ Limit (fun x -> sin (5. / (x - 1.))) 1. @@>
displayLimit (fun x -> sin (5. / (x - 1.))) 1.
printfn ""

printf "f: "
printExpression <@@ Limit (fun x -> ((2. + x) ** 2. - 4.) / x) 0. @@>
displayLimit (fun x -> ((2. + x) ** 2. - 4.) / x) 0.
printfn ""

printf "f: "
printExpression <@@ Limit (fun x -> (1.+ x) ** (1. / x)) 0. @@>
displayLimit (fun x -> (1.+ x) ** (1. / x)) 0.
printfn ""

printf "f: "
printExpression <@@ Limit (fun x -> 3. * x + 2.) 1. @@>
displayLimit (fun x -> 3. * x + 2.) 0.
printfn ""

printf "f: "
printExpression <@@ Limit (fun x -> (2. * x ** 2. - 3.) / (3. * x + 2.)) 1. @@>
displayLimit (fun x -> (2. * x ** 2. - 3.) / (3. * x + 2.)) 1.
printfn ""

printf "f: "
printExpression <@@ Limit (fun x -> (x ** 2. - 9.) / (x - 3.)) 3. @@>
displayLimit (fun x -> (x ** 2. - 9.) / (x - 3.)) 3.
printfn ""

printf "f: "
printExpression <@@ Limit (fun x -> (x ** 2. - (2. * x) - 3.) / (x - 3.)) 3. @@>
displayLimit (fun x -> (x ** 2. - (2. * x) - 3.) / (x - 3.)) 3.
printfn ""

printf "f: "
printExpression <@@ Limit (fun x -> 2. / (x - 1.)) 1. @@>
displayLimit (fun x -> 2. / (x - 1.)) 1.
printfn ""

printf "f: "
printExpression <@@ Limit (fun x -> (x ** 2. + 1.) / (x - 1.)) 1. @@>
displayLimit (fun x -> (x ** 2. + 1.) / (x - 1.)) 1.
printfn ""

printf "f: "
printExpression <@@ Limit (fun x -> ((1. + x) ** 2. - 1.) / x) 0. @@>
displayLimit (fun x -> ((1. + x) ** 2. - 1.) / x) 0.
printfn ""

printf "f: "
printExpression <@@ Limit (fun x -> exp x) 0. @@>
displayLimit (fun x -> exp x) 0.
printfn ""

printf "f: "
printExpression <@@ Limit (fun x -> (sin x) / x) 0. @@>
displayLimit (fun x -> (sin x) / x) 0.

printfn "------------------"

seq[1.; 0.1; 0.01; 0.] |> Seq.iter (fun x0 -> displayLimit (fun x -> (sin x) / x) x0)
seq[-1.5; 1.; 1.5; 2.3; 4.] |> Seq.iter (fun x0 -> displayLimit (fun x -> sin (5. / (x - 1.))) x0)

printfn "------------------"

printfn "%s" ((<@@ Limit (fun x -> (sin x) / x) 0. @@>).ToString())
printfn "%s" ((<@@ Limit (fun x -> 3. * (x ** 2.)) 2. @@>).ToString())

printfn "------------------"

printfn "%A" (Limit (fun x -> 3. * (x ** 2.)) 2.)

printfn "%A" (Limit (fun x -> (sin x) / x) 0.)

for i in seq[1.; 0.1; 0.01; 0.] do    
    printfn "%A" (Limit (fun x -> (sin x) / x) i)

for i in seq[-1.5; 1.; 1.5; 2.3; 4.] do
    printfn "%A" (Limit (fun x -> sin (5. / (x - 1.))) i)

