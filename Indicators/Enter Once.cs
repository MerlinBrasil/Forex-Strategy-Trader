// Enter Once Indicator
// Last changed on 2009-05-05
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2009 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Enter Once Indicator
    /// </summary>
    public class Enter_Once : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Enter_Once(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "Enter Once";
            PossibleSlots = SlotTypes.OpenFilter;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;
            IndParam.IndicatorType = TypeOfIndicator.DateTime;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption = "Logic";
            IndParam.ListParam[0].ItemList = new string[]
            {
                "Enter no more than once a bar",
                "Enter no more than once a day",
                "Enter no more than once a week",
                "Enter no more than once a month"
            };
            IndParam.ListParam[0].Index   = 0;
            IndParam.ListParam[0].Text    = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Indicator's logic.";

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
            if (IndParam.ListParam[0].Text == "Enter no more than once a bar")
            {
                EntryFilterLongDescription  = "this is the first entry during the bar";
                EntryFilterShortDescription = "this is the first entry during the bar";
            }
            else if (IndParam.ListParam[0].Text == "Enter no more than once a day")
            {
                EntryFilterLongDescription  = "this is the first entry during the day";
                EntryFilterShortDescription = "this is the first entry during the day";
            }
            else if (IndParam.ListParam[0].Text == "Enter no more than once a week")
            {
                EntryFilterLongDescription  = "this is the first entry during the week";
                EntryFilterShortDescription = "this is the first entry during the week";
            }
            else if (IndParam.ListParam[0].Text == "Enter no more than once a month")
            {
                EntryFilterLongDescription  = "this is the first entry during the month";
                EntryFilterShortDescription = "this is the first entry during the month";
            }

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
