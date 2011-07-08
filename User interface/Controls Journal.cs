// Controls Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Class Controls : Menu_and_StatusBar
    /// </summary>
    public partial class Controls : Menu_and_StatusBar
    {
        Journal   journal;
        ToolStrip tsJournal;
        bool isShowTicks          = Configs.JournalShowTicks;
        bool isShowSystemMessages = Configs.JournalShowSystemMessages;

        /// <summary>
        /// Gets or sets if the journal shows ticks.
        /// </summary>
        public bool JournalShowTicks
        {
            get { return isShowTicks; }
        }

        /// <summary>
        /// Gets or sets if the journal shows system messages.
        /// </summary>
        public bool JournalShowSystemMessages
        {
            get { return isShowSystemMessages; }
        }

        /// <summary>
        /// Initializes page Chart.
        /// </summary>
        public void Initialize_PageJournal()
        {
            // tabPageJournal
            tabPageJournal.Name = "tabPageJournal";
            tabPageJournal.Text = Language.T("Journal");
            tabPageJournal.ImageIndex = 4;

            journal = new Journal();
            journal.Parent = tabPageJournal;
            journal.Dock   = DockStyle.Fill;

            tsJournal = new ToolStrip();
            tsJournal.Parent = tabPageJournal;
            tsJournal.Dock   = DockStyle.Top;

            Font fontMessage = new Font(Font.FontFamily, 9);
            Graphics g = CreateGraphics();
            float fTimeWidth = g.MeasureString(DateTime.Now.ToString(Data.DF + " HH:mm:ss"), fontMessage).Width;
            g.Dispose();

            ToolStripLabel lblTime = new ToolStripLabel(Language.T("Time"));
            lblTime.AutoSize = false;
            lblTime.Width    = 16 + (int)fTimeWidth - 5;
            tsJournal.Items.Add(lblTime);

            tsJournal.Items.Add(new ToolStripSeparator());

            ToolStripLabel lblMessage = new ToolStripLabel(Language.T("Message"));
            lblMessage.AutoSize = false;
            lblMessage.Width    = 250;
            tsJournal.Items.Add(lblMessage);

            // Tool strip buttons
            ToolStripButton tsbClear = new ToolStripButton();
            tsbClear.Image        = Properties.Resources.clear;
            tsbClear.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbClear.Alignment    = ToolStripItemAlignment.Right;
            tsbClear.ToolTipText  = Language.T("Clear journal's messages.");
            tsbClear.Click       += new EventHandler(TsbClear_Click);
            tsJournal.Items.Add(tsbClear);

            ToolStripSeparator sep = new ToolStripSeparator();
            sep.Alignment = ToolStripItemAlignment.Right;
            tsJournal.Items.Add(sep);

            ToolStripComboBox tscbxJounalLength = new ToolStripComboBox();
            tscbxJounalLength.Alignment = ToolStripItemAlignment.Right;
            tscbxJounalLength.DropDownStyle = ComboBoxStyle.DropDownList;
            tscbxJounalLength.AutoSize = false;
            tscbxJounalLength.Size = new System.Drawing.Size(60, 25);
            tscbxJounalLength.Items.AddRange(new object[] { "20", "200", "500", "1000", "5000", "10000" });
            tscbxJounalLength.SelectedItem = Configs.JournalLength.ToString();
            tscbxJounalLength.ToolTipText  = Language.T("Maximum messages in the journal.");
            tscbxJounalLength.SelectedIndexChanged += new EventHandler(TscbxJounalLenght_SelectedIndexChanged);
            tsJournal.Items.Add(tscbxJounalLength);

            ToolStripButton tsbShowTicks = new ToolStripButton();
            tsbShowTicks.Image        = Properties.Resources.show_ticks;
            tsbShowTicks.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbShowTicks.Alignment    = ToolStripItemAlignment.Right;
            tsbShowTicks.Checked      = isShowTicks;
            tsbShowTicks.ToolTipText  = Language.T("Show incoming ticks.");
            tsbShowTicks.Click       += new EventHandler(TsbShowTicks_Click);
            tsJournal.Items.Add(tsbShowTicks);

            ToolStripButton tsbShowSystemMessages = new ToolStripButton();
            tsbShowSystemMessages.Image        = Properties.Resources.show_system_messages;
            tsbShowSystemMessages.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbShowSystemMessages.Alignment    = ToolStripItemAlignment.Right;
            tsbShowSystemMessages.Checked      = isShowSystemMessages;
            tsbShowSystemMessages.ToolTipText  = Language.T("Show system messages.");
            tsbShowSystemMessages.Click       += new EventHandler(TsbShowSystems_Click);
            tsJournal.Items.Add(tsbShowSystemMessages);

            ToolStripSeparator sep1 = new ToolStripSeparator();
            sep1.Alignment = ToolStripItemAlignment.Right;
            tsJournal.Items.Add(sep1);

            return;
        }

        /// <summary>
        /// Journal Length changed
        /// </summary>
        void TscbxJounalLenght_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToolStripComboBox comboBox = (ToolStripComboBox)sender;
            Configs.JournalLength = int.Parse(comboBox.SelectedItem.ToString());
            if (messages.Count > Configs.JournalLength)
                messages.RemoveRange(0, messages.Count - Configs.JournalLength);

            tabPageJournal.Select();
            UpdateJournal(messages);
        }

        /// <summary>
        /// Page journal was selected.
        /// </summary>
        void PageJournalSelected()
        {
            journal.SelectVScrollBar();

            return;
        }

        /// <summary>
        /// Clears the journal messages.
        /// </summary>
        void TsbClear_Click(object sender, EventArgs e)
        {
            messages.Clear();
            journal.ClearMessages();

            return;
        }

        /// <summary>
        /// Journal starts showing ticks.
        /// </summary>
        void TsbShowTicks_Click(object sender, EventArgs e)
        {
            ToolStripButton btn = (ToolStripButton)sender;
            btn.Checked = !btn.Checked;
            isShowTicks = btn.Checked;
            Configs.JournalShowTicks = isShowTicks;

            return;
        }

        /// <summary>
        /// Journal starts showing system messages.
        /// </summary>
        void TsbShowSystems_Click(object sender, EventArgs e)
        {
            ToolStripButton btn = (ToolStripButton)sender;
            btn.Checked = !btn.Checked;
            isShowSystemMessages = btn.Checked;
            Configs.JournalShowSystemMessages = isShowSystemMessages;

            return;
        }

        List<JournalMessage> messages = new List<JournalMessage>();
        /// <summary>
        /// Adds a message to the journal.
        /// </summary>
        protected void AppendJournalMessage(JournalMessage message)
        {
            messages.Add(message);
            if (messages.Count > Configs.JournalLength)
                messages.RemoveRange(0, messages.Count - Configs.JournalLength);

            UpdateJournal(messages);

            return;
        }

        delegate void UpdateJournalDelegate(List<JournalMessage> newMessages);
        /// <summary>
        /// Updates journal.
        /// </summary>
        void UpdateJournal(List<JournalMessage> newMessages)
        {
            if (journal.InvokeRequired)
            {
                journal.BeginInvoke(new UpdateJournalDelegate(UpdateJournal), new object[] { newMessages });
            }
            else
            {
                journal.UpdateMessages(newMessages);
            }

            return;
        }

        /// <summary>
        /// Sets the colors of tab page Journal.
        /// </summary>
        void SetJournalColors()
        {
            tabPageJournal.BackColor = LayoutColors.ColorFormBack;
            journal.SetColors();

            return;
        }
    }
}
