// Directional Indicators
// Last changed on 2010-07-15
// Copyright (c) 2006 - 2010 Miroslav Popov - All rights reserved.
// Part of Forex Strategy Builder and Forex Strategy Trader
// Website http://forexsb.com/
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Directional Indicators
    /// </summary>
    public class Directional_Indicators : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Directional_Indicators(SlotTypes slotType)
        {
            // General properties
            IndicatorName  = "Directional Indicators";
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
                "The ADI+ rises",
                "The ADI+ falls",
                "The ADI- rises",
                "The ADI- falls",
                "The ADI+ is higher than ADI-",
                "The ADI+ is lower than ADI-",
                "The ADI+ crosses the ADI- line upward",
                "The ADI+ crosses the ADI- line downward",
                "The ADI+ changes its direction upward",
                "The ADI+ changes its direction downward",
                "The ADI- changes its direction upward",
                "The ADI- changes its direction downward"
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
            IndParam.ListParam[1].ToolTip  = "The Moving Average method used for ADI smoothing.";

            IndParam.ListParam[2].Caption  = "Base price";
            IndParam.ListParam[2].ItemList = new string[] { "Bar range" };
            IndParam.ListParam[2].Index    = 0;
            IndParam.ListParam[2].Text     = IndParam.ListParam[2].ItemList[IndParam.ListParam[2].Index];
            IndParam.ListParam[2].Enabled  = true;
            IndParam.ListParam[2].ToolTip  = "ADI uses the current bar range.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "Period";
            IndParam.NumParam[0].Value   = 14;
            IndParam.NumParam[0].Min     = 1;
            IndParam.NumParam[0].Max     = 200;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "The period of ADI.";

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
            int prev = IndParam.CheckParam[0].Checked ? 1 : 0;

            // Calculation
            int firstBar = period + 2;

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

            double[] ADIOsc = new double[Bars];

            for (int bar = 0; bar < Bars; bar++)
                ADIOsc[bar] = ADIPos[bar] - ADINeg[bar];

            // Saving the components
            Component = new IndicatorComp[4];

            Component[0] = new IndicatorComp();
            Component[0].CompName   = "The ADI+";
            Component[0].DataType   = IndComponentType.IndicatorValue;
            Component[0].ChartType  = IndChartType.Line;
            Component[0].ChartColor = Color.Green;
            Component[0].FirstBar   = firstBar;
            Component[0].Value      = ADIPos;

            Component[1] = new IndicatorComp();
            Component[1].CompName   = "The ADI-";
            Component[1].DataType   = IndComponentType.IndicatorValue;
            Component[1].ChartType  = IndChartType.Line;
            Component[1].ChartColor = Color.Red;
            Component[1].FirstBar   = firstBar;
            Component[1].Value      = ADINeg;

            Component[2] = new IndicatorComp();
            Component[2].ChartType = IndChartType.NoChart;
            Component[2].FirstBar  = firstBar;
            Component[2].Value     = new double[Bars];

            Component[3] = new IndicatorComp();
            Component[3].ChartType = IndChartType.NoChart;
            Component[3].FirstBar  = firstBar;
            Component[3].Value     = new double[Bars];

            // Sets the Component's type
            if (slotType == SlotTypes.OpenFilter)
            {
                Component[2].DataType = IndComponentType.AllowOpenLong;
                Component[2].CompName = "Is long entry allowed";
                Component[3].DataType = IndComponentType.AllowOpenShort;
                Component[3].CompName = "Is short entry allowed";
            }
            else if (slotType == SlotTypes.CloseFilter)
            {
                Component[2].DataType = IndComponentType.ForceCloseLong;
                Component[2].CompName = "Close out long position";
                Component[3].DataType = IndComponentType.ForceCloseShort;
                Component[3].CompName = "Close out short position";
            }

            switch (IndParam.ListParam[0].Text)
            {
                case "The ADI+ rises":
                    OscillatorLogic(firstBar, prev, ADIPos, 0, 0, ref Component[2], ref Component[3], IndicatorLogic.The_indicator_rises);
                    break;

                case "The ADI+ falls":
                    OscillatorLogic(firstBar, prev, ADIPos, 0, 0, ref Component[2], ref Component[3], IndicatorLogic.The_indicator_falls);
                    break;

                case "The ADI- rises":
                    OscillatorLogic(firstBar, prev, ADINeg, 0, 0, ref Component[2], ref Component[3], IndicatorLogic.The_indicator_rises);
                    break;

                case "The ADI- falls":
                    OscillatorLogic(firstBar, prev, ADINeg, 0, 0, ref Component[2], ref Component[3], IndicatorLogic.The_indicator_falls);
                    break;

                case "The ADI+ is higher than ADI-":
                    OscillatorLogic(firstBar, prev, ADIOsc, 0, 0, ref Component[2], ref Component[3], IndicatorLogic.The_indicator_is_higher_than_the_level_line);
                    break;

                case "The ADI+ is lower than ADI-":
                    OscillatorLogic(firstBar, prev, ADIOsc, 0, 0, ref Component[2], ref Component[3], IndicatorLogic.The_indicator_is_lower_than_the_level_line);
                    break;

                case "The ADI+ crosses the ADI- line upward":
                    OscillatorLogic(firstBar, prev, ADIOsc, 0, 0, ref Component[2], ref Component[3], IndicatorLogic.The_indicator_crosses_the_level_line_upward);
                    break;

                case "The ADI+ crosses the ADI- line downward":
                    OscillatorLogic(firstBar, prev, ADIOsc, 0, 0, ref Component[2], ref Component[3], IndicatorLogic.The_indicator_crosses_the_level_line_downward);
                    break;

                case "The ADI+ changes its direction upward":
                    OscillatorLogic(firstBar, prev, ADIPos, 0, 0, ref Component[2], ref Component[3], IndicatorLogic.The_indicator_changes_its_direction_upward);
                    break;

                case "The ADI+ changes its direction downward":
                    OscillatorLogic(firstBar, prev, ADIPos, 0, 0, ref Component[2], ref Component[3], IndicatorLogic.The_indicator_changes_its_direction_downward);
                    break;

                case "The ADI- changes its direction upward":
                    OscillatorLogic(firstBar, prev, ADINeg, 0, 0, ref Component[2], ref Component[3], IndicatorLogic.The_indicator_changes_its_direction_upward);
                    break;

                case "The ADI- changes its direction downward":
                    OscillatorLogic(firstBar, prev, ADINeg, 0, 0, ref Component[2], ref Component[3], IndicatorLogic.The_indicator_changes_its_direction_downward);
                    break;

                default:
                    break;
            }

            return;
        }

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {
            EntryFilterLongDescription  = ToString() + "; ";
            EntryFilterShortDescription = ToString() + "; ";
            ExitFilterLongDescription   = ToString() + "; ";
            ExitFilterShortDescription  = ToString() + "; ";

            switch (IndParam.ListParam[0].Text)
            {
                case "The ADI+ rises":
                    EntryFilterLongDescription  += "the ADI+ rises";
                    EntryFilterShortDescription += "the ADI+ falls";
                    ExitFilterLongDescription   += "the ADI+ rises";
                    ExitFilterShortDescription  += "the ADI+ falls";
                    break;

                case "The ADI+ falls":
                    EntryFilterLongDescription  += "the ADI+ falls";
                    EntryFilterShortDescription += "the ADI+ rises";
                    ExitFilterLongDescription   += "the ADI+ falls";
                    ExitFilterShortDescription  += "the ADI+ rises";
                    break;

                case "The ADI- rises":
                    EntryFilterLongDescription  += "the ADI- rises";
                    EntryFilterShortDescription += "the ADI- falls";
                    ExitFilterLongDescription   += "the ADI- rises";
                    ExitFilterShortDescription  += "the ADI- falls";
                    break;

                case "The ADI- falls":
                    EntryFilterLongDescription  += "the ADI- falls";
                    EntryFilterShortDescription += "the ADI- rises";
                    ExitFilterLongDescription   += "the ADI- falls";
                    ExitFilterShortDescription  += "the ADI- rises";
                    break;

                case "The ADI+ is higher than ADI-":
                    EntryFilterLongDescription  += "the ADI+ is higher than the ADI-";
                    EntryFilterShortDescription += "the ADI+ is lower than the ADI-";
                    ExitFilterLongDescription   += "the ADI+ is higher than the ADI-";
                    ExitFilterShortDescription  += "the ADI+ is lower than the ADI-";
                    break;

                case "The ADI+ is lower than ADI-":
                    EntryFilterLongDescription  += "the ADI+ is lower than the ADI-";
                    EntryFilterShortDescription += "the ADI+ is higher than the ADI-";
                    ExitFilterLongDescription   += "the ADI+ is lower than the ADI-";
                    ExitFilterShortDescription  += "the ADI+ is higher than the ADI-";
                    break;

                case "The ADI+ crosses the ADI- line upward":
                    EntryFilterLongDescription  += "the ADI+ crosses the ADI- line upward";
                    EntryFilterShortDescription += "the ADI+ crosses the ADI- line downward";
                    ExitFilterLongDescription   += "the ADI+ crosses the ADI- line upward";
                    ExitFilterShortDescription  += "the ADI+ crosses the ADI- line downward";
                    break;

                case "The ADI+ crosses the ADI- line downward":
                    EntryFilterLongDescription  += "the ADI+ crosses the ADI- line downward";
                    EntryFilterShortDescription += "the ADI+ crosses the ADI- line upward";
                    ExitFilterLongDescription   += "the ADI+ crosses the ADI- line downward";
                    ExitFilterShortDescription  += "the ADI+ crosses the ADI- line upward";
                    break;

                case "The ADI+ changes its direction upward":
                    EntryFilterLongDescription  += "the ADI+ changes its direction upward";
                    EntryFilterShortDescription += "the ADI+ changes its direction downward";
                    ExitFilterLongDescription   += "the ADI+ changes its direction upward";
                    ExitFilterShortDescription  += "the ADI+ changes its direction downward";
                    break;

                case "The ADI+ changes its direction downward":
                    EntryFilterLongDescription  += "the ADI+ changes its direction downward";
                    EntryFilterShortDescription += "the ADI+ changes its direction upward";
                    ExitFilterLongDescription   += "the ADI+ changes its direction downward";
                    ExitFilterShortDescription  += "the ADI+ changes its direction upward";
                    break;

                case "The ADI- changes its direction upward":
                    EntryFilterLongDescription  += "the ADI- changes its direction upward";
                    EntryFilterShortDescription += "the ADI- changes its direction downward";
                    ExitFilterLongDescription   += "the ADI- changes its direction upward";
                    ExitFilterShortDescription  += "the ADI- changes its direction downward";
                    break;

                case "The ADI- changes its direction downward":
                    EntryFilterLongDescription  += "the ADI- changes its direction downward";
                    EntryFilterShortDescription += "the ADI- changes its direction upward";
                    ExitFilterLongDescription   += "the ADI- changes its direction downward";
                    ExitFilterShortDescription  += "the ADI- changes its direction upward";
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
                IndParam.ListParam[2].Text         + ", " + // Base price
                IndParam.NumParam[0].ValueToString + ")";   // Period

            return sString;
        }
	}
}