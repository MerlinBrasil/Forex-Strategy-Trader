// Moving Averages Crossover Indicator
// Last changed on 2009-05-05
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2009 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Moving Averages Crossover Indicator
    /// </summary>
    public class Moving_Averages_Crossover : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Moving_Averages_Crossover(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "Moving Averages Crossover";
            PossibleSlots = SlotTypes.OpenFilter | SlotTypes.CloseFilter;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption  = "Logic";
            IndParam.ListParam[0].ItemList = new string[]
            {
                "The Fast MA crosses the Slow MA upward",
                "The Fast MA crosses the Slow MA downward",
                "The Fast MA is higher than the Slow MA",
                "The Fast MA is lower than the Slow MA",
            };
            IndParam.ListParam[0].Index    = 0;
            IndParam.ListParam[0].Text     = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled  = true;
            IndParam.ListParam[0].ToolTip  = "Logic of application of the indicator.";

            IndParam.ListParam[1].Caption  = "Base price";
            IndParam.ListParam[1].ItemList = Enum.GetNames(typeof(BasePrice));
            IndParam.ListParam[1].Index    = (int)BasePrice.Close;
            IndParam.ListParam[1].Text     = IndParam.ListParam[1].ItemList[IndParam.ListParam[1].Index];
            IndParam.ListParam[1].Enabled  = true;
            IndParam.ListParam[1].ToolTip  = "The price both Moving Averages are based on.";

            IndParam.ListParam[3].Caption  = "Fast MA method";
            IndParam.ListParam[3].ItemList = Enum.GetNames(typeof(MAMethod));
            IndParam.ListParam[3].Index    = (int)MAMethod.Simple;
            IndParam.ListParam[3].Text     = IndParam.ListParam[3].ItemList[IndParam.ListParam[3].Index];
            IndParam.ListParam[3].Enabled  = true;
            IndParam.ListParam[3].ToolTip  = "The method used for smoothing the Fast Moving Averages.";

            IndParam.ListParam[4].Caption  = "Slow MA method";
            IndParam.ListParam[4].ItemList = Enum.GetNames(typeof(MAMethod));
            IndParam.ListParam[4].Index    = (int)MAMethod.Simple;
            IndParam.ListParam[4].Text     = IndParam.ListParam[4].ItemList[IndParam.ListParam[4].Index];
            IndParam.ListParam[4].Enabled  = true;
            IndParam.ListParam[4].ToolTip  = "The method used for smoothing the slow Moving Averages.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption   = "Fast MA period";
            IndParam.NumParam[0].Value     = 13;
            IndParam.NumParam[0].Min       = 1;
            IndParam.NumParam[0].Max       = 200;
            IndParam.NumParam[0].Enabled   = true;
            IndParam.NumParam[0].ToolTip   = "The period of Fast MA.";

            IndParam.NumParam[1].Caption   = "Slow MA period";
            IndParam.NumParam[1].Value     = 21;
            IndParam.NumParam[1].Min       = 1;
            IndParam.NumParam[1].Max       = 200;
            IndParam.NumParam[1].Enabled   = true;
            IndParam.NumParam[1].ToolTip   = "The period of Slow MA.";

            IndParam.NumParam[2].Caption   = "Fast MA shift";
            IndParam.NumParam[2].Value     = 0;
            IndParam.NumParam[2].Min       = 0;
            IndParam.NumParam[2].Max       = 100;
            IndParam.NumParam[2].Point     = 0;
            IndParam.NumParam[2].Enabled   = true;
            IndParam.NumParam[2].ToolTip   = "The shifting value of Fast MA.";

            IndParam.NumParam[3].Caption   = "Slow MA shift";
            IndParam.NumParam[3].Value     = 0;
            IndParam.NumParam[3].Min       = 0;
            IndParam.NumParam[3].Max       = 100;
            IndParam.NumParam[3].Point     = 0;
            IndParam.NumParam[3].Enabled   = true;
            IndParam.NumParam[3].ToolTip   = "The shifting value of Slow MA.";

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
            BasePrice basePrice    = (BasePrice)IndParam.ListParam[1].Index;
            MAMethod  fastMAMethod = (MAMethod )IndParam.ListParam[3].Index;
            MAMethod  slowMAMethod = (MAMethod )IndParam.ListParam[4].Index;
            int       iNFastMA  = (int)IndParam.NumParam[0].Value;
            int       iNSlowMA  = (int)IndParam.NumParam[1].Value;
            int       iSFastMA  = (int)IndParam.NumParam[2].Value;
            int       iSSlowMA  = (int)IndParam.NumParam[3].Value;
            int       iPrvs     = IndParam.CheckParam[0].Checked ? 1 : 0;

            int     iFirstBar = (int)Math.Max(iNFastMA + iSFastMA, iNSlowMA + iSSlowMA) + 2;
            double[] adMAFast = MovingAverage(iNFastMA, iSFastMA, fastMAMethod, Price(basePrice));
            double[] adMASlow = MovingAverage(iNSlowMA, iSSlowMA, slowMAMethod, Price(basePrice));
            double[] adMAOscillator = new double[Bars];

            for (int iBar = iFirstBar; iBar < Bars; iBar++)
                adMAOscillator[iBar] = adMAFast[iBar] - adMASlow[iBar];

            // Saving the components
            Component = new IndicatorComp[4];

            Component[0] = new IndicatorComp();
            Component[0].CompName   = "Fast Moving Average";
            Component[0].ChartColor = Color.Goldenrod;
            Component[0].DataType   = IndComponentType.IndicatorValue;
            Component[0].ChartType  = IndChartType.Line;
            Component[0].FirstBar   = iFirstBar;
            Component[0].Value      = adMAFast;

            Component[1] = new IndicatorComp();
            Component[1].CompName   = "Slow Moving Average";
            Component[1].ChartColor = Color.IndianRed;
            Component[1].DataType   = IndComponentType.IndicatorValue;
            Component[1].ChartType  = IndChartType.Line;
            Component[1].FirstBar   = iFirstBar;
            Component[1].Value      = adMASlow;

            Component[2] = new IndicatorComp();
            Component[2].ChartType = IndChartType.NoChart;
            Component[2].FirstBar  = iFirstBar;
            Component[2].Value     = new double[Bars];

            Component[3] = new IndicatorComp();
            Component[3].ChartType = IndChartType.NoChart;
            Component[3].FirstBar  = iFirstBar;
            Component[3].Value     = new double[Bars];

            // Sets the Component's type
            if (slotType == SlotTypes.OpenFilter)
            {
                Component[2].DataType = IndComponentType.AllowOpenLong;
                Component[2].CompName = "Is long entry allowed";
                Component[3].DataType = IndComponentType.AllowOpenShort;
                Component[3].CompName = "Is short entry allowed";
            }
            else if (slotType == SlotTypes.CloseFilter)
            {
                Component[2].DataType = IndComponentType.ForceCloseLong;
                Component[2].CompName = "Close out long position";
                Component[3].DataType = IndComponentType.ForceCloseShort;
                Component[3].CompName = "Close out short position";
            }

            // Calculation of the logic
            IndicatorLogic indLogic = IndicatorLogic.It_does_not_act_as_a_filter;

            switch (IndParam.ListParam[0].Text)
            {
                case "The Fast MA crosses the Slow MA upward":
                    indLogic = IndicatorLogic.The_indicator_crosses_the_level_line_upward;
                    break;
                case "The Fast MA crosses the Slow MA downward":
                    indLogic = IndicatorLogic.The_indicator_crosses_the_level_line_downward;
                    break;
                case "The Fast MA is higher than the Slow MA":
                    indLogic = IndicatorLogic.The_indicator_is_higher_than_the_level_line;
                    break;
                case "The Fast MA is lower than the Slow MA":
                    indLogic = IndicatorLogic.The_indicator_is_lower_than_the_level_line;
                    break;
                default:
                    break;
            }

            OscillatorLogic(iFirstBar, iPrvs, adMAOscillator, 0, 0, ref Component[2], ref Component[3], indLogic);

            return;
        }

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {
            EntryFilterLongDescription  = ToString() + "; the Fast MA ";
            EntryFilterShortDescription = ToString() + "; the Fast MA ";
            ExitFilterLongDescription   = ToString() + "; the Fast MA ";
            ExitFilterShortDescription  = ToString() + "; the Fast MA ";

            switch (IndParam.ListParam[0].Text)
            {
                case "The Fast MA crosses the Slow MA upward":
                    EntryFilterLongDescription  += "crosses the Slow MA upward";
                    EntryFilterShortDescription += "crosses the Slow MA downward";
                    ExitFilterLongDescription   += "crosses the Slow MA upward";
                    ExitFilterShortDescription  += "crosses the Slow MA downward";
                    break;

                case "The Fast MA crosses the Slow MA downward":
                    EntryFilterLongDescription  += "crosses the Slow MA downward";
                    EntryFilterShortDescription += "crosses the Slow MA upward";
                    ExitFilterLongDescription   += "crosses the Slow MA downward";
                    ExitFilterShortDescription  += "crosses the Slow MA upward";
                    break;

                case "The Fast MA is higher than the Slow MA":
                    EntryFilterLongDescription  += "is higher than the Slow MA";
                    EntryFilterShortDescription += "is lower than the Slow MA";
                    ExitFilterLongDescription   += "is higher than the Slow MA";
                    ExitFilterShortDescription  += "is lower than the Slow MA";
                    break;

                case "The Fast MA is lower than the Slow MA":
                    EntryFilterLongDescription  += "is lower than the Slow MA";
                    EntryFilterShortDescription += "is higher than the Slow MA";
                    ExitFilterLongDescription   += "is lower than the Slow MA";
                    ExitFilterShortDescription  += "is higher than the Slow MA";
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
                IndParam.ListParam[1].Text         + ", " + // Price
                IndParam.ListParam[3].Text         + ", " + // Fast MA Method
                IndParam.ListParam[4].Text         + ", " + // Slow MA Method
                IndParam.NumParam[0].ValueToString + ", " + // Fast MA period
                IndParam.NumParam[1].ValueToString + ", " + // Slow MA period
                IndParam.NumParam[2].ValueToString + ", " + // Fast MA shift
                IndParam.NumParam[3].ValueToString + ")";   // Slow MA shift

            return sString;
        }
    }
}
