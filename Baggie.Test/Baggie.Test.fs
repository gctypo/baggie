namespace Baggie.Test

open Baggie

open DSharpPlus
open FsUnit
open NUnit.Framework

module TestBaggie =

    let makeBotStr (timeoutStr: string) =
        let time = FakeTimeProvider 0
        let bot = BaggieBot ()
        bot.AppConfig <- FakeConfigProvider timeoutStr
        bot.TimeProvider <- time
        (bot, time)

    let makeBot (timeoutSec: int) = makeBotStr $"{timeoutSec}"

    let makeContext () =
        let client = new DiscordClient (DiscordConfiguration ())
        ()

    [<Test>]
    let inst () =
        BaggieBot ()
        |> should not' (be Null)

    [<Test>]
    let minTime_fromAppConfig () =
        let (bot, time) = makeBot 10
        bot.MinTime.TotalSeconds
        |> should equal 10

    [<Test>]
    let minTime_appConfigBadFormat () =
        let (bot, time) = makeBotStr "hello there"
        bot.MinTime.TotalSeconds
        |> should equal BaggieVals.DEFAULT_TIMEOUT_SEC
