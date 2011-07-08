// Controls Strategy
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
        Panel           pnlStrategyBase;
        Panel           pnlOverviewBase;
        Strategy_Layout strategyLayout;
        Fancy_Panel     pnlBrawserBase;
        WebBrowser      browserOverview;

        ToolStrip       tsStrategy;

        /// <summary>
        /// Sets the controls in tabPageStrategy
        /// </summary>
        void Initialize_PageStrategy()
        {
            // tabPageStrategy
            tabPageStrategy.Name = "tabPageStrategy";
            tabPageStrategy.Text = Language.T("Strategy");
            tabPageStrategy.ImageIndex = 1;
            tabPageStrategy.Resize += new EventHandler(TabPageStrategy_Resize);

            pnlOverviewBase = new Panel();
            pnlOverviewBase.Parent  = tabPageStrategy;
            pnlOverviewBase.Dock    = DockStyle.Fill;

            pnlStrategyBase = new Panel();
            pnlStrategyBase.Parent  = tabPageStrategy;
            pnlStrategyBase.Dock    = DockStyle.Left;

            // Panel Browser Base
            pnlBrawserBase = new Fancy_Panel(Language.T("Strategy Overview"));
            pnlBrawserBase.Padding = new Padding(2, (int)pnlBrawserBase.CaptionHeight, 2, 2);
            pnlBrawserBase.Parent  = pnlOverviewBase;
            pnlBrawserBase.Dock    = DockStyle.Fill;

            // BrowserOverview
            browserOverview = new WebBrowser();
            browserOverview.Parent = pnlBrawserBase;
            browserOverview.Dock   = DockStyle.Fill;
            //browserOverview.AllowNavigation = false;
            browserOverview.WebBrowserShortcutsEnabled = false;

            // StrategyLayout
            strategyLayout = new Strategy_Layout(Data.Strategy.Clone());
            strategyLayout.Parent = pnlStrategyBase;
            strategyLayout.btnAddOpenFilter.Click  += new EventHandler(BtnAddOpenFilter_Click);
            strategyLayout.btnAddCloseFilter.Click += new EventHandler(BtnAddCloseFilter_Click);

            // ToolStrip Strategy
            tsStrategy = new ToolStrip();
            tsStrategy.Parent = pnlStrategyBase;
            tsStrategy.Dock   = DockStyle.None;
            tsStrategy.AutoSize = false;

            // Button tsbtStrategyNew
            ToolStripButton tsbtStrategyNew = new ToolStripButton();
            tsbtStrategyNew.Name         = "New";
            tsbtStrategyNew.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbtStrategyNew.Image        = Properties.Resources.strategy_new;
            tsbtStrategyNew.Click       += new EventHandler(BtnStrategyIO_Click);
            tsbtStrategyNew.ToolTipText  = Language.T("Open the default strategy \"New.xml\".");
            tsStrategy.Items.Add(tsbtStrategyNew);

            // Button tsbtStrategyOpen
            ToolStripButton tsbtStrategyOpen = new ToolStripButton();
            tsbtStrategyOpen.Name         = "Open";
            tsbtStrategyOpen.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbtStrategyOpen.Image        = Properties.Resources.strategy_open;
            tsbtStrategyOpen.Click       += new EventHandler(BtnStrategyIO_Click);
            tsbtStrategyOpen.ToolTipText  = Language.T("Open a strategy.");
            tsStrategy.Items.Add(tsbtStrategyOpen);

            // Button tsbtStrategySave
            ToolStripButton tsbtStrategySave = new ToolStripButton();
            tsbtStrategySave.Name         = "Save";
            tsbtStrategySave.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbtStrategySave.Image        = Properties.Resources.strategy_save;
            tsbtStrategySave.Click       += new EventHandler(BtnStrategyIO_Click);
            tsbtStrategySave.ToolTipText  = Language.T("Save the strategy.");
            tsStrategy.Items.Add(tsbtStrategySave);

            // Button tsbtStrategySaveAs
            ToolStripButton tsbtStrategySaveAs = new ToolStripButton();
            tsbtStrategySaveAs.Name         = "SaveAs";
            tsbtStrategySaveAs.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbtStrategySaveAs.Image        = Properties.Resources.strategy_save_as;
            tsbtStrategySaveAs.Click       += new EventHandler(BtnStrategyIO_Click);
            tsbtStrategySaveAs.ToolTipText  = Language.T("Save a copy of the strategy.");
            tsStrategy.Items.Add(tsbtStrategySaveAs);

            tsStrategy.Items.Add(new ToolStripSeparator());

            // Button tsbtStrategyUndo
            ToolStripButton tsbtStrategyUndo = new ToolStripButton();
            tsbtStrategyUndo.Name         = "Undo";
            tsbtStrategyUndo.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbtStrategyUndo.Image        = Properties.Resources.strategy_undo;
            tsbtStrategyUndo.Click       += new EventHandler(MenuStrategyUndo_OnClick);
            tsbtStrategyUndo.ToolTipText  = Language.T("Undo the last change in the strategy.");
            tsStrategy.Items.Add(tsbtStrategyUndo);

            // Button tsbtStrategyCopy
            ToolStripButton tsbtStrategyCopy = new ToolStripButton();
            tsbtStrategyCopy.Name         = "Copy";
            tsbtStrategyCopy.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbtStrategyCopy.Image        = Properties.Resources.copy;
            tsbtStrategyCopy.Click       += new EventHandler(MenuStrategyCopy_OnClick);
            tsbtStrategyCopy.ToolTipText  = Language.T("Copy the entire strategy to the clipboard.");
            tsStrategy.Items.Add(tsbtStrategyCopy);

            // Button tsbtStrategyPaste
            ToolStripButton tsbtStrategyPaste = new ToolStripButton();
            tsbtStrategyPaste.Name         = "Paste";
            tsbtStrategyPaste.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbtStrategyPaste.Image        = Properties.Resources.paste;
            tsbtStrategyPaste.Click       += new EventHandler(MenuStrategyPaste_OnClick);
            tsbtStrategyPaste.ToolTipText  = Language.T("Load a strategy from the clipboard.");
            tsStrategy.Items.Add(tsbtStrategyPaste);

            tsStrategy.Items.Add(new ToolStripSeparator());

            // Button tsbtStrategyZoomIn
            ToolStripButton tsbtStrategyZoomIn = new ToolStripButton();
            tsbtStrategyZoomIn.Name         = "ZoomIn";
            tsbtStrategyZoomIn.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbtStrategyZoomIn.Image        = Properties.Resources.strategy_zoom_in;
            tsbtStrategyZoomIn.Click       += new EventHandler(BtnStrategyZoom_Click);
            tsbtStrategyZoomIn.ToolTipText  = Language.T("Expand the information in the strategy slots.");
            tsStrategy.Items.Add(tsbtStrategyZoomIn);

            // Button tsbtStrategyZoomOut
            ToolStripButton tsbtStrategyZoomOut = new ToolStripButton();
            tsbtStrategyZoomOut.Name         = "ZoomOut";
            tsbtStrategyZoomOut.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbtStrategyZoomOut.Image        = Properties.Resources.strategy_zoom_out;
            tsbtStrategyZoomOut.Click       += new EventHandler(BtnStrategyZoom_Click);
            tsbtStrategyZoomOut.ToolTipText  = Language.T("Reduce the information in the strategy slots.");
            tsStrategy.Items.Add(tsbtStrategyZoomOut);

            tsStrategy.Items.Add(new ToolStripSeparator());

            // Button tsbtStrategyDescription
            ToolStripButton tsbtStrategyDescription = new ToolStripButton();
            tsbtStrategyDescription.Name         = "Description";
            tsbtStrategyDescription.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbtStrategyDescription.Image        = Properties.Resources.strategy_description;
            tsbtStrategyDescription.Click       += new EventHandler(BtnStrategyDescription_Click);
            tsbtStrategyDescription.ToolTipText  = Language.T("Edit the strategy description.");
            tsStrategy.Items.Add(tsbtStrategyDescription);

            // Button tsbtStrategyPublish
            ToolStripButton tsbtStrategyPublish = new ToolStripButton();
            tsbtStrategyPublish.Name         = "Publish";
            tsbtStrategyPublish.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbtStrategyPublish.Image        = Properties.Resources.strategy_publish;
            tsbtStrategyPublish.Click       += new EventHandler(MenuStrategyBBcode_OnClick);
            tsbtStrategyPublish.ToolTipText  = Language.T("Publish the strategy in the program's forum.");
            tsStrategy.Items.Add(tsbtStrategyPublish);

            tsStrategy.Items.Add(new ToolStripSeparator());

            // Button tsbtStrategySettings
            ToolStripButton tsbtStrategySettings = new ToolStripButton();
            tsbtStrategySettings.Name         = "Settings";
            tsbtStrategySettings.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbtStrategySettings.Image        = Properties.Resources.strategy_settings;
            tsbtStrategySettings.Click       += new EventHandler(BtnStrategySettings_Click);
            tsbtStrategySettings.ToolTipText  = Language.T("Trade settings.");
            tsStrategy.Items.Add(tsbtStrategySettings);

            SetStrategyColors();
            RebuildStrategyLayout();
            SetSrategyOverview();

            return;
        }

        /// <summary>
        /// TabPageStrategy_Resize
        /// </summary>
        void TabPageStrategy_Resize(object sender, EventArgs e)
        {
            pnlStrategyBase.Width = tabPageStrategy.ClientSize.Width / 2;

            tsStrategy.Width    =  pnlStrategyBase.Width - space;
            tsStrategy.Location = Point.Empty;

            strategyLayout.Width    = pnlStrategyBase.Width - space;
            strategyLayout.Height   = tabPageStrategy.ClientSize.Height - tsStrategy.Bottom - space;
            strategyLayout.Location = new Point(0, tsStrategy.Bottom + space);

            return;
        }

        /// <summary>
        /// Sets the colors of tab page Strategy.
        /// </summary>
        void SetStrategyColors()
        {
            tabPageStrategy.BackColor = LayoutColors.ColorFormBack;
            pnlBrawserBase.SetColors();
            SetSrategyOverview();

            return;
        }

        /// <summary>
        /// Creates a new strategy layout using Data.Strategy
        /// </summary>
        protected void RebuildStrategyLayout()
        {
            strategyLayout.RebuildStrategyControls(Data.Strategy.Clone());
            strategyLayout.pnlProperties.Click += new EventHandler(PnlAveraging_Click);
            for (int iSlot = 0; iSlot < Data.Strategy.Slots; iSlot++)
            {
                ToolStripMenuItem miEdit = new ToolStripMenuItem();
                miEdit.Text   = Language.T("Edit") + "...";
                miEdit.Image  = Properties.Resources.edit;
                miEdit.Name   = "Edit";
                miEdit.Tag    = iSlot;
                miEdit.Click += new EventHandler(SlotContextMenu_Click);

                ToolStripMenuItem miUpwards = new ToolStripMenuItem();
                miUpwards.Text    = Language.T("Move Up");
                miUpwards.Image   = Properties.Resources.up_arrow;
                miUpwards.Name    = "Upwards";
                miUpwards.Tag     = iSlot;
                miUpwards.Click  += new EventHandler(SlotContextMenu_Click);
                miUpwards.Enabled = (iSlot > 1 && Data.Strategy.Slot[iSlot].SlotType == Data.Strategy.Slot[iSlot - 1].SlotType);

                ToolStripMenuItem miDownwards = new ToolStripMenuItem();
                miDownwards.Text    = Language.T("Move Down");
                miDownwards.Image   = Properties.Resources.down_arrow;
                miDownwards.Name    = "Downwards";
                miDownwards.Tag     = iSlot;
                miDownwards.Click  += new EventHandler(SlotContextMenu_Click);
                miDownwards.Enabled = (iSlot < Data.Strategy.Slots - 1 && Data.Strategy.Slot[iSlot].SlotType == Data.Strategy.Slot[iSlot + 1].SlotType);

                ToolStripMenuItem miDuplicate = new ToolStripMenuItem();
                miDuplicate.Text    = Language.T("Duplicate");
                miDuplicate.Image   = Properties.Resources.duplicate;
                miDuplicate.Name    = "Duplicate";
                miDuplicate.Tag     = iSlot;
                miDuplicate.Click  += new EventHandler(SlotContextMenu_Click);
                miDuplicate.Enabled = ( Data.Strategy.Slot[iSlot].SlotType == SlotTypes.OpenFilter  && Data.Strategy.OpenFilters  < Strategy.MaxOpenFilters ||
                                        Data.Strategy.Slot[iSlot].SlotType == SlotTypes.CloseFilter && Data.Strategy.CloseFilters < Strategy.MaxCloseFilters);

                ToolStripMenuItem miDelete = new ToolStripMenuItem();
                miDelete.Text    = Language.T("Delete");
                miDelete.Image   = Properties.Resources.close_button;
                miDelete.Name    = "Delete";
                miDelete.Tag     = iSlot;
                miDelete.Click  += new EventHandler(SlotContextMenu_Click);
                miDelete.Enabled = (Data.Strategy.Slot[iSlot].SlotType == SlotTypes.OpenFilter || Data.Strategy.Slot[iSlot].SlotType == SlotTypes.CloseFilter);

                strategyLayout.apnlSlot[iSlot].ContextMenuStrip = new ContextMenuStrip();
                strategyLayout.apnlSlot[iSlot].ContextMenuStrip.Items.AddRange(new ToolStripMenuItem[] { miEdit, miUpwards, miDownwards, miDuplicate, miDelete });

                strategyLayout.apnlSlot[iSlot].MouseClick += new MouseEventHandler(PnlSlot_MouseUp);
                if (iSlot != Data.Strategy.OpenSlot && iSlot != Data.Strategy.CloseSlot)
                    strategyLayout.abtnRemoveSlot[iSlot].Click += new EventHandler(BtnRemoveSlot_Click);
            }

            return;
        }

        /// <summary>
        /// Repaint the strategy slots without changing its kind and count.
        /// </summary>
        protected void RepaintStrategyLayout()
        {
            strategyLayout.RepaintStrategyControls(Data.Strategy.Clone());
        }

        /// <summary>
        /// Rearranges the strategy slots without changing its kind and count.
        /// </summary>
        protected void RearangeStrategyLayout()
        {
            strategyLayout.RearangeStrategyControls();
        }

        /// <summary>
        /// Opens the averaging parameters dialog.
        /// </summary>
        protected virtual void PnlAveraging_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Click on a strategy slot
        /// </summary>
        protected virtual void PnlSlot_MouseUp(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// Click on a strategy slot
        /// </summary>
        protected virtual void SlotContextMenu_Click(object sender, EventArgs e)
        {
        }
        
        /// <summary>
        /// Performs actions after the button add open filter was clicked.
        /// </summary>
        protected virtual void BtnAddOpenFilter_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Performs actions after the button add close filter was clicked.
        /// </summary>
        protected virtual void BtnAddCloseFilter_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Removes the corresponding slot.
        /// </summary>
        protected virtual void BtnRemoveSlot_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// IO strategy
        /// </summary>
        protected virtual void BtnStrategyIO_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Changes the slot size
        /// </summary>
        protected virtual void BtnStrategyZoom_Click(object sender, EventArgs e)
        {
            ToolStripButton btn = (ToolStripButton)sender;

            switch (btn.Name)
            {
                case "ZoomIn":
                    if (strategyLayout.SlotMinMidMax == SlotSizeMinMidMax.min)
                    {
                        strategyLayout.SlotMinMidMax = SlotSizeMinMidMax.mid;
                    }
                    else if (strategyLayout.SlotMinMidMax == SlotSizeMinMidMax.mid)
                    {
                        strategyLayout.SlotMinMidMax = SlotSizeMinMidMax.max;
                    }
                    break;
                case "ZoomOut":
                    if (strategyLayout.SlotMinMidMax == SlotSizeMinMidMax.max)
                    {
                        strategyLayout.SlotMinMidMax = SlotSizeMinMidMax.mid;
                    }
                    else if (strategyLayout.SlotMinMidMax == SlotSizeMinMidMax.mid)
                    {
                        strategyLayout.SlotMinMidMax = SlotSizeMinMidMax.min;
                    }
                    break;
                default:
                    break;
            }

            RearangeStrategyLayout();
        }

        /// <summary>
        /// View / edit the strategy description.
        /// </summary>
        protected virtual void BtnStrategyDescription_Click(object sender, EventArgs e)
        {
            string sOldInfo = Data.Strategy.Description;
            Strategy_Description si = new Strategy_Description();
            si.ShowDialog();
            if (sOldInfo != Data.Strategy.Description)
            {
                Data.SetStrategyIndicators();
                SetSrategyOverview();
                Data.IsStrategyChanged = true;
            }

            return;
        }

        /// <summary>
        /// Sets the strategy overview
        /// </summary>
        protected void SetSrategyOverview()
        {
            browserOverview.DocumentText = Data.Strategy.GenerateHTMLOverview();
            
            return;
        }

        /// <summary>
        /// Trade settings
        /// </summary>
        void BtnStrategySettings_Click(object sender, EventArgs e)
        {
            Trade_Settings ts = new Trade_Settings();
            ts.ShowDialog();

            return;
        }
    }
}
