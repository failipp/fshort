module fshort.Forwarding

open Giraffe
open Microsoft.AspNetCore.Http

let links =
    Map.empty
    |> Map.add "gog" "https://google.com"
    |> Map.add "ytbe" "https://youtube.com"

let forward (subRoute: string) : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        match Map.tryFind subRoute links with
        | Some url -> redirectTo false url next ctx
        | None -> RequestErrors.NOT_FOUND "Not Found" next ctx
