module App
open Sutil
open Sutil.DOM
open Sutil.Attr
open Sutil.Bulma

// In Sutil, the view() function is called *once*
let view() =
    Html.div [
    DOM.text "Hello World!"
]

// Start the app
view() |> mountElement "sutil-app"