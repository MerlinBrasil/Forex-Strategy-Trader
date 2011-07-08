// NumericParam Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Describes a parameter represented by means of a NumericUpDown control.
    /// </summary>
    public class NumericParam
    {
        private string caption;
        private double value;
        private double min;
        private double max;
        private int    point;
        private bool   isEnabled;
        private string toolTip;

        /// <summary>
        /// Gets or sets the text describing the parameter.
        /// </summary>
        public string Caption { get { return caption; } set { caption = value; } }

        /// <summary>
        /// Gets or sets the value of parameter.
        /// </summary>
        public double Value { get { return value; } set { this.value = value; } }

        /// <summary>
        /// Gets the value of parameter as a string.
        /// </summary>
        public string ValueToString { get { return String.Format("{0:F" + point.ToString() + "}", value); } }

        /// <summary>
        /// Gets the corrected value of parameter as a string.
        /// </summary>
        public string AnotherValueToString(double dAnotherValue)
        {
            return String.Format("{0:F" + point.ToString() + "}", dAnotherValue);
        }

        /// <summary>
        /// Gets or sets the minimum value of parameter.
        /// </summary>
        public double Min { get { return min; } set { min = value; } }

        /// <summary>
        /// Gets or sets the maximum value of parameter.
        /// </summary>
        public double Max { get { return max; } set { max = value; } }

        /// <summary>
        /// Gets or sets the number of meaning decimal points of parameter.
        /// </summary>
        public int Point { get { return point; } set { point = value; } }

        /// <summary>
        /// Gets or sets the value indicating whether the control can respond to user interaction.
        /// </summary>
        public bool Enabled { get { return isEnabled; } set { isEnabled = value; } }

        /// <summary>
        /// Gets or sets the text of tool tip associated with this control.
        /// </summary>
        public string ToolTip { get { return toolTip; } set { toolTip = value; } }

        /// <summary>
        /// The default constructor.
        /// </summary>
        public NumericParam()
        {
            caption   = String.Empty;
            value     = 0;
            min       = 0;
            max       = 100;
            point     = 0;
            isEnabled = false;
            toolTip   = String.Empty;
        }

        /// <summary>
        /// Returns a copy
        /// </summary>
        public NumericParam Clone()
        {
            NumericParam nparam = new NumericParam();

            nparam.caption   = caption;
            nparam.value     = value;
            nparam.min       = min;
            nparam.max       = max;
            nparam.point     = point;
            nparam.isEnabled = isEnabled;
            nparam.toolTip   = toolTip;

            return nparam;
        }
    }
}
