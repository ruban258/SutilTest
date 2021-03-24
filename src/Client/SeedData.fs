module SeedData
open Types
let stock =
        {
            Symbol = Symbol "GME"
            CurrentPrice = CurrentStockPrice (StockPrice 2.0M<price>)
            LastClosePrice = LastClosePrice (StockPrice 3.0M<price>)
        }
let positionInfo =
    {
        Stock = stock
        OpenQty = ShareQty(Quantity 300u)
        AverageOpenPrice = AverageOpenPrice(AveragePrice 1.0M<price>)
    }
let portfolio =
    {
        Balances = Undefined
        Positions = [positionInfo]
    }