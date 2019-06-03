(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#I "../../bin/FSharpGephiStreamer/net47"
#I "../../lib/ForDocs"
#r "BioFSharp"
#r "BioFSharp.IO"
#r "FSharpAux" 
#r "FSharpAux.IO"
#r "FSharpGephiStreamer.dll"
open FSharpGephiStreamer

(**
# Introduction to Exploratory data analysis using FSharpGephiStreamer and Gephi

_23/1/2019 (applies to all site/dowload requests done for this document) ;_
_[Kevin Schneider](https://github.com/kMutagene)_

## Table of contents

 * [Introduction](#Introduction)
 * [The dataset](#The-dataset)
 * [Exploratory data analysis using FSharpGephiStreamer & Gephi](#Exploratory-data-analysis-using-FSharpGephiStreamer)
    * [Data aquisation](#Data-aquisation)
    * [Preparing nodes and edges](#Preparing-node-and-edges)
    * [Streaming to gephi](#Streaming-to-gephi)
 * [Results](#Results)
    * [The network](#The-network)
    * [Some metrics](#Some-metrics)
    * [Network sections](#Network-sections)

## Introduction

Exploratory data analysis is an essential part of data analysis, especially if you are working with large datasets. It is always helpful
to visualize your data to have an idea of the tendencies and structure of it. In the context of networks, 
gephi has proven to be a powerful tool various CSB projects for this exact purpose.

For the purpose of this tutorial/walkthrough, we will create a node and edge list from real data and stream those to gephi. 
Afterwards we will explore the resulting network a little.

However, this is **not** intended to be a guide on how to use gephi in general, although a few words will be said about the things done inside gephi to visualize the network.

_Note: It is currently planned to flesh out the analysis of the network to become a full blog post on our blog. A link will be added here when that is done._


## The dataset

In computer science and information science, an ontology encompasses a representation, formal naming, and definition of the categories, properties, and relations between the 
concepts, data, and entities that substantiate one, many, or all domains. Every field creates ontologies to limit complexity and organize information into data and knowledge. ([from wikipedia](https://en.wikipedia.org/wiki/Ontology_(information_science)))

Ontologies are providing an extensible and queryable knowledge base. In the context of computational biology, they are a valuable tool to characterize all 
kinds of biological processes and/or entities and are often used to see if specific types of these are enriched in an experiment (ontology enrichment).

The dataset of interest for this tutorial is the knowledgebase provided by the [Gene Ontology Consortium](http://www.geneontology.org/) (also known as GO). It provides concepts/classes used to describe 
gene function, and relationships between these concepts.

One of the main uses of the GO is to perform enrichment analysis on gene sets. For example, given a set of genes that are up-regulated under certain conditions, an enrichment analysis will find which GO terms are over-represented 
(or under-represented) using annotations for that gene set. ([from GO's website](http://geneontology.org/page/go-enrichment-analysis))

The full ontology can be downloaded [here](http://purl.obolibrary.org/obo/go.obo).


## Exploratory data analysis using FSharpGephiStreamer & Gephi

The data was originally parsed using the Obo parser from our bioinformatics toolbox [BioFSharp](https://github.com/CSBiology/BioFSharp). if you want to see the code , expand the section below. However, 
to avoid dependencies and assure reproducibility of this tutorial the data was also prepared to be usable without any dependency other than FSharpGephiStreamer itself.
The Node and Edgelists can be found as .csv files [here](https://github.com/CSBiology/FSharpGephiStreamer/tree/master/docsrc/content/data). 
If you want to reproduce this analysis, just parse these files and construct the node and edge types from them. Just keep in mind that you loose a lot of information contained in the obo file that way, 
as the csv files only contains term names and is-A relationships 

*)

(** 

### Preparing nodes and edges

We define nodes as GO terms as our nodes and edges as Is-A relations between those terms. This will result in a network that shows how the knowledgebase is structured. 
There are a few interesting thigs that can be visualized by this:

 * **The most descriptive terms**: The nodes with the highest In-Degree are the terms which describe the most knowledge in the network.
  Maybe we can also infere from this what the main fields of (geneomic) biological research are.
 * **Sub graphs of the network** may show that there are different well described knowledge types that are highly differentiaded from each other
 * **Connectivity between hubs**: Terms that connect subgraphs or hubs and act as 'knowledge glue'

However, there is much more information in the obi file than these relationships. Visualizing other relationships is a topic for another day.

*)

open FSharpGephiStreamer
open FSharpGephiStreamer.Colors

(***do-not-eval***)
/// Simplified GO Term as node
type GONode = { 
    //GO term (e.g. "GO:0000001")
    Id              : string
    //full term description e.g. "RNA polymerase I transcription factor complex"
    TermDescription : string
    //e.g. "biological process"
    NameSpace       : string
    //The color for the node
    Color           : Colors.Color
    }

/// Creates GONode
let createGONode id descr nameSpace col =
    {Id = id; TermDescription = descr; NameSpace = nameSpace ; Color = col}

/// Represents the Is_A relationship of GO terms as a directed edge
type GOEdge = {
    Id          : int
    Source      : string
    Target      : string
    TargetColor : Colors.Color
}

/// Creates GOEdge
let createGOEdge i source target tc = {Id = i; Source = source; Target = target; TargetColor = tc}


(**
###Data aquisition

First we parse the .obo file using BioFSharps Obo parser:

Parsing the csv files can be done without dependencies using this code:

<button type="button" class="btn" data-toggle="collapse" data-target="#CsvParse">Show csv parsing code</button>
<div id="CsvParse" class="collapse fileExamples ">
*)

open System.IO
open System.Text

let readFromFile (file:string) =
        seq {use textReader = new StreamReader(file, Encoding.Default)
             while not textReader.EndOfStream do
                 yield textReader.ReadLine()}

let nodes = 
    readFromFile (__SOURCE_DIRECTORY__ + "/data/goNodeList.csv") 
    |> List.ofSeq
    //Skip the header line of the csv file
    |> List.skip 1
    |> List.map (fun n -> let tmp = n.Split([|','|])
                          createGONode tmp.[0] tmp.[1] tmp.[2] (Colors.Table.StatisticalGraphics24.getRandomColor())) 

let edges = 
    readFromFile (__SOURCE_DIRECTORY__ + "/data/goEdgeList.csv") |> List.ofSeq
    |> List.ofSeq
    //Skip the header line of the csv file
    |> List.skip 1
    |> List.map (fun n -> let tmp = n.Split([|','|])
                          createGOEdge (tmp.[0] |> int) tmp.[1] tmp.[2] (nodes |> List.find(fun n -> n.Id = tmp.[2])).Color) // this will take some time but ensures that edges have the same color as target nodes.


(**

</div>

*)


open FSharpAux
open FSharpAux.IO
open BioFSharp
open BioFSharp.IO
open BioFSharp.IO.Obo

let readFile path =
    FileIO.readFile path
    |> Obo.parseOboTerms
    |> Seq.toList

(***do-not-eval***)
let goObo = readFile @"go.obo"



///Node list containing all GO terms
let goNodes =
    goObo
    |> List.map (fun x -> (createGONode x.Id x.Name x.Namespace (Colors.Table.StatisticalGraphics24.getRandomColor())))


///Edge list containing all Is-A relationships in the knowledge base
let goEdges =
    goObo
    //ignore terms that have no Is-A relationship to any term
    |> List.filter (fun x -> not (List.isEmpty x.IsA))
    |> List.map (fun x -> 
                            [for target in x.IsA do 
                                yield ( x.Id, 
                                        target , 
                                        //ensure edges have the color of the node they target;
                                        (goNodes |> List.find(fun node -> node.Id = target) ).Color)
                                        ])
    //Aggregate resulting edges in a single list
    |> List.concat
    |> List.mapi (fun i (sourceId, targetId, col) -> createGOEdge (i+1) sourceId targetId col)

(** 
### Streaming to gephi

The `Grammar` module provides a set of rules that will convert the streamed data into JSON objects which gephi will understand.
To stream the nodes and edges to Gephi, we need a converter function for both. This function will take:
 * The edge/node
 * A list of Grammar attributes that define the mapping of attributes of the data to Gephi-readable attributes

We then use functional compostion with the `Streamer.addNode`/`Streamer.addEdge` functions to create our final addNode/Edge functions.

*)

(***do-not-eval***)
let addOboNode (node:GONode) =

    let nodeConverter (node:GONode) =
        [
            Grammar.Attribute.Label (sprintf "%s | %s {%s}" node.Id node.TermDescription node.NameSpace); 
            Grammar.Attribute.Size  10.; 
            Grammar.Attribute.Color node.Color; 
            Grammar.Attribute.UserDef ("UserData",node.TermDescription); 
        ]
    Streamer.addNode nodeConverter node.Id node


let addOboEdge (edge:GOEdge) =

    let edgeConverter (edge:GOEdge) =
        [
            Grammar.Attribute.Size  1.; 
            Grammar.Attribute.EdgeType  Grammar.EdgeDirection.Directed;             
            Grammar.Attribute.Color edge.TargetColor ;      
        ]
    
    Streamer.addEdge edgeConverter edge.Id edge.Source edge.Target edge

(**
To stream the nodes and edges to gephi, we use the addNode/addEdge functions on the list of nodes/edges:
*)

(***do-not-eval***)
goNodes |> List.map addOboNode
goEdges |> List.map addOboEdge

(**
Alternatively, when using the data parsed from the provided csv files:
*)

(***do-not-eval***)
nodes |> List.map addOboNode
edges |> List.map addOboEdge

(**
Thats it. in roughly 40 lines of code we streamed a complete knowledge graph with 47345 nodes and 77187 edges to gephi. The network is now ready to be explored.

*)

(**

# Results

## The network

After applying some styles in the preview section (e.g. black background, rounded edges) the final rendered network looks like this:

![](./img/OboNetworkOverview.png)

## Network sections

By eye, there are 9 large communities in the network, clustering knowledge about the following processes/entities (click to view a close-up):

<img src="./img/OboNetworkMeta.png" usemap="#NetworkMap">
<map name="NetworkMap">
    <!-- Image Map Generated by http://www.image-map.net/ -->
    <area target="" alt="Binding"                   title="Binding"                 href="#Binding"                 coords="127,192,115" shape="circle">
    <area target="" alt="Transferases"              title="Transferases"            href="#Transferases"            coords="276,81,343,31,433,3,488,28,461,116,374,171,298,179,264,147,261,112" shape="poly">
    <area target="" alt="Regulation"                title="Regulation"              href="#Regulation"              coords="469,823,139" shape="circle">
    <area target="" alt="Protein-Complex"           title="Protein-Complex"         href="#Protein-Complex"         coords="468,207,62" shape="circle">
    <area target="" alt="Metabolic-Processes"       title="Metabolic-Processes"     href="#Metabolic-Processes"     coords="125,608,88,628,44,669,41,740,89,795,180,821,248,817,308,778,336,727,328,659,283,623,219,602,170,601" shape="poly">
    <area target="" alt="Oxireductases"             title="Oxireductases"           href="#Oxireductases"           coords="571,267,532,262,498,280,492,299,507,350,540,423,607,498,676,558,749,594,782,598,800,584,806,558,793,505,733,404,656,323" shape="poly">
    <area target="" alt="Intracellular-Transport"   title="Intracellular-Transport" href="#Intracellular-Transport" coords="482,527,533,485,576,486,596,503,602,524,606,553,596,594,565,632,533,651,490,655,464,632,459,584,469,555" shape="poly">
    <area target="" alt="Reproduction"              title="Reproduction"            href="#Reproduction"            coords="600,610,640,587,684,584,719,610,724,643,709,695,675,717,621,732,576,709,569,676,574,643" shape="poly">
    <area target="" alt="Transmembrane-Transport"   title="Transmembrane-Transport" href="#Transmembrane-Transport" coords="622,131,79" shape="circle">
</map>

## Metrics

###Average Degree & Degree distribution

![](./img/AverageDegree.jpg)

The average Degree is 1.63. The degree distribution is highly right-skewed (meaning many nodes have a low degree, and there exist hubs with high degree). This is a property of a real network.

###Modularity

Calculating the network modularity with a low resolution, the large communities correlate well with the previous by-eye observation, although some of these communities split into large sub-communities:
The overall modularity of the network with a resolution of 3 is 0.89 (high modularity). 

Below is the network with nodes colored by their community membership:

![](./img/Communities.png)

# Close up of some communities

## Binding

<a href="#Network-sections">Back to overview</a>

![](./img/BindingNetwork.png)

## Transferases

<a href="#Network-sections">Back to overview</a>

![](./img/TransferasesNetwork.png)

## Regulation

<a href="#Network-sections">Back to overview</a>

![](./img/RegulationNetwork.png)

## Protein-Complex

<a href="#Network-sections">Back to overview</a>

![](./img/ProteinComplexNetwork.png)

## Metabolic-Processes

<a href="#Network-sections">Back to overview</a>

![](./img/MetabolicNetwork.png)

## Oxireductases

<a href="#Network-sections">Back to overview</a>

![](./img/OxireductasesNetwork.png)

## Intracellular-Transport

<a href="#Network-sections">Back to overview</a>

![](./img/TransportNetwork.png)

## Reproduction

<a href="#Network-sections">Back to overview</a>

![](./img/DevelopmentNetwork.png)

## Transmembrane-Transport

<a href="#Network-sections">Back to overview</a>

![](./img/TransmembraneTransportNetwork.png)

*)