(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#I "../../bin/FSharpGephiStreamer/net47"

(**
# FSharpGephiStreamer

FSharpGephiStreamer is intended to close the gap between F# and the functionality of the [Gephi software project](https://gephi.org/), integrating network visualization
power of gephi into any kind of data science workflow designed in F#. It leverages the functionality of the [graph streaming plugin](https://gephi.org/plugins/#/plugin/graphstreaming) of the It uses a short Grammar which makes it possible to convert any
kind of Object to nodes and any kind of relationship between these objects to edges of a graph. This is especially useful because network 
science is independent from specific data structures/types.

![WorkflowOverview](./img/workflowOverview.png)

# Ressources

## Installation

See how to [setup FSharpGephiStreamer](InstallationInstructions.html)

## Docs

 * See how to [convert any kind of nodes/edges](Grammar.html) to gephi readable objects using the `Grammar` module
 * See how to [stream graph data to gephi](Streaming.html)
 * Check out the [API Reference](https://csbiology.github.io/FSharpGephiStreamer/reference/index.html) for information about types/functions in this library

## Examples

 * [Exploratory analysis of the Gene Ontology knowledgebase](exampleAnalysis.html)

*)