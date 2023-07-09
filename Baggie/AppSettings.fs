namespace Baggie

open System.IO
open Microsoft.Extensions.Configuration

module AppSettings =

    let public appConfig =
        ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("AppSettings.json", true, true)
            .Build()

type AppConfigProvider() =
    interface IAppConfigProvider with
        member this.GetConfigValue (key: string) = AppSettings.appConfig[key]
