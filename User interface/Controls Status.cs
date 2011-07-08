// Status page - Controls class 
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Class Controls : Menu_and_StatusBar
    /// </summary>
    public partial class Controls : Menu_and_StatusBar
    {
        Fancy_Panel pnlConnection;
        Label lblConnection;
        
        Fancy_Panel pnlDataInfoBase;
        Panel       pnlDataInfoButtons;
        TextBox     tbxDataInfo;

        Info_Panel pnlMarketInfo;
        protected LinkPanel pnlUsefulLinks;
        protected LinkPanel pnlForexBrokers;

        Button btnShowMarketInfo;
        Button btnShowBars;
        Button btnShowAccountInfo;

        /// <summary>
        /// Sets the controls in tabPageStatus
        /// </summary>
        void Initialize_PageStatus()
        {
            // tabPageStatus
            tabPageStatus.Name = "tabPageStatus";
            tabPageStatus.Text = Language.T("Status");
            tabPageStatus.ImageIndex = 0;
            tabPageStatus.Resize += new EventHandler(TabPageStatus_Resize);

            // Panel Connection
            pnlConnection = new Fancy_Panel(Language.T("Connection Status"));
            pnlConnection.Parent = tabPageStatus;

            // lblConnection
            lblConnection = new Label();
            lblConnection.Name      = "lblConnection";
            lblConnection.Parent    = pnlConnection;
            lblConnection.Text      = Language.T("Not Connected. You have to connect to a MetaTrader terminal.");
            lblConnection.TextAlign = ContentAlignment.MiddleLeft;

            // Panel Data Info
            pnlDataInfoBase = new Fancy_Panel(Language.T("Data Info"));
            pnlDataInfoBase.Parent  = tabPageStatus;
            pnlDataInfoBase.Padding = new Padding(2, (int)pnlDataInfoBase.CaptionHeight, 2, 2);

            tbxDataInfo = new TextBox();
            tbxDataInfo.Parent        = pnlDataInfoBase;
            tbxDataInfo.BorderStyle   = BorderStyle.None;
            tbxDataInfo.Dock          = DockStyle.Fill;
            tbxDataInfo.TabStop       = false;
            tbxDataInfo.Multiline     = true;
            tbxDataInfo.AcceptsReturn = true;
            tbxDataInfo.AcceptsTab    = true;
            tbxDataInfo.WordWrap      = false;
            tbxDataInfo.ScrollBars    = ScrollBars.Vertical;
            tbxDataInfo.Font          = new Font("Courier New", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(204)));

            pnlDataInfoButtons = new Panel();
            pnlDataInfoButtons.Parent = pnlDataInfoBase;
            pnlDataInfoButtons.Dock   = DockStyle.Top;
            pnlDataInfoButtons.Paint += new PaintEventHandler(PnlDataInfoButtons_Paint);

            btnShowMarketInfo = new Button();
            btnShowMarketInfo.Parent = pnlDataInfoButtons;
            btnShowMarketInfo.Text   = Language.T("Market Info");
            btnShowMarketInfo.Click += new EventHandler(BtnShowMarketInfo_Click);
            btnShowMarketInfo.UseVisualStyleBackColor = true;

            btnShowAccountInfo = new Button();
            btnShowAccountInfo.Parent = pnlDataInfoButtons;
            btnShowAccountInfo.Text   = Language.T("Account Info");
            btnShowAccountInfo.Click += new EventHandler(BtnShowAccountInfo_Click);
            btnShowAccountInfo.UseVisualStyleBackColor = true;

            btnShowBars = new Button();
            btnShowBars.Parent = pnlDataInfoButtons;
            btnShowBars.Text   = Language.T("Loaded Bars");
            btnShowBars.Click += new EventHandler(BtnShowBars_Click);
            btnShowBars.UseVisualStyleBackColor = true;

            // Panel Market Info
            pnlMarketInfo = new Info_Panel(Language.T("Market Information"));
            pnlMarketInfo.Parent = tabPageStatus;

            // Panel Useful Links
            pnlUsefulLinks = new LinkPanel(Language.T("Useful Links"));
            pnlUsefulLinks.Parent = tabPageStatus;

            // Panel Forex Brokers
            pnlForexBrokers = new LinkPanel(Language.T("Forex Brokers"));
            pnlForexBrokers.Parent = tabPageStatus;

            SetStatusColors();

            return;
        }

        void PnlDataInfoButtons_Paint(object sender, PaintEventArgs e)
        {
            Panel pnl = (Panel)sender;
            Graphics g = e.Graphics;

            // Paint the panel background
            Data.GradientPaint(g, pnl.ClientRectangle, LayoutColors.ColorControlBack, LayoutColors.DepthCaption);

            return;
        }

        /// <summary>
        /// Sets controls size and position after resizing.
        /// </summary>
        void TabPageStatus_Resize(object sender, EventArgs e)
        {
            tabPageStatus.SuspendLayout();

            int iBorder = 2;

            int iWidth = tabPageStatus.ClientSize.Width;
            int iHeight = tabPageStatus.ClientSize.Height;
            pnlMarketInfo.Size     = new Size(220, 150);
            pnlMarketInfo.Location = new Point(iWidth - pnlMarketInfo.Width, 0);

            pnlConnection.Size     = new Size(pnlMarketInfo.Left - space, 110);
            pnlConnection.Location = new Point(0, 0);

            lblConnection.Size     = new Size(pnlConnection.Width - 2 * space - 2 * iBorder, 21);
            lblConnection.Location = new Point(iBorder + space, (int)pnlConnection.CaptionHeight + space);

            pnlDataInfoBase.Size     = new Size(pnlConnection.Width, iHeight - pnlConnection.Bottom - space);
            pnlDataInfoBase.Location = new Point(0, pnlConnection.Bottom + space);

            pnlUsefulLinks.Size     = new Size(pnlMarketInfo.Width, (iHeight - pnlMarketInfo.Bottom - 2 * space) / 2);
            pnlUsefulLinks.Location = new Point(pnlMarketInfo.Left, pnlMarketInfo.Bottom + space);

            pnlForexBrokers.Size     = new Size(pnlMarketInfo.Width, iHeight - pnlUsefulLinks.Bottom - space);
            pnlForexBrokers.Location = new Point(pnlMarketInfo.Left, pnlUsefulLinks.Bottom + space);
            
            int iButtonWith   = 100;
            int iButtonHeight = btnShowAccountInfo.Height;
            pnlDataInfoButtons.Height = iButtonHeight + 2 * space;

            btnShowMarketInfo.Width  = iButtonWith;
            btnShowAccountInfo.Width = iButtonWith;
            btnShowBars.Width        = iButtonWith;
            btnShowMarketInfo.Location  = new Point(space, space);
            btnShowAccountInfo.Location = new Point(btnShowMarketInfo.Right  + space, space);
            btnShowBars.Location        = new Point(btnShowAccountInfo.Right + space, space);

            tabPageStatus.ResumeLayout();

            return;
        }

        /// <summary>
        /// sets colors of controls in Status page.
        /// </summary>
        void SetStatusColors()
        {
            tabPageStatus.BackColor = LayoutColors.ColorFormBack;

            lblConnection.BackColor = Color.Transparent;
            lblConnection.ForeColor = LayoutColors.ColorControlText;

            tbxDataInfo.BackColor = LayoutColors.ColorControlBack;
            tbxDataInfo.ForeColor = LayoutColors.ColorControlText;

            pnlDataInfoBase.SetColors();
            pnlMarketInfo.SetColors();
            pnlConnection.SetColors();
            pnlUsefulLinks.SetColors();
            pnlForexBrokers.SetColors();

            return;
        }

        protected virtual void BtnShowMarketInfo_Click(object sender, EventArgs e)
        {
        }

        protected virtual void BtnShowBars_Click(object sender, EventArgs e)
        {
        }

        protected virtual void BtnShowAccountInfo_Click(object sender, EventArgs e)
        {
        }

        delegate  void SetLblConnectionDelegate(string text);
        /// <summary>
        /// Sets the lblConnection.Text
        /// </summary>
        protected void SetLblConnectionText(string text)
        {
            if (lblConnection.InvokeRequired)
            {
                lblConnection.BeginInvoke(new SetLblConnectionDelegate(SetLblConnectionText), new object[] { text });
            }
            else
            {
                lblConnection.Text = text;
            }

            return;
        }

        delegate  void SetBarDataTextDelegate(string text);
        /// <summary>
        /// Sets the tbxBarData.Text
        /// </summary>
        protected void SetBarDataText(string text)
        {
            if (tbxDataInfo.InvokeRequired)
            {
                tbxDataInfo.BeginInvoke(new SetBarDataTextDelegate(SetBarDataText), new object[] { text });
            }
            else
            {
                tbxDataInfo.Text = text;
            }

            return;
        }

        delegate  void UpdateMarketInfoDelegate(string[] info);
        /// <summary>
        /// Sets the tbxBarData.Text
        /// </summary>
        protected void UpdateStatusPageMarketInfo(string[] values)
        {

            if (pnlMarketInfo.InvokeRequired)
            {
                pnlMarketInfo.BeginInvoke(new UpdateMarketInfoDelegate(UpdateStatusPageMarketInfo), new object[] { values });
            }
            else
            {
                string caption = Language.T("Market Information");
                string[] parameters = new string[7] {
                    Language.T("Symbol"),
                    Language.T("Period"),
                    Language.T("Lot size"),
                    Language.T("Point"),
                    Language.T("Spread"),
                    Language.T("Swap long"),
                    Language.T("Swap short")
                };
                pnlMarketInfo.Update(parameters, (string[])values, caption);
            }

            return;
        }
    }
}
