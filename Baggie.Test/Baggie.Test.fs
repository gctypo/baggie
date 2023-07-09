module Baggie.Test

open FsUnit
open NUnit.Framework

[<Test>]
let InstFs () =
    BaggieBot()
    |> should not' (be Null)
