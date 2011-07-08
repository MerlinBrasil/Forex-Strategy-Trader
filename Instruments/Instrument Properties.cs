// Instrument_Properties Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Contains the instrument properties.
    /// </summary>
    public class Instrument_Properties
    {
        string symbol;
        int    digits;
        int    lotSize;
        double stopLevel;
        double spread;
        double swapLong;
        double swapShort;
        double tickValue;
        double minLot;
        double maxLot;
        double lotStep;
        double marginRequired;

        public string Symbol         { get { return symbol;     } set { symbol    = value; } }
        public int    Digits         { get { return digits;     } set { digits    = value; } }
        public int    LotSize        { get { return lotSize;    } set { lotSize   = value; } }
        public double StopLevel      { get { return stopLevel;  } set { stopLevel = value; } }
        public double Spread         { get { return spread;     } set { spread    = value; } }
        public double SwapLong       { get { return swapLong;   } set { swapLong  = value; } }
        public double SwapShort      { get { return swapShort;  } set { swapShort = value; } }
        public double TickValue      { get { return tickValue;  } set { tickValue = value; } }
        public double MinLot         { get { return minLot;     } set { minLot    = value; } }
        public double MaxLot         { get { return maxLot;     } set { maxLot    = value; } }
        public double LotStep        { get { return lotStep;    } set { lotStep   = value; } }
        public double MarginRequired { get { return marginRequired; } set { marginRequired = value; } }
        public double Point          { get { return (1 / Math.Pow(10, digits)); } }

        /// <summary>
        /// Constructor
        /// </summary>
        public Instrument_Properties(string sSymbol)
        {
            this.symbol    = sSymbol;
            digits         = 4;
            lotSize        = 10000;
            stopLevel      = 5;
            spread         = 2;
            swapLong       = 1;
            swapShort      = -1;
            tickValue      = lotSize * Point;
            minLot         = 0.01;
            maxLot         = 100;
            lotStep        = 0.01;
            marginRequired = 1000;
        }

        /// <summary>
        /// Clones the Instrument_Properties.
        /// </summary>
        public Instrument_Properties Clone()
        {
            Instrument_Properties copy = new Instrument_Properties(symbol);

            copy.Symbol         = Symbol;
            copy.Digits         = Digits;
            copy.LotSize        = LotSize;
            copy.Spread         = Spread;
            copy.StopLevel      = StopLevel;
            copy.SwapLong       = SwapLong;
            copy.SwapShort      = SwapShort;
            copy.TickValue      = TickValue;
            copy.MinLot         = MinLot;
            copy.MaxLot         = MaxLot;
            copy.LotStep        = LotStep;
            copy.MarginRequired = MarginRequired;

            return copy;
        }
    }
}
