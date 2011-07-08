// Strategy_Publish Form
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Trader
{
    public class Strategy_Publish : Form
    {
        Fancy_Panel pnlBBCodeBase;
        Fancy_Panel pnlInfoBase;
        TextBox txboxBBCode;
        Label   lblInformation;
        Button  btnClose;
        Button  btnConnect;

        /// <summary>
        /// Make a form
        /// </summary>
        public Strategy_Publish()
        {
            pnlBBCodeBase  = new Fancy_Panel();
            pnlInfoBase    = new Fancy_Panel();
            txboxBBCode    = new TextBox();
            lblInformation = new Label();
            btnClose       = new Button();
            btnConnect     = new Button();
            
            // BBCode_viewer
            AcceptButton = btnClose;
            BackColor    = LayoutColors.ColorFormBack;
            Icon         = Data.Icon;
            Controls.Add(btnConnect);
            Controls.Add(btnClose);
            Controls.Add(pnlBBCodeBase);
            Controls.Add(pnlInfoBase);
            MinimumSize = new System.Drawing.Size(400, 400);
            Text = Language.T("Publish a Strategy");

            pnlBBCodeBase.Padding = new Padding(4, 4, 2, 2);
            pnlInfoBase.Padding   = new Padding(4, 4, 2, 2);

            // txboxBBCode
            txboxBBCode.Parent        = pnlBBCodeBase;
            txboxBBCode.BorderStyle   = BorderStyle.None;
            txboxBBCode.Dock          = DockStyle.Fill;
            txboxBBCode.BackColor     = LayoutColors.ColorControlBack;
            txboxBBCode.ForeColor     = LayoutColors.ColorControlText;
            txboxBBCode.Multiline     = true;
            txboxBBCode.AcceptsReturn = true;
            txboxBBCode.AcceptsTab    = true;
            txboxBBCode.ScrollBars    = ScrollBars.Vertical;
            txboxBBCode.KeyDown      += new KeyEventHandler(TxboxBBCode_KeyDown);
            txboxBBCode.Text          = Data.Strategy.GenerateBBCode();

            // lblInformation
            lblInformation.Parent      = pnlInfoBase;
            lblInformation.Dock        = DockStyle.Fill;
            lblInformation.BackColor   = Color.Transparent;
            lblInformation.ForeColor   = LayoutColors.ColorControlText;
            string strInfo = Language.T("Publishing a strategy in the program's forum:") + Environment.NewLine +
                "1) " + Language.T("Open a new topic in the forum") + " \"Trading Strategies\";" + Environment.NewLine +
                "2) " + Language.T("Copy / Paste the following code;") + Environment.NewLine +
                "3) " + Language.T("Describe the strategy.");
            lblInformation.Text = strInfo;

            // btnClose
            btnClose.Text   = Language.T("Close");
            btnClose.Click += new System.EventHandler(btnClose_Click);
            btnClose.UseVisualStyleBackColor = true;

            // btnConnect
            btnConnect.Text   = Language.T("Connect to") + " http://forexsb.com/forum";
            btnConnect.Click += new System.EventHandler(btnConnect_Click);
            btnConnect.UseVisualStyleBackColor = true;
        }

        /// <summary>
        /// Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            int iButtonWidth = (int)(Data.HorizontalDLU * 60);
            int iBtnHrzSpace = (int)(Data.HorizontalDLU * 3);
            ClientSize  = new Size(4 * iButtonWidth + 3 * iBtnHrzSpace, 480);
            MinimumSize = new Size(Width, 300);
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            int iButtonHeight = (int)(Data.VerticalDLU * 15.5);
            int iButtonWidth  = (int)(Data.HorizontalDLU * 60);
            int iBtnVertSpace = (int)(Data.VerticalDLU * 5.5);
            int iBtnHrzSpace  = (int)(Data.HorizontalDLU * 3);
            int iBorder       = iBtnHrzSpace;
            int iTextHeight   = Font.Height;

            // Button Close
            btnClose.Size     = new Size(iButtonWidth, iButtonHeight);
            btnClose.Location = new Point(ClientSize.Width - iButtonWidth - iBtnHrzSpace, ClientSize.Height - iButtonHeight - iBtnVertSpace);

            // Button Connect
            btnConnect.Size     = new Size(3 * iButtonWidth, iButtonHeight);
            btnConnect.Location = new Point(btnClose.Left - btnConnect.Width - iBtnHrzSpace, ClientSize.Height - iButtonHeight - iBtnVertSpace);

            // pnlInfoBase
            pnlInfoBase.Size     = new Size(ClientSize.Width - 2 * iBorder, 65);
            pnlInfoBase.Location = new Point(iBorder, iBorder);

            // pnlBBCodeBase 
            pnlBBCodeBase.Size     = new Size(ClientSize.Width - 2 * iBorder, btnClose.Top - pnlInfoBase.Bottom - iBtnVertSpace - iBorder);
            pnlBBCodeBase.Location = new Point(iBorder, pnlInfoBase.Bottom + iBorder);

            return;
        }

        /// <summary>
        /// Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }

        /// <summary>
        /// Accept Ctrl-A
        /// </summary>
        void TxboxBBCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.A))
            {
                ((TextBox)sender).SelectAll();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        /// </summary>
        /// Connects to the forum
        /// </summary>
        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://forexsb.com/forum/");
            }
            catch { }
        }

        /// </summary>
        /// Closes the form
        /// </summary>
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
