[<RequireQualifiedAccess>]
module Distribution.Command.DistributeCommands

open Distribution
open Distribution.Types
open FSharp.Data
open Nichelson

type DistributionCsv = CsvProvider<"sample.csv", ";", ResolutionFolder=__SOURCE_DIRECTORY__>

let precision = bigint (pown 10 8)

let scale (v: int) = (bigint v) * precision

let private csvToDistribution (csv: string) =
    let data = DistributionCsv.Load csv

    data.Rows
    |> Seq.map (fun v -> (v.Address, scale v.Amount))
    |> Seq.toList

let packPayload (multisig: MultisigTarget) (tokenContract: string) (csv: string) =

    let distributions = csvToDistribution csv

    Multisig.packDistributeTokens
        multisig
        { Address = tokenContract
          Distribution = distributions }
    |> Encoder.byteToHex

let call counter tokenContract signatures csv =
    let distributions = csvToDistribution csv

    Multisig.distributionCall
        counter
        { Address = tokenContract
          Distribution = distributions }
        signatures
