open System.IO
open DSharpPlus
open Microsoft.Extensions.Configuration

// Following: https://brandewinder.com/2021/10/30/fsharp-discord-bot/

let appConfig =
    ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("AppSettings.json", true, true)
        .Build()

[<EntryPoint>]
let main argv =
    let token = appConfig.["discord.token"]
    printfn "Hello world"

    let config = DiscordConfiguration()
    config.Token <- token
    config.TokenType <- TokenType.Bot

    let client = new DiscordClient(config)

    0
