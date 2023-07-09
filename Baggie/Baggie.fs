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

    let guildsLastUsed = Dictionary<uint64, DateTime> ()

    let mutable appConfig : IAppConfigProvider = AppConfigProvider ()

    let mutable time : ITimeNowProvider = TimeNowProvider ()

    let minTime =
        let DEF_SEC = 60
        try
            appConfig.GetConfigValue "baggie.timeoutSec"
            |> double
        with | :? FormatException as ex ->
            eprintf $"Failed to convert %s{ex.Source} to int"
            eprintf $"{ex}"
            DEF_SEC
        |> TimeSpan.FromSeconds

    let isTooSoon (guildId: uint64) : bool =
        if not (guildsLastUsed.ContainsKey(guildId)) then
            false
        else
            let elapsed = time.Now - guildsLastUsed[guildId]
            elapsed < minTime

    let registerUsage (guildId: uint64) =
        if guildsLastUsed.ContainsKey(guildId) then
            guildsLastUsed[guildId] <- time.Now
        else
            guildsLastUsed.Add(guildId, time.Now)

    let logReject (logger: ILogger, command: Command, username: string, guildId: uint64) =
        let elapsed = time.Now - guildsLastUsed[guildId]
        logger.Log(
            LogLevel.Information,
            $"Rejecting {command} from user {username}, \
            only waited %.1f{elapsed.TotalSeconds} of {minTime.Seconds} sec"
        )

    let logPasta (logger: ILogger, command: Command, username: string) =
        logger.Log(
            LogLevel.Information,
            $"Invoking {command} from user {username}"
        )

    let respondTo (ctx: CommandContext) (message: string) : Task =
        task {
            do! ctx.TriggerTypingAsync()
            let! _ = message |> ctx.RespondAsync
            return ()
        } :> Task

    let sendPasta (ctx: CommandContext) (pasta: string) : Task =
        if isTooSoon ctx.Guild.Id then
            logReject (ctx.Client.Logger, ctx.Command, ctx.User.Username, ctx.Guild.Id)
            Task.CompletedTask
        else
            registerUsage ctx.Guild.Id
            logPasta (ctx.Client.Logger, ctx.Command, ctx.User.Username)
            pasta |> respondTo ctx

    [<Command "baggie">]
    let baggie (ctx: CommandContext) =
        PASTA |> sendPasta ctx

    [<Command "baggie">]
    let baggie (ctx: CommandContext) (user: DiscordMember) =
        user.Mention + " " + PASTA
        |> sendPasta ctx

    member this.AppConfig
        with public set value = appConfig <- value

    member this.TimeProvider
        with public set value = time <- value

    member this.LastUsed
        with public get () = guildsLastUsed
