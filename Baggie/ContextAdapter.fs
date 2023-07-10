namespace Baggie

open System.Threading.Tasks
open DSharpPlus.CommandsNext
open DSharpPlus.Entities
open Microsoft.Extensions.Logging

type IContextAdapter =
    abstract member CommandName : string
    abstract member Username : string
    abstract member GuildId : uint64
    abstract member Logger : ILogger
    abstract member TriggerTypingAsync : unit -> Task
    abstract member RespondAsync : string -> Task<DiscordMessage>

type ContextAdapter (ctx: CommandContext) =
    member public x.Context = ctx

    interface IContextAdapter with
        member x.CommandName = ctx.Command.Name
        member x.Username = ctx.User.Username
        member x.GuildId = ctx.Guild.Id
        member x.Logger = ctx.Client.Logger
        member x.TriggerTypingAsync () = ctx.TriggerTypingAsync()
        member x.RespondAsync (msg: string) =
            ctx.RespondAsync msg

    member public x.Command = ctx.Command
    member public x.User = ctx.User
    member public x.Guild = ctx.Guild
    member public x.Client = ctx.Client

    member public this.TriggerTypingAsync () =
        (this :> IContextAdapter).TriggerTypingAsync()

    member public this.RespondAsync (msg: string) =
        (this :> IContextAdapter).RespondAsync msg
