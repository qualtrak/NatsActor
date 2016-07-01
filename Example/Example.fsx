#r "../packages/Akka/lib/net45/Akka.dll"
#r "../packages/Akka.FSharp/lib/net45/Akka.FSharp.dll"
#r "../packages/NATS.Client/lib/net45/NATS.Client.dll"
#r "../packages/Newtonsoft.Json/lib/net45/Newtonsoft.Json.dll"
#r "../packages/FsPickler/lib/net45/FsPickler.dll"
#r "../packages/System.Collections.Immutable/lib/netstandard1.0/System.Collections.Immutable.dll" 
#load "../NatsActor/NatsActor.fs"

open Akka  
open Akka.Actor
open Akka.FSharp
open NATS.Client 
open NatsActor
 
let cf = new ConnectionFactory()

let system = System.create "system" <| Configuration.load ()
let subscriber1 = spawn system "subscriber1" (natsActor cf)

subscriber1 <! Connect
subscriber1 <! Subscribe "foo"

let subscriber2 = spawn system "subscriber2" (natsActor cf)

subscriber2 <! Connect
subscriber2 <! Subscribe "*"

let publisher = spawn system "publisher" (natsActor cf)

publisher <! Connect
publisher <! Publish ("foo", "Hello World!")
publisher <! Publish ("bar", "Hello Again!")

subscriber2 <! Disconnect
publisher <! Publish ("foo", "Goodbye!")

subscriber1 <! Disconnect
publisher <! Publish ("foo", "Goodbye Again!")
publisher <! Disconnect
