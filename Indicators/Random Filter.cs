// Random Filter Indicator
// Last changed on 2009-10-07
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// This code or any part of it cannot be used in other applications without a permission.
// Copyright (c) 2006 - 2009 Miroslav Popov - All rights reserved.

using System;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Random Filter Indicator
    /// </summary>
    public class Random_Filter : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public Random_Filter(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "Random Filter";
            PossibleSlots = SlotTypes.OpenFilter | SlotTypes.CloseFilter;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;
            IndParam.IndicatorType = TypeOfIndicator.Additional;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption = "Logic";
            if (slotType == SlotTypes.OpenFilter)
                IndParam.ListParam[0].ItemList = new string[] 
                { 
                    "Gives a random entry signal"
                };
            else if (slotType == SlotTypes.CloseFilter)
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "Gives a random exit signal" 
                };
            else
                IndParam.ListParam[0].ItemList = new string[]
                {
                    "Not Defined"
                };
            IndParam.ListParam[0].Index = 0;
            IndParam.ListParam[0].Text = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Logic of application of the indicator.";

            // The NumericUpDown parameters
            if (slotType == SlotTypes.OpenFilter)
            {
                IndParam.NumParam[0].Caption = "Probability";
                IndParam.NumParam[0].Value   = 80;
                IndParam.NumParam[0].Min     = 0;
                IndParam.NumParam[0].Max     = 100;
                IndParam.NumParam[0].Enabled = true;
                IndParam.NumParam[0].ToolTip = "The probability to allow a new position opening in %.";

                IndParam.NumParam[1].Caption = "Long vs short";
                IndParam.NumParam[1].Value   = 50;
                IndParam.NumParam[1].Min     = 0;
                IndParam.NumParam[1].Max     = 100;
                IndParam.NumParam[1].Enabled = true;
                IndParam.NumParam[1].ToolTip = "The probability to open Long vs. short in %.";
            }
            else if (slotType == SlotTypes.CloseFilter)
            {
                IndParam.NumParam[0].Caption = "Probability";
                IndParam.NumParam[0].Value   = 20;
                IndParam.NumParam[0].Min     = 0;
                IndParam.NumParam[0].Max     = 100;
                IndParam.NumParam[0].Enabled = true;
                IndParam.NumParam[0].ToolTip = "The probability to close the position in %.";
            }

            return;
        }

        /// <summary>
        /// Calculates the indicator's components
        /// </summary>
        public override void Calculate(SlotTypes slotType)
        {
            // Reading the parameters
            int iProbability = (int)IndParam.NumParam[0].Value;
            int iLongShort   = (int)IndParam.NumParam[1].Value;

            Random random = new Random();

            // Saving the components
            if (slotType == SlotTypes.OpenFilter)
            {
                Component = new IndicatorComp[2];

                Component[0] = new IndicatorComp();
                Component[0].ChartType = IndChartType.NoChart;
                Component[0].FirstBar  = 0;
                Component[0].Value     = new double[Bars];
                Component[0].DataType  = IndComponentType.AllowOpenLong;
                Component[0].CompName  = "Is long entry allowed";

                Component[1] = new IndicatorComp();
                Component[1].ChartType = IndChartType.NoChart;
                Component[1].FirstBar  = 0;
                Component[1].Value     = new double[Bars];
                Component[1].DataType  = IndComponentType.AllowOpenShort;
                Component[1].CompName  = "Is short entry allowed";

                // Calculation of the logic
                for (int i = 0; i < Bars; i++)
                {
                    if (random.Next(100) < iProbability)
                    {
                        int iRandNumb = random.Next(100);
                        Component[0].Value[i] = (iRandNumb <= iLongShort) ? 1 : 0;
                        Component[1].Value[i] = (iRandNumb >  iLongShort) ? 1 : 0;
                    }
                    else
                    {
                        Component[0].Value[i] = 0;
                        Component[1].Value[i] = 0;
                    }
                }
            }
            else
            {
                Component = new IndicatorComp[1];

                Component[0] = new IndicatorComp();
                Component[0].ChartType = IndChartType.NoChart;
                Component[0].FirstBar  = 0;
                Component[0].Value     = new double[Bars];
                Component[0].DataType  = IndComponentType.ForceClose;
                Component[0].CompName  = "Force Close";

                for (int i = 0; i < Bars; i++)
                {
                    Component[0].Value[i] = (random.Next(100) < iProbability) ? 1 : 0;
                }
            }

            return;
        }

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {
            EntryFilterLongDescription  = "the " + ToString() + " allows a long position";
            EntryFilterShortDescription = "the " + ToString() + " allows a short position";
            ExitFilterLongDescription   = "the " + ToString() + " allows closing";
            ExitFilterShortDescription  = "the " + ToString() + " allows closing";

            return;
        }

        /// <summary>
        /// Indicator to string
        /// </summary>
        public override string ToString()
        {
            string sString = IndicatorName + " (" +
                IndParam.NumParam[0].ValueToString + ", " + // Probability
                IndParam.NumParam[1].ValueToString + ")";   // Long vs Short

            return sString;
        }
    }
}
