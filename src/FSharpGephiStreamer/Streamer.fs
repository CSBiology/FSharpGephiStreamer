namespace FSharpGephiStreamer

module Streamer =

    open RestfulAux        
    open Grammar
    open FSharp.Net

    /// Converter type to map from a custom node to a Grammar.Attribute list
    type NodeConverter<'node> = 'node -> Grammar.Attribute list
    
    /// Converter type to map from a custom edge to a Grammar.Attribute list 
    type EdgeConverter<'edge> = 'edge -> Grammar.Attribute list

    let mutable private envirmonmentURL = "http://localhost:8080/workspace1?operation=updateGraph"        

    ///change the url of the target streamer server (default:"http://localhost:8080/workspace1?operation=updateGraph")
    let setEnvirmonment ip port workspace = envirmonmentURL <- sprintf "http://%s:%i/%s/?operation=updateGraph" ip port workspace

    ///returns the url of the current target streamer server
    let getEnvirmonment () = envirmonmentURL    
        

    /// Adds a node to the gephi graph using the given node converters grammar attribute mapping
    let addNode (nodeConverter:NodeConverter<'node>) nodeId =
        let nodeId' = string nodeId
        (Either.tryCatch 
            (fun (node:'node) ->  (jsonFormatNode Action.Add nodeId' (nodeConverter node)))
            (fun exn  -> Error.CreateJsonFailed exn.Message))
        >> Either.bind (generatePostRequest (getEnvirmonment ()) )
        >> Either.bind evalHttpResponseText
        >> Either.either 
            (fun _     -> Success nodeId' )
            (fun error -> Failure error )

    /// Adds a node to the gephi graph given an id mapping (node -> nodeId)
    let addNodeBy (f:'node -> string) =                
        (Either.tryCatch 
            (fun node ->  (jsonFormatNode Action.Add (f node) [Grammar.Attribute.Size 1.0]))
            (fun exn -> Error.CreateJsonFailed exn.Message))
        >> Either.bind (generatePostRequest (getEnvirmonment ()) )
        >> Either.bind evalHttpResponseText
        >> Either.either 
            (fun s  -> Success s        )
            (fun error -> Failure error )
    
    /// Updates a node on the gephi graph using the given node converters grammar attribute mapping
    let updateNode (nodeConverter:NodeConverter<'node>) nodeId =
        let nodeId' = string nodeId
        (Either.tryCatch 
            (fun (node:'node) ->  (jsonFormatNode Action.Change nodeId' (nodeConverter node)))
            (fun exn  -> Error.CreateJsonFailed exn.Message))
        >> Either.bind (generatePutRequest (getEnvirmonment ()) )
        >> Either.bind evalHttpResponseText
        >> Either.either 
            (fun _     -> Success nodeId' )
            (fun error -> Failure error )


    /// Removes a node by nodeId
    let removeNode nodeId =
        let nodeId' = string nodeId
        (Either.tryCatch 
            (fun (node:'node) ->  (jsonFormatNode Action.Delete nodeId' []))
            (fun exn  -> Error.CreateJsonFailed exn.Message))
        >> Either.bind (generatePostRequest (getEnvirmonment ()) )
        >> Either.bind evalHttpResponseText
        >> Either.either 
            (fun _     -> Success nodeId' )
            (fun error -> Failure error )


    /// Adds an edge to the gephi graph using the given node converters grammar attribute mapping
    let addEdge (edgeConverter:EdgeConverter<'edge>) edgeId sourceId targetId  =
        let edgeId', sourceId', targetId' = (string edgeId), (string sourceId), (string targetId)
        (Either.tryCatch 
            (fun (edge:'edge) -> (jsonFormatEdge Action.Add edgeId' sourceId' targetId' (edgeConverter edge)))
            (fun exn  -> Error.CreateJsonFailed exn.Message))
        >> Either.bind (generatePostRequest (getEnvirmonment ()) )
        >> Either.bind evalHttpResponseText
        >> Either.either 
            (fun _     -> Success edgeId' )
            (fun error -> Failure error )

    /// Adds an edge given a mapping from edge to (edgeId*sourceId*targetId)
    let addEdgeBy (f:'edge -> string*string*string)  =
        
        (Either.tryCatch 
            (fun (edge:'edge) -> 
                let edgeId', sourceId', targetId' = f edge
                (jsonFormatEdge Action.Add (string edgeId') (string sourceId') (string targetId') []))
            (fun exn  -> Error.CreateJsonFailed exn.Message))
        >> Either.bind (generatePostRequest (getEnvirmonment ()) )
        >> Either.bind evalHttpResponseText
        >> Either.either 
            (fun s     -> Success s )
            (fun error -> Failure error )


    /// Update edge (source and target are immutable)
    // (source and targe are not needed) 
    let updateEdge (edgeConverter:EdgeConverter<'edge>) edgeId sourceId targetId  =
        let edgeId', sourceId', targetId' = (string edgeId), (string sourceId), (string targetId)
        (Either.tryCatch 
            (fun (edge:'edge) -> (jsonFormatEdge Action.Change edgeId' sourceId' targetId' (edgeConverter edge)))
            (fun exn  -> Error.CreateJsonFailed exn.Message))
        >> Either.bind (generatePutRequest (getEnvirmonment ()) )
        >> Either.bind evalHttpResponseText
        >> Either.either 
            (fun _     -> Success edgeId' )
            (fun error -> Failure error )


    /// Removes an edge by edgeId
    let removeEdge edgeId  =
        let edgeId'= string edgeId
        (Either.tryCatch 
            (fun (edge:'edge) -> (jsonFormatEdge Action.Delete edgeId' "-1" "-1" []))
            (fun exn  -> Error.CreateJsonFailed exn.Message))
        >> Either.bind (generatePostRequest (getEnvirmonment ()) )
        >> Either.bind evalHttpResponseText
        >> Either.either 
            (fun _     -> Success edgeId' )
            (fun error -> Failure error )


//    // {"dn":{"filter":"ALL"}}
//    let clearWorkspace () =
//        let callDn =
//            JsonObject.newJprop "filter" "ALL"
//            |> JsonObject.newJObj
//            |> JsonObject.newJprop "dn"
//            |> JsonObject.newJObj
//            |> JsonObject.toStringSingleLine
//
//        
//        (Either.tryCatch 
//            (fun (_) -> generatePostRequest (getEnvirmonment ()) callDn )  //(jsonFormatEdge Action.Change edgeId' sourceId' targetId' (edgeConverter edge)))
//            (fun exn  -> Error.CreateJsonFailed exn.Message))
        //>> Either.bind evalHttpResponseText
        
        


