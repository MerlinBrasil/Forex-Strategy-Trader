// Entry Time indicator
// Part of Forex Strategy Builder & Forex Strategy Trader
// Copyright (c) 2006 - 2009 Miroslav Popov - All rights reserved!
// Last changed on: 2009-04-15
// http://forexsb.com

using System;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Entry Time indicator
    /// </summary>
    public class Entry_Time : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Entry_Time(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "Entry Time";
            PossibleSlots = SlotTypes.OpenFilter;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;
            IndParam.IndicatorType = TypeOfIndicator.DateTime;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption = "Logic";
            IndParam.ListParam[0].ItemList = new string[]
            {
                "Enter the market between the specified hours"
            };
            IndParam.ListParam[0].Index   = 0;
            IndParam.ListParam[0].Text    = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Indicator's logic.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "From hour (incl.)";
            IndParam.NumParam[0].Value   = 0;
            IndParam.NumParam[0].Min     = 0;
            IndParam.NumParam[0].Max     = 23;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "Beginning of the entry period.";

            IndParam.NumParam[1].Caption = "From min (incl.)";
            IndParam.NumParam[1].Value   = 0;
            IndParam.NumParam[1].Min     = 0;
            IndParam.NumParam[1].Max     = 59;
            IndParam.NumParam[1].Enabled = true;
            IndParam.NumParam[1].ToolTip = "Beginning of the entry period.";

            IndParam.NumParam[2].Caption = "Until hour (excl.)";
            IndParam.NumParam[2].Value   = 24;
            IndParam.NumParam[2].Min     = 0;
            IndParam.NumParam[2].Max     = 24;
            IndParam.NumParam[2].Enabled = true;
            IndParam.NumParam[2].ToolTip = "End of the entry period.";

            IndParam.NumParam[3].Caption = "Until min( excl.)";
            IndParam.NumParam[3].Value   = 0;
            IndParam.NumParam[3].Min     = 0;
            IndParam.NumParam[3].Max     = 59;
            IndParam.NumParam[3].Enabled = true;
            IndParam.NumParam[3].ToolTip = "End of the entry period.";

            return;
        }

        /// <summary>
        /// Calculates the indicator's components
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            // Reading the parameters
            int iFromHour  = (int)IndParam.NumParam[0].Value;
            int iFromMin   = (int)IndParam.NumParam[1].Value;
            int iUntilHour = (int)IndParam.NumParam[2].Value;
            int iUntilMin  = (int)IndParam.NumParam[3].Value;
            TimeSpan tsFromTime  = new TimeSpan(iFromHour, iFromMin, 0);
            TimeSpan tsUntilTime = new TimeSpan(iUntilHour, iUntilMin, 0);

            // Calculation
            int iFirstBar = 1;
            double[] adBars = new double[Bars];

            // Calculation of the logic
            for (int iBar = iFirstBar; iBar < Bars; iBar++)
            {
                if(tsFromTime < tsUntilTime)
                    adBars[iBar] = Time[iBar].TimeOfDay >= tsFromTime &&
                                   Time[iBar].TimeOfDay <  tsUntilTime ? 1 : 0;
                else if(tsFromTime > tsUntilTime)
                    adBars[iBar] = Time[iBar].TimeOfDay >= tsFromTime ||
                                   Time[iBar].TimeOfDay <  tsUntilTime ? 1 : 0;
                else
                    adBars[iBar] = 1;
            }

            // Saving the components
            Component = new IndicatorComp[2];

            Component[0] = new IndicatorComp();
            Component[0].CompName      = "Is long entry allowed";
            Component[0].DataType      = IndComponentType.AllowOpenLong;
            Component[0].ChartType     = IndChartType.NoChart;
            Component[0].ShowInDynInfo = false;
            Component[0].FirstBar      = iFirstBar;
            Component[0].Value         = adBars;

            Component[1] = new IndicatorComp();
            Component[1].CompName      = "Is short entry allowed";
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
            int iFromHour  = (int)IndParam.NumParam[0].Value;
            int iFromMin   = (int)IndParam.NumParam[1].Value;
            int iUntilHour = (int)IndParam.NumParam[2].Value;
            int iUntilMin  = (int)IndParam.NumParam[3].Value;

            string sFromTime  = iFromHour.ToString("00")  + ":" + iFromMin.ToString("00");
            string sUntilTime = iUntilHour.ToString("00") + ":" + iUntilMin.ToString("00");

            EntryFilterLongDescription  = "the entry time is between " + sFromTime + " (incl.) and " + sUntilTime + " (excl.)";
            EntryFilterShortDescription = "the entry time is between " + sFromTime + " (incl.) and " + sUntilTime + " (excl.)";

            return;
        }

        /// <summary>
        /// Indicator to string
        /// </summary>
        public override string ToString()
        {
            int iFromHour  = (int)IndParam.NumParam[0].Value;
            int iFromMin   = (int)IndParam.NumParam[1].Value;
            int iUntilHour = (int)IndParam.NumParam[2].Value;
            int iUntilMin  = (int)IndParam.NumParam[3].Value;

            string sFromTime  = iFromHour.ToString("00")  + ":" + iFromMin.ToString("00");
            string sUntilTime = iUntilHour.ToString("00") + ":" + iUntilMin.ToString("00");

            string sString = IndicatorName + " (" +
                sFromTime + " - " + // From
                sUntilTime + ")";   // Until

            return sString;
        }
    }
}
