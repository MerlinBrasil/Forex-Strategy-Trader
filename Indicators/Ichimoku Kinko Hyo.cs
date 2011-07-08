// Ichimoku Kinko Hyo indicator 
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
    /// Ichimoku Kinko Hyo indicator 
    /// </summary>
    public class Ichimoku_Kinko_Hyo : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Ichimoku_Kinko_Hyo(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "Ichimoku Kinko Hyo";
            PossibleSlots = SlotTypes.Open | SlotTypes.OpenFilter | SlotTypes.Close;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption = "Logic";
            if (slotType == SlotTypes.Open)
                IndParam.ListParam[0].ItemList = new string[] {
                    "Enter the market at the Tenkan Sen",
                    "Enter the market at the Kijun Sen",
                };
            else if (slotType == SlotTypes.OpenFilter)
                IndParam.ListParam[0].ItemList = new string[] {
                    "The Tenkan Sen rises",
                    "The Kijun Sen rises",
                    "The Tenkan Sen is higher than the Kijun Sen",
                    "The Tenkan Sen crosses the Kijun Sen upward",
                    "The bar opens above the Tenkan Sen",
                    "The bar opens above the Kijun Sen",
                    "The Chikou Span is above the closing price",
                    "The position opens above the Kumo",
                    "The position opens inside or above the Kumo",
                    "The Tenkan Sen is above the Kumo",
                    "The Tenkan Sen is inside or above the Kumo",
                    "The Kijun Sen is above the Kumo",
                    "The Kijun Sen is inside or above the Kumo",
                    "The Senkou Span A is higher than the Senkou Span B",
                    "The Senkou Span A crosses the Senkou Span B upward",
                };
            else if (slotType == SlotTypes.Close)
                IndParam.ListParam[0].ItemList = new string[] {
                    "Exit the market at the Tenkan Sen",
                    "Exit the market at the Kijun Sen",
                };
            else
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "Not Defined"
                };
            IndParam.ListParam[0].Index   = 0;
            IndParam.ListParam[0].Text    = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Application of Ichimoku Kinko Hyo.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "Tenkan";
            IndParam.NumParam[0].Value   = 9;
            IndParam.NumParam[0].Min     = 6;
            IndParam.NumParam[0].Max     = 12;
            IndParam.NumParam[0].Point   = 0;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "The Tenkan Sen period.";

            IndParam.NumParam[2].Caption = "Kijun";
            IndParam.NumParam[2].Value   = 26;
            IndParam.NumParam[2].Min     = 18;
            IndParam.NumParam[2].Max     = 34;
            IndParam.NumParam[2].Point   = 0;
            IndParam.NumParam[2].Enabled = true;
            IndParam.NumParam[2].ToolTip = "The Kijun Sen period, Chikou Span period and Senkou Span shift.";

            IndParam.NumParam[4].Caption = "Senkou Span B";
            IndParam.NumParam[4].Value   = 52;
            IndParam.NumParam[4].Min     = 36;
            IndParam.NumParam[4].Max     = 84;
            IndParam.NumParam[4].Point   = 0;
            IndParam.NumParam[4].Enabled = true;
            IndParam.NumParam[4].ToolTip = "The Senkou Span B period.";

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
            int iTenkan = (int)IndParam.NumParam[0].Value;
            int iKijun  = (int)IndParam.NumParam[2].Value;
            int iSenkou = (int)IndParam.NumParam[4].Value;
            int iPrvs   = IndParam.CheckParam[0].Checked ? 1 : 0;

            double[] adMedianPrice = Price(BasePrice.Median);

            int iFirstBar = 1 + iKijun + iSenkou;

            double[] adTenkanSen = new double[Bars];
            for (int iBar = iFirstBar; iBar < Bars; iBar++)
            {
                double dHighestHigh = double.MinValue;
                double dLowestLow   = double.MaxValue;
                for (int i = 0; i < iTenkan; i++)
                {
                    if (High[iBar - i] > dHighestHigh)
                        dHighestHigh = High[iBar - i];
                    if (Low[iBar - i] < dLowestLow)
                        dLowestLow = Low[iBar - i];
                }
                adTenkanSen[iBar] = (dHighestHigh + dLowestLow) / 2;
            }

            double[] adKijunSen = new double[Bars];
            for (int iBar = iFirstBar; iBar < Bars; iBar++)
            {
                double dHighestHigh = double.MinValue;
                double dLowestLow   = double.MaxValue;
                for (int i = 0; i < iKijun; i++)
                {
                    if (High[iBar - i] > dHighestHigh)
                        dHighestHigh = High[iBar - i];
                    if (Low[iBar - i] < dLowestLow)
                        dLowestLow = Low[iBar - i];
                }
                adKijunSen[iBar] = (dHighestHigh + dLowestLow) / 2;
            }

            double[] adChikouSpan  = new double[Bars];
            for (int iBar = 0; iBar < Bars - iKijun; iBar++)
            {
                adChikouSpan[iBar] = Close[iBar + iKijun];
            }

            double[] adSenkouSpanA  = new double[Bars];
            for (int iBar = iFirstBar; iBar < Bars - iKijun; iBar++)
            {
                adSenkouSpanA[iBar + iKijun] = (adTenkanSen[iBar] + adKijunSen[iBar]) / 2;
            }

            double[] adSenkouSpanB  = new double[Bars];
            for (int iBar = iFirstBar; iBar < Bars - iKijun; iBar++)
            {
                double dHighestHigh = double.MinValue;
                double dLowestLow   = double.MaxValue;
                for (int i = 0; i < iSenkou; i++)
                {
                    if (High[iBar - i] > dHighestHigh)
                        dHighestHigh = High[iBar - i];
                    if (Low[iBar - i] < dLowestLow)
                        dLowestLow = Low[iBar - i];
                }
                adSenkouSpanB[iBar + iKijun] = (dHighestHigh + dLowestLow) / 2;
            }

            // Saving the components
            if (slotType == SlotTypes.OpenFilter)
                Component = new IndicatorComp[7];
            else
                Component = new IndicatorComp[6];

            Component[0] = new IndicatorComp();
            Component[0].CompName   = "Tenkan Sen";
            Component[0].DataType   = IndComponentType.IndicatorValue;
            Component[0].ChartType  = IndChartType.Line;
            Component[0].ChartColor = Color.Red;
            Component[0].FirstBar   = iFirstBar;
            Component[0].Value      = adTenkanSen;

            Component[1] = new IndicatorComp();
            Component[1].CompName   = "Kijun Sen";
            Component[1].DataType   = IndComponentType.IndicatorValue;
            Component[1].ChartType  = IndChartType.Line;
            Component[1].ChartColor = Color.Blue;
            Component[1].FirstBar   = iFirstBar;
            Component[1].Value      = adKijunSen;

            Component[2] = new IndicatorComp();
            Component[2].CompName   = "Chikou Span";
            Component[2].DataType   = IndComponentType.IndicatorValue;
            Component[2].ChartType  = IndChartType.Line;
            Component[2].ChartColor = Color.Green;
            Component[2].FirstBar   = iFirstBar;
            Component[2].Value      = adChikouSpan;

            Component[3] = new IndicatorComp();
            Component[3].CompName   = "Senkou Span A";
            Component[3].DataType   = IndComponentType.IndicatorValue;
            Component[3].ChartType  = IndChartType.CloudUp;
            Component[3].ChartColor = Color.SandyBrown;
            Component[3].FirstBar   = iFirstBar;
            Component[3].Value      = adSenkouSpanA;

            Component[4] = new IndicatorComp();
            Component[4].CompName   = "Senkou Span B";
            Component[4].DataType   = IndComponentType.IndicatorValue;
            Component[4].ChartType  = IndChartType.CloudDown;
            Component[4].ChartColor = Color.Thistle;
            Component[4].FirstBar   = iFirstBar;
            Component[4].Value      = adSenkouSpanB;

            Component[5] = new IndicatorComp();
            Component[5].FirstBar = iFirstBar;
            Component[5].Value    = new double[Bars];
            Component[5].DataType = IndComponentType.Other;

            if (slotType == SlotTypes.OpenFilter)
            {
                Component[5].CompName = "Is long entry allowed";
                Component[5].DataType = IndComponentType.AllowOpenLong;

                Component[6] = new IndicatorComp();
                Component[6].FirstBar = iFirstBar;
                Component[6].Value    = new double[Bars];
                Component[6].CompName = "Is short entry allowed";
                Component[6].DataType = IndComponentType.AllowOpenShort;
            }

            switch (IndParam.ListParam[0].Text)
            {
                case "Enter the market at the Tenkan Sen":
                    Component[5].CompName  = "Tenkan Sen entry price";
                    Component[5].DataType  = IndComponentType.OpenPrice;
                    Component[5].ChartType = IndChartType.NoChart;
                    for (int iBar = iFirstBar + iPrvs; iBar < Bars; iBar++)
                    {
                        Component[5].Value[iBar] = adTenkanSen[iBar - iPrvs];
                    }
                    break;
                case "Enter the market at the Kijun Sen":
                    Component[5].CompName  = "Kijun Sen entry price";
                    Component[5].DataType  = IndComponentType.OpenPrice;
                    Component[5].ChartType = IndChartType.NoChart;
                    for (int iBar = iFirstBar + iPrvs; iBar < Bars; iBar++)
                    {
                        Component[5].Value[iBar] = adKijunSen[iBar - iPrvs];
                    }
                    break;
                case "Exit the market at the Tenkan Sen":
                    Component[5].CompName  = "Tenkan Sen exit price";
                    Component[5].DataType  = IndComponentType.ClosePrice;
                    Component[5].ChartType = IndChartType.NoChart;
                    for (int iBar = iFirstBar + iPrvs; iBar < Bars; iBar++)
                    {
                        Component[5].Value[iBar] = adTenkanSen[iBar - iPrvs];
                    }
                    break;
                case "Exit the market at the Kijun Sen":
                    Component[5].CompName  = "Kijun Sen exit price";
                    Component[5].DataType  = IndComponentType.ClosePrice;
                    Component[5].ChartType = IndChartType.NoChart;
                    for (int iBar = iFirstBar + iPrvs; iBar < Bars; iBar++)
                    {
                        Component[5].Value[iBar] = adKijunSen[iBar - iPrvs];
                    }
                    break;
                case "The Tenkan Sen rises":
                    for (int iBar = iFirstBar + iPrvs; iBar < Bars; iBar++)
                    {
                        Component[5].Value[iBar] = adTenkanSen[iBar - iPrvs] > adTenkanSen[iBar - iPrvs - 1] + Sigma() ? 1 : 0;
                        Component[6].Value[iBar] = adTenkanSen[iBar - iPrvs] < adTenkanSen[iBar - iPrvs - 1] - Sigma() ? 1 : 0;
                    }
                    break;
                case "The Kijun Sen rises":
                    for (int iBar = iFirstBar + iPrvs; iBar < Bars; iBar++)
                    {
                        Component[5].Value[iBar] = adKijunSen[iBar - iPrvs] > adKijunSen[iBar - iPrvs - 1] + Sigma() ? 1 : 0;
                        Component[6].Value[iBar] = adKijunSen[iBar - iPrvs] < adKijunSen[iBar - iPrvs - 1] - Sigma() ? 1 : 0;
                    }
                    break;
                case "The Tenkan Sen is higher than the Kijun Sen":
                    IndicatorIsHigherThanAnotherIndicatorLogic(iFirstBar, iPrvs, adTenkanSen, adKijunSen, ref Component[5], ref Component[6]);
                    break;
                case "The Tenkan Sen crosses the Kijun Sen upward":
                    IndicatorCrossesAnotherIndicatorUpwardLogic(iFirstBar, iPrvs, adTenkanSen, adKijunSen, ref Component[5], ref Component[6]);
                    break;
                case "The bar opens above the Tenkan Sen":
                    BarOpensAboveIndicatorLogic(iFirstBar, iPrvs, adTenkanSen, ref Component[5], ref Component[6]);
                    break;
                case "The bar opens above the Kijun Sen":
                    BarOpensAboveIndicatorLogic(iFirstBar, iPrvs, adKijunSen, ref Component[5], ref Component[6]);
                    break;
                case "The Chikou Span is above the closing price":
                    for (int iBar = iFirstBar + iPrvs; iBar < Bars; iBar++)
                    {
                        Component[5].Value[iBar] = adChikouSpan[iBar - iKijun - iPrvs] > Close[iBar - iKijun - iPrvs] + Sigma() ? 1 : 0;
                        Component[6].Value[iBar] = adChikouSpan[iBar - iKijun - iPrvs] < Close[iBar - iKijun - iPrvs] - Sigma() ? 1 : 0;
                    }
                    break;

                case "The position opens above the Kumo":
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        Component[5].Value[iBar] = Math.Max(adSenkouSpanA[iBar], adSenkouSpanB[iBar]);
                        Component[6].Value[iBar] = Math.Min(adSenkouSpanA[iBar], adSenkouSpanB[iBar]);
                    }
                    Component[5].PosPriceDependence = PositionPriceDependence.PriceBuyHigher;
                    Component[5].DataType = IndComponentType.Other;
                    Component[5].UsePreviousBar = iPrvs;
                    Component[5].ShowInDynInfo  = false;

                    Component[6].PosPriceDependence = PositionPriceDependence.PriceSellLower;
                    Component[6].DataType = IndComponentType.Other;
                    Component[6].UsePreviousBar = iPrvs;
                    Component[6].ShowInDynInfo  = false;
                    break;

                case "The position opens inside or above the Kumo":
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        Component[5].Value[iBar] = Math.Min(adSenkouSpanA[iBar], adSenkouSpanB[iBar]);
                        Component[6].Value[iBar] = Math.Max(adSenkouSpanA[iBar], adSenkouSpanB[iBar]);
                    }
                    Component[5].PosPriceDependence = PositionPriceDependence.PriceBuyHigher;
                    Component[5].DataType = IndComponentType.Other;
                    Component[5].UsePreviousBar = iPrvs;
                    Component[5].ShowInDynInfo  = false;

                    Component[6].PosPriceDependence = PositionPriceDependence.PriceSellLower;
                    Component[6].DataType = IndComponentType.Other;
                    Component[6].UsePreviousBar = iPrvs;
                    Component[6].ShowInDynInfo  = false;
                    break;

                case "The Tenkan Sen is above the Kumo":
                    for (int iBar = iFirstBar + iPrvs; iBar < Bars; iBar++)
                    {
                        Component[5].Value[iBar] = adTenkanSen[iBar - iPrvs] > Math.Max(adSenkouSpanA[iBar - iPrvs], adSenkouSpanB[iBar - iPrvs]) + Sigma() ? 1 : 0;
                        Component[6].Value[iBar] = adTenkanSen[iBar - iPrvs] < Math.Min(adSenkouSpanA[iBar - iPrvs], adSenkouSpanB[iBar - iPrvs]) - Sigma() ? 1 : 0;
                    }
                    break;

                case "The Tenkan Sen is inside or above the Kumo":
                    for (int iBar = iFirstBar + iPrvs; iBar < Bars; iBar++)
                    {
                        Component[5].Value[iBar] = adTenkanSen[iBar - iPrvs] > Math.Min(adSenkouSpanA[iBar - iPrvs], adSenkouSpanB[iBar - iPrvs]) + Sigma() ? 1 : 0;
                        Component[6].Value[iBar] = adTenkanSen[iBar - iPrvs] < Math.Max(adSenkouSpanA[iBar - iPrvs], adSenkouSpanB[iBar - iPrvs]) - Sigma() ? 1 : 0;
                    }
                    break;

                case "The Kijun Sen is above the Kumo":
                    for (int iBar = iFirstBar + iPrvs; iBar < Bars; iBar++)
                    {
                        Component[5].Value[iBar] = adKijunSen[iBar - iPrvs] > Math.Max(adSenkouSpanA[iBar - iPrvs], adSenkouSpanB[iBar - iPrvs]) + Sigma() ? 1 : 0;
                        Component[6].Value[iBar] = adKijunSen[iBar - iPrvs] < Math.Min(adSenkouSpanA[iBar - iPrvs], adSenkouSpanB[iBar - iPrvs]) - Sigma() ? 1 : 0;
                    }
                    break;

                case "The Kijun Sen is inside or above the Kumo":
                    for (int iBar = iFirstBar + iPrvs; iBar < Bars; iBar++)
                    {
                        Component[5].Value[iBar] = adKijunSen[iBar - iPrvs] > Math.Min(adSenkouSpanA[iBar - iPrvs], adSenkouSpanB[iBar - iPrvs]) + Sigma() ? 1 : 0;
                        Component[6].Value[iBar] = adKijunSen[iBar - iPrvs] < Math.Max(adSenkouSpanA[iBar - iPrvs], adSenkouSpanB[iBar - iPrvs]) - Sigma() ? 1 : 0;
                    }
                    break;

                case "The Senkou Span A is higher than the Senkou Span B":
                    IndicatorIsHigherThanAnotherIndicatorLogic(iFirstBar, iPrvs, adSenkouSpanA, adSenkouSpanB, ref Component[5], ref Component[6]);
                    break;

                case "The Senkou Span A crosses the Senkou Span B upward":
                    IndicatorCrossesAnotherIndicatorUpwardLogic(iFirstBar, iPrvs, adSenkouSpanA, adSenkouSpanB, ref Component[5], ref Component[6]);
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
            switch (IndParam.ListParam[0].Text)
            {
                case "Enter the market at the Tenkan Sen":
                    EntryPointLongDescription  = "at the Tenkan Sen of " + ToString();
                    EntryPointShortDescription = "at the Tenkan Sen of " + ToString();
                    break;
                case "Enter the market at the Kijun Sen":
                    EntryPointLongDescription  = "at the Kijun Sen of " + ToString();
                    EntryPointShortDescription = "at the Kijun Sen of " + ToString();
                    break;
                case "Exit the market at the Tenkan Sen":
                    ExitPointLongDescription  = "at the Tenkan Sen of " + ToString();
                    ExitPointShortDescription = "at the Tenkan Sen of " + ToString();
                    break;
                case "Exit the market at the Kijun Sen":
                    ExitPointLongDescription  = "at the Kijun Sen of " + ToString();
                    ExitPointShortDescription = "at the Kijun Sen of " + ToString();
                    break;
                case "The Tenkan Sen rises":
                    EntryFilterLongDescription  = "the Tenkan Sen of " + ToString() + " rises";
                    EntryFilterShortDescription = "the Tenkan Sen of " + ToString() + " fals";
                    break;
                case "The Kijun Sen rises":
                    EntryFilterLongDescription  = "the Kijun Sen of " + ToString() + " rises";
                    EntryFilterShortDescription = "the Kijun Sen of " + ToString() + " fals";
                    break;
                case "The Tenkan Sen is higher than the Kijun Sen":
                    EntryFilterLongDescription  = ToString() + " - the Tenkan Sen is higher than the Kijun Sen";
                    EntryFilterShortDescription = ToString() + " - the Tenkan Sen is lower than the Kijun Sen";
                    break;
                case "The Tenkan Sen crosses the Kijun Sen upward":
                    EntryFilterLongDescription  = ToString() + " - the Tenkan Sen crosses the Kijun Sen upward";
                    EntryFilterShortDescription = ToString() + " - the Tenkan Sen crosses the Kijun Sen downward";
                    break;
                case "The bar opens above the Tenkan Sen":
                    EntryFilterLongDescription  = "the bar opens above the Tenkan Sen of " + ToString();
                    EntryFilterShortDescription = "the bar opens below the Tenkan Sen of " + ToString();
                    break;
                case "The bar opens above the Kijun Sen":
                    EntryFilterLongDescription  = "the bar opens above the Kijun Sen of " + ToString();
                    EntryFilterShortDescription = "the bar opens below the Kijun Sen of " + ToString();
                    break;
                case "The Chikou Span is above the closing price":
                    EntryFilterLongDescription  = "the Chikou Span of " + ToString() + " is above the closing price of the corresponding bar";
                    EntryFilterShortDescription = "the Chikou Span of " + ToString() + " is below the closing price of the corresponding bar";
                    break;
                case "The position opens above the Kumo":
                    EntryFilterLongDescription  = "the position opens above the Kumo of " + ToString();
                    EntryFilterShortDescription = "the position opens below the Kumo of " + ToString();
                    break;
                case "The position opens inside or above the Kumo":
                    EntryFilterLongDescription  = "the position opens inside or above the Kumo of " + ToString();
                    EntryFilterShortDescription = "the position opens inside or below the Kumo of " + ToString();
                    break;
                case "The Tenkan Sen is above the Kumo":
                    EntryFilterLongDescription  = ToString() + " - the Tenkan Sen is above the Kumo";
                    EntryFilterShortDescription = ToString() + " - the Tenkan Sen is below the Kumo";
                    break;
                case "The Tenkan Sen is inside or above the Kumo":
                    EntryFilterLongDescription  = ToString() + " - the Tenkan Sen is inside or above the Kumo";
                    EntryFilterShortDescription = ToString() + " - the Tenkan Sen is inside or below the Kumo";
                    break;
                case "The Kijun Sen is above the Kumo":
                    EntryFilterLongDescription  = ToString() + " - the Kijun Sen is above the Kumo";
                    EntryFilterShortDescription = ToString() + " - the Kijun Sen is below the Kumo";
                    break;
                case "The Kijun Sen is inside or above the Kumo":
                    EntryFilterLongDescription  = ToString() + " - the Kijun Sen is inside or above the Kumo";
                    EntryFilterShortDescription = ToString() + " - the Kijun Sen is inside or below the Kumo";
                    break;
                case "The Senkou Span A is higher than the Senkou Span B":
                    EntryFilterLongDescription  = ToString() + " - Senkou Span A is higher than the Senkou Span B";
                    EntryFilterShortDescription = ToString() + " - Senkou Span A is lower than the Senkou Span B";
                    break;
                case "The Senkou Span A crosses the Senkou Span B upward":
                    EntryFilterLongDescription  = ToString() + " - Senkou Span A crosses the Senkou Span B upward";
                    EntryFilterShortDescription = ToString() + " - Senkou Span A crosses the Senkou Span B downward";
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
                IndParam.NumParam[0].ValueToString + ", " + // Tenkan
                IndParam.NumParam[2].ValueToString + ", " + // Kijun
                IndParam.NumParam[4].ValueToString + ")";   // Senkou Span B

            return sString;
        }
    }
}