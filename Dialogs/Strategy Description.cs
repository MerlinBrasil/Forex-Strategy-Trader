// Strategy_Description Form
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Trader
{
    public class Strategy_Description : Form
    {
        Panel       pnlBase;
        Fancy_Panel pnlWarnBase;
        Label       lblWarning;
        Fancy_Panel pnlTbxBase;
        TextBox     txboxInfo;
        Button      btnClose;
        Button      btnAccept;
        Button      btnClear;
        String      oldInfo;

        /// <summary>
        /// Make a form
        /// </summary>
        public Strategy_Description()
        {
            pnlBase     = new Panel();
            pnlWarnBase = new Fancy_Panel();
            lblWarning  = new Label();
            pnlTbxBase  = new Fancy_Panel(Language.T("Strategy Description"));
            txboxInfo   = new TextBox();
            btnClose    = new Button();
            btnAccept   = new Button();
            btnClear    = new Button();
            
            // BBCode_viewer
            AcceptButton = btnClose;
            BackColor    = LayoutColors.ColorFormBack;
            Icon         = Data.Icon;
            MinimumSize  = new System.Drawing.Size(400, 400);
            Text         = Language.T("Strategy Description");
            FormClosing += new FormClosingEventHandler(Actions_FormClosing);

            Controls.Add(pnlBase);
            Controls.Add(btnAccept);
            Controls.Add(btnClose);
            Controls.Add(btnClear);

            // pnlWarnBase
            pnlWarnBase.Parent = this;
            pnlWarnBase.Padding = new Padding(2, 4, 2, 2);

            // lblWarning
            lblWarning.Parent = pnlWarnBase;
            lblWarning.TextAlign   = ContentAlignment.MiddleCenter;
            lblWarning.BackColor = Color.Transparent; ;
            lblWarning.ForeColor   = LayoutColors.ColorControlText;
            lblWarning.AutoSize = false;
            lblWarning.Dock = DockStyle.Fill;
            if (Data.Strategy.Description != "")
            {
                if (!Data.IsStrDescriptionRelevant())
                {
                    lblWarning.Font = new Font(Font, FontStyle.Bold);
                    lblWarning.Text = Language.T("This description might be outdated!");
                }
                else
                    lblWarning.Text = System.IO.Path.GetFileNameWithoutExtension(Data.StrategyName);
            }
            else
                lblWarning.Text = Language.T("You can write a description to the strategy!");

            pnlTbxBase.Parent = pnlBase;
            pnlTbxBase.Padding = new Padding(4, (int)pnlTbxBase.CaptionHeight + 1, 2, 3);
            pnlTbxBase.Dock = DockStyle.Fill;


            // txboxInfo
            txboxInfo.Parent        = pnlTbxBase;
            txboxInfo.Dock          = DockStyle.Fill;
            txboxInfo.BackColor     = LayoutColors.ColorControlBack;
            txboxInfo.ForeColor     = LayoutColors.ColorControlText;
            txboxInfo.BorderStyle   = BorderStyle.None;
            txboxInfo.Multiline     = true;
            txboxInfo.AcceptsReturn = true;
            txboxInfo.AcceptsTab    = true;
            txboxInfo.ScrollBars    = ScrollBars.Vertical;
            txboxInfo.KeyDown      += new KeyEventHandler(TxboxInfo_KeyDown);
            txboxInfo.Text          = Data.Strategy.Description;
            txboxInfo.Select(0, 0);

            oldInfo = Data.Strategy.Description;

            // btnClose
            btnClose.Text   = Language.T("Close");
            btnClose.Click += new System.EventHandler(BtnClose_Click);
            btnClose.UseVisualStyleBackColor = true;

            // btnAccept
            btnAccept.Text   = Language.T("Accept");
            btnAccept.Click += new System.EventHandler(BtnAccept_Click);
            btnAccept.UseVisualStyleBackColor = true;

            // btnClear
            btnClear.Text   = Language.T("Clear");
            btnClear.Click += new System.EventHandler(BtnClear_Click);
            btnClear.UseVisualStyleBackColor = true;
        }

        /// <summary>
        /// Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Size = new Size(300, 380);
            
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            int buttonHeight = (int)(Data.VerticalDLU * 15.5);
            int buttonWidth  = (int)(Data.HorizontalDLU * 60);
            int btnVertSpace = (int)(Data.VerticalDLU * 5.5);
            int btnHrzSpace  = (int)(Data.HorizontalDLU * 3);
            int border       = btnHrzSpace;
            int textHeight   = Font.Height;

            // Label Warning
            pnlWarnBase.Size = new Size(ClientSize.Width - 2 * border, 30);
            pnlWarnBase.Location = new Point(border, border);

            // Button Close
            btnClose.Size     = new Size(buttonWidth, buttonHeight);
            btnClose.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Accept
            btnAccept.Size     = new Size(buttonWidth, buttonHeight);
            btnAccept.Location = new Point(btnClose.Left - btnAccept.Width - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Clear
            btnClear.Size     = new Size(buttonWidth, buttonHeight);
            btnClear.Location = new Point(btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // pnlBase 
            pnlBase.Size     = new Size(ClientSize.Width - 2 * border, btnClose.Top - btnVertSpace - border - pnlWarnBase.Bottom - border);
            pnlBase.Location = new Point(border, pnlWarnBase.Bottom + border);

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
        void TxboxInfo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.A))
            {
                ((TextBox)sender).SelectAll();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        /// </summary>
        /// Accept the changes
        /// </summary>
        private void BtnAccept_Click(object sender, EventArgs e)
        {
            Data.Strategy.Description = txboxInfo.Text;
            oldInfo = txboxInfo.Text;
            Close();
        }

        /// </summary>
        /// Closes the form
        /// </summary>
        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// </summary>
        /// Cleans the info
        /// </summary>
        private void BtnClear_Click(object sender, EventArgs e)
        {
            txboxInfo.Text = "";
        }

        /// <summary>
        /// Check whether the strategy have been changed.
        /// </summary>
        void Actions_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (oldInfo != txboxInfo.Text)
            {
                DialogResult dr = MessageBox.Show(Language.T("Do you want to accept the changes?"),
                    Data.ProgramName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (dr == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
                else if (dr == DialogResult.Yes)
                {
                    Data.Strategy.Description = txboxInfo.Text;
                    oldInfo = txboxInfo.Text;
                    Close();
                }
                else if (dr == DialogResult.No)
                {
                    oldInfo = txboxInfo.Text;
                    Close();
                }
            }
        }
    }
}
