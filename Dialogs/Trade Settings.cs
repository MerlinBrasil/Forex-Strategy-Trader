// Trade Settings
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Trader
{
    class Trade_Settings : Form
    {
        Fancy_Panel pnlSettings;

        Label lblCloseAdvance;
        Label lblSlippageEntry;
        Label lblSlippageExit;
        Label lblLongLogicPrice;
        Label lblMinChartBars;

        ComboBox cbxLongLogicPrice;
        CheckBox chbAutoSlippage;
        NumericUpDown nudCloseAdvance;
        NumericUpDown nudSlippageEntry;
        NumericUpDown nudSlippageExit;
        NumericUpDown nudMinChartBars;

        Button  btnDefault;
        Button  btnAccept;
        Button  btnCancel;
        
        ToolTip toolTip = new ToolTip();

        Font   font;
        Color  colorText;

        /// <summary>
        /// Constructor
        /// </summary>
        public Trade_Settings()
        {
            pnlSettings  = new Fancy_Panel();

            lblCloseAdvance   = new Label();
            lblSlippageEntry  = new Label();

            cbxLongLogicPrice = new ComboBox();
            chbAutoSlippage   = new CheckBox();
            nudCloseAdvance   = new NumericUpDown();
            nudSlippageEntry  = new NumericUpDown();
            nudSlippageExit   = new NumericUpDown();
            lblSlippageExit   = new Label();
            lblLongLogicPrice = new Label();
            lblMinChartBars   = new Label();
            nudMinChartBars   = new NumericUpDown();

            btnDefault = new Button();
            btnCancel  = new Button();
            btnAccept  = new Button();

            font      = this.Font;
            colorText = LayoutColors.ColorControlText;

            MaximizeBox     = false;
            MinimizeBox     = false;
            ShowInTaskbar   = false;
            Icon            = Data.Icon;
            BackColor       = LayoutColors.ColorFormBack;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton    = btnAccept;
            Text            = Language.T("Trade Settings");

            // pnlAveraging
            pnlSettings.Parent = this;

            // ComboBox Long Logic Price
            cbxLongLogicPrice.Parent        = pnlSettings;
            cbxLongLogicPrice.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxLongLogicPrice.Items.AddRange(new string[] { Language.T("Bid"), Language.T("Ask") });
            cbxLongLogicPrice.SelectedIndex = (Configs.LongTradeLogicPrice == "Bid" ? 0 : 1);

            // Label close advance
            lblCloseAdvance.Parent    = pnlSettings;
            lblCloseAdvance.ForeColor = colorText;
            lblCloseAdvance.BackColor = Color.Transparent;
            lblCloseAdvance.AutoSize  = true;
            lblCloseAdvance.Text      = Language.T("'Bar Closing' time advance in seconds");

            // Check Box Auto Slippage
            chbAutoSlippage.Parent    = pnlSettings;
            chbAutoSlippage.ForeColor = colorText;
            chbAutoSlippage.BackColor = Color.Transparent;
            chbAutoSlippage.AutoSize  = true;
            chbAutoSlippage.Checked   = Configs.AutoSlippage;
            chbAutoSlippage.Text      = Language.T("Auto slippage depending on the spread.");
            chbAutoSlippage.CheckedChanged += new EventHandler(chbAutoSlippage_CheckedChanged);

            // Label Entry slippage
            lblSlippageEntry.Parent    = pnlSettings;
            lblSlippageEntry.ForeColor = colorText;
            lblSlippageEntry.BackColor = Color.Transparent;
            lblSlippageEntry.AutoSize  = true;
            lblSlippageEntry.Text      = Language.T("Slippage for entry orders");

            // Label Entry slippage
            lblSlippageExit.Parent    = pnlSettings;
            lblSlippageExit.ForeColor = colorText;
            lblSlippageExit.BackColor = Color.Transparent;
            lblSlippageExit.AutoSize  = true;
            lblSlippageExit.Text      = Language.T("Slippage for exit orders");

            // NumericUpDown Entry Lots
            nudCloseAdvance.Parent    = pnlSettings;
            nudCloseAdvance.BeginInit();
            nudCloseAdvance.Minimum   = 1;
            nudCloseAdvance.Maximum   = 15;
            nudCloseAdvance.Increment = 1;
            nudCloseAdvance.Value     = Configs.BarCloseAdvance;
            nudCloseAdvance.DecimalPlaces = 0;
            nudCloseAdvance.TextAlign = HorizontalAlignment.Center;
            nudCloseAdvance.EndInit();

            // Label lblLongLogicPrice
            lblLongLogicPrice.Parent    = pnlSettings;
            lblLongLogicPrice.ForeColor = colorText;
            lblLongLogicPrice.BackColor = Color.Transparent;
            lblLongLogicPrice.AutoSize  = true;
            lblLongLogicPrice.Text      = Language.T("Long logic rules base price");

            // NUD Entry slippage
            nudSlippageEntry.Parent    = pnlSettings;
            nudSlippageEntry.BeginInit();
            nudSlippageEntry.Minimum   = 0;
            nudSlippageEntry.Maximum   = 1000;
            nudSlippageEntry.Increment = 1;
            nudSlippageEntry.Value     = Configs.SlippageEntry;
            nudSlippageEntry.DecimalPlaces = 0;
            nudSlippageEntry.TextAlign = HorizontalAlignment.Center;
            nudSlippageEntry.Enabled   = !Configs.AutoSlippage;
            nudSlippageEntry.EndInit();

            // NUD Exit slippage
            nudSlippageExit.Parent    = pnlSettings;
            nudSlippageExit.BeginInit();
            nudSlippageExit.Minimum   = 0;
            nudSlippageExit.Maximum   = 1000;
            nudSlippageExit.Increment = 1;
            nudSlippageExit.Value     = Configs.SlippageExit;
            nudSlippageExit.DecimalPlaces = 0;
            nudSlippageExit.TextAlign = HorizontalAlignment.Center;
            nudSlippageExit.Enabled   = !Configs.AutoSlippage;
            nudSlippageExit.EndInit();

            // Label lblMinChartBars
            lblMinChartBars.Parent    = pnlSettings;
            lblMinChartBars.ForeColor = colorText;
            lblMinChartBars.BackColor = Color.Transparent;
            lblMinChartBars.AutoSize  = true;
            lblMinChartBars.Text      = Language.T("Minimum number of bars in the chart");

            // NUD Exit slippage
            nudMinChartBars.Parent    = pnlSettings;
            nudMinChartBars.BeginInit();
            nudMinChartBars.Minimum   = 300;
            nudMinChartBars.Maximum   = 5000;
            nudMinChartBars.Increment = 1;
            nudMinChartBars.Value     = Configs.MinChartBars;
            nudMinChartBars.DecimalPlaces = 0;
            nudMinChartBars.TextAlign = HorizontalAlignment.Center;
            nudMinChartBars.EndInit();

            //Button Default
            btnDefault.Parent = this;
            btnDefault.Name   = "Default";
            btnDefault.Text   = Language.T("Default");
            btnDefault.Click += new EventHandler(BtnDefault_Click);
            btnDefault.UseVisualStyleBackColor = true;

            //Button Cancel
            btnCancel.Parent       = this;
            btnCancel.Text         = Language.T("Cancel");
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.UseVisualStyleBackColor = true;

            //Button Accept
            btnAccept.Parent       = this;
            btnAccept.Name         = "Accept";
            btnAccept.Text         = Language.T("Accept");
            btnAccept.Click       += new EventHandler(BtnAccept_Click);
            btnAccept.DialogResult = DialogResult.OK;
            btnAccept.UseVisualStyleBackColor = true;

            return;
        }

        /// <summary>
        /// Changes the visibility of nuds slippage.
        /// </summary>
        void chbAutoSlippage_CheckedChanged(object sender, EventArgs e)
        {
            nudSlippageEntry.Enabled = !chbAutoSlippage.Checked;
            nudSlippageExit.Enabled  = !chbAutoSlippage.Checked;
        }

        /// <summary>
        /// Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            int iButtonWidth = (int)(Data.HorizontalDLU * 60);
            int iBtnHrzSpace = (int)(Data.HorizontalDLU * 3);

            ClientSize = new Size(3 * iButtonWidth + 4 * iBtnHrzSpace, 245);

            btnAccept.Focus();

            return;
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
            int iSpace        = iBtnHrzSpace;
            int iTextHeight   = Font.Height;
            int iBorder       = 2;

            pnlSettings.Size     = new Size(ClientSize.Width - 2 * iSpace, ClientSize.Height - iSpace - iButtonHeight - 2 * iBtnVertSpace);
            pnlSettings.Location = new Point(iSpace, iSpace);

            int iCBxWith  = 60;
            int iCBxLeft  = pnlSettings.ClientSize.Width - iCBxWith - iSpace - iBorder;
            int iNudWidth = 60;
            int iNudLeft  = pnlSettings.ClientSize.Width - iNudWidth - iBtnHrzSpace - iBorder;

            lblLongLogicPrice.Location = new Point(iBtnHrzSpace,19);
            cbxLongLogicPrice.Width    = iCBxWith;
            cbxLongLogicPrice.Location = new Point(iCBxLeft, 15);

            lblCloseAdvance.Location   = new Point(iBtnHrzSpace, 47);
            nudCloseAdvance.Size       = new Size(iNudWidth, iButtonHeight);
            nudCloseAdvance.Location   = new Point(iNudLeft, 45);

            chbAutoSlippage.Location   = new Point(iBtnHrzSpace + 3, 77);

            lblSlippageEntry.Location  = new Point(iBtnHrzSpace, 107);
            nudSlippageEntry.Size      = new Size(iNudWidth, iButtonHeight);
            nudSlippageEntry.Location  = new Point(iNudLeft, 105);

            lblSlippageExit.Location   = new Point(iBtnHrzSpace, 137);
            nudSlippageExit.Size       = new Size(iNudWidth, iButtonHeight);
            nudSlippageExit.Location   = new Point(iNudLeft, 135);

            lblMinChartBars.Location   = new Point(iBtnHrzSpace, 167);
            nudMinChartBars.Size       = new Size(iNudWidth, iButtonHeight);
            nudMinChartBars.Location   = new Point(iNudLeft, 165);

            iButtonWidth = (pnlSettings.Width - 2 * iBtnHrzSpace) / 3;

            // Button Cancel
            btnCancel.Size     = new Size(iButtonWidth, iButtonHeight);
            btnCancel.Location = new Point(ClientSize.Width - iButtonWidth - iBtnHrzSpace, ClientSize.Height - iButtonHeight - iBtnVertSpace);

            // Button Default
            btnDefault.Size     = new Size(iButtonWidth, iButtonHeight);
            btnDefault.Location = new Point(btnCancel.Left - iButtonWidth - iBtnHrzSpace, ClientSize.Height - iButtonHeight - iBtnVertSpace);

            // Button Accept
            btnAccept.Size     = new Size(iButtonWidth, iButtonHeight);
            btnAccept.Location = new Point(btnDefault.Left - iButtonWidth - iBtnHrzSpace, ClientSize.Height - iButtonHeight - iBtnVertSpace);

            return;
        }

        /// <summary>
        /// Button Default Click
        /// </summary>
        void BtnDefault_Click(object sender, EventArgs e)
        {
            cbxLongLogicPrice.SelectedIndex = 0;
            nudCloseAdvance.Value   = 3;
            nudSlippageEntry.Value  = 5;
            nudSlippageExit.Value   = 10;
            nudMinChartBars.Value   = 400;

            return;
        }

        /// <summary>
        /// Button Default Click
        /// </summary>
        void BtnAccept_Click(object sender, EventArgs e)
        {
            Configs.LongTradeLogicPrice = (cbxLongLogicPrice.SelectedIndex == 0 ? "Bid" : "Ask");
            Configs.BarCloseAdvance     = (int)nudCloseAdvance.Value;
            Configs.AutoSlippage        = chbAutoSlippage.Checked;
            Configs.SlippageEntry       = (int)nudSlippageEntry.Value;
            Configs.SlippageExit        = (int)nudSlippageExit.Value;
            Configs.MinChartBars        = (int)nudMinChartBars.Value;
        }

        /// <summary>
        /// Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }
    }
}
