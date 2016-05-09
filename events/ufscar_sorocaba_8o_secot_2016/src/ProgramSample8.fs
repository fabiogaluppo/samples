//Sample provided by Fabio Galuppo 
//May 2016 

open System
open FSharp.Data

let NUMBER_OF_PAGES = 9
let NUMBER_OF_PROGRAMS = 58

type RockOn = HtmlProvider<"https://programarockon.com.br/">

type Data = { ProgramsAndTracks : seq<string * string>; Bands : seq<string> }

let dumpPage (xs : HtmlNode) =
    let hasProgram (x : string) =
        x.StartsWith("Programa Rock ON")        
    let hasBand (x : string) = 
        x.StartsWith("Faixas") ||
        x.StartsWith("Entrevista") ||
        x.StartsWith("Bandas")
    let textWithoutPeriod (x : string) =
        let idx = x.LastIndexOf('.')
        if idx = -1 then x
        else x.Substring(0, idx)
    let textWithoutSentence (text : string) (x : string) =
        let idx = x.LastIndexOf(text)
        if idx = -1 then x
        else x.Substring(0, idx)
    let ys = xs.Elements() 
                |> List.filter (fun x -> x.Name() = "div" && x.HasAttribute("id", "page"))
                |> List.collect (fun x -> x.Descendants() |> Seq.toList)
                |> Seq.filter (fun x -> x.Name() = "div" && x.HasAttribute("id", "content"))
                |> Seq.collect (fun x -> x.Descendants())
                |> Seq.filter (fun x -> x.Name() = "div" && x.HasAttribute("id", "primary"))
                |> Seq.collect (fun x -> x.Descendants())
                |> Seq.filter (fun x -> x.Name() = "main" && x.HasAttribute("id", "main"))
                |> Seq.collect (fun x -> x.Descendants())
                |> Seq.filter (fun x -> x.Name() = "article")
                |> Seq.collect (fun x -> x.Descendants())
                |> Seq.filter (fun x -> x.Name() = "div" && x.HasAttribute("class", "entry-content"))
                |> Seq.collect (fun x -> x.Descendants())
                |> Seq.filter (fun x -> (x.Name() = "p" || x.Name() = "div") && 
                                        (x.InnerText() |> hasProgram || x.InnerText() |> hasBand))
                |> Seq.map (fun x -> x.InnerText())
    let tracks = ys |> Seq.filter hasBand 
                    |> Seq.map (fun x -> x |> textWithoutSentence "Apresen")                    
    let programs = ys |> Seq.filter hasProgram 
                      |> Seq.map (fun y -> y.Substring(0, min (y.Length) 20))
    let bands (x : string) =
        let split n = 
            let ys = x.Split([| ',' |], System.StringSplitOptions.RemoveEmptyEntries) 
            ys.[0] <- ys.[0].Substring(n)
            ys
        seq {
            if x.StartsWith("Faixas") then
               for y in (split 8) -> y.Trim()
            else if x.StartsWith("Bandas") then
               for y in (split 8) -> y.Trim() 
        }
    let uniqueBands = tracks |> Seq.collect (fun x -> bands x)                             
                             |> Seq.map (fun x -> x |> textWithoutPeriod)
                             |> Seq.map (fun x -> x.ToUpper())
                             |> Seq.sort 
                             |> Seq.distinct
    { ProgramsAndTracks = Seq.zip programs tracks; Bands = uniqueBands }
    
let dumpPages n = 
    seq {
        for i = 1 to n do
            let instanceRockOn = RockOn.Load("https://programarockon.com.br/page/" + (string i) + "/")
            let xs = instanceRockOn.Lists.Html.Body()
            yield dumpPage xs
    }

let allPages = dumpPages NUMBER_OF_PAGES

let programsAndBands = allPages |> Seq.collect (fun x -> x.ProgramsAndTracks) 
            
let uniqueBands = allPages |> Seq.collect (fun x -> x.Bands) 
                           |> Seq.sort 
                           |> Seq.distinct
                           |> Seq.toList

printfn "--------------------"
//All programs
//for (program, _) in programsAndBands do
//    printfn "%s" program  
//printfn "--------------------"
//All programs with bands
//for x in programsAndBands do
//    printfn "%A" x
//printfn "--------------------"
let queryProgram () =
    let getProgram (n : int) = 
        let pred (x : string) k = x.Equals(sprintf "Programa Rock ON #%d" k)
        programsAndBands |> Seq.tryFind (fun (p, _) -> pred p n)
    let rec loop () =
        printf "Programa #"
        let line = System.Console.ReadLine()
        let mutable p = 0
        if System.Int32.TryParse(line, &p) then
            if 1 <= p && p <= NUMBER_OF_PROGRAMS then
                printfn "%A" (getProgram p)
                printfn "--------------------"
                loop ()    
    loop ()
queryProgram ()
printfn "--------------------"
printfn "# of Bands = %d\n List of Bands:" (uniqueBands |> List.length)
for x in uniqueBands do
    printf "[%s] " x
printfn ""
printfn "--------------------"

