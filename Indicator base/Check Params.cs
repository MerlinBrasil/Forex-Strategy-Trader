// CheckParam Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Describes a parameter who can be used or not.
    /// </summary>
    public class CheckParam
    {
        private string caption;
        private bool   isChecked;
        private bool   isEnabled;
        private string toolTip;

        /// <summary>
        /// Gets or sets the text describing the parameter.
        /// </summary>
        public string Caption { get { return caption; } set { caption = value; } }

        /// <summary>
        /// Gets or sets the value indicating whether the control is checked.
        /// </summary>
        public bool Checked { get { return isChecked; } set { isChecked = value; } }

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
        public CheckParam()
        {
            caption   = String.Empty;
            isEnabled = false;
            isChecked = false;
            toolTip   = String.Empty;
        }

        /// <summary>
        /// Returns a copy of the class.
        /// </summary>
        public CheckParam Clone()
        {
            CheckParam cparam = new CheckParam();

            cparam.caption   = caption;
            cparam.isEnabled = isEnabled;
            cparam.isChecked = isChecked;
            cparam.toolTip   = toolTip;

            return cparam;
        }
    }
}
