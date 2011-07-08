// Oscillator of MACD Indicator
// Last changed on 2009-05-05
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// This code or any part of it cannot be used in other applications without a permission.
// Copyright (c) 2006 - 2009 Miroslav Popov - All rights reserved.

using System;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Oscillator of MACD Indicator
    /// </summary>
    public class Oscillator_of_MACD : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Oscillator_of_MACD(SlotTypes slotType)
        {
            // General properties
            IndicatorName  = "Oscillator of MACD";
            PossibleSlots  = SlotTypes.OpenFilter | SlotTypes.CloseFilter;
            SeparatedChart = true;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;
            IndParam.IndicatorType = TypeOfIndicator.OscillatorOfIndicators;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption  = "Logic";
            IndParam.ListParam[0].ItemList = new string[]
            {
                "The Oscillator of MACD rises",
                "The Oscillator of MACD falls",
                "The Oscillator of MACD is higher than the zero line",
                "The Oscillator of MACD is lower than the zero line",
                "The Oscillator of MACD crosses the zero line upward",
                "The Oscillator of MACD crosses the zero line downward",
                "The Oscillator of MACD changes its direction upward",
                "The Oscillator of MACD changes its direction downward"
            };
            IndParam.ListParam[0].Index   = 0;
            IndParam.ListParam[0].Text    = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Logic of application of the oscillator.";

            IndParam.ListParam[1].Caption  = "Smoothing method";
            IndParam.ListParam[1].ItemList = Enum.GetNames(typeof(MAMethod));
            IndParam.ListParam[1].Index    = (int)MAMethod.Exponential;
            IndParam.ListParam[1].Text     = IndParam.ListParam[1].ItemList[IndParam.ListParam[1].Index];
            IndParam.ListParam[1].Enabled  = true;
            IndParam.ListParam[1].ToolTip  = "The Moving Average method used for smoothing the MACD value.";

            IndParam.ListParam[2].Caption  = "Base price";
            IndParam.ListParam[2].ItemList = Enum.GetNames(typeof(BasePrice));
            IndParam.ListParam[2].Index    = (int)BasePrice.Close;
            IndParam.ListParam[2].Text     = IndParam.ListParam[2].ItemList[IndParam.ListParam[2].Index];
            IndParam.ListParam[2].Enabled  = true;
            IndParam.ListParam[2].ToolTip  = "The price the indicator is based on.";

            IndParam.ListParam[3].Caption  = "Signal line method";
            IndParam.ListParam[3].ItemList = Enum.GetNames(typeof(MAMethod));
            IndParam.ListParam[3].Index    = (int)MAMethod.Exponential;
            IndParam.ListParam[3].Text     = IndParam.ListParam[3].ItemList[IndParam.ListParam[3].Index];
            IndParam.ListParam[3].Enabled  = true;
            IndParam.ListParam[3].ToolTip  = "The smoothing method of the signal line.";

            IndParam.ListParam[4].Caption  = "What to compare";
            IndParam.ListParam[4].ItemList = new string[] { "Histogram 1 to Histogram 2", "Signal line 1 to Signal line 2", "MACD line 1 to MACD line 2" };
            IndParam.ListParam[4].Index    = 0;
            IndParam.ListParam[4].Text     = IndParam.ListParam[4].ItemList[IndParam.ListParam[4].Index];
            IndParam.ListParam[4].Enabled  = true;
            IndParam.ListParam[4].ToolTip  = "The smoothing method of the signal line.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "MACD1 slow MA";
            IndParam.NumParam[0].Value   = 26;
            IndParam.NumParam[0].Min     = 1;
            IndParam.NumParam[0].Max     = 200;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "The period of first MACD slow line.";

            IndParam.NumParam[1].Caption = "MACD2 slow MA";
            IndParam.NumParam[1].Value   = 32;
            IndParam.NumParam[1].Min     = 1;
            IndParam.NumParam[1].Max     = 200;
            IndParam.NumParam[1].Enabled = true;
            IndParam.NumParam[1].ToolTip = "The period of second MACD slow line.";

            IndParam.NumParam[2].Caption = "MACD1 fast MA";
            IndParam.NumParam[2].Value   = 12;
            IndParam.NumParam[2].Min     = 1;
            IndParam.NumParam[2].Max     = 200;
            IndParam.NumParam[2].Enabled = true;
            IndParam.NumParam[2].ToolTip = "The period of first MACD fast line.";

            IndParam.NumParam[3].Caption = "MACD2 fast MA";
            IndParam.NumParam[3].Value   = 21;
            IndParam.NumParam[3].Min     = 1;
            IndParam.NumParam[3].Max     = 200;
            IndParam.NumParam[3].Enabled = true;
            IndParam.NumParam[3].ToolTip = "The period of second MACD fast line.";

            IndParam.NumParam[4].Caption = "MACD1 Signal line";
            IndParam.NumParam[4].Value   = 9;
            IndParam.NumParam[4].Min     = 1;
            IndParam.NumParam[4].Max     = 200;
            IndParam.NumParam[4].Enabled = true;
            IndParam.NumParam[4].ToolTip = "The period of Signal line.";

            IndParam.NumParam[5].Caption = "MACD2 Signal line";
            IndParam.NumParam[5].Value   = 13;
            IndParam.NumParam[5].Min     = 1;
            IndParam.NumParam[5].Max     = 200;
            IndParam.NumParam[5].Enabled = true;
            IndParam.NumParam[5].ToolTip = "The period of Signal line.";

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
            MACD MACD1 = new MACD(slotType);
            MACD1.IndParam.ListParam[1].Index = IndParam.ListParam[1].Index;
            MACD1.IndParam.ListParam[2].Index = IndParam.ListParam[2].Index;
            MACD1.IndParam.ListParam[3].Index = IndParam.ListParam[3].Index;
            MACD1.IndParam.NumParam[0].Value  = IndParam.NumParam[0].Value;
            MACD1.IndParam.NumParam[1].Value  = IndParam.NumParam[2].Value;
            MACD1.IndParam.NumParam[2].Value  = IndParam.NumParam[4].Value;
            MACD1.IndParam.CheckParam[0].Checked = IndParam.CheckParam[0].Checked;
            MACD1.Calculate(slotType);

            MACD MACD2 = new MACD(slotType);
            MACD2.IndParam.ListParam[1].Index = IndParam.ListParam[1].Index;
            MACD2.IndParam.ListParam[2].Index = IndParam.ListParam[2].Index;
            MACD2.IndParam.ListParam[3].Index = IndParam.ListParam[3].Index;
            MACD2.IndParam.NumParam[0].Value  = IndParam.NumParam[1].Value;
            MACD2.IndParam.NumParam[1].Value  = IndParam.NumParam[3].Value;
            MACD2.IndParam.NumParam[2].Value  = IndParam.NumParam[5].Value;
            MACD2.IndParam.CheckParam[0].Checked = IndParam.CheckParam[0].Checked;
            MACD2.Calculate(slotType);

            // Calculation
            int iPrvs    = IndParam.CheckParam[0].Checked ? 1 : 0;
            int iPeriod1 = (int)IndParam.NumParam[0].Value;
            int iPeriod2 = (int)IndParam.NumParam[1].Value;
            int iFirstBar = iPeriod1 + iPeriod2 + 2;
            double[] adIndicator1 = new double[Bars];
            double[] adIndicator2 = new double[Bars];

            if (IndParam.ListParam[4].Index == 0)
            {
                adIndicator1 = MACD1.Component[0].Value;
                adIndicator2 = MACD2.Component[0].Value;
            }
            else if (IndParam.ListParam[4].Index == 1)
            {
                adIndicator1 = MACD1.Component[1].Value;
                adIndicator2 = MACD2.Component[1].Value;
            }
            else
            {
                adIndicator1 = MACD1.Component[2].Value;
                adIndicator2 = MACD2.Component[2].Value;
            }

            double[] adOscllator  = new double[Bars];
            for (int iBar = iFirstBar; iBar < Bars; iBar++)
                adOscllator[iBar] = adIndicator1[iBar] - adIndicator2[iBar];

            // Saving the components
            Component = new IndicatorComp[3];

            Component[0] = new IndicatorComp();
            Component[0].CompName  = "Oscillator";
            Component[0].DataType  = IndComponentType.IndicatorValue;
            Component[0].ChartType = IndChartType.Histogram;
            Component[0].FirstBar  = iFirstBar;
            Component[0].Value     = adOscllator;

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
                case "The Oscillator of MACD rises":
                    indLogic = IndicatorLogic.The_indicator_rises;
                    break;

                case "The Oscillator of MACD falls":
                    indLogic = IndicatorLogic.The_indicator_falls;
                    break;

                case "The Oscillator of MACD is higher than the zero line":
                    indLogic = IndicatorLogic.The_indicator_is_higher_than_the_level_line;
                    break;

                case "The Oscillator of MACD is lower than the zero line":
                    indLogic = IndicatorLogic.The_indicator_is_lower_than_the_level_line;
                    break;

                case "The Oscillator of MACD crosses the zero line upward":
                    indLogic = IndicatorLogic.The_indicator_crosses_the_level_line_upward;
                    break;

                case "The Oscillator of MACD crosses the zero line downward":
                    indLogic = IndicatorLogic.The_indicator_crosses_the_level_line_downward;
                    break;

                case "The Oscillator of MACD changes its direction upward":
                    indLogic = IndicatorLogic.The_indicator_changes_its_direction_upward;
                    break;

                case "The Oscillator of MACD changes its direction downward":
                    indLogic = IndicatorLogic.The_indicator_changes_its_direction_downward;
                    break;

                default:
                    break;
            }

            OscillatorLogic(iFirstBar, iPrvs, adOscllator, 0, 0, ref Component[1], ref Component[2], indLogic);

            return;
        }

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {
            EntryFilterLongDescription  = "the " + ToString() + " ";
            EntryFilterShortDescription = "the " + ToString() + " ";
            ExitFilterLongDescription   = "the " + ToString() + " ";
            ExitFilterShortDescription  = "the " + ToString() + " ";

            switch (IndParam.ListParam[0].Text)
            {
                case "The Oscillator of MACD rises":
                    EntryFilterLongDescription  += "rises";
                    EntryFilterShortDescription += "falls";
                    ExitFilterLongDescription   += "rises";
                    ExitFilterShortDescription  += "falls";
                    break;

                case "The Oscillator of MACD falls":
                    EntryFilterLongDescription  += "falls";
                    EntryFilterShortDescription += "rises";
                    ExitFilterLongDescription   += "falls";
                    ExitFilterShortDescription  += "rises";
                    break;

                case "The Oscillator of MACD is higher than the zero line":
                    EntryFilterLongDescription  += "is higher than the zero line";
                    EntryFilterShortDescription += "is lower than the zero line";
                    ExitFilterLongDescription   += "is higher than the zero line";
                    ExitFilterShortDescription  += "is lower than the zero line";
                    break;

                case "The Oscillator of MACD is lower than the zero line":
                    EntryFilterLongDescription  += "is lower than the zero line";
                    EntryFilterShortDescription += "is higher than the zero line";
                    ExitFilterLongDescription   += "is lower than the zero line";
                    ExitFilterShortDescription  += "is higher than the zero line";
                    break;

                case "The Oscillator of MACD crosses the zero line upward":
                    EntryFilterLongDescription  += "crosses the zero line upward";
                    EntryFilterShortDescription += "crosses the zero line downward";
                    ExitFilterLongDescription   += "crosses the zero line upward";
                    ExitFilterShortDescription  += "crosses the zero line downward";
                    break;

                case "The Oscillator of MACD crosses the zero line downward":
                    EntryFilterLongDescription  += "crosses the zero line downward";
                    EntryFilterShortDescription += "crosses the zero line upward";
                    ExitFilterLongDescription   += "crosses the zero line downward";
                    ExitFilterShortDescription  += "crosses the zero line upward";
                    break;

                case "The Oscillator of MACD changes its direction upward":
                    EntryFilterLongDescription  += "changes its direction upward";
                    EntryFilterShortDescription += "changes its direction downward";
                    ExitFilterLongDescription   += "changes its direction upward";
                    ExitFilterShortDescription  += "changes its direction downward";
                    break;

                case "The Oscillator of MACD changes its direction downward":
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
                IndParam.ListParam[4].Text         + ", " + // What to compare
                IndParam.ListParam[2].Text         + ", " + // Price
                IndParam.ListParam[1].Text         + ", " + // Method
                IndParam.ListParam[3].Text         + ", " + // Signal line method
                IndParam.NumParam[0].ValueToString + ", " + // Period
                IndParam.NumParam[2].ValueToString + ", " + // Period
                IndParam.NumParam[4].ValueToString + ", " + // Period
                IndParam.NumParam[1].ValueToString + ", " + // Period
                IndParam.NumParam[3].ValueToString + ", " + // Period
                IndParam.NumParam[5].ValueToString + ")";   // Period

            return sString;
        }
    }
}
