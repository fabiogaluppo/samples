//Sample provided by Fabio Galuppo 
//May 2016 

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
        | 2              -> if (0 = year % 4 && (0 = year % 400 || 0 <> year % 100)) 
                            then 29 else 28
        | 4 | 6 | 9 | 11 -> 30
        | _              -> 31

    let min (x: int32, y: int32) = System.Math.Min(x, y)
    let ld = lastDayOfMonth(month, year)
    let dw = 7 - dayOfWeek(month, year)
    //days of the month by week
    let xss = [[1..dw];
               [dw + 1..dw + 7];
               [dw + 8..dw + 14]; 
               [dw + 15..dw + 21]; 
               [dw + 22..min(ld, dw + 28)]; 
               [min(ld + 1, dw + 29)..ld]]

    //string helper functions
    let mask_builder mask = Printf.StringFormat<string -> string>(mask)
    let left n (s:string)  = sprintf (mask_builder ("%-" + (n.ToString()) + "s")) s
    let right n (s:string) = sprintf (mask_builder ("%" + (n.ToString()) + "s")) s
    let list2string xs = xs |> List.fold (fun acc x -> acc + (sprintf "%2d " x)) ""
    let toStringLn x = x |> List.fold (fun acc x -> acc + x + "\n") ""
    let stringFormat format xs = sprintf "%s" (format (xs |> list2string))

    //build calendar as string
    let h = xss |> List.head
    let ts = xss |> List.tail
    let months = [|"Jan"; "Fev"; "Mar"; "Abr"; "Mai"; "Jun"; "Jul"; "Ago"; "Set"; "Out"; "Nov"; "Dez"|]
    let title = [sprintf "%12s" months.[month - 1]; " D  S  T  Q  Q  S  S"]    
    let firstRow = [h |> stringFormat (right 21)]
    let remainingRows = ts |> List.collect (fun xs -> [xs |> stringFormat (left 21)])
    remainingRows |> List.append firstRow 
                  |> List.append title           
                  |> toStringLn

printCalendar (5, 2016) |> printfn "%s"
printCalendar (7, 2016) |> printfn "%s"


