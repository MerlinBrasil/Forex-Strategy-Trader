// Chart_Data Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.Text;

namespace Forex_Strategy_Trader
{
    public class Chart_Data
    {
        Instrument_Properties instrumentProperties;
        int bars;
        DateTime[] time;
        double[] open;
        double[] high;
        double[] low;
        double[] close;
        int[] volume;
        Strategy strategy;
        int strategyFirstBar;
        Dictionary<DateTime, BarStats> barStats;


        string strategyName;

        public string StrategyName
        {
            get { return strategyName; }
            set { strategyName = value; }
        }
        string symbol;

        public string Symbol
        {
            get { return symbol; }
            set { symbol = value; }
        }
        string periodStr;

        public string PeriodStr
        {
            get { return periodStr; }
            set { periodStr = value; }
        }
        double bid;

        public double Bid
        {
            get { return bid; }
            set { bid = value; }
        }
        PosDirection positionDirection;

        public PosDirection PositionDirection
        {
            get { return positionDirection; }
            set { positionDirection = value; }
        }
        double positionOpenPrice;

        public double PositionOpenPrice
        {
            get { return positionOpenPrice; }
            set { positionOpenPrice = value; }
        }
        double positionProfit;

        public double PositionProfit
        {
            get { return positionProfit; }
            set { positionProfit = value; }
        }
        double positionTakeProfit;

        public double PositionTakeProfit
        {
            get { return positionTakeProfit; }
            set { positionTakeProfit = value; }
        }
        double positionStopLoss;

        public double PositionStopLoss
        {
            get { return positionStopLoss; }
            set { positionStopLoss = value; }
        }

        public Instrument_Properties InstrumentProperties
        {
            get { return instrumentProperties; }
            set { instrumentProperties = value; }
        }

        public int Bars
        {
            get { return bars; }
            set { bars = value; }
        }

        public DateTime[] Time
        {
            get { return time; }
            set { time = value; }
        }

        public double[] Open
        {
            get { return open; }
            set { open = value; }
        }

        public double[] High
        {
            get { return high; }
            set { high = value; }
        }

        public double[] Low
        {
            get { return low; }
            set { low = value; }
        }

        public double[] Close
        {
            get { return close; }
            set { close = value; }
        }

        public int[] Volume
        {
            get { return volume; }
            set { volume = value; }
        }

        public int FirstBar
        {
            get { return strategyFirstBar; }
            set { strategyFirstBar = value; }
        }

        public Strategy Strategy
        {
            get { return strategy; }
            set { strategy = value; }
        }

        public Dictionary<DateTime, BarStats> BarStatistics
        {
            get { return barStats; }
            set { barStats = value; }
        }

    }
}
