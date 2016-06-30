// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open Akka.FSharp
open NATS.Client
open NatsActor

[<EntryPoint>]
let main args =

    let cf = new ConnectionFactory()

    let system = System.create "system" <| Configuration.load ()
    let nats = spawn system "queues" (natsActor cf)

    nats <! Connect
    nats <! Subscribe "foo"

    System.Console.ReadKey() |> ignore
    0
//

//    let opts = ConnectionFactory.GetDefaultOptions()
//    use c = cf.CreateConnection(opts)
//
//    let event = new System.Threading.AutoResetEvent(false)
//
//    let mutable recieved = 0
//
//    let handler = fun (args : MsgHandlerEventArgs) ->
//        recieved <- recieved + 1
//        if recieved > 10000 then event.Set() |> ignore
//        let s = System.Text.Encoding.UTF8.GetString(args.Message.Data)
//        printfn "received: %s" s
//
//    //use s = c.SubscribeAsync("foo", handler)
//
//    // You should also be able to use
//    use s = c.SubscribeAsync("foo")
//    // and then add the handler like this - but that does not compile in F#
//    // There is a type error:
//            //    Severity	Code	Description	Project	File	Line	Suppression State
//            //    Error		Type mismatch. Expecting a
//            //    MsgHandlerEventArgs -> unit
//            //    but given a
//            //    MsgHandlerEventArgs -> 'a -> unit
//    s.MessageHandler.Add handler
//    // Start() must be called to begin receiving messages.
//
//    s.MessageHandler
//    |> Observable.subscribe (fun e -> printfn "Observed")
//    |> ignore
//
//    s.Start()
//
//
//    event.WaitOne() |> ignore

    // return an integer exit code
