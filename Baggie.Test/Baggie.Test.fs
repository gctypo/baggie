module Baggie.Test

open NUnit.Framework

[<SetUp>]
let Setup () = ()

[<Test>]
let Inst () =
    BaggieBot()
    |> Assert.IsNotNull
