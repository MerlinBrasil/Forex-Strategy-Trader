// Instrument Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Trader
{
    public class Instrument
    {
        Instrument_Properties instrProperties;
        int period;
        int bars;
        Bar[] DataBar;

        // General instrument info
        public string   Symbol { get { return instrProperties.Symbol; } }
        public int      Period { get { return period; } }
        public double   Point  { get { return instrProperties.Point; } }
        public int      Bars   { get { return bars; } }
        
        // Bar info
        public DateTime	Time	(int iBar) { return DataBar[iBar].Time  ; }
        public double	Open	(int iBar) { return DataBar[iBar].Open  ; }
        public double	High	(int iBar) { return DataBar[iBar].High  ; }
        public double	Low		(int iBar) { return DataBar[iBar].Low   ; }
        public double	Close	(int iBar) { return DataBar[iBar].Close ; }
        public int		Volume	(int iBar) { return DataBar[iBar].Volume; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Instrument(Instrument_Properties instrProperties, int iPeriod)
        {
            this.instrProperties = instrProperties;
            this.period = iPeriod;
        }

        /// <summary>
        /// Loads the data file
        /// </summary>
        /// <returns>0 - success</returns>
        public int LoadResourceData()
        {
            string data = Properties.Resources.EURUSD1440;

            Data_Parser parser = new Data_Parser(data);
            int iResult = parser.Parse();

            if (iResult == 0)
            {
                DataBar = parser.Bar;
                bars = parser.Bars;
            }

            return iResult;
        }
    }
}