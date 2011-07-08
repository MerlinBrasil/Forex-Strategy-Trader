// Parabolic SAR Indicator
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
    /// Parabolic SAR Indicator
    /// </summary>
    public class Parabolic_SAR : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Parabolic_SAR(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "Parabolic SAR";
            PossibleSlots = SlotTypes.OpenFilter | SlotTypes.Close;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption  = "Logic";
            if (slotType == SlotTypes.OpenFilter)
                IndParam.ListParam[0].ItemList = new string[] 
                {
                    "The price is higher than the PSAR value" 
                };
            else if (slotType == SlotTypes.Close)
                IndParam.ListParam[0].ItemList = new string[]
                { 
                    "Exit the market at PSAR" 
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

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "Starting AF";
            IndParam.NumParam[0].Value   = 0.02;
            IndParam.NumParam[0].Min     = 0.00;
            IndParam.NumParam[0].Max     = 5.00;
            IndParam.NumParam[0].Point   = 2;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "The starting value of Acceleration Factor.";

            IndParam.NumParam[1].Caption = "Increment";
            IndParam.NumParam[1].Value   = 0.02;
            IndParam.NumParam[1].Min     = 0.01;
            IndParam.NumParam[1].Max     = 5.00;
            IndParam.NumParam[1].Point   = 2;
            IndParam.NumParam[1].Enabled = true;
            IndParam.NumParam[1].ToolTip = "Increment value.";

            IndParam.NumParam[2].Caption = "Maximum AF";
            IndParam.NumParam[2].Value   = 2.00;
            IndParam.NumParam[2].Min     = 0.01;
            IndParam.NumParam[2].Max     = 9.00;
            IndParam.NumParam[2].Point   = 2;
            IndParam.NumParam[2].Enabled = true;
            IndParam.NumParam[2].ToolTip = "The maximum value of the Acceleration Factor.";

            return;
        }

        /// <summary>
        /// Calculates the indicator's components
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            double dAFMin = IndParam.NumParam[0].Value;
            double dAFInc = IndParam.NumParam[1].Value;
            double dAFMax = IndParam.NumParam[2].Value;

            // Reading the parameters
            int intDirNew;
            double dAF;
            double dPExtr;
            double dPSARNew = 0;
            int[]    aiDir  = new int[Bars];
            double[] adPSAR = new double[Bars];

            //----	Calculating the initial values
            adPSAR[0] = 0;
            dAF       = dAFMin;
            intDirNew = 0;
            if (Close[1] > Open[0])
            {
                aiDir[0]  = 1;
                aiDir[1]  = 1;
                dPExtr    = Math.Max(High[0], High[1]);
                adPSAR[1] = Math.Min(Low[0],  Low[1]);
            }
            else
            {
                aiDir[0]  = -1;
                aiDir[1]  = -1;
                dPExtr    = Math.Min(Low[0],  Low[1]);
                adPSAR[1] = Math.Max(High[0], High[1]);
            }

            for (int iBar = 2; iBar < Bars; iBar++)
            {
                //----	PSAR for the current period
                if (intDirNew != 0)
                {	
                    // The direction was changed during the last period
                    aiDir[iBar]  = intDirNew;
                    intDirNew    = 0;
                    adPSAR[iBar] = dPSARNew + dAF * (dPExtr - dPSARNew);
                }
                else
                {
                    aiDir[iBar]  = aiDir[iBar - 1];
                    adPSAR[iBar] = adPSAR[iBar - 1] + dAF * (dPExtr - adPSAR[iBar - 1]);
                }

                // PSAR has to be out of the previous two bars limits
                if (aiDir[iBar] > 0 && adPSAR[iBar] > Math.Min(Low[iBar - 1], Low[iBar - 2]))
                    adPSAR[iBar] = Math.Min(Low[iBar - 1], Low[iBar - 2]);
                else if (aiDir[iBar] < 0 && adPSAR[iBar] < Math.Max(High[iBar - 1], High[iBar - 2]))
                    adPSAR[iBar] = Math.Max(High[iBar - 1], High[iBar - 2]);

                //----	PSAR for the next period

                // Calculation of the new values of flPExtr and flAF
                // if there is a new extreme price in the PSAR direction
                if (aiDir[iBar] > 0 && High[iBar] > dPExtr)
                {
                    dPExtr = High[iBar];
                    dAF    = Math.Min(dAF + dAFInc, dAFMax);
                }

                if (aiDir[iBar] < 0 && Low[iBar] < dPExtr)
                {
                    dPExtr = Low[iBar];
                    dAF    = Math.Min(dAF + dAFInc, dAFMax);
                }

                // Wheather the price reaches PSAR
                if (Low[iBar] <= adPSAR[iBar] && adPSAR[iBar] <= High[iBar])
                {
                    intDirNew = -aiDir[iBar];
                    dPSARNew  = dPExtr;
                    dAF       = dAFMin;
                    if (intDirNew > 0)
                        dPExtr = High[iBar];
                    else
                        dPExtr = Low[iBar];
                }

            }
            int iFirstBar = 8;

            // Saving the components
            Component = new IndicatorComp[1];

            Component[0] = new IndicatorComp();
            Component[0].CompName = "PSAR value";
            if (slotType == SlotTypes.Close)
                Component[0].DataType = IndComponentType.ClosePrice;
            else
                Component[0].DataType = IndComponentType.IndicatorValue;
            Component[0].ChartType  = IndChartType.Dot;
            Component[0].ChartColor = Color.Violet;
            Component[0].FirstBar   = iFirstBar;
            Component[0].PosPriceDependence = PositionPriceDependence.BuyHigherSellLower;
            Component[0].Value      = adPSAR;

            return;
        }

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {
            EntryFilterLongDescription  = "the price is higher than the " + ToString();
            EntryFilterShortDescription = "the price is lower than the " + ToString();
            ExitPointLongDescription    = "at the " + ToString() + ". It determines the position direction also";
            ExitPointShortDescription   = "at the " + ToString() + ". It determines the position direction also";

            return;
        }

        /// <summary>
        /// Indicator to string
        /// </summary>
        public override string ToString()
        {
            string sString = IndicatorName + " (" +
                IndParam.NumParam[0].ValueToString + ", " + // Starting AF
                IndParam.NumParam[1].ValueToString + ", " + // Increment
                IndParam.NumParam[2].ValueToString + ")";   // Max AF

            return sString;
        }
    }
}