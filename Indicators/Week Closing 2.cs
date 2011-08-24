// Week Closing Indicator
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Week Closing Indicator
    /// </summary>
    public class Week_Closing_2 : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Week_Closing_2(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "Week Closing 2";
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
            IndParam.ListParam[0].ItemList = new string[] { "Exit the market at the end of the week" };
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
            IndParam.NumParam[0].Caption = "Friday close hour";
            IndParam.NumParam[0].Value   = 19;
            IndParam.NumParam[0].Min     = 0;
            IndParam.NumParam[0].Max     = 23;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "The hour we want to close at.";

            IndParam.NumParam[1].Caption = "Friday close min";
            IndParam.NumParam[1].Value   = 59;
            IndParam.NumParam[1].Min     = 0;
            IndParam.NumParam[1].Max     = 59;
            IndParam.NumParam[1].Enabled = true;
            IndParam.NumParam[1].ToolTip = "The minutes of the closing hour";

            return;
        }

        /// <summary>
        /// Calculates the indicator's components
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            int fridayClosingHour = (int)IndParam.NumParam[0].Value;
            int fridayClosingMin  = (int)IndParam.NumParam[1].Value;

            // Calculation
            DateTime time = ServerTime;
            DateTime fridayTime = new DateTime(time.Year, time.Month, time.Day, fridayClosingHour, fridayClosingMin, 0);

            int firstBar = 1;
            double[] adClosePrice = new double[Bars];

            // Calculation of the logic
            for (int bar = firstBar; bar < Bars - 1; bar++)
            {
                if (Time[bar].DayOfWeek > DayOfWeek.Wednesday &&
                    Time[bar + 1].DayOfWeek < DayOfWeek.Wednesday)
                    adClosePrice[bar] = Close[bar];
                else
                    adClosePrice[bar] = 0;
            }
            
            // Check the last bar
            if (time.DayOfWeek == DayOfWeek.Friday)
                if (time >= fridayTime)
                    adClosePrice[Bars - 1] = Close[Bars - 1];

            // Saving the components
            Component = new IndicatorComp[1];

            Component[0] = new IndicatorComp();
            Component[0].CompName      = "Week Closing";
            Component[0].DataType      = IndComponentType.ClosePrice;
            Component[0].ChartType     = IndChartType.NoChart;
            Component[0].ShowInDynInfo = false;
            Component[0].FirstBar      = firstBar;
            Component[0].Value         = adClosePrice;

            return;
        }

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {
            ExitPointLongDescription  = "at the end of the week";
            ExitPointShortDescription = "at the end of the week";

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
