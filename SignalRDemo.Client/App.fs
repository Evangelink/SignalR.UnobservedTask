namespace SignalRDemo.Client

open System

open Fable.SignalR

open SignalRDemo.Shared

open FSharp.Control
open System.Threading.Tasks
open System.Diagnostics

[<RequireQualifiedAccess>]
module AsyncSeq =

    // TODO #1491: remove when we can use a version of FSharp.Control.AsyncSeq that includes AsyncSeq.ofAsyncEnum.
    // That function is not available in the Nuget package that can be used on .NET Framework.
    // Copied from https://github.com/fsprojects/FSharp.Control.AsyncSeq/commit/1648b8a861e367ad6f6ea62379604d6d1a0d4561
    let ofAsyncEnum (source: Collections.Generic.IAsyncEnumerable<_>) = asyncSeq {
        let! ct = Async.CancellationToken
        let e = source.GetAsyncEnumerator ct
        use _ =
            { new IDisposable with
                member __.Dispose() =
                    e.DisposeAsync().AsTask() |> Async.AwaitTask |> Async.RunSynchronously }

        let mutable currentResult = true
        while currentResult do
            let! r = e.MoveNextAsync().AsTask() |> Async.AwaitTask
            currentResult <- r
            if r then yield e.Current
    }

[<RequireQualifiedAccess>]
module Async =

    let bind f computation =
        async.Bind (computation, f)

module App =
    let mutable count = 0

    let onServerMessage msg =
        match msg with
        | Something -> count <- count + 1

    let buildHub (serverUrl: string) =
        SignalR.Connect (fun hub ->
            hub.WithUrl(serverUrl)
                .WithAutomaticReconnect())

    let start () =
        TaskScheduler.UnobservedTaskException
        |> Event.add (fun args ->
            Debug.Fail (sprintf "Unobserved after %d\n\n%A" count args.Exception))

        let hub = buildHub "http://localhost:8085/SignalR"
        hub.StartNow ()

        hub.StreamFrom GetNotifications
        |> Async.bind (AsyncSeq.ofAsyncEnum >> AsyncSeq.iter onServerMessage)
        |> Async.Start