namespace Distribution.App.Arguments

open Argu

type DistributeCallArgs =
    | [<Mandatory>] Signatures of signature: string list
    | [<Mandatory>] Csv_file of path: string

    interface IArgParserTemplate with

        member this.Usage =
            match this with
            | Signatures _ -> "Signatures from multisig members"
            | Csv_file _ -> "CSV with distribution to apply"

type DistributePayloadArgs =
    | [<AltCommandLine("-cid"); Mandatory>] ChainId of string
    | [<AltCommandLine("-ms"); Mandatory>] Multisig_contract of string
    | [<Mandatory>] Csv_file of path: string

    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Multisig_contract _ -> "Multisig contract address"
            | ChainId _ -> "Targeted chain id"
            | Csv_file _ -> "Csv file with addresses + amounts"

type DistributeArgs =
    | [<AltCommandLine("-tc"); Mandatory>] Token_contract of address: string
    | [<AltCommandLine("-c"); Mandatory>] Counter of bigint
    | [<CliPrefix(CliPrefix.None)>] Payload of ParseResults<DistributePayloadArgs>
    | [<CliPrefix(CliPrefix.None)>] Call of ParseResults<DistributeCallArgs>


    interface IArgParserTemplate with

        member this.Usage =
            match this with
            | Token_contract _ -> "Token contract address"
            | Counter _ -> "Current contract counter"
            | Payload _ -> "Craft payload to sign"
            | Call _ -> "Create the multisig call"



type AdminArgs =
    | Threshold of int
    | Keys of keys: string list

    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Threshold _ -> "New threshold to set"
            | Keys _ -> "New public keys to set"

type ProgramArgs =
    | [<CliPrefix(CliPrefix.None)>] Distribute of ParseResults<DistributeArgs>
    | [<CliPrefix(CliPrefix.None)>] Admin of ParseResults<AdminArgs>

    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Distribute _ -> "Manage token distribution"
            | Admin _ -> "Change quorum"
