//http://elemarjr.net/2015/10/29/um-active-pattern-para-reconhecer-nomes-validos-de-variaveis-ele-poderia-ser-mais-simples/

let (|Id|_|) (s : System.String) =
    let l_  ch = ch = '_' || System.Char.IsLetter(ch)
    let ld_ ch = ch = '_' || System.Char.IsLetterOrDigit(ch)
    let rec split (i, j) =
        if (i < j) then
            if (ld_ s.[i]) then 
                split (i + 1, j)
            else i
        else j
    if (l_ s.[0]) then
        let len = String.length s
        let index = split (1, len)
        if len = index then
            Some(s, System.String.Empty)
        else
            Some(s.Substring(0, index), s.Substring(index))
    else
        None

let a = "identifier = value" |> (|Id|_|)
let b = "a2 = value" |> (|Id|_|)
let c = "_myVariable = value" |> (|Id|_|)

printfn "%A" a 
printfn "%A" b
printfn "%A" c
