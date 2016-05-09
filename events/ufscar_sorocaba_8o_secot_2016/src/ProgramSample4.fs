//Sample provided by Fabio Galuppo 
//May 2016 

open System

type Rnd = { rnd : Random }
let createRndWithSeed (seed) = { rnd = new Random(seed) }
let inline precondition cond dueTo = 
    if (not cond) then 
        failwith ("precondition violation: " + dueTo)
//[0.0, 1.0)
let uniform (r : Rnd) = r.rnd.NextDouble()
//[0, n)
let uniformInt (r : Rnd) n = 
    precondition (n > 0) "less or equal than 0"
    r.rnd.Next(n)
//[n, m)
let uniformRange (r : Rnd) n m = 
    precondition (m > n) "invalid range"
    n + uniform r * (m - n)
//[n, m)
let uniformIntRange (r : Rnd) n m = 
    precondition (m > n) "invalid range"
    precondition (int64 (m - n) < int64 Int32.MaxValue) "invalid range"
    n + uniformInt r (m - n)
let shuffle (r : Rnd) (xs : 'a array) =
    precondition (xs <> null) "array is null"
    let n = xs |> Array.length
    for i = 0 to n - 1 do
        let j = i + uniformInt r (n - i)
        let temp = xs.[i]
        xs.[i] <- xs.[j]
        xs.[j] <- temp

//Magic 8 Ball (https://en.wikipedia.org/wiki/Magic_8-Ball)
let magic8Ball () =
    let magic8BallAnswers = 
        [|
            //positive
            "It is certain";
            "It is decidedly so";
            "Without a doubt";
            "Yes, definitely";
            "You may rely on it";
            "As I see it, yes";
            "Most likely";
            "Outlook good";
            "Yes";
            "Signs point to yes";
            //neutral
            "Reply hazy try again";
            "Ask again later";
            "Better not tell you now";
            "Cannot predict now";
            "Concentrate and ask again";        
            //negative
            "Don't count on it";
            "My reply is no";
            "My sources say no";
            "Outlook not so good";
            "Very doubtful"
        |]
    Console.Title <- "The wonderful Magic 8 Ball. Ask me something..."
    let rec loop (f : unit -> int) =
        printfn "Magic ball: What's your question?"
        printf "You: "
        let ask = Console.ReadLine()
        if String.IsNullOrEmpty(ask) then ()
        else 
            printfn "Magic ball: %s" magic8BallAnswers.[f ()]
            loop f
    let n = magic8BallAnswers |> Array.length
    let now = DateTime.Now
    let r = createRndWithSeed (now.Millisecond + now.Second * 1000)
    magic8BallAnswers |> shuffle r 
    loop (fun () -> uniformIntRange r 0 n)

magic8Ball ()

(*
    Magic ball: What's your question?
    You: Will I be rich?
    Magic ball: My sources say no
    Magic ball: What's your question?
    You: Will I be famous?
    Magic ball: Concentrate and ask again
    Magic ball: What's your question?
    You: Will I be famous?
    Magic ball: Cannot predict now
    Magic ball: What's your question?
    You: Will my team win the championship?
    Magic ball: Very doubtful
    Magic ball: What's your question?
    You: Will I win the first prize?
    Magic ball: Signs point to yes
    Magic ball: What's your question?
    You:
*)

(*
    Some questions:
    Will I be rich?
    Will I be famous?
    Will my team win the championship?
    Will I win the first prize?
    ...
*)