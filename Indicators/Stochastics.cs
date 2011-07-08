// Stochastics Indicator
// Last changed on 2009-11-17
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2009 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Stochastics Indicator
    /// </summary>
    public class Stochastics : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Stochastics(SlotTypes slotType)
        {
            // General properties
            IndicatorName  = "Stochastics";
            PossibleSlots  = SlotTypes.OpenFilter | SlotTypes.CloseFilter;
            SeparatedChart = true;
            SeparatedChartMinValue = 0;
            SeparatedChartMaxValue = 100;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption  = "Logic";
            IndParam.ListParam[0].ItemList = new string[]
            {
                "The Slow %D rises",
                "The Slow %D falls",
                "The Slow %D is higher than the Level line",
                "The Slow %D is lower than the Level line",
                "The Slow %D crosses the Level line upward",
                "The Slow %D crosses the Level line downward",
                "The Slow %D changes its direction upward",
                "The Slow %D changes its direction downward",
                "The %K is higher than the Slow %D",
                "The %K is lower than the Slow %D",
                "The %K crosses the Slow %D upward",
                "The %K crosses the Slow %D downward",
            };
            IndParam.ListParam[0].Index    = 0;
            IndParam.ListParam[0].Text     = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled  = true;
            IndParam.ListParam[0].ToolTip  = "Logic of application of the indicator.";

            IndParam.ListParam[1].Caption  = "Smoothing method";
            IndParam.ListParam[1].ItemList = Enum.GetNames(typeof(MAMethod));
            IndParam.ListParam[1].Index    = (int)MAMethod.Simple;
            IndParam.ListParam[1].Text     = IndParam.ListParam[1].ItemList[IndParam.ListParam[1].Index];
            IndParam.ListParam[1].Enabled  = true;
            IndParam.ListParam[1].ToolTip  = "The Moving Average method used for smoothing.";

            IndParam.NumParam[0].Caption = "%K period";
            IndParam.NumParam[0].Value   = 5;
            IndParam.NumParam[0].Min     = 1;
            IndParam.NumParam[0].Max     = 200;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "The smoothing period of %K.";

            IndParam.NumParam[1].Caption = "Fast %D period";
            IndParam.NumParam[1].Value   = 3;
            IndParam.NumParam[1].Min     = 1;
            IndParam.NumParam[1].Max     = 200;
            IndParam.NumParam[1].Enabled = true;
            IndParam.NumParam[1].ToolTip = "The smoothing period of Fast %D.";

            IndParam.NumParam[2].Caption = "Slow %D period";
            IndParam.NumParam[2].Value   = 3;
            IndParam.NumParam[2].Min     = 1;
            IndParam.NumParam[2].Max     = 200;
            IndParam.NumParam[2].Enabled = true;
            IndParam.NumParam[2].ToolTip = "The smoothing period of Slow %D.";

            IndParam.NumParam[3].Caption = "Level";
            IndParam.NumParam[3].Value   = 20;
            IndParam.NumParam[3].Min     = 0;
            IndParam.NumParam[3].Max     = 100;
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
            MAMethod maMethod = (MAMethod)IndParam.ListParam[1].Index;
            int iK     = (int)IndParam.NumParam[0].Value;
            int iDFast = (int)IndParam.NumParam[1].Value;
            int iDSlow = (int)IndParam.NumParam[2].Value;
            int iLevel = (int)IndParam.NumParam[3].Value;
            int iPrvs  = IndParam.CheckParam[0].Checked ? 1 : 0;

            // Calculation
            int iFirstBar = iK + iDFast + iDSlow + 3;

            double[] adHighs = new double[Bars];
            double[] adLows  = new double[Bars];
            for (int iBar = 0; iBar < iK; iBar++)
            {
                double dMin = double.MaxValue;
                double dMax = double.MinValue;
                for (int i = 0; i < iBar; i++)
                {
                    if (High[iBar - i] > dMax) dMax = High[iBar - i];
                    if (Low[iBar  - i] < dMin) dMin = Low[iBar  - i];
                }
                adHighs[iBar] = dMax;
                adLows[iBar]  = dMin;
            }
            adHighs[0] = High[0];
            adLows[0]  = Low[0];

            for (int iBar = iK; iBar < Bars; iBar++)
            {
                double dMin = double.MaxValue;
                double dMax = double.MinValue;
                for (int i = 0; i < iK; i++)
                {
                    if (High[iBar - i] > dMax) dMax = High[iBar - i];
                    if (Low[iBar  - i] < dMin) dMin = Low[iBar  - i];
                }
                adHighs[iBar] = dMax;
                adLows[iBar]  = dMin;
            }

            double[] adK = new double[Bars];
            for (int iBar = iK; iBar < Bars; iBar++)
            {
                if (adHighs[iBar] == adLows[iBar])
                    adK[iBar] = 50;
                else
                    adK[iBar] = 100 * (Close[iBar] - adLows[iBar]) / (adHighs[iBar] - adLows[iBar]);
            }

            double[] adDFast = new double[Bars];
            for (int iBar = iDFast; iBar < Bars; iBar++)
            {
                double dSumHigh = 0;
                double dSumLow  = 0;
                for (int i = 0; i < iDFast; i++)
                {
                    dSumLow  += Close[iBar - i]   - adLows[iBar - i];
                    dSumHigh += adHighs[iBar - i] - adLows[iBar - i];
                }
                if (dSumHigh == 0)
                    adDFast[iBar] = 100;
                else
                    adDFast[iBar] = 100 * dSumLow / dSumHigh;
            }

            double[] adDSlow = MovingAverage(iDSlow, 0, maMethod, adDFast);

            // Saving the components
            Component = new IndicatorComp[5];

            Component[0] = new IndicatorComp();
            Component[0].CompName   = "%K";
            Component[0].DataType   = IndComponentType.IndicatorValue;
            Component[0].ChartType  = IndChartType.Line;
            Component[0].ChartColor = Color.Brown;
            Component[0].FirstBar   = iFirstBar;
            Component[0].Value      = adK;

            Component[1] = new IndicatorComp();
            Component[1].CompName   = "Fast %D";
            Component[1].DataType   = IndComponentType.IndicatorValue;
            Component[1].ChartType  = IndChartType.Line;
            Component[1].ChartColor = Color.Gold;
            Component[1].FirstBar   = iFirstBar;
            Component[1].Value      = adDFast;

            Component[2] = new IndicatorComp();
            Component[2].CompName   = "Slow %D";
            Component[2].DataType   = IndComponentType.IndicatorValue;
            Component[2].ChartType  = IndChartType.Line;
            Component[2].ChartColor = Color.Blue;
            Component[2].FirstBar   = iFirstBar;
            Component[2].Value      = adDSlow;

            Component[3] = new IndicatorComp();
            Component[3].ChartType = IndChartType.NoChart;
            Component[3].FirstBar  = iFirstBar;
            Component[3].Value     = new double[Bars];

            Component[4] = new IndicatorComp();
            Component[4].ChartType = IndChartType.NoChart;
            Component[4].FirstBar  = iFirstBar;
            Component[4].Value     = new double[Bars];

            // Sets the Component's type
            if (slotType == SlotTypes.OpenFilter)
            {
                Component[3].DataType = IndComponentType.AllowOpenLong;
                Component[3].CompName = "Is long entry allowed";
                Component[4].DataType = IndComponentType.AllowOpenShort;
                Component[4].CompName = "Is short entry allowed";
            }
            else if (slotType == SlotTypes.CloseFilter)
            {
                Component[3].DataType = IndComponentType.ForceCloseLong;
                Component[3].CompName = "Close out long position";
                Component[4].DataType = IndComponentType.ForceCloseShort;
                Component[4].CompName = "Close out short position";
            }

            // Calculation of the logic
            IndicatorLogic indLogic = IndicatorLogic.It_does_not_act_as_a_filter;

            if (IndParam.ListParam[0].Text == "The %K crosses the Slow %D upward")
            {
                SpecialValues = new double[1] { 50 };
                IndicatorCrossesAnotherIndicatorUpwardLogic(iFirstBar, iPrvs,adK, adDSlow, ref Component[3], ref Component[4]);
                return;
            }
            else if (IndParam.ListParam[0].Text == "The %K crosses the Slow %D downward")
            {
                SpecialValues = new double[1] { 50 };
                IndicatorCrossesAnotherIndicatorDownwardLogic(iFirstBar, iPrvs, adK, adDSlow, ref Component[3], ref Component[4]);
                return;
            }
            else if (IndParam.ListParam[0].Text == "The %K is higher than the Slow %D")
            {
                SpecialValues = new double[1] { 50 };
                IndicatorIsHigherThanAnotherIndicatorLogic(iFirstBar, iPrvs, adK, adDSlow, ref Component[3], ref Component[4]);
                return;
            }
            else if (IndParam.ListParam[0].Text == "The %K is lower than the Slow %D")
            {
                SpecialValues = new double[1] { 50 };
                IndicatorIsLowerThanAnotherIndicatorLogic(iFirstBar, iPrvs, adK, adDSlow, ref Component[3], ref Component[4]);
                return;
            }
            else
            {
                switch (IndParam.ListParam[0].Text)
                {
                    case "The Slow %D rises":
                        indLogic = IndicatorLogic.The_indicator_rises;
                        SpecialValues = new double[1] { 50 };
                        break;

                    case "The Slow %D falls":
                        indLogic = IndicatorLogic.The_indicator_falls;
                        SpecialValues = new double[1] { 50 };
                        break;

                    case "The Slow %D is higher than the Level line":
                        indLogic = IndicatorLogic.The_indicator_is_higher_than_the_level_line;
                        SpecialValues = new double[2] { iLevel, 100 - iLevel };
                        break;

                    case "The Slow %D is lower than the Level line":
                        indLogic = IndicatorLogic.The_indicator_is_lower_than_the_level_line;
                        SpecialValues = new double[2] { iLevel, 100 - iLevel };
                        break;

                    case "The Slow %D crosses the Level line upward":
                        indLogic = IndicatorLogic.The_indicator_crosses_the_level_line_upward;
                        SpecialValues = new double[2] { iLevel, 100 - iLevel };
                        break;

                    case "The Slow %D crosses the Level line downward":
                        indLogic = IndicatorLogic.The_indicator_crosses_the_level_line_downward;
                        SpecialValues = new double[2] { iLevel, 100 - iLevel };
                        break;

                    case "The Slow %D changes its direction upward":
                        indLogic = IndicatorLogic.The_indicator_changes_its_direction_upward;
                        SpecialValues = new double[1] { 50 };
                        break;

                    case "The Slow %D changes its direction downward":
                        indLogic = IndicatorLogic.The_indicator_changes_its_direction_downward;
                        SpecialValues = new double[1] { 50 };
                        break;

                    default:
                        break;
                }

                OscillatorLogic(iFirstBar, iPrvs, adDSlow, iLevel, 100 - iLevel, ref Component[3], ref Component[4], indLogic);
            }

            return;
        }

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {
            string sLevelLong  = IndParam.NumParam[3].ValueToString;
            string sLevelShort = IndParam.NumParam[3].AnotherValueToString(100 - IndParam.NumParam[3].Value);

            EntryFilterLongDescription  = ToString() + " - ";
            EntryFilterShortDescription = ToString() + " - ";
            ExitFilterLongDescription   = ToString() + " - ";
            ExitFilterShortDescription  = ToString() + " - ";

            switch (IndParam.ListParam[0].Text)
            {
                case "The Slow %D rises":
                    EntryFilterLongDescription  += "the Slow %D rises";
                    EntryFilterShortDescription += "the Slow %D falls";
                    ExitFilterLongDescription   += "the Slow %D rises";
                    ExitFilterShortDescription  += "the Slow %D falls";
                    break;

                case "The Slow %D falls":
                    EntryFilterLongDescription  += "the Slow %D falls";
                    EntryFilterShortDescription += "the Slow %D rises";
                    ExitFilterLongDescription   += "the Slow %D falls";
                    ExitFilterShortDescription  += "the Slow %D rises";
                    break;

                case "The Slow %D is higher than the Level line":
                    EntryFilterLongDescription  += "the Slow %D is higher than the Level " + sLevelLong;
                    EntryFilterShortDescription += "the Slow %D is lower than the Level "  + sLevelShort;
                    ExitFilterLongDescription   += "the Slow %D is higher than the Level " + sLevelLong;
                    ExitFilterShortDescription  += "the Slow %D is lower than the Level "  + sLevelShort;
                    break;

                case "The Slow %D is lower than the Level line":
                    EntryFilterLongDescription  += "the Slow %D is lower than the Level "  + sLevelLong;
                    EntryFilterShortDescription += "the Slow %D is higher than the Level " + sLevelShort;
                    ExitFilterLongDescription   += "the Slow %D is lower than the Level "  + sLevelLong;
                    ExitFilterShortDescription  += "the Slow %D is higher than the Level " + sLevelShort;
                    break;

                case "The Slow %D crosses the Level line upward":
                    EntryFilterLongDescription  += "the Slow %D crosses the Level " + sLevelLong  + " upward";
                    EntryFilterShortDescription += "the Slow %D crosses the Level " + sLevelShort + " downward";
                    ExitFilterLongDescription   += "the Slow %D crosses the Level " + sLevelLong  + " upward";
                    ExitFilterShortDescription  += "the Slow %D crosses the Level " + sLevelShort + " downward";
                    break;

                case "The Slow %D crosses the Level line downward":
                    EntryFilterLongDescription  += "the Slow %D crosses the Level " + sLevelLong  + " downward";
                    EntryFilterShortDescription += "the Slow %D crosses the Level " + sLevelShort + " upward";
                    ExitFilterLongDescription   += "the Slow %D crosses the Level " + sLevelLong  + " downward";
                    ExitFilterShortDescription  += "the Slow %D crosses the Level " + sLevelShort + " upward";
                    break;

                case "The %K crosses the Slow %D upward":
                    EntryFilterLongDescription  += "the %K crosses the Slow %D upward";
                    EntryFilterShortDescription += "the %K crosses the Slow %D downward";
                    ExitFilterLongDescription   += "the %K crosses the Slow %D upward";
                    ExitFilterShortDescription  += "the %K crosses the Slow %D downward";
                    break;

                case "The %K crosses the Slow %D downward":
                    EntryFilterLongDescription  += "the %K crosses the Slow %D downward";
                    EntryFilterShortDescription += "the %K crosses the Slow %D upward";
                    ExitFilterLongDescription   += "the %K crosses the Slow %D downward";
                    ExitFilterShortDescription  += "the %K crosses the Slow %D upward";
                    break;

                case "The %K is higher than the Slow %D":
                    EntryFilterLongDescription  += "the %K is higher than the Slow %D";
                    EntryFilterShortDescription += "the %K is lower than the Slow %D";
                    ExitFilterLongDescription   += "the %K is higher than the Slow %D";
                    ExitFilterShortDescription  += "the %K is lower than the Slow %D";
                    break;

                case "The %K is lower than  the Slow %D":
                    EntryFilterLongDescription  += "the %K is lower than the Slow %D";
                    EntryFilterShortDescription += "the %K is higher than than the Slow %D";
                    ExitFilterLongDescription   += "the %K is lower than the Slow %D";
                    ExitFilterShortDescription  += "the %K is higher than than the Slow %D";
                    break;

                case "The Slow %D changes its direction upward":
                    EntryFilterLongDescription  += "the Slow %D changes its direction upward";
                    EntryFilterShortDescription += "the Slow %D changes its direction downward";
                    ExitFilterLongDescription   += "the Slow %D changes its direction upward";
                    ExitFilterShortDescription  += "the Slow %D changes its direction downward";
                    break;

                case "The Slow %D changes its direction downward":
                    EntryFilterLongDescription  += "the Slow %D changes its direction downward";
                    EntryFilterShortDescription += "the Slow %D changes its direction upward";
                    ExitFilterLongDescription   += "the Slow %D changes its direction downward";
                    ExitFilterShortDescription  += "the Slow %D changes its direction upward";
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
                IndParam.ListParam[1].Text         + ", " + // Smoothing method
                IndParam.NumParam[0].ValueToString + ", " + // %K period
                IndParam.NumParam[1].ValueToString + ", " + // Fast %D period
                IndParam.NumParam[2].ValueToString + ")";   // Slow %D period

            return sString;
        }
    }
}
