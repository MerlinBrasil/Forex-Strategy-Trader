// Trix Index Indicator
// Last changed on 2009-05-05
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2009 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;

namespace Forex_Strategy_Trader
{
    /// <summary>
    ///  Trix Index Indicator
    /// </summary>
    public class Trix_Index : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Trix_Index(SlotTypes slotType)
        {
            // General properties
            IndicatorName  = "Trix Index";
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
                "The Trix Index line rises",
                "The Trix Index line falls",
                "The Trix Index line is higher than zero",
                "The Trix Index line is lower than zero",
                "The Trix Index line crosses the zero line upward",
                "The Trix Index line crosses the zero line downward",
                "The Trix Index line changes its direction upward",
                "The Trix Index line changes its direction downward",
                "The Trix Index line crosses the Signal line upward",
                "The Trix Index line crosses the Signal line downward",
                "The Trix Index line is higher than the Signal line",
                "The Trix Index line is lower than the Signal line"
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
            IndParam.ListParam[1].ToolTip  = "The Moving Average method used for smoothing the Trix Index value.";

            IndParam.ListParam[2].Caption  = "Base price";
            IndParam.ListParam[2].ItemList = Enum.GetNames(typeof(BasePrice));
            IndParam.ListParam[2].Index    = (int)BasePrice.Close;
            IndParam.ListParam[2].Text     = IndParam.ListParam[2].ItemList[IndParam.ListParam[2].Index];
            IndParam.ListParam[2].Enabled  = true;
            IndParam.ListParam[2].ToolTip  = "The price the indicator is based on.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "Period of Trix";
            IndParam.NumParam[0].Value   = 9;
            IndParam.NumParam[0].Min     = 1;
            IndParam.NumParam[0].Max     = 200;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "The period of Trix Moving Averages.";

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
            BasePrice basePrice = (BasePrice)IndParam.ListParam[2].Index;
            int       nPeriod   = (int)IndParam.NumParam[0].Value;
            int       iPrvs     = IndParam.CheckParam[0].Checked ? 1 : 0;

            // Calculation
            int iFirstBar = 2 * nPeriod + 2;

            double[] ma1 = MovingAverage(nPeriod, 0, maMethod, Price(basePrice));
            double[] ma2 = MovingAverage(nPeriod, 0, maMethod, ma1);
            double[] ma3 = MovingAverage(nPeriod, 0, maMethod, ma2);

            double[] adTrix = new double[Bars];

            for (int iBar = iFirstBar; iBar < Bars; iBar++)
                adTrix[iBar] = 100 * (ma3[iBar] - ma3[iBar - 1]) / ma3[iBar - 1];

            double[] adSignal = MovingAverage(nPeriod, 0, maMethod, adTrix);

            // adHistogram reprezents the Trix Index oscillator
            double[] adHistogram = new double[Bars];
            for (int iBar = iFirstBar; iBar < Bars; iBar++)
                adHistogram[iBar] = adTrix[iBar] - adSignal[iBar];

            // Saving the components
            Component = new IndicatorComp[5];

            Component[0] = new IndicatorComp();
            Component[0].CompName   = "Histogram";
            Component[0].DataType   = IndComponentType.IndicatorValue;
            Component[0].ChartType  = IndChartType.Histogram;
            Component[0].FirstBar   = iFirstBar;
            Component[0].Value      = adHistogram;

            Component[1] = new IndicatorComp();
            Component[1].CompName   = "Signal";
            Component[1].DataType   = IndComponentType.IndicatorValue;
            Component[1].ChartType  = IndChartType.Line;
            Component[1].ChartColor = Color.Gold;
            Component[1].FirstBar   = iFirstBar;
            Component[1].Value      = adSignal;

            Component[2] = new IndicatorComp();
            Component[2].CompName   = "Trix Line";
            Component[2].DataType   = IndComponentType.IndicatorValue;
            Component[2].ChartType  = IndChartType.Line;
            Component[2].ChartColor = Color.Blue;
            Component[2].FirstBar   = iFirstBar;
            Component[2].Value      = adTrix;

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

