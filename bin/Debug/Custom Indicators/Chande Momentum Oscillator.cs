// Chande Momentum Oscillator Indicator
// Last changed on 2009-12-04
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// This code or any part of it cannot be used in other applications without a permission.
// Copyright (c) 2006 - 2009 Miroslav Popov - All rights reserved.

using System;
using System.Drawing;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// CMO - Chande Momentum Oscillator
    /// </summary>
    public class CMO : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public CMO(SlotTypes slotType)
        {
            // General properties
            IndicatorName  = "Chande Momentum Oscillator";
            PossibleSlots  = SlotTypes.OpenFilter | SlotTypes.CloseFilter;
            SeparatedChart = true;
            SeparatedChartMinValue = -100;
            SeparatedChartMaxValue = 100;
			CustomIndicator = true;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption = "Logic";
            IndParam.ListParam[0].ItemList = new string[]
            {
                "The CMO rises",
                "The CMO falls",
                "The CMO is higher than the Level line",
                "The CMO is lower than the Level line",
                "The CMO crosses the Level line upward",
                "The CMO crosses the Level line downward",
                "The CMO changes its direction upward",
                "The CMO changes its direction downward"
            };
            IndParam.ListParam[0].Index    = 0;
            IndParam.ListParam[0].Text     = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled  = true;
            IndParam.ListParam[0].ToolTip  = "Logic of application of the indicator.";

            IndParam.ListParam[2].Caption  = "Base price";
            IndParam.ListParam[2].ItemList = Enum.GetNames(typeof(BasePrice));
            IndParam.ListParam[2].Index    = (int)BasePrice.Close;
            IndParam.ListParam[2].Text     = IndParam.ListParam[2].ItemList[IndParam.ListParam[2].Index];
            IndParam.ListParam[2].Enabled  = true;
            IndParam.ListParam[2].ToolTip  = "The price the CMO is based on.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption   = "Smoothing period";
            IndParam.NumParam[0].Value     = 14;
            IndParam.NumParam[0].Min       = 1;
            IndParam.NumParam[0].Max       = 200;
            IndParam.NumParam[0].Enabled   = true;
            IndParam.NumParam[0].ToolTip   = "The period of smoothing of the CMO value.";

            IndParam.NumParam[1].Caption   = "Level";
            IndParam.NumParam[1].Value     = 30;
            IndParam.NumParam[1].Min       = 0;
            IndParam.NumParam[1].Max       = 100;
            IndParam.NumParam[1].Enabled   = true;
            IndParam.NumParam[1].ToolTip   = "A critical level (for the appropriate logic).";

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
            BasePrice basePrice = (BasePrice)IndParam.ListParam[2].Index;
            int       iPeriod   = (int)IndParam.NumParam[0].Value;
            double    dLevel    = IndParam.NumParam[1].Value;
            int       iPrvs     = IndParam.CheckParam[0].Checked ? 1 : 0;

            // Calculation
            int      iFirstBar   = iPeriod + 2;
            double[] adBasePrice = Price(basePrice);
            double[] adCMO1      = new double[Bars];
            double[] adCMO2      = new double[Bars];
            double[] adCMO1Sum   = new double[Bars];
            double[] adCMO2Sum   = new double[Bars];
            double[] adCMO       = new double[Bars];

            for (int iBar = 1; iBar < Bars; iBar++)
            {
				adCMO1[iBar] = 0;
				adCMO2[iBar] = 0;
                if (adBasePrice[iBar] > adBasePrice[iBar - 1])
					adCMO1[iBar] = adBasePrice[iBar] - adBasePrice[iBar - 1];
                if (adBasePrice[iBar] < adBasePrice[iBar - 1])
					adCMO2[iBar] = adBasePrice[iBar - 1] - adBasePrice[iBar];
            }

            for (int iBar = 0; iBar < iPeriod; iBar++)
            {
                adCMO1Sum[iPeriod - 1] += adCMO1[iBar];
                adCMO2Sum[iPeriod - 1] += adCMO2[iBar];
            }

            for (int iBar = iPeriod; iBar < Bars; iBar++)
            {
                adCMO1Sum[iBar] = adCMO1Sum[iBar - 1] + adCMO1[iBar] - adCMO1[iBar - iPeriod];
                adCMO2Sum[iBar] = adCMO2Sum[iBar - 1] + adCMO2[iBar] - adCMO2[iBar - iPeriod];

                if (adCMO1Sum[iBar] + adCMO2Sum[iBar] == 0)
                    adCMO[iBar] = 100;
                else
                    adCMO[iBar] = 100 * (adCMO1Sum[iBar] - adCMO2Sum[iBar]) / (adCMO1Sum[iBar] + adCMO2Sum[iBar]);
            }

            // Saving the components
            Component = new IndicatorComp[3];

            Component[0] = new IndicatorComp();
            Component[0].CompName   = "CMO";
            Component[0].DataType   = IndComponentType.IndicatorValue;
            Component[0].ChartType  = IndChartType.Line;
            Component[0].ChartColor = Color.RoyalBlue;
            Component[0].FirstBar   = iFirstBar;
            Component[0].Value      = adCMO;

            Component[1] = new IndicatorComp();
            Component[1].ChartType  = IndChartType.NoChart;
            Component[1].FirstBar   = iFirstBar;
            Component[1].Value      = new double[Bars];

            Component[2] = new IndicatorComp();
            Component[2].ChartType  = IndChartType.NoChart;
            Component[2].FirstBar   = iFirstBar;
            Component[2].Value      = new double[Bars];

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
                case "The CMO rises":
                    indLogic = IndicatorLogic.The_indicator_rises;
                    SpecialValues = new double[1] { 0 };
                    break;

                case "The CMO falls":
                    indLogic = IndicatorLogic.The_indicator_falls;
                    SpecialValues = new double[1] { 0 };
                    break;

                case "The CMO is higher than the Level line":
                    indLogic = IndicatorLogic.The_indicator_is_higher_than_the_level_line;
                    SpecialValues = new double[2] { dLevel, -dLevel };
                    break;

                case "The CMO is lower than the Level line":
                    indLogic = IndicatorLogic.The_indicator_is_lower_than_the_level_line;
                    SpecialValues = new double[2] { dLevel, -dLevel };
                    break;

                case "The CMO crosses the Level line upward":
                    indLogic = IndicatorLogic.The_indicator_crosses_the_level_line_upward;
                    SpecialValues = new double[2] { dLevel, -dLevel };
                    break;

                case "The CMO crosses the Level line downward":
                    indLogic = IndicatorLogic.The_indicator_crosses_the_level_line_downward;
                    SpecialValues = new double[2] { dLevel, -dLevel };
                    break;

                case "The CMO changes its direction upward":
                    indLogic = IndicatorLogic.The_indicator_changes_its_direction_upward;
                    SpecialValues = new double[1] { 0 };
                    break;

                case "The CMO changes its direction downward":
                    indLogic = IndicatorLogic.The_indicator_changes_its_direction_downward;
                    SpecialValues = new double[1] { 0 };
                    break;

                default:
                    break;
            }

            OscillatorLogic(iFirstBar, iPrvs, adCMO, dLevel, -dLevel, ref Component[1], ref Component[2], indLogic);

            return;
        }

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {
            string sLevelLong  = IndParam.NumParam[1].ValueToString;
            string sLevelShort = IndParam.NumParam[1].AnotherValueToString(-IndParam.NumParam[1].Value);

            EntryFilterLongDescription  = "the " + ToString() + " ";
            EntryFilterShortDescription = "the " + ToString() + " ";
            ExitFilterLongDescription   = "the " + ToString() + " ";
            ExitFilterShortDescription  = "the " + ToString() + " ";

            switch (IndParam.ListParam[0].Text)
            {
                case "The CMO rises":
                    EntryFilterLongDescription  += "rises";
                    EntryFilterShortDescription += "falls";
                    ExitFilterLongDescription   += "rises";
                    ExitFilterShortDescription  += "falls";
                    break;

                case "The CMO falls":
                    EntryFilterLongDescription  += "falls";
                    EntryFilterShortDescription += "rises";
                    ExitFilterLongDescription   += "falls";
                    ExitFilterShortDescription  += "rises";
                    break;

                case "The CMO is higher than the Level line":
                    EntryFilterLongDescription  += "is higher than the Level " + sLevelLong;
                    EntryFilterShortDescription += "is lower than the Level "  + sLevelShort;
                    ExitFilterLongDescription   += "is higher than the Level " + sLevelLong;
                    ExitFilterShortDescription  += "is lower than the Level "  + sLevelShort;
                    break;

                case "The CMO is lower than the Level line":
                    EntryFilterLongDescription  += "is lower than the Level "  + sLevelLong;
                    EntryFilterShortDescription += "is higher than the Level " + sLevelShort;
                    ExitFilterLongDescription   += "is lower than the Level "  + sLevelLong;
                    ExitFilterShortDescription  += "is higher than the Level " + sLevelShort;
                    break;

                case "The CMO crosses the Level line upward":
                    EntryFilterLongDescription  += "crosses the Level " + sLevelLong  + " upward";
                    EntryFilterShortDescription += "crosses the Level " + sLevelShort + " downward";
                    ExitFilterLongDescription   += "crosses the Level " + sLevelLong  + " upward";
                    ExitFilterShortDescription  += "crosses the Level " + sLevelShort + " downward";
                    break;

                case "The CMO crosses the Level line downward":
                    EntryFilterLongDescription  += "crosses the Level " + sLevelLong  + " downward";
                    EntryFilterShortDescription += "crosses the Level " + sLevelShort + " upward";
                    ExitFilterLongDescription   += "crosses the Level " + sLevelLong  + " downward";
                    ExitFilterShortDescription  += "crosses the Level " + sLevelShort + " upward";
                    break;

                case "The CMO changes its direction upward":
                    EntryFilterLongDescription  += "changes its direction upward";
                    EntryFilterShortDescription += "changes its direction downward";
                    ExitFilterLongDescription   += "changes its direction upward";
                    ExitFilterShortDescription  += "changes its direction downward";
                    break;

                case "The CMO changes its direction downward":
                    EntryFilterLongDescription  += "changes its direction downward";
                    EntryFilterShortDescription += "changes its direction upward";
                    ExitFilterLongDescription   += "changes its direction downward";
                    ExitFilterShortDescription  += "changes its direction upward";
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
            string sString = IndicatorName +
                (IndParam.CheckParam[0].Checked ? "* (" : " (") +
                IndParam.ListParam[2].Text         + ", " + // Base price
                IndParam.NumParam[0].ValueToString + ")";   // Smoothing period

            return sString;
        }
    }
}
