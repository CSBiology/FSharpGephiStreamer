(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#I "../../bin/FSharpGephiStreamer/net47"
#r "FSharpGephiStreamer.dll"
open FSharpGephiStreamer

(**
Streaming a graph to gephi 
==========================

First of all, you need to enable the master server of the graph streaming plugin in gephi (using default settings):

The next step is the creation of a workspace in Gephi. If you name it other than the default name (Workspace 1), 
make sure you adjust the streamer environment as shown [here](https://csbiology.github.io/FSharpGephiStreamer/InstallationInstructions.html#Streamer environment setup)

Given the following node/edge types and converters introduced in the [`Grammar` docs](Grammar.html)

*)
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

(**

There are several ways to stream data to gephi:

##Adding nodes & edges with node converters

*)

///Adds a node of type MyNode using the node converter to the gephi graph
let addMyNode (node:MyNode) = 
    Streamer.addNode nodeConverter node.Id node
    

///Adds an edge of type MyEdge using the edge converter to the gephi graph
let addMyEdge (edge: MyEdge) =
    Streamer.addEdge edgeConverter edge.Id edge.Source edge.Target edge

(**
##Adding nodes & edges with simple id mapping

When no specific attribute conversion is needed, the `addBy` functions only need a simple Id mapping:
*)

///Adds a node of type MyNode given a mapping from node type to an ID to the gephi graph
Streamer.addNodeBy (fun (node:MyNode) -> string node.Id)


///Adds an edge of type MyEdge given a mapping from edge type to an (ID,sourceId,targetID) tuple to the gephi graph
Streamer.addEdgeBy (fun (edge:MyEdge) -> (string edge.Id), (string edge.Source), (string edge.Target))

(**

##Update nodes & edges

The update functions are pretty similar to the `Streamer.addNode` functions. Note that you can use another converter to update all exisiting nodes in the gephi graph.

*)

///Updates a node of type MyNode using the node converter to the gephi graph.
let updateMyNode (node:MyNode) =
    Streamer.updateNode nodeConverter node.Id node

let updateMyEdge (edge:MyEdge) =
    Streamer.updateEdge edgeConverter edge.Id edge.Source edge.Target edge

(**

##Removing nodes & edges

*)

///Removes a node of type MyNode given a mapping from node type to an ID from the gephi graph
let removeMyNode (node:MyNode) =
    Streamer.removeNode node.Id node

///Removes an edge of type MyEdge given a mapping from edge type to an ID from the gephi graph
let removeMyEdge (edge:MyEdge) =
    Streamer.removeEdge edge.Id edge