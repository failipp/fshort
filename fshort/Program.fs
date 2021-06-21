open System.Threading.Tasks
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe

open FSharp.Control.Tasks

open fshort
open fshort.Forwarding


let loadUrlMapping () =
    task {
        let! stuff = "urls.json" |> readFileAsStringAsync
        return stuff |> Json.deserialize<Urls>
    }

let getLongUrl shortUrl =
    match loadUrlMapping ()
          |> Async.AwaitTask
          |> Async.RunSynchronously with
    | Ok urls -> Some(urls |> forward shortUrl)
    | Error _ -> None

let skip : HttpFuncResult = Task.FromResult None

let time () = System.DateTime.Now.ToString()

let webApp =
    choose [ route "/" >=> redirectTo false "/app"
             routeCi "/app"
             >=> RequestErrors.NOT_FOUND "Reserved for later use"
             routeCix "/app/.*"
             >=> RequestErrors.NOT_FOUND "Reserved for later use"
             GET
             >=> choose [ routef
                              "/%s"
                              (fun shortUrl ->
                                  match getLongUrl shortUrl with
                                  | Some successHandler -> successHandler
                                  | None -> (fun _ _ -> skip)) ]
             RequestErrors.NOT_FOUND "Not Found" ]

let configureApp (app: IApplicationBuilder) =
    // Add Giraffe to the ASP.NET Core pipeline
    app.UseGiraffe webApp

let configureServices (services: IServiceCollection) =
    // Add Giraffe dependencies
    services.AddGiraffe() |> ignore

[<EntryPoint>]
let main _ =
    Host
        .CreateDefaultBuilder()
        .ConfigureWebHostDefaults(fun webHostBuilder ->
            webHostBuilder
                .Configure(configureApp)
                .ConfigureServices(configureServices)
            |> ignore)
        .Build()
        .Run()

    0
