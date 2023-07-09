namespace Baggie.Test

open System
open System.Collections.Generic
open Baggie

type FakeTimeProvider (offsetSec: int) =
    let START_TIME = DateTime (2000, 1, 1)

    let current : DateTime =
        START_TIME + TimeSpan.FromSeconds(offsetSec)

    new () = FakeTimeProvider 0

    interface ITimeNowProvider with
        member this.Now = current

    member this.OffsetTime (sec: int) =
        current = current.AddSeconds(sec)

type FakeConfigProvider (timeoutStr: string) =
    interface IAppConfigProvider with
        member this.GetConfigValue (key: string) =
            match key with
            | "baggie.timeoutSec" -> timeoutStr
            | "discord.tokenpath" -> "/tmp/baggie.tok"
            | _ -> raise (KeyNotFoundException($"Invalid config key {key}"))
