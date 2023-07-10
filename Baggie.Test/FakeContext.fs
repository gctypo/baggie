namespace Baggie.Test

open System.Threading.Tasks
open Baggie
open DSharpPlus.Entities
open Microsoft.Extensions.Logging

type FakeContext () =

    interface IContextAdapter with
        member x.CommandName = "testcmd"
        member x.Username = "gctypo"
        member x.Logger = (new LoggerFactory()).CreateLogger()
        member this.GuildId = 420UL
        member this.RespondAsync(_) = Task<DiscordMessage>.FromResult(null)
        member this.TriggerTypingAsync() = Task.CompletedTask
