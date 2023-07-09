namespace Baggie

open System.Reflection

module Program =

    open System.Threading.Tasks
    open System.IO
    open DSharpPlus
    open DSharpPlus.CommandsNext

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

    let helpText =
        "Usage: baggie\n   or: baggie --version\n\n \
        Runs 'baggie' Discord bot. ^C to exit.\n \
        Expects token at path set for 'discord.tokenpath' in AppSettings.json."

    [<EntryPoint>]
    let main argv =
        if argv.Length > 0 then
            if argv[0] = "--version" then
                Assembly.GetExecutingAssembly().GetName().Version.ToString()
                |> printfn "baggie version %s"
                0
            elif argv[0] = "--help" then
                printfn "%s" helpText
                0
            else
                eprintfn "%s" helpText
                1
        else
            try
                AppSettings.appConfig["discord.tokenpath"]
                |> retrieveToken
                |> startBot
                0
            with
                | ex -> eprintf $"ERROR: {ex}"; 1
