module App
open Sutil.Html
open Sutil.DOM
open Sutil.Bulma
open Sutil.Attr
open Sutil.Styling

type Model = Empty

type Message = NoOp

let init ():Model = Empty

let update (msg:Message) (model:Model) : Model = model

let mainStyleSheet =
    Sutil.Bulma.withBulmaHelpers
        [rule "nav.navbar" [Css.backgroundColor "#EEEEEE"]]

module Navbar =
    open Sutil.Html
    open Bulma
    let section =
        bulma.navbar[Navbar.brand [Navbar.item[ Html.text "STONK"]]]

module Main =
    let section =
        bulma.columns
           [bulma.column[column.is2 ];
                bulma.column[column.is10]];


let view () =
    bulma.container [Navbar.section; Main.section]
    |> withStyle mainStyleSheet

view () |> mountElement "sutil-app"