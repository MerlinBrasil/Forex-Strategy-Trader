// IndicatorSlot Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

namespace Forex_Strategy_Trader
{
    public enum OppositeDirSignalAction { Nothing, Reduce, Close, Reverse }
    public enum SameDirSignalAction { Nothing, Winner, Add }

    /// <summary>
    /// Class IndicatorSlot.
    /// </summary>
    public class IndicatorSlot
    {
        int       slotNumb;
        SlotTypes slotType;
        string    group;
        bool      isDefined;
        bool      isLocked;
        string    indicatorName;
        bool      isSeparatedChart;
        IndicatorParam  indicatorParam;
        IndicatorComp[] component;
        double[]  adSpecValue;
        double    minValue;
        double    maxValue;

        /// <summary>
        /// Gets or sets the number of the slot.
        /// </summary>
        public int SlotNumber { get { return slotNumb; } set { slotNumb = value; } }

        /// <summary>
        /// Gets or sets the type of the slot.
        /// </summary>
        public SlotTypes SlotType { get { return slotType; } set { slotType = value; } }

        /// <summary>
        /// Gets or sets the logical group of the slot.
        /// </summary>
        public string LogicalGroup { get { return group; } set { group = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether the indicator is defined.
        /// </summary>
        public bool IsDefined { get { return isDefined; } set { isDefined = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether it is a locked slot (Generator)
        /// </summary>
        public bool IsLocked { get { return isLocked; } set { isLocked = value; } }

        /// <summary>
        /// Gets or sets the indicator name.
        /// </summary>
        public string IndicatorName { get { return indicatorName; } set { indicatorName = value; } }

        /// <summary>
        /// Gets or sets the indicator parameters.
        /// </summary>
        public IndicatorParam IndParam { get { return indicatorParam; } set { indicatorParam = value; } }

        /// <summary>
        /// If the chart is drown in separated panel.
        /// </summary>
        public bool SeparatedChart { get { return isSeparatedChart; } set { isSeparatedChart = value; } }

        /// <summary>
        /// Gets or sets an indicator component.
        /// </summary>
        public IndicatorComp[] Component { get { return component; } set { component = value; } }

        /// <summary>
        /// Gets or sets an indicator's special values.
        /// </summary>
        public double[] SpecValue { get { return adSpecValue; } set { adSpecValue = value; } }

        /// <summary>
        /// Gets or sets an indicator's min value.
        /// </summary>
        public double MinValue { get { return minValue; } set { minValue = value; } }

        /// <summary>
        /// Gets or sets an indicator's max value.
        /// </summary>
        public double MaxValue { get { return maxValue; } set { maxValue = value; } }

        /// <summary>
        ///  The default constructor.
        /// </summary>
        public IndicatorSlot()
        {
            slotNumb         = 0;
            slotType         = SlotTypes.NotDefined;
            group            = "";
            isDefined        = false;
            isLocked         = false;
            indicatorName    = "Not defined";
            indicatorParam   = new IndicatorParam();
            isSeparatedChart = false;
            component        = new IndicatorComp[] { };
            adSpecValue      = new double[] { };
            minValue         = double.MaxValue;
            maxValue         = double.MinValue;
        }

        /// <summary>
        ///  Returns a copy
        /// </summary>
        public IndicatorSlot Clone()
        {
            IndicatorSlot slot = new IndicatorSlot();
            slot.slotNumb         = slotNumb;
            slot.slotType         = slotType;
            slot.LogicalGroup     = group;
            slot.isDefined        = isDefined;
            slot.indicatorName    = indicatorName;
            slot.isSeparatedChart = isSeparatedChart;
            slot.minValue         = minValue;
            slot.maxValue         = maxValue;
            slot.indicatorParam   = indicatorParam.Clone();

            slot.adSpecValue = new double[adSpecValue.Length];
            adSpecValue.CopyTo(slot.adSpecValue, 0);

            slot.component = new IndicatorComp[component.Length];
            for (int i = 0; i < component.Length; i++)
                slot.component[i] = component[i].Clone();

            return slot;
        }
    }
}
