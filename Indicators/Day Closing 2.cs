// Day Closing Indicator
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Day Closing Indicator
    /// </summary>
    public class Day_Closing_2 : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Day_Closing_2(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "Day Closing 2";
            PossibleSlots = SlotTypes.Close;
            AllowClosingFilters = true;
            WarningMessage = "The indicator sends a close signal at first tick after the selected time." + Environment.NewLine +
                "It continues sending close signals to all positions till the end of the day." + Environment.NewLine +
                "The indicator uses the server time that comes from the broker together with ticks.";

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;
            IndParam.IndicatorType = TypeOfIndicator.DateTime;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption  = "Logic";
            IndParam.ListParam[0].ItemList = new string[] { "Exit the market at the end of the day" };
            IndParam.ListParam[0].Index    = 0;
            IndParam.ListParam[0].Text     = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled  = true;
            IndParam.ListParam[1].ToolTip  = "The execution price of all exit orders.";

            IndParam.ListParam[1].Caption  = "Base price";
            IndParam.ListParam[1].ItemList = new string[] { "Close" };
            IndParam.ListParam[1].Index    = 0;
            IndParam.ListParam[1].Text     = IndParam.ListParam[1].ItemList[IndParam.ListParam[1].Index];
            IndParam.ListParam[1].Enabled  = true;
            IndParam.ListParam[1].ToolTip  = "Exit price of the position.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "Day close hour";
            IndParam.NumParam[0].Value   = 23;
            IndParam.NumParam[0].Min     = 0;
            IndParam.NumParam[0].Max     = 23;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "The hour we want to close at.";

            IndParam.NumParam[1].Caption = "Day close min";
            IndParam.NumParam[1].Value   = 59;
            IndParam.NumParam[1].Min     = 0;
            IndParam.NumParam[1].Max     = 59;
            IndParam.NumParam[1].Enabled = true;
            IndParam.NumParam[1].ToolTip = "The minutes of the closing hour";

            IndParam.NumParam[2].Caption = "Friday close hour";
            IndParam.NumParam[2].Value   = 19;
            IndParam.NumParam[2].Min     = 0;
            IndParam.NumParam[2].Max     = 23;
            IndParam.NumParam[2].Enabled = true;
            IndParam.NumParam[2].ToolTip = "The hour we want to close at on Friday.";

            IndParam.NumParam[3].Caption = "Friday close min";
            IndParam.NumParam[3].Value   = 59;
            IndParam.NumParam[3].Min     = 0;
            IndParam.NumParam[3].Max     = 59;
            IndParam.NumParam[3].Enabled = true;
            IndParam.NumParam[3].ToolTip = "The minutes of the closing hour";

            return;
        }

        /// <summary>
        /// Calculates the indicator's components
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            // Reading the parameters
            int dayClosingHour    = (int)IndParam.NumParam[0].Value;
            int dayClosingMin     = (int)IndParam.NumParam[1].Value;
            int fridayClosingHour = (int)IndParam.NumParam[2].Value;
            int fridayClosingMin  = (int)IndParam.NumParam[3].Value;

            // Calculation
            DateTime time = ServerTime;
            DateTime closingTime = new DateTime(time.Year, time.Month, time.Day, dayClosingHour, dayClosingMin, 0);
            DateTime fridayTime  = new DateTime(time.Year, time.Month, time.Day, fridayClosingHour, fridayClosingMin, 0);

            double[] adClosePrice = new double[Bars];

            for (int bar = 1; bar < Bars; bar++)
                if (Time[bar - 1].Day != Time[bar].Day)
                    adClosePrice[bar - 1] = Close[bar - 1];
                    
            if (time.DayOfWeek != DayOfWeek.Friday)
            {   // Not Friday
                if (time >= closingTime)
                { 
                    adClosePrice[Bars - 1] = Close[Bars - 1];
                }
            }
            else
            {   // Friday
                if (time >= fridayTime)
                {
                    adClosePrice[Bars - 1] = Close[Bars - 1];
                }
            }

            // Saving the components
            Component = new IndicatorComp[1];

            Component[0]           = new IndicatorComp();
            Component[0].CompName  = "Closing price of the day";
            Component[0].DataType  = IndComponentType.ClosePrice;
            Component[0].ChartType = IndChartType.NoChart;
            Component[0].FirstBar  = 2;
            Component[0].Value     = adClosePrice;

            return;
        }

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {
            ExitPointLongDescription   = "at the end of the day";
            ExitPointShortDescription  = "at the end of the day";

            return;
        }

        /// <summary>
        /// Indicator to string
        /// </summary>
        public override string ToString()
        {
            return IndicatorName;
        }
    }
}