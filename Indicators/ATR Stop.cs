// ATR Stop Indicator
// Last changed on 2010-07-15
// Copyright (c) 2006 - 2010 Miroslav Popov - All rights reserved.
// Part of Forex Strategy Builder & Forex Strategy Trader
// Website http://forexsb.com/
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// ATR Stop Indicator
    /// </summary>
    public class ATR_Stop : Indicator
    {
        /// <summary>
        /// Sets the default indicator parameters for the designated slot type
        /// </summary>
        public ATR_Stop(SlotTypes slotType)
        {
            // General properties
            IndicatorName = "ATR Stop";
            PossibleSlots = SlotTypes.Close;

            // Setting up the indicator parameters
            IndParam = new IndicatorParam();
            IndParam.IndicatorName = IndicatorName;
            IndParam.SlotType      = slotType;
            IndParam.IndicatorType = TypeOfIndicator.Additional;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption  = "Logic";
            IndParam.ListParam[0].ItemList = new string[]
            {
                "Exit at the ATR Stop level"
            };
            IndParam.ListParam[0].Index   = 0;
            IndParam.ListParam[0].Text    = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Logic of application of the indicator.";

            IndParam.ListParam[1].Caption  = "Smoothing method";
            IndParam.ListParam[1].ItemList = Enum.GetNames(typeof(MAMethod));
            IndParam.ListParam[1].Index    = (int)MAMethod.Simple;
            IndParam.ListParam[1].Text     = IndParam.ListParam[1].ItemList[IndParam.ListParam[1].Index];
            IndParam.ListParam[1].Enabled  = true;
            IndParam.ListParam[1].ToolTip  = "The Moving Average method used for smoothing the ATR.";

            IndParam.ListParam[2].Caption  = "Base price";
            IndParam.ListParam[2].ItemList = new string[] { "Bar range" };
            IndParam.ListParam[2].Index    = 0;
            IndParam.ListParam[2].Text     = IndParam.ListParam[2].ItemList[IndParam.ListParam[2].Index];
            IndParam.ListParam[2].Enabled  = true;
            IndParam.ListParam[2].ToolTip  = "ATR uses the range of the current bar";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "Smoothing period";
            IndParam.NumParam[0].Value   = 14;
            IndParam.NumParam[0].Min     = 1;
            IndParam.NumParam[0].Max     = 200;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "The period of ATR smoothing.";

            IndParam.NumParam[1].Caption = "Multiplier";
            IndParam.NumParam[1].Value   = 2;
            IndParam.NumParam[1].Min     = 1;
            IndParam.NumParam[1].Max     = 10;
            IndParam.NumParam[1].Point   = 2;
            IndParam.NumParam[1].Enabled = true;
            IndParam.NumParam[1].ToolTip = "Determines the stop level.";

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
            MAMethod maMethod = (MAMethod)IndParam.ListParam[1].Index;
            int period = (int)IndParam.NumParam[0].Value;
            int multipl = (int)IndParam.NumParam[1].Value;
            int prev = IndParam.CheckParam[0].Checked ? 1 : 0;

            // Calculation
            int firstBar = period + 2;
	
			double[] ATR = new double[Bars];

            for (int bar = 1; bar < Bars; bar++)
                ATR[bar] = Math.Max(High[bar], Close[bar - 1]) - Math.Min(Low[bar], Close[bar - 1]);

            ATR = MovingAverage(period, 0, maMethod, ATR);

			double[] ATRStop = new double[Bars];
            double minStop = 5 * Point;

            for (int bar = firstBar; bar < Bars - prev; bar++)
                ATRStop[bar + prev] = Math.Max(ATR[bar] * multipl, minStop);
            
            // Saving the components
            Component = new IndicatorComp[2];

            Component[0] = new IndicatorComp();
            Component[0].CompName      = "ATR Stop margin";
            Component[0].DataType      = IndComponentType.IndicatorValue;
            Component[0].FirstBar      = firstBar;
            Component[0].ShowInDynInfo = false;
            Component[0].Value         = ATRStop;

			Component[1]			   = new IndicatorComp();
            Component[1].CompName      = "ATR Stop for the transferred position";
			Component[1].DataType	   = IndComponentType.Other;
            Component[1].ShowInDynInfo = false;
			Component[1].FirstBar	   = firstBar;
			Component[1].Value	       = new double[Bars];

            return;
        }

        /// <summary>
        /// Sets the indicator logic description
        /// </summary>
        public override void SetDescription(SlotTypes slotType)
        {
            ExitPointLongDescription  = "when the market falls to the " + ToString() + " level";
            ExitPointShortDescription = "when the market rises to the " + ToString() + " level";

            return;
        }

        /// <summary>
        /// Indicator to string
        /// </summary>
        public override string ToString()
        {
            string indicator = IndicatorName +
                (IndParam.CheckParam[0].Checked ? "* (" : " (") +
                IndParam.ListParam[1].Text         + ", " + // Smoothing method
                IndParam.NumParam[0].ValueToString + ", " + // Smoothing period
                IndParam.NumParam[1].ValueToString + ")" ;  // Multiplier

            return indicator;
        }
    }
}