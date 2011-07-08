// Previous Bar Closing Indicator
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
    /// Previous Bar Closing Indicator
    /// </summary>
    public class Previous_Bar_Closing : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Previous_Bar_Closing(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "Previous Bar Closing";
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
                    "Enter the market at the previous Bar Closing"
                };
            else if (slotType == SlotTypes.OpenFilter)
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "The bar opens above the previous Bar Closing",
                    "The bar opens below the previous Bar Closing",
                    "The position opens above the previous Bar Closing",
                    "The position opens below the previous Bar Closing"
                };
            else if (slotType == SlotTypes.Close)
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "Exit the market at the previous Bar Closing"
                };
            else if (slotType == SlotTypes.CloseFilter)
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "The bar closes above the previous Bar Closing",
                    "The bar closes below the previous Bar Closing"
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
            IndParam.ListParam[1].ItemList = new string[] { "Previous Bar Closing" };
            IndParam.ListParam[1].Index    = 0;
            IndParam.ListParam[1].Text     = IndParam.ListParam[1].ItemList[IndParam.ListParam[1].Index];
            IndParam.ListParam[1].Enabled  = true;
            IndParam.ListParam[1].ToolTip  = "Used price from the indicator.";

            return;
        }

        /// <summary>
        /// Calculates the indicator's components
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            // Calculation
            double[] adPrevBarClosing = new double[Bars];

            int iFirstBar = 1;

            for (int iBar = iFirstBar; iBar < Bars; iBar++)
            {
                adPrevBarClosing[iBar] = Close[iBar - 1];
            }

            // Saving the components
            if (slotType == SlotTypes.Open || slotType == SlotTypes.Close)
            {
                Component = new IndicatorComp[1];
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
            Component[0].DataType  = IndComponentType.IndicatorValue;
            Component[0].CompName  = "Previous Bar Closing";
            Component[0].ChartType = IndChartType.NoChart;
            Component[0].FirstBar  = iFirstBar;
            Component[0].Value     = adPrevBarClosing;

            // Sets the Component's type
            if (slotType == SlotTypes.Open)
            {
                Component[0].DataType = IndComponentType.OpenPrice;
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
                Component[0].DataType = IndComponentType.ClosePrice;
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
                    case "The bar opens below the previous Bar Closing":
                        BarOpensBelowIndicatorLogic(iFirstBar, 0, adPrevBarClosing, ref Component[1], ref Component[2]);
                        break;

                    case "The bar opens above the previous Bar Closing":
                        BarOpensAboveIndicatorLogic(iFirstBar, 0, adPrevBarClosing, ref Component[1], ref Component[2]);
                        break;

                    case "The position opens above the previous Bar Closing":
                        Component[0].PosPriceDependence = PositionPriceDependence.BuyHigherSellLower;
                        Component[1].DataType = IndComponentType.Other;
                        Component[2].DataType = IndComponentType.Other;
                        Component[1].ShowInDynInfo = false;
                        Component[2].ShowInDynInfo = false;
                        break;

                    case "The position opens below the previous Bar Closing":
                        Component[0].PosPriceDependence = PositionPriceDependence.BuyLowerSelHigher;
                        Component[1].DataType = IndComponentType.Other;
                        Component[2].DataType = IndComponentType.Other;
                        Component[1].ShowInDynInfo = false;
                        Component[2].ShowInDynInfo = false;
                        break;

                    case "The bar closes below the previous Bar Closing":
                        BarClosesBelowIndicatorLogic(iFirstBar, 0, adPrevBarClosing, ref Component[1], ref Component[2]);
                        break;

                    case "The bar closes above the previous Bar Closing":
                        BarClosesAboveIndicatorLogic(iFirstBar, 0, adPrevBarClosing, ref Component[1], ref Component[2]);
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
            switch (IndParam.ListParam[0].Text)
            {
                case "Enter the market at the previous Bar Closing":
                    EntryPointLongDescription  = "at the closing price of the previous bar";
                    EntryPointShortDescription = "at the closing price of the previous bar";
                    break;

                case "The position opens above the previous Bar Closing":
                    EntryFilterLongDescription  = "the position opens above the closing price of the previous bar";
                    EntryFilterShortDescription = "the position opens below the closing price of the previous bar";
                    break;
                case "The position opens below the previous Bar Closing":
                    EntryFilterLongDescription  = "the position opens below the closing price of the previous bar";
                    EntryFilterShortDescription = "the position opens above the closing price of the previous bar";
                    break;

                case "The bar opens above the previous Bar Closing":
                    EntryFilterLongDescription  = "the bar opens above the closing price of the previous bar";
                    EntryFilterShortDescription = "the bar opens below the closing price of the previous bar";
                    break;
                case "The bar opens below the previous Bar Closing":
                    EntryFilterLongDescription  = "the bar opens below the closing price of the previous bar";
                    EntryFilterShortDescription = "the bar opens above the closing price of the previous bar";
                    break;

                case "The bar closes above the previous Bar Closing":
                    ExitFilterLongDescription  = "the bar closes above the closing price of the previous bar";
                    ExitFilterShortDescription = "the bar closes below the closing price of the previous bar";
                    break;
                case "The bar closes below the previous Bar Closing":
                    ExitFilterLongDescription  = "the bar closes below the closing price of the previous bar";
                    ExitFilterShortDescription = "the bar closes above the closing price of the previous bar";
                    break;

                case "Exit the market at the previous Bar Closing":
                    ExitPointLongDescription  = "at the closing price of the previous bar";
                    ExitPointShortDescription = "at the closing price of the previous bar";
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
            string sString = IndicatorName;

            return sString;
        }
    }
}
