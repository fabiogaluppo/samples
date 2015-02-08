//Sample provided by Fabio Galuppo 
//February 2015 

module SerializationProgram

open System.Runtime.Serialization

[<DataContract>]
type Msg(a : string, b : int32, c : float) =
    let mutable _A = a
    let mutable _B = b
    let mutable _C = c
    [<DataMember>]
    member x.A with get() = _A and set(value) = _A <- value
    [<DataMember>]
    member x.B with get() = _B and set(value) = _B <- value
    [<DataMember>]
    member x.C with get() = _C and set(value) = _C <- value
    override x.ToString() = sprintf "{A: \"%s\", B: %d, C: %f}" _A _B _C

let test1 () =
    let msg0 = new Msg(a = "Hello World", b = 123, c = 456.789)
    printfn "%A" msg0

    let buf1 = Util.serializeToBuffer(msg0)
    printfn "%A" buf1

    let msg1 = Util.deserializeFromBuffer<Msg>(buf1)
    printfn "%A" msg1

    let msg2 : Msg = (Util.deserializeFromBuffer << Util.serializeToBuffer) msg0
    printfn "%A" msg2