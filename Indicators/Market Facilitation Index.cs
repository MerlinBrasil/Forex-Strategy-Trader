// Market Facilitation Index Indicator
// Last changed on 2009-05-05
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// This code or any part of it cannot be used in other applications without a permission.
// Copyright (c) 2006 - 2009 Miroslav Popov - All rights reserved.

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Market Facilitation Index Indicator
    /// </summary>
    public class Market_Facilitation_Index : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Market_Facilitation_Index(SlotTypes slotType)
        {
            // General properties
            IndicatorName  = "Market Facilitation Index";
            PossibleSlots  = SlotTypes.OpenFilter | SlotTypes.CloseFilter;
            SeparatedChart = true;
            SeparatedChartMinValue = 0;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption  = "Logic";
            IndParam.ListParam[0].ItemList = new string[]
            {
                "The Market Facilitation Index rises",
                "The Market Facilitation Index falls",
                "The Market Facilitation Index changes its direction upward",
                "The Market Facilitation Index changes its direction downward"
            };
            IndParam.ListParam[0].Index   = 0;
            IndParam.ListParam[0].Text    = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Logic of application of the indicator.";

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
            // Reading the parameters
            int iPrvs = IndParam.CheckParam[0].Checked ? 1 : 0;

            // Calculation
            double[] adMFI  = new double[Bars];

            int iFirstBar = 5;

            for (int iBar = 0; iBar < Bars; iBar++)
            {
                if (Volume[iBar] > 0)
                    adMFI[iBar] = 10000 * (High[iBar] - Low[iBar]) / Volume[iBar];
                else
                    adMFI[iBar] = 10000 * (High[iBar] - Low[iBar]);
            }

            // Saving the components
            Component = new IndicatorComp[3];

            Component[0] = new IndicatorComp();
            Component[0].CompName  = "Market Facilitation Index";
            Component[0].DataType  = IndComponentType.IndicatorValue;
            Component[0].ChartType = IndChartType.Histogram;
            Component[0].FirstBar  = iFirstBar;
            Component[0].Value     = adMFI;

            Component[1] = new IndicatorComp();
            Component[1].ChartType = IndChartType.NoChart;
            Component[1].FirstBar  = iFirstBar;
            Component[1].Value     = new double[Bars];

            Component[2] = new IndicatorComp();
            Component[2].ChartType = IndChartType.NoChart;
            Component[2].FirstBar  = iFirstBar;
            Component[2].Value     = new double[Bars];

            // Sets the Component's type
            if (slotType == SlotTypes.OpenFilter)
            {
                Component[1].DataType = IndComponentType.AllowOpenLong;
                Component[1].CompName = "Is long entry allowed";
                Component[2].DataType = IndComponentType.AllowOpenShort;
                Component[2].CompName = "Is short entry allowed";
            }
            else if (slotType == SlotTypes.CloseFilter)
            {
                Component[1].DataType = IndComponentType.ForceCloseLong;
                Component[1].CompName = "Close out long position";
                Component[2].DataType = IndComponentType.ForceCloseShort;
                Component[2].CompName = "Close out short position";
            }

            // Calculation of the logic
            IndicatorLogic indLogic = IndicatorLogic.It_does_not_act_as_a_filter;

            switch (IndParam.ListParam[0].Text)
            {
                case "The Market Facilitation Index rises":
                    indLogic = IndicatorLogic.The_indicator_rises;
                    break;

                case "The Market Facilitation Index falls":
                    indLogic = IndicatorLogic.The_indicator_falls;
                    break;

                case "The Market Facilitation Index changes its direction upward":
                    indLogic = IndicatorLogic.The_indicator_changes_its_direction_upward;
                    break;

                case "The Market Facilitation Index changes its direction downward":
                    indLogic = IndicatorLogic.The_indicator_changes_its_direction_downward;
                    break;

                default:
                    break;
            }

            NoDirectionOscillatorLogic(iFirstBar, iPrvs, adMFI, 0, ref Component[1], indLogic);
            Component[2].Value = Component[1].Value;

            return;
        }

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {
            EntryFilterLongDescription  = "the " + ToString() + " ";
            EntryFilterShortDescription = "the " + ToString() + " ";
            ExitFilterLongDescription   = "the " + ToString() + " ";
            ExitFilterShortDescription  = "the " + ToString() + " ";

            switch (IndParam.ListParam[0].Text)
            {
                case "The Market Facilitation Index rises":
                    EntryFilterLongDescription  += "rises";
                    EntryFilterShortDescription += "rises";
                    ExitFilterLongDescription   += "rises";
                    ExitFilterShortDescription  += "rises";
                    break;

                case "The Market Facilitation Index falls":
                    EntryFilterLongDescription  += "falls";
                    EntryFilterShortDescription += "falls";
                    ExitFilterLongDescription   += "falls";
                    ExitFilterShortDescription  += "falls";
                    break;

                case "The Market Facilitation Index changes its direction upward":
                    EntryFilterLongDescription  += "changes its direction upward";
                    EntryFilterShortDescription += "changes its direction upward";
                    ExitFilterLongDescription   += "changes its direction upward";
                    ExitFilterShortDescription  += "changes its direction upward";
                    break;

                case "The Market Facilitation Index changes its direction downward":
                    EntryFilterLongDescription  += "changes its direction downward";
                    EntryFilterShortDescription += "changes its direction downward";
                    ExitFilterLongDescription   += "changes its direction downward";
                    ExitFilterShortDescription  += "changes its direction downward";
                    break;

                default:
                    break;
            }

            return;
        }

        /// <summary>
        /// Indicator to string
        /// </summary>
        public override string ToString()
        {
            string sString = IndicatorName + (IndParam.CheckParam[0].Checked ? "*" : "");

            return sString;
        }
    }
}
 