// Controls class 
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
        TabControl tabControlBase;

        TabPage tabPageStatus;
        TabPage tabPageStrategy;
        TabPage tabPageChart;
        TabPage tabPageAccount;
        TabPage tabPageJournal;
        TabPage tabPageOperation;

        /// <summary>
        /// The default constructor.
        /// </summary>
        public Controls()
        {
            ImageList imageList = new ImageList();
            imageList.ImageSize = new Size(20, 22);
            imageList.Images.Add(Properties.Resources.tab_status);
            imageList.Images.Add(Properties.Resources.tab_strategy);
            imageList.Images.Add(Properties.Resources.tab_chart);
            imageList.Images.Add(Properties.Resources.tab_account);
            imageList.Images.Add(Properties.Resources.tab_journal);
            imageList.Images.Add(Properties.Resources.tab_operation);

            tabControlBase  = new TabControl();
            tabControlBase.Name      = "tabControlBase";
            tabControlBase.Parent    = pnlWorkspace;
            tabControlBase.Dock      = DockStyle.Fill;
            tabControlBase.ImageList = imageList;
            tabControlBase.HotTrack  = true;
            tabControlBase.SelectedIndexChanged += new EventHandler(TabControlBase_SelectedIndexChanged);

            tabPageStatus    = new TabPage();
            tabPageStrategy  = new TabPage();
            tabPageChart     = new TabPage();
            tabPageAccount   = new TabPage();
            tabPageJournal   = new TabPage();
            tabPageOperation = new TabPage();

            tabControlBase.Controls.Add(tabPageStatus);
            tabControlBase.Controls.Add(tabPageStrategy);
            tabControlBase.Controls.Add(tabPageChart);
            tabControlBase.Controls.Add(tabPageAccount);
            tabControlBase.Controls.Add(tabPageJournal);
            tabControlBase.Controls.Add(tabPageOperation);

            Initialize_StripTrade();
            Initialize_PageStatus();
            Initialize_PageStrategy();
            Initialize_PageChart();
            Initialize_PageAccount();
            Initialize_PageJournal();
            Initialize_PageOperation();

            return;
        }

        /// <summary>
        /// Changes the active tab page.
        /// </summary>
        protected void ChangeTabPage(int index)
        {
            tabControlBase.SelectedIndex = index;
            TabControlBase_SelectedIndexChanged(new Object(), new EventArgs());

            return;
        }

        /// <summary>
        /// Sets tab pages and menu items.
        /// </summary>
        void TabControlBase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlBase.SelectedTab == tabPageStatus)
            {
                DisposeChart();
                miTabStatus.Checked    = true;
                miTabStrategy.Checked  = false;
                miTabChart.Checked     = false;
                miTabAccount.Checked   = false;
                miTabJournal.Checked   = false;
                miTabOperation.Checked = false;
            }
            else if (tabControlBase.SelectedTab == tabPageStrategy)
            {
                DisposeChart();
                miTabStatus.Checked    = false;
                miTabStrategy.Checked  = true;
                miTabChart.Checked     = false;
                miTabAccount.Checked   = false;
                miTabJournal.Checked   = false;
                miTabOperation.Checked = false;
            }
            else if (tabControlBase.SelectedTab == tabPageChart)
            {
                CreateChart();
                miTabStatus.Checked    = false;
                miTabStrategy.Checked  = false;
                miTabChart.Checked     = true;
                miTabAccount.Checked   = false;
                miTabJournal.Checked   = false;
                miTabOperation.Checked = false;
            }
            else if (tabControlBase.SelectedTab == tabPageAccount)
            {
                DisposeChart();
                miTabStatus.Checked    = false;
                miTabStrategy.Checked  = false;
                miTabChart.Checked     = false;
                miTabAccount.Checked   = true;
                miTabJournal.Checked   = false;
                miTabOperation.Checked = false;
            }
            else if (tabControlBase.SelectedTab == tabPageJournal)
            {
                DisposeChart();
                PageJournalSelected();
                miTabStatus.Checked    = false;
                miTabStrategy.Checked  = false;
                miTabChart.Checked     = false;
                miTabAccount.Checked   = false;
                miTabJournal.Checked   = true;
                miTabOperation.Checked = false;
            }
            else if (tabControlBase.SelectedTab == tabPageOperation)
            {
                DisposeChart();
                miTabStatus.Checked    = false;
                miTabStrategy.Checked  = false;
                miTabChart.Checked     = false;
                miTabAccount.Checked   = false;
                miTabJournal.Checked   = false;
                miTabOperation.Checked = true;
            }

            Configs.LastTab = tabControlBase.SelectedIndex;

            return;
        }

        /// <summary>
        /// Sets colors of tab pages.
        /// </summary>
        protected void SetColors()
        {
            SetStatusColors();
            SetStrategyColors();
            SetChartColors();
            SetAccountColors();
            SetJournalColors();
            SetOperationColors();

            return;
        }
    }
}
