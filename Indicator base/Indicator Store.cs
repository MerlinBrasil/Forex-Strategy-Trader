// Indicator_Store Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;

namespace Forex_Strategy_Trader
{
    public static class Indicator_Store
    {
        static Dictionary<string, Indicator> originalIndicators = new Dictionary<string, Indicator>
        {
            {"Accelerator Oscillator",    new Accelerator_Oscillator(SlotTypes.NotDefined)},
            {"Account Percent Stop",      new Account_Percent_Stop(SlotTypes.NotDefined)},
            {"Accumulation Distribution", new Accumulation_Distribution(SlotTypes.NotDefined)},
            {"ADX",                       new ADX(SlotTypes.NotDefined)},
            {"Alligator",                 new Alligator(SlotTypes.NotDefined)},
            {"Aroon Histogram",           new Aroon_Histogram(SlotTypes.NotDefined)},
            {"ATR MA Oscillator",         new ATR_MA_Oscillator(SlotTypes.NotDefined)},
            {"ATR Stop",                  new ATR_Stop(SlotTypes.NotDefined)},
            {"Average True Range",        new Average_True_Range(SlotTypes.NotDefined)},
            {"Awesome Oscillator",        new Awesome_Oscillator(SlotTypes.NotDefined)},
            {"Balance of Power",          new Balance_of_Power(SlotTypes.NotDefined)},
            {"Bar Closing",               new Bar_Closing(SlotTypes.NotDefined)},
            {"Bar Opening",               new Bar_Opening(SlotTypes.NotDefined)},
            {"Bar Range",                 new Bar_Range(SlotTypes.NotDefined)},
            {"BBP MA Oscillator",         new BBP_MA_Oscillator(SlotTypes.NotDefined)},
            {"Bears Power",               new Bears_Power(SlotTypes.NotDefined)},
            {"Bollinger Bands",           new Bollinger_Bands(SlotTypes.NotDefined)},
            {"Bulls Bears Power",         new Bulls_Bears_Power(SlotTypes.NotDefined)},
            {"Bulls Power",               new Bulls_Power(SlotTypes.NotDefined)},
            {"CCI MA Oscillator",         new CCI_MA_Oscillator(SlotTypes.NotDefined)},
            {"Close and Reverse",         new Close_and_Reverse(SlotTypes.NotDefined)},
            {"Commodity Channel Index",   new Commodity_Channel_Index(SlotTypes.NotDefined)},
            {"Cumulative Sum",            new Cumulative_Sum(SlotTypes.NotDefined)},
            {"Data Bars Filter",          new Data_Bars_Filter(SlotTypes.NotDefined)},
            {"Date Filter",               new Date_Filter(SlotTypes.NotDefined)},
            {"Day Closing",               new Day_Closing(SlotTypes.NotDefined)},
            {"Day of Week",               new Day_of_Week(SlotTypes.NotDefined)},
            {"Day Opening",               new Day_Opening(SlotTypes.NotDefined)},
            {"DeMarker",                  new DeMarker(SlotTypes.NotDefined)},
            {"Detrended Oscillator",      new Detrended_Oscillator(SlotTypes.NotDefined)},
            {"Directional Indicators",    new Directional_Indicators(SlotTypes.NotDefined)},
            {"Donchian Channel",          new Donchian_Channel(SlotTypes.NotDefined)},
            {"Ease of Movement",          new Ease_of_Movement(SlotTypes.NotDefined)},
            {"Enter Once",                new Enter_Once(SlotTypes.NotDefined)},
            {"Entry Hour",                new Entry_Hour(SlotTypes.NotDefined)},
            {"Entry Time",                new Entry_Time(SlotTypes.NotDefined)},
            {"Envelopes",                 new Envelopes(SlotTypes.NotDefined)},
            {"Exit Hour",                 new Exit_Hour(SlotTypes.NotDefined)},
            //{"Fibonacci",                 new Fibonacci(SlotTypes.NotDefined)},
            {"Fisher Transform",          new Fisher_Transform(SlotTypes.NotDefined)},
            {"Force Index",               new Force_Index(SlotTypes.NotDefined)},
            {"Fractal",                   new Fractal(SlotTypes.NotDefined)},
            {"Heiken Ashi",               new Heiken_Ashi(SlotTypes.NotDefined)},
            {"Hourly High Low",           new Hourly_High_Low(SlotTypes.NotDefined)},
            {"Ichimoku Kinko Hyo",        new Ichimoku_Kinko_Hyo(SlotTypes.NotDefined)},
            {"Inside Bar",                new Inside_Bar(SlotTypes.NotDefined)},
            {"Keltner Channel",           new Keltner_Channel(SlotTypes.NotDefined)},
            {"Long or Short",             new Long_or_Short(SlotTypes.NotDefined)},
            {"Lot Limiter",               new Lot_Limiter(SlotTypes.NotDefined)},
            {"MA Oscillator",             new MA_Oscillator(SlotTypes.NotDefined)},
            {"MACD Histogram",            new MACD_Histogram(SlotTypes.NotDefined)},
            {"MACD",                      new MACD(SlotTypes.NotDefined)},
            {"Market Facilitation Index", new Market_Facilitation_Index(SlotTypes.NotDefined)},
            {"Momentum MA Oscillator",    new Momentum_MA_Oscillator(SlotTypes.NotDefined)},
            {"Momentum",                  new Momentum(SlotTypes.NotDefined)},
            {"Money Flow Index",          new Money_Flow_Index(SlotTypes.NotDefined)},
            {"Money Flow",                new Money_Flow(SlotTypes.NotDefined)},
            {"Moving Average",            new Moving_Average(SlotTypes.NotDefined)},
            {"Moving Averages Crossover", new Moving_Averages_Crossover(SlotTypes.NotDefined)},
            {"N Bars Exit",               new N_Bars_Exit(SlotTypes.NotDefined)},
            {"Narrow Range",              new Narrow_Range(SlotTypes.NotDefined)},
            {"OBOS MA Oscillator",        new OBOS_MA_Oscillator(SlotTypes.NotDefined)},
            {"On Balance Volume",         new On_Balance_Volume(SlotTypes.NotDefined)},
            {"Oscillator of ATR",         new Oscillator_of_ATR(SlotTypes.NotDefined)},
            {"Oscillator of BBP",         new Oscillator_of_BBP(SlotTypes.NotDefined)},
            {"Oscillator of CCI",         new Oscillator_of_CCI(SlotTypes.NotDefined)},
            {"Oscillator of MACD",        new Oscillator_of_MACD(SlotTypes.NotDefined)},
            {"Oscillator of Momentum",    new Oscillator_of_Momentum(SlotTypes.NotDefined)},
            {"Oscillator of OBOS",        new Oscillator_of_OBOS(SlotTypes.NotDefined)},
            {"Oscillator of ROC",         new Oscillator_of_ROC(SlotTypes.NotDefined)},
            {"Oscillator of RSI",         new Oscillator_of_RSI(SlotTypes.NotDefined)},
            {"Oscillator of Trix",        new Oscillator_of_Trix(SlotTypes.NotDefined)},
            {"Overbought Oversold Index", new Overbought_Oversold_Index(SlotTypes.NotDefined)},
            {"Parabolic SAR",             new Parabolic_SAR(SlotTypes.NotDefined)},
            {"Percent Change",            new Percent_Change(SlotTypes.NotDefined)},
            {"Pivot Points",              new Pivot_Points(SlotTypes.NotDefined)},
            {"Previous Bar Closing",      new Previous_Bar_Closing(SlotTypes.NotDefined)},
            {"Previous Bar Opening",      new Previous_Bar_Opening(SlotTypes.NotDefined)},
            {"Previous High Low",         new Previous_High_Low(SlotTypes.NotDefined)},
            {"Price Move",                new Price_Move(SlotTypes.NotDefined)},
            {"Price Oscillator",          new Price_Oscillator(SlotTypes.NotDefined)},
            {"Random Filter",             new Random_Filter(SlotTypes.NotDefined)},
            {"Rate of Change",            new Rate_of_Change(SlotTypes.NotDefined)},
            {"Relative Vigor Index",      new Relative_Vigor_Index(SlotTypes.NotDefined)},
            {"ROC MA Oscillator",         new ROC_MA_Oscillator(SlotTypes.NotDefined)},
            {"Ross Hook",                 new Ross_Hook(SlotTypes.NotDefined)},
            {"Round Number",              new Round_Number(SlotTypes.NotDefined)},
            {"RSI MA Oscillator",         new RSI_MA_Oscillator(SlotTypes.NotDefined)},
            {"RSI",                       new RSI(SlotTypes.NotDefined)},
            {"Standard Deviation",        new Standard_Deviation(SlotTypes.NotDefined)},
            {"Starc Bands",               new Starc_Bands(SlotTypes.NotDefined)},
            {"Steady Bands",              new Steady_Bands(SlotTypes.NotDefined)},
            {"Stochastics",               new Stochastics(SlotTypes.NotDefined)},
            {"Stop Limit",                new Stop_Limit(SlotTypes.NotDefined)},
            {"Stop Loss",                 new Stop_Loss(SlotTypes.NotDefined)},
            {"Take Profit",               new Take_Profit(SlotTypes.NotDefined)},
            {"Top Bottom Price",          new Top_Bottom_Price(SlotTypes.NotDefined)},
            {"Trailing Stop Limit",       new Trailing_Stop_Limit(SlotTypes.NotDefined)},
            {"Trailing Stop",             new Trailing_Stop(SlotTypes.NotDefined)},
            {"Trix Index",                new Trix_Index(SlotTypes.NotDefined)},
            {"Trix MA Oscillator",        new Trix_MA_Oscillator(SlotTypes.NotDefined)},
            {"Week Closing",              new Week_Closing(SlotTypes.NotDefined)},
            {"Williams' Percent Range",   new Williams_Percent_Range(SlotTypes.NotDefined)},
        };

