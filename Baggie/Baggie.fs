namespace Baggie

open System
open System.Collections.Generic
open System.Threading.Tasks

open DSharpPlus.CommandsNext
open DSharpPlus.CommandsNext.Attributes
open DSharpPlus.Entities
open Microsoft.Extensions.Logging

type BaggieBot () =
    inherit BaseCommandModule ()

    // I'm not sorry.
    let PASTA = "*D'awwww Baggie Waggie being all UwU and OwO on chat? \
        D'awwww wook he's so embawassed uWu Does Baggie Waggie need a \
        huggie wuggie poo? What about a kith? Baggie Waggie wan a kith? \
        UwU ehe te nandayo~*"

    let guildsLastUsed = Dictionary<uint64, DateTime>()

    let mutable appConfig : IAppConfigProvider = AppConfigProvider()

    let minTime =
        let defSec = 60
        try
            appConfig.GetConfigValue "baggie.timeoutSec"
            |> double
        with | :? FormatException as ex ->
            eprintf $"Failed to convert %s{ex.Source} to int"
            eprintf $"{ex}"
            defSec
        |> TimeSpan.FromSeconds

    let isTooSoon (ctx: CommandContext) : bool =
        if not (guildsLastUsed.ContainsKey(ctx.Guild.Id)) then
            false
        else
            let elapsed = DateTime.Now - guildsLastUsed[ctx.Guild.Id]
            elapsed < minTime

    let registerUsage (ctx: CommandContext) =
        if guildsLastUsed.ContainsKey(ctx.Guild.Id) then
            guildsLastUsed[ctx.Guild.Id] <- DateTime.Now
        else
            guildsLastUsed.Add(ctx.Guild.Id, DateTime.Now)

    let logReject (ctx: CommandContext) =
        let elapsed = DateTime.Now - guildsLastUsed[ctx.Guild.Id]
        ctx.Client.Logger.Log(
            LogLevel.Information,
            $"Rejecting {ctx.Command} from user {ctx.User.Username}, \
            only waited %.1f{elapsed.TotalSeconds} of {minTime.Seconds} sec"
        )

    let logPasta (ctx: CommandContext) =
        ctx.Client.Logger.Log(
            LogLevel.Information,
            $"Invoking {ctx.Command} from user {ctx.User.Username}"
        )

    let respondTo (ctx: CommandContext) (message: string) : Task =
        task {
            do! ctx.TriggerTypingAsync()
            let! _ = message |> ctx.RespondAsync
            return ()
        } :> Task

    let sendPasta (ctx: CommandContext) (pasta: string) : Task =
        if isTooSoon ctx then
            logReject ctx
            Task.CompletedTask
        else
            registerUsage ctx
            logPasta ctx
            pasta |> respondTo ctx

    [<Command "baggie">]
    let baggie (ctx: CommandContext) =
        PASTA |> sendPasta ctx

    [<Command "baggie">]
    let baggie (ctx: CommandContext) (user: DiscordMember) =
        user.Mention + " " + PASTA
        |> sendPasta ctx

    member this.AppConfig with set (value) = appConfig <- value
