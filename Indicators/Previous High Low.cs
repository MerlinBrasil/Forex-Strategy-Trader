// Previous High Low Indicator
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
    /// Previous High Low Indicator
    /// </summary>
    public class Previous_High_Low : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Previous_High_Low(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "Previous High Low";
            PossibleSlots = SlotTypes.Open | SlotTypes.OpenFilter | SlotTypes.Close | SlotTypes.CloseFilter;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;
            IndParam.IndicatorType = TypeOfIndicator.Additional;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption = "Logic";
            if (slotType == SlotTypes.Open)
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "Enter long at the previous high",
                    "Enter long at the previous low"
                };
            else if (slotType == SlotTypes.OpenFilter)
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "The position opens above the previous high",
                    "The position opens below the previous high",
                    "The position opens above the previous low",
                    "The position opens below the previous low"
                };
            else if (slotType == SlotTypes.Close)
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "Exit long at the previous high",
                    "Exit long at the previous low"
                };
            else if (slotType == SlotTypes.CloseFilter)
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "The bar closes above the previous high",
                    "The bar closes below the previous high",
                    "The bar closes above the previous low",
                    "The bar closes below the previous low"
                };
            else
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "Not Defined"
                };
            IndParam.ListParam[0].Index   = 0;
            IndParam.ListParam[0].Text    = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Logic of application of the indicator.";

            IndParam.ListParam[1].Caption  = "Base price";
            IndParam.ListParam[1].ItemList = new string[] { "High and Low" };
            IndParam.ListParam[1].Index    = 0;
            IndParam.ListParam[1].Text     = IndParam.ListParam[1].ItemList[IndParam.ListParam[1].Index];
            IndParam.ListParam[1].Enabled  = true;
            IndParam.ListParam[1].ToolTip  = "Used price from the indicator.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "Vertical shift";
            IndParam.NumParam[0].Value   = 0;
            IndParam.NumParam[0].Min     = -2000;
            IndParam.NumParam[0].Max     = +2000;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "A vertical shift above the high and below the low price.";

            return;
        }

        /// <summary>
        /// Calculates the indicator's components
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            double dShift = IndParam.NumParam[0].Value * Point;

            // Calculation
            double[] adHighPrice = new double[Bars];
            double[] adLowPrice  = new double[Bars];

            int iFirstBar = 2;

            for (int iBar = iFirstBar; iBar < Bars; iBar++)
            {
                adHighPrice[iBar] = High[iBar - 1];
                adLowPrice[iBar]  = Low[iBar - 1];
            }

            double[] adUpperBand = new double[Bars];
            double[] adLowerBand = new double[Bars];
            for (int iBar = iFirstBar; iBar < Bars; iBar++)
            {
                adUpperBand[iBar] = adHighPrice[iBar] + dShift;
                adLowerBand[iBar] = adLowPrice[iBar]  - dShift;
            }

            // Saving the components
            Component = new IndicatorComp[4];

            Component[0] = new IndicatorComp();
            Component[0].CompName   = "Previous High";
            Component[0].DataType   = IndComponentType.IndicatorValue;
            Component[0].ChartType  = IndChartType.Level;
            Component[0].ChartColor = Color.DarkGreen;
            Component[0].FirstBar   = iFirstBar;
            Component[0].Value      = adHighPrice;

            Component[1] = new IndicatorComp();
            Component[1].CompName   = "Previous Low";
            Component[1].DataType   = IndComponentType.IndicatorValue;
            Component[1].ChartType  = IndChartType.Level;
            Component[1].ChartColor = Color.DarkRed;
            Component[1].FirstBar   = iFirstBar;
            Component[1].Value      = adLowPrice;

            Component[2] = new IndicatorComp();
            Component[2].ChartType  = IndChartType.NoChart;
            Component[2].FirstBar   = iFirstBar;
            Component[2].Value      = new double[Bars];

            Component[3] = new IndicatorComp();
            Component[3].ChartType  = IndChartType.NoChart;
            Component[3].FirstBar   = iFirstBar;
            Component[3].Value      = new double[Bars];

            // Sets the Component's type
            if (slotType == SlotTypes.Open)
            {
                Component[2].CompName = "Long position entry price";
                Component[2].DataType = IndComponentType.OpenLongPrice;
                Component[3].CompName = "Short position entry price";
                Component[3].DataType = IndComponentType.OpenShortPrice;
            }
            else if (slotType == SlotTypes.OpenFilter)
            {
                Component[2].CompName = "Is long entry allowed";
                Component[2].DataType = IndComponentType.AllowOpenLong;
                Component[3].CompName = "Is short entry allowed";
                Component[3].DataType = IndComponentType.AllowOpenShort;
            }
            else if (slotType == SlotTypes.Close)
            {
                Component[2].CompName = "Long position closing price";
                Component[2].DataType = IndComponentType.CloseLongPrice;
                Component[3].CompName = "Short position closing price";
                Component[3].DataType = IndComponentType.CloseShortPrice;
            }
            else if (slotType == SlotTypes.CloseFilter)
            {
                Component[2].CompName = "Close out long position";
                Component[2].DataType = IndComponentType.ForceCloseLong;
                Component[3].CompName = "Close out short position";
                Component[3].DataType = IndComponentType.ForceCloseShort;
            }

            switch (IndParam.ListParam[0].Text)
            {
                case "Enter long at the previous high":
                case "Exit long at the previous high":
                    Component[2].Value = adUpperBand;
                    Component[3].Value = adLowerBand;
                    break;
                case "Enter long at the previous low":
                case "Exit long at the previous low":
                    Component[2].Value = adLowerBand;
                    Component[3].Value = adUpperBand;
                    break;
                case "The bar opens below the previous high":
                    BandIndicatorLogic(iFirstBar, 0, adLowerBand, adUpperBand, ref Component[2], ref Component[3], BandIndLogic.The_bar_opens_below_the_Upper_Band);
                    break;
                case "The bar opens above the previous high":
                    BandIndicatorLogic(iFirstBar, 0, adLowerBand, adUpperBand, ref Component[2], ref Component[3], BandIndLogic.The_bar_opens_above_the_Upper_Band);
                    break;
                case "The bar opens below the previous low":
                    BandIndicatorLogic(iFirstBar, 0, adLowerBand, adUpperBand, ref Component[2], ref Component[3], BandIndLogic.The_bar_opens_below_the_Lower_Band);
                    break;
                case "The bar opens above the previous low":
                    BandIndicatorLogic(iFirstBar, 0, adLowerBand, adUpperBand, ref Component[2], ref Component[3], BandIndLogic.The_bar_opens_above_the_Lower_Band);
                    break;
                case "The bar closes below the previous high":
                    BandIndicatorLogic(iFirstBar, 0, adLowerBand, adUpperBand, ref Component[2], ref Component[3], BandIndLogic.The_bar_closes_below_the_Upper_Band);
                    break;
                case "The bar closes above the previous high":
                    BandIndicatorLogic(iFirstBar, 0, adLowerBand, adUpperBand, ref Component[2], ref Component[3], BandIndLogic.The_bar_closes_above_the_Upper_Band);
                    break;
                case "The bar closes below the previous low":
                    BandIndicatorLogic(iFirstBar, 0, adLowerBand, adUpperBand, ref Component[2], ref Component[3], BandIndLogic.The_bar_closes_below_the_Lower_Band);
                    break;
                case "The bar closes above the previous low":
                    BandIndicatorLogic(iFirstBar, 0, adLowerBand, adUpperBand, ref Component[2], ref Component[3], BandIndLogic.The_bar_closes_above_the_Lower_Band);
                    break;
                case "The position opens above the previous high":
                    Component[0].DataType = IndComponentType.Other;
                    Component[1].DataType = IndComponentType.Other;
                    Component[2].CompName = "Shifted previous high";
                    Component[2].DataType = IndComponentType.Other;
                    Component[2].PosPriceDependence = PositionPriceDependence.PriceBuyHigher;
                    Component[3].CompName = "Shifted previous low";
                    Component[3].DataType = IndComponentType.Other;
                    Component[3].PosPriceDependence = PositionPriceDependence.PriceSellLower;
                    Component[2].Value = adUpperBand;
                    Component[3].Value = adLowerBand;
                    break;
                case "The position opens below the previous high":
                    Component[0].DataType = IndComponentType.Other;
                    Component[1].DataType = IndComponentType.Other;
                    Component[2].CompName = "Shifted previous high";
                    Component[2].DataType = IndComponentType.Other;
                    Component[2].PosPriceDependence = PositionPriceDependence.PriceBuyLower;
                    Component[3].CompName = "Shifted previous low";
                    Component[3].DataType = IndComponentType.Other;
                    Component[3].PosPriceDependence = PositionPriceDependence.PriceSellHigher;
                    Component[2].Value = adUpperBand;
                    Component[3].Value = adLowerBand;
                    break;
                case "The position opens above the previous low":
                    Component[0].DataType = IndComponentType.Other;
                    Component[1].DataType = IndComponentType.Other;
                    Component[2].CompName = "Shifted previous low";
                    Component[2].DataType = IndComponentType.Other;
                    Component[2].PosPriceDependence = PositionPriceDependence.PriceBuyHigher;
                    Component[3].CompName = "Shifted previous high";
                    Component[3].DataType = IndComponentType.Other;
                    Component[3].PosPriceDependence = PositionPriceDependence.PriceSellLower;
                    Component[2].Value = adLowerBand;
                    Component[3].Value = adUpperBand;
                    break;
                case "The position opens below the previous low":
                    Component[0].DataType = IndComponentType.Other;
                    Component[1].DataType = IndComponentType.Other;
                    Component[2].CompName = "Shifted previous low";
                    Component[2].DataType = IndComponentType.Other;
                    Component[2].PosPriceDependence = PositionPriceDependence.PriceBuyLower;
                    Component[3].CompName = "Shifted previous high";
                    Component[3].DataType = IndComponentType.Other;
                    Component[3].PosPriceDependence = PositionPriceDependence.PriceSellHigher;
                    Component[2].Value = adLowerBand;
                    Component[3].Value = adUpperBand;
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
            int iShift = (int)IndParam.NumParam[0].Value;

            string sUpperTrade;
            string sLowerTrade;
            
            if (iShift > 0)
            {
                sUpperTrade = iShift + " pips above the ";
                sLowerTrade = iShift + " pips below the ";
            }
            else if (iShift == 0)
            {
                if (IndParam.ListParam[0].Text == "Enter long at the previous high" ||
                    IndParam.ListParam[0].Text == "Enter long at the previous low"  ||
                    IndParam.ListParam[0].Text == "Exit long at the previous high"  ||
                    IndParam.ListParam[0].Text == "Exit long at the previous low")
                {
                    sUpperTrade = "at the ";
                    sLowerTrade = "at the ";
                }
                else
                {
                    sUpperTrade = "the ";
                    sLowerTrade = "the ";
                }
            }
            else
            {
                sUpperTrade = -iShift + " pips below the ";
                sLowerTrade = -iShift + " pips above the ";
            }

            switch (IndParam.ListParam[0].Text)
            {
                case "Enter long at the previous high":
                    EntryPointLongDescription  = sUpperTrade + "previous high";
                    EntryPointShortDescription = sLowerTrade + "previous low";
                    break;
                case "Enter long at the previous low":
                    EntryPointLongDescription  = sLowerTrade + "previous low";
                    EntryPointShortDescription = sUpperTrade + "previous high";
                    break;
                case "Exit long at the previous high":
                    ExitPointLongDescription  = sUpperTrade + "previous high";
                    ExitPointShortDescription = sLowerTrade + "previous low ";
                    break;
                case "Exit long at the previous low":
                    ExitPointLongDescription  = sLowerTrade + "previous low";
                    ExitPointShortDescription = sUpperTrade + "previous high";
                    break;

                case "The position opens below the previous high":
                    EntryFilterLongDescription  = "the position opens lower than "  + sUpperTrade + "previous high";
                    EntryFilterShortDescription = "the position opens higher than " + sLowerTrade + "previous low";
                    break;
                case "The position opens above the previous high":
                    EntryFilterLongDescription  = "the position opens higher than " + sUpperTrade + "previous high";
                    EntryFilterShortDescription = "the position opens lower than "  + sLowerTrade + "previous low";
                    break;
                case "The position opens below the previous low":
                    EntryFilterLongDescription  = "the position opens lower than "  + sLowerTrade + "previous low";
                    EntryFilterShortDescription = "the position opens higher than " + sUpperTrade + "previous high";
                    break;
                case "The position opens above the previous low":
                    EntryFilterLongDescription  = "the position opens higher than " + sLowerTrade + "previous low";
                    EntryFilterShortDescription = "the position opens lower than "  + sUpperTrade + "previous high";
                    break;

                case "The bar closes below the previous high":
                    ExitFilterLongDescription  = "the bar closes lower than "  + sUpperTrade + "previous high";
                    ExitFilterShortDescription = "the bar closes higher than " + sLowerTrade + "previous low ";
                    break;
                case "The bar closes above the previous high":
                    ExitFilterLongDescription  = "the bar closes higher than " + sUpperTrade + "previous high";
                    ExitFilterShortDescription = "the bar closes lower than "  + sLowerTrade + "previous low";
                    break;
                case "The bar closes below the previous low":
                    ExitFilterLongDescription  = "the bar closes lower than "  + sLowerTrade + "previous low";
                    ExitFilterShortDescription = "the bar closes higher than " + sUpperTrade + "previous high";
                    break;
                case "The bar closes above the previous low":
                    ExitFilterLongDescription  = "the bar closes higher than " + sLowerTrade + "previous low ";
                    ExitFilterShortDescription = "the bar closes lower than "  + sUpperTrade + "previous high";
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
            string sString = IndicatorName + "(" +
                IndParam.NumParam[0].ValueToString + ")";  // Shift in Pips

            return sString;
        }
    }
}
