(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#I "../../bin"

(**
Streaming a graph to gephi 
==========================

[Gephi.](https://gephi.org/) enables the visualization and exploration of all kinds of graphs and networks.
The library FSharpGephiStreamer provides functions suitable for use from F# scripting to stream graph data 
to [gephi.](https://gephi.org/).

*)
#r "FSharpGephiStreamer.dll"
open FSharpGephiStreamer

/// Record type that represents a custum node 
type MyNode = {
    Id    : int
    Label : string
    Size  : int
    Group : int
    Data  : string
    }

/// Create a custum node
let createMyNode id label size group data =
    {Id=id; Label=label; Size=size; Group=group; Data=data};    


type MyEdge = {
    Id :     int
    Source : int
    Target : int
    Weight : int    
    }

let createMyEdge id sourceId targetId weight =
    {Id=id; Source=sourceId; Target=targetId; Weight=weight;};    

/// Returns a color according to group membership
let getColorByGroup g =
    match g with
    | v when v = 0 -> Colors.Table.Office.blue
    | _ -> Colors.Table.Office.orange



let addMyNode (node:MyNode) =

    let nodeConverter (node:MyNode) =
        [
            Grammar.Attribute.Label node.Label; 
            Grammar.Attribute.Size  node.Size; 
            Grammar.Attribute.Color (getColorByGroup node.Group); 
            Grammar.Attribute.UserDef ("UserData",node.Data); 
        ]

    Streamer.addNode nodeConverter node.Id node



let addMyEdge (edge:MyEdge) =

    let edgeConverter (edge:MyEdge) =
        [
            Grammar.Attribute.Size  edge.Weight; 
            Grammar.Attribute.Color Colors.Table.Office.yellow ;      
        ]
    
    Streamer.addEdge edgeConverter edge.Id edge.Source edge.Target edge


let rnd = System.Random(42)

let rndGroup () = 
    match rnd.NextDouble() with
    | x when x <= 0.3 -> 0
    | _ -> 1

let nodes =
    [|0..10|] 
    |> Array.map (fun id -> 
                    createMyNode id (string id) ((2+id)*10) (rndGroup()) "userdef.data")



nodes
|> Array.map addMyNode
        

for i=0 to 10 do
    for ii=i+1 to 10 do        
        let tmpedge = 
            createMyEdge (i*i+ii) nodes.[i].Id nodes.[ii].Id 20
        if rnd.NextDouble() <= 0.3 then
            addMyEdge tmpedge |> ignore







Streamer.updateNode id 7 ([Grammar.Color Colors.Table.Office.red])








