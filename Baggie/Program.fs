namespace Baggie

open Microsoft.Extensions.Logging

// Following: https://brandewinder.com/2021/10/30/fsharp-discord-bot/
module Program =

    open System.Threading.Tasks
    open System.IO
    open Microsoft.Extensions.Configuration
    open DSharpPlus
    open DSharpPlus.CommandsNext

    let appConfig =
        ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("AppSettings.json", true, true)
            .Build()

    [<EntryPoint>]
    let main argv =
        let token = appConfig.["discord.token"]
        printfn "Hello world"

        let config = DiscordConfiguration ()
        config.Token <- token
        config.MinimumLogLevel <- LogLevel.Debug
        config.TokenType <- TokenType.Bot
        config.Intents <- DiscordIntents.GuildMessages
            ||| DiscordIntents.Guilds
            ||| DiscordIntents.MessageContents

        let discord = new DiscordClient(config)

        let commandsConfig = CommandsNextConfiguration ()
        commandsConfig.StringPrefixes <- ["!"; "/"]

        let commands = discord.UseCommandsNext(commandsConfig)
        commands.RegisterCommands<BaggieBot>()

        printfn "Connecting to Discord"
        discord.ConnectAsync()
        |> Async.AwaitTask
        |> Async.RunSynchronously

        printfn "Ready."
        Task.Delay(-1)
        |> Async.AwaitTask
        |> Async.RunSynchronously

        1
