(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#I "../../bin/FSharpGephiStreamer/net47"
#r "FSharpGephiStreamer.dll"
open FSharpGephiStreamer
(**
# Using FSharpGephiStreamer's `Grammar` module to prepare nodes and edges for streaming

The `Grammar` module provides a short set of semantics that allow the conversion of any node/edge type to gephi readable objects.

Suppose you have the following types for your nodes and edges
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

(**

The attributes currently supported are:

 * Size         
 * Color        
 * EdgeType     
 * PositionX    
 * PositionY    
 * PositionZ    
 * Label        
 * UserDef 

This means you have full control over the size, position, color ,and labels of the graph. You could even use your own layout algorithms without being reliant on the ones implemented in gephi.

All you need is a converter function that returns a list of Grammar attributes. The following example does exactly that for 
the label, size, color, and data of the `MyNode` type and the size, edge type, and color of the `MyEdge` type:
*)

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
