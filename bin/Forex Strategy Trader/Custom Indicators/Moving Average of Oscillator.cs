// Moving Average of Oscillator Indicator
// Last changed on 2009-05-15
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// This code or any part of it cannot be used in other applications without a permission.
// Copyright (c) 2006 - 2009 Miroslav Popov - All rights reserved.

using System;
using System.Drawing;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// MACD Histogram Indicator
    /// </summary>
    public class Moving_Average_of_Oscillator : Indicator
    {
        /// <summary>
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Moving_Average_of_Oscillator(SlotTypes slotType)
        {
            // General properties
            IndicatorName   = "Moving Average of Oscillator";
            PossibleSlots   = SlotTypes.OpenFilter | SlotTypes.CloseFilter;
            SeparatedChart  = true;
            CustomIndicator = true;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption = "Logic";
            IndParam.ListParam[0].ItemList = new string[]
            {
                "The OsMA rises",
                "The OsMA falls",
                "The OsMA is higher than the Level line",
                "The OsMA is lower than the Level line",
                "The OsMA crosses the Level line upward",
                "The OsMA crosses the Level line downward",
                "The OsMA changes its direction upward",
                "The OsMA changes its direction downward"
            };
            IndParam.ListParam[0].Index    = 0;
            IndParam.ListParam[0].Text     = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled  = true;
            IndParam.ListParam[0].ToolTip  = "Logic of application of the indicator.";

            IndParam.ListParam[1].Caption  = "Smoothing method";
            IndParam.ListParam[1].ItemList = Enum.GetNames(typeof(MAMethod));
            IndParam.ListParam[1].Index    = (int)MAMethod.Exponential;
            IndParam.ListParam[1].Text     = IndParam.ListParam[1].ItemList[IndParam.ListParam[1].Index];
            IndParam.ListParam[1].Enabled  = true;
            IndParam.ListParam[1].ToolTip  = "The smoothing method of Moving Averages.";

            IndParam.ListParam[2].Caption  = "Base price";
            IndParam.ListParam[2].ItemList = Enum.GetNames(typeof(BasePrice));
            IndParam.ListParam[2].Index    = (int)BasePrice.Close;
            IndParam.ListParam[2].Text     = IndParam.ListParam[2].ItemList[IndParam.ListParam[2].Index];
            IndParam.ListParam[2].Enabled  = true;
            IndParam.ListParam[2].ToolTip  = "The price the Moving Averages are based on.";

            IndParam.ListParam[3].Caption  = "Signal line method";
            IndParam.ListParam[3].ItemList = Enum.GetNames(typeof(MAMethod));
            IndParam.ListParam[3].Index    = (int)MAMethod.Simple;
            IndParam.ListParam[3].Text     = IndParam.ListParam[3].ItemList[IndParam.ListParam[3].Index];
            IndParam.ListParam[3].Enabled  = true;
            IndParam.ListParam[3].ToolTip  = "The smoothing method of the signal line.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "Slow MA period";
            IndParam.NumParam[0].Value   = 26;
            IndParam.NumParam[0].Min     = 1;
            IndParam.NumParam[0].Max     = 200;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "The period of Slow MA.";

            IndParam.NumParam[1].Caption = "Fast MA period";
            IndParam.NumParam[1].Value   = 12;
            IndParam.NumParam[1].Min     = 1;
            IndParam.NumParam[1].Max     = 200;
            IndParam.NumParam[1].Enabled = true;
            IndParam.NumParam[1].ToolTip = "The period of Fast MA.";

            IndParam.NumParam[2].Caption = "Signal line period";
            IndParam.NumParam[2].Value   = 9;
            IndParam.NumParam[2].Min     = 1;
            IndParam.NumParam[2].Max     = 200;
            IndParam.NumParam[2].Enabled = true;
            IndParam.NumParam[2].ToolTip = "The period of Signal line.";

            IndParam.NumParam[3].Caption = "Level";
            IndParam.NumParam[3].Value   = 0;
            IndParam.NumParam[3].Min     = 0;
            IndParam.NumParam[3].Max     = 5;
            IndParam.NumParam[3].Point   = 4;
            IndParam.NumParam[3].Enabled = true;
            IndParam.NumParam[3].ToolTip = "A critical level (for the appropriate logic).";

            // The CheckBox parameters
            IndParam.CheckParam[0].Caption = "Use previous bar value";
            IndParam.CheckParam[0].Checked = PrepareUsePrevBarValueCheckBox(slotType);
            IndParam.CheckParam[0].Enabled = true;
            IndParam.CheckParam[0].ToolTip = "Use the indicator value from the previous bar.";

            return;
        }

        /// <summary>
        /// Calculates the indicator's components
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            // Reading the parameters
            MAMethod  maMethod  = (MAMethod )IndParam.ListParam[1].Index;
            MAMethod  slMethod  = (MAMethod )IndParam.ListParam[3].Index;
            BasePrice basePrice = (BasePrice)IndParam.ListParam[2].Index;
            int       nSlow     = (int)IndParam.NumParam[0].Value;
            int       nFast     = (int)IndParam.NumParam[1].Value;
            int       nSignal   = (int)IndParam.NumParam[2].Value;
            double    dLevel    = IndParam.NumParam[3].Value;
            int       iPrvs     = IndParam.CheckParam[0].Checked ? 1 : 0;

            // Calculation
            int iFirstBar = nSlow + nFast + 2;

            double[] adMASlow = MovingAverage(nSlow, 0, maMethod, Price(basePrice));
            double[] adMAFast = MovingAverage(nFast, 0, maMethod, Price(basePrice));

            double[] adMACD = new double[Bars];

            for (int iBar = nSlow - 1; iBar < Bars; iBar++)
                adMACD[iBar] = adMAFast[iBar] - adMASlow[iBar];

            double[] maSignalLine = MovingAverage(nSignal, 0, slMethod, adMACD);

            // adHistogram reprezents the MACD oscillator
            double[] adHistogram = new double[Bars];
            for (int iBar = nSlow + nSignal - 1; iBar < Bars; iBar++)
                adHistogram[iBar] = adMACD[iBar] - maSignalLine[iBar];

            // Saving the components
            Component = new IndicatorComp[3];

            Component[0] = new IndicatorComp();
            Component[0].CompName   = "OsMA";
            Component[0].DataType   = IndComponentType.IndicatorValue;
            Component[0].ChartType  = IndChartType.Histogram;
            Component[0].FirstBar   = iFirstBar;
            Component[0].Value      = adHistogram;

            Component[1] = new IndicatorComp();
            Component[1].ChartType = IndChartType.NoChart;
            Component[1].FirstBar  = iFirstBar;
            Component[1].Value     = new double[Bars];

            Component[2] = new IndicatorComp();
            Component[2].ChartType = IndChartType.NoChart;
            Component[2].FirstBar  = iFirstBar;
            Component[2].Value     = new double[Bars];

            // Sets the Component's type
            if (slotType == SlotTypes.OpenFilter)
            {
                Component[1].DataType = IndComponentType.AllowOpenLong;
                Component[1].CompName = "Is long entry allowed";
                Component[2].DataType = IndComponentType.AllowOpenShort;
                Component[2].CompName = "Is short entry allowed";
            }
            else if (slotType == SlotTypes.CloseFilter)
            {
                Component[1].DataType = IndComponentType.ForceCloseLong;
                Component[1].CompName = "Close out long position";
                Component[2].DataType = IndComponentType.ForceCloseShort;
                Component[2].CompName = "Close out short position";
            }

            // Calculation of the logic
            IndicatorLogic indLogic = IndicatorLogic.It_does_not_act_as_a_filter;

            switch (IndParam.ListParam[0].Text)
            {
                case "The OsMA rises":
                    indLogic = IndicatorLogic.The_indicator_rises;
                    break;

                case "The OsMA falls":
                    indLogic = IndicatorLogic.The_indicator_falls;
                    break;

                case "The OsMA is higher than the Level line":
                    indLogic = IndicatorLogic.The_indicator_is_higher_than_the_level_line;
                    SpecialValues = new double[2] { dLevel, - dLevel };
                    break;

                case "The OsMA is lower than the Level line":
                    indLogic = IndicatorLogic.The_indicator_is_lower_than_the_level_line;
                    SpecialValues = new double[2] { dLevel, - dLevel };
                    break;

                case "The OsMA crosses the Level line upward":
                    indLogic = IndicatorLogic.The_indicator_crosses_the_level_line_upward;
                    SpecialValues = new double[2] { dLevel, - dLevel };
                    break;

                case "The OsMA crosses the Level line downward":
                    indLogic = IndicatorLogic.The_indicator_crosses_the_level_line_downward;
                    SpecialValues = new double[2] { dLevel, - dLevel };
                    break;

                case "The OsMA changes its direction upward":
                    indLogic = IndicatorLogic.The_indicator_changes_its_direction_upward;
                    break;

                case "The OsMA changes its direction downward":
                    indLogic = IndicatorLogic.The_indicator_changes_its_direction_downward;
                    break;

                default:
                    break;
            }

            OscillatorLogic(iFirstBar, iPrvs, adHistogram, dLevel, -dLevel, ref Component[1], ref Component[2], indLogic);

            return;
        }

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {
            string sLevelLong  = (IndParam.NumParam[3].Value == 0 ? "0" : IndParam.NumParam[3].ValueToString);
            string sLevelShort = (IndParam.NumParam[3].Value == 0 ? "0" : "-" + IndParam.NumParam[3].ValueToString);

            EntryFilterLongDescription  = "the " + ToString() + " ";
            EntryFilterShortDescription = "the " + ToString() + " ";
            ExitFilterLongDescription   = "the " + ToString() + " ";
            ExitFilterShortDescription  = "the " + ToString() + " ";

            switch (IndParam.ListParam[0].Text)
            {
                case "The OsMA rises":
                    EntryFilterLongDescription  += "rises";
                    EntryFilterShortDescription += "falls";
                    ExitFilterLongDescription   += "rises";
                    ExitFilterShortDescription  += "falls";
                    break;

                case "The OsMA falls":
                    EntryFilterLongDescription  += "falls";
                    EntryFilterShortDescription += "rises";
                    ExitFilterLongDescription   += "falls";
                    ExitFilterShortDescription  += "rises";
                    break;

                case "The OsMA is higher than the Level line":
                    EntryFilterLongDescription  += "is higher than the Level " + sLevelLong;
                    EntryFilterShortDescription += "is lower than the Level "  + sLevelShort;
                    ExitFilterLongDescription   += "is higher than the Level " + sLevelLong;
                    ExitFilterShortDescription  += "is lower than the Level "  + sLevelShort;
                    break;

                case "The OsMA is lower than the Level line":
                    EntryFilterLongDescription  += "is lower than the Level "  + sLevelLong;
                    EntryFilterShortDescription += "is higher than the Level " + sLevelShort;
                    ExitFilterLongDescription   += "is lower than the Level "  + sLevelLong;
                    ExitFilterShortDescription  += "is higher than the Level " + sLevelShort;
                    break;

                case "The OsMA crosses the Level line upward":
                    EntryFilterLongDescription  += "crosses the Level " + sLevelLong  + " upward";
                    EntryFilterShortDescription += "crosses the Level " + sLevelShort + " downward";
                    ExitFilterLongDescription   += "crosses the Level " + sLevelLong  + " upward";
                    ExitFilterShortDescription  += "crosses the Level " + sLevelShort + " downward";
                    break;

                case "The OsMA crosses the Level line downward":
                    EntryFilterLongDescription  += "crosses the Level " + sLevelLong  + " downward";
                    EntryFilterShortDescription += "crosses the Level " + sLevelShort + " upward";
                    ExitFilterLongDescription   += "crosses the Level " + sLevelLong  + " downward";
                    ExitFilterShortDescription  += "crosses the Level " + sLevelShort + " upward";
                    break;

                case "The OsMA changes its direction upward":
                    EntryFilterLongDescription  += "changes its direction upward";
                    EntryFilterShortDescription += "changes its direction downward";
                    ExitFilterLongDescription   += "changes its direction upward";
                    ExitFilterShortDescription  += "changes its direction downward";
                    break;

                case "The OsMA changes its direction downward":
                    EntryFilterLongDescription  += "changes its direction downward";
                    EntryFilterShortDescription += "changes its direction upward";
                    ExitFilterLongDescription   += "changes its direction downward";
                    ExitFilterShortDescription  += "changes its direction upward";
                    break;

                default:
                    break;
            }

            return;
        }

        /// <summary>
        /// Indicator to string
        /// </summary>
        public override string ToString()
        {
            string sString = IndicatorName +
                (IndParam.CheckParam[0].Checked ? "* (" : " (") +
                IndParam.ListParam[1].Text         + ", " + // Method
                IndParam.ListParam[2].Text         + ", " + // Price
                IndParam.ListParam[3].Text         + ", " + // Signal MA Method
                IndParam.NumParam[0].ValueToString + ", " + // Slow MA period
                IndParam.NumParam[1].ValueToString + ", " + // Fast MA period
                IndParam.NumParam[2].ValueToString + ")";   // Signal MA period

            return sString;
        }
    }
}
