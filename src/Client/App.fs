module App
open Sutil.Html
open Sutil.DOM
open Sutil.Bulma
open Sutil.Attr
open Sutil.Styling
open Sutil
open System
open Types
open SeedData

type PortfolioTab =
    | Positions
    | Balances

type Model =
    {
        Portfolio:Portfolio
        CurrentPortfolioTab:PortfolioTab
    }

type Message =
    | SelectedPaneChanged of PortfolioTab
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
module Pnl =
    let percentageSpan (percentage:decimal<percentage>)=
            Html.span[
                class' "pnl-percent"
                if percentage >= 0.0M<percentage> then
                    class' "positive"
                else
                    class' "negative"
                Html.text $"""{percentage}{"%"}"""
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
    let positionsTable (positionsStore:IObservable<PositionInfo list>)=
        let header =
            Html.thead[
                Html.tr[
                    Html.th[Html.text "Symbol"]
                    Html.th[Html.text "Open Price"]
                    Html.th[Html.text "Current Price"]
                    Html.th[
                        Html.text "Qty"
                        Html.abbr[attr ("Title", "Open Quantity")]
                    ]
                    Html.th[
                        Html.text "Open Pnl"
                        Html.abbr[attr ("Title", "Open profit and loss")]
                    ]
                ]
            ]
        let getRowFromPositionInfo (position:PositionInfo) =
            let openQtyString =
                ShareQty.get position.OpenQty
                |> string
            let averageOpenPriceString =
                AverageOpenPrice.get position.AverageOpenPrice
                |> string
            let currentPrice =
                position.Stock.CurrentPrice
                |> CurrentStockPrice.get
                |> string
            let openPnl =
                position
                |> PositionOpenPnl.calculate
                |> PositionOpenPnl.get


            Html.tr[
                Html.td[
                    Stock.getSymbolString position.Stock
                    |> Html.text
                ]
                Html.td[
                    averageOpenPriceString
                    |> Html.text
                ]
                Html.td[
                    currentPrice
                    |> Html.text
                ]
                Html.td[
                    openQtyString
                    |> Html.text
                ]
                Html.td[
                    openPnl
                    |> Pnl.percentageSpan
                ]
            ]
        let rows (positions:PositionInfo list) =
            positions
            |> List.map getRowFromPositionInfo
        let getTableFromPositions positions =
            header::rows positions
            |>bulma.table

        Bind.fragment positionsStore getTableFromPositions


    let pnlElement (title:string) (percentage:decimal<percentage>) =

        Bulma.Level.item[
            bulma.container[
                style[Css.textAlignCenter]
                Html.h5 [
                    class' "mb-2"
                    Html.text title
                ]
                Pnl.percentageSpan percentage
            ]
        ]
    let buttons portfolioTab (isSelectedStore:IObservable<bool>) dispatch=
        let button =
            Bulma.Level.item[
                Html.button[
                    string portfolioTab
                    |> Html.text
                    onClick (fun _ -> dispatch <| SelectedPaneChanged portfolioTab )[]
                    bindClass isSelectedStore "selected"
                ]
            ]
        button
    let level (selectedPaneStore:IObservable<PortfolioTab>) dispatch =
        let isPositionsSelected =
            selectedPaneStore
            |> Store.map (function Positions -> true | _ -> false)
        let isBalancesSelected =
            selectedPaneStore
            |> Store.map (function Balances -> true | _ -> false)
        bulma.level[
            Bulma.Level.left [
                buttons Positions isPositionsSelected dispatch
                buttons Balances  isBalancesSelected dispatch
            ]
            Bulma.Level.right[
                pnlElement "Open pnl" -0.3M<percentage>
                pnlElement "Day pnl" 0.3M<percentage>
            ]
        ]
    let ContentView (model:IStore<Model>) dispatch=
        let selectedPaneStore =
            model
            |> Store.map(fun m -> m.CurrentPortfolioTab)
            |> Store.distinct
        let portfolioStore =
            model
            |> Store.map(fun m -> m.Portfolio)
            |> Store.distinct

        let getViewForSelectedPane = function
        | Positions ->
            let positionListStore =
                 portfolioStore
                |>Store.map(fun p -> p.Positions)
                |>Store.distinct
            positionsTable positionListStore
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
    let section (model:IStore<Model>) dispatch=
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
let init ():Model =
    {
        Portfolio = portfolio
        CurrentPortfolioTab = Positions
    }

let update (msg:Message) (model:Model) : Model =
    match msg with
    | SelectedPaneChanged portfolioTab ->
        if portfolioTab <> model.CurrentPortfolioTab then
            {model with CurrentPortfolioTab = portfolioTab}
        else
            model
let view () =
    let model, dispatch = Store.makeElmishSimple init update ignore ()

    Html.div [
        disposeOnUnmount [model]
        Navbar.section
        Main.section model dispatch
    ]
    |> withStyle mainStyleSheet

view () |> mountElement "sutil-app"