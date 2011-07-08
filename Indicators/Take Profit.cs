// Take Profit Indicator
// Last changed on 2010-04-04
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2010 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Take Profit Indicator
    /// The implimentation of logic is in Backtester.AnalyseClose(int iBar)
    /// </summary>
    public class Take_Profit : Indicator
	{
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Take_Profit(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "Take Profit";
            PossibleSlots = SlotTypes.Close;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;
            IndParam.IndicatorType = TypeOfIndicator.Additional;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption  = "Logic";
            IndParam.ListParam[0].ItemList = new string[]
            {
                "Exit at the Take Profit level",
            };
            IndParam.ListParam[0].Index   = 0;
            IndParam.ListParam[0].Text    = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Logic of application of the indicator.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "Take Profit";
            IndParam.NumParam[0].Value   = 200;
            IndParam.NumParam[0].Min     = 5;
            IndParam.NumParam[0].Max     = 5000;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "The Take Profit value (in pips).";

            return;
		}

        /// <summary>
        /// Calculates the indicator's components
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            return;
		}

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {
            int iTakeProfit = (int)IndParam.NumParam[0].Value;

            ExitPointLongDescription  = "when the market rises " + iTakeProfit + " pips from the last entry price";
            ExitPointShortDescription = "when the market falls " + iTakeProfit + " pips from the last entry price";

            return;
        }

        /// <summary>
        /// Indicator to string
        /// </summary>
        public override string ToString()
        {
            string sString = IndicatorName + " (" +
                IndParam.NumParam[0].ValueToString + ")";  // Take Profit

            return sString;
        }
	}
}