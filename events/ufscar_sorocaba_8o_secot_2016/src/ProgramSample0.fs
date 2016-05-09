//Sample provided by Fabio Galuppo 
//May 2016 

let sumOnlyPositivesImperative (xs : int list) = 
    let mutable acc = 0
    for x in xs do
        if (x > 0) then acc <- acc + x
    acc

let sumOnlyPositivesFunctional (xs : int list) = 
    xs |> List.filter (fun x -> x > 0)
       |> List.sum

let xs = [1; -1; 2; -3; 5; -6; 10]
printfn "Equals? %A" (sumOnlyPositivesImperative xs = sumOnlyPositivesFunctional xs)