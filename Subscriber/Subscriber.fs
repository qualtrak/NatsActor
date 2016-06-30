open Akka.FSharp
open NATS.Client
open NatsActor

[<EntryPoint>]
let main args =

    let cf = new ConnectionFactory()

    let system = System.create "system" <| Configuration.load ()
    let subscriber = spawn system "subscriber" (natsActor cf)

    subscriber <! Connect
    subscriber <! Subscribe "foo"

    printfn "Press any key to exit application"
    System.Console.ReadKey() |> ignore
    0
