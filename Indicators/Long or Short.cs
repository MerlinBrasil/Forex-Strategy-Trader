// Long or Short Indicator
// Last changed on 2009-05-05
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2009 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Long or Short Indicator
    /// </summary>
    public class Long_or_Short : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Long_or_Short(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "Long or Short";
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
                "Open long positions only",
                "Open short positions only",
            };
            IndParam.ListParam[0].Index   = 0;
            IndParam.ListParam[0].Text    = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Logic of application of the indicator.";

            return;
        }

        /// <summary>
        /// Calculates the indicator's components
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            // Saving the components
            Component = new IndicatorComp[2];

            Component[0] = new IndicatorComp();
            Component[0].CompName  = "Is long entry allowed";
            Component[0].DataType  = IndComponentType.AllowOpenLong;
            Component[0].ChartType = IndChartType.NoChart;
            Component[0].FirstBar  = 0;
            Component[0].Value     = new double[Bars];

            Component[1] = new IndicatorComp();
            Component[1].CompName  = "Is short entry allowed";
            Component[1].DataType  = IndComponentType.AllowOpenShort;
            Component[1].ChartType = IndChartType.NoChart;
            Component[1].FirstBar  = 0;
            Component[1].Value     = new double[Bars];

            // Calculation of the logic
            switch (IndParam.ListParam[0].Text)
            {
                case "Open long positions only":
                    for (int i = 0; i < Bars; i++)
                    {
                        Component[0].Value[i] = 1;
                        Component[1].Value[i] = 0;
                    }
                    break;

                case "Open short positions only":
                    for (int i = 0; i < Bars; i++)
                    {
                        Component[0].Value[i] = 0;
                        Component[1].Value[i] = 1;
                    }
                    break;

                default:
                    break;
            }

            return;
        }

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {
            // Calculation of the logic
            switch (IndParam.ListParam[0].Text)
            {
                case "Open long positions only":
                    EntryFilterLongDescription  = "the Long or Short filter permits long opening";
                    EntryFilterShortDescription = "the Long or Short filter does not permit short opening";
                    break;

                case "Open short positions only":
                    EntryFilterLongDescription  = "the Long or Short filter does not permit long opening";
                    EntryFilterShortDescription = "the Long or Short filter permits short opening";
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
            string sString = IndicatorName;

            return sString;
        }
    }
}
