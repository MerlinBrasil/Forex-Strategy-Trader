// Fractal indicator
// Last changed on 2009-05-05
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2009 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System.Drawing;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Fractal Indicator
    /// </summary>
    public class Fractal : Indicator
	{
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Fractal(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "Fractal";
            PossibleSlots = SlotTypes.Open | SlotTypes.Close;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption = "Logic";
            if (slotType == SlotTypes.Open)
            {
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "Enter long at an Up Fractal",
                    "Enter long at a Down Fractal"
                };
            }
            else if (slotType == SlotTypes.Close)
            {
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "Exit long at an Up Fractal",
                    "Exit long at a Down Fractal"
                };
            }
            else
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "Not Defined"
                };
            IndParam.ListParam[0].Index = 0;
            IndParam.ListParam[0].Text     = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled  = true;
            IndParam.ListParam[0].ToolTip  = "Logic of application of the indicator.";

            IndParam.ListParam[1].Caption  = "Visibility";
            IndParam.ListParam[1].ItemList = new string[]
            {
                "The fractal is visible",
                "The fractal can be shadowed"
            };
            IndParam.ListParam[1].Index   = 0;
            IndParam.ListParam[1].Text    = IndParam.ListParam[1].ItemList[IndParam.ListParam[1].Index];
            IndParam.ListParam[1].Enabled = true;
            IndParam.ListParam[1].ToolTip = "Is the fractal visible from the current market point.";

            IndParam.NumParam[0].Caption = "Vertical shift";
            IndParam.NumParam[0].Value   = 0;
            IndParam.NumParam[0].Min     = -2000;
            IndParam.NumParam[0].Max     = +2000;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "A vertical shift above Up Fractal and below Down Fractal.";

            return;
        }

        /// <summary>
        /// Calculates the indicator's components
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            // Reading the parameters
            bool   bIsVisible = IndParam.ListParam[1].Text == "The fractal is visible";
            double dShift     = (double)IndParam.NumParam[0].Value * Point;
            int    iFirstBar  = 8;

            double[] adFrUp = new double[Bars];
			double[] adFrDn = new double[Bars];

            for (int iBar = 8; iBar < Bars - 1; iBar++)
            {
                if (High[iBar - 1] < High[iBar - 2] && High[iBar] < High[iBar - 2])
                {
                    // Fractal type 1
                    if (High[iBar - 4] < High[iBar - 2] &&
                        High[iBar - 3] < High[iBar - 2])
                        adFrUp[iBar + 1] = High[iBar - 2];

                    // Fractal type 2
                    if (High[iBar - 5] <  High[iBar - 2] &&
                        High[iBar - 4] <  High[iBar - 2] &&
                        High[iBar - 3] == High[iBar - 2])
                        adFrUp[iBar + 1] = High[iBar - 2];

                    // Fractal type 3, 4
                    if (High[iBar - 6] <  High[iBar - 2] &&
                        High[iBar - 5] <  High[iBar - 2] &&
                        High[iBar - 4] == High[iBar - 2] &&
                        High[iBar - 3] <= High[iBar - 2])
                        adFrUp[iBar + 1] = High[iBar - 2];

                    // Fractal type 5
                    if (High[iBar - 7] <  High[iBar - 2] &&
                        High[iBar - 6] <  High[iBar - 2] &&
                        High[iBar - 5] == High[iBar - 2] &&
                        High[iBar - 4] <  High[iBar - 2] &&
                        High[iBar - 3] == High[iBar - 2])
                        adFrUp[iBar + 1] = High[iBar - 2];

                    // Fractal type 6
                    if (High[iBar - 7] <  High[iBar - 2] &&
                        High[iBar - 6] <  High[iBar - 2] &&
                        High[iBar - 5] == High[iBar - 2] &&
                        High[iBar - 4] == High[iBar - 2] &&
                        High[iBar - 3] <  High[iBar - 2])
                        adFrUp[iBar + 1] = High[iBar - 2];

                    // Fractal type 7
                    if (High[iBar - 8] <  High[iBar - 2] &&
                        High[iBar - 7] <  High[iBar - 2] &&
                        High[iBar - 6] == High[iBar - 2] &&
                        High[iBar - 5] <  High[iBar - 2] &&
                        High[iBar - 4] == High[iBar - 2] &&
                        High[iBar - 3] <  High[iBar - 2])
                        adFrUp[iBar + 1] = High[iBar - 2];
                }

                if (Low[iBar - 1] > Low[iBar - 2] && Low[iBar] > Low[iBar - 2])
                {
                    // Fractal type 1
                    if (Low[iBar - 4] >  Low[iBar - 2] &&
                        Low[iBar - 3] >  Low[iBar - 2])
                        adFrDn[iBar + 1] = Low[iBar - 2];

                    // Fractal type 2
                    if (Low[iBar - 5] >  Low[iBar - 2] &&
                        Low[iBar - 4] >  Low[iBar - 2] &&
                        Low[iBar - 3] == Low[iBar - 2])
                        adFrDn[iBar + 1] = Low[iBar - 2];

                    // Fractal type 3, 4
                    if (Low[iBar - 6] >  Low[iBar - 2] &&
                        Low[iBar - 5] >  Low[iBar - 2] &&
                        Low[iBar - 4] == Low[iBar - 2] &&
                        Low[iBar - 3] >= Low[iBar - 2])
                        adFrDn[iBar + 1] = Low[iBar - 2];

                    // Fractal type 5
                    if (Low[iBar - 7] >  Low[iBar - 2] &&
                        Low[iBar - 6] >  Low[iBar - 2] &&
                        Low[iBar - 5] == Low[iBar - 2] &&
                        Low[iBar - 4] >  Low[iBar - 2] &&
                        Low[iBar - 3] == Low[iBar - 2])
                        adFrDn[iBar + 1] = Low[iBar - 2];

                    // Fractal type 6
                    if (Low[iBar - 7] >  Low[iBar - 2] &&
                        Low[iBar - 6] >  Low[iBar - 2] &&
                        Low[iBar - 5] == Low[iBar - 2] &&
                        Low[iBar - 4] == Low[iBar - 2] &&
                        Low[iBar - 3] >  Low[iBar - 2])
                        adFrDn[iBar + 1] = Low[iBar - 2];

                    // Fractal type 7
                    if (Low[iBar - 8] >  Low[iBar - 2] &&
                        Low[iBar - 7] >  Low[iBar - 2] &&
                        Low[iBar - 6] == Low[iBar - 2] &&
                        Low[iBar - 5] >  Low[iBar - 2] &&
                        Low[iBar - 4] == Low[iBar - 2] &&
                        Low[iBar - 3] > Low[iBar - 2])
                        adFrDn[iBar + 1] = Low[iBar - 2];
                }
            }

			// Is visible
            if (bIsVisible)
                for (int iBar = iFirstBar; iBar < Bars; iBar++)
                {
                    if (adFrUp[iBar - 1] > 0 && adFrUp[iBar] == 0 && High[iBar - 1] < adFrUp[iBar - 1])
                        adFrUp[iBar] = adFrUp[iBar - 1];
                    if (adFrDn[iBar - 1] > 0 && adFrDn[iBar] == 0 && Low[iBar - 1]  > adFrDn[iBar - 1])
                        adFrDn[iBar] = adFrDn[iBar - 1];
                }
            else
                for (int iBar = iFirstBar; iBar < Bars; iBar++)
                {
                    if (adFrUp[iBar] == 0) adFrUp[iBar] = adFrUp[iBar - 1];
                    if (adFrDn[iBar] == 0) adFrDn[iBar] = adFrDn[iBar - 1];
                }

            double[] adFrUpEntry = new double[Bars];
            double[] adFrDnEntry = new double[Bars];

            // Saving the components
            Component = new IndicatorComp[4];

			Component[0] = new IndicatorComp();
            Component[0].CompName   = "Up Fractal";
            Component[0].DataType   = IndComponentType.IndicatorValue;
			Component[0].ChartType	= IndChartType.Level;
			Component[0].ChartColor	= Color.SpringGreen;
			Component[0].FirstBar	= iFirstBar;
			Component[0].Value	    = adFrUp;

			Component[1] = new IndicatorComp();
		    Component[1].CompName   = "Down Fractal";
            Component[1].DataType   = IndComponentType.IndicatorValue;
			Component[1].ChartType	= IndChartType.Level;
            Component[1].ChartColor = Color.DarkRed;
			Component[1].FirstBar	= iFirstBar;
			Component[1].Value	    = adFrDn;

            Component[2] = new IndicatorComp();
            Component[2].ChartType = IndChartType.NoChart;
            Component[2].FirstBar  = iFirstBar;
            Component[2].Value     = new double[Bars];

            Component[3] = new IndicatorComp();
            Component[3].ChartType = IndChartType.NoChart;
            Component[3].FirstBar  = iFirstBar;
            Component[3].Value     = new double[Bars];

            if (slotType == SlotTypes.Open)
            {
                Component[2].CompName = "Long position entry price";
                Component[2].DataType = IndComponentType.OpenLongPrice;
                Component[3].CompName = "Short position entry price";
                Component[3].DataType = IndComponentType.OpenShortPrice;
            }
            else if (slotType == SlotTypes.Close)
            {
                Component[2].CompName = "Long position closing price";
                Component[2].DataType = IndComponentType.CloseLongPrice;
                Component[3].CompName = "Short position closing price";
                Component[3].DataType = IndComponentType.CloseShortPrice;
            }

            switch (IndParam.ListParam[0].Text)
            {
                case "Enter long at an Up Fractal":
                case "Exit long at an Up Fractal":
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        if(adFrUp[iBar] > Point)
                            Component[2].Value[iBar] = adFrUp[iBar] + dShift;
                        if (adFrDn[iBar] > Point)
                            Component[3].Value[iBar] = adFrDn[iBar] - dShift;
                    }
                    break;
                case "Enter long at a Down Fractal":
                case "Exit long at a Down Fractal":
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        if (adFrDn[iBar] > Point)
                            Component[2].Value[iBar] = adFrDn[iBar] - dShift;
                        if(adFrUp[iBar] > Point)
                            Component[3].Value[iBar] = adFrUp[iBar] + dShift;
                    }
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
                sUpperTrade = iShift + " pips above ";
                sLowerTrade = iShift + " pips below ";
            }
            else if (iShift == 0)
            {
                sUpperTrade = "at ";
                sLowerTrade = "at ";
            }
            else
            {
                sUpperTrade = -iShift + " pips below ";
                sLowerTrade = -iShift + " pips above ";
            }

            switch (IndParam.ListParam[0].Text)
            {
                case "Enter long at an Up Fractal":
                    EntryPointLongDescription  = sUpperTrade + "an Up Fractal";
                    EntryPointShortDescription = sLowerTrade + "a Down Fractal";
                    break;
                case "Exit long at an Up Fractal":
                    ExitPointLongDescription  = sUpperTrade + "an Up Fractal";
                    ExitPointShortDescription = sLowerTrade + "a Down Fractal";
                    break;
                case "Enter long at a Down Fractal":
                    EntryPointLongDescription  = sLowerTrade + "a Down Fractal";
                    EntryPointShortDescription = sUpperTrade + "an Up Fractal";
                    break;
                case "Exit long at a Down Fractal":
                    ExitPointLongDescription  = sLowerTrade + "a Down Fractal";
                    ExitPointShortDescription = sUpperTrade + "an Up Fractal";
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
            return IndicatorName;
        }
    }
}