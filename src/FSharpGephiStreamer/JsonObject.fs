namespace FSharpGephiStreamer

///Helper functions to create and convert JSON objects/properties
module JsonObject =
    
    open Newtonsoft.Json.Linq

    let inline newJprop name (value:obj) =
        new JProperty(name, value) :> JToken

    let inline newJObj (value:JToken) =
        new JObject(value) :> JToken
    
    let inline newJObjOfSeq (value:JToken seq) =
        new JObject(value) :> JToken

    let toStringSingleLine (j:JToken) =
        let tmp = j.ToString(Newtonsoft.Json.Formatting.None)
        printfn "%s" tmp
        tmp

