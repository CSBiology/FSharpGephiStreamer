(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#I "../../bin/FSharpGephiStreamer/net47"
#r "FSharpGephiStreamer.dll"
open FSharpGephiStreamer

(**
Simple Network streaming example 
================================
This is a small "hello world" example building on the node/edge types and converters introduced in the [`Grammar` docs](Grammar.html). Adjust those types to your liking.

*)

open FSharpGephiStreamer

/// Record type that represents a custom node 
type MyNode = {
    Id    : int
    Label : string
    Size  : float
    Data  : string
    }

/// Record type that represents a custom edge 
type MyEdge = {
    Id :     int
    Source : int
    Target : int
    Weight : float    
    }

///converts a MyNode type to a list of grammar attributes
let nodeConverter (node:MyNode) =
    [
        Grammar.Attribute.Label node.Label; 
        Grammar.Attribute.Size  node.Size; 
        Grammar.Attribute.Color (Colors.Table.StatisticalGraphics24.getRandomColor()); 
        Grammar.Attribute.UserDef ("UserData",node.Data); 
    ]

///converts a MyEdge type to a list of grammar attributes
let edgeConverter (edge:MyEdge) =
    [
        Grammar.Attribute.Size  edge.Weight; 
        Grammar.Attribute.EdgeType  Grammar.EdgeDirection.Undirected;             
        Grammar.Attribute.Color Colors.Table.Office.grey ;      
    ]

///Adds a node of type MyNode using the node converter to the gephi graph
let addMyNode (node:MyNode) = 
    Streamer.addNode nodeConverter node.Id node
    

///Adds an edge of type MyEdge using the edge converter to the gephi graph
let addMyEdge (edge: MyEdge) =
    Streamer.addEdge edgeConverter edge.Id edge.Source edge.Target edge


(**
Now its time to deliver a message from F# to Gephi. The following simple network will consist of three nodes and 2 edges:
*)

let nodes =
    [
        {Id = 1; Label = "F# Says:" ; Size = 30.; Data = "FOO"}
        {Id = 2; Label = "Hello World" ; Size = 10.; Data = "BAR"}
        {Id = 3; Label = "To Gephi!" ; Size = 25.; Data = "FOOBAR"}
    ]

let edges =
    [
        {Id = 1; Source = 1 ; Target = 2; Weight = 10.}
        {Id = 2; Source = 2 ; Target = 3; Weight = 10.}
    ]

(**
To add the nodes/edges to the graph, we apply the prepared `addMyNode/addMyEdge` functions to the nodes. 
Note that the result is either a `Success` or `Failure` type, so you can add error handling.
The following blocks of code should return an empty list if streaming all nodes/edges is successfull or yield the resulting `RestfulAux.Error` type otherwise.

*)

(*** do-not-eval ***)

let nodeAddResult = 
    nodes
    |> List.choose 
        (fun node -> 
            match addMyNode node with
            |Success s -> None
            |Failure f -> 
                //You could also do some type of error handling here
                Some f
            )

let edgeAddResults =
    edges
    |> List.choose 
        (fun node -> 
            match addMyEdge node with
            |Success s -> None
            |Failure f -> 
                //You could also do some type of error handling here
                Some f
            )
(**
The resulting network should look like this:

![](./img/HelloWorld.png)

*)
