// Balance of Power Indicator
// Last changed on 2009-04-15
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2009 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Balance of Power Indicator
    /// </summary>
    public class Balance_of_Power : Indicator
	{
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Balance_of_Power(SlotTypes slotType)
		{
            // General properties
            IndicatorName  = "Balance of Power";
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
                "The Balance of Power rises",
                "The Balance of Power falls",
                "The Balance of Power is higher than the zero line",
                "The Balance of Power is lower than the zero line",
                "The Balance of Power crosses the zero line upward",
                "The Balance of Power crosses the zero line downward",
                "The Balance of Power changes its direction upward",
                "The Balance of Power changes its direction downward"
            };
            IndParam.ListParam[0].Index   = 0;
            IndParam.ListParam[0].Text    = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Logic of application of the indicator.";

            IndParam.ListParam[1].Caption  = "Smoothing method";
            IndParam.ListParam[1].ItemList = Enum.GetNames(typeof(MAMethod));
            IndParam.ListParam[1].Index    = (int)MAMethod.Simple;
            IndParam.ListParam[1].Text     = IndParam.ListParam[1].ItemList[IndParam.ListParam[1].Index];
            IndParam.ListParam[1].Enabled  = true;
            IndParam.ListParam[1].ToolTip  = "The method of smoothing.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "Period";
            IndParam.NumParam[0].Value   = 14;
            IndParam.NumParam[0].Min     = 1;
            IndParam.NumParam[0].Max     = 100;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "The smoothing period.";

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
            int      iPeriod  = (int)IndParam.NumParam[0].Value;
            int      iPrvs    = IndParam.CheckParam[0].Checked ? 1 : 0;

            // Calculation
            int iFirstBar = iPeriod + 2;

			double[] adBOP = new double[Bars];

            for (int iBar = 1; iBar < Bars; iBar++)
            {
                if (High[iBar] - Low[iBar] > Point)
                    adBOP[iBar] = (Close[iBar] - Open[iBar]) / (High[iBar] - Low[iBar]);
                else
                    adBOP[iBar] = 0;
            }
            
            adBOP = MovingAverage(iPeriod, 0, maMethod, adBOP);

            // Saving the components
            Component = new IndicatorComp[3];

            Component[0] = new IndicatorComp();
            Component[0].CompName  = "Balance of Power";
            Component[0].DataType  = IndComponentType.IndicatorValue;
            Component[0].ChartType = IndChartType.Histogram;
            Component[0].FirstBar  = iFirstBar;
            Component[0].Value     = adBOP;

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
                case "The Balance of Power rises":
                    indLogic = IndicatorLogic.The_indicator_rises;
                    break;

                case "The Balance of Power falls":
                    indLogic = IndicatorLogic.The_indicator_falls;
                    break;

                case "The Balance of Power is higher than the zero line":
                    indLogic = IndicatorLogic.The_indicator_is_higher_than_the_level_line;
                    break;

                case "The Balance of Power is lower than the zero line":
                    indLogic = IndicatorLogic.The_indicator_is_lower_than_the_level_line;
                    break;

                case "The Balance of Power crosses the zero line upward":
                    indLogic = IndicatorLogic.The_indicator_crosses_the_level_line_upward;
                    break;

                case "The Balance of Power crosses the zero line downward":
                    indLogic = IndicatorLogic.The_indicator_crosses_the_level_line_downward;
                    break;

                case "The Balance of Power changes its direction upward":
                    indLogic = IndicatorLogic.The_indicator_changes_its_direction_upward;
                    break;

                case "The Balance of Power changes its direction downward":
                    indLogic = IndicatorLogic.The_indicator_changes_its_direction_downward;
                    break;

                default:
                    break;
            }

            OscillatorLogic(iFirstBar, iPrvs, adBOP, 0, 0, ref Component[1], ref Component[2], indLogic);

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
                case "The Balance of Power rises":
                    EntryFilterLongDescription  += "rises";
                    EntryFilterShortDescription += "falls";
                    ExitFilterLongDescription   += "rises";
                    ExitFilterShortDescription  += "falls";
                    break;

                case "The Balance of Power falls":
                    EntryFilterLongDescription  += "falls";
                    EntryFilterShortDescription += "rises";
                    ExitFilterLongDescription   += "falls";
                    ExitFilterShortDescription  += "rises";
                    break;

                case "The Balance of Power is higher than the zero line":
                    EntryFilterLongDescription  += "is higher than the zero line";
                    EntryFilterShortDescription += "is lower than the zero line";
                    ExitFilterLongDescription   += "is higher than the zero line";
                    ExitFilterShortDescription  += "is lower than the zero line";
                    break;

                case "The Balance of Power is lower than the zero line":
                    EntryFilterLongDescription  += "is lower than the zero line";
                    EntryFilterShortDescription += "is higher than the zero line";
                    ExitFilterLongDescription   += "is lower than the zero line";
                    ExitFilterShortDescription  += "is higher than the zero line";
                    break;

                case "The Balance of Power crosses the zero line upward":
                    EntryFilterLongDescription  += "crosses the zero line upward";
                    EntryFilterShortDescription += "crosses the zero line downward";
                    ExitFilterLongDescription   += "crosses the zero line upward";
                    ExitFilterShortDescription  += "crosses the zero line downward";
                    break;

                case "The Balance of Power crosses the zero line downward":
                    EntryFilterLongDescription  += "crosses the zero line downward";
                    EntryFilterShortDescription += "crosses the zero line upward";
                    ExitFilterLongDescription   += "crosses the zero line downward";
                    ExitFilterShortDescription  += "crosses the zero line upward";
                    break;

                case "The Balance of Power changes its direction upward":
                    EntryFilterLongDescription  += "changes its direction upward";
                    EntryFilterShortDescription += "changes its direction downward";
                    ExitFilterLongDescription   += "changes its direction upward";
                    ExitFilterShortDescription  += "changes its direction downward";
                    break;

                case "The Balance of Power changes its direction downward":
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
                IndParam.NumParam[0].ValueToString + ")"  ; // Period

            return sString;
        }
	}
}