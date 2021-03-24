module Types
    [<Measure>]type percentage
    [<Measure>]type price
    type Symbol = Symbol of string
    type StockPrice = StockPrice of decimal<price>
    type Quantity = Quantity of uint
    type ShareQty = ShareQty of Quantity
    [<RequireQualifiedAccess>]
    module ShareQty =
        let get (ShareQty(Quantity num)) = num

    type Pnl = Pnl of decimal<percentage>
    type OpenPnl = OpenPnl of Pnl
    type DayPnl = DayPnl of Pnl
    type PortfolioOpenPnl = PortfolioOpenPnl of OpenPnl
    type PortfolioDayPnl = PortfolioDayPnl of DayPnl
    type PositionOpenPnl = PositionOpenPnl of OpenPnl
    type PositionDayPnl = PositionDayPnl of DayPnl
    type CurrentStockPrice = CurrentStockPrice of StockPrice
    [<RequireQualifiedAccess>]
    module CurrentStockPrice =
        let get (CurrentStockPrice(StockPrice num)) = num
    type LastClosePrice = LastClosePrice of StockPrice
    type AveragePrice= AveragePrice of decimal<price>
    type AverageOpenPrice = AverageOpenPrice of AveragePrice
    module AverageOpenPrice =
        let get (AverageOpenPrice(AveragePrice num))= num
    type Balances = Undefined
    type Stock =
        {
            Symbol: Symbol
            CurrentPrice: CurrentStockPrice
            LastClosePrice: LastClosePrice
        }
    [<RequireQualifiedAccess>]
    module Stock =
        let getSymbolString ({Symbol = Symbol str}) =
            str
    type PositionInfo =
        {
            Stock: Stock
            OpenQty: ShareQty
            AverageOpenPrice: AverageOpenPrice
        }
    type Portfolio =
        {
            Positions: PositionInfo list
            Balances: Balances
        }



    module PositionOpenPnl =
        let calculate (positioninfo:PositionInfo): PositionOpenPnl =
            let current = CurrentStockPrice.get positioninfo.Stock.CurrentPrice
            let openPrice = AverageOpenPrice.get positioninfo.AverageOpenPrice
            let pnlRatio = ((current - openPrice) / openPrice)
            let pnl = pnlRatio * 100.0M<percentage>
            (PositionOpenPnl << OpenPnl << Pnl) pnl
        let get (PositionOpenPnl(OpenPnl(Pnl num ))) = num
