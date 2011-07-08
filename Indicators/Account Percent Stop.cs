// Account Percent Stop Indicator
// Last changed on 2010-04-04
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2010 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Account Percent Stop Indicator
    /// </summary>
    public class Account_Percent_Stop : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type.
        /// </summary>
        public Account_Percent_Stop(SlotTypes slotType)
        {
            // General properties
            IndicatorName   = "Account Percent Stop";
            PossibleSlots   = SlotTypes.Close;
            SeparatedChart  = false;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;
            IndParam.IndicatorType = TypeOfIndicator.Additional;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption  = "Logic";
            IndParam.ListParam[0].ItemList = new string[]
            {
                "Limit the risk to percent of the account"
            };
            IndParam.ListParam[0].Index   = 0;
            IndParam.ListParam[0].Text    = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Logic of application of the indicator.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "Account percent";
            IndParam.NumParam[0].Value   = 2;
            IndParam.NumParam[0].Min     = 1;
            IndParam.NumParam[0].Max     = 20;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "Maximum account to risk.";

            return;
        }

        /// <summary>
        /// Calculates the indicator's components.
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            // Reading the parameters
            int percent = (int)IndParam.NumParam[0].Value;

            // Calculation
            int firstBar = 1;
           
            // Saving the components
            Component = new IndicatorComp[1];

            Component[0] = new IndicatorComp();
            Component[0].CompName      = "Stop to a transferred position";
			Component[0].DataType	   = IndComponentType.Other;
            Component[0].ShowInDynInfo = false;
			Component[0].FirstBar	   = firstBar;
			Component[0].Value	       = new double[Bars];

            return;
        }

        /// <summary>
        /// Sets the indicator logic description.
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {
            int percent = (int)IndParam.NumParam[0].Value;

            ExitPointLongDescription  = "at a loss of " + percent + "% of the account";
            ExitPointShortDescription = "at a loss of " + percent + "% of the account";

            return;
        }

        /// <summary>
        /// Indicator to string
        /// </summary>
        public override string ToString()
        {
            string sString = IndicatorName + " (" + IndParam.NumParam[0].ValueToString + ")";

            return sString;
        }
    }
}