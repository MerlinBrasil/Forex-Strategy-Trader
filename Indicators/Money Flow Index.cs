// Money Flow Index Indicator
// Last changed on 2009-05-05
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// This code or any part of it cannot be used in other applications without a permission.
// Copyright (c) 2006 - 2009 Miroslav Popov - All rights reserved.

using System.Drawing;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Money Flow Index Indicator
    /// </summary>
    public class Money_Flow_Index : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Money_Flow_Index(SlotTypes slotType)
        {
            // General properties
            IndicatorName  = "Money Flow Index";
            PossibleSlots  = SlotTypes.OpenFilter | SlotTypes.CloseFilter;
            SeparatedChart = true;
            SeparatedChartMinValue = 0;
            SeparatedChartMaxValue = 100;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption = "Logic";
            IndParam.ListParam[0].ItemList = new string[]
            {
                "The MFI rises",
                "The MFI falls",
                "The MFI is higher than the Level line",
                "The MFI is lower than the Level line",
                "The MFI crosses the Level line upward",
                "The MFI crosses the Level line downward",
                "The MFI changes its direction upward",
                "The MFI changes its direction downward"
            };
            IndParam.ListParam[0].Index   = 0;
            IndParam.ListParam[0].Text    = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Logic of application of the MFI.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption   = "Period";
            IndParam.NumParam[0].Value     = 14;
            IndParam.NumParam[0].Min       = 1;
            IndParam.NumParam[0].Max       = 200;
            IndParam.NumParam[0].Enabled   = true;
            IndParam.NumParam[0].ToolTip   = "The period of Money Flow Index.";

            IndParam.NumParam[1].Caption = "Level";
            IndParam.NumParam[1].Value   = 50 ;
            IndParam.NumParam[1].Min     = 0  ;
            IndParam.NumParam[1].Max     = 100;
            IndParam.NumParam[1].Enabled = true;
            IndParam.NumParam[1].ToolTip = "A critical level (for the appropriate logic).";

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
            int    iPeriod = (int)IndParam.NumParam[0].Value;
            double dLevel  = IndParam.NumParam[1].Value;
            int    iPrvs   = IndParam.CheckParam[0].Checked ? 1 : 0;

            int iFirstBar = iPeriod + iPrvs;

            // Calculating Money Flow
            double[] adMF  = new double[Bars];
            for (int iBar = 1; iBar < Bars; iBar++)
            {
                double dAVG  = (High[iBar] + Low[iBar] + Close[iBar]) / 3;
                double dAVG1 = (High[iBar - 1] + Low[iBar - 1] + Close[iBar - 1]) / 3;
                if (dAVG > dAVG1)
                    adMF[iBar] = adMF[iBar - 1] + dAVG * Volume[iBar];
                else if (dAVG < dAVG1)
                    adMF[iBar] = adMF[iBar - 1] - dAVG * Volume[iBar];
                else
                    adMF[iBar] = adMF[iBar - 1];
            }

            // Calculating Money Flow Index
            double[] adMFI = new double[Bars];
            for (int iBar = iPeriod + 1; iBar < Bars; iBar++)
            {
                double dPMF = 0;
                double dNMF = 0;
                for (int index = 0; index < iPeriod; index++)
                {
                    if (adMF[iBar - index] > adMF[iBar - index - 1])
                        dPMF += adMF[iBar - index] - adMF[iBar - index - 1];
                    if (adMF[iBar - index] < adMF[iBar - index - 1])
                        dNMF += adMF[iBar - index - 1] - adMF[iBar - index];
                }
                if (dNMF == 0)
                    adMFI[iBar] = 100.0;
                else
                    adMFI[iBar] = 100.0 - (100.0 / (1.0 + (dPMF / dNMF)));
            }
            
            // Saving the components
            Component = new IndicatorComp[3];

            Component[0] = new IndicatorComp();
            Component[0].CompName   = "Money Flow Index";
            Component[0].DataType   = IndComponentType.IndicatorValue;
            Component[0].ChartType  = IndChartType.Line;
            Component[0].ChartColor = Color.Blue;
            Component[0].FirstBar   = iFirstBar;
            Component[0].Value      = adMFI;

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
                case "The MFI rises":
                    indLogic = IndicatorLogic.The_indicator_rises;
                    SpecialValues = new double[1] { 50 };
                    break;

                case "The MFI falls":
                    indLogic = IndicatorLogic.The_indicator_falls;
                    SpecialValues = new double[1] { 50 };
                    break;

                case "The MFI is higher than the Level line":
                    indLogic = IndicatorLogic.The_indicator_is_higher_than_the_level_line;
                    SpecialValues = new double[2] { dLevel, 100 - dLevel };
                    break;

                case "The MFI is lower than the Level line":
                    indLogic = IndicatorLogic.The_indicator_is_lower_than_the_level_line;
                    SpecialValues = new double[2] { dLevel, 100 - dLevel };
                    break;

                case "The MFI crosses the Level line upward":
                    indLogic = IndicatorLogic.The_indicator_crosses_the_level_line_upward;
                    SpecialValues = new double[2] { dLevel, 100 - dLevel };
                    break;

                case "The MFI crosses the Level line downward":
                    indLogic = IndicatorLogic.The_indicator_crosses_the_level_line_downward;
                    SpecialValues = new double[2] { dLevel, 100 - dLevel };
                    break;

                case "The MFI changes its direction upward":
                    indLogic = IndicatorLogic.The_indicator_changes_its_direction_upward;
                    SpecialValues = new double[1] { 50 };
                    break;

                case "The MFI changes its direction downward":
                    indLogic = IndicatorLogic.The_indicator_changes_its_direction_downward;
                    SpecialValues = new double[1] { 50 };
                    break;

                default:
                    break;
            }

            OscillatorLogic(iFirstBar, iPrvs, adMFI, dLevel, 100 - dLevel, ref Component[1], ref Component[2], indLogic);

            return;
        }

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {
            string sLevelLong  = IndParam.NumParam[1].ValueToString;
            string sLevelShort = IndParam.NumParam[1].AnotherValueToString(100 - IndParam.NumParam[1].Value);

            EntryFilterLongDescription  = "the " + ToString() + " ";
            EntryFilterShortDescription = "the " + ToString() + " ";
            ExitFilterLongDescription   = "the " + ToString() + " ";
            ExitFilterShortDescription  = "the " + ToString() + " ";

            switch (IndParam.ListParam[0].Text)
            {
                case "The MFI rises":
                    EntryFilterLongDescription  += "rises";
                    EntryFilterShortDescription += "falls";
                    ExitFilterLongDescription   += "rises";
                    ExitFilterShortDescription  += "falls";
                    break;

                case "The MFI falls":
                    EntryFilterLongDescription  += "falls";
                    EntryFilterShortDescription += "rises";
                    ExitFilterLongDescription   += "falls";
                    ExitFilterShortDescription  += "rises";
                    break;

                case "The MFI is higher than the Level line":
                    EntryFilterLongDescription  += "is higher than the Level " + sLevelLong;
                    EntryFilterShortDescription += "is lower than the Level "  + sLevelShort;
                    ExitFilterLongDescription   += "is higher than the Level " + sLevelLong;
                    ExitFilterShortDescription  += "is lower than the Level "  + sLevelShort;
                    break;

                case "The MFI is lower than the Level line":
                    EntryFilterLongDescription  += "is lower than the Level "  + sLevelLong;
                    EntryFilterShortDescription += "is higher than the Level " + sLevelShort;
                    ExitFilterLongDescription   += "is lower than the Level "  + sLevelLong;
                    ExitFilterShortDescription  += "is higher than the Level " + sLevelShort;
                    break;

                case "The MFI crosses the Level line upward":
                    EntryFilterLongDescription  += "crosses the Level " + sLevelLong  + " upward";
                    EntryFilterShortDescription += "crosses the Level " + sLevelShort + " downward";
                    ExitFilterLongDescription   += "crosses the Level " + sLevelLong  + " upward";
                    ExitFilterShortDescription  += "crosses the Level " + sLevelShort + " downward";
                    break;

                case "The MFI crosses the Level line downward":
                    EntryFilterLongDescription  += "crosses the Level " + sLevelLong  + " downward";
                    EntryFilterShortDescription += "crosses the Level " + sLevelShort + " upward";
                    ExitFilterLongDescription   += "crosses the Level " + sLevelLong  + " downward";
                    ExitFilterShortDescription  += "crosses the Level " + sLevelShort + " upward";
                    break;

                case "The MFI changes its direction upward":
                    EntryFilterLongDescription  += "changes its direction upward";
                    EntryFilterShortDescription += "changes its direction downward";
                    ExitFilterLongDescription   += "changes its direction upward";
                    ExitFilterShortDescription  += "changes its direction downward";
                    break;

                case "The MFI changes its direction downward":
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
                IndParam.NumParam[0].ValueToString + ")"; // MFI period

            return sString;
        }
    }
}