// ListParam Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Describes a parameter that has to be selected from a list.
    /// </summary>
    public class ListParam
    {
        private string   caption;
        private string[] asItemList;
        private string   text;
        private int      index;
        private bool     isEnabled;
        private string   toolTip;

        /// <summary>
        /// Gets or sets the text describing the parameter.
        /// </summary>
        public string Caption { get { return caption; } set { caption = value; } }

        /// <summary>
        /// Gets or sets the list of parameter values.
        /// </summary>
        public string[] ItemList { get { return asItemList; } set { asItemList = value; } }

        /// <summary>
        /// Gets or sets the text associated whit this parameter.
        /// </summary>
        public string Text { get { return text; } set { text = value; } }

        /// <summary>
        /// Gets or sets the index specifying the currently selected item.
        /// </summary>
        public int Index { get { return index; } set { index = value; } }

        /// <summary>
        /// Gets or sets the value indicating whether the control can respond to user interaction.
        /// </summary>
        public bool Enabled { get { return isEnabled; } set { isEnabled = value; } }

        /// <summary>
        /// Gets or sets the text of tool tip associated with this control.
        /// </summary>
        public string ToolTip { get { return toolTip; } set { toolTip = value; } }

        /// <summary>
        /// Zeroing the parameters.
        /// </summary>
        public ListParam()
        {
            caption    = String.Empty;
            asItemList = new string[] { "" };
            index      = 0;
            text       = String.Empty;
            isEnabled  = false;
            toolTip    = String.Empty;
        }

        /// <summary>
        /// Returns a copy
        /// </summary>
        public ListParam Clone()
        {
            ListParam lparam = new ListParam();

            lparam.caption    = caption;
            lparam.asItemList = new string[asItemList.Length];
            asItemList.CopyTo(lparam.asItemList, 0);
            lparam.index      = index;
            lparam.text       = text;
            lparam.isEnabled  = isEnabled;
            lparam.toolTip    = toolTip;

            return lparam;
        }
    }
}
