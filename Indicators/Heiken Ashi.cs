// Heiken Ashi Indicator
// Last changed on 2009-05-05
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2009 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System.Drawing;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Heiken Ashi Indicator
    /// </summary>
    public class Heiken_Ashi : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Heiken_Ashi(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "Heiken Ashi";
            PossibleSlots = SlotTypes.Open | SlotTypes.OpenFilter | SlotTypes.Close | SlotTypes.CloseFilter;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;
            IndParam.IndicatorType = TypeOfIndicator.Indicator;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption = "Logic";
            if (slotType == SlotTypes.Open)
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "Enter long at the H.A. High",
                    "Enter long at the H.A. Low",
                };
            else if (slotType == SlotTypes.OpenFilter)
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "White H.A. bar without lower shadow",
                    "White H.A. bar",
                    "Black H.A. bar",
                    "Black H.A. bar without upper shadow"
                };
            else if (slotType == SlotTypes.Close)
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "Exit long at the H.A. High",
                    "Exit long at the H.A. Low",
                };
            else if (slotType == SlotTypes.CloseFilter)
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "Black H.A. bar without upper shadow",
                    "Black H.A. bar",
                    "White H.A. bar",
                    "White H.A. bar without lower shadow"
                };
            else
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "Not Defined"
                };
            IndParam.ListParam[0].Index    = 0;
            IndParam.ListParam[0].Text     = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled  = true;
            IndParam.ListParam[0].ToolTip  = "Logic of application of the Donchian Channel.";

            IndParam.ListParam[1].Caption  = "Base price";
            IndParam.ListParam[1].ItemList = new string[] { "High, Low, Open, Close" };
            IndParam.ListParam[1].Index    = 0;
            IndParam.ListParam[1].Text     = IndParam.ListParam[1].ItemList[IndParam.ListParam[1].Index];
            IndParam.ListParam[1].Enabled  = true;

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
            int iPrvs = IndParam.CheckParam[0].Checked ? 1 : 0;

            // Calculation
            double[] adHAOpen  = new double[Bars];
            double[] adHAHigh  = new double[Bars];
            double[] adHALow   = new double[Bars];
            double[] adHAClose = new double[Bars];

            adHAOpen[0]  = Open[0];
            adHAHigh[0]  = High[0];
            adHALow[0]   = Low[0];
            adHAClose[0] = Close[0];

            int iFirstBar = 1 + iPrvs;

            for (int iBar = 1; iBar < Bars; iBar++)
            {
                adHAClose[iBar] = (Open[iBar] + High[iBar] + Low[iBar] + Close[iBar]) / 4;
                adHAOpen[iBar]  = (adHAOpen[iBar - 1] + adHAClose[iBar - 1]) / 2;
                adHAHigh[iBar]  = High[iBar] > adHAOpen[iBar] ? High[iBar] : adHAOpen[iBar];
                adHAHigh[iBar]  = adHAClose[iBar] > adHAHigh[iBar] ? adHAClose[iBar] : adHAHigh[iBar];
                adHALow[iBar]   = Low[iBar] < adHAOpen[iBar] ? Low[iBar] : adHAOpen[iBar];
                adHALow[iBar]   = adHAClose[iBar] < adHALow[iBar] ? adHAClose[iBar] : adHALow[iBar];
            }

            // Saving the components
            Component = new IndicatorComp[6];

            Component[0] = new IndicatorComp();
            Component[0].CompName   = "H.A. Open";
            Component[0].DataType   = IndComponentType.IndicatorValue;
            Component[0].ChartType  = IndChartType.Dot;
            Component[0].ChartColor = Color.Green;
            Component[0].FirstBar   = iFirstBar;
            Component[0].Value      = adHAOpen;

            Component[1] = new IndicatorComp();
            Component[1].CompName   = "H.A. High";
            Component[1].DataType   = IndComponentType.IndicatorValue;
            Component[1].ChartType  = IndChartType.Dot;
            Component[1].ChartColor = Color.Blue;
            Component[1].FirstBar   = iFirstBar;
            Component[1].Value      = adHAHigh;

            Component[2] = new IndicatorComp();
            Component[2].CompName   = "H.A. Low";
            Component[2].DataType   = IndComponentType.IndicatorValue;
            Component[2].ChartType  = IndChartType.Dot;
            Component[2].ChartColor = Color.Blue;
            Component[2].FirstBar   = iFirstBar;
            Component[2].Value      = adHALow;

            Component[3] = new IndicatorComp();
            Component[3].CompName   = "H.A. Close";
            Component[3].DataType   = IndComponentType.IndicatorValue;
            Component[3].ChartType  = IndChartType.Dot;
            Component[3].ChartColor = Color.Red;
            Component[3].FirstBar   = iFirstBar;
            Component[3].Value      = adHAClose;

            Component[4] = new IndicatorComp();
            Component[4].ChartType = IndChartType.NoChart;
            Component[4].FirstBar  = iFirstBar;
            Component[4].Value     = new double[Bars];

            Component[5] = new IndicatorComp();
            Component[5].ChartType = IndChartType.NoChart;
            Component[5].FirstBar  = iFirstBar;
            Component[5].Value     = new double[Bars];

            // Sets the Component's type
            if (slotType == SlotTypes.Open)
            {
                Component[4].DataType = IndComponentType.OpenLongPrice;
                Component[4].CompName = "Long position entry price";
                Component[5].DataType = IndComponentType.OpenShortPrice;
                Component[5].CompName = "Short position entry price";
            }
            else if (slotType == SlotTypes.OpenFilter)
            {
                Component[4].DataType = IndComponentType.AllowOpenLong;
                Component[4].CompName = "Is long entry allowed";
                Component[5].DataType = IndComponentType.AllowOpenShort;
                Component[5].CompName = "Is short entry allowed";
            }
            else if (slotType == SlotTypes.Close)
            {
                Component[4].DataType = IndComponentType.CloseLongPrice;
                Component[4].CompName = "Long position closing price";
                Component[5].DataType = IndComponentType.CloseShortPrice;
                Component[5].CompName = "Short position closing price";
            }
            else if (slotType == SlotTypes.CloseFilter)
            {
                Component[4].DataType = IndComponentType.ForceCloseLong;
                Component[4].CompName = "Close out long position";
                Component[5].DataType = IndComponentType.ForceCloseShort;
                Component[5].CompName = "Close out short position";
            }

            if (slotType == SlotTypes.Open || slotType == SlotTypes.Close)
            {
                for (int iBar = 2; iBar < Bars; iBar++)
                {
                    if (IndParam.ListParam[0].Text == "Enter long at the H.A. High" ||
                        IndParam.ListParam[0].Text == "Exit long at the H.A. High")
                    {
                        Component[4].Value[iBar] = adHAHigh[iBar - iPrvs];
                        Component[5].Value[iBar] = adHALow[iBar - iPrvs];
                    }
                    else
                    {
                        Component[4].Value[iBar] = adHALow[iBar - iPrvs];
                        Component[5].Value[iBar] = adHAHigh[iBar - iPrvs];
                    }
                }
            }
            else
            {
                switch (IndParam.ListParam[0].Text)
                {
                    case "White H.A. bar without lower shadow":
                        for (int iBar = iFirstBar; iBar < Bars; iBar++)
                        {
                            Component[4].Value[iBar] = adHAClose[iBar - iPrvs] > adHAOpen[iBar - iPrvs] &&
                                adHALow[iBar - iPrvs] == adHAOpen[iBar - iPrvs] ? 1 : 0;
                            Component[5].Value[iBar] = adHAClose[iBar - iPrvs] < adHAOpen[iBar - iPrvs] &&
                                adHAHigh[iBar - iPrvs] == adHAOpen[iBar - iPrvs] ? 1 : 0;
                        }
                        break;

                    case "White H.A. bar":
                        for (int iBar = iFirstBar; iBar < Bars; iBar++)
                        {
                            Component[4].Value[iBar] = adHAClose[iBar - iPrvs] > adHAOpen[iBar - iPrvs] ? 1 : 0;
                            Component[5].Value[iBar] = adHAClose[iBar - iPrvs] < adHAOpen[iBar - iPrvs] ? 1 : 0;
                        }
                        break;

                    case "Black H.A. bar":
                        for (int iBar = iFirstBar; iBar < Bars; iBar++)
                        {
                            Component[4].Value[iBar] = adHAClose[iBar - iPrvs] < adHAOpen[iBar - iPrvs] ? 1 : 0;
                            Component[5].Value[iBar] = adHAClose[iBar - iPrvs] > adHAOpen[iBar - iPrvs] ? 1 : 0;
                        }
                        break;

                    case "Black H.A. bar without upper shadow":
                        for (int iBar = iFirstBar; iBar < Bars; iBar++)
                        {
                            Component[4].Value[iBar] = adHAClose[iBar - iPrvs] < adHAOpen[iBar - iPrvs] &&
                                adHAHigh[iBar - iPrvs] == adHAOpen[iBar - iPrvs] ? 1 : 0;
                            Component[5].Value[iBar] = adHAClose[iBar - iPrvs] > adHAOpen[iBar - iPrvs] &&
                                adHALow[iBar - iPrvs] == adHAOpen[iBar - iPrvs] ? 1 : 0;
                        }
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
                case "Enter long at the H.A. High":
                    EntryPointLongDescription  = "at the " + ToString() + " High";
                    EntryPointShortDescription = "at the " + ToString() + " Low";
                    break;

                case "Enter long at the H.A. Low":
                    EntryPointLongDescription  = "at the " + ToString() + " Low";
                    EntryPointShortDescription = "at the " + ToString() + " High";
                    break;

                case "Exit long at the H.A. High":
                    ExitPointLongDescription  = "at the " + ToString() + " High";
                    ExitPointShortDescription = "at the " + ToString() + " Low";
                    break;

                case "Exit long at the H.A. Low":
                    ExitPointLongDescription  = "at the " + ToString() + " Low";
                    ExitPointShortDescription = "at the " + ToString() + " High";
                    break;

                case "White H.A. bar without lower shadow":
                    EntryFilterLongDescription  = "the " + ToString() + " bar is white and without lower shadow";
                    EntryFilterShortDescription = "the " + ToString() + " bar is black and without upper shadow";
                    ExitFilterLongDescription   = "the " + ToString() + " bar is white and without lower shadow";
                    ExitFilterShortDescription  = "the " + ToString() + " bar is black and without upper shadow";
                    break;

                case "Black H.A. bar without upper shadow":
                    EntryFilterLongDescription  = "the " + ToString() + " bar is black and without upper shadow";
                    EntryFilterShortDescription = "the " + ToString() + " bar is white and without lower shadow";
                    ExitFilterLongDescription   = "the " + ToString() + " bar is black and without upper shadow";
                    ExitFilterShortDescription  = "the " + ToString() + " bar is white and without lower shadow";
                    break;

                case "White H.A. bar":
                    EntryFilterLongDescription  = "the " + ToString() + " bar is white";
                    EntryFilterShortDescription = "the " + ToString() + " bar is black";
                    ExitFilterLongDescription   = "the " + ToString() + " bar is white";
                    ExitFilterShortDescription  = "the " + ToString() + " bar is black";
                    break;

                case "Black H.A. bar":
                    EntryFilterLongDescription  = "the " + ToString() + " bar is black";
                    EntryFilterShortDescription = "the " + ToString() + " bar is white";
                    ExitFilterLongDescription   = "the " + ToString() + " bar is black";
                    ExitFilterShortDescription  = "the " + ToString() + " bar is white";
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
            string sString = IndicatorName + (IndParam.CheckParam[0].Checked ? "*" : "");

            return sString;
        }
    }
}
