// Exit Hour Indicator
// Last changed on 2009-09-20
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2009 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Exit Hour Indicator
    /// </summary>
    public class Exit_Hour : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Exit_Hour(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "Exit Hour";
            PossibleSlots = SlotTypes.Close;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;
            IndParam.ExecutionTime = ExecutionTime.AtBarClosing;
            IndParam.IndicatorType = TypeOfIndicator.DateTime;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption  = "Logic";
            IndParam.ListParam[0].ItemList = new string[]
            {
                "Exit the market before the specified hour"
            };
            IndParam.ListParam[0].Index   = 0;
            IndParam.ListParam[0].Text    = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Indicator's logic.";

            IndParam.ListParam[1].Caption  = "Base price";
            IndParam.ListParam[1].ItemList = new string[] { "Close" };
            IndParam.ListParam[1].Index    = 0;
            IndParam.ListParam[1].Text     = IndParam.ListParam[1].ItemList[IndParam.ListParam[1].Index];
            IndParam.ListParam[1].Enabled  = true;
            IndParam.ListParam[1].ToolTip  = "Exit price of the position.";

            // The NumericUpDown parameters.
            IndParam.NumParam[0].Caption = "Exit hour";
            IndParam.NumParam[0].Value   = 0;
            IndParam.NumParam[0].Min     = 0;
            IndParam.NumParam[0].Max     = 24;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "The position's closing hour.";

            return;
        }

        /// <summary>
        /// Calculates the indicator's components
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            // Reading the parameters
            int iExitHour = (int)IndParam.NumParam[0].Value;
            TimeSpan tsExitHour = new TimeSpan(iExitHour, 0, 0);

            // Calculation
            int iFirstBar = 1;
            double[] adBars = new double[Bars];

            // Calculation of the logic
            for (int iBar = iFirstBar; iBar < Bars; iBar++)
            {
                if (Time[iBar - 1].DayOfYear == Time[iBar].DayOfYear &&
                    Time[iBar - 1].TimeOfDay < tsExitHour &&
                    Time[iBar].TimeOfDay >= tsExitHour)
                    adBars[iBar - 1] = Close[iBar - 1];
                else if (Time[iBar - 1].DayOfYear != Time[iBar].DayOfYear &&
                    Time[iBar - 1].TimeOfDay < tsExitHour )
                    adBars[iBar - 1] = Close[iBar - 1];
                else
                    adBars[iBar] = 0;
            }
            
            // Check the last bar
            if(Time[Bars - 1].TimeOfDay.Add(new TimeSpan(0, (int)Period, 0)) == tsExitHour)
                adBars[Bars - 1] = Close[Bars - 1];

            // Saving the components
            Component = new IndicatorComp[1];

            Component[0] = new IndicatorComp();
            Component[0].CompName      = "Exit hour";
            Component[0].DataType      = IndComponentType.ClosePrice;
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
            ExitPointLongDescription  = "at the end of the last bar before " + IndParam.NumParam[0].Value + " o'clock";
            ExitPointShortDescription = "at the end of the last bar before " + IndParam.NumParam[0].Value + " o'clock";

            return;
        }

        /// <summary>
        /// Indicator to string
        /// </summary>
        public override string ToString()
        {
            string sString = IndicatorName + " (" +
                IndParam.NumParam[0].ValueToString + ")";  // Exit Hour

            return sString;
        }
    }
}
