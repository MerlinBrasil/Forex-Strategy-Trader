// Inside Bar Indicator
// Last changed on 2009-05-05
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2009 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Inside Bar Indicator
    /// </summary>
    public class Inside_Bar : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Inside_Bar(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "Inside Bar";
            PossibleSlots = SlotTypes.OpenFilter;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;
            IndParam.IndicatorType = TypeOfIndicator.Additional;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption  = "Logic";
            IndParam.ListParam[0].ItemList = new string[]
            {
                "There is an Inside Bar formation",
            };
            IndParam.ListParam[0].Index   = 0;
            IndParam.ListParam[0].Text    = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Indicator's logic.";

            // The CheckBox parameters
            IndParam.CheckParam[0].Caption = "Use previous bar value";
            IndParam.CheckParam[0].Checked = PrepareUsePrevBarValueCheckBox(slotType);
            IndParam.CheckParam[0].Enabled = true;
            IndParam.CheckParam[0].ToolTip = "Use the indicator value from the previous bar.";

            return;
        }

        /// <summary>
        /// Calculates the indicator's components
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            // Calculation
            int iFirstBar = 2;
            double[] adIB = new double[Bars];

            for (int iBar = 2; iBar < Bars; iBar++)
            {
                adIB[iBar] = ((High[iBar - 1] < High[iBar - 2]) && (Low[iBar - 1] > Low[iBar - 2])) ? 1 : 0;
            }

            // Saving the components
            Component = new IndicatorComp[2];

            Component[0] = new IndicatorComp();
            Component[0].CompName  = "Allow long entry";
            Component[0].DataType  = IndComponentType.AllowOpenLong;
            Component[0].ChartType = IndChartType.NoChart;
            Component[0].FirstBar  = iFirstBar;
            Component[0].Value     = adIB;

            Component[1] = new IndicatorComp();
            Component[1].CompName  = "Allow short entry";
            Component[1].DataType  = IndComponentType.AllowOpenShort;
            Component[1].ChartType = IndChartType.NoChart;
            Component[1].FirstBar  = iFirstBar;
            Component[1].Value     = adIB;

            return;
        }

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {
            EntryFilterLongDescription  = "there is an Inside Bar formation";
            EntryFilterShortDescription = "there is an Inside Bar formation";

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
