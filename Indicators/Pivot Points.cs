// Pivot Points indicator
// Last changed on 2010-12-04
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// This code or any part of it cannot be used in other applications without a permission.
// Copyright (c) 2006 - 2010 Miroslav Popov - All rights reserved.

using System.Drawing;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Pivot Points indicator
    /// </summary>
    public class Pivot_Points : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Pivot_Points(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "Pivot Points";
            PossibleSlots = SlotTypes.Open | SlotTypes.Close;

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
                    "Enter long at R3 (short at S3)",
                    "Enter long at R2 (short at S2)",
                    "Enter long at R1 (short at S1)",
                    "Enter the market at the Pivot Point",
                    "Enter long at S1 (short at R1)",
                    "Enter long at S2 (short at R2)",
                    "Enter long at S3 (short at R3)",
                };
            else if (slotType == SlotTypes.Close)
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "Exit long at R3 (short at S3)",
                    "Exit long at R2 (short at S2)",
                    "Exit long at R1 (short at S1)",
                    "Exit the market at the Pivot Point",
                    "Exit long at S1 (short at R1)",
                    "Exit long at S2 (short at R2)",
                    "Exit long at S3 (short at R3)",
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

            IndParam.ListParam[1].Caption  = "Base price";
            IndParam.ListParam[1].ItemList = new string[] { "One day", "One bar" };
            IndParam.ListParam[1].Index    = 0;
            IndParam.ListParam[1].Text     = IndParam.ListParam[1].ItemList[IndParam.ListParam[1].Index];
            IndParam.ListParam[1].Enabled  = true;

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "Vertical shift";
            IndParam.NumParam[0].Value   = 0;
            IndParam.NumParam[0].Max     = +2000;
            IndParam.NumParam[0].Min     = -2000;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "A vertical shift above the Resistance and below the Support levels.";

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
            double dShift = IndParam.NumParam[0].Value * Point;
            int    iPrvs  = IndParam.CheckParam[0].Checked ? 1 : 0;

            // Calculation
            int iFirstBar = 1;
            double[] adPP = new double[Bars];
            double[] adR1 = new double[Bars];
            double[] adR2 = new double[Bars];
            double[] adR3 = new double[Bars];
            double[] adS1 = new double[Bars];
            double[] adS2 = new double[Bars];
            double[] adS3 = new double[Bars];

            double[] adO = new double[Bars];
            double[] adH = new double[Bars];
            double[] adL = new double[Bars];
            double[] adC = new double[Bars];

            if (IndParam.ListParam[1].Text == "One bar" ||
                Period == DataPeriods.day || Period == DataPeriods.week)
            {
                adH = High;
                adL = Low;
                adC = Close;
            }
            else
            {
                iPrvs = 0;

                adH[0] = 0;
                adL[0] = 0;
                adC[0] = 0;

                double dTop    = double.MinValue;
                double dBottom = double.MaxValue;

                for (int iBar = 1; iBar < Bars; iBar++)
                {
                    if (High[iBar - 1] > dTop)
                        dTop = High[iBar - 1];
                    if (Low[iBar - 1] < dBottom)
                        dBottom = Low[iBar - 1];

                    if (Time[iBar].Day != Time[iBar - 1].Day)
                    {
                        adH[iBar] = dTop;
                        adL[iBar] = dBottom;
                        adC[iBar] = Close[iBar - 1];
                        dTop      = double.MinValue;
                        dBottom   = double.MaxValue;
                    }
                    else
                    {
                        adH[iBar] = adH[iBar - 1];
                        adL[iBar] = adL[iBar - 1];
                        adC[iBar] = adC[iBar - 1];
                    }
                }

                // first Bar
                for (int iBar = 1; iBar < Bars; iBar++)
                {
                    if (Time[iBar].Day != Time[iBar - 1].Day)
                    {
                        iFirstBar = iBar;
                        break;
                    }
                }
            }

            for (int iBar = iFirstBar; iBar < Bars; iBar++)
            {
                adPP[iBar] = (adH[iBar] + adL[iBar] + adC[iBar]) / 3;
                adR1[iBar] = 2 * adPP[iBar] - adL[iBar];
                adS1[iBar] = 2 * adPP[iBar] - adH[iBar];
                adR2[iBar] = adPP[iBar] + adH[iBar] - adL[iBar];
                adS2[iBar] = adPP[iBar] - adH[iBar] + adL[iBar];
                adR3[iBar] = adH[iBar] + 2 * (adPP[iBar] - adL[iBar]);
                adS3[iBar] = adL[iBar] - 2 * (adH[iBar] - adPP[iBar]);
            }

            Component = new IndicatorComp[9];

            for (int iComp = 0; iComp < 7; iComp++)
            {
                Component[iComp] = new IndicatorComp();
                Component[iComp].ChartType  = IndChartType.Dot;
                Component[iComp].ChartColor = Color.Violet;
                Component[iComp].DataType   = IndComponentType.IndicatorValue;
                Component[iComp].FirstBar   = iFirstBar;
            }
            Component[3].ChartColor = Color.Blue;

            Component[0].Value = adR3;
            Component[1].Value = adR2;
            Component[2].Value = adR1;
            Component[3].Value = adPP;
            Component[4].Value = adS1;
            Component[5].Value = adS2;
            Component[6].Value = adS3;

            Component[0].CompName = "Resistance 3";
            Component[1].CompName = "Resistance 2";
            Component[2].CompName = "Resistance 1";
            Component[3].CompName = "Pivot Point";
            Component[4].CompName = "Support 1";
            Component[5].CompName = "Support 2";
            Component[6].CompName = "Support 3";

            Component[7] = new IndicatorComp();
            Component[7].ChartType = IndChartType.NoChart;
            Component[7].FirstBar  = iFirstBar;
            Component[7].Value     = new double[Bars];

            Component[8] = new IndicatorComp();
            Component[8].ChartType = IndChartType.NoChart;
            Component[8].FirstBar  = iFirstBar;
            Component[8].Value     = new double[Bars];

            if (slotType == SlotTypes.Open)
            {
                Component[7].CompName = "Long position entry price";
                Component[7].DataType = IndComponentType.OpenLongPrice;
                Component[8].CompName = "Short position entry price";
                Component[8].DataType = IndComponentType.OpenShortPrice;
            }
            else if (slotType == SlotTypes.Close)
            {
                Component[7].CompName = "Long position closing price";
                Component[7].DataType = IndComponentType.CloseLongPrice;
                Component[8].CompName = "Short position closing price";
                Component[8].DataType = IndComponentType.CloseShortPrice;
            }

            switch (IndParam.ListParam[0].Text)
            {
                case "Enter long at R3 (short at S3)":
                case "Exit long at R3 (short at S3)":
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        Component[7].Value[iBar] = adR3[iBar - iPrvs] + dShift;
                        Component[8].Value[iBar] = adS3[iBar - iPrvs] - dShift;
                    }
                    break;
                case "Enter long at R2 (short at S2)":
                case "Exit long at R2 (short at S2)":
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        Component[7].Value[iBar] = adR2[iBar - iPrvs] + dShift;
                        Component[8].Value[iBar] = adS2[iBar - iPrvs] - dShift;
                    }
                    break;
                case "Enter long at R1 (short at S1)":
                case "Exit long at R1 (short at S1)":
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        Component[7].Value[iBar] = adR1[iBar - iPrvs] + dShift;
                        Component[8].Value[iBar] = adS1[iBar - iPrvs] - dShift;
                    }
                    break;
                //---------------------------------------------------------------------
                case "Enter the market at the Pivot Point":
                case "Exit the market at the Pivot Point":
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        Component[7].Value[iBar] = adPP[iBar - iPrvs] + dShift;
                        Component[8].Value[iBar] = adPP[iBar - iPrvs] - dShift;
                    }
                    break;
                //---------------------------------------------------------------------
                case "Enter long at S1 (short at R1)":
                case "Exit long at S1 (short at R1)":
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        Component[7].Value[iBar] = adS1[iBar - iPrvs] - dShift;
                        Component[8].Value[iBar] = adR1[iBar - iPrvs] + dShift;
                    }
                    break;
                case "Enter long at S2 (short at R2)":
                case "Exit long at S2 (short at R2)":
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        Component[7].Value[iBar] = adS2[iBar - iPrvs] - dShift;
                        Component[8].Value[iBar] = adR2[iBar - iPrvs] + dShift;
                    }
                    break;
                case "Enter long at S3 (short at R3)":
                case "Exit long at S3 (short at R3)":
                    for (int iBar = iFirstBar; iBar < Bars; iBar++)
                    {
                        Component[7].Value[iBar] = adS3[iBar - iPrvs] - dShift;
                        Component[8].Value[iBar] = adR3[iBar - iPrvs] + dShift;
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
                case "Enter long at R3 (short at S3)":
                    EntryPointLongDescription  = sUpperTrade + "Pivot Point Resistance 3 level";
                    EntryPointShortDescription = sLowerTrade + "Pivot Point Support 3 level";
                    break;
                case "Exit long at R3 (short at S3)":
                    ExitPointLongDescription  = sUpperTrade + "Pivot Point Resistance 3 level";
                    ExitPointShortDescription = sLowerTrade + "Pivot Point Support 3 level";
                    break;
                case "Enter long at R2 (short at S2)":
                    EntryPointLongDescription  = sUpperTrade + "Pivot Point Resistance 2 level";
                    EntryPointShortDescription = sLowerTrade + "Pivot Point Support 2 level";
                    break;
                case "Exit long at R2 (short at S2)":
                    ExitPointLongDescription  = sUpperTrade + "Pivot Point Resistance 2 level";
                    ExitPointShortDescription = sLowerTrade + "Pivot Point Support 2 level";
                    break;
                case "Enter long at R1 (short at S1)":
                    EntryPointLongDescription  = sUpperTrade + "Pivot Point Resistance 1 level";
                    EntryPointShortDescription = sLowerTrade + "Pivot Point Support 1 level";
                    break;
                case "Exit long at R1 (short at S1)":
                    ExitPointLongDescription  = sUpperTrade + "Pivot Point Resistance 1 level";
                    ExitPointShortDescription = sLowerTrade + "Pivot Point Support 1 level";
                    break;
                //---------------------------------------------------------------------
                case "Enter the market at the Pivot Point":
                    EntryPointLongDescription  = sUpperTrade + "Pivot Point";
                    EntryPointShortDescription = sLowerTrade + "Pivot Point";
                    break;
                case "Exit the market at the Pivot Point":
                    ExitPointLongDescription  = sUpperTrade + "Pivot Point";
                    ExitPointShortDescription = sLowerTrade + "Pivot Point";
                    break;
                //---------------------------------------------------------------------
                case "Enter long at S1 (short at R1)":
                    EntryPointLongDescription  = sLowerTrade + "Pivot Point Support 1 level";
                    EntryPointShortDescription = sUpperTrade + "Pivot Point Resistance 1 level";
                    break;
                case "Exit long at S1 (short at R1)":
                    ExitPointLongDescription  = sLowerTrade + "Pivot Point Support 1 level";
                    ExitPointShortDescription = sUpperTrade + "Pivot Point Resistance 1 level";
                    break;
                case "Enter long at S2 (short at R2)":
                    EntryPointLongDescription  = sLowerTrade + "Pivot Point Support 2 level";
                    EntryPointShortDescription = sUpperTrade + "Pivot Point Resistance 2 level";
                    break;
                case "Exit long at S2 (short at R2)":
                    ExitPointLongDescription  = sLowerTrade + "Pivot Point Support 2 level";
                    ExitPointShortDescription = sUpperTrade + "Pivot Point Resistance 2 level";
                    break;
                case "Enter long at S3 (short at R3)":
                    EntryPointLongDescription  = sLowerTrade + "Pivot Point Support 3 level";
                    EntryPointShortDescription = sUpperTrade + "Pivot Point Resistance 3 level";
                    break;
                case "Exit long at S3 (short at R3)":
                    ExitPointLongDescription  = sLowerTrade + "Pivot Point Support 3 level";
                    ExitPointShortDescription = sUpperTrade + "Pivot Point Resistance 3 level";
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
                (IndParam.CheckParam[0].Checked ? "*" : "");
            if (IndParam.ListParam[1].Text == "One day")
                sString += "(Daily)";

            return sString;
        }
    }
}
