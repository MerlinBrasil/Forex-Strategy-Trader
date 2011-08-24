// Indicator class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Class Indicator.
    /// </summary>
    public class Indicator
    {
        string    indicatorName;
        SlotTypes possibleSlots;
        bool      isSeparatedChart;
        double    sepChartMinValue;
        double    sepChartMaxValue;
        double[]  adSpecialValues;
        bool      isDescreteValues;
        bool      isCustomIndicator;
        string    warningMessage;
        bool      allowClosingFilters;

        IndicatorParam  parameters;
        IndicatorComp[] component;

        string entryPointLongDescription   = "Not defined";
        string entryPointShortDescription  = "Not defined";
        string entryFilterLongDescription  = "Not defined";
        string entryFilterShortDescription = "Not defined";
        string exitPointLongDescription    = "Not defined";
        string exitPointShortDescription   = "Not defined";
        string exitFilterLongDescription   = "Not defined";
        string exitFilterShortDescription  = "Not defined";

        /// <summary>
        /// Gets or sets the indicator name.
        /// </summary>
        public string IndicatorName { get { return indicatorName; } set { indicatorName = value; } }

        /// <summary>
        /// Gets or sets the possible slots 
        /// </summary>
        protected SlotTypes PossibleSlots { set { possibleSlots = value; } }

        /// <summary>
        /// Tests if this is one of the possible slots.
        /// </summary>
        /// <param name="slotType">The slot we test.</param>
        /// <returns>True if the slot is possible.</returns>
        public bool TestPossibleSlot(SlotTypes slotType)
        {
            if ((slotType & possibleSlots) == SlotTypes.Open)
                return true;

            if ((slotType & possibleSlots) == SlotTypes.OpenFilter) 
                return true;

            if ((slotType & possibleSlots) == SlotTypes.Close)
                return true;

            if ((slotType & possibleSlots) == SlotTypes.CloseFilter)
                return true;

            return false;
        }

        /// <summary>
        /// Gets or sets the indicator current parameters.
        /// </summary>
        public IndicatorParam IndParam { get { return parameters; } set { parameters = value; } }

        /// <summary>
        /// If the chart is drown in separated panel.
        /// </summary>
        public bool SeparatedChart { get { return isSeparatedChart; } protected set { isSeparatedChart = value; } }

        /// <summary>
        /// Gets the indicator components.
        /// </summary>
        public IndicatorComp[] Component { get { return component; } protected set { component = value; } }

        /// <summary>
        /// Gets the indicator's special values.
        /// </summary>
        public double[] SpecialValues { get { return adSpecialValues; } protected set { adSpecialValues = value; } }

        /// <summary>
        /// Gets the indicator's min value.
        /// </summary>
        public double SeparatedChartMinValue { get { return sepChartMinValue; } protected set { sepChartMinValue = value; } }

        /// <summary>
        /// Gets the indicator's max value.
        /// </summary>
        public double SeparatedChartMaxValue { get { return sepChartMaxValue; } protected set { sepChartMaxValue = value; } }

        /// <summary>
        /// Shows if the indicator has discrete values.
        /// </summary>
        protected bool IsDescreteValues { set { isDescreteValues = value; } }

        /// <summary>
        /// Gets or sets a warning message about the indicator
        /// </summary>
        public string WarningMessage { get { return warningMessage; } protected set { warningMessage = value; } }

        /// <summary>
        /// Shows if a closing point indicator can be used with closing logic conditions.
        /// </summary>
        public bool AllowClosingFilters { get { return allowClosingFilters; } protected set { allowClosingFilters = value; } }

        /// <summary>
        /// Shows if the indicator is custom.
        /// </summary>
        public bool CustomIndicator { get { return isCustomIndicator; } protected set { isCustomIndicator = value; } }

        /// <summary>
        /// Gets the indicator Entry Point Long Description
        /// </summary>
        public string EntryPointLongDescription { get { return entryPointLongDescription; } protected set { entryPointLongDescription = value; } }

        /// <summary>
        /// Gets the indicator Entry Point Short Description
        /// </summary>
        public string EntryPointShortDescription { get { return entryPointShortDescription; } protected set { entryPointShortDescription = value; } }

        /// <summary>
        /// Gets the indicator Exit Point Long Description
        /// </summary>
        public string ExitPointLongDescription { get { return exitPointLongDescription; } protected set { exitPointLongDescription = value; } }

        /// <summary>
        /// Gets the indicator Exit Point Short Description
        /// </summary>
        public string ExitPointShortDescription { get { return exitPointShortDescription; } protected set { exitPointShortDescription = value; } }

        /// <summary>
        /// Gets the indicator Entry Filter Description
        /// </summary>
        public string EntryFilterLongDescription { get { return entryFilterLongDescription; } protected set { entryFilterLongDescription = value; } }

        /// <summary>
        /// Gets the indicator Exit Filter Description
        /// </summary>
        public string ExitFilterLongDescription { get { return exitFilterLongDescription; } protected set { exitFilterLongDescription = value; } }

        /// <summary>
        /// Gets the indicator Entry Filter Description
        /// </summary>
        public string EntryFilterShortDescription { get { return entryFilterShortDescription; } protected set { entryFilterShortDescription = value; } }

        /// <summary>
        /// Gets the indicator Exit Filter Description
        /// </summary>
        public string ExitFilterShortDescription { get { return exitFilterShortDescription; } protected set { exitFilterShortDescription = value; } }

        /// <summary>
        /// The default constructor
        /// </summary>
        public Indicator()
        {
            indicatorName       = string.Empty;
            possibleSlots       = SlotTypes.NotDefined;
            isSeparatedChart    = false;
            sepChartMinValue    = double.MaxValue;
            sepChartMaxValue    = double.MinValue;
            isDescreteValues    = false;
            isCustomIndicator   = false;
            warningMessage      = string.Empty;
            allowClosingFilters = false;
            adSpecialValues     = new double[] { };
            parameters          = new IndicatorParam();
            component           = new IndicatorComp[] { };
        }

        /// <summary>
        /// Calculates the components
        /// </summary>
        public virtual void Calculate(SlotTypes slotType)
        {
        }

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public virtual void SetDescription(SlotTypes slotType)
        {
        }

        /// <summary>
        /// Time frame of the loaded historical data
        /// </summary>
        protected static DataPeriods Period { get { return Data.Period; } }

        /// <summary>
        /// The minimal price change.
        /// </summary>
        protected static double Point { get { return Data.InstrProperties.Point; } }

        /// <summary>
        /// Number of digits after the decimal point of the historical data.
        /// </summary>
        protected static int Digits { get { return Data.InstrProperties.Digits; } }

        /// <summary>
        /// Number of loaded bars
        /// </summary>
        protected static int Bars { get { return Data.Bars; } }

        /// <summary>
        /// Obsolete! Use 'Time' instead.
        /// </summary>
        protected static DateTime[] Date { get { return Data.Time; } }

        /// <summary>
        /// Bar opening date and time
        /// </summary>
        protected static DateTime[] Time { get { return Data.Time; } }

        /// <summary>
        /// Bar opening price
        /// </summary>
        protected static double[] Open { get { return Data.Open; } }

        /// <summary>
        /// Bar highest price
        /// </summary>
        protected static double[] High { get { return Data.High; } }

        /// <summary>
        /// Bar lowest price
        /// </summary>
        protected static double[] Low { get { return Data.Low; } }

        /// <summary>
        /// Bar closing price
        /// </summary>
        protected static double[] Close { get { return Data.Close; } }

        /// <summary>
        /// Bar volume
        /// </summary>
        protected static int[] Volume { get { return Data.Volume; } }

        /// <summary>
        /// Server time
        /// </summary>
        protected static DateTime ServerTime { get { return Data.ServerTime; } }

        /// <summary>
        /// Calculates the base price.
        /// </summary>
        /// <param name="price">The base price type.</param>
        /// <returns>Base price.</returns>
        protected static double[] Price(BasePrice price)
        {
            double[] adPrice = new double[Bars];

            switch(price)
            {
                case BasePrice.Open:
                    adPrice = Open;
                    break;
                case BasePrice.High:
                    adPrice = High;
                    break;
                case BasePrice.Low:
                    adPrice = Low;
                    break;
                case BasePrice.Close:
                    adPrice = Close;
                    break;
                case BasePrice.Median:
                    for (int bar = 0; bar < Bars; bar++)
                        adPrice[bar] = (Low[bar] + High[bar]) / 2;
                    break;
                case BasePrice.Typical:
                    for (int bar = 0; bar < Bars; bar++)
                        adPrice[bar] = (Low[bar] + High[bar] + Close[bar]) / 3;
                    break;
                case BasePrice.Weighted:
                    for (int bar = 0; bar < Bars; bar++)
                        adPrice[bar] = (Low[bar] + High[bar] + 2 * Close[bar]) / 4;
                    break;
                default:
                    break;
            }
            return adPrice;
        }

        /// <summary>
        /// Sets the check box "Use previous bar value"
        /// </summary>
        protected bool PrepareUsePrevBarValueCheckBox(SlotTypes slotType)
        {
            return Data.Strategy.PrepareUsePrevBarValueCheckBox(slotType);
        }

        /// <summary>
        /// Calculates a Moving Average
        /// </summary>
        /// <param name="iPeriod">Period</param>
        /// <param name="iShift">Shift</param>
        /// <param name="maMethod">Method of calculation</param>
        /// <param name="afSource">The array of source data</param>
        /// <returns>the Moving Average</returns>
        protected static double[] MovingAverage(int period, int shift, MAMethod maMethod, double[] sourceData)
        {
            int      bar;
            double   sum;
            double[] movingAverage = new double[Bars];

            if (period <= 1 && shift == 0)
            {   // There is no smoothing
                return sourceData;
            }

            if (period > Bars || period + shift <= 0 || period + shift > Bars)
            {   // Error in the parameters
                return null;
            }

            for (bar = 0; bar < period + shift - 1; bar++)
            {
                movingAverage[bar] = 0;
            }

            for (bar = 0, sum = 0; bar < period; bar++)
            {
                sum += sourceData[bar];
            }

            movingAverage[period + shift - 1] = sum / period;

            // Simple Moving Average
            if (maMethod == MAMethod.Simple)
            {   
                for (bar = period; bar < Math.Min(Bars, Bars - shift); bar++)
                {
                    movingAverage[bar + shift] = movingAverage[bar + shift - 1] + sourceData[bar] / period - sourceData[bar - period] / period;
                }
            }

            // Exponential Moving Average
            else if (maMethod == MAMethod.Exponential)
            {   
                double pr = 2d / (period + 1);

                for (bar = period; bar < Math.Min(Bars, Bars - shift); bar++)
                {
                    movingAverage[bar + shift] = sourceData[bar] * pr + movingAverage[bar + shift - 1] * (1 - pr);
                }
            }

            // Weighted Moving Average
            else if (maMethod == MAMethod.Weighted)
            {
                double weight = period * (period + 1) / 2d;

                for (bar = period; bar < Math.Min(Bars, Bars - shift); bar++)
                {
                    sum = 0;
                    for (int i = 0; i < period; i++)
                    {
                        sum += sourceData[bar - i] * (period - i);
                    }

                    movingAverage[bar + shift] = sum / weight;
                }
            }

            // Smoothed Moving Average
            else if (maMethod == MAMethod.Smoothed)
            {
                for (bar = period; bar < Math.Min(Bars, Bars - shift); bar++)
                {
                    movingAverage[bar + shift] = (movingAverage[bar + shift - 1] * (period - 1) + sourceData[bar]) / period;
                }
            }

            for (bar = Bars + shift; bar < Bars; bar++)
            {
                movingAverage[bar] = 0;
            }

            return movingAverage;
        }

        /// <summary>
        /// Maximum error for comparing indicator values
        /// </summary>
        protected double Sigma()
        {
            int sigmaMode = isSeparatedChart ?
                Configs.SIGMA_MODE_SEPARATED_CHART : // Indicators ploted on its own chart (MACD, RSI, ADX, Momentum, ...)
                Configs.SIGMA_MODE_MAIN_CHART;       // Indicators ploted on the main chart (MA, Bollinger Bands, Alligator, ...)

            double dSigma;

            switch (sigmaMode)
            {
                case 0:
                    dSigma = 0;
                    break;
                case 1:
                    dSigma = Data.InstrProperties.Point * 0.5;
                    break;
                case 2:
                    dSigma = Data.InstrProperties.Point * 0.05;
                    break;
                case 3:
                    dSigma = Data.InstrProperties.Point * 0.005;
                    break;
                case 4:
                    dSigma = 0.00005;
                    break;
                case 5:
                    dSigma = 0.000005;
                    break;
                case 6:
                    dSigma = 0.0000005;
                    break;
                case 7:
                    dSigma = 0.00000005;
                    break;
                case 8:
                    dSigma = 0.000000005;
                    break;
                default:
                    dSigma = 0;
                    break;
            }

            return dSigma;
        }

        /// <summary>
        /// Calculates the logic of an Oscillator.
        /// </summary>
        /// <param name="firstBar">The first bar number.</param>
        /// <param name="previous">To use the previous bar or not.</param>
        /// <param name="adIndValue">The indicator values.</param>
        /// <param name="levelLong">The Level value for a Long position.</param>
        /// <param name="levelShort">The Level value for a Short position.</param>
        /// <param name="indCompLong">Indicator component for Long position.</param>
        /// <param name="indCompShort">Indicator component for Short position.</param>
        /// <param name="indLogic">The chosen logic.</param>
        /// <returns>True if everyting is ok.</returns>
        protected bool OscillatorLogic(int firstBar, int previous, double[] indValue, double levelLong,
            double levelShort, ref IndicatorComp indCompLong, ref IndicatorComp indCompShort, IndicatorLogic indLogic)
        {
            double sigma = Sigma();

            switch (indLogic)
            {
                case IndicatorLogic.The_indicator_rises:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        int  currBar  = bar - previous;
                        int  baseBar  = currBar - 1;
                        bool isHigher = indValue[currBar] > indValue[baseBar];

                        if (!isDescreteValues)  // Aroon oscillator uses isDescreteValues = true
                        {
                            bool isNoChange = true;
                            while (Math.Abs(indValue[currBar] - indValue[baseBar]) < sigma && isNoChange && baseBar > firstBar)
                            {
                                isNoChange = (isHigher == (indValue[baseBar + 1] > indValue[baseBar]));
                                baseBar--;
                            }
                        }

                        indCompLong.Value[bar]  = indValue[baseBar] < indValue[currBar] - sigma ? 1 : 0;
                        indCompShort.Value[bar] = indValue[baseBar] > indValue[currBar] + sigma ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_falls:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        int  currBar  = bar - previous;
                        int  baseBar  = currBar - 1;
                        bool isHigher = indValue[currBar] > indValue[baseBar];

                        if (!isDescreteValues)
                        {
                            bool isNoChange = true;
                            while (Math.Abs(indValue[currBar] - indValue[baseBar]) < sigma && isNoChange && baseBar > firstBar)
                            {
                                isNoChange = (isHigher == (indValue[baseBar + 1] > indValue[baseBar]));
                                baseBar--;
                            }
                        }

                        indCompLong.Value[bar]  = indValue[baseBar] > indValue[currBar] + sigma ? 1 : 0;
                        indCompShort.Value[bar] = indValue[baseBar] < indValue[currBar] - sigma ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_is_higher_than_the_level_line:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        indCompLong.Value[bar]  = indValue[bar - previous] > levelLong  + sigma ? 1 : 0;
                        indCompShort.Value[bar] = indValue[bar - previous] < levelShort - sigma ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_is_lower_than_the_level_line:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        indCompLong.Value[bar]  = indValue[bar - previous] < levelLong  - sigma ? 1 : 0;
                        indCompShort.Value[bar] = indValue[bar - previous] > levelShort + sigma ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_crosses_the_level_line_upward:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        int baseBar = bar - previous - 1;
                        while (Math.Abs(indValue[baseBar] - levelLong) < sigma && baseBar > firstBar)
                        { baseBar--; }

                        indCompLong.Value[bar]  = (indValue[baseBar] < levelLong  - sigma && indValue[bar - previous] > levelLong  + sigma) ? 1 : 0;
                        indCompShort.Value[bar] = (indValue[baseBar] > levelShort + sigma && indValue[bar - previous] < levelShort - sigma) ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_crosses_the_level_line_downward:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        int baseBar = bar - previous - 1;
                        while (Math.Abs(indValue[baseBar] - levelLong) < sigma && baseBar > firstBar)
                        { baseBar--; }

                        indCompLong.Value[bar]  = (indValue[baseBar] > levelLong  + sigma && indValue[bar - previous] < levelLong  - sigma) ? 1 : 0;
                        indCompShort.Value[bar] = (indValue[baseBar] < levelShort - sigma && indValue[bar - previous] > levelShort + sigma) ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_changes_its_direction_upward:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        int bar0 = bar  - previous;
                        int bar1 = bar0 - 1;
                        while (Math.Abs(indValue[bar0] - indValue[bar1]) < sigma && bar1 > firstBar)
                        { bar1--; }

                        int bar2 = bar1 - 1 > firstBar ? bar1 - 1 : firstBar;
                        while (Math.Abs(indValue[bar1] - indValue[bar2]) < sigma && bar2 > firstBar)
                        { bar2--; }

                        indCompLong.Value[bar]  = (indValue[bar2] > indValue[bar1] && indValue[bar1] < indValue[bar0] && bar1 == bar0 - 1) ? 1 : 0;
                        indCompShort.Value[bar] = (indValue[bar2] < indValue[bar1] && indValue[bar1] > indValue[bar0] && bar1 == bar0 - 1) ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_changes_its_direction_downward:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        int bar0 = bar  - previous;
                        int bar1 = bar0 - 1;
                        while (Math.Abs(indValue[bar0] - indValue[bar1]) < sigma && bar1 > firstBar)
                        { bar1--; }

                        int bar2 = bar1 - 1 > firstBar ? bar1 - 1 : firstBar;
                        while (Math.Abs(indValue[bar1] - indValue[bar2]) < sigma && bar2 > firstBar)
                        { bar2--; }

                        indCompLong.Value[bar]  = (indValue[bar2] < indValue[bar1] && indValue[bar1] > indValue[bar0] && bar1 == bar0 - 1) ? 1 : 0;
                        indCompShort.Value[bar] = (indValue[bar2] > indValue[bar1] && indValue[bar1] < indValue[bar0] && bar1 == bar0 - 1) ? 1 : 0;
                    }
                    break;

                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Calculates the logic of a No Direction Oscillator.
        /// </summary>
        /// <param name="firstBar">The first bar number.</param>
        /// <param name="previous">To use the previous bar or not.</param>
        /// <param name="adIndValue">The indicator values.</param>
        /// <param name="level">The Level value.</param>
        /// <param name="indComp">Indicator component where to save the results.</param>
        /// <param name="indLogic">The chosen logic.</param>
        /// <returns>True if everyting is ok.</returns>
        protected bool NoDirectionOscillatorLogic(int firstBar, int previous, double[] adIndValue, double level, ref IndicatorComp indComp, IndicatorLogic indLogic)
        {
            double sigma = Sigma();

            switch (indLogic)
            {
                case IndicatorLogic.The_indicator_rises:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        int  currentBar = bar - previous;
                        int  baseBar    = currentBar - 1;
                        bool isHigher   = adIndValue[currentBar] > adIndValue[baseBar];
                        bool isNoChange = true;

                        while (Math.Abs(adIndValue[currentBar] - adIndValue[baseBar]) < sigma && isNoChange && baseBar > firstBar)
                        {
                            isNoChange = (isHigher == (adIndValue[baseBar + 1] > adIndValue[baseBar]));
                            baseBar--;
                        }
                        
                        indComp.Value[bar] = adIndValue[baseBar] < adIndValue[currentBar] - sigma ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_falls:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        int  currentBar = bar - previous;
                        int  baseBar    = currentBar - 1;
                        bool isHigher   = adIndValue[currentBar] > adIndValue[baseBar];
                        bool isNoChange = true;

                        while (Math.Abs(adIndValue[currentBar] - adIndValue[baseBar]) < sigma && isNoChange && baseBar > firstBar)
                        {
                            isNoChange = (isHigher == (adIndValue[baseBar + 1] > adIndValue[baseBar]));
                            baseBar--;
                        }

                        indComp.Value[bar] = adIndValue[baseBar] > adIndValue[currentBar] + sigma ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_is_higher_than_the_level_line:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        indComp.Value[bar] = adIndValue[bar - previous] > level + sigma ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_is_lower_than_the_level_line:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        indComp.Value[bar] = adIndValue[bar - previous] < level - sigma ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_crosses_the_level_line_upward:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        int baseBar = bar - previous - 1;
                        while (Math.Abs(adIndValue[baseBar] - level) < sigma && baseBar > firstBar)
                        { baseBar--; }

                        indComp.Value[bar] = (adIndValue[baseBar] < level - sigma && adIndValue[bar - previous] > level + sigma) ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_crosses_the_level_line_downward:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        int baseBar = bar - previous - 1;
                        while (Math.Abs(adIndValue[baseBar] - level) < sigma && baseBar > firstBar)
                        { baseBar--; }

                        indComp.Value[bar] = (adIndValue[baseBar] > level + sigma && adIndValue[bar - previous] < level - sigma) ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_changes_its_direction_upward:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        int bar0 = bar - previous;
                        int bar1 = bar0 - 1;
                        while (Math.Abs(adIndValue[bar0] - adIndValue[bar1]) < sigma && bar1 > firstBar)
                        { bar1--; }

                        int bar2 = bar1 - 1 > firstBar ? bar1 - 1 : firstBar;
                        while (Math.Abs(adIndValue[bar1] - adIndValue[bar2]) < sigma && bar2 > firstBar)
                        { bar2--; }

                        indComp.Value[bar] = (adIndValue[bar2] > adIndValue[bar1] && adIndValue[bar1] < adIndValue[bar0] && bar1 == bar0 - 1) ? 1 : 0;
                    }
                    break;

                case IndicatorLogic.The_indicator_changes_its_direction_downward:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        int bar0 = bar  - previous;
                        int bar1 = bar0 - 1;
                        while (Math.Abs(adIndValue[bar0] - adIndValue[bar1]) < sigma && bar1 > firstBar)
                        { bar1--; }

                        int bar2 = bar1 - 1 > firstBar ? bar1 - 1 : firstBar;
                        while (Math.Abs(adIndValue[bar1] - adIndValue[bar2]) < sigma && bar2 > firstBar)
                        { bar2--; }

                        indComp.Value[bar] = (adIndValue[bar2] < adIndValue[bar1] && adIndValue[bar1] > adIndValue[bar0] && bar1 == bar0 - 1) ? 1 : 0;
                    }
                    break;

                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Calculates the logic of a band indicator.
        /// </summary>
        /// <param name="firstBar">The first bar number.</param>
        /// <param name="previous">To use the previous bar or not.</param>
        /// <param name="adUpperBand">The Upper band values.</param>
        /// <param name="adLowerBand">The Lower band values.</param>
        /// <param name="indCompLong">Indicator component for Long position.</param>
        /// <param name="indCompShort">Indicator component for Short position.</param>
        /// <param name="indLogic">The chosen logic.</param>
        /// <returns>True if everyting is ok.</returns>
        protected bool BandIndicatorLogic(int firstBar, int previous, double[] adUpperBand, double[] adLowerBand,
            ref IndicatorComp indCompLong, ref IndicatorComp indCompShort, BandIndLogic indLogic)
        {
            double sigma = Sigma();

            switch (indLogic)
            {
                case BandIndLogic.The_bar_opens_below_the_Upper_Band:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        indCompLong.Value[bar]  = Open[bar] < adUpperBand[bar - previous] - sigma ? 1 : 0;
                        indCompShort.Value[bar] = Open[bar] > adLowerBand[bar - previous] + sigma ? 1 : 0;
                    }
                    break;

                case BandIndLogic.The_bar_opens_above_the_Upper_Band:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        indCompLong.Value[bar]  = Open[bar] > adUpperBand[bar - previous] + sigma ? 1 : 0;
                        indCompShort.Value[bar] = Open[bar] < adLowerBand[bar - previous] - sigma ? 1 : 0;
                    }
                    break;

                case BandIndLogic.The_bar_opens_below_the_Lower_Band:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        indCompLong.Value[bar]  = Open[bar] < adLowerBand[bar - previous] - sigma ? 1 : 0;
                        indCompShort.Value[bar] = Open[bar] > adUpperBand[bar - previous] + sigma ? 1 : 0;
                    }
                    break;

                case BandIndLogic.The_bar_opens_above_the_Lower_Band:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        indCompLong.Value[bar]  = Open[bar] > adLowerBand[bar - previous] + sigma ? 1 : 0;
                        indCompShort.Value[bar] = Open[bar] < adUpperBand[bar - previous] - sigma ? 1 : 0;
                    }
                    break;

                case BandIndLogic.The_bar_opens_below_the_Upper_Band_after_opening_above_it:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        int baseBar = bar - 1;
                        while (Math.Abs(Open[baseBar] - adUpperBand[baseBar - previous]) < sigma && baseBar > firstBar)
                        { baseBar--; }

                        indCompLong.Value[bar]  = Open[bar] < adUpperBand[bar - previous] - sigma && Open[baseBar] > adUpperBand[baseBar - previous] + sigma ? 1 : 0;

                        baseBar = bar - 1;
                        while (Math.Abs(Open[baseBar] - adLowerBand[baseBar - previous]) < sigma && baseBar > firstBar)
                        { baseBar--; }

                        indCompShort.Value[bar] = Open[bar] > adLowerBand[bar - previous] + sigma && Open[baseBar] < adLowerBand[baseBar - previous] - sigma ? 1 : 0;
                    }
                    break;

                case BandIndLogic.The_bar_opens_above_the_Upper_Band_after_opening_below_it:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        int baseBar = bar - 1;
                        while (Math.Abs(Open[baseBar] - adUpperBand[baseBar - previous]) < sigma && baseBar > firstBar)
                        { baseBar--; }

                        indCompLong.Value[bar]  = Open[bar] > adUpperBand[bar - previous] + sigma && Open[baseBar] < adUpperBand[baseBar - previous] - sigma ? 1 : 0;

                        baseBar = bar - 1;
                        while (Math.Abs(Open[baseBar] - adLowerBand[baseBar - previous]) < sigma && baseBar > firstBar)
                        { baseBar--; }

                        indCompShort.Value[bar] = Open[bar] < adLowerBand[bar - previous] - sigma && Open[baseBar] > adLowerBand[baseBar - previous] + sigma ? 1 : 0;
                    }
                    break;

                case BandIndLogic.The_bar_opens_below_the_Lower_Band_after_opening_above_it:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        int baseBar = bar - 1;
                        while (Math.Abs(Open[baseBar] - adLowerBand[baseBar - previous]) < sigma && baseBar > firstBar)
                        { baseBar--; }

                        indCompLong.Value[bar]  = Open[bar] < adLowerBand[bar - previous] - sigma && Open[baseBar] > adLowerBand[baseBar - previous] + sigma ? 1 : 0;

                        baseBar = bar - 1;
                        while (Math.Abs(Open[baseBar] - adUpperBand[baseBar - previous]) < sigma && baseBar > firstBar)
                        { baseBar--; }

                        indCompShort.Value[bar] = Open[bar] > adUpperBand[bar - previous] + sigma && Open[baseBar] < adUpperBand[baseBar - previous] - sigma ? 1 : 0;
                    }
                    break;

                case BandIndLogic.The_bar_opens_above_the_Lower_Band_after_opening_below_it:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        int baseBar = bar - 1;
                        while (Math.Abs(Open[baseBar] - adLowerBand[baseBar - previous]) < sigma && baseBar > firstBar)
                        { baseBar--; }

                        indCompLong.Value[bar]  = Open[bar] > adLowerBand[bar - previous] + sigma && Open[baseBar] < adLowerBand[baseBar - previous] - sigma ? 1 : 0;

                        baseBar = bar - 1;
                        while (Math.Abs(Open[baseBar] - adUpperBand[baseBar - previous]) < sigma && baseBar > firstBar)
                        { baseBar--; }

                        indCompShort.Value[bar] = Open[bar] < adUpperBand[bar - previous] - sigma && Open[baseBar] > adUpperBand[baseBar - previous] + sigma ? 1 : 0;
                    }
                    break;

                case BandIndLogic.The_bar_closes_below_the_Upper_Band:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        indCompLong.Value[bar]  = Close[bar] < adUpperBand[bar - previous] - sigma ? 1 : 0;
                        indCompShort.Value[bar] = Close[bar] > adLowerBand[bar - previous] + sigma ? 1 : 0;
                    }
                    break;

                case BandIndLogic.The_bar_closes_above_the_Upper_Band:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        indCompLong.Value[bar]  = Close[bar] > adUpperBand[bar - previous] + sigma ? 1 : 0;
                        indCompShort.Value[bar] = Close[bar] < adLowerBand[bar - previous] - sigma ? 1 : 0;
                    }
                    break;

                case BandIndLogic.The_bar_closes_below_the_Lower_Band:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        indCompLong.Value[bar]  = Close[bar] < adLowerBand[bar - previous] - sigma ? 1 : 0;
                        indCompShort.Value[bar] = Close[bar] > adUpperBand[bar - previous] + sigma ? 1 : 0;
                    }
                    break;

                case BandIndLogic.The_bar_closes_above_the_Lower_Band:
                    for (int bar = firstBar; bar < Bars; bar++)
                    {
                        indCompLong.Value[bar]  = Close[bar] > adLowerBand[bar - previous] + sigma ? 1 : 0;
                        indCompShort.Value[bar] = Close[bar] < adUpperBand[bar - previous] - sigma ? 1 : 0;
                    }
                    break;

                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns signals for the logic rule "Indicator rises".
        /// </summary>
        protected void IndicatorRisesLogic(int firstBar, int previous, double[] adIndValue, ref IndicatorComp indCompLong, ref IndicatorComp indCompShort)
        {
            double sigma = Sigma();

            for (int bar = firstBar; bar < Bars; bar++)
            {
                int  currentBar = bar - previous;
                int  baseBar    = currentBar - 1;
                bool isNoChange = true;
                bool isHigher   = adIndValue[currentBar] > adIndValue[baseBar];

                while (Math.Abs(adIndValue[currentBar] - adIndValue[baseBar]) < sigma && isNoChange && baseBar > firstBar)
                {
                    isNoChange = (isHigher == (adIndValue[baseBar + 1] > adIndValue[baseBar]));
                    baseBar--;
                }

                indCompLong.Value[bar]  = adIndValue[currentBar] > adIndValue[baseBar] + sigma ? 1 : 0;
                indCompShort.Value[bar] = adIndValue[currentBar] < adIndValue[baseBar] - sigma ? 1 : 0;
            }

            return;
        }

        /// <summary>
        /// Returns signals for the logic rule "Indicator falls"
        /// </summary>
        protected void IndicatorFallsLogic(int firstBar, int previous, double[] adIndValue, ref IndicatorComp indCompLong, ref IndicatorComp indCompShort)
        {
            double sigma = Sigma();

            for (int bar = firstBar; bar < Bars; bar++)
            {
                int  currentBar = bar - previous;
                int  baseBar    = currentBar - 1;
                bool isNoChange = true;
                bool isLower    = adIndValue[currentBar] < adIndValue[baseBar];

                while (Math.Abs(adIndValue[currentBar] - adIndValue[baseBar]) < sigma && isNoChange && baseBar > firstBar)
                {
                    isNoChange = (isLower == (adIndValue[baseBar + 1] < adIndValue[baseBar]));
                    baseBar--;
                }

                indCompLong.Value[bar]  = adIndValue[currentBar] < adIndValue[baseBar] - sigma ? 1 : 0;
                indCompShort.Value[bar] = adIndValue[currentBar] > adIndValue[baseBar] + sigma ? 1 : 0;
            }

            return;
        }

        /// <summary>
        /// Returns signals for the logic rule "The Indicator is higher than the AnotherIndicator"
        /// </summary>
        protected void IndicatorIsHigherThanAnotherIndicatorLogic(int firstBar, int previous, double[] adIndValue, double[] adAnotherIndValue,
            ref IndicatorComp indCompLong, ref IndicatorComp indCompShort)
        {
            double sigma = Sigma();

            for (int bar = firstBar; bar < Bars; bar++)
            {
                int currentBar = bar - previous;
                indCompLong.Value[bar]  = adIndValue[currentBar] > adAnotherIndValue[currentBar] + sigma ? 1 : 0;
                indCompShort.Value[bar] = adIndValue[currentBar] < adAnotherIndValue[currentBar] - sigma ? 1 : 0;
            }

            return;
        }

        /// <summary>
        /// Returns signals for the logic rule "The Indicator is lower than the AnotherIndicator"
        /// </summary>
        protected void IndicatorIsLowerThanAnotherIndicatorLogic(int firstBar, int previous, double[] adIndValue, double[] adAnotherIndValue,
            ref IndicatorComp indCompLong, ref IndicatorComp indCompShort)
        {
            double sigma = Sigma();

            for (int bar = firstBar; bar < Bars; bar++)
            {
                int currentBar = bar - previous;
                indCompLong.Value[bar]  = adIndValue[currentBar] < adAnotherIndValue[currentBar] - sigma ? 1 : 0;
                indCompShort.Value[bar] = adIndValue[currentBar] > adAnotherIndValue[currentBar] + sigma ? 1 : 0;
            }

            return;
        }

        /// <summary>
        /// Returns signals for the logic rule "The Indicator crosses AnotherIndicator upward"
        /// </summary>
        protected void IndicatorCrossesAnotherIndicatorUpwardLogic(int firstBar, int previous, double[] adIndValue, double[] adAnotherIndValue,
            ref IndicatorComp indCompLong, ref IndicatorComp indCompShort)
        {
            double sigma = Sigma();

            for (int bar = firstBar; bar < Bars; bar++)
            {
                int currentBar = bar - previous;
                int baseBar = currentBar - 1;
                while (Math.Abs(adIndValue[baseBar] - adAnotherIndValue[baseBar]) < sigma && baseBar > firstBar)
                { baseBar--; }

                indCompLong.Value[bar]  = adIndValue[currentBar] > adAnotherIndValue[currentBar] + sigma && adIndValue[baseBar] < adAnotherIndValue[baseBar] - sigma ? 1 : 0;
                indCompShort.Value[bar] = adIndValue[currentBar] < adAnotherIndValue[currentBar] - sigma && adIndValue[baseBar] > adAnotherIndValue[baseBar] + sigma ? 1 : 0;
            }

            return;
        }

        /// <summary>
        /// Returns signals for the logic rule "The Indicator crosses AnotherIndicator downward"
        /// </summary>
        protected void IndicatorCrossesAnotherIndicatorDownwardLogic(int firstBar, int previous, double[] adIndValue, double[] adAnotherIndValue,
            ref IndicatorComp indCompLong, ref IndicatorComp indCompShort)
        {
            double sigma = Sigma();

            for (int bar = firstBar; bar < Bars; bar++)
            {
                int currentBar = bar - previous;
                int baseBar = currentBar - 1;
                while (Math.Abs(adIndValue[baseBar] - adAnotherIndValue[baseBar]) < sigma && baseBar > firstBar)
                { baseBar--; }

                indCompLong.Value[bar]  = adIndValue[currentBar] < adAnotherIndValue[currentBar] - sigma && adIndValue[baseBar] > adAnotherIndValue[baseBar] + sigma ? 1 : 0;
                indCompShort.Value[bar] = adIndValue[currentBar] > adAnotherIndValue[currentBar] + sigma && adIndValue[baseBar] < adAnotherIndValue[baseBar] - sigma ? 1 : 0;
            }

            return;
        }

        /// <summary>
        /// Returns signals for the logic rule "The bar opens above the Indicator"
        /// </summary>
        protected void BarOpensAboveIndicatorLogic(int firstBar, int previous, double[] adIndValue, ref IndicatorComp indCompLong, ref IndicatorComp indCompShort)
        {
            double sigma = Sigma();

            for (int bar = firstBar; bar < Bars; bar++)
            {
                indCompLong.Value[bar]  = Open[bar] > adIndValue[bar - previous] + sigma ? 1 : 0;
                indCompShort.Value[bar] = Open[bar] < adIndValue[bar - previous] - sigma ? 1 : 0;
            }

            return;
        }

        /// <summary>
        /// Returns signals for the logic rule "The bar opens below the Indicator"
        /// </summary>
        protected void BarOpensBelowIndicatorLogic(int firstBar, int previous, double[] adIndValue, ref IndicatorComp indCompLong, ref IndicatorComp indCompShort)
        {
            double sigma = Sigma();

            for (int bar = firstBar; bar < Bars; bar++)
            {
                indCompLong.Value[bar]  = Open[bar] < adIndValue[bar - previous] - sigma ? 1 : 0;
                indCompShort.Value[bar] = Open[bar] > adIndValue[bar - previous] + sigma ? 1 : 0;
            }

            return;
        }

        /// <summary>
        /// Returns signals for the logic rule "The bar opens above the Indicator after opening below it"
        /// </summary>
        protected void BarOpensAboveIndicatorAfterOpeningBelowLogic(int firstBar, int previous, double[] adIndValue, ref IndicatorComp indCompLong, ref IndicatorComp indCompShort)
        {
            double sigma = Sigma();

            for (int bar = firstBar; bar < Bars; bar++)
            {
                int baseBar = bar - 1;
                while (Math.Abs(Open[baseBar] - adIndValue[baseBar - previous]) < sigma && baseBar > firstBar)
                { baseBar--; }

                indCompLong.Value[bar]  = Open[bar] > adIndValue[bar - previous] + sigma && Open[baseBar] < adIndValue[baseBar - previous] - sigma ? 1 : 0;
                indCompShort.Value[bar] = Open[bar] < adIndValue[bar - previous] - sigma && Open[baseBar] > adIndValue[baseBar - previous] + sigma ? 1 : 0;
            }

            return;
        }

        /// <summary>
        /// Returns signals for the logic rule "The bar opens below the Indicator after opening above it"
        /// </summary>
        protected void BarOpensBelowIndicatorAfterOpeningAboveLogic(int firstBar, int previous, double[] adIndValue, ref IndicatorComp indCompLong, ref IndicatorComp indCompShort)
        {
            double sigma = Sigma();

            for (int bar = firstBar; bar < Bars; bar++)
            {
                int baseBar = bar - 1;
                while (Math.Abs(Open[baseBar] - adIndValue[baseBar - previous]) < sigma && baseBar > firstBar)
                { baseBar--; }

                indCompLong.Value[bar]  = Open[bar] < adIndValue[bar - previous] - sigma && Open[baseBar] > adIndValue[baseBar - previous] + sigma ? 1 : 0;
                indCompShort.Value[bar] = Open[bar] > adIndValue[bar - previous] + sigma && Open[baseBar] < adIndValue[baseBar - previous] - sigma ? 1 : 0;
            }

            return;
        }

        /// <summary>
        /// Returns signals for the logic rule "The bar closes above the Indicator"
        /// </summary>
        protected void BarClosesAboveIndicatorLogic(int firstBar, int previous, double[] adIndValue, ref IndicatorComp indCompLong, ref IndicatorComp indCompShort)
        {
            double sigma = Sigma();

            for (int bar = firstBar; bar < Bars; bar++)
            {
                indCompLong.Value[bar]  = Close[bar] > adIndValue[bar - previous] + sigma ? 1 : 0;
                indCompShort.Value[bar] = Close[bar] < adIndValue[bar - previous] - sigma ? 1 : 0;
            }

            return;
        }

        /// <summary>
        /// Returns signals for the logic rule "The bar closes below the Indicator"
        /// </summary>
        protected void BarClosesBelowIndicatorLogic(int firstBar, int previous, double[] adIndValue, ref IndicatorComp indCompLong, ref IndicatorComp indCompShort)
        {
            double sigma = Sigma();

            for (int bar = firstBar; bar < Bars; bar++)
            {
                indCompLong.Value[bar]  = Close[bar] < adIndValue[bar - previous] - sigma ? 1 : 0;
                indCompShort.Value[bar] = Close[bar] > adIndValue[bar - previous] + sigma ? 1 : 0;
            }

            return;
        }
    }
}
