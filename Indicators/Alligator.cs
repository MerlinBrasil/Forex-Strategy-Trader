// Alligator Indicator
// Last changed on 2009-05-05
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// This code or any part of it cannot be used in other applications without a permission.
// Copyright (c) 2006 - 2009 Miroslav Popov - All rights reserved.

using System;
using System.Drawing;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Alligator Indicator
    /// </summary>
    public class Alligator : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Alligator(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "Alligator";
            PossibleSlots = SlotTypes.OpenFilter | SlotTypes.CloseFilter;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption  = "Logic";
            IndParam.ListParam[0].ItemList = new string[]
            {
                "The Lips rises",
                "The Lips falls",
                "The Teeth rises",
                "The Teeth falls",
                "The Jaws rises",
                "The Jaws falls",
                "The Lips crosses the Teeth upward",
                "The Lips crosses the Teeth downward",
                "The Lips crosses the Jaws upward",
                "The Lips crosses the Jaws downward",
                "The Teeth crosses the Jaws upward",
                "The Teeth crosses the Jaws downward"
            };
            IndParam.ListParam[0].Index    = 0;
            IndParam.ListParam[0].Text     = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled  = true;
            IndParam.ListParam[0].ToolTip  = "Logic of application of the indicator.";

            IndParam.ListParam[1].Caption  = "Smoothing method";
            IndParam.ListParam[1].ItemList = Enum.GetNames(typeof(MAMethod));
            IndParam.ListParam[1].Index    = (int)MAMethod.Smoothed;
            IndParam.ListParam[1].Text     = IndParam.ListParam[1].ItemList[IndParam.ListParam[1].Index];
            IndParam.ListParam[1].Enabled  = true;
            IndParam.ListParam[1].ToolTip  = "The method of Moving Average used for the calculations.";

            IndParam.ListParam[2].Caption  = "Base price";
            IndParam.ListParam[2].ItemList = Enum.GetNames(typeof(BasePrice));
            IndParam.ListParam[2].Index    = (int)BasePrice.Median;
            IndParam.ListParam[2].Text     = IndParam.ListParam[2].ItemList[IndParam.ListParam[2].Index];
            IndParam.ListParam[2].Enabled  = true;
            IndParam.ListParam[2].ToolTip  = "The price the indicator is based on.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "Jaws period";
            IndParam.NumParam[0].Value   = 13;
            IndParam.NumParam[0].Min     = 1;
            IndParam.NumParam[0].Max     = 200;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "The Moving Average period.";

            IndParam.NumParam[1].Caption = "Jaws shift";
            IndParam.NumParam[1].Value   = 8;
            IndParam.NumParam[1].Min     = 0;
            IndParam.NumParam[1].Max     = 200;
            IndParam.NumParam[1].Enabled = true;
            IndParam.NumParam[1].ToolTip = "How many bars to shift with.";

            IndParam.NumParam[2].Caption = "Teeth period";
            IndParam.NumParam[2].Value   = 8;
            IndParam.NumParam[2].Min     = 1;
            IndParam.NumParam[2].Max     = 200;
            IndParam.NumParam[2].Enabled = true;
            IndParam.NumParam[2].ToolTip = "The Moving Average period.";

            IndParam.NumParam[3].Caption = "Teeth shift";
            IndParam.NumParam[3].Value   = 5;
            IndParam.NumParam[3].Min     = 0;
            IndParam.NumParam[3].Max     = 200;
            IndParam.NumParam[3].Enabled = true;
            IndParam.NumParam[3].ToolTip = "How many bars to shift with.";

            IndParam.NumParam[4].Caption = "Lips period";
            IndParam.NumParam[4].Value   = 5;
            IndParam.NumParam[4].Min     = 1;
            IndParam.NumParam[4].Max     = 200;
            IndParam.NumParam[4].Enabled = true;
            IndParam.NumParam[4].ToolTip = "The Moving Average period.";

            IndParam.NumParam[5].Caption = "Lips shift";
            IndParam.NumParam[5].Value   = 3;
            IndParam.NumParam[5].Min     = 0;
            IndParam.NumParam[5].Max     = 200;
            IndParam.NumParam[5].Enabled = true;
            IndParam.NumParam[5].ToolTip = "How many bars to shift with.";

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
            MAMethod  maMethod  = (MAMethod )IndParam.ListParam[1].Index;
            BasePrice basePrice = (BasePrice)IndParam.ListParam[2].Index;
            int iNJaws  = (int)IndParam.NumParam[0].Value;
            int iSJaws  = (int)IndParam.NumParam[1].Value;
            int iNTeeth = (int)IndParam.NumParam[2].Value;
            int iSTeeth = (int)IndParam.NumParam[3].Value;
            int iNLips  = (int)IndParam.NumParam[4].Value;
            int iSLips  = (int)IndParam.NumParam[5].Value;
            int iPrvs   =      IndParam.CheckParam[0].Checked ? 1 : 0;

            int iFirstBar = Math.Max(iNJaws + iSJaws + 2, iNTeeth + iSTeeth + 2);
            iFirstBar = Math.Max(iFirstBar, iNLips + iSLips + 2);

            // Calculation
            double[] adJaws  = MovingAverage(iNJaws , iSJaws , maMethod, Price(basePrice));
            double[] adTeeth = MovingAverage(iNTeeth, iSTeeth, maMethod, Price(basePrice));
            double[] adLips  = MovingAverage(iNLips , iSLips , maMethod, Price(basePrice));

            // Saving the components
            Component = new IndicatorComp[5];

            Component[0] = new IndicatorComp();
            Component[0].CompName   = "Jaws";
            Component[0].DataType   = IndComponentType.IndicatorValue;
            Component[0].ChartType  = IndChartType.Line;
            Component[0].ChartColor = Color.Blue;
            Component[0].FirstBar   = iFirstBar;
            Component[0].Value      = adJaws;

            Component[1] = new IndicatorComp();
            Component[1].CompName   = "Teeth";
            Component[1].DataType   = IndComponentType.IndicatorValue;
            Component[1].ChartType  = IndChartType.Line;
            Component[1].ChartColor = Color.Red;
            Component[1].FirstBar   = iFirstBar;
            Component[1].Value      = adTeeth;

            Component[2] = new IndicatorComp();
            Component[2].CompName   = "Lips";
            Component[2].DataType   = IndComponentType.IndicatorValue;
            Component[2].ChartType  = IndChartType.Line;
            Component[2].ChartColor = Color.Lime;
            Component[2].FirstBar   = iFirstBar;
            Component[2].Value      = adLips;

            Component[3] = new IndicatorComp();
            Component[3].ChartType = IndChartType.NoChart;
            Component[3].FirstBar  = iFirstBar;
            Component[3].Value     = new double[Bars];

            Component[4] = new IndicatorComp();
            Component[4].ChartType = IndChartType.NoChart;
            Component[4].FirstBar  = iFirstBar;
            Component[4].Value     = new double[Bars];

            // Sets the Component's type.
            if (slotType == SlotTypes.OpenFilter)
            {
                Component[3].DataType = IndComponentType.AllowOpenLong;
                Component[3].CompName = "Is long entry allowed";
                Component[4].DataType = IndComponentType.AllowOpenShort;
                Component[4].CompName = "Is short entry allowed";
            }
            else if (slotType == SlotTypes.CloseFilter)
            {
                Component[3].DataType = IndComponentType.ForceCloseLong;
                Component[3].CompName = "Close out long position";
                Component[4].DataType = IndComponentType.ForceCloseShort;
                Component[4].CompName = "Close out short position";
            }

            switch (IndParam.ListParam[0].Text)
            {
                case "The Jaws rises":
                    IndicatorRisesLogic(iFirstBar, iPrvs, adJaws, ref Component[3], ref Component[4]);
                    break;

                case "The Jaws falls":
                    IndicatorFallsLogic(iFirstBar, iPrvs, adJaws, ref Component[3], ref Component[4]);
                    break;

                case "The Teeth rises":
                    IndicatorRisesLogic(iFirstBar, iPrvs, adTeeth, ref Component[3], ref Component[4]);
                    break;

                case "The Teeth falls":
                    IndicatorFallsLogic(iFirstBar, iPrvs, adTeeth, ref Component[3], ref Component[4]);
                    break;

                case "The Lips rises":
                    IndicatorRisesLogic(iFirstBar, iPrvs, adLips, ref Component[3], ref Component[4]);
                    break;

                case "The Lips falls":
                    IndicatorFallsLogic(iFirstBar, iPrvs, adLips, ref Component[3], ref Component[4]);
                    break;

                case "The Lips crosses the Teeth upward":
                    IndicatorCrossesAnotherIndicatorUpwardLogic(iFirstBar, iPrvs, adLips, adTeeth, ref Component[3], ref Component[4]);
                    break;

                case "The Lips crosses the Teeth downward":
                    IndicatorCrossesAnotherIndicatorDownwardLogic(iFirstBar, iPrvs, adLips, adTeeth, ref Component[3], ref Component[4]);
                    break;

                case "The Lips crosses the Jaws upward":
                    IndicatorCrossesAnotherIndicatorUpwardLogic(iFirstBar, iPrvs, adLips, adJaws, ref Component[3], ref Component[4]);
                    break;

                case "The Lips crosses the Jaws downward":
                    IndicatorCrossesAnotherIndicatorDownwardLogic(iFirstBar, iPrvs, adLips, adJaws, ref Component[3], ref Component[4]);
                    break;

                case "The Teeth crosses the Jaws upward":
                    IndicatorCrossesAnotherIndicatorUpwardLogic(iFirstBar, iPrvs, adTeeth, adJaws, ref Component[3], ref Component[4]);
                    break;

                case "The Teeth crosses the Jaws downward":
                    IndicatorCrossesAnotherIndicatorDownwardLogic(iFirstBar, iPrvs, adTeeth, adJaws, ref Component[3], ref Component[4]);
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
            EntryFilterLongDescription  = "the " + ToString() + " ";
            EntryFilterShortDescription = "the " + ToString() + " ";
            ExitFilterLongDescription   = "the " + ToString() + " ";
            ExitFilterShortDescription  = "the " + ToString() + " ";

            switch (IndParam.ListParam[0].Text)
            {
                case "The Jaws rises":
                    EntryFilterLongDescription  += "Jaws rises";
                    EntryFilterShortDescription += "Jaws falls";
                    ExitFilterLongDescription   += "Jaws rises";
                    ExitFilterShortDescription  += "Jaws falls";
                    break;

                case "The Jaws falls":
                    EntryFilterLongDescription  += "Jaws falls";
                    EntryFilterShortDescription += "Jaws rises";
                    ExitFilterLongDescription   += "Jaws falls";
                    ExitFilterShortDescription  += "Jaws rises";
                    break;

                case "The Teeth rises":
                    EntryFilterLongDescription  += "Teeth rises";
                    EntryFilterShortDescription += "Teeth falls";
                    ExitFilterLongDescription   += "Teeth rises";
                    ExitFilterShortDescription  += "Teeth falls";
                    break;

                case "The Teeth falls":
                    EntryFilterLongDescription  += "Teeth falls";
                    EntryFilterShortDescription += "Teeth rises";
                    ExitFilterLongDescription   += "Teeth falls";
                    ExitFilterShortDescription  += "Teeth rises";
                    break;

                case "The Lips rises":
                    EntryFilterLongDescription  += "Lips rises";
                    EntryFilterShortDescription += "Lips falls";
                    ExitFilterLongDescription   += "Lips rises";
                    ExitFilterShortDescription  += "Lips falls";
                    break;

                case "The Lips falls":
                    EntryFilterLongDescription  += "Lips falls";
                    EntryFilterShortDescription += "Lips rises";
                    ExitFilterLongDescription   += "Lips falls";
                    ExitFilterShortDescription  += "Lips rises";
                    break;

                case "The Lips crosses the Teeth upward":
                    EntryFilterLongDescription  += "Lips crosses the Alligator Teeth upward";
                    EntryFilterShortDescription += "Lips crosses the Alligator Teeth downward";
                    ExitFilterLongDescription   += "Lips crosses the Alligator Teeth upward";
                    ExitFilterShortDescription  += "Lips crosses the Alligator Teeth downward";
                    break;

                case "The Lips crosses the Teeth downward":
                    EntryFilterLongDescription  += "Lips crosses the Alligator Teeth downward";
                    EntryFilterShortDescription += "Lips crosses the Alligator Teeth upward";
                    ExitFilterLongDescription   += "Lips crosses the Alligator Teeth downward";
                    ExitFilterShortDescription  += "Lips crosses the Alligator Teeth upward";
                    break;

                case "The Lips crosses the Jaws upward":
                    EntryFilterLongDescription  += "Lips crosses the Alligator Jaws upward";
                    EntryFilterShortDescription += "Lips crosses the Alligator Jaws downward";
                    ExitFilterLongDescription   += "Lips crosses the Alligator Jaws upward";
                    ExitFilterShortDescription  += "Lips crosses the Alligator Jaws downward";
                    break;

                case "The Lips crosses the Jaws downward":
                    EntryFilterLongDescription  += "Lips crosses the Alligator Jaws downward";
                    EntryFilterShortDescription += "Lips crosses the Alligator Jaws upward";
                    ExitFilterLongDescription   += "Lips crosses the Alligator Jaws downward";
                    ExitFilterShortDescription  += "Lips crosses the Alligator Jaws upward";
                    break;

                case "The Teeth crosses the Jaws upward":
                    EntryFilterLongDescription  += "Teeth crosses the Alligator Jaws upward";
                    EntryFilterShortDescription += "Teeth crosses the Alligator Jaws downward";
                    ExitFilterLongDescription   += "Teeth crosses the Alligator Jaws upward";
                    ExitFilterShortDescription  += "Teeth crosses the Alligator Jaws downward";
                    break;

                case "The Teeth crosses the Jaws downward":
                    EntryFilterLongDescription  += "Teeth crosses the Alligator Jaws downward";
                    EntryFilterShortDescription += "Teeth crosses the Alligator Jaws upward";
                    ExitFilterLongDescription   += "Teeth crosses the Alligator Jaws downward";
                    ExitFilterShortDescription  += "Teeth crosses the Alligator Jaws upward";
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
                IndParam.ListParam[1].Text         + ", "+ // Method
                IndParam.ListParam[2].Text         + ", "+ // Price
                IndParam.NumParam[0].ValueToString + ", "+ // Jaws period
                IndParam.NumParam[1].ValueToString + ", "+ // Jaws shift
                IndParam.NumParam[2].ValueToString + ", "+ // Teeth period
                IndParam.NumParam[3].ValueToString + ", "+ // Teeth shift
                IndParam.NumParam[4].ValueToString + ", "+ // Lips period
                IndParam.NumParam[5].ValueToString + ")";  // Lips shift

            return sString;
        }
    }
}
