// Actions Class
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
    /// Class Actions : Controls
    /// </summary>
    public partial class Actions : Controls
    {
        /// <summary>
        /// The starting point of the application
        /// </summary>
        [STAThreadAttribute]
        public static void Main()
        {

            UpdateSplashScreeStatus("Loading...");
            Data.Start();
            Configs.LoadConfigs();

            // Checks if this is the only running copy of FST.
            if (!Configs.MultipleInstances)
            {
                System.Diagnostics.Process[] procs = System.Diagnostics.Process.GetProcessesByName(Data.ProgramName);
                if (procs.Length > 1)
                {
                    RemoveSplashScreen();
                    MessageBox.Show("Forex Strategy Trader is already running! You can allow multiple instances of the program from Tools menu.", Data.ProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    return;
                }
            }

            Language.InitLanguages();
            LayoutColors.InitColorSchemes();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Actions());

            return;
        }

        /// <summary>
        /// The default constructor
        /// </summary>
        public Actions()
        {
            StartPosition     = FormStartPosition.CenterScreen;
            Size              = new Size(785, 560);
            MinimumSize       = new Size(600, 370);
            Icon              = Data.Icon;
            Text              = Data.ProgramName;
            FormClosing      += new FormClosingEventHandler(Actions_FormClosing);
            Application.Idle += new EventHandler(Application_Idle);

            // Load a data file
            LoadInstrument(true);

            // Prepare custom indicators
            UpdateSplashScreeStatus("Loading custom indicators...");
            if (Configs.LoadCustomIndicators)
                Custom_Indicators.LoadCustomIndicators();

            // Load a strategy
            UpdateSplashScreeStatus("Loading strategy...");
            string sStrategyPath = Data.StrategyPath;
            if (Configs.RememberLastStr && Configs.LastStrategy != "")
            {
                string sLastStrategy = Path.GetDirectoryName(Configs.LastStrategy);
                if (sLastStrategy != "")
                    sLastStrategy = Configs.LastStrategy;
                else
                {
                    string sPath = Path.Combine(Data.ProgramDir, Data.DefaultStrategyDir);
                    sLastStrategy = Path.Combine(sPath, Configs.LastStrategy);
                }
                if (File.Exists(sLastStrategy))
                    sStrategyPath = sLastStrategy;
            }

            if (OpenStrategy(sStrategyPath) == 0)
            {
                CalculateStrategy(true);
                AfterStrategyOpening();
            }

            ChangeTabPage(Configs.LastTab);

            Live_Content liveContent = new Live_Content(Data.SystemDir, miLiveContent, miForex, pnlUsefulLinks, pnlForexBrokers);

            // Starting tips
            if (Configs.ShowStartingTip)
            {
                Starting_Tips startingTips = new Starting_Tips();
                startingTips.Show();
            }

            UpdateSplashScreeStatus("Loading user interface...");

            return;
        }

        /// <summary>
        /// Application idle
        /// </summary>
        void Application_Idle(object sender, EventArgs e)
        {
            Application.Idle -= new EventHandler(Application_Idle);
            RemoveSplashScreen();

            SetSrategyOverview();

            if (!Configs.MultipleInstances)
                InitDataFeed();

            return;
        }

        /// <summary>
        /// Removes the splash screen.
        /// </summary>
        static void RemoveSplashScreen()
        {
            string sLockFile = GetLockFile();
            if (!string.IsNullOrEmpty(sLockFile))
                File.Delete(sLockFile);
        }

        /// <summary>
        /// Updates the splash screen label.
        /// </summary>
        static void UpdateSplashScreeStatus(string comment)
        {
            try
            {
                TextWriter tw = new StreamWriter(GetLockFile(), false);
                tw.WriteLine(comment);
                tw.Close();
            }
            catch { }
        }

        /// <summary>
        /// The lockfile name will be passed automatically by Splash.exe as a  
        /// command line argument -lockfile="c:\temp\C1679A85-A4FA-48a2-BF77-E74F73E08768.lock"
        /// </summary>
        /// <returns>Lock file path</returns>
        static string GetLockFile()
        {
            foreach (string arg in Environment.GetCommandLineArgs())
                if (arg.StartsWith("-lockfile="))
                    return arg.Replace("-lockfile=", String.Empty);

            return string.Empty;
        }

        /// <summary>
        /// Checks whether the strategy have been saved or not
        /// </summary>
        void Actions_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialogResult = WhetherSaveChangedStrategy();

            if (dialogResult == DialogResult.Yes)
                SaveStrategy();
            else if (dialogResult == DialogResult.Cancel)
                e.Cancel = true;

            // Remember the last used strategy
            if (Configs.RememberLastStr)
            {
                if (Data.LoadedSavedStrategy != "")
                {
                    string sStrategyPath = Path.GetDirectoryName(Data.LoadedSavedStrategy) + "\\";
                    string sDefaultPath  = Path.Combine(Data.ProgramDir, Data.DefaultStrategyDir);
                    if (sStrategyPath == sDefaultPath)
                        Data.LoadedSavedStrategy = Path.GetFileName(Data.LoadedSavedStrategy);
                }
                Configs.LastStrategy = Data.LoadedSavedStrategy;
            }

            DeinitDataFeed();
            Configs.SaveConfigs();
            if(!e.Cancel) this.Hide();
            Data.SendStats();

            return;
        }

