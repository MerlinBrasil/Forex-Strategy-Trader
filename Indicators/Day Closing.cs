// Day Closing Indicator
// Last changed on 2010-08-01
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2010 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Day Closing Indicator
    /// </summary>
    public class Day_Closing : Indicator
	{
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Day_Closing(SlotTypes slotType)
		{
            // General properties
            IndicatorName = "Day Closing";
            PossibleSlots = SlotTypes.Close;
            AllowClosingFilters = true;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;
            IndParam.IndicatorType = TypeOfIndicator.DateTime;
            IndParam.ExecutionTime = ExecutionTime.AtBarClosing;

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

            return;
        }

        /// <summary>
        /// Calculates the indicator's components
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            // Calculation
            double[] adClosePrice = new double[Bars];

            for (int iBar = 1; iBar < Bars; iBar++)
                if (Time[iBar - 1].Day != Time[iBar].Day)
                    adClosePrice[iBar - 1] = Close[iBar - 1];
					
			// Check the last bar
            TimeSpan tsBarClosing = Time[Bars - 1].TimeOfDay.Add(new TimeSpan(0, (int)Period, 0));
            TimeSpan tsDayClosing = new TimeSpan(24, 0, 0);
            if (tsBarClosing == tsDayClosing)
                adClosePrice[Bars - 1] = Close[Bars - 1];

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
            string sString = IndicatorName;

            return sString;
        }
    }
}