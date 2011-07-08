// Lot Limiter Indicator
// Last changed on 2009-05-05
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2009 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Lot Limiter Indicator
    /// </summary>
    public class Lot_Limiter : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Lot_Limiter(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "Lot Limiter";
            PossibleSlots = SlotTypes.OpenFilter;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;
            IndParam.IndicatorType = TypeOfIndicator.Additional;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption  = "Logic";
            IndParam.ListParam[0].ItemList = new string[] { "Limit the number of open lots" };
            IndParam.ListParam[0].Index    = 0;
            IndParam.ListParam[0].Text     = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled  = true;
            IndParam.ListParam[0].ToolTip  = "Indicator's logic";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "Maximum lots";
            IndParam.NumParam[0].Value   = 5;
            IndParam.NumParam[0].Min     = 1;
            IndParam.NumParam[0].Max     = 100;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "Maximum number of open lots.";

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
            int iMaxLots = (int)IndParam.NumParam[0].Value;

            EntryFilterLongDescription  = "the open lots cannot be more than " + iMaxLots + ". This rule overrides the maximum number of open lots set in the strategy properties dialog";
            EntryFilterShortDescription = "the open lots cannot be more than " + iMaxLots + ". This rule overrides the maximum number of open lots set in the strategy properties dialog";

            return;
        }

        /// <summary>
        /// Indicator to string
        /// </summary>
        public override string ToString()
        {
            string sString = IndicatorName + " (" +
                IndParam.NumParam[0].ValueToString + ")"; // Maximum lots

            return sString;
        }
    }
}
