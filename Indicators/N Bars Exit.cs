// N Bars Exit Indicator
// Contributed by Krog
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2011 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// N Bars Stop Indicator
    /// Exit N Bars after entry
    /// The implementation of logic is in Market.AnalyseClose(int iBar)
    /// </summary>
    public class N_Bars_Exit : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public N_Bars_Exit(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "N Bars Exit";
            PossibleSlots = SlotTypes.CloseFilter;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;
            IndParam.IndicatorType = TypeOfIndicator.Additional;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption  = "Logic";
            IndParam.ListParam[0].ItemList = new string[]
            {
                "Exit N Bars after entry",
            };
            IndParam.ListParam[0].Index   = 0;
            IndParam.ListParam[0].Text    = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Logic of application of the indicator.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "N Bars";
            IndParam.NumParam[0].Value   = 10;
            IndParam.NumParam[0].Min     = 1;
            IndParam.NumParam[0].Max     = 10000;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "The number of bars after entry to exit the position.";

            return;
        }

        /// <summary>
        /// Calculates the indicator's components
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            // FST sends the N bars for exit to the expert. Expert watches the position and closes it.

            return;
        }

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {
            int iNExit = (int)IndParam.NumParam[0].Value;

            ExitFilterLongDescription  = iNExit.ToString() + " bars passed after the entry";
            ExitFilterShortDescription = iNExit.ToString() + " bars passed after the entry";

            return;
        }

        /// <summary>
        /// Indicator to string
        /// </summary>
        public override string ToString()
        {
            string sString = IndicatorName + " (" +
                IndParam.NumParam[0].ValueToString + ")"; // Number of Bars

            return sString;
        }
    }
}
