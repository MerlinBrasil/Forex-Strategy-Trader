// Ross Hook Indicator
// Last changed on 2009-05-15
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// This code or any part of it cannot be used in other applications without a permission.
// Copyright (c) 2006 - 2009 Miroslav Popov - All rights reserved.

using System.Drawing;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Ross Hook Indicator
    /// </summary>
    public class Ross_Hook : Indicator
	{
        /// <summary>
        /// Constructor
        /// </summary>
        public Ross_Hook(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "Ross Hook";
            PossibleSlots = SlotTypes.Open | SlotTypes.Close;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption  = "Logic";
            if (slotType == SlotTypes.Open)
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "Enter long at an Up Ross hook",
                    "Enter long at a Down Ross hook"
                };
            else if (slotType == SlotTypes.Close)
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "Exit long at an Up Ross hook",
                    "Exit long at a Down Ross hook"
                };
            else
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "Not Defined"
                };
            IndParam.ListParam[0].Index = 0;
            IndParam.ListParam[0].Text    = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Logic of application of the indicator.";

            return;
        }

        /// <summary>
        /// Calculates the indicator's components
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            double[] adRhUp = new double[Bars];
			double[] adRhDn = new double[Bars];

            for (int iBar = 5; iBar < Bars - 1; iBar++)
            {
                if (High[iBar] < High[iBar - 1])
                {
                    if (High[iBar - 3] < High[iBar - 1] && High[iBar - 2] < High[iBar - 1])
                        adRhUp[iBar + 1] = High[iBar - 1];
                }

                if (Low[iBar] > Low[iBar - 1])
                {
                    if (Low[iBar - 3] > Low[iBar - 1] && Low[iBar - 2] > Low[iBar - 1])
                        adRhDn[iBar + 1] = Low[iBar - 1];
                }
            }

            // Is visible
            for (int iBar = 5; iBar < Bars; iBar++)
            {
                if (adRhUp[iBar - 1] > 0 && adRhUp[iBar] == 0 && High[iBar - 1] < adRhUp[iBar - 1])
                    adRhUp[iBar] = adRhUp[iBar - 1];
                if (adRhDn[iBar - 1] > 0 && adRhDn[iBar] == 0 && Low[iBar - 1] > adRhDn[iBar - 1])
                    adRhDn[iBar] = adRhDn[iBar - 1];
            }

            // Saving the components
            Component = new IndicatorComp[2];

			Component[0]			= new IndicatorComp();
            Component[0].ChartType  = IndChartType.Level;
			Component[0].ChartColor	= Color.SpringGreen;
			Component[0].FirstBar	= 5;
			Component[0].Value	    = adRhUp;

			Component[1]			= new IndicatorComp();
            Component[1].ChartType  = IndChartType.Level;
            Component[1].ChartColor = Color.DarkRed;
			Component[1].FirstBar	= 5;
			Component[1].Value	    = adRhDn;

            // Sets the Component's type
            if (slotType == SlotTypes.Open)
            {
                if (IndParam.ListParam[0].Text == "Enter long at an Up Ross hook")
                {
                    Component[0].DataType = IndComponentType.OpenLongPrice;
                    Component[1].DataType = IndComponentType.OpenShortPrice;
                }
                else
                {
                    Component[0].DataType = IndComponentType.OpenShortPrice;
                    Component[1].DataType = IndComponentType.OpenLongPrice;
                }
                Component[0].CompName = "Up Ross hook";
                Component[1].CompName = "Down Ross hook";
            }
            else if (slotType == SlotTypes.Close)
            {
                if (IndParam.ListParam[0].Text == "Exit long at an Up Ross hook")
                {
                    Component[0].DataType = IndComponentType.CloseLongPrice;
                    Component[1].DataType = IndComponentType.CloseShortPrice;
                }
                else
                {
                    Component[0].DataType = IndComponentType.CloseShortPrice;
                    Component[1].DataType = IndComponentType.CloseLongPrice;
                }
                Component[0].CompName = "Up Ross hook";
                Component[1].CompName = "Down Ross hook";
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
                case "Enter long at an Up Ross hook":
                    EntryPointLongDescription  = "at the peak of an Up Ross hook";
                    EntryPointShortDescription = "at the bottom of a Down  Ross hook";
                    break;

                case "Enter long at a Down Ross hook":
                    EntryPointLongDescription  = "at the bottom of a Down Ross hook";
                    EntryPointShortDescription = "at the peak of an Up Ross hook";
                    break;

                case "Exit long at an Up Ross hook":
                    ExitPointLongDescription  = "at the peak of an Up Ross hook";
                    ExitPointShortDescription = "at the bottom of a Down Ross hook";
                    break;

                case "Exit long at a Down Ross hook":
                    ExitPointLongDescription  = "at the bottom of a Down Ross hook";
                    ExitPointShortDescription = "at the peak of an Up Fractal";
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