// ---------------------------------------------------------- //

        /// <summary>
        /// Edits the Strategy Properties Slot
        /// </summary>
        void EditStrategyProperties()
        {
            Strategy_Properties strprp = new Strategy_Properties();
            strprp.SameDirAverg    = Data.Strategy.SameSignalAction;
            strprp.OppDirAverg     = Data.Strategy.OppSignalAction;
            strprp.UseAccountPercentEntry = Data.Strategy.UseAccountPercentEntry;
            strprp.MaxOpenLots     = Data.Strategy.MaxOpenLots;
            strprp.EntryLots       = Data.Strategy.EntryLots;
            strprp.AddingLots      = Data.Strategy.AddingLots;
            strprp.ReducingLots    = Data.Strategy.ReducingLots;
            strprp.UsePermanentSL  = Data.Strategy.UsePermanentSL;
            strprp.PermanentSLType = Data.Strategy.PermanentSLType;
            strprp.PermanentSL     = Data.Strategy.PermanentSL;
            strprp.UsePermanentTP  = Data.Strategy.UsePermanentTP;
            strprp.PermanentTPType = Data.Strategy.PermanentTPType;
            strprp.PermanentTP     = Data.Strategy.PermanentTP;
            strprp.UseBreakEven    = Data.Strategy.UseBreakEven;
            strprp.BreakEven       = Data.Strategy.BreakEven;
            strprp.SetParams();
            strprp.ShowDialog();

            if (strprp.DialogResult == DialogResult.OK)
            {
                OnStrategyChange();

                Data.StackStrategy.Push(Data.Strategy.Clone());

                Data.Strategy.SameSignalAction = strprp.SameDirAverg;
                Data.Strategy.OppSignalAction  = strprp.OppDirAverg;
                Data.Strategy.UseAccountPercentEntry = strprp.UseAccountPercentEntry;
                Data.Strategy.MaxOpenLots      = strprp.MaxOpenLots;
                Data.Strategy.EntryLots        = strprp.EntryLots;
                Data.Strategy.AddingLots       = strprp.AddingLots;
                Data.Strategy.ReducingLots     = strprp.ReducingLots;
                Data.Strategy.UsePermanentSL   = strprp.UsePermanentSL;
                Data.Strategy.PermanentSLType  = strprp.PermanentSLType;
                Data.Strategy.PermanentSL      = strprp.PermanentSL;
                Data.Strategy.UsePermanentTP   = strprp.UsePermanentTP;
                Data.Strategy.PermanentTPType  = strprp.PermanentTPType;
                Data.Strategy.PermanentTP      = strprp.PermanentTP;
                Data.Strategy.UseBreakEven     = strprp.UseBreakEven;
                Data.Strategy.BreakEven        = strprp.BreakEven;

                RebuildStrategyLayout();
                SetSrategyOverview();

                Data.IsStrategyChanged = true;

                CalculateStrategy(false);
            }

            return;
        }

        /// <summary>
        /// Edits the Strategy Slot
        /// </summary>
        /// <param name="iSlot">The slot number</param>
        void EditSlot(int iSlot)
        {
            Data.IsStrategyReady = false;

            SlotTypes slotType   = Data.Strategy.Slot[iSlot].SlotType;
            bool      bIsDefined = Data.Strategy.Slot[iSlot].IsDefined;

            //We put the current Strategy into the stack only if this function is called from the
            //button SlotButton. If it is called from Add/Remove Filters the stack is already updated.
            if (bIsDefined)
            {
                Data.StackStrategy.Push(Data.Strategy.Clone());
            }

            Indicator_Dialog id = new Indicator_Dialog(iSlot, slotType, bIsDefined);
            id.ShowDialog();

            if (id.DialogResult == DialogResult.OK)
            {
                OnStrategyChange();

                Data.IsStrategyChanged = true;

                RebuildStrategyLayout();
                SetSrategyOverview();
            }
            else
            {   // Cancel was pressed
                UndoStrategy();
            }

            Data.IsStrategyReady = true;

            return;
        }

        /// <summary>
        /// Moves a Slot Upwards
        /// </summary>
        void MoveSlotUpwards(int iSlotToMove)
        {
            Data.StackStrategy.Push(Data.Strategy.Clone());
            Data.Strategy.MoveFilterUpwards(iSlotToMove);

            Data.IsStrategyChanged = true;

            RebuildStrategyLayout();
            SetSrategyOverview();

            CalculateStrategy(true);

            return;
        }

        /// <summary>
        /// Moves a Slot Downwards
        /// </summary>
        void MoveSlotDownwards(int iSlotToMove)
        {
            Data.StackStrategy.Push(Data.Strategy.Clone());
            Data.Strategy.MoveFilterDownwards(iSlotToMove);

            Data.IsStrategyChanged = true;

            RebuildStrategyLayout();
            SetSrategyOverview();

            CalculateStrategy(true);

            return;
        }

        /// <summary>
        /// Duplicates a Slot
        /// </summary>
        void DuplicateSlot(int iSlotToDuplicate)
        {
            OnStrategyChange();

            Data.StackStrategy.Push(Data.Strategy.Clone());
            Data.Strategy.DuplicateFilter(iSlotToDuplicate);

            Data.IsStrategyChanged = true;

            RebuildStrategyLayout();
            SetSrategyOverview();

            CalculateStrategy(true);

            return;
        }

        /// <summary>
        /// Adds a new Open filter
        /// </summary>
        void AddOpenFilter()
        {
            OnStrategyChange();

            Data.StackStrategy.Push(Data.Strategy.Clone());
            Data.Strategy.AddOpenFilter();
            EditSlot(Data.Strategy.OpenFilters);

            return;
        }

        /// <summary>
        /// Adds a new Close filter
        /// </summary>
        void AddCloseFilter()
        {
            OnStrategyChange();

            Data.StackStrategy.Push(Data.Strategy.Clone());
            Data.Strategy.AddCloseFilter();
            EditSlot(Data.Strategy.Slots - 1);

            return;
        }

        /// <summary>
        /// Removes a strategy slot.
        /// </summary>
        /// <param name="iSlot">Slot to remove</param>
        void RemoveSlot(int iSlot)
        {
            OnStrategyChange();

            Data.IsStrategyChanged = true;

            Data.StackStrategy.Push(Data.Strategy.Clone());
            Data.Strategy.RemoveFilter(iSlot);

            RebuildStrategyLayout();
            SetSrategyOverview();

            CalculateStrategy(false);
        }

        /// <summary>
        /// Undoes the strategy
        /// </summary>
        void UndoStrategy()
        {
            OnStrategyChange();

            if (Data.StackStrategy.Count <= 1)
            {
                Data.IsStrategyChanged = false;
            }

            if (Data.StackStrategy.Count > 0)
            {
                Data.Strategy = Data.StackStrategy.Pop();

                RebuildStrategyLayout();
                SetSrategyOverview();

                CalculateStrategy(true);
            }

            return;
        }

        /// <summary>
        /// Performs actions when UPBV has been changed
        /// </summary>
        void UsePreviousBarValue_Change()
        {
            if (miStrategyAUPBV.Checked == false)
            {
                // Confirmation Message
                string sMessageText = Language.T("Are you sure you want to control \"Use previous bar value\" manually?");
                DialogResult dialogResult = MessageBox.Show(sMessageText, Language.T("Use previous bar value"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.Yes)
                {   // OK, we are sure
                    OnStrategyChange();

                    Data.AutoUsePrvBarValue = false;

                    foreach (IndicatorSlot indicatorSlot in Data.Strategy.Slot)
                        foreach (CheckParam checkParam in indicatorSlot.IndParam.CheckParam)
                            if (checkParam.Caption == "Use previous bar value")
                                checkParam.Enabled = true;
                }
                else
                {   // Not just now
                    miStrategyAUPBV.Checked = true;
                }
            }
            else
            {
                OnStrategyChange();

                Data.AutoUsePrvBarValue = true;
                Data.Strategy.AdjustUsePreviousBarValue();
                RepaintStrategyLayout();
                CalculateStrategy(true);
            }

            return;
        }

        /// <summary>
        /// Ask for saving the changed strategy
        /// </summary>
        DialogResult WhetherSaveChangedStrategy()
        {
            DialogResult dr = DialogResult.No;
            if (Data.IsStrategyChanged)
            {
                string sMessageText = Language.T("Do you want to save the current strategy?") + "\r\n" + Data.StrategyName;
                dr = MessageBox.Show(sMessageText, Data.ProgramName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            }

            return dr;
        }

        /// <summary>
        /// LoadInstrument
        /// </summary>
        int LoadInstrument(bool bUseResource)
        {
            string      sSymbol    = "EURUSD";
            DataPeriods dataPeriod = DataPeriods.day;

            Instrument_Properties instrProperties = new Instrument_Properties(sSymbol);
            Instrument instrument = new Instrument(instrProperties, (int)dataPeriod);
            int iLoadDataResult = instrument.LoadResourceData();

            if (instrument.Bars > 0 && iLoadDataResult == 0)
            {
                Data.InstrProperties = instrProperties.Clone();

                Data.Bars   = instrument.Bars;
                Data.Period = dataPeriod;

                Data.Time   = new DateTime[Data.Bars];
                Data.Open   = new double[Data.Bars];
                Data.High   = new double[Data.Bars];
                Data.Low    = new double[Data.Bars];
                Data.Close  = new double[Data.Bars];
                Data.Volume = new int[Data.Bars];

                for (int iBar = 0; iBar < Data.Bars; iBar++)
                {
                    Data.Open[iBar]   = instrument.Open(iBar);
                    Data.High[iBar]   = instrument.High(iBar);
                    Data.Low[iBar]    = instrument.Low(iBar);
                    Data.Close[iBar]  = instrument.Close(iBar);
                    Data.Time[iBar]   = instrument.Time(iBar);
                    Data.Volume[iBar] = instrument.Volume(iBar);
                }

                Data.IsData = true;
            }

            return 0;
        }

        /// <summary>
        /// Open a strategy file
        /// </summary>
        void ShowOpenFileDialog()
        {
            OpenFileDialog opendlg = new OpenFileDialog();

            opendlg.InitialDirectory = Data.StrategyDir;
            opendlg.Filter = Language.T("Strategy file") + " (*.xml)|*.xml";
            opendlg.Title  = Language.T("Open Strategy");

            if (opendlg.ShowDialog() == DialogResult.OK)
            {
                string sStrategyfullName = opendlg.FileName;
                LoadStrategyFile(sStrategyfullName);
            }

            return;
        }

        /// <summary>
        /// New Strategy
        /// </summary>
        void NewStrategy()
        {
            Data.StrategyDir = Path.Combine(Data.ProgramDir, Data.DefaultStrategyDir);
            string sStrategyfullName = Path.Combine(Data.StrategyDir, "New.xml");

            LoadStrategyFile(sStrategyfullName);

            return;
        }

        /// <summary>
        /// Loads the strategy given.
        /// </summary>
        void LoadStrategyFile(string strategyfullName)
        {
            try
            {
                OnStrategyChange();

                OpenStrategy(strategyfullName);
                CalculateStrategy(true);
                AfterStrategyOpening();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, Text);
            }

            return;
        }

        /// <summary>
        ///Reloads the Custom Indicators.
        /// </summary>
        void ReloadCustomIndicators()
        {   // Check if the strategy contains custom indicators
            bool bStrategyHasCustomIndicator = false;
            foreach (IndicatorSlot slot in Data.Strategy.Slot)
            {   // Searching the strategy slots for a custom indicator
                if (Indicator_Store.CustomIndicatorNames.Contains(slot.IndicatorName))
                {
                    bStrategyHasCustomIndicator = true;
                    break;
                }
            }

            // Reload all the custom indicators
            Custom_Indicators.LoadCustomIndicators();

            if (bStrategyHasCustomIndicator)
            {   // Load and calculate a new strategy
                Data.StrategyDir = Path.Combine(Data.ProgramDir, Data.DefaultStrategyDir);

                if (OpenStrategy(Path.Combine(Data.StrategyDir, "New.xml")) == 0)
                {
                    CalculateStrategy(true);
                    AfterStrategyOpening();
                }
            }

            return;
        }

        /// <summary>
        /// Reads the strategy from a file.
        /// </summary>
        /// <param name="sStrategyName">The strategy name.</param>
        /// <returns>0 - success.</returns>
        int OpenStrategy(string sStrategyName)
        {
            try
            {
                if (File.Exists(sStrategyName) && Strategy.Load(sStrategyName) == 0)
                {   // Successfully opened
                    Data.Strategy.StrategyName = Path.GetFileNameWithoutExtension(sStrategyName);
                    Data.StrategyDir  = Path.GetDirectoryName(sStrategyName);
                    Data.StrategyName = Path.GetFileName(sStrategyName);
                }
                else
                {
                    Strategy.GenerateNew();
                    string sMessageText = Language.T("The strategy could not be loaded correctly!") + Environment.NewLine + Language.T("A new strategy has been generated!");
                    MessageBox.Show(sMessageText, Language.T("Strategy Loading"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Data.LoadedSavedStrategy = "";
                }

                Data.SetStrategyIndicators();

                RebuildStrategyLayout();
                SetSrategyOverview();

                SetFormText(); 
                Data.IsStrategyChanged = false;
                Data.LoadedSavedStrategy = Data.StrategyPath;

                Data.StackStrategy.Clear();
            }
            catch
            {
                Strategy.GenerateNew();
                string sMessageText = Language.T("The strategy could not be loaded correctly!") + Environment.NewLine + Language.T("A new strategy has been generated!");
                MessageBox.Show(sMessageText, Language.T("Strategy Loading"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Data.LoadedSavedStrategy = "";
                SetFormText();
                RebuildStrategyLayout();
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// Save the current strategy
        /// </summary>
        int SaveStrategy()
        {
            if (Data.StrategyName == "New.xml")
            {
                SaveAsStrategy();
            }
            else
            {
                try
                {
                    Data.Strategy.Save(Data.StrategyPath);
                    Data.IsStrategyChanged = false;
                    Data.LoadedSavedStrategy = Data.StrategyPath;
                    Data.SavedStrategies++;
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, Text);
                    return -1;
                }
            }

            return 0;
        }

        /// <summary>
        /// Save the current strategy
        /// </summary>
        int SaveAsStrategy()
        {
            //Creates a dialog form SaveFileDialog
            SaveFileDialog savedlg = new SaveFileDialog();

            savedlg.InitialDirectory = Data.StrategyDir;
            savedlg.FileName         = Path.GetFileName(Data.StrategyName);
            savedlg.AddExtension     = true;
            savedlg.Title   = Language.T("Save the Strategy As");
            savedlg.Filter  = Language.T("Strategy file") + " (*.xml)|*.xml";

            if (savedlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Data.StrategyName = Path.GetFileName(savedlg.FileName);
                    Data.StrategyDir  = Path.GetDirectoryName(savedlg.FileName);
                    Data.Strategy.Save(savedlg.FileName);
                    Data.IsStrategyChanged = false;
                    Data.LoadedSavedStrategy = Data.StrategyPath;
                    Data.SavedStrategies++;
                    SetFormText();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, Text);
                    return -1;
                }
            }

            return 0;
        }

        /// <summary>
        /// Calculates the strategy.
        /// </summary>
        /// <param name="bRecalcIndicators">true - to recalculate all the indicators.</param>
        void CalculateStrategy(bool bRecalcIndicators)
        {
            // Calculates the indicators by slots if it's necessary
            if (bRecalcIndicators)
                Data.FirstBar = Data.Strategy.Calculate();

            return;
        }

        /// <summary>
        /// Stops trade and shows a message.
        /// </summary>
        void AfterStrategyOpening()
        {
            StopTrade();
            JournalMessage msg = new JournalMessage(JournalIcons.Information, DateTime.Now,
                Language.T("Strategy") + " \"" + Data.Strategy.StrategyName + "\" " + Language.T("loaded successfully."));
            AppendJournalMessage(msg);

            return;
        }

        /// <summary>
        /// Stops trade and selects Strategy page.
        /// </summary>
        void OnStrategyChange()
        {
            // Stops auto trade
            StopTrade();

            // Selects Strategy tab page.
            ChangeTabPage(1);
        }

        /// <summary>
        /// Loads a color scheme.
        /// </summary>
        void LoadColorScheme()
        {
            string colorSchemeFile = Path.Combine(Data.ColorDir, Configs.ColorScheme + ".xml");

            if (File.Exists(colorSchemeFile))
            {
                LayoutColors.LoadColorScheme(colorSchemeFile);
                SetColors();
            }

            return;
        }

        /// <summary>
        /// Sets the caption text of the application.
        /// </summary>
        void SetFormText()
        {
            string connection = "";
            if (Configs.MultipleInstances)
                connection = "ID=" + Data.ConnectionID + " ";

            if (Data.IsConnected)
                connection += Data.Symbol + " " + Data.PeriodStr + ", ";

            string text = connection + Path.GetFileNameWithoutExtension(Data.StrategyName) + " - " + Data.ProgramName;

            SetFormTextThreadSafely(text);
        }

        delegate void SetFormTextDelegate(string text);
        void SetFormTextThreadSafely(string text)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new SetFormTextDelegate(SetFormTextThreadSafely), new object[] { text });
            }
            else
            {
                this.Text = text;
            }
        }
    }
}
