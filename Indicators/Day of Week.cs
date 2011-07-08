// Day of Week Indicator
// Last changed on 2009-05-05
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2009 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Day of Week Indicator
    /// </summary>
    public class Day_of_Week : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Day_of_Week(SlotTypes slotType)
        {
            // General properties
            PossibleSlots = SlotTypes.OpenFilter;
            IndicatorName = "Day of Week";

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;
            IndParam.IndicatorType = TypeOfIndicator.DateTime;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption = "Logic";
            IndParam.ListParam[0].ItemList = new string[]
            {
                "Enter the market between the specified days"
            };
            IndParam.ListParam[0].Index   = 0;
            IndParam.ListParam[0].Text    = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Indicators' logic.";

            IndParam.ListParam[1].Caption  = "From (incl.)";
            IndParam.ListParam[1].ItemList = Enum.GetNames(typeof(DayOfWeek));
            IndParam.ListParam[1].Index    = (int)DayOfWeek.Monday;
            IndParam.ListParam[1].Text     = IndParam.ListParam[1].ItemList[IndParam.ListParam[1].Index];
            IndParam.ListParam[1].Enabled  = true;
            IndParam.ListParam[1].ToolTip  = "Day of beginning for the entry period.";

            IndParam.ListParam[2].Caption  = "To (excl.)";
            IndParam.ListParam[2].ItemList = Enum.GetNames(typeof(DayOfWeek));
            IndParam.ListParam[2].Index    = (int)DayOfWeek.Saturday;
            IndParam.ListParam[2].Text     = IndParam.ListParam[2].ItemList[IndParam.ListParam[2].Index];
            IndParam.ListParam[2].Enabled  = true;
            IndParam.ListParam[2].ToolTip  = "End day for the entry period.";

            return;
        }

        /// <summary>
        /// Calculates the indicator's components
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            // Reading the parameters
            DayOfWeek dowFromDay  = (DayOfWeek)IndParam.ListParam[1].Index;
            DayOfWeek dowUntilDay = (DayOfWeek)IndParam.ListParam[2].Index;

            // Calculation
            int iFirstBar = 1;
            double[] adBars = new double[Bars];

            // Calculation of the logic
            for (int iBar = iFirstBar; iBar < Bars; iBar++)
            {
                if (dowFromDay < dowUntilDay)
                    adBars[iBar] = Time[iBar].DayOfWeek >= dowFromDay &&
                                   Time[iBar].DayOfWeek <  dowUntilDay ? 1 : 0;
                else if (dowFromDay > dowUntilDay)
                    adBars[iBar] = Time[iBar].DayOfWeek >= dowFromDay ||
                                   Time[iBar].DayOfWeek <  dowUntilDay ? 1 : 0;
                else
                    adBars[iBar] = 1;
            }

            // Saving the components
            Component = new IndicatorComp[2];

            Component[0] = new IndicatorComp();
            Component[0].CompName      = "Allow long entry";
            Component[0].DataType      = IndComponentType.AllowOpenLong;
            Component[0].ChartType     = IndChartType.NoChart;
            Component[0].ShowInDynInfo = false;
            Component[0].FirstBar      = iFirstBar;
            Component[0].Value         = adBars;

            Component[1] = new IndicatorComp();
            Component[1].CompName      = "Allow short entry";
            Component[1].DataType      = IndComponentType.AllowOpenShort;
            Component[1].ChartType     = IndChartType.NoChart;
            Component[1].ShowInDynInfo = false;
            Component[1].FirstBar      = iFirstBar;
            Component[1].Value         = adBars;

            return;
        }

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {
            DayOfWeek dowFromDay  = (DayOfWeek)IndParam.ListParam[1].Index;
            DayOfWeek dowUntilDay = (DayOfWeek)IndParam.ListParam[2].Index;

            EntryFilterLongDescription  = "the day of week is from " + dowFromDay + " (incl.) to " + dowUntilDay + " (excl.)";
            EntryFilterShortDescription = "the day of week is from " + dowFromDay + " (incl.) to " + dowUntilDay + " (excl.)";

            return;
        }

        /// <summary>
        /// Indicator to string
        /// </summary>
        public override string ToString()
        {
            DayOfWeek dowFromDay  = (DayOfWeek)IndParam.ListParam[1].Index;
            DayOfWeek dowUntilDay = (DayOfWeek)IndParam.ListParam[2].Index;

            string sString = IndicatorName + " (" +
                dowFromDay  + ", " + // From
                dowUntilDay + ")";   // Until

            return sString;
        }
    }
}
