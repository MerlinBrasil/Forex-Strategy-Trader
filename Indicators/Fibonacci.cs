// Fibonacci Indicator (For demo only!)
// Last changed on 2010-04-11
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2010 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Fibonacci Indicator
    /// </summary>
    public class Fibonacci : Indicator
	{
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Fibonacci(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "Fibonacci";
            PossibleSlots = SlotTypes.Open;
            WarningMessage = "Fibonacci indicator is included in the program for demonstration only." +
                " This indicator tends to change past signals. That behaviour leads to unreliable back test results." + 
                " It’s not recommended Fibonacci to be used in real trading strategies.";

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption  = "Logic";
            IndParam.ListParam[0].ItemList = new string[]
            {
                "This indicator is for demo only!!!"
            };
            IndParam.ListParam[0].Index    = 0;
            IndParam.ListParam[0].Text     = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled  = true;
            IndParam.ListParam[0].ToolTip  = "Logic of application of the indicator.";

            IndParam.ListParam[1].Caption  = "Direction";
            IndParam.ListParam[1].ItemList = new string[]
            {
                "Breakout",
                "Retracement"
            };
            IndParam.ListParam[1].Index   = 0;
            IndParam.ListParam[1].Text    = IndParam.ListParam[1].ItemList[IndParam.ListParam[1].Index];
            IndParam.ListParam[1].Enabled = true;
            IndParam.ListParam[1].ToolTip = "Direction of trade.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "Sensitivity";
            IndParam.NumParam[0].Value   = 15;
            IndParam.NumParam[0].Min     = 10;
            IndParam.NumParam[0].Max     = 200;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "Sensitivity of the indicator.";

            return;
		}

        /// <summary>
        /// Calculates the indicator's components
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            // Reading the parameters
            int iSense = (int)IndParam.NumParam[0].Value;

            int iFirstBar = iSense;

            double[] adFibo0;
            double[] adFibo382;
            double[] adFibo50;
            double[] adFibo618;
            double[] adFibo100;

            bool bReverseLogic = (IndParam.ListParam[1].Text == "Retracement");
            int  iDeviation    = 5;
            int  iBackStep     = 3;

            double[] adHighPoint = new double[Bars];
            double[] adLowPoint  = new double[Bars];

            adFibo0   = new double[Bars];
            adFibo382 = new double[Bars];
            adFibo50  = new double[Bars];
            adFibo618 = new double[Bars];
            adFibo100 = new double[Bars];

            double dLastHigh = 0;
            double dLastLow  = 0;
            for (int iBar = iSense; iBar < Bars; iBar++)
            {
                // The highest High in the period [iBar-iSence, iBar]
                double dHigh = 0;
                for (int iShift = 0; iShift < iSense; iShift++)
                    if (dHigh < High[iBar - iShift])
                        dHigh = High[iBar - iShift];

                if (dHigh == dLastHigh)
                    dHigh = 0;
                else
                {
                    dLastHigh = dHigh;
                    if (dHigh - High[iBar] > iDeviation * Point)
                        dHigh = 0;
                    else
                        for (int iBack = 1; iBack <= iBackStep; iBack++)
                            if (adHighPoint[iBar - iBack] > 0 && adHighPoint[iBar - iBack] < dHigh)
                                adHighPoint[iBar - iBack] = 0;
                }

                adHighPoint[iBar] = dHigh;

                // The lowest Low in the period [iBar-iSence, iBar]
                double dLow = 10000;
                for (int iShift = 0; iShift < iSense; iShift++)
                    if (Low[iBar - iShift] < dLow)
                        dLow = Low[iBar - iShift];

                if (dLow == dLastLow)
                    dLow = 0;
                else
                {
                    dLastLow = dLow;
                    if (Low[iBar] - dLow > iDeviation * Point)
                        dLow = 0;
                    else
                        for (int iBack = 1; iBack <= iBackStep; iBack++)
                            if (adLowPoint[iBar - iBack] > 0 && adLowPoint[iBar - iBack] > dLow)
                                adLowPoint[iBar - iBack] = 0;
                }

                adLowPoint[iBar] = dLow;
            }

            int iLastHighBar = -1;
            int iLastLowBar  = -1;
            double dCurHigh;
            double dCurLow;
            dLastHigh = -1;
            dLastLow  = -1;

            for (int iBar = iSense; iBar < Bars; iBar++)
            {
                dCurHigh = adHighPoint[iBar];
                dCurLow  = adLowPoint[iBar];
                if (dCurLow == 0 && dCurHigh == 0) continue;

                if (dCurHigh != 0)
                {
                    if (dLastHigh > 0)
                    {
                        if (dLastHigh < dCurHigh) adHighPoint[iLastHighBar] = 0;
                        else adHighPoint[iBar] = 0;
                    }
                    if (dLastHigh < dCurHigh || dLastHigh < 0)
                    {
                        dLastHigh    = dCurHigh;
                        iLastHighBar = iBar;
                    }
                    dLastLow = -1;
                }

                if (dCurLow != 0)
                {
                    if (dLastLow > 0)
                    {
                        if (dLastLow > dCurLow) adLowPoint[iLastLowBar] = 0;
                        else adLowPoint[iBar] = 0;
                    }
                    if (dCurLow < dLastLow || dLastLow < 0)
                    {
                        dLastLow    = dCurLow;
                        iLastLowBar = iBar;
                    }
                    dLastHigh = -1;
                }
            }

            dLastHigh = 0;
            dLastLow  = 0;
            int iFirstLowBar  = 0;
            int iFirstHighBar = 0;
            for (int iBar = 0; iBar < Bars; iBar++)
            {
                if (adHighPoint[iBar] > 0)
                {
                    dLastHigh = adHighPoint[iBar];
                    iFirstHighBar = iBar;
                }
                if (adLowPoint[iBar] > 0)
                {
                    dLastLow = adLowPoint[iBar];
                    iFirstLowBar = iBar;
                }
                if (iFirstHighBar > 0 && iFirstLowBar > 0) break;
            }

            for (int iBar = Math.Max(iFirstLowBar, iFirstHighBar); iBar < Bars; iBar++)
            {
                if (adHighPoint[iBar - 1] > 0)
                {
                    dLastHigh = adHighPoint[iBar - 1];
                    adFibo0  [iBar] = dLastHigh;
                    adFibo382[iBar] = dLastHigh - (dLastHigh - dLastLow) * 0.382;
                    adFibo50 [iBar] = dLastHigh - (dLastHigh - dLastLow) * 0.500;
                    adFibo618[iBar] = dLastHigh - (dLastHigh - dLastLow) * 0.618;
                    adFibo100[iBar] = dLastLow;
                }
                else if (adLowPoint[iBar - 1] > 0)
                {
                    dLastLow = adLowPoint[iBar - 1];
                    adFibo0  [iBar] = dLastLow;
                    adFibo382[iBar] = dLastLow + (dLastHigh - dLastLow) * 0.382;
                    adFibo50 [iBar] = dLastLow + (dLastHigh - dLastLow) * 0.500;
                    adFibo618[iBar] = dLastLow + (dLastHigh - dLastLow) * 0.618;
                    adFibo100[iBar] = dLastHigh;
                }
                else
                {
                    adFibo0  [iBar] = adFibo0  [iBar - 1];
                    adFibo382[iBar] = adFibo382[iBar - 1];
                    adFibo50 [iBar] = adFibo50 [iBar - 1];
                    adFibo618[iBar] = adFibo618[iBar - 1];
                    adFibo100[iBar] = adFibo100[iBar - 1];
                }
            }

            // Saving the components
            Component = new IndicatorComp[8];

            Component[0] = new IndicatorComp();
            Component[0].CompName   = "Position entry price";
            Component[0].DataType   = IndComponentType.OpenPrice;
            Component[0].ChartType  = IndChartType.NoChart;
            Component[0].FirstBar   = iFirstBar;
            Component[0].Value      = new double[Bars];

            Component[1] = new IndicatorComp();
            Component[1].CompName   = "Fibonacci retracement 0%";
            Component[1].DataType   = IndComponentType.IndicatorValue;
            Component[1].ChartType  = IndChartType.Level;
            Component[1].ChartColor = Color.Green;
            Component[1].FirstBar   = iFirstBar;
            Component[1].Value      = adFibo0;

            Component[2] = new IndicatorComp();
            Component[2].CompName   = "Fibonacci retracement 38.2%";
            Component[2].DataType   = IndComponentType.IndicatorValue;
            Component[2].ChartType  = IndChartType.Level;
            Component[2].ChartColor = Color.Gold;
            Component[2].FirstBar   = iFirstBar;
            Component[2].Value      = adFibo382;

            Component[3] = new IndicatorComp();
            Component[3].CompName   = "Fibonacci retracement 50%";
            Component[3].DataType   = IndComponentType.IndicatorValue;
            Component[3].ChartType  = IndChartType.Level;
            Component[3].ChartColor = Color.Orchid;
            Component[3].FirstBar   = iFirstBar;
            Component[3].Value      = adFibo50;

            Component[4] = new IndicatorComp();
            Component[4].CompName   = "Fibonacci retracement 61.8%";
            Component[4].DataType   = IndComponentType.IndicatorValue;
            Component[4].ChartType  = IndChartType.Level;
            Component[4].ChartColor = Color.Purple;
            Component[4].FirstBar   = iFirstBar;
            Component[4].Value      = adFibo618;

            Component[5] = new IndicatorComp();
            Component[5].CompName   = "Fibonacci retracement 100%";
            Component[5].DataType   = IndComponentType.IndicatorValue;
            Component[5].ChartType  = IndChartType.Level;
            Component[5].ChartColor = Color.Red;
            Component[5].FirstBar   = iFirstBar;
            Component[5].Value      = adFibo100;

            Component[6] = new IndicatorComp();
            Component[6].CompName  = "Is long entry allowed";
            Component[6].DataType  = IndComponentType.AllowOpenLong;
            Component[6].ChartType = IndChartType.NoChart;
            Component[6].FirstBar  = iFirstBar;
            Component[6].Value     = new double[Bars];

			Component[7]           = new IndicatorComp();
			Component[7].CompName  = "Is short entry allowed";
			Component[7].DataType  = IndComponentType.AllowOpenShort;
			Component[7].ChartType = IndChartType.NoChart;
            Component[7].FirstBar  = iFirstBar;
			Component[7].Value	   = new double[Bars];

			int iBarFibo382Reached = 0;
			int iBarFibo500Reached = 0;
			int iBarFibo618Reached = 0;
			int iBarFibo100Reached = 0;

            for (int iBar = Math.Max(iFirstLowBar, iFirstHighBar); iBar < Bars; iBar++)
            {
                Component[0].Value[iBar] = 0;

                // Reset
                if (adHighPoint[iBar - 1] > 0 || adLowPoint[iBar - 1] > 0)
                {
                    iBarFibo382Reached = 0;
                    iBarFibo500Reached = 0;
                    iBarFibo618Reached = 0;
                    iBarFibo100Reached = 0;
                }

                // Up trend
                if (adFibo0[iBar] < adFibo100[iBar])
                {
                    if (iBarFibo382Reached == 0 && Low[iBar] <= adFibo382[iBar] && High[iBar] >= adFibo382[iBar])
                    {
                        Component[6].Value[iBar] = bReverseLogic ? 0 : 1;
                        Component[7].Value[iBar] = bReverseLogic ? 1 : 0;
                        Component[0].Value[iBar] = adFibo382[iBar];
                        iBarFibo382Reached = iBar;
                    }
                    if (iBarFibo500Reached == 0 && Low[iBar] <= adFibo50[iBar] && High[iBar] >= adFibo50[iBar])
                    {
                        Component[6].Value[iBar] = bReverseLogic ? 0 : 1;
                        Component[7].Value[iBar] = bReverseLogic ? 1 : 0;
                        if (iBarFibo382Reached != iBar)
                            Component[0].Value[iBar] = adFibo50[iBar];
                        iBarFibo500Reached = iBar;
                    }
                    if (iBarFibo618Reached == 0 && Low[iBar] <= adFibo618[iBar] && High[iBar] >= adFibo618[iBar])
                    {
                        Component[6].Value[iBar] = bReverseLogic ? 0 : 1;
                        Component[7].Value[iBar] = bReverseLogic ? 1 : 0;
                        if (iBarFibo500Reached != iBar)
                            Component[0].Value[iBar] = adFibo618[iBar];
                        iBarFibo618Reached = iBar;
                    }
                    if (iBarFibo100Reached == 0 && Low[iBar] <= adFibo100[iBar] && High[iBar] >= adFibo100[iBar])
                    {
                        Component[6].Value[iBar] = bReverseLogic ? 0 : 1;
                        Component[7].Value[iBar] = bReverseLogic ? 1 : 0;
                        if (iBarFibo618Reached != iBar)
                            Component[0].Value[iBar] = adFibo100[iBar];
                        iBarFibo100Reached = iBar;
                    }
                }

                // Down trend
                if (adFibo0[iBar] > adFibo100[iBar])
                {
                    if (iBarFibo382Reached == 0 && Low[iBar] <= adFibo382[iBar] && High[iBar] >= adFibo382[iBar])
                    {
                        Component[6].Value[iBar] = bReverseLogic ? 1 : 0;
                        Component[7].Value[iBar] = bReverseLogic ? 0 : 1;
                        Component[0].Value[iBar] = adFibo382[iBar];
                        iBarFibo382Reached = iBar;
                    }
                    if (iBarFibo500Reached == 0 && Low[iBar] <= adFibo50[iBar] && High[iBar] >= adFibo50[iBar])
                    {
                        Component[6].Value[iBar] = bReverseLogic ? 1 : 0;
                        Component[7].Value[iBar] = bReverseLogic ? 0 : 1;
                        if (iBarFibo382Reached != iBar)
                            Component[0].Value[iBar] = adFibo50[iBar];
                        iBarFibo500Reached = iBar;
                    }
                    if (iBarFibo618Reached == 0 && Low[iBar] <= adFibo618[iBar] && High[iBar] >= adFibo618[iBar])
                    {
                        Component[6].Value[iBar] = bReverseLogic ? 1 : 0;
                        Component[7].Value[iBar] = bReverseLogic ? 0 : 1;
                        if (iBarFibo500Reached != iBar)
                            Component[0].Value[iBar] = adFibo618[iBar];
                        iBarFibo618Reached = iBar;
                    }
                    if (iBarFibo100Reached == 0 && Low[iBar] <= adFibo100[iBar] && High[iBar] >= adFibo100[iBar])
                    {
                        Component[6].Value[iBar] = bReverseLogic ? 1 : 0;
                        Component[7].Value[iBar] = bReverseLogic ? 0 : 1;
                        if (iBarFibo618Reached != iBar)
                            Component[0].Value[iBar] = adFibo100[iBar];
                        iBarFibo100Reached = iBar;
                    }
                }
			}

            return;
		}

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {
            switch (IndParam.ListParam[1].Text)
            {
               case "Breakout":
                    EntryPointLongDescription  = "at a Fibonacci Level when the price rises";
                    EntryPointShortDescription = "at a Fibonacci Level when the price falls";
                    break;

                case "Retracement":
                    EntryPointLongDescription  = "at a Fibonacci Level when the price falls";
                    EntryPointShortDescription = "at a Fibonacci Level when the price rises";
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