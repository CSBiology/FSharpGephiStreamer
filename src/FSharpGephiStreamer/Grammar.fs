namespace FSharpGephiStreamer

// https://github.com/gephi/gephi/wiki/GraphStreaming

// https://github.com/graphstream/gs-gephi

///Semantics and mappings to represent Gephi interpretable Attributes of Nodes and Edges
module Grammar =

    ///Action performed on the Gephi graph
    type Action = 
        | Add    
        | Change 
        | Delete 

    ///Node action performed on the Gephi graph
    let formatNodeAction= function
        | Add    -> "an"
        | Change -> "cn"
        | Delete -> "dn"
    
    ///Edge action performed on the Gephi graph
    let formatEdgeAction= function
        | Add    -> "ae"
        | Change -> "ce"
        | Delete -> "de"

    ///Direction of an edge
    type EdgeDirection =        
        | Directed
        | Undirected

        static member convert = function
            | Directed   -> true
            | Undirected -> false

        

    // TODO: LabelSize ? LabelColor ? LabelVisible
    ///Common attributes of nodes and edges
    type Attribute =        
        | Size         of float
        | Color        of Colors.Color

        | EdgeType     of EdgeDirection

        | PositionX    of float
        | PositionY    of float
        | PositionZ    of float

        | Label        of string 
//        | LabelSize    of float
//        | LabelColor   of Colors.Color
//        | LabelVisible of bool
        
        | UserDef of string * obj

    ///converts an attribute to a JSON token
    let attibuteToJsonProperty = function         
        | Size            v -> JsonObject.newJprop "size"  v    
        | Color           v -> JsonObject.newJprop "color" (Colors.toWebColor v)
        | EdgeType        v -> JsonObject.newJprop "directed"  (EdgeDirection.convert v)

        | PositionX       v -> JsonObject.newJprop "x" v
        | PositionY       v -> JsonObject.newJprop "y" v
        | PositionZ       v -> JsonObject.newJprop "z" v
                          
        | Label           v -> JsonObject.newJprop "label" v
//        | LabelSize       v -> JsonObject.newJprop "label size" v
//        | LabelColor      v -> JsonObject.newJprop "LabelColor" (Colors.toWebColor v)
//        | LabelVisible    v -> JsonObject.newJprop "Visible" v        

        | Attribute.UserDef (a,v)  -> JsonObject.newJprop a  v


    /// Returns a properly formatted JSON string for a node
    //{"an":{"1":{"label":"Test label","size": 50}}}
    let jsonFormatNode (action:Action) nodeId (attributes:Attribute list) =
         let jsonAttributes = attributes |> List.map attibuteToJsonProperty
         jsonAttributes
         |> JsonObject.newJObjOfSeq
         |> JsonObject.newJprop nodeId
         |> JsonObject.newJObj
         |> JsonObject.newJprop (formatNodeAction action)      
         |> JsonObject.newJObj
         |> JsonObject.toStringSingleLine



    /// Returns a properly formatted JSON string for an edge
    //"""{"ae":{"2":{"source":0,"target":1,"directed":true}}}"""
    let jsonFormatEdge (action:Action) edgeId sourceId targetId (attributes:Attribute list) =
         let jsonAttributes =         
            attributes
            |> List.map attibuteToJsonProperty
     
         (JsonObject.newJprop "source" sourceId)::
         (JsonObject.newJprop "target" targetId)     
            ::jsonAttributes
         |> JsonObject.newJObjOfSeq
         |> JsonObject.newJprop edgeId
         |> JsonObject.newJObj
         |> JsonObject.newJprop (formatEdgeAction action)      
         |> JsonObject.newJObj
         |> JsonObject.toStringSingleLine

