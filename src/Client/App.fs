module App
open Sutil
open Sutil.DOM
open Sutil.Attr
open Sutil.Bulma
open Sutil.Styling
open Feliz

let css = [
        rule "p" [
            Css.color "orange"
            Css.fontFamily "'Comic Sans MS', cursive"
            Css.fontSize (length.em 2.0)
        ]
    ]

let Nested() =
   Html.p [ DOM.text "...don't affect this element" ] |> withStyle []

// In Sutil, the view() function is called *once*
let view() =
    Html.div [
        Html.p [
            DOM.text "These styles..."
        ]
        Nested()
        Html.p [
            DOM.text "just trying some shit,,"

        ]

        withStyle css <| Nested()

    ]


// Start the app
view() |> mountElement "sutil-app"