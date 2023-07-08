namespace Baggie

open System.Threading.Tasks

open DSharpPlus.CommandsNext
open DSharpPlus.CommandsNext.Attributes
open DSharpPlus.Entities

type BaggieBot () =

    inherit BaseCommandModule ()

    // I'm not sorry.
    let PASTA = "*D'awwww Baggie Waggie being all UwU and OwO on chat? \
        D'awwww wook he's so embawassed uWu Does Baggie Waggie need a \
        huggie wuggie poo? What about a kith? Baggie Waggie wan a kith? \
        UwU ehe te nandayo~*"

    let respondTo (ctx: CommandContext) (message: string) : Task =
        task {
            do!
                ctx.TriggerTypingAsync()

            let! _ =
                message
                |> ctx.RespondAsync

            return ()
        }
        :> Task

    [<Command "baggie">]
    let baggie (ctx: CommandContext) =
        PASTA
        |> respondTo ctx

    [<Command "baggie">]
    let baggie (ctx: CommandContext) (user: DiscordMember) =
        user.Mention + " " + PASTA
        |> respondTo ctx
