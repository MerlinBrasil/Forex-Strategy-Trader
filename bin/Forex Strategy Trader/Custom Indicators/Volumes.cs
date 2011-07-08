// Volumes indicator
// Last changed on 2009-05-14
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// This code or any part of it cannot be used in other applications without a permission.
// Copyright (c) 2006 - 2009 Miroslav Popov - All rights reserved.

using System.Drawing;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Volumes indicator
    /// </summary>
    public class Volumes : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Volumes(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "Volumes";
            PossibleSlots = SlotTypes.OpenFilter | SlotTypes.CloseFilter;
            SeparatedChartMinValue = 0;
            SeparatedChart  = true;
            CustomIndicator = true;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption  = "Logic";
            IndParam.ListParam[0].ItemList = new string[]
            {
                "The Volume rises",
                "The Volume falls",
                "The Volume is higher than the Level line",
                "The Volume is lower than the Level line",
            };
            IndParam.ListParam[0].Index   = 0;
            IndParam.ListParam[0].Text    = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Logic of application of the indicator.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "Level";
            IndParam.NumParam[0].Value   = 1000;
            IndParam.NumParam[0].Min     = 0;
            IndParam.NumParam[0].Max     = 100000;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "A critical level (for the appropriate logic).";

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
            double dLevel = IndParam.NumParam[0].Value;
            int    iPrvs  = IndParam.CheckParam[0].Checked ? 1 : 0;

            // Calculation
            double[] adVolumes = new double[Bars];

            int iFirstBar = iPrvs + 1;

            for (int iBar = 0; iBar < Bars; iBar++)
            {
                adVolumes[iBar] = Volume[iBar];
            }

            // Saving the components
            Component = new IndicatorComp[3];

            Component[0] = new IndicatorComp();
            Component[0].CompName  = "Volumes";
            Component[0].DataType  = IndComponentType.IndicatorValue;
            Component[0].ChartType = IndChartType.Histogram;
            Component[0].FirstBar  = iFirstBar;
            Component[0].Value     = adVolumes;

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
            switch (IndParam.ListParam[0].Text)
            {
                case "The Volume rises":
                    for (int iBar = iPrvs + 1; iBar < Bars; iBar++)
                    {
                        Component[1].Value[iBar] = adVolumes[iBar - iPrvs] > adVolumes[iBar - iPrvs - 1] + Sigma() ? 1 : 0;
                        Component[2].Value[iBar] = adVolumes[iBar - iPrvs] > adVolumes[iBar - iPrvs - 1] + Sigma() ? 1 : 0;
                    }
                    break;

                case "The Volume falls":
                    for (int iBar = iPrvs + 1; iBar < Bars; iBar++)
                    {
                        Component[1].Value[iBar] = adVolumes[iBar - iPrvs] < adVolumes[iBar - iPrvs - 1] - Sigma() ? 1 : 0;
                        Component[2].Value[iBar] = adVolumes[iBar - iPrvs] < adVolumes[iBar - iPrvs - 1] - Sigma() ? 1 : 0;
                    }
                    break;

                case "The Volume is higher than the Level line":
                    for (int iBar = iPrvs; iBar < Bars; iBar++)
                    {
                        Component[1].Value[iBar] = adVolumes[iBar - iPrvs] > dLevel + Sigma() ? 1 : 0;
                        Component[2].Value[iBar] = adVolumes[iBar - iPrvs] > dLevel + Sigma() ? 1 : 0;
                    }
                    SpecialValues = new double[1] { dLevel };
                    break;

                case "The Volume is lower than the Level line":
                    for (int iBar = iPrvs; iBar < Bars; iBar++)
                    {
                        Component[1].Value[iBar] = adVolumes[iBar - iPrvs] < dLevel - Sigma() ? 1 : 0;
                        Component[2].Value[iBar] = adVolumes[iBar - iPrvs] < dLevel - Sigma() ? 1 : 0;
                    }
                    SpecialValues = new double[1] { dLevel };
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
            string sLevelLong  = IndParam.NumParam[0].ValueToString;
            string sLevelShort = IndParam.NumParam[0].ValueToString;

            EntryFilterLongDescription  = "the " + ToString() + " ";
            EntryFilterShortDescription = "the " + ToString() + " ";
            ExitFilterLongDescription   = "the " + ToString() + " ";
            ExitFilterShortDescription  = "the " + ToString() + " ";

            switch (IndParam.ListParam[0].Text)
            {
                case "The Volume rises":
                    EntryFilterLongDescription  += "rises";
                    EntryFilterShortDescription += "rises";
                    ExitFilterLongDescription   += "rises";
                    ExitFilterShortDescription  += "rises";
                    break;

                case "The Volume falls":
                    EntryFilterLongDescription  += "falls";
                    EntryFilterShortDescription += "falls";
                    ExitFilterLongDescription   += "falls";
                    ExitFilterShortDescription  += "falls";
                    break;

                case "The Volume is higher than the Level line":
                    EntryFilterLongDescription  += "is higher than the Level " + sLevelLong;
                    EntryFilterShortDescription += "is higher than the Level " + sLevelShort;
                    ExitFilterLongDescription   += "is higher than the Level " + sLevelLong;
                    ExitFilterShortDescription  += "is higher than the Level " + sLevelShort;
                    break;

                case "The Volume is lower than the Level line":
                    EntryFilterLongDescription  += "is lower than the Level " + sLevelLong;
                    EntryFilterShortDescription += "is lower than the Level " + sLevelShort;
                    ExitFilterLongDescription   += "is lower than the Level " + sLevelLong;
                    ExitFilterShortDescription  += "is lower than the Level " + sLevelShort;
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
 