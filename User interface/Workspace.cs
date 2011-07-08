// Workspace Form
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// This is the base application form.
    /// </summary>
    public class Workspace : Form
    {
        protected Panel       pnlWorkspace;
        protected StatusStrip statusStrip;
        protected ToolTip     toolTip;
        protected ToolStrip   tsTradeControl;

        protected int space = 4;

        /// <summary>
        /// The default constructor sets the base controls.
        /// </summary>
        public Workspace()
        {
            // Graphical measures
            Graphics g = CreateGraphics();
            SizeF sizeString   = g.MeasureString("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890", Font);
            Data.HorizontalDLU = (sizeString.Width / 62) / 4;
            Data.VerticalDLU   = sizeString.Height / 8;
            g.Dispose();

            toolTip = new ToolTip();

            tsTradeControl = new ToolStrip();
            MainMenuStrip  = new MenuStrip();
            pnlWorkspace   = new Panel();
            statusStrip    = new StatusStrip();

            // Panel Workspace
            pnlWorkspace.Parent  = this;
            pnlWorkspace.Dock    = DockStyle.Fill;
            pnlWorkspace.Padding = new Padding(2);

            // Tool Strip Trade control
            tsTradeControl.Parent = this;
            tsTradeControl.Dock   = DockStyle.Top;

            // Main menu
            MainMenuStrip.Parent = this;
            MainMenuStrip.Dock   = DockStyle.Top;

            // Status bar
            statusStrip.Parent = this;
            statusStrip.Dock   = DockStyle.Bottom;

            return;
        }
    }
}
