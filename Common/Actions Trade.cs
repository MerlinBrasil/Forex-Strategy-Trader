// Action Trade
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Collections.Generic;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Class Actions : Controls
    /// </summary>
    public partial class Actions : Controls
    {
        StrategyPriceType openStrPriceType;
        StrategyPriceType closeStrPriceType;
        ExecutionTime openTimeExec;
        ExecutionTime closeTimeExec;
        double maximumLots;
        double micron;

        DateTime timeLastEntryBar; // The time of last executed entry;
        bool isEnteredLong;  // Whether we have already entered long during this bar.
        bool isEnteredShort; // Whether we have already entered short during this bar.

        // Logical Groups
        Dictionary<string, bool> groupsAllowLong;
        Dictionary<string, bool> groupsAllowShort;
        List<string> openingLogicGroups;
        List<string> closingLogicGroups;

        // N Bars Exit indicator - Krog
        static int NBarsExit;

        /// <summary>
        /// Initializes the global variables.
        /// </summary>
        void InitTrade()
        {
            micron = Data.InstrProperties.Point / 2d;

            // Sets the maximum lots
            maximumLots = 100;
            foreach (IndicatorSlot slot in Data.Strategy.Slot)
                if (slot.IndicatorName == "Lot Limiter")
                    maximumLots = (int)slot.IndParam.NumParam[0].Value;
            maximumLots = Math.Min(maximumLots, Data.Strategy.MaxOpenLots);

            openTimeExec  = Data.Strategy.Slot[Data.Strategy.OpenSlot].IndParam.ExecutionTime;
            openStrPriceType = StrategyPriceType.Unknown;
            if (openTimeExec == ExecutionTime.AtBarOpening)
                openStrPriceType = StrategyPriceType.Open;
            else if (openTimeExec == ExecutionTime.AtBarClosing)
                openStrPriceType = StrategyPriceType.Close;
            else
                openStrPriceType = StrategyPriceType.Indicator;

            closeTimeExec = Data.Strategy.Slot[Data.Strategy.CloseSlot].IndParam.ExecutionTime;
            closeStrPriceType = StrategyPriceType.Unknown;
            if (closeTimeExec == ExecutionTime.AtBarOpening)
                closeStrPriceType = StrategyPriceType.Open;
            else if (closeTimeExec == ExecutionTime.AtBarClosing)
                closeStrPriceType = StrategyPriceType.Close;
            else if (closeTimeExec == ExecutionTime.CloseAndReverse)
                closeStrPriceType = StrategyPriceType.CloseAndReverse;
            else
                closeStrPriceType = StrategyPriceType.Indicator;

            if (Configs.UseLogicalGroups)
            {
                Data.Strategy.Slot[Data.Strategy.OpenSlot].LogicalGroup  = "All"; // Allows calculation of open slot for each group.
                Data.Strategy.Slot[Data.Strategy.CloseSlot].LogicalGroup = "All"; // Allows calculation of close slot for each group.

                groupsAllowLong  = new Dictionary<string, bool>();
                groupsAllowShort = new Dictionary<string, bool>();
                for (int slot = Data.Strategy.OpenSlot; slot < Data.Strategy.CloseSlot; slot++)
                {
                    if (!groupsAllowLong.ContainsKey(Data.Strategy.Slot[slot].LogicalGroup))
                        groupsAllowLong.Add(Data.Strategy.Slot[slot].LogicalGroup, false);
                    if (!groupsAllowShort.ContainsKey(Data.Strategy.Slot[slot].LogicalGroup))
                        groupsAllowShort.Add(Data.Strategy.Slot[slot].LogicalGroup, false);
                }

                // List of logical groups
                openingLogicGroups = new List<string>();
                foreach (System.Collections.Generic.KeyValuePair<string, bool> kvp in groupsAllowLong)
                    openingLogicGroups.Add(kvp.Key);


                // Logical groups of the closing conditions.
                closingLogicGroups = new List<string>();
                for (int slot = Data.Strategy.CloseSlot + 1; slot < Data.Strategy.Slots; slot++)
                {
                    string group = Data.Strategy.Slot[slot].LogicalGroup;
                    if (!closingLogicGroups.Contains(group) && group != "all")
                        closingLogicGroups.Add(group); // Adds all groups except "all"
                }

                if (closingLogicGroups.Count == 0)
                    closingLogicGroups.Add("all"); // If all the slots are in "all" group, adds "all" to the list.
            }

            // Search if N Bars Exit is present as CloseFilter, could be any slot after first closing slot. - Krog
            NBarsExit = 0;
            for (int slot = Data.Strategy.CloseSlot; slot < Data.Strategy.Slots; slot++)
            {
                if (Data.Strategy.Slot[slot].IndicatorName == "N Bars Exit")
                {
                    NBarsExit = (int)Data.Strategy.Slot[slot].IndParam.NumParam[0].Value;
                    break;
                }
            }

            return;
        }

        /// <summary>
        /// Reinitializes global variables.
        /// </summary>
        void DeinitTrade()
        {
            if (Configs.UseLogicalGroups)
            {
                Data.Strategy.Slot[Data.Strategy.OpenSlot].LogicalGroup  = ""; // Delete the group of open slot.
                Data.Strategy.Slot[Data.Strategy.CloseSlot].LogicalGroup = ""; // Delete the group of close slot.
            }

            NBarsExit = 0;

            return;
        }

        /// <summary>
        /// Checks whether entry price was reached.
        /// </summary>
        TradeDirection AnalyseEntryPrice()
        {
            int bar = Data.Bars - 1;

            double buyPrice  = 0;
            double sellPrice = 0;
            foreach (IndicatorComp component in Data.Strategy.Slot[Data.Strategy.OpenSlot].Component)
            {
                IndComponentType compType = component.DataType;
                if (compType == IndComponentType.OpenLongPrice)
                    buyPrice = component.Value[bar];
                else if (compType == IndComponentType.OpenShortPrice)
                    sellPrice = component.Value[bar];
                else if (compType == IndComponentType.OpenPrice || compType == IndComponentType.OpenClosePrice)
                    buyPrice = sellPrice = component.Value[bar];
            }

            double basePrice = Configs.LongTradeLogicPrice == "Bid" ? Data.Bid    : Data.Ask;
            double oldPrice  = Configs.LongTradeLogicPrice == "Bid" ? Data.OldBid : Data.OldAsk;

            bool canOpenLong = (buyPrice > oldPrice  + micron && buyPrice < basePrice + micron) ||
                               (buyPrice > basePrice - micron && buyPrice < oldPrice  - micron);

            bool canOpenShort = (sellPrice > Data.OldBid + micron && sellPrice < Data.Bid    + micron) ||
                                (sellPrice > Data.Bid    - micron && sellPrice < Data.OldBid - micron);

            TradeDirection direction = TradeDirection.None;

            if (canOpenLong && canOpenShort)
                direction = TradeDirection.Both;
            else if (canOpenLong)
                direction = TradeDirection.Long;
            else if (canOpenShort)
                direction = TradeDirection.Short;

            return direction;
        }

        /// <summary>
        /// Determines the direction of market entry.
        /// </summary>
        TradeDirection AnalyseEntryDirection()
        {
            int bar = Data.Bars - 1;

            // Do not send entry order when we are not on time
            if (openTimeExec == ExecutionTime.AtBarOpening && Data.Strategy.Slot[Data.Strategy.OpenSlot].Component[0].Value[bar] == 0)
                return TradeDirection.None;

            foreach (IndicatorSlot slot in Data.Strategy.Slot)
                if (slot.IndicatorName == "Enter Once")
                {
                    switch (slot.IndParam.ListParam[0].Text)
                    {
                        case "Enter no more than once a bar":
                            if (Data.Time[bar] == timeLastEntryBar)
                                return TradeDirection.None;
                            break;
                        case "Enter no more than once a day":
                            if (Data.Time[bar].DayOfYear == timeLastEntryBar.DayOfYear)
                                return TradeDirection.None;
                            break;
                        case "Enter no more than once a week":
                            if (Data.Time[bar].DayOfWeek >= timeLastEntryBar.DayOfWeek && Data.Time[bar] < timeLastEntryBar.AddDays(7))
                                return TradeDirection.None;
                            break;
                        case "Enter no more than once a month":
                            if (Data.Time[bar].Month == timeLastEntryBar.Month)
                                return TradeDirection.None;
                            break;
                        default:
                            break;
                    }
                }

            // Determining of the buy/sell entry prices.
            double buyPrice  = 0;
            double sellPrice = 0;
            foreach (IndicatorComp component in Data.Strategy.Slot[Data.Strategy.OpenSlot].Component)
            {
                IndComponentType compType = component.DataType;
                if (compType == IndComponentType.OpenLongPrice)
                    buyPrice = component.Value[bar];
                else if (compType == IndComponentType.OpenShortPrice)
                    sellPrice = component.Value[bar];
                else if (compType == IndComponentType.OpenPrice || compType == IndComponentType.OpenClosePrice)
                    buyPrice = sellPrice = component.Value[bar];
            }

            // Decide whether to open 
            bool canOpenLong  = buyPrice  > Data.InstrProperties.Point;
            bool canOpenShort = sellPrice > Data.InstrProperties.Point;

            if (Configs.UseLogicalGroups)
            {
                foreach (string group in openingLogicGroups)
                {
                    bool groupOpenLong  = canOpenLong;
                    bool groupOpenShort = canOpenShort;

                    AnalyseEntryLogicConditions(bar, group, buyPrice, sellPrice, ref groupOpenLong, ref groupOpenShort);

                    groupsAllowLong[group]  = groupOpenLong;
                    groupsAllowShort[group] = groupOpenShort;
                }

                bool groupLongEntry = false;
                foreach (KeyValuePair<string, bool> groupLong in groupsAllowLong)
                    if ((groupsAllowLong.Count > 1 && groupLong.Key != "All") || groupsAllowLong.Count == 1)
                        groupLongEntry = groupLongEntry || groupLong.Value;

                bool groupShortEntry = false;
                foreach (KeyValuePair<string, bool> groupShort in groupsAllowShort)
                    if ((groupsAllowShort.Count > 1 && groupShort.Key != "All") || groupsAllowShort.Count == 1)
                        groupShortEntry = groupShortEntry || groupShort.Value;

                canOpenLong  = canOpenLong  && groupLongEntry  && groupsAllowLong["All"];
                canOpenShort = canOpenShort && groupShortEntry && groupsAllowShort["All"];
            }
            else
            {
                AnalyseEntryLogicConditions(bar, "A", buyPrice, sellPrice, ref canOpenLong, ref canOpenShort);
            }

            TradeDirection direction = TradeDirection.None;
            if (canOpenLong && canOpenShort)
                direction = TradeDirection.Both;
            else if (canOpenLong)
                direction = TradeDirection.Long;
            else if (canOpenShort)
                direction = TradeDirection.Short;

            return direction;
        }

        /// <summary>
        /// checks if the opening logic conditions allow long or short entry.
        /// </summary>
        void AnalyseEntryLogicConditions(int bar, string group, double buyPrice, double sellPrice, ref bool canOpenLong, ref bool canOpenShort)
        {
            for (int slot = Data.Strategy.OpenSlot; slot <= Data.Strategy.CloseSlot; slot++)
            {
                if (Configs.UseLogicalGroups &&
                    Data.Strategy.Slot[slot].LogicalGroup != group && 
                    Data.Strategy.Slot[slot].LogicalGroup != "All")
                    continue;

                foreach (IndicatorComp component in Data.Strategy.Slot[slot].Component)
                {
                    if (component.DataType == IndComponentType.AllowOpenLong && component.Value[bar] < 0.5)
                        canOpenLong = false;

                    if (component.DataType == IndComponentType.AllowOpenShort && component.Value[bar] < 0.5)
                        canOpenShort = false;

                    if (component.PosPriceDependence != PositionPriceDependence.None)
                    {
                        double indicatorValue = component.Value[bar - component.UsePreviousBar];
                        switch (component.PosPriceDependence)
                        {
                            case PositionPriceDependence.PriceBuyHigher:
                                canOpenLong = canOpenLong == true && buyPrice > indicatorValue + micron;
                                break;
                            case PositionPriceDependence.PriceBuyLower:
                                canOpenLong = canOpenLong == true && buyPrice < indicatorValue - micron;
                                break;
                            case PositionPriceDependence.PriceSellHigher:
                                canOpenShort = canOpenShort == true && sellPrice > indicatorValue + micron;
                                break;
                            case PositionPriceDependence.PriceSellLower:
                                canOpenShort = canOpenShort == true && sellPrice < indicatorValue - micron;
                                break;
                            case PositionPriceDependence.BuyHigherSellLower:
                                canOpenLong  = canOpenLong  == true && buyPrice  > indicatorValue + micron;
                                canOpenShort = canOpenShort == true && sellPrice < indicatorValue - micron;
                                break;
                            case PositionPriceDependence.BuyLowerSelHigher:
                                canOpenLong  = canOpenLong  == true && buyPrice  < indicatorValue - micron;
                                canOpenShort = canOpenShort == true && sellPrice > indicatorValue + micron;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the size of an entry order.
        /// </summary>
        double AnalyseEntrySize(OrderDirection ordDir, ref PosDirection newPosDir)
        {
            double size = 0;
            PosDirection posDir = Data.PositionDirection;

            // Orders modification on a fly
            // Checks whether we are on the market 
            if (posDir == PosDirection.Long || posDir == PosDirection.Short)
            {   // We are on the market
                if (ordDir == OrderDirection.Buy  && posDir == PosDirection.Long ||
                    ordDir == OrderDirection.Sell && posDir == PosDirection.Short)
                {   // In case of a Same Dir Signal 
                    switch (Data.Strategy.SameSignalAction)
                    {
                        case SameDirSignalAction.Add:
                            if (Data.PositionLots + TradingSize(Data.Strategy.AddingLots) < maximumLots + Data.InstrProperties.LotStep / 2)
                            {   // Adding
                                size = TradingSize(Data.Strategy.AddingLots);
                                newPosDir = posDir;
                            }
                            else
                            {   // Cancel the Adding
                                size = 0;
                                newPosDir = posDir;
                            }
                            break;
                        case SameDirSignalAction.Winner:
                            if (Data.PositionProfit > micron && Data.PositionLots + TradingSize(Data.Strategy.AddingLots) < maximumLots + Data.InstrProperties.LotStep / 2)
                            {   // Adding
                                size = TradingSize(Data.Strategy.AddingLots);
                                newPosDir = posDir;
                            }
                            else
                            {   // Cancel the Adding
                                size = 0;
                                newPosDir = posDir;
                            }
                            break;
                        case SameDirSignalAction.Nothing:
                            size = 0;
                            newPosDir = posDir;
                            break;
                        default:
                            break;
                    }
                }
                else if (ordDir == OrderDirection.Buy  && posDir == PosDirection.Short ||
                         ordDir == OrderDirection.Sell && posDir == PosDirection.Long)
                {   // In case of an Opposite Dir Signal 
                    switch (Data.Strategy.OppSignalAction)
                    {
                        case OppositeDirSignalAction.Reduce:
                            if (Data.PositionLots > TradingSize(Data.Strategy.ReducingLots))
                            {   // Reducing
                                size = TradingSize(Data.Strategy.ReducingLots);
                                newPosDir = posDir;
                            }
                            else
                            {   // Closing
                                size = Data.PositionLots;
                                newPosDir = PosDirection.Closed;
                            }
                            break;
                        case OppositeDirSignalAction.Close:
                            size = Data.PositionLots;
                            newPosDir = PosDirection.Closed;
                            break;
                        case OppositeDirSignalAction.Reverse:
                            size = Data.PositionLots + TradingSize(Data.Strategy.EntryLots);
                            newPosDir = posDir == PosDirection.Long ? PosDirection.Short : PosDirection.Long;
                            break;
                        case OppositeDirSignalAction.Nothing:
                            size = 0;
                            newPosDir = posDir;
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {   // We are square on the market
                size = Math.Min(TradingSize(Data.Strategy.EntryLots), maximumLots);
                newPosDir = ordDir == OrderDirection.Buy ? PosDirection.Long : PosDirection.Short;
            }

            return size;
        }

        /// <summary>
        /// Checks if exit price was reached.
        /// </summary>
        TradeDirection AnalyseExitPrice()
        {
            IndicatorSlot  slot = Data.Strategy.Slot[Data.Strategy.CloseSlot];
            int bar = Data.Bars - 1;

            // Searching the exit price in the exit indicator slot.
            double buyPrice  = 0;
            double sellPrice = 0;
            for (int comp = 0; comp < slot.Component.Length; comp++)
            {
                IndComponentType compType = slot.Component[comp].DataType;

                if (compType == IndComponentType.CloseLongPrice)
                    sellPrice = slot.Component[comp].Value[bar];
                else if (compType == IndComponentType.CloseShortPrice)
                    buyPrice = slot.Component[comp].Value[bar];
                else if (compType == IndComponentType.ClosePrice || compType == IndComponentType.OpenClosePrice)
                    buyPrice = sellPrice = slot.Component[comp].Value[bar];
            }

            // We can close if the closing price is higher than zero.
            bool canCloseLong  = sellPrice > Data.InstrProperties.Point;
            bool canCloseShort = buyPrice  > Data.InstrProperties.Point;

            // Check if the closing price was reached.
            if (canCloseLong)
            {
                canCloseLong = (sellPrice > Data.OldBid + micron && sellPrice < Data.Bid    + micron) ||
                               (sellPrice > Data.Bid    - micron && sellPrice < Data.OldBid - micron);
            }
            if (canCloseShort)
            {
                canCloseShort = (buyPrice > Data.OldBid + micron && buyPrice < Data.Bid    + micron) ||
                                (buyPrice > Data.Bid    - micron && buyPrice < Data.OldBid - micron);
            }

            // Determine the trading direction.
            TradeDirection direction = TradeDirection.None;

            if (canCloseLong && canCloseShort)
                direction = TradeDirection.Both;
            else if (canCloseLong)
                direction = TradeDirection.Short;
            else if (canCloseShort)
                direction = TradeDirection.Long;

            return direction;
        }

        /// <summary>
        /// Gets the direction of closing of the current position.
        /// </summary>
        TradeDirection AnalyseExitDirection()
        {
            int bar = Data.Bars - 1;

            if (closeTimeExec == ExecutionTime.AtBarClosing &&
                Data.Strategy.Slot[Data.Strategy.CloseSlot].Component[0].Value[bar] == 0)
                return TradeDirection.None;

            if (Data.Strategy.CloseFilters == 0)
                return TradeDirection.Both;

            if (NBarsExit > 0)
                if (Data.PositionOpenTime.AddMinutes(NBarsExit * (int)Data.Period) < Data.Time[Data.Bars - 1].AddMinutes((int)Data.Period))
                    return TradeDirection.Both;
         
            TradeDirection direction = TradeDirection.None;

            if (Configs.UseLogicalGroups)
            {
                foreach (string group in closingLogicGroups)
                {
                    TradeDirection groupDirection = TradeDirection.Both;

                    // Determining of the slot direction
                    for (int slot = Data.Strategy.CloseSlot + 1; slot < Data.Strategy.Slots; slot++)
                    {
                        TradeDirection slotDirection = TradeDirection.None;
                        if (Data.Strategy.Slot[slot].LogicalGroup == group || Data.Strategy.Slot[slot].LogicalGroup == "all")
                        {
                            foreach (IndicatorComp component in Data.Strategy.Slot[slot].Component)
                                if (component.Value[bar] > 0)
                                    slotDirection = GetClosingDirection(slotDirection, component.DataType);

                            groupDirection = ReduceDirectionStatus(groupDirection, slotDirection);
                        }
                    }

                    direction = IncreaseDirectionStatus(direction, groupDirection);
                }
            }
            else
            {   // Search close filters for a closing signal.
                for (int slot = Data.Strategy.CloseSlot + 1; slot < Data.Strategy.Slots; slot++)
                    foreach (IndicatorComp component in Data.Strategy.Slot[slot].Component)
                        if (component.Value[bar] > 0)
                            direction = GetClosingDirection(direction, component.DataType);
                
            }

            return direction;
        }

        /// <summary>
        /// Reduces the status of baseDirection to direction.
        /// </summary>
        TradeDirection ReduceDirectionStatus(TradeDirection baseDirection, TradeDirection direction)
        {
            if (baseDirection == direction || direction == TradeDirection.Both)
                return baseDirection;

            if (baseDirection == TradeDirection.Both)
                return direction;

            return TradeDirection.None;
        }

        /// <summary>
        /// Increases the status of baseDirection to direction.
        /// </summary>
        TradeDirection IncreaseDirectionStatus(TradeDirection baseDirection, TradeDirection direction)
        {
            if (baseDirection == direction || direction == TradeDirection.None)
                return baseDirection;

            if (baseDirection == TradeDirection.None)
                return direction;

            return TradeDirection.Both;
        }

        /// <summary>
        /// Adjusts the closing direction.
        /// </summary>
        TradeDirection GetClosingDirection(TradeDirection baseDirection, IndComponentType compDataType)
        {
            TradeDirection newDirection = baseDirection;

            if (compDataType == IndComponentType.ForceClose)
            {
                newDirection = TradeDirection.Both;
            }
            else if (compDataType == IndComponentType.ForceCloseShort)
            {
                if (baseDirection == TradeDirection.None)
                    newDirection = TradeDirection.Long;
                else if (baseDirection == TradeDirection.Short)
                    newDirection = TradeDirection.Both;
            }
            else if (compDataType == IndComponentType.ForceCloseLong)
            {
                if (baseDirection == TradeDirection.None)
                    newDirection = TradeDirection.Short;
                else if (baseDirection == TradeDirection.Long)
                    newDirection = TradeDirection.Both;
            }

            return newDirection;
        }

        /// <summary>
        /// Calculates the Stop Loss distance.
        /// </summary>
        double GetStopLossPips(double lots)
        {
            double indStop = double.MaxValue;
            bool isIndStop = true;

            switch (Data.Strategy.Slot[Data.Strategy.CloseSlot].IndicatorName)
            {
                case "Account Percent Stop":
                    indStop = AccountPercentStopPips(Data.Strategy.Slot[Data.Strategy.CloseSlot].IndParam.NumParam[0].Value, lots);
                    break;
                case "ATR Stop":
                    indStop = Data.Strategy.Slot[Data.Strategy.CloseSlot].Component[0].Value[Data.Bars - 1];
                    break;
                case "Stop Loss":
                case "Stop Limit":
                    indStop = Data.Strategy.Slot[Data.Strategy.CloseSlot].IndParam.NumParam[0].Value;
                    break;
                case "Trailing Stop":
                case "Trailing Stop Limit":
                    indStop = Data.Strategy.Slot[Data.Strategy.CloseSlot].IndParam.NumParam[0].Value;
                    break;
                default:
                    isIndStop = false;
                    break;
            }

            double permStop = Data.Strategy.UsePermanentSL ? Data.Strategy.PermanentSL : double.MaxValue;
            double stopLoss = 0;

            if (isIndStop || Data.Strategy.UsePermanentSL)
            {
                stopLoss = Math.Min(indStop, permStop);
                if (stopLoss < Data.InstrProperties.StopLevel)
                    stopLoss = Data.InstrProperties.StopLevel;
                stopLoss = Math.Round(stopLoss);
            }

            return stopLoss;
        }

        /// <summary>
        /// Calculates the Take Profit distance.
        /// </summary>
        double GetTakeProfitPips()
        {
            double takeprofit = 0;
            double permLimit  = Data.Strategy.UsePermanentTP ? Data.Strategy.PermanentTP : double.MaxValue;
            double indLimit   = double.MaxValue;
            bool   isIndLimit = true;

            switch (Data.Strategy.Slot[Data.Strategy.CloseSlot].IndicatorName)
            {
                case "Take Profit":
                    indLimit = Data.Strategy.Slot[Data.Strategy.CloseSlot].IndParam.NumParam[0].Value;
                    break;
                case "Stop Limit":
                case "Trailing Stop Limit":
                    indLimit = Data.Strategy.Slot[Data.Strategy.CloseSlot].IndParam.NumParam[1].Value;
                    break;
                default:
                    isIndLimit = false;
                    break;
            }

            if (isIndLimit || Data.Strategy.UsePermanentTP)
            {
                takeprofit = Math.Min(indLimit, permLimit);
                if (takeprofit < Data.InstrProperties.StopLevel)
                    takeprofit = Data.InstrProperties.StopLevel;
                takeprofit = Math.Round(takeprofit);
            }

            return takeprofit;
        }

        /// <summary>
        /// Sets and sends an entry order.
        /// </summary>
        bool DoEntryTrade(TradeDirection tradeDir)
        {
            bool responseOK = false;

            double              price;
            OrderDirection      ordDir;
            OperationType       opType;
            MT4Bridge.OrderType type;
            JournalIcons        icon;

            if (timeLastEntryBar != Data.Time[Data.Bars - 1])
                isEnteredLong = isEnteredShort = false;

            if (tradeDir == TradeDirection.Long)
            {   // Buy
                if (isEnteredLong)
                    return false; // Only one long entry is allowed.

                price  = Data.Ask;
                ordDir = OrderDirection.Buy;
                opType = OperationType.Buy;
                type   = MT4Bridge.OrderType.Buy;
                icon   = JournalIcons.OrderBuy;
            }
            else if (tradeDir == TradeDirection.Short)
            {   // Sell
                if (isEnteredShort)
                    return false; // Only one short entry is allowed.

                price  = Data.Bid;
                ordDir = OrderDirection.Sell;
                opType = OperationType.Sell;
                type   = MT4Bridge.OrderType.Sell;
                icon   = JournalIcons.OrderSell;
            }
            else
            {   // Wrong direction of trade.
                return false;
            }

            PosDirection newPosDir = PosDirection.None;
            double size = AnalyseEntrySize(ordDir, ref newPosDir);
            if (size < Data.InstrProperties.MinLot / 2)
            {   // The entry trade is cancelled.  
                return false;
            }

            string symbol     = Data.Symbol;
            double lots       = size;
            int    slippage   = Configs.AutoSlippage ? (int)Data.InstrProperties.Spread * 3 : Configs.SlippageEntry;
            double stoploss   = GetStopLossPips(size);
            double takeprofit = GetTakeProfitPips();
            double point      = Data.InstrProperties.Point;

            string sStopLoss = "0";
            if (stoploss > 0)
            {
                double stopLossPrice = 0;
                if (newPosDir == PosDirection.Long)
                    stopLossPrice = Data.Bid - stoploss * point;
                else if (newPosDir == PosDirection.Short)
                    stopLossPrice = Data.Ask + stoploss * point;
                sStopLoss = stopLossPrice.ToString(Data.FF);
            }

            string sTakeProfit  = "0";
            if (takeprofit > 0)
            {
                double takeProfitPrice = 0;
                if (newPosDir == PosDirection.Long)
                    takeProfitPrice = Data.Bid + takeprofit * point;
                else if (newPosDir == PosDirection.Short)
                    takeProfitPrice = Data.Ask - takeprofit * point;
                sTakeProfit = takeProfitPrice.ToString(Data.FF);
            }

            if (Configs.PlaySounds)
                Data.SoundOrderSent.Play();

            JournalMessage jmsg = new JournalMessage(icon, DateTime.Now, string.Format(symbol + " " + Data.PeriodMTStr + " " +
                Language.T("An entry order sent") + ": " + Language.T(ordDir.ToString()) + " {0} " +
                (lots == 1 ? Language.T("lot") : Language.T("lots")) + " " + Language.T("at") + " {1}, " +
                Language.T("Stop Loss") + " {2}, " + Language.T("Take Profit") + " {3}",
                lots, price.ToString(Data.FF), sStopLoss, sTakeProfit));
            AppendJournalMessage(jmsg);

            string parameters = OrderParameters();

            int response = bridge.OrderSend(symbol, type, lots, price, slippage, stoploss, takeprofit, parameters);

            if (response >= 0)
            {   // The order was executed successfully.
                responseOK = true;

                Data.AddBarStats(opType, lots, price);

                timeLastEntryBar = Data.Time[Data.Bars - 1];
                if (type == MT4Bridge.OrderType.Buy)
                    isEnteredLong = true;
                else
                    isEnteredShort = true;

                Data.WrongStopLoss = 0;
                Data.WrongTakeProf = 0;
                Data.WrongStopsRetry = 0;
            }
            else
            {   // Error in operation execution.
                responseOK = false;

                if (Configs.PlaySounds)
                    Data.SoundError.Play();

                if (bridge.LastError == 0)
                    jmsg = new JournalMessage(JournalIcons.Warning, DateTime.Now,
                        Language.T("Operation execution") + ": " + Language.T("MetaTrader is not responding!").Replace("MetaTrader", Data.TerminalName));
                else
                    jmsg = new JournalMessage(JournalIcons.Error, DateTime.Now,
                        Language.T("MetaTrader failed to execute order! Returned").Replace("MetaTrader", Data.TerminalName) + ": " +
                        MT4Bridge.MT4_Errors.ErrorDescription(bridge.LastError));
                AppendJournalMessage(jmsg);

                Data.WrongStopLoss = (int)stoploss;
                Data.WrongTakeProf = (int)takeprofit;
            }

            return responseOK;
        }

        /// <summary>
        /// Sets and sends an exit order.
        /// </summary>
        bool DoExitTrade()
        {
            string symbol   = Data.Symbol;
            double lots     = Data.PositionLots;
            double price    = Data.PositionType == (int)MT4Bridge.OrderType.Buy ? Data.Bid : Data.Ask;
            int    slippage = Configs.AutoSlippage ? (int)Data.InstrProperties.Spread * 6 : Configs.SlippageExit;
            int    ticket   = Data.PositionTicket;

            if (Configs.PlaySounds)
                Data.SoundOrderSent.Play();

            JournalMessage jmsg = new JournalMessage(JournalIcons.OrderClose, DateTime.Now,
                string.Format(symbol + " " + Data.PeriodMTStr + " " + Language.T("An exit order sent") + ": " + Language.T("Close") + 
                " {0} " + (lots == 1 ? Language.T("lot") : Language.T("lots")) + " " + Language.T("at") + " {1}",
                lots, price.ToString(Data.FF)));
            AppendJournalMessage(jmsg);

            bool responseOK = bridge.OrderClose(ticket, lots, price, slippage);

            if (responseOK)
                Data.AddBarStats(OperationType.Close, lots, price);
            else
            {
                if (Configs.PlaySounds)
                    Data.SoundError.Play();

                if (bridge.LastError == 0)
                    jmsg = new JournalMessage(JournalIcons.Warning, DateTime.Now,
                        Language.T("Operation execution") + ": " + Language.T("MetaTrader is not responding!").Replace("MetaTrader", Data.TerminalName));
                else
                    jmsg = new JournalMessage(JournalIcons.Error, DateTime.Now,
                        Language.T("MetaTrader failed to execute order! Returned").Replace("MetaTrader", Data.TerminalName) + ": " +
                        MT4Bridge.MT4_Errors.ErrorDescription(bridge.LastError));
                AppendJournalMessage(jmsg);
            }

            Data.WrongStopLoss = 0;
            Data.WrongTakeProf = 0;
            Data.WrongStopsRetry = 0;

            return responseOK;
        }

        /// <summary>
        /// Calculates a trade.
        /// </summary>
        void CalculateTrade(TickType ticktype)
        {
            // Exit
            bool closeOK = false;
            if (closeStrPriceType != StrategyPriceType.CloseAndReverse && Data.PositionTicket != 0)
            {
                if (closeStrPriceType == StrategyPriceType.Open  && (ticktype == TickType.Open  || ticktype == TickType.OpenClose) ||
                    closeStrPriceType == StrategyPriceType.Close && (ticktype == TickType.Close || ticktype == TickType.OpenClose))
                {   // Exit at Bar Open or Bar Close.
                    TradeDirection direction = AnalyseExitDirection();
                    if (direction == TradeDirection.Both ||
                       (direction == TradeDirection.Long  && Data.PositionDirection == PosDirection.Short) ||
                       (direction == TradeDirection.Short && Data.PositionDirection == PosDirection.Long ) )
                        closeOK = DoExitTrade(); // Close the current position.
                }
                else if (closeStrPriceType == StrategyPriceType.Indicator)
                {   // Exit at an indicator value.
                    TradeDirection priceReached = AnalyseExitPrice();
                    if (priceReached == TradeDirection.Long)
                    {
                        TradeDirection direction = AnalyseExitDirection();
                        if (direction == TradeDirection.Long || direction == TradeDirection.Both)
                            if (Data.PositionDirection == PosDirection.Short)
                                closeOK = DoExitTrade(); // Close a short position.
                    }
                    else if (priceReached == TradeDirection.Short)
                    {
                        TradeDirection direction = AnalyseExitDirection();
                        if (direction == TradeDirection.Short || direction == TradeDirection.Both)
                            if (Data.PositionDirection == PosDirection.Long)
                                closeOK = DoExitTrade(); // Close a long position.
                    }
                    else if (priceReached == TradeDirection.Both)
                    {
                        TradeDirection direction = AnalyseExitDirection();
                        if (direction == TradeDirection.Long || direction == TradeDirection.Short || direction == TradeDirection.Both)
                           closeOK = DoExitTrade(); // Close the current position.
                    }
                }
            }

            // Checks if we closed a position successfully.
            if (closeOK)
                return;

            // This is to prevent new entry after Bar Closing has been executed.
            if (closeStrPriceType == StrategyPriceType.Close && ticktype == TickType.AfterClose)
                return;

            // Entry at Bar Open or Bar Close.
            bool openOK = false;
            if (openStrPriceType == StrategyPriceType.Open  && (ticktype == TickType.Open  || ticktype == TickType.OpenClose) ||
                openStrPriceType == StrategyPriceType.Close && (ticktype == TickType.Close || ticktype == TickType.OpenClose))
            {
                TradeDirection direction = AnalyseEntryDirection();
                if (direction == TradeDirection.Long || direction == TradeDirection.Short)
                    openOK = DoEntryTrade(direction);
            }
            else if (openStrPriceType == StrategyPriceType.Indicator)
            {   // Entry at an indicator value.
                TradeDirection priceReached = AnalyseEntryPrice();
                if (priceReached == TradeDirection.Long)
                {
                    TradeDirection direction = AnalyseEntryDirection();
                    if (direction == TradeDirection.Long || direction == TradeDirection.Both)
                        openOK = DoEntryTrade(TradeDirection.Long);
                }
                else if (priceReached == TradeDirection.Short)
                {
                    TradeDirection direction = AnalyseEntryDirection();
                    if (direction == TradeDirection.Short || direction == TradeDirection.Both)
                        openOK = DoEntryTrade(TradeDirection.Short);
                }
                else if (priceReached == TradeDirection.Both)
                {
                    TradeDirection direction = AnalyseEntryDirection();
                    if (direction == TradeDirection.Long || direction == TradeDirection.Short)
                        openOK = DoEntryTrade(direction);
                }
            }

            return;
        }

        /// <summary>
        /// Checks for failed set of SL and TP.
        /// </summary>
        /// <returns></returns>
        bool IsWrongStopsExecution()
        {
            if (Data.PositionDirection == PosDirection.Closed ||
                Data.PositionLots      == 0 ||
                Data.WrongStopsRetry   >= 4)
            {
                Data.WrongStopLoss   = 0;
                Data.WrongTakeProf   = 0;
                Data.WrongStopsRetry = 0;

                return false;
            }
            else if (Data.WrongStopLoss > 0 && Data.PositionTakeProfit < 0.001 ||
                     Data.WrongTakeProf > 0 && Data.PositionStopLoss   < 0.001)
                return true;

            return false;
        }

        /// <summary>
        /// Sets SL and TP after wrong execution.
        /// </summary>
        void ResendWrongStops()
        {
            string symbol = Data.Symbol;
            double lots   = NormalizeEntrySize(Data.PositionLots);
            double price  = Data.PositionDirection == PosDirection.Long ? Data.Bid : Data.Ask;
            int    ticket = Data.PositionTicket;

            if (Configs.PlaySounds)
                Data.SoundOrderSent.Play();

            double stoploss   = Data.WrongStopLoss;
            double takeprofit = Data.WrongTakeProf;

            string sStopLoss = "0";
            if (stoploss > 0)
            {
                double stopLossPrice = 0;
                if (Data.PositionDirection == PosDirection.Long)
                    stopLossPrice = Data.Bid - stoploss * Data.InstrProperties.Point;
                else if (Data.PositionDirection == PosDirection.Short)
                    stopLossPrice = Data.Ask + stoploss * Data.InstrProperties.Point;
                sStopLoss = stopLossPrice.ToString(Data.FF);
            }

            string sTakeProfit = "0";
            if (takeprofit > 0)
            {
                double takeProfitPrice = 0;
                if (Data.PositionDirection == PosDirection.Long)
                    takeProfitPrice = Data.Bid + takeprofit * Data.InstrProperties.Point;
                else if (Data.PositionDirection == PosDirection.Short)
                    takeProfitPrice = Data.Ask - takeprofit * Data.InstrProperties.Point;
                sTakeProfit = takeProfitPrice.ToString(Data.FF);
            }

            JournalMessage jmsg = new JournalMessage(JournalIcons.Warning, DateTime.Now, string.Format(symbol + " " + Data.PeriodMTStr + " " +
                Language.T("A modify order sent") + ": " + Language.T("Stop Loss") + " {0}, " + Language.T("Take Profit") + " {1}",
                sStopLoss, sTakeProfit));
            AppendJournalMessage(jmsg);

            string parameters = "TS1=" + 0 + ";BRE=" + 0;

            bool responseOK = bridge.OrderModify(ticket, price, stoploss, takeprofit, parameters);

            if (responseOK)
            {
                Data.AddBarStats(OperationType.Modify, lots, price);
                Data.WrongStopLoss = 0;
                Data.WrongTakeProf = 0;
                Data.WrongStopsRetry = 0;
            }
            else
            {
                if (Configs.PlaySounds)
                    Data.SoundError.Play();

                if (bridge.LastError == 0)
                    jmsg = new JournalMessage(JournalIcons.Warning, DateTime.Now,
                        Language.T("Operation execution") + ": " + Language.T("MetaTrader is not responding!").Replace("MetaTrader", Data.TerminalName));
                else
                    jmsg = new JournalMessage(JournalIcons.Error, DateTime.Now,
                        Language.T("MetaTrader failed to execute order! Returned").Replace("MetaTrader", Data.TerminalName) + ": " +
                        MT4Bridge.MT4_Errors.ErrorDescription(bridge.LastError));
                AppendJournalMessage(jmsg);
                Data.WrongStopsRetry++;
            }
        }

        /// <summary>
        /// Calculates the trading size in normalized lots
        /// </summary>
        double TradingSize(double size)
        {
            if (Data.Strategy.UseAccountPercentEntry)
                size = (size / 100) * Data.AccountEquity / Data.InstrProperties.MarginRequired;

            size = NormalizeEntrySize(size);

            return size;
        }

        /// <summary>
        /// Normalizes an entry order's size.
        /// <summary>
        double NormalizeEntrySize(double size)
        {
            double minlot  = Data.InstrProperties.MinLot;
            double maxlot  = Data.InstrProperties.MaxLot;
            double lotstep = Data.InstrProperties.LotStep;

            if (size <= 0)
                return (0);

            int steps = (int)Math.Round((size - minlot) / lotstep);
            size = minlot + steps * lotstep;

            if (size <= minlot)
                return (minlot);

            if (size >= maxlot)
                return (maxlot);

            return size;
        }

        /// <summary>
        /// Converts account percentage to Stop Loss in pips.
        /// </summary>
        double AccountPercentStopPips(double percent, double lots)
        {
            double balance   = Data.AccountBalance;
            double moneyrisk = balance * percent / 100;
            double spread    = Data.InstrProperties.Spread;
            double tickvalue = Data.InstrProperties.TickValue;

            double stoploss  = moneyrisk / (lots * tickvalue) - spread;

            return (stoploss);
        }
        
        /// <summary>
        /// Generates order parameters string
        /// <summary>
        string OrderParameters()
        {
            // Trailing Stop
            int trailTrailingStop = 0;
            string trailingStopMode = "TS0";
            switch (Data.Strategy.Slot[Data.Strategy.CloseSlot].IndicatorName)
            {
                case "Trailing Stop":
                case "Trailing Stop Limit":
                    trailTrailingStop = (int)Data.Strategy.Slot[Data.Strategy.CloseSlot].IndParam.NumParam[0].Value;
                    if (Data.Strategy.Slot[Data.Strategy.CloseSlot].IndParam.ListParam[1].Text == "Trails at a new top/bottom")
                        trailingStopMode = "TS1";
                    break;
                default:
                    break;
            }
            string trailingStopParam = trailingStopMode + "=" + trailTrailingStop;

            // Break Even
            int distanceBreakEven = 0;
            if (Data.Strategy.UseBreakEven)
                distanceBreakEven = Data.Strategy.BreakEven;
            string breakEvenParam = "BRE=" + distanceBreakEven;

            string parameters = trailingStopParam + ";" + breakEvenParam;

            return parameters;
        }
    }
}
