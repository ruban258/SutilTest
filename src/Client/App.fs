module App
open Sutil.Html
open Sutil.DOM
open Sutil.DOM.CssRules
open Sutil.Bulma
open Sutil.Attr
open Sutil.Styling

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

type Message = NoOp

let init ():Model =
    {
        OpenPnl = 3.0M<percentage>
        DayPnl = -0.5M<percentage>
        SelectedPane = Positions
    }

let update (msg:Message) (model:Model) : Model = model

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

module Main =
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
    let buttons (text:string) =
        let button =
            Bulma.Level.item[
                bulma.buttons[Html.text text]
            ]
        button
    let level =
        bulma.level[
            Bulma.Level.left [
                buttons "Positions"
                buttons "Balances"
            ]
            Bulma.Level.right[
                pnlElement "Open pnl" -0.3M<percentage>
                pnlElement "Day pnl" 0.3M<percentage>
            ]
        ]
    let contentView =
        bulma.section[
            Html.div [
                style [Css.backgroundColor Color.lightGrey; Css.overflowHidden]
                bulma.container[
                    class' "p-5"
                    Html.text "Account summary" |> Html.h3
                    bulma.container[
                        class' "pt-5"
                        level
                    ]
                ]
            ]
        ]
    let section =
        bulma.columns[
            columns.isDesktop
            bulma.column[
                column.is2
                color.hasBackgroundLight
            ]
            bulma.column[
                column.is10
                color.hasBackgroundLight
                contentView
            ]
        ]

let view () =
    Html.div [
        Navbar.section;
        Main.section
    ]
    |> withStyle mainStyleSheet

view () |> mountElement "sutil-app"