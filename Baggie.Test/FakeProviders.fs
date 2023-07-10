namespace Baggie.Test

open System
open System.Collections.Generic
open System.IO
open Baggie

type FakeTimeProvider (offsetSec: int) =
    let START_TIME = DateTime (2000, 1, 1)

    let mutable current : DateTime =
        START_TIME + TimeSpan.FromSeconds(offsetSec)

    new () = FakeTimeProvider 0

    interface ITimeNowProvider with
        member this.Now = current

    member this.Now = (this :> ITimeNowProvider).Now

    member this.OffsetTime (sec: int) =
        current <- current.AddSeconds(sec)

type FakeConfigProvider (timeoutStr: string) =
    interface IAppConfigProvider with
        member this.GetConfigValue (key: string) =
            match key with
            | "baggie.timeoutSec" -> timeoutStr
            | "discord.tokenpath" -> "/tmp/baggie.tok"
            | _ -> raise (KeyNotFoundException($"Invalid config key {key}"))

type FakeFileProvider (tokenPath: string, token: string) =
    interface IFileContentProvider with
        member this.Exists (path: string) =
            path = tokenPath

        member this.ReadAllText (path: string) =
            if path = tokenPath then token
            else raise (FileNotFoundException())
