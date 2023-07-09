namespace Baggie.Test

open Baggie

open DSharpPlus
open DSharpPlus.CommandsNext
open FsUnit
open NUnit.Framework

module TestBaggie =

    [<Test>]
    let Inst () =
        BaggieBot ()
        |> should not' (be Null)

    let makeBot (timeoutSec: int) =
        let time = FakeTimeProvider 0
        let bot = BaggieBot ()
        bot.AppConfig <- FakeConfigProvider timeoutSec
        bot.TimeProvider <- time
        (bot, time)

    let makeContext () =
        let client = new DiscordClient (DiscordConfiguration ())
        ()
