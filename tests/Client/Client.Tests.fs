module Client.Tests
open Fable.Mocha
open Types
let pnl = testList "Pnl" [
    testCase "Position Open Pnl Calculates Correctly" <| fun _ ->
        let position = SeedData.positionInfo
        let expected = PositionOpenPnl(OpenPnl(Pnl 100.0M<percentage>))
        let actual = PositionOpenPnl.calculate position
        Expect.equal actual expected "Open Pnl Calculates Correctly"

]
let client = testList "Client" [pnl]

let all =
    testList "All"
        [
#if FABLE_COMPILER // This preprocessor directive makes editor happy
            Shared.Tests.shared
#endif
            client
        ]

[<EntryPoint>]
let main _ = Mocha.runTests all