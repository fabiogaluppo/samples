//Sample provided by Fabio Galuppo 
//February 2015 

module Util

open System
open System.IO
open System.Runtime.Serialization.Json
open System.Text

let getSerializer<'a> () = new DataContractJsonSerializer(typedefof<'a>)

let serializeToJson<'a> (value : 'a) : string =
    let serializer = getSerializer<'a> ()
    let serializationStream = new MemoryStream()
    serializer.WriteObject(serializationStream, value)
    serializationStream.Position <- 0L
    use sr = new StreamReader(serializationStream)
    sr.ReadToEnd()

let deserializeFromJson<'a> (jsonString : string) : 'a = 
    let serializer = getSerializer<'a> ()
    use ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString))
    serializer.ReadObject(ms) :?> 'a

let serializeToBuffer<'a> (value : 'a) : byte array =
    let getBytes (jsonString : string) = Encoding.Unicode.GetBytes(jsonString)
    (getBytes << serializeToJson) value

let deserializeFromBuffer<'a> (buffer : byte array) : 'a =
    let getString buffer' = Encoding.Unicode.GetString(buffer') 
    (deserializeFromJson << getString) buffer
