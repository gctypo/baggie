namespace Baggie.Test

open Baggie

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

    [<Test>]
    let inst () =
        BaggieBot ()
        |> should not' (be Null)

    [<Test>]
    let minTime_fromAppConfig () =
        let (bot, _) = makeBot 10
        bot.MinTime.TotalSeconds
        |> should equal 10

    [<Test>]
    let minTime_appConfigBadFormat () =
        let (bot, _) = makeBotStr "hello there"
        bot.MinTime.TotalSeconds
        |> should equal BaggieVals.DEFAULT_TIMEOUT_SEC

    [<Test>]
    let isTooSoon_noKey () =
        let (bot, _) = makeBot 10
        bot.isTooSoon 420UL
        |> should equal false

    [<Test>]
    [<TestCase(10, 60)>]
    [<TestCase(20, 10)>]
    let isTooSoon_hasKey (timeout: int, offset: int) =
        let (bot, time) = makeBot timeout
        bot.LastUsed.Add(420UL, time.Now)
        time.OffsetTime offset
        bot.isTooSoon 420UL
        |> should equal (offset < timeout)
