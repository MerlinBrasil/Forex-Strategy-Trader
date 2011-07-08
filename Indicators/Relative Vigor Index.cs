// Relative Vigor Index Indicator
// Last changed on 2009-05-05
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// This code or any part of it cannot be used in other applications without a permission.
// Copyright (c) 2006 - 2009 Miroslav Popov - All rights reserved.

using System;
using System.Drawing;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Relative Vigor Index Indicator
    /// </summary>
    public class Relative_Vigor_Index : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Relative_Vigor_Index(SlotTypes slotType)
        {
            // General properties
            IndicatorName  = "Relative Vigor Index";
            PossibleSlots  = SlotTypes.OpenFilter | SlotTypes.CloseFilter;
            SeparatedChart = true;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption  = "Logic";
            IndParam.ListParam[0].ItemList = new string[]
            {
                "The RVI line rises",
                "The RVI line falls",
                "The RVI line is higher than zero",
                "The RVI line is lower than zero",
                "The RVI line crosses the zero line upward",
                "The RVI line crosses the zero line downward",
                "The RVI line changes its direction upward",
                "The RVI line changes its direction downward",
                "The RVI line crosses the Signal line upward",
                "The RVI line crosses the Signal line downward",
                "The RVI line is higher than the Signal line",
                "The RVI line is lower than the Signal line"
            };
            IndParam.ListParam[0].Index    = 0;
            IndParam.ListParam[0].Text     = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled  = true;
            IndParam.ListParam[0].ToolTip  = "Logic of application of the indicator.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "RVI period";
            IndParam.NumParam[0].Value   = 10;
            IndParam.NumParam[0].Min     = 1;
            IndParam.NumParam[0].Max     = 200;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "The period of Slow MA.";

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
            int nRVI  = (int)IndParam.NumParam[0].Value;
            int iPrvs = IndParam.CheckParam[0].Checked ? 1 : 0;

            // Calculation
            int iFirstBar = nRVI + 4;

            double[] adRVI = new double[Bars];
            for (int iBar = nRVI + 3; iBar < Bars; iBar++)
            {
                double dNum   = 0;
                double dDeNum = 0;
                for (int j = iBar; j > iBar - nRVI; j--)
                {
                    double dValueUp   = ((Close[j] - Open[j]) + 2 * (Close[j - 1] - Open[j - 1]) + 2 * (Close[j - 2] - Open[j - 2]) + (Close[j - 3] - Open[j - 3])) / 6;
                    double dValueDown = ((High[j]  -  Low[j]) + 2 * (High[j - 1]  -  Low[j - 1]) + 2 * (High[j - 2]  -  Low[j - 2]) + (High[j - 3]  -  Low[j - 3])) / 6;
                    dNum   += dValueUp;
                    dDeNum += dValueDown;
                }
                if (dDeNum != 0)
                    adRVI[iBar] = dNum / dDeNum;
                else
                    adRVI[iBar] = dNum;
            }

            double[] adMASignal = new double[Bars];
            for (int iBar = 4; iBar < Bars; iBar++)
                adMASignal[iBar] = (adRVI[iBar] + 2 * adRVI[iBar - 1] + 2 * adRVI[iBar - 2] + adRVI[iBar - 3]) / 6;

            // Saving the components
            Component = new IndicatorComp[4];

            Component[0] = new IndicatorComp();
            Component[0].CompName   = "RVI Line";
            Component[0].DataType   = IndComponentType.IndicatorValue;
            Component[0].ChartType  = IndChartType.Line;
            Component[0].ChartColor = Color.Green;
            Component[0].FirstBar   = iFirstBar;
            Component[0].Value      = adRVI;

            Component[1] = new IndicatorComp();
            Component[1].CompName   = "Signal line";
            Component[1].DataType   = IndComponentType.IndicatorValue;
            Component[1].ChartType  = IndChartType.Line;
            Component[1].ChartColor = Color.Red;
            Component[1].FirstBar   = iFirstBar;
            Component[1].Value      = adMASignal;

            Component[2] = new IndicatorComp();
            Component[2].ChartType = IndChartType.NoChart;
            Component[2].FirstBar  = iFirstBar;
            Component[2].Value     = new double[Bars];

            Component[3] = new IndicatorComp();
            Component[3].ChartType = IndChartType.NoChart;
            Component[3].FirstBar  = iFirstBar;
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
                case "The RVI line rises":
                    OscillatorLogic(iFirstBar, iPrvs, adRVI, 0, 0, ref Component[2], ref Component[3], IndicatorLogic.The_indicator_rises);
                    break;

                case "The RVI line falls":
                    OscillatorLogic(iFirstBar, iPrvs, adRVI, 0, 0, ref Component[2], ref Component[3], IndicatorLogic.The_indicator_falls);
                    break;

                case "The RVI line is higher than zero":
                    OscillatorLogic(iFirstBar, iPrvs, adRVI, 0, 0, ref Component[2], ref Component[3], IndicatorLogic.The_indicator_is_higher_than_the_level_line);
                    break;

                case "The RVI line is lower than zero":
                    OscillatorLogic(iFirstBar, iPrvs, adRVI, 0, 0, ref Component[2], ref Component[3], IndicatorLogic.The_indicator_is_lower_than_the_level_line);
                    break;

                case "The RVI line crosses the zero line upward":
                    OscillatorLogic(iFirstBar, iPrvs, adRVI, 0, 0, ref Component[2], ref Component[3], IndicatorLogic.The_indicator_crosses_the_level_line_upward);
                    break;

                case "The RVI line crosses the zero line downward":
                    OscillatorLogic(iFirstBar, iPrvs, adRVI, 0, 0, ref Component[2], ref Component[3], IndicatorLogic.The_indicator_crosses_the_level_line_downward);
                    break;

                case "The RVI line changes its direction upward":
                    OscillatorLogic(iFirstBar, iPrvs, adRVI, 0, 0, ref Component[2], ref Component[3], IndicatorLogic.The_indicator_changes_its_direction_upward);
                    break;

                case "The RVI line changes its direction downward":
                    OscillatorLogic(iFirstBar, iPrvs, adRVI, 0, 0, ref Component[2], ref Component[3], IndicatorLogic.The_indicator_changes_its_direction_downward);
                    break;

                case "The RVI line crosses the Signal line upward":
                    IndicatorCrossesAnotherIndicatorUpwardLogic(iFirstBar, iPrvs, adRVI, adMASignal, ref Component[2], ref Component[3]);
                    break;

                case "The RVI line crosses the Signal line downward":
                    IndicatorCrossesAnotherIndicatorDownwardLogic(iFirstBar, iPrvs, adRVI, adMASignal, ref Component[2], ref Component[3]);
                    break;

                case "The RVI line is higher than the Signal line":
                    IndicatorIsHigherThanAnotherIndicatorLogic(iFirstBar, iPrvs, adRVI, adMASignal, ref Component[2], ref Component[3]);
                    break;

                case "The RVI line is lower than the Signal line":
                    IndicatorIsLowerThanAnotherIndicatorLogic(iFirstBar, iPrvs, adRVI, adMASignal, ref Component[2], ref Component[3]);
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
            EntryFilterLongDescription  = ToString() + "; the RVI line ";
            EntryFilterShortDescription = ToString() + "; the RVI line ";
            ExitFilterLongDescription   = ToString() + "; the RVI line ";
            ExitFilterShortDescription  = ToString() + "; the RVI line ";

            switch (IndParam.ListParam[0].Text)
            {
                case "The RVI line rises":
                    EntryFilterLongDescription  += "rises";
                    EntryFilterShortDescription += "falls";
                    ExitFilterLongDescription   += "rises";
                    ExitFilterShortDescription  += "falls";
                    break;

                case "The RVI line falls":
                    EntryFilterLongDescription  += "falls";
                    EntryFilterShortDescription += "rises";
                    ExitFilterLongDescription   += "falls";
                    ExitFilterShortDescription  += "rises";
                    break;

                case "The RVI line is higher than zero":
                    EntryFilterLongDescription  += "is higher than the zero line";
                    EntryFilterShortDescription += "is lower than the zero line";
                    ExitFilterLongDescription   += "is higher than the zero line";
                    ExitFilterShortDescription  += "is lower than the zero line";
                    break;

                case "The RVI line is lower than zero":
                    EntryFilterLongDescription  += "is lower than the zero line";
                    EntryFilterShortDescription += "is higher than the zero line";
                    ExitFilterLongDescription   += "is lower than the zero line";
                    ExitFilterShortDescription  += "is higher than the zero line";
                    break;

                case "The RVI line crosses the zero line upward":
                    EntryFilterLongDescription  += "crosses the zero line upward";
                    EntryFilterShortDescription += "crosses the zero line downward";
                    ExitFilterLongDescription   += "crosses the zero line upward";
                    ExitFilterShortDescription  += "crosses the zero line downward";
                    break;

                case "The RVI line crosses the zero line downward":
                    EntryFilterLongDescription  += "crosses the zero line downward";
                    EntryFilterShortDescription += "crosses the zero line upward";
                    ExitFilterLongDescription   += "crosses the zero line downward";
                    ExitFilterShortDescription  += "crosses the zero line upward";
                    break;

                case "The RVI line changes its direction upward":
                    EntryFilterLongDescription  += "changes its direction upward";
                    EntryFilterShortDescription += "changes its direction downward";
                    ExitFilterLongDescription   += "changes its direction upward";
                    ExitFilterShortDescription  += "changes its direction downward";
                    break;

                case "The RVI line changes its direction downward":
                    EntryFilterLongDescription  += "changes its direction downward";
                    EntryFilterShortDescription += "changes its direction upward";
                    ExitFilterLongDescription   += "changes its direction downward";
                    ExitFilterShortDescription  += "changes its direction upward";
                    break;

                case "The RVI line is higher than the Signal line":
                    EntryFilterLongDescription  += "is higher than the Signal line";
                    EntryFilterShortDescription += "is lower than the Signal line";
                    ExitFilterLongDescription   += "is higher than the Signal line";
                    ExitFilterShortDescription  += "is lower than the Signal line";
                    break;

                case "The RVI line is lower than the Signal line":
                    EntryFilterLongDescription  += "is lower than the Signal line";
                    EntryFilterShortDescription += "is higher than the Signal line";
                    ExitFilterLongDescription   += "is lower than the Signal line";
                    ExitFilterShortDescription  += "is higher than the Signal line";
                    break;

                case "The RVI line crosses the Signal line upward":
                    EntryFilterLongDescription  += "crosses the Signal line upward";
                    EntryFilterShortDescription += "crosses the Signal line downward";
                    ExitFilterLongDescription   += "crosses the Signal line upward";
                    ExitFilterShortDescription  += "crosses the Signal line downward";
                    break;

                case "The RVI line crosses the Signal line downward":
                    EntryFilterLongDescription  += "crosses the Signal line downward";
                    EntryFilterShortDescription += "crosses the Signal line upward";
                    ExitFilterLongDescription   += "crosses the Signal line downward";
                    ExitFilterShortDescription  += "crosses the Signal line upward";
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
                IndParam.NumParam[0].ValueToString + ")";  // RVI period

            return sString;
        }
    }
}
