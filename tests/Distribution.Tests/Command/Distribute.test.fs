module Distribution.Command.``Distribute test``

open System
open FsUnit.Xunit
open Xunit
open Distribution.Command

[<Fact>]
let ``Should scale`` () =
    let v = Decimal.Parse "76771.87313888" 

    let r = DistributeCommands.precision v
    v |> should equal 76771.87313888m
    r |> should equal 7677187313888I
