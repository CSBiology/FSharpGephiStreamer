#light

#r "../../packages/Newtonsoft.Json.8.0.3/lib/net45/Newtonsoft.Json.dll"

#load "Color.fs"
#load "Either.fs"
#load "Http.fs"
#load "JsonObject.fs"
#load "RestfulAux.fs"
#load "Grammar.fs"
#load "Streamer.fs"

open FSharpGephiStreamer

type MyNode = {
    Id    : int
    Label : string
    Size  : int
    Group : int
    Data  : string
    }

let createMyNode id label size group data =
    {Id=id; Label=label  ; Size=size; Group=group; Data=data};    


type MyEdge = {
    Id :     int
    Source : int
    Target : int
    Weight : int    
    }

let createMyEdge id sourceId targetId weight =
    {Id=id; Source=sourceId; Target=targetId; Weight=weight;};    



let mapColorByGroup g =
    if g = 0 then
        Colors.Table.Office.blue
    elif g = 1 then 
        Colors.Table.Office.grey
    else 
        Colors.Table.Office.orange


let rnd = System.Random(42)

let rndfloatByMax max = 
    rnd.NextDouble() * max
    |> round
    

let rndIntByMax max = 
    rnd.NextDouble() * max
    |> round
    |> int

let nodeConverter (node:MyNode) =
    [Grammar.Attribute.Label node.Label; 
     Grammar.Attribute.Size  node.Size; 
//     Grammar.Attribute.PositionX (rndfloatByMax 100.); 
//     Grammar.Attribute.PositionY (rndfloatByMax 100.); 
     Grammar.Attribute.Color (mapColorByGroup node.Group); 
     Grammar.Attribute.UserDef ("UserData",node.Data); 
     ]


let edgeConverter (edge:MyEdge) =
    [Grammar.Attribute.Size  edge.Weight; 
     Grammar.Attribute.Color Colors.Table.Office.yellow ;      
     ]



let addMyNode (node:MyNode) =
    Streamer.addNode nodeConverter node.Id node

let addMyEdge (edge:MyEdge) =
    Streamer.addEdge edgeConverter edge.Id edge.Source edge.Target edge


//let nodes = [
//    {Id=1; Label="One"  ; Size=10; Group=1; Data="userdef."};
//    {Id=2; Label="Two"  ; Size= 5; Group=1; Data="userdef."};
//    {Id=3; Label="Three"; Size= 5; Group=0; Data="userdef."};
//    {Id=4; Label="Four" ; Size= 5; Group=0; Data="userdef."};
//    ]
//
//let edges = [
//    {Id=1; Source=1; Target=2; Weight=10;};
//    {Id=2; Source=2; Target=3; Weight=10;};
//    {Id=3; Source=4; Target=1; Weight=10;};
//    ]
//
//
//nodes |> List.map addMyNode
//edges |> List.map addMyEdge




let rndGroup () = 
    let g = rndIntByMax 10.
    match g with
    | x when x <= 3 -> 0
    | x when x <= 6 -> 1
    | _ -> 2


[0..10] 
|> List.map (fun id -> createMyNode id (string id) (rndIntByMax 10.) (rndGroup()) "userdef.data")
|> List.map addMyNode
        

[0..20] 
|> List.map (fun id -> createMyEdge id (rndIntByMax 10.) (rndIntByMax 10.) (rndIntByMax 10.) )
|> List.map addMyEdge


Streamer.updateEdge id 1 "" "" ([Grammar.Color Colors.Table.black])

Streamer.updateNode id 7 ([Grammar.Color Colors.Table.Office.red])


