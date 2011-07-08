// Trailing Stop Limit Indicator
// Last changed on 2010-07-24
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2010 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Trailing Stop Limit Indicator
    /// The implimentation of logic is in Market.AnalyseClose(int iBar)
    /// </summary>
    public class Trailing_Stop_Limit : Indicator
	{
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Trailing_Stop_Limit(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "Trailing Stop Limit";
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
                "Exit at the trailing Stop Loss or at the constant Take Profit level",
            };
            IndParam.ListParam[0].Index   = 0;
            IndParam.ListParam[0].Text    = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Logic of application of the indicator.";

            IndParam.ListParam[1].Caption  = "Trailing mode";
            IndParam.ListParam[1].ItemList = new string[]
            {
                "Trails once a bar",
                "Trails at a new top/bottom",
            };
            IndParam.ListParam[1].Index   = 0;
            IndParam.ListParam[1].Text    = IndParam.ListParam[1].ItemList[IndParam.ListParam[1].Index];
            IndParam.ListParam[1].Enabled = true;
            IndParam.ListParam[1].ToolTip = "Mode of operation of Trailing Stop.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "Initial Stop Loss";
            IndParam.NumParam[0].Value   = 200;
            IndParam.NumParam[0].Min     = 5;
            IndParam.NumParam[0].Max     = 5000;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "The initial Stop Loss value (in pips).";

            IndParam.NumParam[1].Caption = "Take Profit";
            IndParam.NumParam[1].Value   = 200;
            IndParam.NumParam[1].Min     = 5;
            IndParam.NumParam[1].Max     = 5000;
            IndParam.NumParam[1].Enabled = true;
            IndParam.NumParam[1].ToolTip = "The constant Take Profit value (in pips).";

            return;
        }

        /// <summary>
        /// Calculates the indicator's components
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            // Saving the components
            Component = new IndicatorComp[1];

			Component[0]			   = new IndicatorComp();
            Component[0].CompName      = "Trailing Stop for a transferred position";
			Component[0].DataType	   = IndComponentType.Other;
            Component[0].ShowInDynInfo = false;
			Component[0].FirstBar	   = 1;
			Component[0].Value	       = new double[Bars];

            return;
		}

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {
            int iStopLoss   = (int)IndParam.NumParam[0].Value;
            int iTakeProfit = (int)IndParam.NumParam[1].Value;

            ExitPointLongDescription  = "at the Trailing Stop level or at the constant Take Profit level. Initial Stop Loss: " +
                iStopLoss + " pips; Take Profit: " + iTakeProfit + " pips";
            ExitPointShortDescription = "at the Trailing Stop level or at the constant Take Profit level. Initial Stop Loss: " +
                iStopLoss + " pips; Take Profit: " + iTakeProfit + " pips";

            return;
        }

        /// <summary>
        /// Indicator to string
        /// </summary>
        public override string ToString()
        {
            string sString = IndicatorName + " (" +
                IndParam.NumParam[0].ValueToString + ", " + // Stop Loss
                IndParam.NumParam[1].ValueToString + ")";   // Take Profit

            return sString;
        }
	}
}