        // Stores the custom indicators
        static SortableDictionary<string, Indicator> customIndicators = new SortableDictionary<string, Indicator>();

        // Stores all the indicators
        static SortableDictionary<string, Indicator> indicators = new SortableDictionary<string, Indicator>();

        // Stores all the closing Point indicators, which can be used with Closing Logic conditions
        static List<string> closingIndicatorsWithClosingFilters = new List<string>();

        /// <summary>
        /// Gets the names of all the original indicators
        /// </summary>
        public static List<string> OriginalIndicatorNames
        {
            get { return new List<string>(originalIndicators.Keys); }
        }

        /// <summary>
        /// Gets the names of all custom indicators
        /// </summary>
        public static List<string> CustomIndicatorNames
        {
            get { return new List<string>(customIndicators.Keys); }
        }

        /// <summary>
        /// Gets the names of all indicators.
        /// </summary>
        public static List<string> IndicatorNames
        {
            get { return new List<string>(indicators.Keys); }
        }

        /// <summary>
        /// Gets the names of all losing Point indicators that allow use of Closing Filter indicators.
        /// </summary>
        public static List<string> ClosingIndicatorsWithClosingFilters 
        {
            get { return closingIndicatorsWithClosingFilters; } 
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        static Indicator_Store()
        {
            foreach (KeyValuePair<string, Indicator> record in originalIndicators)
                indicators.Add(record.Key, record.Value);

            return;
        }

        /// <summary>
        /// Gets the names of all indicators for a given slot type.
        /// </summary>
        public static List<string> GetIndicatorNames(SlotTypes slotType)
        {
            List<string> list = new List<string>();

            foreach (KeyValuePair<string, Indicator> record in indicators)
                if (record.Value.TestPossibleSlot(slotType))
                    list.Add(record.Value.IndicatorName);

            return list;
        }

        /// <summary>
        /// Resets the custom indicators in the custom indicators list.
        /// </summary>
        public static void ResetCustomIndicators(List<Indicator> indicatorList)
        {
            customIndicators.Clear();

            if (indicatorList == null)
                return;

            foreach (Indicator indicator in indicatorList)
                if (!customIndicators.ContainsKey(indicator.IndicatorName))
                    customIndicators.Add(indicator.IndicatorName, indicator);

            customIndicators.Sort();

            return;
        }

        /// <summary>
        /// Clears the indicator list and adds to it the original indicators.
        /// </summary>
        public static void CombineAllIndicators()
        {
            indicators.Clear();

            foreach (KeyValuePair<string, Indicator> record in originalIndicators)
                if (!indicators.ContainsKey(record.Key))
                    indicators.Add(record.Key, record.Value);

            foreach (KeyValuePair<string, Indicator> record in customIndicators)
                if (!indicators.ContainsKey(record.Key))
                    indicators.Add(record.Key, record.Value);

            indicators.Sort();

            List<string> closePointIndicators = GetIndicatorNames(SlotTypes.Close);

            foreach (string indicatorName in closePointIndicators)
            {
                Indicator indicator = ConstructIndicator(indicatorName, SlotTypes.Close);
                if (indicator.AllowClosingFilters)
                    closingIndicatorsWithClosingFilters.Add(indicatorName);
            }

            return;
        }

        /// <summary>
        /// Construct an indicator with specified name and slot type.
        /// </summary>
        public static Indicator ConstructIndicator(string indicatorName, SlotTypes slotType)
        {
            if (!indicators.ContainsKey(indicatorName))
            {
                System.Windows.Forms.MessageBox.Show("There is no indicator named: " + indicatorName);
                return null;
            }

            Type   indicatorType = indicators[indicatorName].GetType();
            Type[] parameterType = new Type[] { slotType.GetType() };
            System.Reflection.ConstructorInfo constructorInfo = indicatorType.GetConstructor(parameterType);
            Indicator indicator = (Indicator)constructorInfo.Invoke(new object[] { slotType });

            return indicator;
        }

    }
}
