[<RequireQualifiedAccess>]
module Distribution.Multisig

open Distribution.Types

open Nichelson
open Nichelson.Contract

let private targetEntrypoint = """(or %oracle
                (or (unit %confirm_oracle_migration)
                    (list %distribute (pair (address %to_) (nat %amount))))
                (address %migrate_oracle))"""

let private multisigAction =
    $"""(or :action
                    (pair %s{targetEntrypoint} address)
                    (pair (nat :threshold) (list key))))"""

let private multisigEntrypoint =
    $"""(pair
            (pair
                (nat :counter)
                %s{multisigAction})
            (list :sigs (option signature)))"""


let private signerPayload =
    $"""(pair
            (pair chain_id address)
            (pair nat %s{multisigAction})"""




let private signerPayloadType = ContractParameters signerPayload

let private tokenContractWithEp addr = $"%s{addr}%%oracle"

let packDistributeTokens
    { ChainId = chainId
      Address = multisigContract
      Counter = counter }
    { Address = tokenContract
      Distribution = distribution }
    =
    let distributionParameter =

        distribution
        |> Seq.map
            (fun (addr, amnt) ->
                Tuple [ Arg.StringArg addr
                        Arg.IntArg amnt ])
        |> Seq.toList

    let p =
        signerPayloadType.Instantiate(
            Tuple [ Arg.StringArg chainId
                    Arg.StringArg multisigContract
                    Arg.IntArg counter
                    Arg.LeftArg(
                        Tuple [ (Record [ ("%distribute", Arg.List distributionParameter) ])
                                Arg.StringArg(tokenContractWithEp tokenContract) ]
                    ) ]
        )

    Encoder.pack p

let distributionCall
    counter
    { Address = tokenContract
      Distribution = distribution }
    (signatures: string seq)
    =

    let template =
        sprintf """(Pair (Pair %A (Left (Pair (Left (Right { %s})) "%s"))) { %s })"""

    let signaturesP =
        signatures
        |> Seq.map (fun s -> $"Some \"%s{s}\"")
        |> String.concat ";"

    let distributionP =
        distribution
        |> Seq.map (fun (addr, amnt) -> $"Pair \"%s{addr}\" %A{amnt}")
        |> String.concat ";"

    template counter distributionP (tokenContractWithEp tokenContract) signaturesP
