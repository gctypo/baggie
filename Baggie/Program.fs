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

    let startBot (token: string) =
        let config = DiscordConfiguration ()
        config.Token <- token
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

    [<EntryPoint>]
    let main argv =
        let tokenPath = appConfig.["discord.tokenpath"]
        if isNull tokenPath then
            eprintf "ERROR: Config discord.tokenpath is null"
            1
        elif not (File.Exists tokenPath) then
            eprintf $"ERROR: File %s{tokenPath} not found"
            1
        else
            let token = File.ReadAllText tokenPath
            if token.Length >= 1 then
                printfn "Found token at %s" tokenPath
                startBot token
                0
            else
                failwith $"ERROR: Could not find token at '%s{tokenPath}'!"
                1
