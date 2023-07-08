namespace Baggie

open System.IO
open Microsoft.Extensions.Configuration

module AppSettings =

    let public appConfig =
        ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("AppSettings.json", true, true)
            .Build()
