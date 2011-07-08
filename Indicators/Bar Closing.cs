// Bar Closing Indicator
// Last changed on 2010-08-01
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2010 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Bar Closing Indicator
    /// </summary>
    public class Bar_Closing : Indicator
	{
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Bar_Closing(SlotTypes slotType)
		{
            // General properties
            IndicatorName = "Bar Closing";
            PossibleSlots = SlotTypes.Open | SlotTypes.Close;
            AllowClosingFilters = true;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;
            IndParam.IndicatorType = TypeOfIndicator.Additional;
            IndParam.ExecutionTime = ExecutionTime.AtBarClosing;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption = "Logic";
            if (slotType == SlotTypes.Open)
            {
                IndParam.ListParam[0].ItemList = new string [] { "Enter the market at the end of the bar" };
                IndParam.ListParam[1].ToolTip  = "The execution price of all entry orders.";
            }
            else
            {
                IndParam.ListParam[0].ItemList = new string[] { "Exit the market at the end of the bar" };
                IndParam.ListParam[1].ToolTip  = "The execution price of all exit orders.";
            }
            IndParam.ListParam[0].Index   = 0;
            IndParam.ListParam[0].Text    = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Logic of application of the indicator.";

            IndParam.ListParam[1].Caption  = "Base price";
            IndParam.ListParam[1].ItemList = new string[] { "Close" };
            IndParam.ListParam[1].Index    = 0;
            IndParam.ListParam[1].Text     = IndParam.ListParam[1].ItemList[IndParam.ListParam[1].Index];
            IndParam.ListParam[1].Enabled  = true;

            return;
        }

        /// <summary>
        /// Calculates the indicator's components
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            // Saving the components
            Component = new IndicatorComp[1];

            Component[0] = new IndicatorComp();
            Component[0].CompName  = "Closing price of the bar";
            Component[0].DataType  = (IndParam.SlotType == SlotTypes.Open) ? IndComponentType.OpenPrice : IndComponentType.ClosePrice;
            Component[0].ChartType = IndChartType.NoChart;
            Component[0].FirstBar  = 2;
            Component[0].Value     = Close;

            return;
        }

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {
            EntryPointLongDescription  = "at the end of the bar";
            EntryPointShortDescription = "at the end of the bar";
            ExitPointLongDescription   = "at the end of the bar";
            ExitPointShortDescription  = "at the end of the bar";

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