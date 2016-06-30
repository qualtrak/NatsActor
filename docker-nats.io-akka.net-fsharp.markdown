---
layout: post
title: Exploring the publish-subscribe pattern with NATS and F# based Akka.NET actors
---

https://seroter.wordpress.com/2016/05/16/modern-open-source-messaging-apache-kafka-rabbitmq-and-nats-in-action/

insert drawing

http://www.enterpriseintegrationpatterns.com/patterns/messaging/PublishSubscribeChannel.html


This post is not meant as an introduction or tutorial about NATS so if you need to learn more about NATS I suggest that you visit the [NATS](http://nats.io/) website and watch [Simple Solutions for Complex Problems](https://www.youtube.com/watch?v=0h0t3og8llc) by [Tyler Treat](http://bravenewgeek.com/)
and optionally [Powered by NATS: Integration Patterns for Microservices Architectures](https://www.youtube.com/watch?v=f5gZdK8ir4M) by [David Williams](https://twitter.com/DavWilliams)

This example was inspired by


http://www.slideshare.net/Apcera/actor-patterns-and-nats-boulder-meetup

and a more general

Before we get started we need to install and run the Nats.IO server. Luckyli there is an official Docker image so installing the server is a simple as

> docker pull nats

and the start the server with

> docker run -d --name nats-main -p 4222:4222 nats

http://getakka.net/docs/clustering/distributed-publish-subscribe
http://doc.akka.io/docs/akka/current/scala/event-bus.html

{% highlight ocaml %}
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
{% endhighlight %}

## Improvements
Build suave frontend
run everything is a docker cluter
Replace NATS client with MyNatsClient
have another actor handling the message to offload work and not block

## References
Enterprise Integraton Patterns, Gregor Hophe and Bobby Woolf, Addison-Wesley, 2004

Reactive Design Patterns, Roland Kuhn and Jamie Allen, Manning Publications, 2016

Reactive Messaging Patterns with the Actor Model, Vaughn Vernon, Addison-Wesley, 2016