module App
open Sutil.Html
open Sutil.DOM
open Sutil.DOM.CssRules
open Sutil.Bulma
open Sutil.Attr
open Sutil.Styling
open Sutil
open System

[<Measure>]type percentage

type SummaryInfo =
    | Positions
    | Balances

type Model =
    {
        OpenPnl: decimal<percentage>
        DayPnl: decimal<percentage>
        SelectedPane:SummaryInfo
    }

type Message =
    | SelectedPaneSelected of SummaryInfo

let init ():Model =
    {
        OpenPnl = 3.0M<percentage>
        DayPnl = -0.5M<percentage>
        SelectedPane = Balances
    }

let update (msg:Message) (model:Model) : Model =
    match msg with
    | SelectedPaneSelected summaryInfo ->
        if summaryInfo <> model.SelectedPane then
            {model with SelectedPane = summaryInfo}
        else
            model

let mainStyleSheet =
    Sutil.Bulma.withBulmaHelpers[
        rule "nav.navbar" [
            Css.backgroundColor "#EEEEEE"
            Css.borderBottomColor "red"
            Css.borderBottomWidth 1
        ]
        rule "div.body" [ Css.height 1000 ]
        rule ".full-height" [ Css.height 100 ]
        rule "span.pnl-percent" [
            Css.fontSize 20
            Css.fontWeight 500
        ]
        rule ".pnl-percent.positive" [ Css.color "green"]
        rule ".pnl-percent.negative" [ Css.color "#ff0000"]
        rule "button.selected" [
            Css.backgroundColor "#6A42B7"
            Css.color "white"
        ]
    ]
module Navbar =
    open Sutil.Html
    open Bulma
    let section =
        bulma.navbar[
            Navbar.brand[
                Navbar.item[
                    Html.text "STONK"
                ]
            ]
        ]
module SummaryPage =
    let positionsTable =
        bulma.table [

        ]
    let pnlElement (title:string) (percentage:decimal<percentage>) =
        let percentageSpan =
            Html.span[
                class' "pnl-percent"
                if percentage >= 0.0M<percentage> then
                    class' "positive"
                else
                    class' "negative"
                Html.text $"""{percentage}{"%"}"""
            ]
        Bulma.Level.item[
            bulma.container[
                style[Css.textAlignCenter]
                Html.h5 [
                    class' "mb-2"
                    Html.text title
                ]
                percentageSpan
            ]
        ]
    let buttons summaryInfo (text:string) (isSelectedStore:IObservable<bool>) dispatch=
        let button =
            Bulma.Level.item[
                Html.button[
                    Html.text text
                    onClick (fun e -> dispatch <| SelectedPaneSelected summaryInfo )[]
                    bindClass isSelectedStore "selected"
                ]
            ]
        button
    let level (selectedPaneStore:IObservable<SummaryInfo>) dispatch =
        let isPositionsSelected =
            selectedPaneStore
            |> Store.map (function Positions -> true | _ -> false)
        let isBalancesSelected =
            selectedPaneStore
            |> Store.map (function Balances -> true | _ -> false)
        bulma.level[
            Bulma.Level.left [
                buttons Positions "Positions" isPositionsSelected dispatch
                buttons Balances "Balances" isBalancesSelected dispatch
            ]
            Bulma.Level.right[
                pnlElement "Open pnl" -0.3M<percentage>
                pnlElement "Day pnl" 0.3M<percentage>
            ]
        ]
    let ContentView model dispatch=
        let selectedPaneStore =
            model
            |> Store.map(fun selectd -> selectd.SelectedPane)
            |> Store.distinct

        let getViewForSelectedPane = function
        | Positions -> positionsTable
        | Balances -> Html.text "not done yet"

        bulma.section[
            Html.div [
                style [Css.backgroundColor Color.lightGrey; Css.overflowHidden]
                bulma.container[
                    class' "p-5"
                    Html.text "Account summary" |> Html.h3
                    bulma.container[
                        class' "pt-5"
                        level selectedPaneStore dispatch
                        Bind.fragment selectedPaneStore getViewForSelectedPane
                    ]
                ]
            ]
        ]
module Main =
    let section model dispatch=
        bulma.columns[
            bulma.column[
                column.is2
                color.hasBackgroundLight
            ]
            bulma.column[
                column.is10
                color.hasBackgroundLight
                SummaryPage.ContentView model dispatch
            ]
        ]
let view () =
    let model, dispatch = Store.makeElmishSimple init update ignore ()

    Html.div [
        disposeOnUnmount [model]
        Navbar.section
        Main.section model dispatch
    ]
    |> withStyle mainStyleSheet

view () |> mountElement "sutil-app"