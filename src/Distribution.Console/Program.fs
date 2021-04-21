open System
open Argu
open Distribution.App.Arguments
open Distribution.Command


let errorHandler =
    ProcessExiter(
        colorizer =
            function
            | ErrorCode.HelpText -> None
            | _ -> Some ConsoleColor.Red
    )

let parser =
    ArgumentParser.Create<ProgramArgs>(programName = "governance", errorHandler = errorHandler)

let distribute (command: DistributeArgs ParseResults) =
    match command.GetSubCommand() with
    | Payload v ->
        match command.TryGetResult Token_contract, v.TryGetResult Csv_file with
        | Some tokenContract, Some csvFile ->

            let payload =
                DistributeCommands.packPayload
                    { Address = v.GetResult Multisig_contract
                      ChainId = v.GetResult ChainId
                      Counter = command.GetResult Counter }
                    tokenContract
                    csvFile

            printfn $"%s{payload}"
        | _, _ ->
            (errorHandler :> IExiter)
                .Exit("Missing required arguments", ErrorCode.CommandLine)
    | Call v ->
        match command.TryGetResult DistributeArgs.Token_contract, v.TryGetResult DistributeCallArgs.Csv_file with
        | Some tokenContract, Some csvFile ->

            let payload =
                DistributeCommands.call
                    (command.GetResult DistributeArgs.Counter)
                    tokenContract
                    (v.GetResult Signatures)
                    csvFile


            printfn $"{payload}"
        | _ as t, _ as c ->
            (errorHandler :> IExiter)
                .Exit($"Missing required arguments %A{t} %A{c}", ErrorCode.CommandLine)
    | v -> failwith $"not implemented yet: %A{v}"

[<EntryPoint>]
let main argv =
    try
        let p =
            parser.Parse(
                inputs = argv,
                raiseOnUsage = true,
                configurationReader = ConfigurationReader.FromAppSettings()
            )

        match p.GetSubCommand() with
        | Distribute v -> distribute v
        | Admin _ -> printfn "a"

    with e -> printfn $"%s{e.Message}"

    0
