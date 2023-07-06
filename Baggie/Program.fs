open System.IO
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
    printfn $"Token: %s{token}"

    0
