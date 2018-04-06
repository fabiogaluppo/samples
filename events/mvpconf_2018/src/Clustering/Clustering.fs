//Sample provided by Fabio Galuppo
//March 2018

open System
open System.Collections.Generic

(* type definitions *)

type sortedset<'a> = System.Collections.Generic.SortedSet<'a>
type mutablelist<'a> = System.Collections.Generic.List<'a>

(* support functions *)

let list2sortedset(xs:'a list) =
    let ys = new sortedset<'a>()
    xs |> List.iter (fun x -> ys.Add(x) |> ignore)
    ys

let list2mutablelist(xs:'a list) =
    let ys = new mutablelist<'a>()
    xs |> List.iter (fun x -> ys.Add(x))
    ys

let mkString1 (xs: 'a seq) =
    let sb = new System.Text.StringBuilder()
    xs |> Seq.iter (fun x -> sb.Append(x) |> ignore)
    sb.ToString()

let mkString2 (xs: 'a seq, sep: string) =
    let N = Seq.length xs
    if (N = 0) then ""
    else
        let init (xs : 'a seq) : 'a seq = Seq.take (System.Math.Max(0, N - 1)) xs
        let sb = new System.Text.StringBuilder()
        xs |> init |> Seq.iter (fun x -> sb.AppendFormat("{0}{1}", x, sep) |> ignore)
        sb.Append(xs |> Seq.last).ToString()

let mkString3 (xs: 'a seq, _start: string, sep: string, _end: string) =
    System.String.Format("{0}{1}{2}", _start, mkString2(xs, sep), _end)

(* top-down merge *)

let mergeTopDown (xs : mutablelist<'a>) (mergeable: 'a * 'a -> bool) (merge: 'a * 'a -> 'a) =
    let mutable merged = true
    while (merged && (xs.Count > 1)) do
        merged <- false
        let mutable pivot = 1
        while (pivot < xs.Count) do
            let mutable i = pivot - 1
            let mutable _break = false
            while ((not _break) && i >= 0) do
                if (mergeable (xs.[i], xs.[pivot])) then
                    xs.[pivot] <- merge (xs.[i], xs.[pivot])
                    xs.RemoveAt(i)
                    merged <- true
                    pivot <- pivot - 1
                    _break <- true
                i <- i - 1
            pivot <- pivot + 1
    xs

(* concrete example  #1 *)

type Model = { 
    A: sortedset<string>;
    B: sortedset<string>;
    C: sortedset<string>
}

let strModel (x: Model) =
    let str xs = mkString3(xs, "{", ", ", "}")
    sprintf "%s %s %s" (str x.A) (str x.B) (str x.C)

let mergeable1 (lhs: Model, rhs: Model) =
    if (lhs.Equals(rhs)) then 
        true
    else
        let notSetEquals (lhs: sortedset<'a>) (rhs: sortedset<'a>) = 
            if lhs.SetEquals(rhs) then 0 else 1
        let exclusive = [rhs.A |> notSetEquals (lhs.A); 
                         rhs.B |> notSetEquals (lhs.B);
                         rhs.C |> notSetEquals (lhs.C)] 
                        |> List.reduce(fun acc x -> acc + x)
        exclusive <= 1

let merge1 (lhs: Model, rhs: Model) =
    let unionWith (lhs: sortedset<'a>) (rhs: sortedset<'a>) = rhs.UnionWith(lhs)
    let temp = { A = new sortedset<string>(rhs.A); B = new sortedset<string>(rhs.B); C = new sortedset<string>(rhs.C) }
    temp.A |> unionWith (lhs.A)
    temp.B |> unionWith (lhs.B)
    temp.C |> unionWith (lhs.C)
    temp

(* concrete example  #2 *)

type BookInfo = { 
    Author: sortedset<string>;
    Title: sortedset<string>    
}

let strBookInfo (x: BookInfo) =
    let str xs = mkString3(xs, "{", ", ", "}")
    sprintf "%s %s" (str x.Author) (str x.Title);;

let mergeableBooks (lhs: BookInfo, rhs: BookInfo) =
    if (lhs.Equals(rhs)) then
        true
    else
        let notSetEquals (lhs: sortedset<'a>) (rhs: sortedset<'a>) = 
            if lhs.SetEquals(rhs) then 0 else 1
        let exclusive = [rhs.Author |> notSetEquals (lhs.Author); 
                         rhs.Title  |> notSetEquals (lhs.Title)] 
                        |> List.reduce(fun acc x -> acc + x)
        exclusive <= 1

let mergeBooks (lhs: BookInfo, rhs: BookInfo) =
    let unionWith (lhs: sortedset<'a>) (rhs: sortedset<'a>) = rhs.UnionWith(lhs)
    let temp = { Author = new sortedset<string>(rhs.Author); Title = new sortedset<string>(rhs.Title) }
    temp.Author |> unionWith (lhs.Author)
    temp.Title  |> unionWith (lhs.Title)
    temp

let display xs ys f =
    printfn "before clustering:"
    xs |> Seq.iteri(fun i  x -> printfn "%4d: %s" (i+1) (f x))
    printfn "%s" (new String('-', 40))
    printfn "after clustering:"
    ys |> Seq.iteri(fun i  x -> printfn "%4d: %s" (i+1) (f x))

[<EntryPoint>]
let main argv =
    (* concrete example  #1 *)
    let model1 = { A = list2sortedset(["C";"D";"A"]); 
                   B = list2sortedset(["D";"B";"C"]); 
                   C = list2sortedset(["A";"C"]) }
    //let model1 = { A = list2sortedset(["C";"D";"A"]); 
    //               B = list2sortedset(["D";"B";"C"]); 
    //               C = list2sortedset(["A"]) }
    let model2 = { A = list2sortedset(["C";"D";"A"]); 
                   B = list2sortedset(["D";"B";"C"]); 
                   C = list2sortedset(["B"]) }
    let model3 = { A = list2sortedset(["C";"X";"A"]); 
                   B = list2sortedset(["D";"B";"C"]); 
                   C = list2sortedset(["C"]) }
    let model4 = { A = list2sortedset(["C";"X";"A"]); 
                   B = list2sortedset(["D";"B";"C"]); 
                   C = list2sortedset(["A"; "B"]) }

    display (seq[model1; model2; model3; model4])
            (mergeTopDown (list2mutablelist([model1; model2; model3; model4])) mergeable1 merge1)
            strModel

    (* concrete example  #2 *)
    let book1 = { Author = list2sortedset(["Donald E. Knuth"]); 
                  Title = list2sortedset(["The Art of Computer Programming: Volume 1"]) }
    let book2 = { Author = list2sortedset(["Donald E. Knuth"]); 
                  Title = list2sortedset(["The Art of Computer Programming: Volume 2"]) }
    let book3 = { Author = list2sortedset(["Donald E. Knuth"]); 
                  Title = list2sortedset(["The Art of Computer Programming: Volume 3"]) }
    let book4 = { Author = list2sortedset(["Donald E. Knuth"]); 
                  Title = list2sortedset(["Concrete Mathematics: A Foundation for Computer Science"]) }
    let book5 = { Author = list2sortedset(["Ronald L. Graham"]); 
                  Title = list2sortedset(["Concrete Mathematics: A Foundation for Computer Science"]) }
    let book6 = { Author = list2sortedset(["Oren Patashnik"]); 
                  Title = list2sortedset(["Concrete Mathematics: A Foundation for Computer Science"]) }
    let book7 = { Author = list2sortedset(["Bjarne Stroustrup"]); 
                  Title = list2sortedset(["A Tour of C++"]) }
    let book8 = { Author = list2sortedset(["Bjarne Stroustrup"]); 
                  Title = list2sortedset(["The C++ Programming Language"]) }

    //display (seq[book1; book2; book3; book4; book5; book6; book7; book8])
    //        //(mergeTopDown (list2mutablelist([book1; book2; book3; book4; book5; book6; book7; book8])) mergeableBooks mergeBooks)
    //        (mergeTopDown (list2mutablelist([book4; book5; book6; book1; book2; book3; book7; book8])) mergeableBooks mergeBooks)
    //        strBookInfo

    0