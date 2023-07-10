namespace Baggie.Test

open System.Threading.Tasks
open Baggie
open DSharpPlus.Entities
open Microsoft.Extensions.Logging
open Microsoft.FSharp.Core

type FakeContext () =
    // Need to sense through these
    let mutable typing = false
    let mutable response : Option<string> = None

    interface IContextAdapter with
        member x.CommandName = "testcmd"
        member x.Username = "gctypo"
        member x.Logger = (new LoggerFactory()).CreateLogger()
        member this.GuildId = 420UL
        member this.RespondAsync(msg) =
            response <- Some(msg)
            Task<DiscordMessage>.FromResult(null)
        member this.TriggerTypingAsync() =
            typing <- true
            Task.CompletedTask

    member x.IsTyping with get () = typing
    member x.Response with get () = response
