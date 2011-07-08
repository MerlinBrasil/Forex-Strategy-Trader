// Average True Range Indicator
// Last changed on 2010-07-15
// Copyright (c) 2006 - 2010 Miroslav Popov - All rights reserved.
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Average True Range Indicator
    /// </summary>
    public class Average_True_Range : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Average_True_Range(SlotTypes slotType)
        {
            // General properties
            IndicatorName   = "Average True Range";
            PossibleSlots   = SlotTypes.OpenFilter | SlotTypes.CloseFilter;
            SeparatedChart  = true;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption  = "Logic";
            IndParam.ListParam[0].ItemList = new string[]
            {
                "The ATR rises",
                "The ATR falls",
                "The ATR is higher than the Level line",
                "The ATR is lower than the Level line",
                "The ATR crosses the Level line upward",
                "The ATR crosses the Level line downward",
                "The ATR changes its direction upward",
                "The ATR changes its direction downward"
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
            IndParam.ListParam[1].ToolTip  = "The Moving Average method used for smoothing the ATR.";

            IndParam.ListParam[2].Caption  = "Base price";
            IndParam.ListParam[2].ItemList = new string[] { "Bar range" };
            IndParam.ListParam[2].Index    = 0;
            IndParam.ListParam[2].Text     = IndParam.ListParam[2].ItemList[IndParam.ListParam[2].Index];
            IndParam.ListParam[2].Enabled  = true;
            IndParam.ListParam[2].ToolTip  = "ATR uses the range of the current bar";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "Smoothing period";
            IndParam.NumParam[0].Value   = 14;
            IndParam.NumParam[0].Min     = 1;
            IndParam.NumParam[0].Max     = 200;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "The period of ATR smoothing.";

            IndParam.NumParam[1].Caption = "Level [pips]";
            IndParam.NumParam[1].Value   = 100;
            IndParam.NumParam[1].Min     = 0;
            IndParam.NumParam[1].Max     = 2000;
            IndParam.NumParam[1].Point   = 0;
            IndParam.NumParam[1].Enabled = true;
            IndParam.NumParam[1].ToolTip = "A critical level in pips(for the appropriate logic).";

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
            int period = (int)IndParam.NumParam[0].Value;
            double level = Point * IndParam.NumParam[1].Value;
            int prev = IndParam.CheckParam[0].Checked ? 1 : 0;

            // Calculation
            int firstBar = period + 2;
	
			double[] ATR = new double[Bars];

            for (int bar = 1; bar < Bars; bar++)
                ATR[bar] = Math.Max(High[bar], Close[bar - 1]) - Math.Min(Low[bar], Close[bar - 1]);

            ATR = MovingAverage(period, 0, maMethod, ATR);
            
            // Saving the components
            Component = new IndicatorComp[3];

            Component[0] = new IndicatorComp();
            Component[0].CompName   = "Average True Range";
            Component[0].DataType   = IndComponentType.IndicatorValue;
            Component[0].ChartType  = IndChartType.Line;
            Component[0].ChartColor = Color.Blue;
            Component[0].FirstBar   = firstBar;
            Component[0].Value      = ATR;

            Component[1] = new IndicatorComp();
            Component[1].ChartType = IndChartType.NoChart;
            Component[1].FirstBar  = firstBar;
            Component[1].Value     = new double[Bars];

            Component[2] = new IndicatorComp();
            Component[2].ChartType = IndChartType.NoChart;
            Component[2].FirstBar  = firstBar;
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
                case "The ATR rises":
                    indLogic = IndicatorLogic.The_indicator_rises;
                    break;

                case "The ATR falls":
                    indLogic = IndicatorLogic.The_indicator_falls;
                    break;

                case "The ATR is higher than the Level line":
                    indLogic = IndicatorLogic.The_indicator_is_higher_than_the_level_line;
                    SpecialValues = new double[1] { level };
                    break;

                case "The ATR is lower than the Level line":
                    indLogic = IndicatorLogic.The_indicator_is_lower_than_the_level_line;
                    SpecialValues = new double[1] { level };
                    break;

                case "The ATR crosses the Level line upward":
                    indLogic = IndicatorLogic.The_indicator_crosses_the_level_line_upward;
                    SpecialValues = new double[1] { level };
                    break;

                case "The ATR crosses the Level line downward":
                    indLogic = IndicatorLogic.The_indicator_crosses_the_level_line_downward;
                    SpecialValues = new double[1] { level };
                    break;

                case "The ATR changes its direction upward":
                    indLogic = IndicatorLogic.The_indicator_changes_its_direction_upward;
                    break;

                case "The ATR changes its direction downward":
                    indLogic = IndicatorLogic.The_indicator_changes_its_direction_downward;
                    break;

                default:
                    break;
            }

            // ATR rises equal signals in both directions!
            NoDirectionOscillatorLogic(firstBar, prev, ATR, level, ref Component[1], indLogic);
            Component[2].Value = Component[1].Value;

            return;
        }

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {
            string sLevelLong  = IndParam.NumParam[1].ValueToString;
            string sLevelShort = sLevelLong;

            EntryFilterLongDescription  = "the " + ToString() + " ";
            EntryFilterShortDescription = "the " + ToString() + " ";
            ExitFilterLongDescription   = "the " + ToString() + " ";
            ExitFilterShortDescription  = "the " + ToString() + " ";

            switch (IndParam.ListParam[0].Text)
            {
                case "The ATR rises":
                    EntryFilterLongDescription  += "rises";
                    EntryFilterShortDescription += "rises";
                    ExitFilterLongDescription   += "rises";
                    ExitFilterShortDescription  += "rises";
                    break;

                case "The ATR falls":
                    EntryFilterLongDescription  += "falls";
                    EntryFilterShortDescription += "falls";
                    ExitFilterLongDescription   += "falls";
                    ExitFilterShortDescription  += "falls";
                    break;

                case "The ATR is higher than the Level line":
                    EntryFilterLongDescription  += "is higher than the Level " + sLevelLong;
                    EntryFilterShortDescription += "is higher than the Level " + sLevelShort;
                    ExitFilterLongDescription   += "is higher than the Level " + sLevelLong;
                    ExitFilterShortDescription  += "is higher than the Level " + sLevelShort;
                    break;

                case "The ATR is lower than the Level line":
                    EntryFilterLongDescription  += "is lower than the Level " + sLevelLong;
                    EntryFilterShortDescription += "is lower than the Level " + sLevelShort;
                    ExitFilterLongDescription   += "is lower than the Level " + sLevelLong;
                    ExitFilterShortDescription  += "is lower than the Level " + sLevelShort;
                    break;

                case "The ATR crosses the Level line upward":
                    EntryFilterLongDescription  += "crosses the Level " + sLevelLong  + " upward";
                    EntryFilterShortDescription += "crosses the Level " + sLevelShort + " upward";
                    ExitFilterLongDescription   += "crosses the Level " + sLevelLong  + " upward";
                    ExitFilterShortDescription  += "crosses the Level " + sLevelShort + " upward";
                    break;

                case "The ATR crosses the Level line downward":
                    EntryFilterLongDescription  += "crosses the Level " + sLevelLong  + " downward";
                    EntryFilterShortDescription += "crosses the Level " + sLevelShort + " downward";
                    ExitFilterLongDescription   += "crosses the Level " + sLevelLong  + " downward";
                    ExitFilterShortDescription  += "crosses the Level " + sLevelShort + " downward";
                    break;

                case "The ATR changes its direction upward":
                    EntryFilterLongDescription  += "changes its direction upward";
                    EntryFilterShortDescription += "changes its direction upward";
                    ExitFilterLongDescription   += "changes its direction upward";
                    ExitFilterShortDescription  += "changes its direction upward";
                    break;

                case "The ATR changes its direction downward":
                    EntryFilterLongDescription  += "changes its direction downward";
                    EntryFilterShortDescription += "changes its direction downward";
                    ExitFilterLongDescription   += "changes its direction downward";
                    ExitFilterShortDescription  += "changes its direction downward";
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
                IndParam.NumParam[0].ValueToString + ")";   // Period

            return sString;
        }
    }
}