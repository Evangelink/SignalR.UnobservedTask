module Server

open FSharp.Control

open Fable.SignalR
open Saturn

open SignalRDemo.Shared

let mutable count = 0

let start msg _ =
    match msg with
    | GetNotifications ->
        asyncSeq {
            while true do
                yield Something
                count <- count + 1
                do! Async.Sleep (2000 * count)
        } |> AsyncSeq.toAsyncEnum

let app =
    application {
        url "http://0.0.0.0:8085"
        memory_cache
        use_static "public"
        use_signalr (
            configure_signalr {
                endpoint "/SignalR"
                send (fun _ -> failwith "SignalR send not implemented")
                invoke (fun _ -> failwith "SignalR invoke not implemented")
                stream_from start
            }
        )
    }

run app