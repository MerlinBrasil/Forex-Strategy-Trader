// ADX Indicator
// Last changed on 2010-07-15
// Copyright (c) 2006 - 2010 Miroslav Popov - All rights reserved.
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// ADX Indicator
    /// </summary>
    public class ADX : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public ADX(SlotTypes slotType)
        {
            // General properties
            IndicatorName  = "ADX";
            PossibleSlots  = SlotTypes.OpenFilter | SlotTypes.CloseFilter;
            SeparatedChart = true;
            SeparatedChartMinValue = 0;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType = slotType;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption  = "Logic";
            IndParam.ListParam[0].ItemList = new string[]
            {
                "The ADX rises",
                "The ADX falls",
                "The ADX is higher than the Level line",
                "The ADX is lower than the Level line",
                "The ADX crosses the Level line upward",
                "The ADX crosses the Level line downward",
                "The ADX changes its direction upward",
                "The ADX changes its direction downward"
            };
            IndParam.ListParam[0].Index   = 0;
            IndParam.ListParam[0].Text    = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Logic of application of the indicator.";

            IndParam.ListParam[1].Caption  = "Smoothing method";
            IndParam.ListParam[1].ItemList = Enum.GetNames(typeof(MAMethod));
            IndParam.ListParam[1].Index    = (int)MAMethod.Exponential;
            IndParam.ListParam[1].Text     = IndParam.ListParam[1].ItemList[IndParam.ListParam[1].Index];
            IndParam.ListParam[1].Enabled  = true;
            IndParam.ListParam[1].ToolTip  = "The Moving Average method used for smoothing the ADX value.";

            IndParam.ListParam[2].Caption  = "Base price";
            IndParam.ListParam[2].ItemList = new string[] { "Bar range" };
            IndParam.ListParam[2].Index    = 0;
            IndParam.ListParam[2].Text     = IndParam.ListParam[2].ItemList[IndParam.ListParam[2].Index];
            IndParam.ListParam[2].Enabled  = true;
            IndParam.ListParam[2].ToolTip  = "ADX uses current and previous bar ranges.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "Period";
            IndParam.NumParam[0].Value   = 14;
            IndParam.NumParam[0].Min     = 1;
            IndParam.NumParam[0].Max     = 200;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "The period of ADX.";

            IndParam.NumParam[1].Caption = "Level";
            IndParam.NumParam[1].Value   = 0;
            IndParam.NumParam[1].Min     = 0;
            IndParam.NumParam[1].Max     = 100;
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
            MAMethod maMethod = (MAMethod)IndParam.ListParam[1].Index;
            int period = (int)IndParam.NumParam[0].Value;
            double level = IndParam.NumParam[1].Value;
            int prev = IndParam.CheckParam[0].Checked ? 1 : 0;

            // Calculation
            int firstBar = 2 * period + 2;

            double[] DIPos = new double[Bars];
            double[] DINeg = new double[Bars];

            for (int bar = 1; bar < Bars; bar++)
            {
                double trueRange = Math.Max(High[bar], Close[bar - 1]) - Math.Min(Low[bar], Close[bar - 1]);

                if (trueRange < Point)
                    trueRange = Point;

                double deltaHigh = High[bar] - High[bar - 1];
                double deltaLow  = Low[bar - 1] - Low[bar];

                if (deltaHigh > 0 && deltaHigh > deltaLow)
                    DIPos[bar] = 100 * deltaHigh / trueRange;
                else
                    DIPos[bar] = 0;

                if (deltaLow > 0 && deltaLow > deltaHigh)
                    DINeg[bar] = 100 * deltaLow / trueRange;
                else
                    DINeg[bar] = 0;
            }

            double[] ADIPos = MovingAverage(period, 0, maMethod, DIPos);
            double[] ADINeg = MovingAverage(period, 0, maMethod, DINeg);

            double[] DX = new double[Bars];

            for (int bar = 0; bar < Bars; bar++)
            {
                if (ADIPos[bar] + ADINeg[bar] == 0)
                    DX[bar] = 0;
                else
                    DX[bar] = 100 * Math.Abs((ADIPos[bar] - ADINeg[bar]) / (ADIPos[bar] + ADINeg[bar]));
            }

            double[] ADX = MovingAverage(period, 0, maMethod, DX);

            // Saving the components
            Component = new IndicatorComp[5];

            Component[0] = new IndicatorComp();
            Component[0].CompName   = "ADX";
            Component[0].DataType   = IndComponentType.IndicatorValue;
            Component[0].ChartType  = IndChartType.Line;
            Component[0].ChartColor = Color.Blue;
            Component[0].FirstBar   = firstBar;
            Component[0].Value      = ADX;

            Component[1] = new IndicatorComp();
            Component[1].CompName   = "ADI+";
            Component[1].DataType   = IndComponentType.IndicatorValue;
            Component[1].ChartType  = IndChartType.Line;
            Component[1].ChartColor = Color.Green;
            Component[1].FirstBar   = firstBar;
            Component[1].Value      = ADIPos;

            Component[2] = new IndicatorComp();
            Component[2].CompName   = "ADI-";
            Component[2].DataType   = IndComponentType.IndicatorValue;
            Component[2].ChartType  = IndChartType.Line;
            Component[2].ChartColor = Color.Red;
            Component[2].FirstBar   = firstBar;
            Component[2].Value      = ADINeg;

            Component[3] = new IndicatorComp();
            Component[3].ChartType  = IndChartType.NoChart;
            Component[3].FirstBar   = firstBar;
            Component[3].Value      = new double[Bars];

            Component[4] = new IndicatorComp();
            Component[4].ChartType  = IndChartType.NoChart;
            Component[4].FirstBar   = firstBar;
            Component[4].Value      = new double[Bars];

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
            IndicatorLogic logicRule;

            switch (IndParam.ListParam[0].Text)
            {
                case "The ADX rises":
                    logicRule = IndicatorLogic.The_indicator_rises;
                    break;

                case "The ADX falls":
                    logicRule = IndicatorLogic.The_indicator_falls;
                    break;

                case "The ADX is higher than the Level line":
                    logicRule = IndicatorLogic.The_indicator_is_higher_than_the_level_line;
                    SpecialValues = new double[1] { level };
                    break;

                case "The ADX is lower than the Level line":
                    logicRule = IndicatorLogic.The_indicator_is_lower_than_the_level_line;
                    SpecialValues = new double[1] { level };
                    break;

                case "The ADX crosses the Level line upward":
                    logicRule = IndicatorLogic.The_indicator_crosses_the_level_line_upward;
                    SpecialValues = new double[1] { level };
                    break;

                case "The ADX crosses the Level line downward":
                    logicRule = IndicatorLogic.The_indicator_crosses_the_level_line_downward;
                    SpecialValues = new double[1] { level };
                    break;

                case "The ADX changes its direction upward":
                    logicRule = IndicatorLogic.The_indicator_changes_its_direction_upward;
                    break;

                case "The ADX changes its direction downward":
                    logicRule = IndicatorLogic.The_indicator_changes_its_direction_downward;
                    break;

                default:
                    logicRule = IndicatorLogic.It_does_not_act_as_a_filter;
                    break;
            }

            // ADX rises equal signals in both directions!
            NoDirectionOscillatorLogic(firstBar, prev, ADX, level, ref Component[3], logicRule);
            Component[4].Value = Component[3].Value;

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
                case "The ADX rises":
                    EntryFilterLongDescription  += "rises";
                    EntryFilterShortDescription += "rises";
                    ExitFilterLongDescription   += "rises";
                    ExitFilterShortDescription  += "rises";
                    break;

                case "The ADX falls":
                    EntryFilterLongDescription  += "falls";
                    EntryFilterShortDescription += "falls";
                    ExitFilterLongDescription   += "falls";
                    ExitFilterShortDescription  += "falls";
                    break;

                case "The ADX is higher than the Level line":
                    EntryFilterLongDescription  += "is higher than the Level " + sLevelLong;
                    EntryFilterShortDescription += "is higher than the Level " + sLevelShort;
                    ExitFilterLongDescription   += "is higher than the Level " + sLevelLong;
                    ExitFilterShortDescription  += "is higher than the Level " + sLevelShort;
                    break;

                case "The ADX is lower than the Level line":
                    EntryFilterLongDescription  += "is lower than the Level " + sLevelLong;
                    EntryFilterShortDescription += "is lower than the Level " + sLevelShort;
                    ExitFilterLongDescription   += "is lower than the Level " + sLevelLong;
                    ExitFilterShortDescription  += "is lower than the Level " + sLevelShort;
                    break;

                case "The ADX crosses the Level line upward":
                    EntryFilterLongDescription  += "crosses the Level " + sLevelLong  + " upward";
                    EntryFilterShortDescription += "crosses the Level " + sLevelShort + " upward";
                    ExitFilterLongDescription   += "crosses the Level " + sLevelLong  + " upward";
                    ExitFilterShortDescription  += "crosses the Level " + sLevelShort + " upward";
                    break;

                case "The ADX crosses the Level line downward":
                    EntryFilterLongDescription  += "crosses the Level " + sLevelLong  + " downward";
                    EntryFilterShortDescription += "crosses the Level " + sLevelShort + " downward";
                    ExitFilterLongDescription   += "crosses the Level " + sLevelLong  + " downward";
                    ExitFilterShortDescription  += "crosses the Level " + sLevelShort + " downward";
                    break;

                case "The ADX changes its direction upward":
                    EntryFilterLongDescription  += "changes its direction upward";
                    EntryFilterShortDescription += "changes its direction upward";
                    ExitFilterLongDescription   += "changes its direction upward";
                    ExitFilterShortDescription  += "changes its direction upward";
                    break;

                case "The ADX changes its direction downward":
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
            string indicator = IndicatorName +
                (IndParam.CheckParam[0].Checked ? "* (" : " (") +
                IndParam.ListParam[1].Text         + ", " + // Smoothing method
                IndParam.ListParam[2].Text         + ", " + // Base price
                IndParam.NumParam[0].ValueToString + ")";   // ADX Period

            return indicator;
        }
    }
}