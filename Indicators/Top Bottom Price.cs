// Top Bottom Price Indicator
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
    /// Top Bottom Price Indicator
    /// </summary>
    public class Top_Bottom_Price : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Top_Bottom_Price(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "Top Bottom Price";
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
                    "Enter long at the top price",
                    "Enter long at the bottom price"
                };
            else if (slotType == SlotTypes.OpenFilter)
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "The bar opens below the top price",
                    "The bar opens above the top price",
                    "The bar opens below the bottom price",
                    "The bar opens above the bottom price",
                    "The position opens below the top price",
                    "The position opens above the top price",
                    "The position opens below the bottom price",
                    "The position opens above the bottom price"
                };
            else if (slotType == SlotTypes.Close)
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "Exit long at the top price",
                    "Exit long at the bottom price"
                };
            else if (slotType == SlotTypes.CloseFilter)
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "The bar closes below the top price",
                    "The bar closes above the top price",
                    "The bar closes below the bottom price",
                    "The bar closes above the bottom price"
                };
            else
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "Not Defined"
                };
            IndParam.ListParam[0].Index    = 0;
            IndParam.ListParam[0].Text     = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled  = true;
            IndParam.ListParam[0].ToolTip  = "Logic of application of the indicator.";

            IndParam.ListParam[1].Caption  = "Base price";
            IndParam.ListParam[1].ItemList = new string[] { "High & Low" };
            IndParam.ListParam[1].Index    = 0;
            IndParam.ListParam[1].Text     = IndParam.ListParam[1].ItemList[IndParam.ListParam[1].Index];
            IndParam.ListParam[1].Enabled  = true;
            IndParam.ListParam[1].ToolTip  = "Used price from the indicator.";

            IndParam.ListParam[2].Caption  = "Base period";
            IndParam.ListParam[2].ItemList = new string[] { "Previous bar", "Previous day", "Previous week", "Previous month" };
            IndParam.ListParam[2].Index    = 1;
            IndParam.ListParam[2].Text     = IndParam.ListParam[2].ItemList[IndParam.ListParam[2].Index];
            IndParam.ListParam[2].Enabled  = true;
            IndParam.ListParam[2].ToolTip  = "The period, the top/bottom prices are based on.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "Vertical shift";
            IndParam.NumParam[0].Value   = 0;
            IndParam.NumParam[0].Min     = -2000;
            IndParam.NumParam[0].Max     = +2000;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "A vertical shift above the top and below the bottom price.";

            return;
        }

        /// <summary>
        /// Checks if the period have been changed
        /// </summary>
        bool IsPeriodChanged(int iBar)
        {
            bool bIsPeriodChanged = false;
            switch (IndParam.ListParam[2].Index)
            {
                case 0: // Previous bar
                    bIsPeriodChanged = true;
                    break;
                case 1: // Previous day
                    bIsPeriodChanged = Time[iBar].Day != Time[iBar - 1].Day;
                    break;
                case 2: // Previous week
                    bIsPeriodChanged = Time[iBar].DayOfWeek <= DayOfWeek.Wednesday && Time[iBar - 1].DayOfWeek > DayOfWeek.Wednesday;
                    break;
                case 3: // Previous month
                    bIsPeriodChanged = Time[iBar].Month != Time[iBar - 1].Month;
                    break;
                default:
                    break;
            }

            return bIsPeriodChanged;
        }

        /// <summary>
        /// Calculates the indicator's components
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            // Reading the parameters
            double dShift = IndParam.NumParam[0].Value * Point;
            int   iFirstBar = 1;

            // Calculation
			double[] adTopPrice    = new double[Bars];
			double[] adBottomPrice = new double[Bars];

            adTopPrice[0]    = 0;
            adBottomPrice[0] = 0;
            
            double dTop    = double.MinValue;
            double dBottom = double.MaxValue;
            
            for (int iBar = 1; iBar < Bars; iBar++)
            {
                if (High[iBar - 1] > dTop)
                    dTop = High[iBar - 1];
                if (Low[iBar - 1] < dBottom)
                    dBottom = Low[iBar - 1];

                if (IsPeriodChanged(iBar))
                {
                    adTopPrice[iBar]    = dTop;
                    adBottomPrice[iBar] = dBottom;
                    dTop    = double.MinValue;
                    dBottom = double.MaxValue;
                }
                else
                {
                    adTopPrice[iBar]    = adTopPrice[iBar - 1];
                    adBottomPrice[iBar] = adBottomPrice[iBar - 1];
                }
            }

            double[] adUpperBand = new double[Bars];
            double[] adLowerBand = new double[Bars];
            for (int iBar = iFirstBar; iBar < Bars; iBar++)
            {
                adUpperBand[iBar] = adTopPrice[iBar]    + dShift;
                adLowerBand[iBar] = adBottomPrice[iBar] - dShift;
            }

            // Saving the components
            Component = new IndicatorComp[4];

            Component[0] = new IndicatorComp();
            Component[0].CompName   = "Top price";
            Component[0].DataType   = IndComponentType.IndicatorValue;
            Component[0].ChartType  = IndChartType.Level;
            Component[0].ChartColor = Color.DarkGreen;
            Component[0].FirstBar   = iFirstBar;
            Component[0].Value      = adTopPrice;

            Component[1] = new IndicatorComp();
            Component[1].CompName   = "Bottom price";
            Component[1].DataType   = IndComponentType.IndicatorValue;
            Component[1].ChartType  = IndChartType.Level;
            Component[1].ChartColor = Color.DarkRed;
            Component[1].FirstBar   = iFirstBar;
            Component[1].Value      = adBottomPrice;

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
                case "Enter long at the top price":
                case "Exit long at the top price":
                    Component[2].Value = adUpperBand;
                    Component[3].Value = adLowerBand;
                    break;
                case "Enter long at the bottom price":
                case "Exit long at the bottom price":
                    Component[2].Value = adLowerBand;
                    Component[3].Value = adUpperBand;
                    break;
                case "The bar opens below the top price":
                    BandIndicatorLogic(iFirstBar, 0, adUpperBand, adLowerBand, ref Component[2], ref Component[3], BandIndLogic.The_bar_opens_below_the_Upper_Band);
                    break;
                case "The bar opens above the top price":
                    BandIndicatorLogic(iFirstBar, 0, adUpperBand, adLowerBand, ref Component[2], ref Component[3], BandIndLogic.The_bar_opens_above_the_Upper_Band);
                    break;
                case "The bar opens below the bottom price":
                    BandIndicatorLogic(iFirstBar, 0, adUpperBand, adLowerBand, ref Component[2], ref Component[3], BandIndLogic.The_bar_opens_below_the_Lower_Band);
                    break;
                case "The bar opens above the bottom price":
                    BandIndicatorLogic(iFirstBar, 0, adUpperBand, adLowerBand, ref Component[2], ref Component[3], BandIndLogic.The_bar_opens_above_the_Lower_Band);
                    break;
                case "The bar closes below the top price":
                    BandIndicatorLogic(iFirstBar, 0, adUpperBand, adLowerBand, ref Component[2], ref Component[3], BandIndLogic.The_bar_opens_below_the_Upper_Band);
                    break;
                case "The bar closes above the top price":
                    BandIndicatorLogic(iFirstBar, 0, adUpperBand, adLowerBand, ref Component[2], ref Component[3], BandIndLogic.The_bar_closes_above_the_Upper_Band);
                    break;
                case "The bar closes below the bottom price":
                    BandIndicatorLogic(iFirstBar, 0, adUpperBand, adLowerBand, ref Component[2], ref Component[3], BandIndLogic.The_bar_closes_below_the_Lower_Band);
                    break;
                case "The bar closes above the bottom price":
                    BandIndicatorLogic(iFirstBar, 0, adUpperBand, adLowerBand, ref Component[2], ref Component[3], BandIndLogic.The_bar_closes_above_the_Lower_Band);
                    break;
                case "The position opens above the top price":
                    Component[0].DataType = IndComponentType.Other;
                    Component[1].DataType = IndComponentType.Other;
                    Component[2].CompName = "Shifted top price";
                    Component[2].DataType = IndComponentType.OpenLongPrice;
                    Component[2].PosPriceDependence = PositionPriceDependence.PriceBuyHigher;
                    Component[3].CompName = "Shifted bottom price";
                    Component[3].DataType = IndComponentType.OpenShortPrice;
                    Component[3].PosPriceDependence = PositionPriceDependence.PriceSellLower;
                    Component[2].Value = adUpperBand;
                    Component[3].Value = adLowerBand;
                    break;
                case "The position opens below the top price":
                    Component[0].DataType = IndComponentType.Other;
                    Component[1].DataType = IndComponentType.Other;
                    Component[2].CompName = "Shifted top price";
                    Component[2].DataType = IndComponentType.OpenLongPrice;
                    Component[2].PosPriceDependence = PositionPriceDependence.PriceBuyLower;
                    Component[3].CompName = "Shifted bottom price";
                    Component[3].DataType = IndComponentType.OpenShortPrice;
                    Component[3].PosPriceDependence = PositionPriceDependence.PriceSellHigher;
                    Component[2].Value = adUpperBand;
                    Component[3].Value = adLowerBand;
                    break;
                case "The position opens above the bottom price":
                    Component[0].DataType = IndComponentType.Other;
                    Component[1].DataType = IndComponentType.Other;
                    Component[2].CompName = "Shifted bottom price";
                    Component[2].DataType = IndComponentType.OpenLongPrice;
                    Component[2].PosPriceDependence = PositionPriceDependence.PriceBuyHigher;
                    Component[3].CompName = "Shifted top price";
                    Component[3].DataType = IndComponentType.OpenShortPrice;
                    Component[3].PosPriceDependence = PositionPriceDependence.PriceSellLower;
                    Component[2].Value = adLowerBand;
                    Component[3].Value = adUpperBand;
                    break;
                case "The position opens below the bottom price":
                    Component[0].DataType = IndComponentType.Other;
                    Component[1].DataType = IndComponentType.Other;
                    Component[2].CompName = "Shifted bottom price";
                    Component[2].DataType = IndComponentType.OpenLongPrice;
                    Component[2].PosPriceDependence = PositionPriceDependence.PriceBuyLower;
                    Component[3].CompName = "Shifted top price";
                    Component[3].DataType = IndComponentType.OpenShortPrice;
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
                if (IndParam.ListParam[0].Text == "Enter long at the top price"    ||
                    IndParam.ListParam[0].Text == "Enter long at the bottom price" ||
                    IndParam.ListParam[0].Text == "Exit long at the top price"     ||
                    IndParam.ListParam[0].Text == "Exit long at the bottom price")
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

            string sPeriod = "of the " + IndParam.ListParam[2].Text.ToLower();
            switch (IndParam.ListParam[0].Text)
            {
                case "Enter long at the top price":
                    EntryPointLongDescription  = sUpperTrade + "top price "    + sPeriod;
                    EntryPointShortDescription = sLowerTrade + "bottom price " + sPeriod;
                    break;
                case "Enter long at the bottom price":
                    EntryPointLongDescription  = sLowerTrade + "bottom price " + sPeriod;
                    EntryPointShortDescription = sUpperTrade + "top price "    + sPeriod;
                    break;
                case "Exit long at the top price":
                    ExitPointLongDescription  = sUpperTrade + "top price "    + sPeriod;
                    ExitPointShortDescription = sLowerTrade + "bottom price " + sPeriod;
                    break;
                case "Exit long at the bottom price":
                    ExitPointLongDescription  = sLowerTrade + "bottom price " + sPeriod;
                    ExitPointShortDescription = sUpperTrade + "top price "    + sPeriod;
                    break;

                case "The bar opens below the top price":
                    EntryFilterLongDescription  = "the bar opens lower than "  + sUpperTrade + "top price "    + sPeriod;
                    EntryFilterShortDescription = "the bar opens higher than " + sLowerTrade + "bottom price " + sPeriod;
                    break;
                case "The bar opens above the top price":
                    EntryFilterLongDescription  = "the bar opens higher than " + sUpperTrade + "top price "    + sPeriod;
                    EntryFilterShortDescription = "the bar opens lower than "  + sLowerTrade + "bottom price " + sPeriod;
                    break;
                case "The bar opens below the bottom price":
                    EntryFilterLongDescription  = "the bar opens lower than "  + sLowerTrade + "bottom price " + sPeriod;
                    EntryFilterShortDescription = "the bar opens higher than " + sUpperTrade + "top price "    + sPeriod;
                    break;
                case "The bar opens above the bottom price":
                    EntryFilterLongDescription  = "the bar opens higher than " + sLowerTrade + "bottom price " + sPeriod;
                    EntryFilterShortDescription = "the bar opens lower than "  + sUpperTrade + "top price "    + sPeriod;
                    break;

                case "The position opens below the top price":
                    EntryFilterLongDescription  = "the position opens lower than "  + sUpperTrade + "top price "    + sPeriod;
                    EntryFilterShortDescription = "the position opens higher than " + sLowerTrade + "bottom price " + sPeriod;
                    break;
                case "The position opens above the top price":
                    EntryFilterLongDescription  = "the position opens higher than " + sUpperTrade + "top price "    + sPeriod;
                    EntryFilterShortDescription = "the position opens lower than "  + sLowerTrade + "bottom price " + sPeriod;
                    break;
                case "The position opens below the bottom price":
                    EntryFilterLongDescription  = "the position opens lower than "  + sLowerTrade + "bottom price " + sPeriod;
                    EntryFilterShortDescription = "the position opens higher than " + sUpperTrade + "top price "    + sPeriod;
                    break;
                case "The position opens above the bottom price":
                    EntryFilterLongDescription  = "the position opens higher than " + sLowerTrade + "bottom price " + sPeriod;
                    EntryFilterShortDescription = "the position opens lower than "  + sUpperTrade + "top price "    + sPeriod;
                    break;

                case "The bar closes below the top price":
                    ExitFilterLongDescription  = "the bar closes lower than "  + sUpperTrade + "top price "    + sPeriod;
                    ExitFilterShortDescription = "the bar closes higher than " + sLowerTrade + "bottom price " + sPeriod;
                    break;
                case "The bar closes above the top price":
                    ExitFilterLongDescription  = "the bar closes higher than " + sUpperTrade + "top price "    + sPeriod;
                    ExitFilterShortDescription = "the bar closes lower than "  + sLowerTrade + "bottom price " + sPeriod;
                    break;
                case "The bar closes below the bottom price":
                    ExitFilterLongDescription  = "the bar closes lower than "  + sLowerTrade + "bottom price " + sPeriod;
                    ExitFilterShortDescription = "the bar closes higher than " + sUpperTrade + "top price "    + sPeriod;
                    break;
                case "The bar closes above the bottom price":
                    ExitFilterLongDescription  = "the bar closes higher than " + sLowerTrade + "bottom price " + sPeriod;
                    ExitFilterShortDescription = "the bar closes lower than "  + sUpperTrade + "top price "    + sPeriod;
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
            string sString = IndicatorName + " (" +
                IndParam.ListParam[2].Text         + ", " + // Base period
                IndParam.NumParam[0].ValueToString + ")";   // Vertical shift

            return sString;
        }
    }
}
