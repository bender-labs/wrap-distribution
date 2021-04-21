namespace Distribution.Types

type MultisigTarget =
    { Address: string
      ChainId: string
      Counter: bigint }

type Distribution = (string * bigint) list

type DistributionParameters =
    { Address: string
      Distribution: Distribution }
