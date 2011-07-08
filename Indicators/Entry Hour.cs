// Entry Hour Indicator
// Last changed on 2009-05-05
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2009 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Entry Hour Indicator
    /// </summary>
    public class Entry_Hour : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Entry_Hour(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "Entry Hour";
            PossibleSlots = SlotTypes.Open;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;
            IndParam.IndicatorType = TypeOfIndicator.DateTime;
            IndParam.ExecutionTime = ExecutionTime.AtBarOpening;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption = "Logic";
            IndParam.ListParam[0].ItemList = new string[]
            {
                "Enter the market at the specified hour"
            };
            IndParam.ListParam[0].Index   = 0;
            IndParam.ListParam[0].Text    = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Indicator's logic.";

            IndParam.ListParam[1].Caption  = "Base price";
            IndParam.ListParam[1].ItemList = new string[] { "Open" };
            IndParam.ListParam[1].Index    = 0;
            IndParam.ListParam[1].Text     = IndParam.ListParam[1].ItemList[IndParam.ListParam[1].Index];
            IndParam.ListParam[1].Enabled  = true;
            IndParam.ListParam[1].ToolTip  = "The execution price of all entry orders.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "Entry hour";
            IndParam.NumParam[0].Value   = 0;
            IndParam.NumParam[0].Min     = 0;
            IndParam.NumParam[0].Max     = 23;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "The position's opening hour.";

            // The NumericUpDown parameters
            IndParam.NumParam[1].Caption = "Entry minutes";
            IndParam.NumParam[1].Value   = 0;
            IndParam.NumParam[1].Min     = 0;
            IndParam.NumParam[1].Max     = 59;
            IndParam.NumParam[1].Enabled = true;
            IndParam.NumParam[1].ToolTip = "The position's opening minute.";

            return;
        }

        /// <summary>
        /// Calculates the indicator's components
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            // Reading the parameters
            int iEntryHour   = (int)IndParam.NumParam[0].Value;
            int iEntryMinute = (int)IndParam.NumParam[1].Value;
            TimeSpan tsEntryHour = new TimeSpan(iEntryHour, iEntryMinute, 0);

            // Calculation
            int iFirstBar = 1;
            double[] adBars = new double[Bars];

            // Calculation of the logic
            for (int iBar = iFirstBar; iBar < Bars; iBar++)
            {
                adBars[iBar] = Time[iBar].TimeOfDay == tsEntryHour ? Open[iBar] : 0;
            }

            // Saving the components
            Component = new IndicatorComp[1];

            Component[0] = new IndicatorComp();
            Component[0].CompName      = "Entry hour";
            Component[0].DataType      = IndComponentType.OpenPrice;
            Component[0].ChartType     = IndChartType.NoChart;
            Component[0].ShowInDynInfo = false;
            Component[0].FirstBar      = iFirstBar;
            Component[0].Value         = adBars;

            return;
        }

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {

            int iEntryHour   = (int)IndParam.NumParam[0].Value;
            int iEntryMinute = (int)IndParam.NumParam[1].Value;
            string sEntryTime = iEntryHour.ToString("00") + ":" + iEntryMinute.ToString("00");

            EntryPointLongDescription  = "at the beginning of the first bar after " + sEntryTime + " hours";
            EntryPointShortDescription = "at the beginning of the first bar after " + sEntryTime + " hours";

            return;
        }

        /// <summary>
        /// Indicator to string
        /// </summary>
        public override string ToString()
        {
            int iEntryHour   = (int)IndParam.NumParam[0].Value;
            int iEntryMinute = (int)IndParam.NumParam[1].Value;

            string sEntryTime = iEntryHour.ToString("00") + ":" + iEntryMinute.ToString("00");

            string sString = IndicatorName + " (" +
                sEntryTime + ")";  // Entry Hour

            return sString;
        }
    }
}
