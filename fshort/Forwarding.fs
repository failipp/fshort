module fshort.Forwarding

open Giraffe
open Microsoft.AspNetCore.Http

type Urls = Map<string, string>

let forward (shortUrl: string) (urlMapping: Urls) : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        match Map.tryFind shortUrl urlMapping with
        | Some url -> redirectTo false url next ctx
        | None -> RequestErrors.NOT_FOUND "Not Found" next ctx
