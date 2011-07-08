// Narrow Range Indicator
// Last changed on 2009-05-05
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2009 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Narrow Range Indicator
    /// </summary>
    public class Narrow_Range : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Narrow_Range(SlotTypes slotType)
        {
            // General properties
            IndicatorName  = "Narrow Range";
            PossibleSlots  = SlotTypes.OpenFilter;
            SeparatedChart = true;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;
            IndParam.IndicatorType = TypeOfIndicator.Additional;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption  = "Logic";
            IndParam.ListParam[0].ItemList = new string[]
            {
                "There is a NR4 formation",
                "There is a NR7 formation",
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
            int iPrvs = IndParam.CheckParam[0].Checked ? 1 : 0;

            // Calculation
            int iStepBack = (IndParam.ListParam[0].Text == "There is a NR4 formation" ? 3 : 6);
            int iFirstBar = iStepBack + iPrvs;
            double[] adNR    = new double[Bars];
            double[] adRange = new double[Bars];

            for (int iBar = 0; iBar < Bars; iBar++)
            {
                adRange[iBar] = High[iBar] - Low[iBar];
                adNR[iBar] = 0;
            }

            // Calculation of the logic
            for (int iBar = iFirstBar; iBar < Bars; iBar++)
            {
                bool bNarrowRange = true;
                for (int i = 1; i <= iStepBack; i++)
                    if (adRange[iBar - i - iPrvs] <= adRange[iBar - iPrvs])
                    {
                        bNarrowRange = false;
                        break;
                    }

                if (bNarrowRange) adNR[iBar] = 1;
            }

            // Saving the components
            Component = new IndicatorComp[3];

            Component[0] = new IndicatorComp();
            Component[0].CompName  = "Bar Range";
            Component[0].DataType  = IndComponentType.IndicatorValue;
            Component[0].ChartType = IndChartType.Histogram;
            Component[0].FirstBar  = iFirstBar;
            Component[0].Value     = new double[Bars];
            for (int i = 0; i < Bars; i++)
                Component[0].Value[i] = (double)Math.Round(adRange[i] / Point);

            Component[1] = new IndicatorComp();
            Component[1].CompName  = "Allow long entry";
            Component[1].DataType  = IndComponentType.AllowOpenLong;
            Component[1].ChartType = IndChartType.NoChart;
            Component[1].FirstBar  = iFirstBar;
            Component[1].Value     = adNR;

            Component[2] = new IndicatorComp();
            Component[2].CompName  = "Allow short entry";
            Component[2].DataType  = IndComponentType.AllowOpenShort;
            Component[2].ChartType = IndChartType.NoChart;
            Component[2].FirstBar  = iFirstBar;
            Component[2].Value     = adNR;

            return;
        }

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {
            string sFormation = (IndParam.ListParam[0].Text == "There is a NR4 formation" ? "NR4" : "NR7");

            EntryFilterLongDescription  = "there is a " + sFormation + " formation";
            EntryFilterShortDescription = "there is a " + sFormation + " formation";

            return;
        }

        /// <summary>
        /// Indicator to string
        /// </summary>
        public override string ToString()
        {
            string sFormation = (IndParam.ListParam[0].Text == "There is a NR4 formation" ? " NR4" : " NR7");
            string sString    = IndicatorName + sFormation;

            return sString;
        }
    }
}
