// Controls - Trade
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
    /// Class Controls : Menu_and_StatusBar
    /// </summary>
    public partial class Controls : Menu_and_StatusBar
    {
        protected ToolStripButton   tsbtnChangeID;
        protected ToolStripButton   tsbtnTrading;    // Button "Start Automatic Execution".
        protected ToolStripLabel    tslblConnection;
        protected ToolStripButton   tsbtnConnectionHelp;
        protected ToolStripButton   tsbtnConnectionGo;

        protected ToolStripLabel    tslblConnectionID;
        protected ToolStripTextBox  tstbxConnectionID;
        protected ToolStripButton   tsbtnConfirmID;

        void Initialize_StripTrade()
        {
            tsbtnConnectionHelp = new ToolStripButton();
            tsbtnConnectionHelp.ToolTipText  = Language.T("Help me get connected!");
            tsbtnConnectionHelp.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            tsbtnConnectionHelp.Image        = Properties.Resources.help;
            tsbtnConnectionHelp.Click       += new EventHandler(ConnectionHelp_Click);
            tsTradeControl.Items.Add(tsbtnConnectionHelp);

            tslblConnectionID = new ToolStripLabel();
            tslblConnectionID.Text    = Language.T("Set connection ID");
            tslblConnectionID.Visible = Configs.MultipleInstances;
            tsTradeControl.Items.Add(tslblConnectionID);

            tstbxConnectionID = new ToolStripTextBox();
            tstbxConnectionID.Width     = 100;
            tstbxConnectionID.BorderStyle = BorderStyle.FixedSingle;
            tstbxConnectionID.Visible   = Configs.MultipleInstances;
            tstbxConnectionID.KeyPress += new KeyPressEventHandler(TstbxConnectionID_KeyPress);
            tsTradeControl.Items.Add(tstbxConnectionID);

            tsbtnConnectionGo = new ToolStripButton();
            tsbtnConnectionGo.ToolTipText = Language.T("Go");
            tsbtnConnectionGo.Image   = Properties.Resources.go_right;
            tsbtnConnectionGo.Width   = 22;
            tsbtnConnectionGo.Visible = Configs.MultipleInstances;
            tsbtnConnectionGo.Enabled = false;
            tsbtnConnectionGo.Click  += new EventHandler(TsbtConnectionGo_Click);
            tsTradeControl.Items.Add(tsbtnConnectionGo);

            tsbtnChangeID = new ToolStripButton();
            tsbtnChangeID.Text         = "ID ";
            tsbtnChangeID.ToolTipText  = Language.T("Click to change the connection ID.");
            tsbtnChangeID.Width        = 100;
            tsbtnChangeID.Enabled      = true;
            tsbtnChangeID.Click       += new EventHandler(TsbtChangeID_Click);
            tsbtnChangeID.Visible      = false;
            tsTradeControl.Items.Add(tsbtnChangeID);

            tslblConnection = new ToolStripLabel();
            tslblConnection.Text        = Language.T("Not Connected");
            tslblConnection.AutoSize    = false;
            tslblConnection.Width       = 200;
            tslblConnection.Visible = !Configs.MultipleInstances;
            tsTradeControl.Items.Add(tslblConnection);

            tsbtnTrading = new ToolStripButton();
            tsbtnTrading.Text         = Language.T("Start Automatic Execution");
            tsbtnTrading.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            tsbtnTrading.Image        = Properties.Resources.play;
            tsbtnTrading.Enabled      = false;
            tsbtnTrading.Visible      = !Configs.MultipleInstances;
            tsbtnTrading.Click       += new EventHandler(TsbtTrading_Click);
            tsTradeControl.Items.Add(tsbtnTrading);

            if (Data.IsProgramBeta)
            {
                ToolStripLabel tslWarning = new ToolStripLabel();
                tslWarning.ForeColor = Color.Tomato;
                tslWarning.Text      = Language.T("Beta version. Test carefully!");
                tslWarning.Alignment = ToolStripItemAlignment.Right;
                tslWarning.Click    += new EventHandler(TslWarning_Click);
                tsTradeControl.Items.Add(tslWarning);
            }

            return;
        }

        protected virtual void TstbxConnectionID_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        /// <summary>
        /// Button Change ID clicked.
        /// </summary>
        protected virtual void TsbtChangeID_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Button Connection Go clicked.
        /// </summary>
        protected virtual void TsbtConnectionGo_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Shows connection help.
        /// </summary>
        void ConnectionHelp_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(@"http://forexsb.com/wiki/fst/connection");
            }
            catch { }

            return;
        }

        /// <summary>
        /// Hides the warning button.
        /// </summary>
        void TslWarning_Click(object sender, EventArgs e)
        {
            ToolStripLabel label = (ToolStripLabel)sender;
            label.Visible = false;
        }

        protected virtual void TsbtTrading_Click(object sender, EventArgs e)
        {
        }
    }
}
