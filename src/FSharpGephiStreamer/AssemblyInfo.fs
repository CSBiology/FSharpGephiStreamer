namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("FSharpGephiStreamer")>]
[<assembly: AssemblyProductAttribute("FSharpGephiStreamer")>]
[<assembly: AssemblyDescriptionAttribute("FSharp functions for streaming graph data to gephi a graph visualization tool")>]
[<assembly: AssemblyVersionAttribute("1.0.0")>]
[<assembly: AssemblyFileVersionAttribute("1.0.0")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "1.0.0"
    let [<Literal>] InformationalVersion = "1.0.0"
