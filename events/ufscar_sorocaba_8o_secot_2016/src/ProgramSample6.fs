//Sample provided by Fabio Galuppo 
//May 2016 

open System
open FSharp.Data

type Wiki_1982_FIFA_World_Cup = HtmlProvider<"https://en.wikipedia.org/wiki/1982_FIFA_World_Cup">
let instance1982FWC = Wiki_1982_FIFA_World_Cup.GetSample()
let retrospective1982 = instance1982FWC.Tables.``FIFA retrospective ranking``

type Wiki_1994_FIFA_World_Cup = HtmlProvider<"https://en.wikipedia.org/wiki/1994_FIFA_World_Cup">
let instance1994FWC = Wiki_1994_FIFA_World_Cup.GetSample()
let retrospective1994 = instance1994FWC.Tables.``Final standings``

let retrospective = retrospective1982 
//let retrospective = retrospective1994 

let print_separator () = printfn "%s" (new String('-', 40))

//raw data
printfn "Raw Data:"
for x in retrospective.Rows do
    printfn "%A" x
print_separator ()

printfn "Filtered Raw Data:"
let data = retrospective.Rows |> Array.filter (fun x -> not (x.R.Contains("Eliminated")))
for x in data do
    printfn "%A" x
print_separator ()

//ranking table
printfn "Ranking Table:"
printfn "%-2s %-19s %3s %2s %2s %2s %2s %-2s %-2s %3s %-3s" "R" "Team" "G" "P" "W" "D" "L" "GF" "GA" "GD" "Pts."
for x in data do
    printfn "%2s %-19s %3s %2s %2s %2s %2s %2s %2s %3s %3s" x.R x.Team x.G x.P x.W x.D x.L x.GF x.GA x.GD x.``Pts.``
print_separator ()

//transforming in more specialized data
type RankInfo = { team : String; goalsFor : int8; goalsAgainst : int8 }
let rank = data |> Array.map (fun x -> { team = x.Team; 
                                         goalsFor = Convert.ToSByte(x.GF);  
                                         goalsAgainst = Convert.ToSByte(x.GA) })

let gd_fun (x : RankInfo) = x.goalsFor - x.goalsAgainst

//sorting by GD   
let topGD = rank |> Array.sortBy (fun x -> -gd_fun x (* - for descending *)) 
                 |> Array.toSeq 
                 |> Seq.map (fun x -> (x.team, Convert.ToInt32(gd_fun x)))

//top 10 GD
printfn "Top 10 in Goal Difference:"
for x in (topGD |> Seq.take 10) do
    printfn "%-16s %3d" (fst x) (snd x)
print_separator ()

//sorting by GF
let topGF = rank |> Array.sortBy (fun x -> -x.goalsFor)
                 |> Array.toSeq

//top 10 GF
printfn "Top 10 in Goals For:"
for x in (topGF |> Seq.take 10) do
    printfn "%-16s %3d" x.team x.goalsFor
