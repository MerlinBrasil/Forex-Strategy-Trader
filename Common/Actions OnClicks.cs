// Actions OnClick
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Class Actions : Controls
    /// </summary>
    public partial class Actions : Controls
    {
        /// <summary>
        /// Opens the averaging parameters dialog.
        /// </summary>
        protected override void PnlAveraging_Click(object sender, EventArgs e)
        {
            EditStrategyProperties();

            return;
        }

        /// <summary>
        /// Opens the indicator parameters dialog.
        /// </summary>
        protected override void PnlSlot_MouseUp(object sender, MouseEventArgs e)
        {
            Panel panel = (Panel)sender;
            int iTag = (int)panel.Tag;
            if (e.Button == MouseButtons.Left)
                EditSlot(iTag);

            return;
        }

        /// <summary>
        /// Strategy panel menu items clicked
        /// </summary>
        protected override void SlotContextMenu_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            int iTag = (int)mi.Tag;
            switch (mi.Name)
            {
                case "Edit":
                    EditSlot(iTag);
                    break;
                case "Upwards":
                    MoveSlotUpwards(iTag);
                    break;
                case "Downwards":
                    MoveSlotDownwards(iTag);
                    break;
                case "Duplicate":
                    DuplicateSlot(iTag);
                    break;
                case "Delete":
                    RemoveSlot(iTag);
                    break;
                default:
                    break;
            }

            return;
        }

        /// <summary>
        /// MenuChangeTabs_OnClick
        /// </summary>
        protected override void MenuChangeTabs_OnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            if (mi.Checked)
                return;

            int iTag = (int)mi.Tag;
            ChangeTabPage(iTag);
            
            return;
        }

        /// <summary>
        /// Performs actions after the button add open filter was clicked.
        /// </summary>
        protected override void BtnAddOpenFilter_Click(object sender, EventArgs e)
        {
            AddOpenFilter();

            return;
        }

        /// <summary>
        /// Performs actions after the button add close filter was clicked.
        /// </summary>
        protected override void BtnAddCloseFilter_Click(object sender, EventArgs e)
        {
            AddCloseFilter();

            return;
        }

        /// <summary>
        /// Remove the corresponding indicator slot.
        /// </summary>
        protected override void BtnRemoveSlot_Click(object sender, EventArgs e)
        {
            int iSlot = (int)((Button)sender).Tag;

            RemoveSlot(iSlot);

            return;
        }

        /// <summary>
        /// Load a color scheme.
        /// </summary>
        protected override void MenuLoadColor_OnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            if (!mi.Checked)
            {
                Configs.ColorScheme = mi.Name;
            }
            foreach (ToolStripMenuItem tsmi in mi.Owner.Items)
            {
                tsmi.Checked = false;
            }
            mi.Checked = true;

            LoadColorScheme();

            return;
        }

        /// <summary>
        /// Gradient View Changed
        /// </summary>
        protected override void MenuGradientView_OnClick(object sender, EventArgs e)
        {
            Configs.GradientView = ((ToolStripMenuItem)sender).Checked;
            pnlWorkspace.Invalidate(true);
            SetColors();
            return;
        }


        /// <summary>
        /// Strategy IO
        /// </summary>
        protected override void BtnStrategyIO_Click(object sender, EventArgs e)
        {
            ToolStripButton btn = (ToolStripButton)sender;

            switch (btn.Name)
            {
                case "New":
                    NewStrategy();
                    break;
                case "Open":
                    ShowOpenFileDialog();
                    break;
                case "Save":
                    SaveStrategy();
                    break;
                case "SaveAs":
                    SaveAsStrategy();
                    break;
                default:
                    break;
            }

            return;
        }

        /// <summary>
        /// Loads the default strategy.
        /// </summary>
        protected override void MenuStrategyNew_OnClick(object sender, EventArgs e)
        {
            NewStrategy();

            return;
        }

        /// <summary>
        /// Opens the dialog form OpenFileDialog.
        /// </summary>
        protected override void MenuFileOpen_OnClick(object sender, EventArgs e)
        {
            ShowOpenFileDialog();

            return;
        }

        /// <summary>
        /// Saves the strategy.
        /// </summary>
        protected override void MenuFileSave_OnClick(object sender, EventArgs e)
        {
            SaveStrategy();

            return;
        }

        /// <summary>
        /// Opens the dialog form SaveFileDialog.
        /// </summary>
        protected override void MenuFileSaveAs_OnClick(object sender, EventArgs e)
        {
            SaveAsStrategy();

            return;
        }

        /// <summary>
        /// Undoes the strategy.
        /// </summary>
        protected override void MenuStrategyUndo_OnClick(object sender, EventArgs e)
        {
            UndoStrategy();

            return;
        }

        /// <summary>
        /// Copies the strategy to clipboard.
        /// </summary>
        protected override void MenuStrategyCopy_OnClick(object sender, EventArgs e)
        {
            Strategy_XML strategyXML = new Strategy_XML();
            System.Xml.XmlDocument xmlDoc = strategyXML.CreateStrategyXmlDoc(Data.Strategy);
            Clipboard.SetText(xmlDoc.InnerXml);

            return;
        }

        /// <summary>
        /// Pastes a strategy from clipboard.
        /// </summary>
        protected override void MenuStrategyPaste_OnClick(object sender, EventArgs e)
        {
            DialogResult dialogResult = WhetherSaveChangedStrategy();

            if (dialogResult == DialogResult.Yes)
                SaveStrategy();
            else if (dialogResult == DialogResult.Cancel)
                return;

            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            Strategy_XML strategyXML = new Strategy_XML();
            Strategy tempStrategy;

            try
            {
                xmlDoc.InnerXml = Clipboard.GetText();
                tempStrategy = strategyXML.ParseXmlStrategy(xmlDoc);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                return;
            }

            OnStrategyChange();

            Data.Strategy = tempStrategy;
            Data.StrategyName = tempStrategy.StrategyName;
            Data.Strategy.StrategyName = tempStrategy.StrategyName;

            Data.SetStrategyIndicators();
            RebuildStrategyLayout();
            SetSrategyOverview();

            SetFormText();
            Data.IsStrategyChanged = false;
            Data.LoadedSavedStrategy = Data.StrategyPath;
            Data.StackStrategy.Clear();

            CalculateStrategy(true);
            AfterStrategyOpening();

            return;
        }

        /// <summary>
        /// Loads a dropped strategy.
        /// </summary>
        protected override void LoadDroppedStrategy(string filePath)
        {
            Data.StrategyDir = System.IO.Path.GetDirectoryName(filePath);
            LoadStrategyFile(filePath);
        }

        /// <summary>
        /// Opens the strategy settings dialogue.
        /// </summary>
        protected override void MenuStrategyAUPBV_OnClick(object sender, EventArgs e)
        {
            UsePreviousBarValue_Change();

            return;
        }

        /// <summary>
        /// Export the strategy in BBCode format - ready to post in the forum
        /// </summary>
        protected override void MenuStrategyBBcode_OnClick(object sender, EventArgs e)
        {
            Strategy_Publish publisher = new Strategy_Publish();
            publisher.Show();

            return;
        }

        /// <summary>
        /// Tools menu
        /// </summary>
        protected override void MenuTools_OnClick(object sender, EventArgs e)
        {
            string sName = ((ToolStripMenuItem)sender).Name;

            switch (sName)
            {
                case "Reset settings":
                    ResetSettings();
                    break;
                case "miResetTrader":
                    ResetTrader();
                    break;
                case "miInstallExpert":
                    InstallMTFiles();
                    break;
                case "miNewTranslation":
                    MakeNewTranslation();
                    break;
                case "miEditTranslation":
                    EditTranslation();
                    break;
                case "miShowEnglishPhrases":
                    Language.ShowPhrases(1);
                    break;
                case "miShowAltPhrases":
                    Language.ShowPhrases(2);
                    break;
                case "miShowAllPhrases":
                    Language.ShowPhrases(3);
                    break;
                case "miOpenIndFolder":
                    try { System.Diagnostics.Process.Start(Data.SourceFolder); }
                    catch (System.Exception ex) { MessageBox.Show(ex.Message); }
                    break;
                case "miReloadInd":
                    Cursor = Cursors.WaitCursor;
                    ReloadCustomIndicators();
                    Cursor = Cursors.Default;
                    break;
                case "miCheckInd":
                    Custom_Indicators.TestCustomIndicators();
                    break;
                case "CommandConsole":
                    ShowCommandConsole();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Installs MT Expert and Library files.
        /// </summary>
        void InstallMTFiles()
        {
            try
            {
                System.Diagnostics.Process.Start(Data.ProgramDir + @"\MetaTrader\Install MT Files.exe");
            }
            catch { }

            return;
        }

        /// <summary>
        /// Manual operation execution.
        /// </summary>
        protected override void BtnOperation_Click(object sender, EventArgs e)
        {
            if (!Data.IsConnected)
            {
                if (Configs.PlaySounds)
                    Data.SoundError.Play();
                return;
            }

            Button btn = (Button)sender;

            switch (btn.Name)
            {
                case "btnBuy":
                    {
                        MT4Bridge.OrderType type = MT4Bridge.OrderType.Buy;
                        string symbol     = Data.Symbol;
                        double lots       = NormalizeEntrySize(OperationLots);
                        double price      = Data.Ask;
                        int    slippage   = Configs.AutoSlippage ? (int)Data.InstrProperties.Spread * 3 : Configs.SlippageEntry;
                        
                        int stopLossPips = 0;
                        if (OperationStopLoss > 0 && OperationTrailingStop > 0)
                            stopLossPips = Math.Min(OperationStopLoss, OperationTrailingStop);
                        else
                            stopLossPips = Math.Max(OperationStopLoss, OperationTrailingStop);

                        double stoploss   = stopLossPips        > 0 ? Data.Bid - Data.InstrProperties.Point * stopLossPips : 0;
                        double takeprofit = OperationTakeProfit > 0 ? Data.Bid + Data.InstrProperties.Point * OperationTakeProfit : 0;

                        if (Configs.PlaySounds)
                            Data.SoundOrderSent.Play();

                        JournalMessage jmsg = new JournalMessage(JournalIcons.OrderBuy, DateTime.Now, string.Format(symbol + " " + Data.PeriodMTStr + " " +
                            Language.T("An entry order sent") + ": " + Language.T("Buy") + " {0} " + (lots == 1 ? Language.T("lot") : Language.T("lots")) + " " + Language.T("at") + " {1}, " +
                            Language.T("Stop Loss") + " {2}, " + Language.T("Take Profit") + " {3}", lots, price.ToString(Data.FF), stoploss.ToString(Data.FF), takeprofit.ToString(Data.FF)));
                        AppendJournalMessage(jmsg);

                        string parameters = "TS1=" + OperationTrailingStop + ";BRE=" + OperationBreakEven;

                        int response = bridge.OrderSend(symbol, type, lots, price, slippage, stopLossPips, OperationTakeProfit, parameters);

                        if (response >= 0)
                        {
                            Data.AddBarStats(OperationType.Buy, lots, price);
                            Data.WrongStopLoss = 0;
                            Data.WrongTakeProf = 0;
                            Data.WrongStopsRetry = 0;
                        }
                        else
                        {   // Error in operation execution.
                            if (Configs.PlaySounds)
                                Data.SoundError.Play();

                            if (bridge.LastError == 0)
                                jmsg = new JournalMessage(JournalIcons.Warning, DateTime.Now,
                                    Language.T("Operation execution") + ": " + Language.T("MetaTrader is not responding!").Replace("MetaTrader", Data.TerminalName));
                            else
                                jmsg = new JournalMessage(JournalIcons.Error, DateTime.Now,
                                    Language.T("MetaTrader failed to execute order! Returned").Replace("MetaTrader", Data.TerminalName) + ": " +
                                    MT4Bridge.MT4_Errors.ErrorDescription(bridge.LastError));
                            AppendJournalMessage(jmsg);
                            Data.WrongStopLoss = stopLossPips;
                            Data.WrongTakeProf = OperationTakeProfit;
                        }
                    }
                    break;
                case "btnSell":
                    {
                        MT4Bridge.OrderType type = MT4Bridge.OrderType.Sell;
                        string symbol     = Data.Symbol;
                        double lots       = NormalizeEntrySize(OperationLots);
                        double price      = Data.Bid;
                        int    slippage   = Configs.AutoSlippage ? (int)Data.InstrProperties.Spread * 3 : Configs.SlippageEntry;
                        
                        int stopLossPips = 0;
                        if (OperationStopLoss > 0 && OperationTrailingStop > 0)
                            stopLossPips = Math.Min(OperationStopLoss, OperationTrailingStop);
                        else
                            stopLossPips = Math.Max(OperationStopLoss, OperationTrailingStop);

                        double stoploss   = stopLossPips       > 0 ? Data.Ask + Data.InstrProperties.Point * stopLossPips : 0;
                        double takeprofit = OperationTakeProfit > 0 ? Data.Ask - Data.InstrProperties.Point * OperationTakeProfit : 0;

                        if (Configs.PlaySounds)
                            Data.SoundOrderSent.Play();

                        JournalMessage jmsg = new JournalMessage(JournalIcons.OrderSell, DateTime.Now, string.Format(symbol + " " + Data.PeriodMTStr + " " +
                            Language.T("An entry order sent") + ": " + Language.T("Sell") + " {0} " + (lots == 1 ? Language.T("lot") : Language.T("lots")) + " " + Language.T("at") + " {1}, " +
                            Language.T("Stop Loss") + " {2}, " + Language.T("Take Profit") + " {3}", lots, price.ToString(Data.FF), stoploss.ToString(Data.FF), takeprofit.ToString(Data.FF)));
                        AppendJournalMessage(jmsg);

                        string parameters = "TS1=" + OperationTrailingStop + ";BRE=" + OperationBreakEven;

                        int response = bridge.OrderSend(symbol, type, lots, price, slippage, stopLossPips, OperationTakeProfit, parameters);

                        if (response >= 0)
                        {
                            Data.AddBarStats(OperationType.Sell, lots, price);
                            Data.WrongStopLoss = 0;
                            Data.WrongTakeProf = 0;
                            Data.WrongStopsRetry = 0;
                        }
                        else
                        {   // Error in operation execution.
                            if (Configs.PlaySounds)
                                Data.SoundError.Play();

                            if (bridge.LastError == 0)
                                jmsg = new JournalMessage(JournalIcons.Warning, DateTime.Now,
                                    Language.T("Operation execution") + ": " + Language.T("MetaTrader is not responding!").Replace("MetaTrader", Data.TerminalName));
                            else
                                jmsg = new JournalMessage(JournalIcons.Error, DateTime.Now,
                                    Language.T("MetaTrader failed to execute order! Returned").Replace("MetaTrader", Data.TerminalName) + ": " + 
                                    MT4Bridge.MT4_Errors.ErrorDescription(bridge.LastError));
                            AppendJournalMessage(jmsg);
                            Data.WrongStopLoss = stopLossPips;
                            Data.WrongTakeProf = OperationTakeProfit;
                        }
                    }
                    break;
                case "btnClose":
                    {
                        string symbol   = Data.Symbol;
                        double lots     = NormalizeEntrySize(Data.PositionLots);
                        double price    = Data.PositionDirection == PosDirection.Long ? Data.Bid : Data.Ask;
                        int    slippage = Configs.AutoSlippage ? (int)Data.InstrProperties.Spread * 6 : Configs.SlippageExit;
                        int    ticket   = Data.PositionTicket;

                        if (ticket == 0)
                        {   // No position.
                            if (Configs.PlaySounds)
                                Data.SoundError.Play();
                            return;
                        }

                        if (Configs.PlaySounds)
                            Data.SoundOrderSent.Play();

                        JournalMessage jmsg = new JournalMessage(JournalIcons.OrderClose, DateTime.Now, string.Format(symbol + " " + Data.PeriodMTStr + " " +
                            Language.T("An exit order sent") + ": " + Language.T("Close") + " {0} " + (lots == 1 ? Language.T("lot") : Language.T("lots")) + " " + 
                            Language.T("at") + " {1}", lots, price.ToString(Data.FF)));
                        AppendJournalMessage(jmsg);

                        bool responseOK = bridge.OrderClose(ticket, lots, price, slippage);

                        if (responseOK)
                            Data.AddBarStats(OperationType.Close, lots, price);
                        else
                        {
                            if (Configs.PlaySounds)
                                Data.SoundError.Play();

                            if (bridge.LastError == 0)
                                jmsg = new JournalMessage(JournalIcons.Warning, DateTime.Now,
                                    Language.T("Operation execution") + ": " + Language.T("MetaTrader is not responding!").Replace("MetaTrader", Data.TerminalName));
                            else
                                jmsg = new JournalMessage(JournalIcons.Error, DateTime.Now,
                                    Language.T("MetaTrader failed to execute order! Returned").Replace("MetaTrader", Data.TerminalName) + ": " +
                                    MT4Bridge.MT4_Errors.ErrorDescription(bridge.LastError));
                            AppendJournalMessage(jmsg);
                        }
                        Data.WrongStopLoss = 0;
                        Data.WrongTakeProf = 0;
                        Data.WrongStopsRetry = 0;
                    }
                    break;
                case "btnModify":
                    {
                        string symbol = Data.Symbol;
                        double lots   = NormalizeEntrySize(Data.PositionLots);
                        double price  = Data.PositionDirection == PosDirection.Long ? Data.Bid : Data.Ask;
                        int    ticket = Data.PositionTicket;
                        double sign   = Data.PositionDirection == PosDirection.Long ? 1 : -1;

                        if (ticket == 0)
                        {   // No position.
                            if (Configs.PlaySounds)
                                Data.SoundError.Play();
                            return;
                        }

                        if (Configs.PlaySounds)
                            Data.SoundOrderSent.Play();

                        int stopLossPips = 0;
                        if (OperationStopLoss > 0 && OperationTrailingStop > 0)
                            stopLossPips = Math.Min(OperationStopLoss, OperationTrailingStop);
                        else
                            stopLossPips = Math.Max(OperationStopLoss, OperationTrailingStop);

                        double stoploss   = stopLossPips       > 0 ? price - sign * Data.InstrProperties.Point * stopLossPips       : 0;
                        double takeprofit = OperationTakeProfit > 0 ? price + sign * Data.InstrProperties.Point * OperationTakeProfit : 0;

                        JournalMessage jmsg = new JournalMessage(JournalIcons.Recalculate, DateTime.Now, string.Format(symbol + " " + Data.PeriodMTStr + " " +
                            Language.T("A modify order sent") + ": " + Language.T("Stop Loss") + " {0}, " + Language.T("Take Profit") + " {1}",
                            stoploss.ToString(Data.FF), takeprofit.ToString(Data.FF)));
                        AppendJournalMessage(jmsg);

                        string parameters = "TS1=" + OperationTrailingStop + ";BRE=" + OperationBreakEven;

                        bool responseOK = bridge.OrderModify(ticket, price, stopLossPips, OperationTakeProfit, parameters);

                        if (responseOK)
                        {
                            Data.AddBarStats(OperationType.Modify, lots, price);
                            Data.WrongStopLoss = 0;
                            Data.WrongTakeProf = 0;
                            Data.WrongStopsRetry = 0;
                        }
                        else
                        {
                            if (Configs.PlaySounds)
                                Data.SoundError.Play();

                            if (bridge.LastError == 0)
                                jmsg = new JournalMessage(JournalIcons.Warning, DateTime.Now,
                                    Language.T("Operation execution") + ": " + Language.T("MetaTrader is not responding!").Replace("MetaTrader", Data.TerminalName));
                            else
                                jmsg = new JournalMessage(JournalIcons.Error, DateTime.Now,
                                    Language.T("MetaTrader failed to execute order! Returned").Replace("MetaTrader", Data.TerminalName) + ": " +
                                    MT4Bridge.MT4_Errors.ErrorDescription(bridge.LastError));
                            AppendJournalMessage(jmsg);
                            Data.WrongStopLoss = stopLossPips;
                            Data.WrongTakeProf = OperationTakeProfit;
                        }
                    }
                    break;
                default:
                    break;
            }

            return;
        }

        /// <summary>
        /// Use logical groups menu item.
        /// </summary>
        protected override void MenuUseLogicalGroups_OnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;

            if (mi.Checked == true)
            {
                Configs.UseLogicalGroups = mi.Checked;
                RebuildStrategyLayout();
                return;
            }

            // Check if the current strategy uses logical groups
            bool usefroup = false;
            List<string> closegroup = new List<string>();
            foreach (IndicatorSlot slot in Data.Strategy.Slot)
            {
                if (slot.SlotType == SlotTypes.OpenFilter && slot.LogicalGroup != "A")
                    usefroup = true;

                if (slot.SlotType == SlotTypes.CloseFilter)
                {
                    if (closegroup.Contains(slot.LogicalGroup) || slot.LogicalGroup == "all")
                        usefroup = true;
                    else
                        closegroup.Add(slot.LogicalGroup);
                }
            }

            if (!usefroup)
            {
                Configs.UseLogicalGroups = false;
                RebuildStrategyLayout();
            }
            else
            {
                MessageBox.Show(
                    Language.T("The strategy requires logical groups.") + Environment.NewLine +
                    Language.T("\"Use Logical Groups\" option cannot be switched off."),
                    Language.T("Logical Groups"),
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Information);

                mi.Checked = true;
            }
            
            return;
        }

        /// <summary>
        /// Menu MenuOpeningLogicSlots_OnClick
        /// </summary>
        protected override void MenuOpeningLogicSlots_OnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            Configs.MAX_ENTRY_FILTERS = (int)mi.Tag;

            foreach (ToolStripMenuItem m in mi.Owner.Items)
                m.Checked = ((int)m.Tag == Configs.MAX_ENTRY_FILTERS);

            RebuildStrategyLayout();
            return;
        }

        /// <summary>
        /// Menu MenuClosingLogicSlots_OnClick
        /// </summary>
        protected override void MenuClosingLogicSlots_OnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            Configs.MAX_EXIT_FILTERS = (int)mi.Tag;

            foreach (ToolStripMenuItem m in mi.Owner.Items)
                m.Checked = ((int)m.Tag == Configs.MAX_EXIT_FILTERS);

            RebuildStrategyLayout();
            return;
        }

        /// <summary>
        /// Reset settings
        /// </summary>
        void ResetSettings()
        {
            DialogResult result = MessageBox.Show(
                Language.T("Do you want to reset all settings?") + Environment.NewLine + Environment.NewLine +
                Language.T("Restart the program to activate the changes!"),
                Language.T("Reset Settings"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
                Configs.ResetParams();
        }

        /// <summary>
        /// Reset data and stats.
        /// </summary>
        void ResetTrader()
        {
            tickLocalTime = DateTime.Now; // Prevents ping for one second.
            StopTrade();
            Data.IsConnected = false;

            bridge.ResetBarsManager();

            Data.ResetBidAsk();
            Data.ResetAccountStats();
            Data.ResetPositionStats();
            Data.ResetBarStats();
            Data.ResetTicks();

            UpdateTickChart(Data.InstrProperties.Point, Data.ListTicks.ToArray());
            UpdateBalanceChart(Data.BalanceData, Data.BalanceDataPoints);
            UpdateChart();

            return;
        }

        /// <summary>
        /// Starts the Calculator.
        /// </summary>
        void ShowCommandConsole()
        {
            Command_Console commandConsole = new Command_Console();
            commandConsole.Show();

            return;
        }

        /// <summary>
        /// Makes new language file.
        /// </summary>
        void MakeNewTranslation()
        {
            New_Translation nt = new New_Translation();
            nt.Show();

            return;
        }

        /// <summary>
        /// Edit translation.
        /// </summary>
        void EditTranslation()
        {
            Edit_Translation et = new Edit_Translation();
            et.Show();

            return;
        }
    }
}
