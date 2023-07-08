namespace Baggie

module Program =

    open System.Threading.Tasks
    open System.IO
    open Microsoft.Extensions.Configuration
    open DSharpPlus
    open DSharpPlus.CommandsNext

    let private appConfig =
        ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("AppSettings.json", true, true)
            .Build()

    let private startBot (token: string) =
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

        printfn "========== Ready =========="
        Task.Delay(-1)
        |> Async.AwaitTask
        |> Async.RunSynchronously

    let private validateToken (token: string) =
        if isNull token then nullArg (nameof token)
        elif token.Length <= 0 then invalidArg (nameof token) "Token cannot be empty"
        else token

    let private retrieveToken (tokenPath: string) =
        if isNull tokenPath then
            nullArg (nameof tokenPath)
        elif not (File.Exists(tokenPath)) then
            raise (FileNotFoundException("Token path not found", tokenPath))
        else
            printfn "Reading token from %s" tokenPath
            let token =
                File.ReadAllText(tokenPath)
                |> validateToken
            printfn $"Token found (len = {token.Length})"
            token

    [<EntryPoint>]
    let main argv =
        try
            appConfig["discord.tokenpath"]
            |> retrieveToken
            |> startBot
            0
        with
            | ex -> eprintf $"ERROR: {ex}"; 1
