(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#I "../../bin/FSharpGephiStreamer/net47"

(**
Streaming a graph to gephi 
==========================
[Gephi](https://gephi.org/) enables the visualization and exploration of all kinds of graphs and networks.
The library FSharpGephiStreamer provides functions suitable for use from F# scripting to stream graph data 
to [gephi](https://gephi.org/).
*)
#r "FSharpGephiStreamer.dll"
open FSharpGephiStreamer

/// Record type that represents a custum node 
type MyNode = {
    Id    : int
    Label : string
    Size  : float
    Data  : string
    }

/// Create a custum node
let createMyNode id label size data =
    {Id=id; Label=label; Size=size; Data=data};    


type MyEdge = {
    Id :     int
    Source : int
    Target : int
    Weight : float    
    }

let createMyEdge id sourceId targetId weight =
    {Id=id; Source=sourceId; Target=targetId; Weight=weight;};    



let rnd = System.Random(42)

/// Returns orange or blue otherwise
let rndColor () = 
    match rnd.NextDouble() with
    | x when x <= 0.3 -> Colors.Table.Office.orange
    | _ -> Colors.Table.Office.blue



let addMyNode (node:MyNode) =

    let nodeConverter (node:MyNode) =
        [
            Grammar.Attribute.Label node.Label; 
            Grammar.Attribute.Size  node.Size; 
            Grammar.Attribute.Color (rndColor ()); 
            Grammar.Attribute.UserDef ("UserData",node.Data); 
        ]

    Streamer.addNode nodeConverter node.Id node



let addMyEdge (edge:MyEdge) =

    let edgeConverter (edge:MyEdge) =
        [
            Grammar.Attribute.Size  edge.Weight; 
            Grammar.Attribute.EdgeType  Grammar.EdgeDirection.Undirected;             
            Grammar.Attribute.Color Colors.Table.Office.grey ;      
        ]
    
    Streamer.addEdge edgeConverter edge.Id edge.Source edge.Target edge



let nodes =
    [|0..1000|] 
    |> Array.map (fun id -> 
                    createMyNode id (string id) 10. "userdef.data")



nodes
|> Array.map addMyNode
        

for i=0 to 1000 do
    for ii=i+1 to 1000 do        
        let tmpedge = 
            createMyEdge (i*i+ii) nodes.[i].Id nodes.[ii].Id 5.
        if rnd.NextDouble() <= 0.3 then
            addMyEdge tmpedge |> ignore




(**
![Demo](./img/gephiStreamingDemo.gif)
*)


Streamer.updateNode id 5 ([Grammar.Color Colors.Table.Office.darkYellow])