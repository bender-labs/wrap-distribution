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

let distribute (p: ProgramArgs ParseResults) (command: DistributeArgs ParseResults) =
    let multisig = p.GetResult Multisig_contract
    let chainId = p.GetResult ChainId
    let tokenContract = p.GetResult Token_contract

    match command.GetSubCommand() with
    | Payload v ->
        match v.TryGetResult Csv_file with
        | Some csvFile ->
            let counter = command.GetResult Counter

            printfn
                $"""Preparing payload with
                    Multisig:{multisig}
                    ChainId:{chainId}
                    Token: {tokenContract}
                    Counter:{counter}
                    """

            let payload =
                DistributeCommands.packPayload
                    { Address = multisig
                      ChainId = chainId
                      Counter = counter }
                    tokenContract
                    csvFile

            printfn $"%s{payload}"
        | _ ->
            (errorHandler :> IExiter)
                .Exit("Missing CSV file", ErrorCode.CommandLine)
    | Call v ->
        match v.TryGetResult DistributeCallArgs.Csv_file with
        | Some csvFile ->

            let payload =
                DistributeCommands.call
                    (command.GetResult DistributeArgs.Counter)
                    tokenContract
                    (v.GetResult Signatures)
                    csvFile


            printfn $"{payload}"
        | c ->
            (errorHandler :> IExiter)
                .Exit($"Missing required arguments %A{c}", ErrorCode.CommandLine)
    | v -> failwith $"not implemented yet: %A{v}"


let configurationReader : IConfigurationReader =
    match System.Environment.GetEnvironmentVariable("env") with
    | null -> ConfigurationReader.FromAppSettings()
    | v -> ConfigurationReader.FromAppSettingsFile($"{v}.config")

[<EntryPoint>]
let main argv =
    try
        let p =
            parser.Parse(inputs = argv, raiseOnUsage = true, configurationReader = configurationReader)

        match p.GetSubCommand() with
        | Distribute v -> distribute p v
        | Admin _ -> printfn "a"
        | r -> printfn $"Not a command: {r}"

    with e -> printfn $"%s{e.Message}"

    0
