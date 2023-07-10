namespace Baggie.Test

open System
open System.IO
open Baggie
open NUnit.Framework
open FsUnit

module TestProgram =

    let tokenPath = "/tmp/baggie.tok"

    let fakeFiles (tok: string) =
        FakeFileProvider(tokenPath, tok)

    [<Test>]
    let retrieveToken_faked () =
        Program.file <- fakeFiles "generalkenobi"
        Program.retrieveToken tokenPath
        |> should equal "generalkenobi"

    [<Test>]
    let retrieveToken_wrongPath () =
        Program.file <- fakeFiles "generalkenobi"
        (fun () -> Program.retrieveToken "/var/cache/baggie.tok" |> ignore)
        |> should throw typeof<FileNotFoundException>

    [<Test>]
    let retrieveToken_nullPath () =
        Program.file <- fakeFiles "generalkenobi"
        (fun () -> Program.retrieveToken null |> ignore)
        |> should throw typeof<ArgumentNullException>

    [<Test>]
    let retrieveToken_emptyToken () =
        Program.file <- fakeFiles ""
        (fun () -> Program.retrieveToken tokenPath |> ignore)
        |> should throw typeof<ArgumentException>
