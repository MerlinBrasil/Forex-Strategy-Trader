// IndicatorParam Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Text;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// IndicatorParam class.
    /// </summary>
    public class IndicatorParam
    {
        int             slotNumb;
        SlotTypes       slotType;
        bool            isDefined;
        string          indicatorName;
        TypeOfIndicator typeOfIndicator;
        ExecutionTime   timeExecution;
        ListParam[]     aListParam;
        NumericParam[]  aNumParam;
        CheckParam[]    aCheckParam;

        /// <summary>
        /// Gets or sets the number of current slot.
        /// </summary>
        public int SlotNumber { get { return slotNumb; } set { slotNumb = value; } }

        /// <summary>
        /// Gets or sets the type of the slot.
        /// </summary>
        public SlotTypes SlotType { get { return slotType; } set { slotType = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether the indicator is defined.
        /// </summary>
        public bool IsDefined { get { return isDefined; } set { isDefined = value; } }

        /// <summary>
        /// Gets or sets the indicator name.
        /// </summary>
        public string IndicatorName { get { return indicatorName; } set { indicatorName = value; } }

        /// <summary>
        /// Gets or sets the type of the indicator
        /// </summary>
        public TypeOfIndicator IndicatorType { get { return typeOfIndicator; } set { typeOfIndicator = value; } }

        /// <summary>
        /// Gets or sets the type of the time execution of the indicator
        /// </summary>
        public ExecutionTime ExecutionTime { get { return timeExecution; } set { timeExecution = value; } }

        /// <summary>
        /// Gets or sets a parameter represented by a ComboBox.
        /// </summary>
        public ListParam[] ListParam { get { return aListParam; } set { aListParam = value; } }

        /// <summary>
        /// Gets or sets a parameter represented by a NumericUpDown.
        /// </summary>
        public NumericParam[] NumParam { get { return aNumParam; } set { aNumParam = value; } }

        /// <summary>
        /// Gets or sets a parameter represented by a CheckBox.
        /// </summary>
        public CheckParam[] CheckParam { get { return aCheckParam; } set { aCheckParam = value; } }

        /// <summary>
        /// Creates empty parameters.
        /// </summary>
        public IndicatorParam()
        {
            slotNumb        = 0;
            isDefined       = false;
            slotType        = SlotTypes.NotDefined;
            indicatorName   = String.Empty;
            typeOfIndicator = TypeOfIndicator.Indicator;
            timeExecution   = ExecutionTime.DuringTheBar;
            aListParam      = new ListParam[5];
            aNumParam       = new NumericParam[6];
            aCheckParam     = new CheckParam[2];

            for (int i = 0; i < 5; i++)
                aListParam[i] = new ListParam();

            for (int i = 0; i < 6; i++)
                aNumParam[i] = new NumericParam();

            for (int i = 0; i < 2; i++)
                aCheckParam[i] = new CheckParam();
        }

        /// <summary>
        /// Returns a copy
        /// </summary>
        public IndicatorParam Clone()
        {
            IndicatorParam iparam = new IndicatorParam();

            iparam.slotNumb        = slotNumb;
            iparam.isDefined       = isDefined;
            iparam.slotType        = slotType;
            iparam.indicatorName   = indicatorName;
            iparam.typeOfIndicator = typeOfIndicator;
            iparam.timeExecution   = timeExecution;
            iparam.aListParam      = new ListParam[5];
            iparam.aNumParam       = new NumericParam[6];
            iparam.aCheckParam     = new CheckParam[2];

            for (int i = 0; i < 5; i++)
                iparam.aListParam[i] = aListParam[i].Clone();

            for (int i = 0; i < 6; i++)
                iparam.aNumParam[i] = aNumParam[i].Clone();

            for (int i = 0; i < 2; i++)
                iparam.aCheckParam[i] = aCheckParam[i].Clone();

            return iparam;
        }

        /// <summary>
        /// Represents the indicator parameters in a readable form.
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach(ListParam listParam in aListParam)
                if (listParam.Enabled)
                    sb.AppendLine(listParam.Caption + " - " + listParam.Text);

            foreach (NumericParam numParam in aNumParam)
                if (numParam.Enabled)
                    sb.AppendLine(numParam.Caption + " - " + numParam.ValueToString);

            foreach (CheckParam checkParam in aCheckParam)
                if (checkParam.Enabled)
                    sb.AppendLine(checkParam.Caption + " - " + (checkParam.Checked ? "Yes" : "No"));

            return sb.ToString();
        }
    }
}
