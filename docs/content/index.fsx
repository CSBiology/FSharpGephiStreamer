(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#r "../../bin/Newtonsoft.Json.dll"

(**
FSharpGephiStreamer
===================

The library FSharpGephiStreamer provides functions suitable for use from F# scripting to stream graph data to [gephi.](https://gephi.org/).
Gephi is the leading visualization and exploration software for all kinds of graphs and networks. Gephi is open-source and free.
In order to use FSharpGephiStreamer functionality it is necessary to install gephi with the plugin [Graph Streaming](https://marketplace.gephi.org/plugin/graph-streaming/) installed and active in "Master mode". 

*)

#r "../../bin/FSharpGephiStreamer.dll"
open FSharpGephiStreamer

(**
Quick use
---------

The following example creates a combined point and line chart:
*)

let nodeList = [1..5]
let edgeList = [(1,2);(2,5);(1,4);(2,3)]

let nodeConverter nodeId =
    [Grammar.Attribute.Label (string nodeId);]


let edgeConverter edge =
    [ Grammar.Attribute.Color Colors.Table.Office.yellow ;]

let addNode node =
    Streamer.addNode nodeConverter node node
    |> ignore

let addEdge id edge =
    Streamer.addEdge edgeConverter id (fst edge) (snd edge) edge
    |> ignore


nodeList
|> List.iter addNode 


edgeList
|> List.iteri addEdge

