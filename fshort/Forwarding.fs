module fshort.Forwarding

open Giraffe
open Microsoft.AspNetCore.Http

type Urls = Map<string, string>

let links: Urls =
    Map.empty
    |> Map.add "gog" "https://google.com"
    |> Map.add "ytbe" "https://youtube.com"
    
let getLongUrl shortUrl =
    Map.tryFind shortUrl links

let forward (shortUrl: string) : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        match getLongUrl shortUrl with
        | Some url -> redirectTo false url next ctx
        | None -> RequestErrors.NOT_FOUND "Not Found" next ctx
