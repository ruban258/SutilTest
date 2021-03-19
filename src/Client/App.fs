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
    Sutil.Bulma.withBulmaHelpers[
        rule "nav.navbar" [
            Css.backgroundColor "#EEEEEE"
            Css.borderBottomColor "red"
            Css.borderBottomWidth 1]
        rule "div.body" [ Css.height 1000 ]
        rule ".full-height" [ Css.height 1000 ]]


module Navbar =
    open Sutil.Html
    open Bulma
    let section =
        bulma.navbar[Navbar.brand [Navbar.item[ Html.text "STONK"]]]

module Main =
    let contentView =
        bulma.section[color.hasBackgroundLight]
    let section =
        bulma.columns[
            bulma.column[
                class' "full-height"
                column.is2
                color.hasBackgroundLight]
            bulma.column[
                column.is10
                class' "full-height"
                contentView]]


let view () =
    Html.div [
        class' "body";
        Navbar.section;
        Main.section]
    |> withStyle mainStyleSheet

view () |> mountElement "sutil-app"