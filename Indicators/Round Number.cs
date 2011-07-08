// Round Numbers Indicator
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
    /// Round Numbers Indicator
    /// </summary>
    public class Round_Number : Indicator
	{
        /// <summary>
        /// Constructor
        /// </summary>
        public Round_Number(SlotTypes slotType)
        {
            // General properties
            IndicatorName  = "Round Number";
            PossibleSlots  = SlotTypes.Open | SlotTypes.Close;

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
                    "Enter long at the higher round number",
                    "Enter long at the lower round number"
                };
            else if (slotType == SlotTypes.Close)
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "Exit long at the higher round number",
                    "Exit long at the lower round number"
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

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "Vertical shift";
            IndParam.NumParam[0].Value   = 0;
            IndParam.NumParam[0].Min     = -2000;
            IndParam.NumParam[0].Max     = +2000;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "A vertical shift above the higher and below the lower round number.";

            // The NumericUpDown parameters
            IndParam.NumParam[1].Caption = "Digids";
            IndParam.NumParam[1].Value   = 2;
            IndParam.NumParam[1].Min     = 2;
            IndParam.NumParam[1].Max     = 4;
            IndParam.NumParam[1].Enabled = true;
            IndParam.NumParam[1].ToolTip = "Number of digids to be rounded.";

            return;
        }

        /// <summary>
        /// Calculates the indicator's components
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            // Reading the parameters
            double dShift  = IndParam.NumParam[0].Value * Point;
            int    iDigids = (int)IndParam.NumParam[1].Value;

            // Calculation
			double[] adUpperRN = new double[Bars];
			double[] adLowerRN = new double[Bars];

            int iFirstBar = 1;
	
            for (int iBar = 1; iBar < Bars; iBar++)
            {
                double dNearestRound;

                int iCutDigids = Digits - iDigids;
                if (iCutDigids >= 0)
                    dNearestRound = Math.Round(Open[iBar], iCutDigids);
                else
                    dNearestRound = Math.Round(Open[iBar] * Math.Pow(10, iCutDigids)) / Math.Pow(10, iCutDigids);


                if (dNearestRound < Open[iBar])
                {
                    adUpperRN[iBar] = dNearestRound + (Point * Math.Pow(10, iDigids));
                    adLowerRN[iBar] = dNearestRound;
                }
                else
                {
                    adUpperRN[iBar] = dNearestRound;
                    adLowerRN[iBar] = dNearestRound - (Point * Math.Pow(10, iDigids));
                }
            }

            // Saving the components
            Component = new IndicatorComp[4];

            Component[0] = new IndicatorComp();
            Component[0].CompName   = "Higher round number";
            Component[0].DataType   = IndComponentType.IndicatorValue;
            Component[0].ChartType  = IndChartType.Level;
            Component[0].ChartColor = Color.SpringGreen;
            Component[0].FirstBar   = iFirstBar;
            Component[0].Value      = adUpperRN;

            Component[1] = new IndicatorComp();
            Component[1].CompName   = "Lower round number";
            Component[1].DataType   = IndComponentType.IndicatorValue;
            Component[1].ChartType  = IndChartType.Level;
            Component[1].ChartColor = Color.DarkRed;
            Component[1].FirstBar   = iFirstBar;
            Component[1].Value      = adLowerRN;

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
                case "Enter long at the higher round number":
                case "Exit long at the higher round number":
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        Component[2].Value[iBar] = adUpperRN[iBar] + dShift;
                        Component[3].Value[iBar] = adLowerRN[iBar] - dShift;
                    }
                    break;
                case "Enter long at the lower round number":
                case "Exit long at the lower round number":
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        Component[2].Value[iBar] = adLowerRN[iBar] - dShift;
                        Component[3].Value[iBar] = adUpperRN[iBar] + dShift;
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
                sUpperTrade = iShift + " pips above the ";
                sLowerTrade = iShift + " pips below the ";
            }
            else if (iShift == 0)
            {
                sUpperTrade = "at the ";
                sLowerTrade = "at the ";
            }
            else
            {
                sUpperTrade = -iShift + " pips below the ";
                sLowerTrade = -iShift + " pips above the ";
            }
            switch (IndParam.ListParam[0].Text)
            {
                case "Enter long at the higher round number":
                    EntryPointLongDescription  = sUpperTrade + "higher round number";
                    EntryPointShortDescription = sLowerTrade + "lower round number";
                    break;
                case "Exit long at the higher round number":
                    ExitPointLongDescription  = sUpperTrade + "higher round number";
                    ExitPointShortDescription = sLowerTrade + "lower round number";
                    break;
                case "Enter long at the lower round number":
                    EntryPointLongDescription  = sLowerTrade + "lower round number";
                    EntryPointShortDescription = sUpperTrade + "higher round number";
                    break;
                case "Exit long at the lower round number":
                    ExitPointLongDescription  = sLowerTrade + "lower round number";
                    ExitPointShortDescription = sUpperTrade + "higher round number";
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
                IndParam.NumParam[0].ValueToString + ")";  // Vertical shift

            return sString;
        }
    }
}
