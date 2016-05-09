//Sample provided by Fabio Galuppo 
//May 2016 

let tid () = System.Threading.Thread.CurrentThread.ManagedThreadId
let sleep (ms : int) = System.Threading.Thread.Sleep(ms)

let accAgent = 
    let receiver (inbox : MailboxProcessor<int>) = 
        let fmt x = sprintf "[%d] receiving : %3d" (tid()) x
        let rec loop x =
            async {
                do printfn "%s" (fmt x)
                let! msg = inbox.Receive()
                return! loop(x + msg)
            }
        loop 0
    MailboxProcessor.Start(receiver)

let post x = 
    let fmt x = sprintf "[%d] sending   : %3d" (tid()) x
    printfn "%s" (fmt x)
    accAgent.Post(x)    

post 1
post 1
post 1
post -1
post -1

System.Console.ReadLine() |> ignore

type IAgent<'msg> =
   abstract member Inbox : MailboxProcessor<'msg * IAgent<'msg>>
   abstract member Send : 'msg * IAgent<'msg> -> unit

type Msg =
    | Stop
    | Ping of int
    | Pong of int

let send msg receiver sender =
        sleep (1000)
        (sender :> IAgent<Msg>).Send(msg, receiver)

type PingAgent() as self =
    let receiver (inbox : MailboxProcessor<Msg * IAgent<Msg>>) =         
        let rec loop () = 
            async {
                let! (msg, actor) = inbox.Receive()
                match msg with                
                | Pong(n) -> 
                    printfn "[%d] Ping received pong : %d" (tid()) n
                    if n > 1 then
                        self |> send (Ping (n - 1)) actor
                        return! loop ()
                    else
                        printfn "[%d] Ping finished" (tid())
                        self |> send Stop actor
                        return ()
                | _ -> return! loop ()
            }
        loop () 
    let inbox = MailboxProcessor.Start(receiver)    
    interface IAgent<Msg> with
          member this.Inbox = inbox
          member this.Send(msg, actor) = actor.Inbox.Post(msg, upcast this)         

type PongAgent() as self =
    let receiver (inbox : MailboxProcessor<Msg * IAgent<Msg>>) = 
        let rec loop () = 
            async {
                let! (msg, actor) = inbox.Receive()
                match msg with
                | Stop -> 
                    printfn "[%d] Pong finished" (tid())         
                    return ()
                | Ping(n) -> 
                    printfn "[%d] Pong received ping : %d" (tid()) n
                    self |> send (Pong n) actor
                    return! loop ()
                | _ -> return! loop ()
            }
        loop () 
    let inbox = MailboxProcessor.Start(receiver)    
    interface IAgent<Msg> with
          member this.Inbox = inbox
          member this.Send(msg, actor) = actor.Inbox.Post(msg, upcast this)

let ping = new PingAgent()
let pong = new PongAgent()

ping |> send (Ping 3) pong

System.Console.ReadLine() |> ignore