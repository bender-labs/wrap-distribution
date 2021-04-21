module Distribution.Tests.``Multisig test``

open Distribution
open FsUnit.Xunit
open Nichelson
open Xunit

[<Fact>]
let ``Should pack token distribution`` () =
    let pack =
        Multisig.packDistributeTokens
            { ChainId = "NetXxkAx4woPLyu"
              Address = "KT1XVUAq9Tbasd5sAJYjma6m1sNWDMeZUCFT"
              Counter = 0I }
            { Distribution = [ ("tz1S792fHX5rvs6GYP49S1U58isZkp2bNmn6", 10000I) ]
              Address = "KT1L1xYJit22TmuhDXaeng4AZDhRqZwcacNj" }

    pack
    |> Encoder.byteToHex
    |> should
        equal
        "0x05070707070a00000004ed9d217c0a0000001601fb4a4ad0f1e0f317e08f0c2a20d2308477ad95a900070700000505070705050508020000002107070a00000016000046f146853a32c121cfdcd4f446876ae36c4afc5800909c010a0000001c017d6cf1aa4d81637fd36efc6a89b2d832b9a0a36e006f7261636c65"
