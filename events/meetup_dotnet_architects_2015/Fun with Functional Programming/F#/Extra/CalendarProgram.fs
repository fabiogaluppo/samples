//Sample provided by Fabio Galuppo 
//February 2015

//Published here, give me a like: http://fssnip.net/pA

let printCalendar (month, year) =
    let dayOfWeek (month: int32, year: int32) =
        let t =  [|0; 3; 2; 5; 0; 3; 5; 1; 4; 6; 2; 4|]
        let y = if month < 3 then year - 1 else year
        let m = month
        let d = 1
        (y + y / 4 - y / 100 + y / 400 + t.[m - 1] + d) % 7
        //0 = Sunday, 1 = Monday, ...

    let lastDayOfMonth (month: int32, year: int32) =
        match month with
        | 2 -> if (0 = year % 4 && (0 = year % 400 || 0 <> year % 100)) then 29 else 28
        | 4 | 6 | 9 | 11 -> 30
        | _ -> 31

    let min (x: int32, y: int32) = if x < y then x else y

    let ld = lastDayOfMonth(month, year)
    let dw = 7 - dayOfWeek(month, year)
    let xss = [[1..dw];
               [dw + 1..dw + 7];
               [dw + 8..dw + 14]; 
               [dw + 15..dw + 21]; 
               [dw + 22..min(ld, dw + 28)]; 
               [min(ld + 1, dw + 29)..ld]]

    let list2string xs lineFormatMask = 
        let ys = (xs |> List.map (fun x -> sprintf "%2d " x))
        let mkString = ys |> List.fold (fun s x -> s + x) ""
        sprintf lineFormatMask mkString

    let printRange xs = (printfn "%s") << (list2string xs)
    let printLeft xs = printRange xs "%-21s"
    let printRight xs = printRange xs "%21s"
    
    //pt-BR
    //let months = [|"Jan"; "Fev"; "Mar"; "Abr"; "Mai"; "Jun"; "Jul"; "Ago"; "Set"; "Out"; "Nov"; "Dez"|]
    //en-US
    let months = [|"Jan"; "Feb"; "Mar"; "Apr"; "May"; "Jun"; "Jul"; "Aug"; "Sep"; "Oct"; "Nov"; "Dec"|]
    //(21 - 3) / 2 + 3 = 12 => 3 == String.length months.[month - 1]
    printfn "%12s" months.[month - 1]
    //pt-BR
    //printfn " D  S  T  Q  Q  S  S"
    //en-US
    printfn " S  M  T  W  T  F  S"
    printRight (xss |> List.head)
    (xss |> List.tail) |> List.iter (fun xs -> printLeft xs)

[1..12] |> List.iter (fun i -> printCalendar(i, 2015)) 
