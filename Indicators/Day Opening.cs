// Day Opening Indicator
// Last changed on 2009-05-05
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2009 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Day Opening Indicator
    /// </summary>
    public class Day_Opening : Indicator
	{
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Day_Opening(SlotTypes slotType)
		{
            // General properties
            IndicatorName  = "Day Opening";
            PossibleSlots  = SlotTypes.Open;
            SeparatedChart = false;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;
            IndParam.IndicatorType = TypeOfIndicator.DateTime;
            IndParam.ExecutionTime = ExecutionTime.AtBarOpening;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption  = "Logic";
            IndParam.ListParam[0].ItemList = new string[] { "Enter the market at the beginning of the day" };
            IndParam.ListParam[0].Index    = 0;
            IndParam.ListParam[0].Text     = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled  = true;
            IndParam.ListParam[0].ToolTip  = "Logic of application of the indicator.";

            IndParam.ListParam[1].Caption  = "Base price";
            IndParam.ListParam[1].ItemList = new string[] { "Open" };
            IndParam.ListParam[1].Index    = 0;
            IndParam.ListParam[1].Text     = IndParam.ListParam[1].ItemList[IndParam.ListParam[1].Index];
            IndParam.ListParam[1].Enabled  = true;
            IndParam.ListParam[1].ToolTip  = "The execution price of all entry orders.";

            return;
        }

        /// <summary>
        /// Calculates the indicator's components
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            // Calculation
            double[] adOpenPrice = new double[Bars];

            for (int iBar = 1; iBar < Bars; iBar++)
                if (Time[iBar - 1].Day != Time[iBar].Day)
                    adOpenPrice[iBar] = Open[iBar];

            // Saving the components
            Component = new IndicatorComp[1];

            Component[0]           = new IndicatorComp();
            Component[0].CompName  = "Opening price of the day";
            Component[0].DataType  = IndComponentType.OpenPrice;
            Component[0].ChartType = IndChartType.NoChart;
            Component[0].FirstBar  = 2;
            Component[0].Value     = adOpenPrice;

            return;
        }

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {
            EntryPointLongDescription  = "at the beginning of the day";
            EntryPointShortDescription = "at the beginning of the day";

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