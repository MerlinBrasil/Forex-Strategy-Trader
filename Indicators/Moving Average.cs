// Moving Average Indicator
// Last changed on 2009-12-15
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// This code or any part of it cannot be used in other applications without a permission.
// Copyright (c) 2006 - 2009 Miroslav Popov - All rights reserved.

using System;
using System.Drawing;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Moving Average Indicator 
    /// </summary>
    public class Moving_Average : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Moving_Average(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "Moving Average";
            PossibleSlots = SlotTypes.Open | SlotTypes.OpenFilter | SlotTypes.Close | SlotTypes.CloseFilter;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption = "Logic";
            if (slotType == SlotTypes.Open)
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "Enter the market at the Moving Average"
                };
            else if (slotType == SlotTypes.OpenFilter)
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "The Moving Average rises",
                    "The Moving Average falls",
                    "The bar opens above the Moving Average",
                    "The bar opens below the Moving Average",
                    "The bar opens above the Moving Average after opening below it",
                    "The bar opens below the Moving Average after opening above it",
                    "The position opens above the Moving Average",
                    "The position opens below the Moving Average",
                };
            else if (slotType == SlotTypes.Close)
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "Exit the market at the Moving Average"
                };
            else if (slotType == SlotTypes.CloseFilter)
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "The Moving Average rises",
                    "The Moving Average falls",
                    "The bar closes below the Moving Average",
                    "The bar closes above the Moving Average",
                };
            else
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "Not Defined"
                };
            IndParam.ListParam[0].Index    = 0;
            IndParam.ListParam[0].Text     = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled  = true;
            IndParam.ListParam[0].ToolTip  = "Logic of application of the Moving Average.";

            IndParam.ListParam[1].Caption  = "Smoothing method";
            IndParam.ListParam[1].ItemList = Enum.GetNames(typeof(MAMethod));
            IndParam.ListParam[1].Index    = (int)MAMethod.Simple;
            IndParam.ListParam[1].Text     = IndParam.ListParam[1].ItemList[IndParam.ListParam[1].Index];
            IndParam.ListParam[1].Enabled  = true;
            IndParam.ListParam[1].ToolTip  = "The smoothing method of Moving Average.";

            IndParam.ListParam[2].Caption  = "Base price";
            IndParam.ListParam[2].ItemList = Enum.GetNames(typeof(BasePrice));
            IndParam.ListParam[2].Index    = (int)BasePrice.Close;
            IndParam.ListParam[2].Text     = IndParam.ListParam[2].ItemList[IndParam.ListParam[2].Index];
            IndParam.ListParam[2].Enabled  = true;
            IndParam.ListParam[2].ToolTip  = "The price the Moving Average is based on.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption   = "Period";
            IndParam.NumParam[0].Value     = 14;
            IndParam.NumParam[0].Min       = 1;
            IndParam.NumParam[0].Max       = 200;
            IndParam.NumParam[0].Enabled   = true;
            IndParam.NumParam[0].ToolTip   = "The Moving Average period.";

            IndParam.NumParam[1].Caption   = "Shift";
            IndParam.NumParam[1].Value     = 0;
            IndParam.NumParam[1].Min       = 0;
            IndParam.NumParam[1].Max       = 200;
            IndParam.NumParam[1].Enabled   = true;
            IndParam.NumParam[1].ToolTip   = "How many bars to shift with.";

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
            MAMethod  maMethod = (MAMethod )IndParam.ListParam[1].Index;
            BasePrice price    = (BasePrice)IndParam.ListParam[2].Index;
            int       iPeriod  = (int)IndParam.NumParam[0].Value;
            int       iShift   = (int)IndParam.NumParam[1].Value;
            int       iPrvs    = IndParam.CheckParam[0].Checked ? 1 : 0;

            // TimeExecution
            if (price == BasePrice.Open && iPeriod == 1 && iShift == 0)
                IndParam.ExecutionTime = ExecutionTime.AtBarOpening;

            // Calculation
            double[] adMA = MovingAverage(iPeriod, iShift, maMethod, Price(price));
            int iFirstBar = iPeriod + iShift + 1 + iPrvs;

            // Saving the components
            if (slotType == SlotTypes.Open || slotType == SlotTypes.Close)
            {
                Component = new IndicatorComp[2];

                Component[1] = new IndicatorComp();
                Component[1].Value = new double[Bars];

                for (int iBar = iFirstBar; iBar < Bars; iBar++)
                {   // Covers the cases when the price can pass through the MA without a signal
                    double dValue  = adMA[iBar - iPrvs];     // Current value
                    double dValue1 = adMA[iBar - iPrvs - 1]; // Previous value
                    double dTempVal = dValue;
                    if ((dValue1 > High[iBar - 1] && dValue < Open[iBar]) || // The Open price jumps above the indicator
                        (dValue1 < Low[iBar  - 1] && dValue > Open[iBar]) || // The Open price jumps below the indicator
                        (Close[iBar - 1] < dValue && dValue < Open[iBar]) || // The Open price is in a positive gap
                        (Close[iBar - 1] > dValue && dValue > Open[iBar]))   // The Open price is in a negative gap
                        dTempVal = Open[iBar];
                    Component[1].Value[iBar] = dTempVal; // Entry or exit value
                }
            }
            else
            {
                Component = new IndicatorComp[3];

                Component[1] = new IndicatorComp();
                Component[1].ChartType = IndChartType.NoChart;
                Component[1].FirstBar  = iFirstBar;
                Component[1].Value     = new double[Bars];

                Component[2] = new IndicatorComp();
                Component[2].ChartType = IndChartType.NoChart;
                Component[2].FirstBar  = iFirstBar;
                Component[2].Value     = new double[Bars];
            }

            Component[0] = new IndicatorComp();
            Component[0].CompName   = "MA Value";
            Component[0].DataType   = IndComponentType.IndicatorValue;
            Component[0].ChartType  = IndChartType.Line;
            Component[0].ChartColor = Color.Red;
            Component[0].FirstBar   = iFirstBar;
            Component[0].Value      = adMA;

            if (slotType == SlotTypes.Open)
            {
                Component[1].CompName = "Position opening price";
                Component[1].DataType = IndComponentType.OpenPrice;
            }
            else if (slotType == SlotTypes.OpenFilter)
            {
                Component[1].DataType = IndComponentType.AllowOpenLong;
                Component[1].CompName = "Is long entry allowed";
                Component[2].DataType = IndComponentType.AllowOpenShort;
                Component[2].CompName = "Is short entry allowed";
            }
            else if (slotType == SlotTypes.Close)
            {
                Component[1].CompName = "Position closing price";
                Component[1].DataType = IndComponentType.ClosePrice;
            }
            else if (slotType == SlotTypes.CloseFilter)
            {
                Component[1].DataType = IndComponentType.ForceCloseLong;
                Component[1].CompName = "Close out long position";
                Component[2].DataType = IndComponentType.ForceCloseShort;
                Component[2].CompName = "Close out short position";
            }

            if (slotType == SlotTypes.OpenFilter || slotType == SlotTypes.CloseFilter)
            {
                switch (IndParam.ListParam[0].Text)
                {
                    case "The Moving Average rises":
                        IndicatorRisesLogic(iFirstBar, iPrvs, adMA, ref Component[1], ref Component[2]);
                        break;

                    case "The Moving Average falls":
                        IndicatorFallsLogic(iFirstBar, iPrvs, adMA, ref Component[1], ref Component[2]);
                        break;

                    case "The bar opens above the Moving Average":
                        BarOpensAboveIndicatorLogic(iFirstBar, iPrvs, adMA, ref Component[1], ref Component[2]);
                        break;

                    case "The bar opens below the Moving Average":
                        BarOpensBelowIndicatorLogic(iFirstBar, iPrvs, adMA, ref Component[1], ref Component[2]);
                        break;

                    case "The bar opens above the Moving Average after opening below it":
                        BarOpensAboveIndicatorAfterOpeningBelowLogic(iFirstBar, iPrvs, adMA, ref Component[1], ref Component[2]);
                        break;

                    case "The bar opens below the Moving Average after opening above it":
                        BarOpensBelowIndicatorAfterOpeningAboveLogic(iFirstBar, iPrvs, adMA, ref Component[1], ref Component[2]);
                        break;

                    case "The position opens above the Moving Average":
                        Component[0].PosPriceDependence = PositionPriceDependence.BuyHigherSellLower;
                        Component[0].UsePreviousBar     = iPrvs;
                        Component[1].DataType           = IndComponentType.Other;
                        Component[1].ShowInDynInfo      = false;
                        Component[2].DataType           = IndComponentType.Other;
                        Component[2].ShowInDynInfo      = false;
                        break;

                    case "The position opens below the Moving Average":
                        Component[0].PosPriceDependence = PositionPriceDependence.BuyLowerSelHigher;
                        Component[0].UsePreviousBar     = iPrvs;
                        Component[1].DataType           = IndComponentType.Other;
                        Component[1].ShowInDynInfo      = false;
                        Component[2].DataType           = IndComponentType.Other;
                        Component[2].ShowInDynInfo      = false;
                        break;

                    case "The bar closes below the Moving Average":
                        BarClosesBelowIndicatorLogic(iFirstBar, iPrvs, adMA, ref Component[1], ref Component[2]);
                        break;

                    case "The bar closes above the Moving Average":
                        BarClosesAboveIndicatorLogic(iFirstBar, iPrvs, adMA, ref Component[1], ref Component[2]);
                        break;

                    default:
                        break;
                }
            }

            return;
        }

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {
            EntryPointLongDescription  = "at the " + ToString();
            EntryPointShortDescription = "at the " + ToString();
            ExitPointLongDescription   = "at the " + ToString();
            ExitPointShortDescription  = "at the " + ToString();

            switch (IndParam.ListParam[0].Text)
            {
                case "The Moving Average rises":
                    EntryFilterLongDescription  = "the " + ToString() + " rises";
                    EntryFilterShortDescription = "the " + ToString() + " falls";
                    ExitFilterLongDescription   = "the " + ToString() + " rises";
                    ExitFilterShortDescription  = "the " + ToString() + " falls";
                    break;

                case "The Moving Average falls":
                    EntryFilterLongDescription  = "the " + ToString() + " falls";
                    EntryFilterShortDescription = "the " + ToString() + " rises";
                    ExitFilterLongDescription   = "the " + ToString() + " falls";
                    ExitFilterShortDescription  = "the " + ToString() + " rises";
                    break;

                case "The bar opens above the Moving Average":
                    EntryFilterLongDescription  = "the bar opens above the " + ToString();
                    EntryFilterShortDescription = "the bar opens below the " + ToString();
                    break;

                case "The bar opens below the Moving Average":
                    EntryFilterLongDescription  = "the bar opens below the " + ToString();
                    EntryFilterShortDescription = "the bar opens above the " + ToString();
                    break;

                case "The position opens above the Moving Average":
                    EntryFilterLongDescription  = "the position opening price is higher than the " + ToString();
                    EntryFilterShortDescription = "the position opening price is lower than the "  + ToString();
                    break;

                case "The position opens below the Moving Average":
                    EntryFilterLongDescription  = "the position opening price is lower than the "  + ToString();
                    EntryFilterShortDescription = "the position opening price is higher than the " + ToString();
                    break;

                case "The bar opens above the Moving Average after opening below it":
                    EntryFilterLongDescription  = "the bar opens above the " + ToString() + " after opening below it";
                    EntryFilterShortDescription = "the bar opens below the " + ToString() + " after opening above it";
                    break;

                case "The bar opens below the Moving Average after opening above it":
                    EntryFilterLongDescription  = "the bar opens below the " + ToString() + " after opening above it";
                    EntryFilterShortDescription = "the bar opens above the " + ToString() + " after opening below it";
                    break;

                case "The bar closes above the Moving Average":
                    ExitFilterLongDescription  = "the bar closes above the " + ToString();
                    ExitFilterShortDescription = "the bar closes below the " + ToString();
                    break;

                case "The bar closes below the Moving Average":
                    ExitFilterLongDescription  = "the bar closes below the " + ToString();
                    ExitFilterShortDescription = "the bar closes above the " + ToString();
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
                IndParam.ListParam[1].Text         + ", " + // Method
                IndParam.ListParam[2].Text         + ", " + // Price
                IndParam.NumParam[0].ValueToString + ", " + // MA period
                IndParam.NumParam[1].ValueToString + ")";   // MA shift

            return sString;
        }
    }
}