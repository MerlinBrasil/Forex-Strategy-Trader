// Hourly High Low indicator
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
    /// Hourly High Low indicator
    /// </summary>
    public class Hourly_High_Low : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Hourly_High_Low(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "Hourly High Low";
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
                    "Enter long at the hourly high",
                    "Enter long at the hourly low"
                };
            else if (slotType == SlotTypes.OpenFilter)
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "The position opens above the hourly high",
                    "The position opens below the hourly high",
                    "The position opens above the hourly low",
                    "The position opens below the hourly low"
                };
            else if (slotType == SlotTypes.Close)
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "Exit long at the hourly high",
                    "Exit long at the hourly low"
                };
            else if (slotType == SlotTypes.CloseFilter)
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "The bar closes above the hourly high",
                    "The bar closes below the hourly high",
                    "The bar closes above the hourly low",
                    "The bar closes below the hourly low"
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
            IndParam.ListParam[1].ItemList = new string[] { "High and Low" };
            IndParam.ListParam[1].Index    = 0;
            IndParam.ListParam[1].Text     = IndParam.ListParam[1].ItemList[IndParam.ListParam[1].Index];
            IndParam.ListParam[1].Enabled  = true;
            IndParam.ListParam[1].ToolTip  = "Used price from the indicator.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "Start hour (incl.)";
            IndParam.NumParam[0].Value   = 0;
            IndParam.NumParam[0].Min     = 0;
            IndParam.NumParam[0].Max     = 24;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "The starting hour of the period.";

            IndParam.NumParam[1].Caption = "Start minutes (incl.)";
            IndParam.NumParam[1].Value   = 0;
            IndParam.NumParam[1].Min     = 0;
            IndParam.NumParam[1].Max     = 59;
            IndParam.NumParam[1].Enabled = true;
            IndParam.NumParam[1].ToolTip = "The starting minutes of the period.";

            IndParam.NumParam[2].Caption = "End hour (excl.)";
            IndParam.NumParam[2].Value   = 24;
            IndParam.NumParam[2].Min     = 0;
            IndParam.NumParam[2].Max     = 24;
            IndParam.NumParam[2].Enabled = true;
            IndParam.NumParam[2].ToolTip = "The ending hour of the period.";

            IndParam.NumParam[3].Caption = "End minutes (excl.)";
            IndParam.NumParam[3].Value   = 0;
            IndParam.NumParam[3].Min     = 0;
            IndParam.NumParam[3].Max     = 59;
            IndParam.NumParam[3].Enabled = true;
            IndParam.NumParam[3].ToolTip = "The ending minutes of the period.";

            IndParam.NumParam[4].Caption = "Vertical shift";
            IndParam.NumParam[4].Value   = 0;
            IndParam.NumParam[4].Min     = -2000;
            IndParam.NumParam[4].Max     = +2000;
            IndParam.NumParam[4].Enabled = true;
            IndParam.NumParam[4].ToolTip = "A vertical shift above the high and below the low price.";

            return;
        }

        /// <summary>
        /// Calculates the indicator's components
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            int iFromHour  = (int)IndParam.NumParam[0].Value;
            int iFromMin   = (int)IndParam.NumParam[1].Value;
            int iUntilHour = (int)IndParam.NumParam[2].Value;
            int iUntilMin  = (int)IndParam.NumParam[3].Value;

            TimeSpan tsFromTime  = new TimeSpan(iFromHour,  iFromMin,  0);
            TimeSpan tsUntilTime = new TimeSpan(iUntilHour, iUntilMin, 0);  
  
            double dShift = IndParam.NumParam[4].Value * Point;
            
            int iFirstBar = 2;

            // Calculation
            double[] adHighPrice = new double[Bars];
            double[] adLowPrice  = new double[Bars];

            double dMinPrice = double.MaxValue;
            double dMaxPrice = double.MinValue;
            adHighPrice[0] = 0;
            adLowPrice[0]  = 0;

            bool bPrevPeriod = false;
            for (int iBar = 1; iBar < Bars; iBar++)
            {
                bool bPeriod = false;

                if (tsFromTime < tsUntilTime)
                    bPeriod = Time[iBar].TimeOfDay >= tsFromTime && Time[iBar].TimeOfDay < tsUntilTime;
                else if (tsFromTime > tsUntilTime)
                    bPeriod = Time[iBar].TimeOfDay >= tsFromTime || Time[iBar].TimeOfDay < tsUntilTime;
                else
                    bPeriod = true;

                if (bPeriod)
                {
                    if (dMaxPrice < High[iBar]) dMaxPrice = High[iBar];
                    if (dMinPrice > Low[iBar])  dMinPrice = Low[iBar];
                }

                if (!bPeriod && bPrevPeriod)
                {
                    adHighPrice[iBar] = dMaxPrice;
                    adLowPrice[iBar]  = dMinPrice;
                    dMaxPrice = double.MinValue;
                    dMinPrice = double.MaxValue;
                }
                else
                {
                    adHighPrice[iBar] = adHighPrice[iBar - 1];
                    adLowPrice[iBar]  = adLowPrice[iBar - 1];
                }

                bPrevPeriod = bPeriod;
            }

            // Shifting the price
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
            Component[0].CompName   = "Hourly High";
            Component[0].DataType   = IndComponentType.IndicatorValue;
            Component[0].ChartType  = IndChartType.Level;
            Component[0].ChartColor = Color.DarkGreen;
            Component[0].FirstBar   = iFirstBar;
            Component[0].Value      = adHighPrice;

            Component[1] = new IndicatorComp();
            Component[1].CompName   = "Hourly Low";
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
                case "Enter long at the hourly high":
                case "Exit long at the hourly high":
                    Component[2].Value = adUpperBand;
                    Component[3].Value = adLowerBand;
                    break;
                case "Enter long at the hourly low":
                case "Exit long at the hourly low":
                    Component[2].Value = adLowerBand;
                    Component[3].Value = adUpperBand;
                    break;
                case "The bar closes below the hourly high":
                    BandIndicatorLogic(iFirstBar, 0, adUpperBand, adLowerBand, ref Component[2], ref Component[3], BandIndLogic.The_bar_closes_below_the_Upper_Band);
                    break;
                case "The bar closes above the hourly high":
                    BandIndicatorLogic(iFirstBar, 0, adUpperBand, adLowerBand, ref Component[2], ref Component[3], BandIndLogic.The_bar_closes_above_the_Upper_Band);
                    break;
                case "The bar closes below the hourly low":
                    BandIndicatorLogic(iFirstBar, 0, adUpperBand, adLowerBand, ref Component[2], ref Component[3], BandIndLogic.The_bar_closes_below_the_Lower_Band);
                    break;
                case "The bar closes above the hourly low":
                    BandIndicatorLogic(iFirstBar, 0, adUpperBand, adLowerBand, ref Component[2], ref Component[3], BandIndLogic.The_bar_closes_above_the_Lower_Band);
                    break;
                case "The position opens above the hourly high":
                    Component[0].DataType = IndComponentType.Other;
                    Component[1].DataType = IndComponentType.Other;
                    Component[2].CompName = "Shifted hourly high";
                    Component[2].DataType = IndComponentType.Other;
                    Component[2].PosPriceDependence = PositionPriceDependence.PriceBuyHigher;
                    Component[3].CompName = "Shifted hourly low";
                    Component[3].DataType = IndComponentType.Other;
                    Component[3].PosPriceDependence = PositionPriceDependence.PriceSellLower;
                    Component[2].Value = adUpperBand;
                    Component[3].Value = adLowerBand;
                    break;
                case "The position opens below the hourly high":
                    Component[0].DataType = IndComponentType.Other;
                    Component[1].DataType = IndComponentType.Other;
                    Component[2].CompName = "Shifted hourly high";
                    Component[2].DataType = IndComponentType.Other;
                    Component[2].PosPriceDependence = PositionPriceDependence.PriceBuyLower;
                    Component[3].CompName = "Shifted hourly low";
                    Component[3].DataType = IndComponentType.Other;
                    Component[3].PosPriceDependence = PositionPriceDependence.PriceSellHigher;
                    Component[2].Value = adUpperBand;
                    Component[3].Value = adLowerBand;
                    break;
                case "The position opens above the hourly low":
                    Component[0].DataType = IndComponentType.Other;
                    Component[1].DataType = IndComponentType.Other;
                    Component[2].CompName = "Shifted hourly low";
                    Component[2].DataType = IndComponentType.Other;
                    Component[2].PosPriceDependence = PositionPriceDependence.PriceBuyHigher;
                    Component[3].CompName = "Shifted hourly high";
                    Component[3].DataType = IndComponentType.Other;
                    Component[3].PosPriceDependence = PositionPriceDependence.PriceSellLower;
                    Component[2].Value = adLowerBand;
                    Component[3].Value = adUpperBand;
                    break;
                case "The position opens below the hourly low":
                    Component[0].DataType = IndComponentType.Other;
                    Component[1].DataType = IndComponentType.Other;
                    Component[2].CompName = "Shifted hourly low";
                    Component[2].DataType = IndComponentType.Other;
                    Component[2].PosPriceDependence = PositionPriceDependence.PriceBuyLower;
                    Component[3].CompName = "Shifted hourly high";
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
            int iShift = (int)IndParam.NumParam[4].Value;

            int iFromHour  = (int)IndParam.NumParam[0].Value;
            int iFromMin   = (int)IndParam.NumParam[1].Value;
            int iUntilHour = (int)IndParam.NumParam[2].Value;
            int iUntilMin  = (int)IndParam.NumParam[3].Value;

            string sFromTime  = iFromHour.ToString("00")  + ":" + iFromMin.ToString("00");
            string sUntilTime = iUntilHour.ToString("00") + ":" + iUntilMin.ToString("00");
            string sInterval  = "(" + sFromTime + " - " + sUntilTime + ")";

            string sUpperTrade;
            string sLowerTrade;
            
            if (iShift > 0)
            {
                sUpperTrade = iShift + " pips above the ";
                sLowerTrade = iShift + " pips below the ";
            }
            else if (iShift == 0)
            {
                if (IndParam.ListParam[0].Text == "Enter long at the hourly high" ||
                    IndParam.ListParam[0].Text == "Enter long at the hourly low"  ||
                    IndParam.ListParam[0].Text == "Exit long at the hourly high"  ||
                    IndParam.ListParam[0].Text == "Exit long at the hourly low")
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
                case "Enter long at the hourly high":
                    EntryPointLongDescription  = sUpperTrade + "hourly high " + sInterval;
                    EntryPointShortDescription = sLowerTrade + "hourly low "  + sInterval;
                    break;
                case "Enter long at the hourly low":
                    EntryPointLongDescription  = sLowerTrade + "hourly low "  + sInterval;
                    EntryPointShortDescription = sUpperTrade + "hourly high " + sInterval;
                    break;
                case "Exit long at the hourly high":
                    ExitPointLongDescription  = sUpperTrade + "hourly high " + sInterval;
                    ExitPointShortDescription = sLowerTrade + "hourly low "  + sInterval;
                    break;
                case "Exit long at the hourly low":
                    ExitPointLongDescription  = sLowerTrade + "hourly low "  + sInterval;
                    ExitPointShortDescription = sUpperTrade + "hourly high " + sInterval;
                    break;

                case "The position opens below the hourly high":
                    EntryFilterLongDescription  = "the position opens lower than "  + sUpperTrade + "hourly high " + sInterval;
                    EntryFilterShortDescription = "the position opens higher than " + sLowerTrade + "hourly low "  + sInterval;
                    break;
                case "The position opens above the hourly high":
                    EntryFilterLongDescription  = "the position opens higher than " + sUpperTrade + "hourly high " + sInterval;
                    EntryFilterShortDescription = "the position opens lower than "  + sLowerTrade + "hourly low "  + sInterval;
                    break;
                case "The position opens below the hourly low":
                    EntryFilterLongDescription  = "the position opens lower than "  + sLowerTrade + "hourly low "  + sInterval;
                    EntryFilterShortDescription = "the position opens higher than " + sUpperTrade + "hourly high " + sInterval;
                    break;
                case "The position opens above the hourly low":
                    EntryFilterLongDescription  = "the position opens higher than " + sLowerTrade + "hourly low "  + sInterval;
                    EntryFilterShortDescription = "the position opens lower than "  + sUpperTrade + "hourly high " + sInterval;
                    break;

                case "The bar closes below the hourly high":
                    ExitFilterLongDescription  = "the bar closes lower than "  + sUpperTrade + "hourly high " + sInterval;
                    ExitFilterShortDescription = "the bar closes higher than " + sLowerTrade + "hourly low "  + sInterval;
                    break;
                case "The bar closes above the hourly high":
                    ExitFilterLongDescription  = "the bar closes higher than " + sUpperTrade + "hourly high " + sInterval;
                    ExitFilterShortDescription = "the bar closes lower than "  + sLowerTrade + "hourly low "  + sInterval;
                    break;
                case "The bar closes below the hourly low":
                    ExitFilterLongDescription  = "the bar closes lower than "  + sLowerTrade + "hourly low "  + sInterval;
                    ExitFilterShortDescription = "the bar closes higher than " + sUpperTrade + "hourly high " + sInterval;
                    break;
                case "The bar closes above the hourly low":
                    ExitFilterLongDescription  = "the bar closes higher than " + sLowerTrade + "hourly low "  + sInterval;
                    ExitFilterShortDescription = "the bar closes lower than "  + sUpperTrade + "hourly high " + sInterval;
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
            int iFromHour  = (int)IndParam.NumParam[0].Value;
            int iFromMin   = (int)IndParam.NumParam[1].Value;
            int iUntilHour = (int)IndParam.NumParam[2].Value;
            int iUntilMin  = (int)IndParam.NumParam[3].Value;

            string sFromTime  = iFromHour.ToString("00")  + ":" + iFromMin.ToString("00");
            string sUntilTime = iUntilHour.ToString("00") + ":" + iUntilMin.ToString("00");

            string sString = IndicatorName + " (" +
                sFromTime   + " - " + // Start time
                sUntilTime  + ", " +  // End time
                IndParam.NumParam[4].ValueToString + ")"; // Vertical shift

            return sString;
        }
    }
}
