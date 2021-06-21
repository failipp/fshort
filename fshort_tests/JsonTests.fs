module fshort_tests.JsonTests

open Xunit
open Swensen.Unquote

open fshort

open Json
open Forwarding

[<Fact>]
let ``Json.serialize serializes correctly`` () =
    let urls : Urls =
        [ "short1", "long1"; "short2", "long2" ]
        |> Map.ofList

    let json = urls |> serialize
    test <@ json = @"{""short1"":""long1"",""short2"":""long2""}" @>

[<Fact>]
let ``Json.deserialize deserializes correctly`` () =
    let json =
        @"{""short1"":""long1"",""short2"":""long2""}"

    let parsed: Result<Urls, _> = deserialize json

    test
        <@ match parsed with
           | Ok parsed_urls ->
               [ "short1", "long1"; "short2", "long2" ]
               |> Map.ofList = parsed_urls
           | Error _ -> false @>
