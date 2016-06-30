module NatsActor

    open Akka.Actor
    open Akka.FSharp
    open NATS.Client
    open System
    open System.Text
    open System.Threading

    type MessageContent = string
    type Subject = string

    type QueueCommand = 
        | Connect // Connect to Nats server
        | Disconnect // Disconnect from server
        | Subscribe of Subject // Subscribe to message tagged with 'Subject'
        | Unsubscribe // Unsubscribe
        | Publish of Subject * MessageContent // Publish message with tag 'Subject'
        | Receive of MessageContent // Receive message

    let natsActor (factory : ConnectionFactory) (mailbox: Actor<_>) =

        let connect() = 
            let opts = ConnectionFactory.GetDefaultOptions()
            let connection = factory.CreateConnection(opts)
            connection

        let disconnect (connection : IConnection) =
            connection.Dispose()

        let unsubscribe (subscription : IAsyncSubscription) =
            subscription.Unsubscribe()
            subscription.Dispose()

        let publishMessage (connection : IConnection) (subject : Subject) (message : MessageContent) =
            let payload = Encoding.UTF8.GetBytes(message)
            connection.Publish(subject, payload)

        let messageHandler (e : MsgHandlerEventArgs) =
            let message = Encoding.UTF8.GetString(e.Message.Data)
            mailbox.Self <! Receive message

        let rec disconnected () = 
            actor {
                let! message = mailbox.Receive ()
                match message with
                | Connect ->
                    let connection = connect()
                    return! connected (connection)
                | _ ->
                    printfn "invalid operation in disconnected state: %A" message
                    return! disconnected()
            }
        and connected (connection : IConnection) = 
            actor {
                let! message = mailbox.Receive ()
                match message with
                | Subscribe subject ->
                    let subscription = connection.SubscribeAsync(subject)
                    subscription.MessageHandler.Add messageHandler
                    subscription.Start()
                    return! subscribed (connection, subscription)
                | Disconnect ->
                    disconnect (connection)
                    return! disconnected ()

                | Publish (subject, content) -> publishMessage connection subject content

                | _ -> printfn "Invalid command in connected state: %A" message

                return! connected connection
            }
        and subscribed (connection : IConnection, subscription : IAsyncSubscription) = 
            actor {
                let! message = mailbox.Receive ()
                match message with
                | Unsubscribe ->
                    unsubscribe subscription
                    return! connected connection
                    
                | Disconnect ->
                    unsubscribe subscription
                    disconnect connection
                    return! disconnected()

                | Publish (subject, content) -> publishMessage connection subject content

                | Receive what -> printfn "Actor: %s received message: %s" mailbox.Self.Path.Name what

                | _ -> printfn "Invalid command in subscribed state: %A" message

                return! subscribed (connection, subscription)
            }
        disconnected()
