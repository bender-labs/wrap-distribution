[<RequireQualifiedAccess>]
module Distribution.Command.DistributeCommands

open System
open Distribution
open Distribution.Types
open FSharp.Data
open Nichelson

[<Literal>]
let sample = __SOURCE_DIRECTORY__ + "/sample.csv"
type DistributionCsv = CsvProvider<sample>

let precision = decimal (pown 10 8) |> (*) >> bigint


let private csvToDistribution (csv: string) =
    let data = DistributionCsv.Load csv

    data.Rows
    |> Seq.map (fun v -> (v.Address, precision v.Amount))
    |> Seq.toList

let packPayload (multisig: MultisigTarget) (tokenContract: string) (csv: string) =

    let distributions = csvToDistribution csv

    let addresses, total =
        distributions
        |> Seq.fold (fun (addresses, total) (_, amount) -> (addresses + 1, total + amount)) (0, 0I)

    let payload =
        Multisig.packDistributeTokens
            multisig
            { Address = tokenContract
              Distribution = distributions }
        |> Encoder.byteToHex

    (addresses, (total |> decimal) / decimal (pown 10 8)), payload

let call counter tokenContract signatures csv =
    let distributions = csvToDistribution csv

    Multisig.distributionCall
        counter
        { Address = tokenContract
          Distribution = distributions }
        signatures
