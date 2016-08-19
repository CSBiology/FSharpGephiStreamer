namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("FSharpGephiStreamer")>]
[<assembly: AssemblyProductAttribute("FSharpGephiStreamer")>]
[<assembly: AssemblyDescriptionAttribute("FSharp functions for streaming graph data to gephi a graph visualization tool")>]
[<assembly: AssemblyVersionAttribute("0.0.1")>]
[<assembly: AssemblyFileVersionAttribute("0.0.1")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "0.0.1"
    let [<Literal>] InformationalVersion = "0.0.1"
