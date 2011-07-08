// Vidya Moving Average Indicator
// Last changed on 2010-01-28
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// This code or any part of it cannot be used in other applications without a permission.
// Copyright (c) 2006 - 2010 Miroslav Popov - All rights reserved.

using System;
using System.Drawing;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Vidya Moving Average Indicator 
    /// </summary>
    public class Moving_Average : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Moving_Average(SlotTypes slotType)
        {
            // General properties
            IndicatorName   = "Vidya Moving Average";
            PossibleSlots   = SlotTypes.Open | SlotTypes.OpenFilter | SlotTypes.Close | SlotTypes.CloseFilter;
			CustomIndicator = true;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption = "Logic";
            if (slotType == SlotTypes.Open)
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "Enter the market at the Vidya Moving Average"
                };
            else if (slotType == SlotTypes.OpenFilter)
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "The Vidya Moving Average rises",
                    "The Vidya Moving Average falls",
                    "The bar opens above the Vidya Moving Average",
                    "The bar opens below the Vidya Moving Average",
                    "The bar opens above the Vidya Moving Average after opening below it",
                    "The bar opens below the Vidya Moving Average after opening above it",
                    "The position opens above the Vidya Moving Average",
                    "The position opens below the Vidya Moving Average",
                };
            else if (slotType == SlotTypes.Close)
				IndParam.ListParam[0].ItemList = new string[]
				{
					"Exit the market at the Vidya Moving Average"
				};
            else if (slotType == SlotTypes.CloseFilter)
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "The Vidya Moving Average rises",
                    "The Vidya Moving Average falls",
                    "The bar closes below the Vidya Moving Average",
                    "The bar closes above the Vidya Moving Average",
                };
            else
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "Not Defined"
                };
            IndParam.ListParam[0].Index    = 0;
            IndParam.ListParam[0].Text     = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled  = true;
            IndParam.ListParam[0].ToolTip  = "Logic of application of the Vidya Moving Average.";

            IndParam.ListParam[2].Caption  = "Base price";
            IndParam.ListParam[2].ItemList = Enum.GetNames(typeof(BasePrice));
            IndParam.ListParam[2].Index    = (int)BasePrice.Close;
            IndParam.ListParam[2].Text     = IndParam.ListParam[2].ItemList[IndParam.ListParam[2].Index];
            IndParam.ListParam[2].Enabled  = true;
            IndParam.ListParam[2].ToolTip  = "The price the Vidya Moving Average is based on.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption   = "Period";
            IndParam.NumParam[0].Value     = 21;
            IndParam.NumParam[0].Min       = 1;
            IndParam.NumParam[0].Max       = 200;
            IndParam.NumParam[0].Enabled   = true;
            IndParam.NumParam[0].ToolTip   = "The Vidya Moving Average period.";

            IndParam.NumParam[1].Caption   = "Smooth";
            IndParam.NumParam[1].Value     = 5;
            IndParam.NumParam[1].Min       = 1;
            IndParam.NumParam[1].Max       = 200;
            IndParam.NumParam[1].Enabled   = true;
            IndParam.NumParam[1].ToolTip   = "Smoothing period of Vidya.";

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
            int iPeriod  = (int)IndParam.NumParam[0].Value;
            int iSmooth  = (int)IndParam.NumParam[1].Value;
            int iPrvs    = IndParam.CheckParam[0].Checked ? 1 : 0;

            // Calculation
            double[] adBasePrice = Price(basePrice);
            double[] adMA = new double[Bars];
            int iFirstBar = iPeriod + iSmooth + 1 + iPrvs;

			// Calculating Chande Momentum Oscillator
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
			
			double SC = 2.0 / (iSmooth + 1);
			
			for (int iBar = 0; iBar < iPeriod; iBar++)
				adMA[iBar] = adBasePrice[iBar];

            for (int iBar = iPeriod; iBar < Bars; iBar++)
			{
				double dAbsCMO = Math.Abs(adCMO[iBar]) / 100;
				adMA[iBar] = SC * dAbsCMO * adBasePrice[iBar] + (1 - SC * dAbsCMO) * adMA[iBar - 1];
			}

            // Saving the components
            if (slotType == SlotTypes.Open || slotType == SlotTypes.Close)
            {
                Component = new IndicatorComp[2];

                Component[1] = new IndicatorComp();
                Component[1].Value = new double[Bars];
                
                for (int iBar = 2; iBar < Bars; iBar++)
                {   // Covers the cases when the price can pass through the MA without a signal
                    double dValue   = adMA[iBar - iPrvs];     // Current value
                    double dValue1  = adMA[iBar - iPrvs - 1]; // Previous value
                    double dTempVal = dValue;
                    if ((dValue1 > High[iBar - 1] && dValue < Open[iBar]) || // It jumps below the current bar
                        (dValue1 < Low[iBar - 1]  && dValue > Open[iBar]) || // It jumps above the current bar
                        (Close[iBar - 1] < dValue && dValue < Open[iBar]) || // Positive gap
                        (Close[iBar - 1] > dValue && dValue > Open[iBar]))   // Negative gap
                        dTempVal = Open[iBar];
                    Component[1].Value[iBar] = dTempVal;
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
                    case "The Vidya Moving Average rises":
                        IndicatorRisesLogic(iFirstBar, iPrvs, adMA, ref Component[1], ref Component[2]);
                        break;

                    case "The Vidya Moving Average falls":
                        IndicatorFallsLogic(iFirstBar, iPrvs, adMA, ref Component[1], ref Component[2]);
                        break;

                    case "The bar opens above the Vidya Moving Average":
                        BarOpensAboveIndicatorLogic(iFirstBar, iPrvs, adMA, ref Component[1], ref Component[2]);
                        break;

                    case "The bar opens below the Vidya Moving Average":
                        BarOpensBelowIndicatorLogic(iFirstBar, iPrvs, adMA, ref Component[1], ref Component[2]);
                        break;

                    case "The bar opens above the Vidya Moving Average after opening below it":
                        BarOpensAboveIndicatorAfterOpeningBelowLogic(iFirstBar, iPrvs, adMA, ref Component[1], ref Component[2]);
                        break;

                    case "The bar opens below the Vidya Moving Average after opening above it":
                        BarOpensBelowIndicatorAfterOpeningAboveLogic(iFirstBar, iPrvs, adMA, ref Component[1], ref Component[2]);
                        break;

                    case "The position opens above the Vidya Moving Average":
                        Component[0].PosPriceDependence = PositionPriceDependence.BuyHigherSellLower;
                        Component[0].UsePreviousBar     = iPrvs;
                        Component[1].DataType           = IndComponentType.Other;
                        Component[1].ShowInDynInfo      = false;
                        Component[2].DataType           = IndComponentType.Other;
                        Component[2].ShowInDynInfo      = false;
                        break;

                    case "The position opens below the Vidya Moving Average":
                        Component[0].PosPriceDependence = PositionPriceDependence.BuyLowerSelHigher;
                        Component[0].UsePreviousBar     = iPrvs;
                        Component[1].DataType           = IndComponentType.Other;
                        Component[1].ShowInDynInfo      = false;
                        Component[2].DataType           = IndComponentType.Other;
                        Component[2].ShowInDynInfo      = false;
                        break;

                    case "The bar closes below the Vidya Moving Average":
                        BarClosesBelowIndicatorLogic(iFirstBar, iPrvs, adMA, ref Component[1], ref Component[2]);
                        break;

                    case "The bar closes above the Vidya Moving Average":
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
                case "The Vidya Moving Average rises":
                    EntryFilterLongDescription  = "the " + ToString() + " rises";
                    EntryFilterShortDescription = "the " + ToString() + " falls";
                    ExitFilterLongDescription   = "the " + ToString() + " rises";
                    ExitFilterShortDescription  = "the " + ToString() + " falls";
                    break;

                case "The Vidya Moving Average falls":
                    EntryFilterLongDescription  = "the " + ToString() + " falls";
                    EntryFilterShortDescription = "the " + ToString() + " rises";
                    ExitFilterLongDescription   = "the " + ToString() + " falls";
                    ExitFilterShortDescription  = "the " + ToString() + " rises";
                    break;

                case "The bar opens above the Vidya Moving Average":
                    EntryFilterLongDescription  = "the bar opens above the " + ToString();
                    EntryFilterShortDescription = "the bar opens below the " + ToString();
                    break;

                case "The bar opens below the Vidya Moving Average":
                    EntryFilterLongDescription  = "the bar opens below the " + ToString();
                    EntryFilterShortDescription = "the bar opens above the " + ToString();
                    break;

                case "The position opens above the Vidya Moving Average":
                    EntryFilterLongDescription  = "the position opening price is higher than the " + ToString();
                    EntryFilterShortDescription = "the position opening price is lower than the "  + ToString();
                    break;

                case "The position opens below the Vidya Moving Average":
                    EntryFilterLongDescription  = "the position opening price is lower than the "  + ToString();
                    EntryFilterShortDescription = "the position opening price is higher than the " + ToString();
                    break;

                case "The bar opens above the Vidya Moving Average after opening below it":
                    EntryFilterLongDescription  = "the bar opens above the " + ToString() + " after opening below it";
                    EntryFilterShortDescription = "the bar opens below the " + ToString() + " after opening above it";
                    break;

                case "The bar opens below the Vidya Moving Average after opening above it":
                    EntryFilterLongDescription  = "the bar opens below the " + ToString() + " after opening above it";
                    EntryFilterShortDescription = "the bar opens above the " + ToString() + " after opening below it";
                    break;

                case "The bar closes above the Vidya Moving Average":
                    ExitFilterLongDescription  = "the bar closes above the " + ToString();
                    ExitFilterShortDescription = "the bar closes below the " + ToString();
                    break;

                case "The bar closes below the Vidya Moving Average":
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
                IndParam.ListParam[2].Text         + ", " + // Price
                IndParam.NumParam[0].ValueToString + ", " + // MA period
                IndParam.NumParam[1].ValueToString + ")";   // MA smooth

            return sString;
        }
    }
}