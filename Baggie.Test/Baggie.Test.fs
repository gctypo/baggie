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
        let (_, err) = ProviderBindings.setStdIO ()

        bot.MinTime.TotalSeconds
        |> should equal BaggieVals.DEFAULT_TIMEOUT_SEC
        err.ToString()
        |> should startWith "Failed to convert timeout"

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

    [<Test>]
    let registerUsage_new () =
        let (bot, time) = makeBot 10
        bot.registerUsage 420UL

        bot.LastUsed[420UL] |> should equal time.Now
        bot.LastUsed.Count |> should equal 1

    [<Test>]
    let registerUsage_existing () =
        let (bot, time) = makeBot 10
        bot.LastUsed.Add(420UL, time.Now)
        time.OffsetTime 20
        bot.registerUsage 420UL

        bot.LastUsed[420UL] |> should equal time.Now
        bot.LastUsed.Count |> should equal 1

    [<Test>]
    let respondTo () =
        let (bot, _) = makeBot 10
        let ctx = FakeContext ()
        bot.respondTo ctx "test message"
        |> Async.AwaitTask |> Async.RunSynchronously

        ctx.IsTyping |> should be True
        ctx.Response |> should equal (Some"test message")

    [<Test>]
    let sendPasta_new () =
        let (bot, _) = makeBot 10
        let ctx = FakeContext ()

        bot.sendPasta ctx "nice pasta"
        |> Async.AwaitTask |> Async.RunSynchronously

        ctx.IsTyping |> should be True
        ctx.Response |> should equal (Some("nice pasta"))

    [<Test>]
    let sendPasta_Later () =
        let (bot, time) = makeBot 20
        let ctx = FakeContext ()
        bot.LastUsed[(ctx :> IContextAdapter).GuildId] <- time.Now
        time.OffsetTime 60

        bot.sendPasta ctx "nice pasta"
        |> Async.AwaitTask |> Async.RunSynchronously

        ctx.IsTyping |> should be True
        ctx.Response |> should equal (Some("nice pasta"))

    [<Test>]
    let sendPasta_tooSoon () =
        let (bot, time) = makeBot 30
        let ctx = FakeContext ()
        bot.LastUsed[(ctx :> IContextAdapter).GuildId] <- time.Now
        time.OffsetTime 10

        bot.sendPasta ctx "nice pasta"
        |> Async.AwaitTask |> Async.RunSynchronously

        ctx.IsTyping |> should be False
        ctx.Response |> should equal None
