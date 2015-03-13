//Sample provided by Fabio Galuppo  
//March 2015  

//"Sets and Set operations" BEGIN
let Sets () =
    let xs = set[4; 2; 1; 5; 7; 2; 1; 2]
    printfn "%A" xs

    let ys = set[8; 1; 2; 4; 9; 1; 2; 4]
    printfn "%A" ys

    let xs_intersect_ys = xs |> Set.intersect ys
    printfn "%A" xs_intersect_ys

    let ys_intersect_xs = ys |> Set.intersect xs
    printfn "%A" ys_intersect_xs

    printfn "%A" (xs_intersect_ys = ys_intersect_xs)

let CartesianProduct () =
    let chars = [| 'a'; 'b'; 'c' |]
    let nums = [| 1 .. 3 |]
    let chars_cross_nums = chars |> Seq.collect (fun c -> nums |> Seq.map(fun n -> (c, n))) |> Seq.toArray
    let nums_cross_chars = nums |> Seq.collect (fun n -> chars |> Seq.map(fun c -> (n, c))) |> Seq.toArray
    printfn "%A" chars_cross_nums
    printfn "%A" nums_cross_chars
//"Sets and Set operations" END

//"Relations" BEGIN
let Relation () =
    let Domain = [| "Mark Twain"; "Lewis Carroll"; "Charles Dickens"; "Stephen King" |]
    let CoDomain = [| "A Christmas Carol"; "Alice's Adventures in Wonderland"; "The Adventures of Tom Sawyer"; "The Left Hand of Darkness" |]
    printfn "%A" Domain
    printfn "%A" CoDomain
    
    let R = [(Domain.[0], CoDomain.[2]); (Domain.[1], CoDomain.[1]); (Domain.[2], CoDomain.[0])] |> Map.ofList
    printfn "%A" R

    let mappingMap dic op = dic |> Map.toSeq |> Seq.map (fun x -> op x)
    let values dic = mappingMap dic snd
    let keys dic = mappingMap dic fst
    let intersect B A = A |> Set.ofSeq |> Set.intersect(B |> Set.ofSeq) |> Set.toSeq

    let Image = CoDomain |> intersect (values R)
    let PreImage = Domain |> intersect (keys R)
    printfn "%A" Image
    printfn "%A" PreImage

    let R_inverse = R |> Map.toSeq |> Seq.map (fun x -> (snd x, fst x)) |> Map.ofSeq
    printfn "%A" R_inverse
//"Relations" END

//"Functions" BEGIN
let Functions () =
    let duplicate (x : byte (* Domain *)) : byte (* CoDomain *) = 
        let y = (2uy * x (* pre-image *))
        y (* image *)
        
    let duplicateWithDomain (x : byte, domain : seq<byte>) : byte =
        let y = (2uy * x) % (byte (domain |> Seq.length)) //modular arithmetic
        y
        
    printfn "%uy" (duplicate 128uy)

    let start_ = System.Byte.MinValue
    let end_ = System.Byte.MaxValue
    let System_Byte_Domain = seq { start_ .. end_ } 
    let Image = System_Byte_Domain |> Seq.map (fun x -> duplicate x) |> Set.ofSeq
    printfn "%A" Image
    
    let System_Byte_PreImage = System_Byte_Domain |> Seq.take 5
    let Image2 = System_Byte_Domain |> Seq.map (fun x -> duplicateWithDomain (x, System_Byte_PreImage)) |> Set.ofSeq
    printfn "%A" System_Byte_PreImage
    printfn "%A" Image2
//"Functions" END

//"Persistent and Non-Persistent Sets" BEGIN
type ContactType = string * string
let MoreSets () =
    let ImmutableUnion (contact : ContactType) (contacts : Set<ContactType>) : Set<ContactType> =
       contacts |> Set.union(set [contact])

    let MutableUnion (contacts : System.Collections.Generic.IList<ContactType>, contact : ContactType) : unit =
        if not (contacts.Contains(contact)) then
            contacts.Add(contact)

    let emptyContacts = Set.empty<ContactType>
    let domain1 =  emptyContacts |> ImmutableUnion ("xyz", "xyz@xyz.com")
    let domain2 =  domain1 |> ImmutableUnion ("abc", "abc@abc.com")
    let domain3 =  domain2 |> ImmutableUnion ("hello", "hello@world.com")
    let domain4 =  domain3 |> ImmutableUnion ("hello", "hello@world.com")
    printfn "%A" emptyContacts
    printfn "%A" domain1
    printfn "%A" domain2
    printfn "%A" domain3
    printfn "%A" domain4

    let contacts = new System.Collections.Generic.List<ContactType>();
    MutableUnion (contacts, ("xyz", "xyz@xyz.com"))
    MutableUnion (contacts, ("abc", "abc@abc.com"))
    MutableUnion (contacts, ("hello", "hello@world.com"))
    MutableUnion (contacts, ("hello", "hello@world.com"))
    printfn "%A" contacts

    contacts.[1] <- ("ops", "ops@ops.com")
    printfn "%A" contacts
//"Persistent and Non-Persistent Sets" END

//"Function composition" BEGIN
let Composition () =
    let compose = fun g f -> fun x -> g (f x)
    
    let f x = 2 * x 
    let g x = x + 1 

    let scope1 () =
        //h = g . f
        //let h = compose g f 
        let h = f >> g
        printfn "%d" (h(1))
        printfn "%d" (h(2))
    
    let scope2 () =
        //h = f . g
        //let h = compose f g
        let h = f << g
        printfn "%d" (h(1))
        printfn "%d" (h(2))

    let scope3 () =
        //f . (f . g)
        let h = f << (f << g)
        //(f . f) . g
        let i = (f << f) << g
        let isAssociative = h 10 = i 10
        printfn "Is function composition an associative binary operation? %b" isAssociative

    scope1 ()
    scope2 ()
    scope3 ()
//"Function composition" END

//"Inverse functions" BEGIN
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

let InverseFunctions () =
    let lg x = System.Math.Log(x, 2.) 
    let powerof2 x = 2. ** x 

    printfn "%f" (lg 8.) 
    printfn "%f" (powerof2 3.) 
    printfn "%f" (((lg) << (powerof2)) 8.) 
    printfn "%f" (((lg) >> (powerof2)) 3.)

    let msg0 = new Msg(a = "Hello World", b = 123, c = 456.789)
    printfn "%A" msg0

    let buf1 = Util.serializeToBuffer(msg0)
    printfn "%A" buf1

    let msg1 = Util.deserializeFromBuffer<Msg>(buf1)
    printfn "%A" msg1

    let msg2 : Msg = (Util.deserializeFromBuffer << Util.serializeToBuffer) msg0
    printfn "%A" msg2
//"Inverse functions" END 

let run = Util.Run

run (Sets, "Sets")
run (CartesianProduct, "CartesianProduct")
run (Relation, "Relation")
run (Functions, "Functions")
run (MoreSets, "MoreSets")
run (Composition, "Composition")
run (InverseFunctions, "InverseFunctions")

System.Console.ReadLine() |> ignore
