// Bar Stats
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// The direction of trade.
    /// </summary>
    public enum TradeDirection { Error, None, Long, Short, Both }

    /// <summary>
    /// The incoming tick type depending on its position in the bar.
    /// </summary>
    public enum TickType { Unset, Open, OpenClose, Regular, Close, AfterClose }

    /// <summary>
    /// The positions' direction
    /// </summary>
    public enum PosDirection { Error, None, Long, Short, Closed }

    /// <summary>
    /// Order direction
    /// </summary>
    public enum OrderDirection { Error, None, Buy, Sell }

    /// <summary>
    /// Entry / exit price type depending on the position of the bar.
    /// </summary>
    public enum StrategyPriceType {Unknown,  Open, Close, Indicator, CloseAndReverse }

    /// <summary>
    /// Type of operation send to the terminal.
    /// </summary>
    public enum OperationType { Error, Buy, Sell, Close, Modify }

    public class Operation
    {
        DateTime      barOpenTime;
        OperationType operationType;
        DateTime      operationTime;
        double        operationLots;
        double        operationPrice;

        public DateTime BarTime
        {
            get { return barOpenTime; }
            set { barOpenTime = value; }
        }

        public OperationType OperationType
        {
            get { return operationType; }
            set { operationType = value; }
        }

        public double OperationLots
        {
            get { return operationLots; }
            set { operationLots = value; }
        }

        public DateTime OperationTime
        {
            get { return operationTime; }
            set { operationTime = value; }
        }

        public double OperationPrice
        {
            get { return operationPrice; }
            set { operationPrice = value; }
        }

        public Operation(DateTime barOpenTime, OperationType operationType, DateTime operationTime, double operationLots, double operationPrice)
        {
            this.barOpenTime    = barOpenTime;
            this.operationType  = operationType;
            this.operationTime  = operationTime;
            this.operationLots  = operationLots;
            this.operationPrice = operationPrice;
        }
    }

    public class BarStats
    {
        DateTime        barTime   = DateTime.MinValue;
        PosDirection    posDir    = PosDirection.None;
        double          posPrice  = 0;
        double          posLots   = 0;
        bool            posFlag   = false; // It shows if there was a position during this bar.
        List<Operation> operation = new List<Operation>();

        public DateTime BarTime
        {
            get { return barTime; }
            set { barTime = value; }
        }

        public PosDirection PositionDir
        {
            get
            {
                if (posFlag && posDir == PosDirection.None)
                    return PosDirection.Closed;
                else
                    return posDir;
            }
            set
            {
                if (posDir == PosDirection.Long ||
                    posDir == PosDirection.Short)
                {
                    posFlag = true;
                }
                posDir = value;
            }
        }

        public double PositionPrice
        {
            get { return posPrice; }
            set { posPrice = value; }
        }

        public double PositionLots
        {
            get { return posLots; }
            set { posLots = value; }
        }

        public List<Operation> Operations
        {
            get { return operation; }
            set { operation = value; }
        }

        public BarStats(DateTime barTime, PosDirection posDir, double posPrice, double posLots)
        {
            this.barTime  = barTime;
            PositionDir   = posDir;
            this.posPrice = posPrice;
            this.posLots  = posLots;
        }

        BarStats()
        {
        }

        public BarStats Clone()
        {
            BarStats barStats  = new BarStats();
            barStats.barTime   = barTime;
            barStats.posDir    = posDir;
            barStats.posPrice  = posPrice;
            barStats.posLots   = posLots;
            barStats.posFlag   = posFlag;
            barStats.operation = operation.GetRange(0, operation.Count);

            return barStats;
        }
    }
}
