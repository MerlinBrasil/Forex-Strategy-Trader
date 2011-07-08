// Day of Month Indicator
// Last changed on 2009-05-05
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2009 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Day of Month Indicator
    /// </summary>
    public class Day_of_Month : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Day_of_Month(SlotTypes slotType)
        {
            // General properties
            IndicatorName   = "Day of Month";
            PossibleSlots   = SlotTypes.OpenFilter;
			CustomIndicator = true;

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
            IndParam.ListParam[0].ToolTip = "Logic of application of the indicator.";

            IndParam.NumParam[0].Caption  = "From (incl.)";
            IndParam.NumParam[0].Value    = 1;
            IndParam.NumParam[0].Min      = 1;
            IndParam.NumParam[0].Max      = 31;
            IndParam.NumParam[0].Point    = 0;
            IndParam.NumParam[0].Enabled  = true;
            IndParam.NumParam[0].ToolTip  = "Day of beginning for the entry period.";

            IndParam.NumParam[1].Caption  = "Until (excl.)";
            IndParam.NumParam[1].Value    = 31;
            IndParam.NumParam[1].Min      = 1;
            IndParam.NumParam[1].Max      = 31;
            IndParam.NumParam[1].Point    = 0;
            IndParam.NumParam[1].Enabled  = true;
            IndParam.NumParam[1].ToolTip  = "Day of ending for the entry period.";

            return;
        }

        /// <summary>
        /// Calculates the indicator's components
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            // Reading the parameters
            int iFromDay  = (int)IndParam.NumParam[0].Value;
            int iUntilDay = (int)IndParam.NumParam[1].Value;

            // Calculation
            int iFirstBar = 1;
            double[] adBars = new double[Bars];

            // Calculation of the logic
            for (int iBar = iFirstBar; iBar < Bars; iBar++)
            {
                if (iFromDay < iUntilDay)
                    adBars[iBar] = Time[iBar].Day >= iFromDay && Time[iBar].Day <  iUntilDay ? 1 : 0;
                else if (iFromDay > iUntilDay)
                    adBars[iBar] = Time[iBar].Day >= iFromDay || Time[iBar].Day <  iUntilDay ? 1 : 0;
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
            int iFromDay  = (int)IndParam.NumParam[0].Value;
            int iUntilDay = (int)IndParam.NumParam[1].Value;

            EntryFilterLongDescription  = "the day of month is from " + iFromDay + " (incl.) to " + iUntilDay + " (excl.)";
            EntryFilterShortDescription = "the day of month is from " + iFromDay + " (incl.) to " + iUntilDay + " (excl.)";

            return;
        }

        /// <summary>
        /// Indicator to string
        /// </summary>
        public override string ToString()
        {
            int iFromDay  = (int)IndParam.NumParam[0].Value;
            int iUntilDay = (int)IndParam.NumParam[1].Value;

            string sString = IndicatorName + " (" +
                iFromDay  + ", " + // From
                iUntilDay + ")";   // Until

            return sString;
        }
    }
}