            switch (IndParam.ListParam[0].Text)
            {
                case "The Trix Index line rises":
                    OscillatorLogic(iFirstBar, iPrvs, adTrix, 0, 0, ref Component[3], ref Component[4], IndicatorLogic.The_indicator_rises);
                    break;

                case "The Trix Index line falls":
                    OscillatorLogic(iFirstBar, iPrvs, adTrix, 0, 0, ref Component[3], ref Component[4], IndicatorLogic.The_indicator_falls);
                    break;

                case "The Trix Index line is higher than zero":
                    OscillatorLogic(iFirstBar, iPrvs, adTrix, 0, 0, ref Component[3], ref Component[4], IndicatorLogic.The_indicator_is_higher_than_the_level_line);
                    break;

                case "The Trix Index line is lower than zero":
                    OscillatorLogic(iFirstBar, iPrvs, adTrix, 0, 0, ref Component[3], ref Component[4], IndicatorLogic.The_indicator_is_lower_than_the_level_line);
                    break;

                case "The Trix Index line crosses the zero line upward":
                    OscillatorLogic(iFirstBar, iPrvs, adTrix, 0, 0, ref Component[3], ref Component[4], IndicatorLogic.The_indicator_crosses_the_level_line_upward);
                    break;

                case "The Trix Index line crosses the zero line downward":
                    OscillatorLogic(iFirstBar, iPrvs, adTrix, 0, 0, ref Component[3], ref Component[4], IndicatorLogic.The_indicator_crosses_the_level_line_downward);
                    break;

                case "The Trix Index line changes its direction upward":
                    OscillatorLogic(iFirstBar, iPrvs, adTrix, 0, 0, ref Component[3], ref Component[4], IndicatorLogic.The_indicator_changes_its_direction_upward);
                    break;

                case "The Trix Index line changes its direction downward":
                    OscillatorLogic(iFirstBar, iPrvs, adTrix, 0, 0, ref Component[3], ref Component[4], IndicatorLogic.The_indicator_changes_its_direction_downward);
                    break;

                case "The Trix Index line crosses the Signal line upward":
                    OscillatorLogic(iFirstBar, iPrvs, adHistogram, 0, 0, ref Component[3], ref Component[4], IndicatorLogic.The_indicator_crosses_the_level_line_upward);
                    break;

                case "The Trix Index line crosses the Signal line downward":
                    OscillatorLogic(iFirstBar, iPrvs, adHistogram, 0, 0, ref Component[3], ref Component[4], IndicatorLogic.The_indicator_crosses_the_level_line_downward);
                    break;

                case "The Trix Index line is higher than the Signal line":
                    OscillatorLogic(iFirstBar, iPrvs, adHistogram, 0, 0, ref Component[3], ref Component[4], IndicatorLogic.The_indicator_is_higher_than_the_level_line);
                    break;

                case "The Trix Index line is lower than the Signal line":
                    OscillatorLogic(iFirstBar, iPrvs, adHistogram, 0, 0, ref Component[3], ref Component[4], IndicatorLogic.The_indicator_is_lower_than_the_level_line);
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
            EntryFilterLongDescription  = "the " + ToString() + " ";
            EntryFilterShortDescription = "the " + ToString() + " ";
            ExitFilterLongDescription   = "the " + ToString() + " ";
            ExitFilterShortDescription  = "the " + ToString() + " ";

            switch (IndParam.ListParam[0].Text)
            {
                case "The Trix Index line rises":
                    EntryFilterLongDescription  += "rises";
                    EntryFilterShortDescription += "falls";
                    ExitFilterLongDescription   += "rises";
                    ExitFilterShortDescription  += "falls";
                    break;

                case "The Trix Index line falls":
                    EntryFilterLongDescription  += "falls";
                    EntryFilterShortDescription += "rises";
                    ExitFilterLongDescription   += "falls";
                    ExitFilterShortDescription  += "rises";
                    break;

                case "The Trix Index line is higher than zero":
                    EntryFilterLongDescription  += "is higher than the zero line";
                    EntryFilterShortDescription += "is lower than the zero line";
                    ExitFilterLongDescription   += "is higher than the zero line";
                    ExitFilterShortDescription  += "is lower than the zero line";
                    break;

                case "The Trix Index line is lower than zero":
                    EntryFilterLongDescription  += "is lower than the zero line";
                    EntryFilterShortDescription += "is higher than the zero line";
                    ExitFilterLongDescription   += "is lower than the zero line";
                    ExitFilterShortDescription  += "is higher than the zero line";
                    break;

                case "The Trix Index line crosses the zero line upward":
                    EntryFilterLongDescription  += "crosses the zero line upward";
                    EntryFilterShortDescription += "crosses the zero line downward";
                    ExitFilterLongDescription   += "crosses the zero line upward";
                    ExitFilterShortDescription  += "crosses the zero line downward";
                    break;

                case "The Trix Index line crosses the zero line downward":
                    EntryFilterLongDescription  += "crosses the zero line downward";
                    EntryFilterShortDescription += "crosses the zero line upward";
                    ExitFilterLongDescription   += "crosses the zero line downward";
                    ExitFilterShortDescription  += "crosses the zero line upward";
                    break;

                case "The Trix Index line changes its direction upward":
                    EntryFilterLongDescription  += "changes its direction upward";
                    EntryFilterShortDescription += "changes its direction downward";
                    ExitFilterLongDescription   += "changes its direction upward";
                    ExitFilterShortDescription  += "changes its direction downward";
                    break;

                case "The Trix Index line changes its direction downward":
                    EntryFilterLongDescription  += "changes its direction downward";
                    EntryFilterShortDescription += "changes its direction upward";
                    ExitFilterLongDescription   += "changes its direction downward";
                    ExitFilterShortDescription  += "changes its direction upward";
                    break;

                case "The Trix Index line crosses the Signal line upward":
                    EntryFilterLongDescription  += "crosses the Signal line upward";
                    EntryFilterShortDescription += "crosses the Signal line downward";
                    ExitFilterLongDescription   += "crosses the Signal line upward";
                    ExitFilterShortDescription  += "crosses the Signal line downward";
                    break;

                case "The Trix Index line crosses the Signal line downward":
                    EntryFilterLongDescription  += "crosses the Signal line downward";
                    EntryFilterShortDescription += "crosses the Signal line upward";
                    ExitFilterLongDescription   += "crosses the Signal line downward";
                    ExitFilterShortDescription  += "crosses the Signal line upward";
                    break;

                case "The Trix Index line is higher than the Signal line":
                    EntryFilterLongDescription  += "is higher than the Signal line";
                    EntryFilterShortDescription += "is lower than the Signal line";
                    ExitFilterLongDescription   += "is higher than the Signal line";
                    ExitFilterShortDescription  += "is lower than the Signal line";
                    break;

                case "The Trix Index line is lower than the Signal line":
                    EntryFilterLongDescription  += "is lower than the Signal line";
                    EntryFilterShortDescription += "is higher than the Signal line";
                    ExitFilterLongDescription   += "is lower than the Signal line";
                    ExitFilterShortDescription  += "is higher than the Signal line";
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
                IndParam.ListParam[2].Text         + ", " + // Base price
                IndParam.NumParam[0].ValueToString + ")";   // Period of Trix

            return sString;
        }
    }
}
