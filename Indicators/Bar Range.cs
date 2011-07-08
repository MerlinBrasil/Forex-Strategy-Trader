// Bar Range Indicator
// Last changed on 2009-05-11
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2009 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Bar Range Indicator
    /// </summary>
    public class Bar_Range : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Bar_Range(SlotTypes slotType)
        {
            // General properties
            IndicatorName  = "Bar Range";
            PossibleSlots  = SlotTypes.OpenFilter | SlotTypes.CloseFilter;
            SeparatedChart = true;
            SeparatedChartMinValue = 0;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption  = "Logic";
            IndParam.ListParam[0].ItemList = new string[]
            {
                "The Bar Range rises",
                "The Bar Range falls",
                "The Bar Range is higher than the Level line",
                "The Bar Range is lower than the Level line"
            };
            IndParam.ListParam[0].Index   = 0;
            IndParam.ListParam[0].Text    = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Logic of application of the indicator.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "Number of bars";
            IndParam.NumParam[0].Value   = 1;
            IndParam.NumParam[0].Min     = 1;
            IndParam.NumParam[0].Max     = 200;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "The number of bars to calculate the range.";

            IndParam.NumParam[1].Caption = "Level";
            IndParam.NumParam[1].Value   = 0;
            IndParam.NumParam[1].Min     = 0;
            IndParam.NumParam[1].Max     = 5000;
            IndParam.NumParam[1].Point   = 0;
            IndParam.NumParam[1].Enabled = true;
            IndParam.NumParam[1].ToolTip = "A critical level (for the appropriate logic).";

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
            int    nBars  = (int)IndParam.NumParam[0].Value;
            double dLevel = IndParam.NumParam[1].Value;
            int    iPrvs  = IndParam.CheckParam[0].Checked ? 1 : 0;

            // Calculation
            int iFirstBar = nBars + 1;
	
			double[] adRange = new double[Bars];

            for (int iBar = iFirstBar; iBar < Bars; iBar++)
            {
                double maxHigh = double.MinValue;
                double minLow  = double.MaxValue;
                for (int i = 0; i < nBars; i++)
                {
                    if (High[iBar - i] > maxHigh)
                        maxHigh = High[iBar - i];
                    if (Low[iBar - i] < minLow)
                        minLow = Low[iBar - i];
                }
                adRange[iBar] = maxHigh - minLow;
            }
            
            // Saving the components
            Component = new IndicatorComp[3];

            Component[0] = new IndicatorComp();
            Component[0].CompName  = "Bar Range";
            Component[0].DataType  = IndComponentType.IndicatorValue;
            Component[0].ChartType = IndChartType.Histogram;
            Component[0].FirstBar  = iFirstBar;
            Component[0].Value     = new double[Bars];
            for (int i = 0; i < Bars; i++)
                Component[0].Value[i] = (double)Math.Round(adRange[i] / Point);

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
                case "The Bar Range rises":
                    indLogic = IndicatorLogic.The_indicator_rises;
                    break;

                case "The Bar Range falls":
                    indLogic = IndicatorLogic.The_indicator_falls;
                    break;

                case "The Bar Range is higher than the Level line":
                    indLogic = IndicatorLogic.The_indicator_is_higher_than_the_level_line;
                    SpecialValues = new double[1] { dLevel };
                    break;

                case "The Bar Range is lower than the Level line":
                    indLogic = IndicatorLogic.The_indicator_is_lower_than_the_level_line;
                    SpecialValues = new double[1] { dLevel };
                    break;

                default:
                    break;
            }

            NoDirectionOscillatorLogic(iFirstBar, iPrvs, adRange, dLevel * Point, ref Component[1], indLogic);
            Component[2].Value = Component[1].Value;

            return;
        }

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {
            int    nBars       = (int) IndParam.NumParam[0].Value;
            string sLevelLong  = IndParam.NumParam[1].ValueToString;
            string sLevelShort = sLevelLong;

            if (nBars == 1)
            {
                EntryFilterLongDescription  = "the range of the bar ";
                EntryFilterShortDescription = "the range of the bar ";
                ExitFilterLongDescription   = "the range of the bar ";
                ExitFilterShortDescription  = "the range of the bar ";
            }
            else
            {
                EntryFilterLongDescription  = "the range of the last " + nBars.ToString() + " bars ";
                EntryFilterShortDescription = "the range of the last " + nBars.ToString() + " bars ";
                ExitFilterLongDescription   = "the range of the last " + nBars.ToString() + " bars ";
                ExitFilterShortDescription  = "the range of the last " + nBars.ToString() + " bars ";
            }
            switch (IndParam.ListParam[0].Text)
            {
                case "The Bar Range rises":
                    EntryFilterLongDescription  += "rises";
                    EntryFilterShortDescription += "rises";
                    ExitFilterLongDescription   += "falls";
                    ExitFilterShortDescription  += "falls";
                    break;

                case "The Bar Range falls":
                    EntryFilterLongDescription  += "falls";
                    EntryFilterShortDescription += "falls";
                    ExitFilterLongDescription   += "rises";
                    ExitFilterShortDescription  += "rises";
                    break;

                case "The Bar Range is higher than the Level line":
                    EntryFilterLongDescription  += "is higher than " + sLevelLong  + " pips";
                    EntryFilterShortDescription += "is higher than " + sLevelShort + " pips";
                    ExitFilterLongDescription   += "is higher than " + sLevelLong  + " pips";
                    ExitFilterShortDescription  += "is higher than " + sLevelShort + " pips";
                    break;

                case "The Bar Range is lower than the Level line":
                    EntryFilterLongDescription  += "is lower than " + sLevelLong  + " pips";
                    EntryFilterShortDescription += "is lower than " + sLevelShort + " pips";
                    ExitFilterLongDescription   += "is lower than " + sLevelLong  + " pips";
                    ExitFilterShortDescription  += "is lower than " + sLevelShort + " pips";
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
                IndParam.NumParam[0].ValueToString + ", " + // Number of bars
                IndParam.NumParam[1].ValueToString + ")";   // Level

            return sString;
        }
    }
}