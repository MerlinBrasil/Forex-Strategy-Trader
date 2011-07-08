// Price Move Indicator
// Last changed on 2009-05-05
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2009 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Price Move Indicator
    /// </summary>
    public class Price_Move : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Price_Move(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "Price Move";
            PossibleSlots = SlotTypes.Open;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;
            IndParam.IndicatorType = TypeOfIndicator.Additional;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption = "Logic";
            IndParam.ListParam[0].ItemList = new string[]
            {
                "Enter long after an upward move",
                "Enter long after a downward move"
            };
            IndParam.ListParam[0].Index    = 0;
            IndParam.ListParam[0].Text     = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled  = true;
            IndParam.ListParam[0].ToolTip  = "Logic of application of the indicator.";

            IndParam.ListParam[1].Caption  = "Base price";
            IndParam.ListParam[1].ItemList = Enum.GetNames(typeof(BasePrice));
            IndParam.ListParam[1].Index    = (int)BasePrice.Open;
            IndParam.ListParam[1].Text     = IndParam.ListParam[1].ItemList[IndParam.ListParam[1].Index];
            IndParam.ListParam[1].Enabled  = true;
            IndParam.ListParam[1].ToolTip  = "The price where the move starts from.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "Price move";
            IndParam.NumParam[0].Value   = 20;
            IndParam.NumParam[0].Min     = 0;
            IndParam.NumParam[0].Max     = 2000;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "The price move in pips.";

            // The CheckBox parameters
            IndParam.CheckParam[0].Caption = "Use previous bar value";
            IndParam.CheckParam[0].Checked = false;
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
            BasePrice price   = (BasePrice)IndParam.ListParam[1].Index;
            double    dMargin = IndParam.NumParam[0].Value * Point;
            int       iPrvs   = IndParam.CheckParam[0].Checked ? 1 : 0;

            // TimeExecution
            if (price == BasePrice.Open && dMargin == 0)
                IndParam.ExecutionTime = ExecutionTime.AtBarOpening;
            else if (price == BasePrice.Close && dMargin == 0)
                IndParam.ExecutionTime = ExecutionTime.AtBarClosing;

            // Calculation
            double[] adBasePr = Price(price);
            double[] adUpBand = new double[Bars];
			double[] adDnBand = new double[Bars];

            int iFirstBar = 1 + iPrvs;

            for (int iBar = iFirstBar; iBar < Bars; iBar++)
            {
                adUpBand[iBar] = adBasePr[iBar - iPrvs] + dMargin;
                adDnBand[iBar] = adBasePr[iBar - iPrvs] - dMargin;
            }

            // Saving the components
            Component = new IndicatorComp[2];

            Component[0]           = new IndicatorComp();
            Component[0].CompName  = "Up Price";
            Component[0].ChartType = IndChartType.NoChart;
            Component[0].FirstBar  = iFirstBar;
            Component[0].Value     = adUpBand;

            Component[1]           = new IndicatorComp();
            Component[1].CompName  = "Down Price";
            Component[1].ChartType = IndChartType.NoChart;
            Component[1].FirstBar  = iFirstBar;
            Component[1].Value     = adDnBand;

            switch (IndParam.ListParam[0].Text)
            {
                case "Enter long after an upward move":
                    Component[0].DataType = IndComponentType.OpenLongPrice;
                    Component[1].DataType = IndComponentType.OpenShortPrice;
                    break;

                case "Enter long after a downward move":
                    Component[0].DataType = IndComponentType.OpenShortPrice;
                    Component[1].DataType = IndComponentType.OpenLongPrice;
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
            int iMargin = (int)IndParam.NumParam[0].Value;
            string sBasePrice = IndParam.ListParam[1].ItemList[IndParam.ListParam[1].Index].ToLower();
            string sPrevious  = (IndParam.CheckParam[0].Checked ? " previous" : "");

            switch (IndParam.ListParam[0].Text)
            {
                case "Enter long after an upward move":
                    EntryPointLongDescription  = iMargin + " pips above the" + sPrevious + " bar " + sBasePrice + " price";
                    EntryPointShortDescription = iMargin + " pips below the" + sPrevious + " bar " + sBasePrice + " price";
                    break;

                case "Enter long after a downward move":
                    EntryPointLongDescription  = iMargin + " pips below the" + sPrevious + " bar " + sBasePrice + " price";
                    EntryPointShortDescription = iMargin + " pips above the" + sPrevious + " bar " + sBasePrice + " price";
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
                IndParam.ListParam[1].Text         + ", " + // Base Price
                IndParam.NumParam[0].ValueToString + ")";   // Margin in Pips

            return sString;
        }
    }
}
