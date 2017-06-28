(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#r "../../bin/Newtonsoft.Json.dll"

(**
FSharpGephiStreamer
===================

The library FSharpGephiStreamer provides functions suitable for use from F# scripting to stream graph data to [gephi](https://gephi.org/).
Gephi is the leading visualization and exploration software for all kinds of graphs and networks. Gephi is open-source and free.
In order to use FSharpGephiStreamer functionality it is necessary to install gephi with the plugin [Graph Streaming](https://marketplace.gephi.org/plugin/graph-streaming/) installed and active in "Master mode". 

*)

#r "../../bin/FSharpGephiStreamer.dll"
open FSharpGephiStreamer

(**
Quick use
---------

The following example creates a simple graph:
*)

let nodeList = 
    [   1; 2; 3; 4; 5;  ]

let edgeList = 
    [   1,(1,2);
        2,(2,5);
        3,(1,4);
        4,(2,3);    ]



nodeList
|> List.map (Streamer.addNodeBy string) 


edgeList
|> List.map 
    (Streamer.addEdgeBy (fun (edgeId,(sourceId,targetId)) -> 
        string edgeId, string sourceId, string targetId ))
                

(**
![graph](./img/SimpleGraph.gif)

Update a node using `NodeConverter<'node>` : 

*)
 


let nodeConverter nodeId =
    [
        Grammar.Attribute.Label (string nodeId);
        Grammar.Attribute.Color Colors.Table.Office.blue;
    ]

nodeList
|> List.map (fun n -> (Streamer.updateNode nodeConverter n) n  )



(**
![graph](./img/updateGraph.gif)


*)






