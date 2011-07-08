// Menu_and_StatusBar Class
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
    /// Provides the StatusBar and MainMenu 
    /// </summary>
    public class Menu_and_StatusBar : Workspace
    {
        ToolStripStatusLabel tsslEquityInfo;
        ToolStripStatusLabel tsslPositionInfo;
        ToolStripStatusLabel tsslConnMarket;
        ToolStripStatusLabel tsslTickInfo;
        ToolStripStatusLabel tsslConnIcon;
        protected ToolStripMenuItem miStrategyAUPBV;
        protected ToolStripMenuItem miForex;
        protected ToolStripMenuItem miLiveContent;
        protected ToolStripMenuItem miTabStatus;
        protected ToolStripMenuItem miTabStrategy;
        protected ToolStripMenuItem miTabChart;
        protected ToolStripMenuItem miTabAccount;
        protected ToolStripMenuItem miTabJournal;
        protected ToolStripMenuItem miTabOperation;

        /// <summary>
        /// The default constructor
        /// </summary>
        public Menu_and_StatusBar()
        {
            InitializeMenu();
            InitializeStatusBar();
        }

        /// <summary>
        /// Sets the Main Menu.
        /// </summary>
        private void InitializeMenu()
        {
            // File
            ToolStripMenuItem miFile = new ToolStripMenuItem(Language.T("File"));

            ToolStripMenuItem miNew = new ToolStripMenuItem();
            miNew.Text         = Language.T("New");
            miNew.Image        = Properties.Resources.strategy_new;
            miNew.ShortcutKeys = Keys.Control | Keys.N;
            miNew.ToolTipText  = Language.T("Open the default strategy \"New.xml\".");
            miNew.Click       += new EventHandler(MenuStrategyNew_OnClick);
            miFile.DropDownItems.Add(miNew);

            ToolStripMenuItem miOpen = new ToolStripMenuItem();
            miOpen.Text         = Language.T("Open...");
            miOpen.Image        = Properties.Resources.strategy_open;
            miOpen.ShortcutKeys = Keys.Control | Keys.O;
            miOpen.ToolTipText  = Language.T("Open a strategy.");
            miOpen.Click       += new EventHandler(MenuFileOpen_OnClick);
            miFile.DropDownItems.Add(miOpen);

            ToolStripMenuItem miSave = new ToolStripMenuItem();
            miSave.Text         = Language.T("Save");
            miSave.Image        = Properties.Resources.strategy_save;
            miSave.ShortcutKeys = Keys.Control | Keys.S;
            miSave.ToolTipText  = Language.T("Save the strategy.");
            miSave.Click       += new EventHandler(MenuFileSave_OnClick);
            miFile.DropDownItems.Add(miSave);

            ToolStripMenuItem miSaveAs = new ToolStripMenuItem();
            miSaveAs.Text        = Language.T("Save As") + "...";
            miSaveAs.Image       = Properties.Resources.strategy_save_as;
            miSaveAs.ToolTipText = Language.T("Save a copy of the strategy.");
            miSaveAs.Click      += new EventHandler(MenuFileSaveAs_OnClick);
            miFile.DropDownItems.Add(miSaveAs);

            miFile.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miClose = new ToolStripMenuItem();
            miClose.Text         = Language.T("Exit");
            miClose.Image        = Properties.Resources.exit;
            miClose.ToolTipText  = Language.T("Close the program.");
            miClose.ShortcutKeys = Keys.Control | Keys.X;
            miClose.Click       += new EventHandler(MenuFileCloseOnClick);
            miFile.DropDownItems.Add(miClose);

            // Edit
            ToolStripMenuItem miEdit = new ToolStripMenuItem(Language.T("Edit"));

            ToolStripMenuItem miStrategyUndo = new ToolStripMenuItem();
            miStrategyUndo.Text         = Language.T("Undo");
            miStrategyUndo.Image        = Properties.Resources.strategy_undo;
            miStrategyUndo.ToolTipText  = Language.T("Undo the last change in the strategy.");
            miStrategyUndo.ShortcutKeys = Keys.Control | Keys.Z;
            miStrategyUndo.Click       += new EventHandler(MenuStrategyUndo_OnClick);
            miEdit.DropDownItems.Add(miStrategyUndo);

            miEdit.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miStrategyCopy = new ToolStripMenuItem();
            miStrategyCopy.Text         = Language.T("Copy Strategy");
            miStrategyCopy.ToolTipText  = Language.T("Copy the entire strategy to the clipboard.");
            miStrategyCopy.Image        = Properties.Resources.copy;
            miStrategyCopy.ShortcutKeys = Keys.Control | Keys.C;
            miStrategyCopy.Click       += new EventHandler(MenuStrategyCopy_OnClick);
            miEdit.DropDownItems.Add(miStrategyCopy);

            ToolStripMenuItem miStrategyPaste = new ToolStripMenuItem();
            miStrategyPaste.Text         = Language.T("Paste Strategy");
            miStrategyPaste.ToolTipText  = Language.T("Load a strategy from the clipboard.");
            miStrategyPaste.Image        = Properties.Resources.paste;
            miStrategyPaste.ShortcutKeys = Keys.Control | Keys.V;
            miStrategyPaste.Click       += new EventHandler(MenuStrategyPaste_OnClick);
            miEdit.DropDownItems.Add(miStrategyPaste);

            //View
            ToolStripMenuItem miView = new ToolStripMenuItem(Language.T("View"));

            ToolStripMenuItem miLanguage = new ToolStripMenuItem();
            miLanguage.Text = "Language";
            miLanguage.Image = Properties.Resources.lang;
            for (int i = 0; i < Language.LanguageList.Length; i++)
            {
                ToolStripMenuItem miLang = new ToolStripMenuItem();
                miLang.Text    = Language.LanguageList[i];
                miLang.Name    = Language.LanguageList[i];
                miLang.Checked = miLang.Name == Configs.Language;
                miLang.Click  += new EventHandler(Language_Click);
                miLanguage.DropDownItems.Add(miLang);
            }

            miView.DropDownItems.Add(miLanguage);

            ToolStripMenuItem miLanguageTools = new ToolStripMenuItem();
            miLanguageTools.Text  = Language.T("Language Tools");
            miLanguageTools.Image = Properties.Resources.lang_tools;

            ToolStripMenuItem miNewTranslation = new ToolStripMenuItem();
            miNewTranslation.Name   = "miNewTranslation";
            miNewTranslation.Text   = Language.T("Make New Translation") + "...";
            miNewTranslation.Image  = Properties.Resources.new_translation;
            miNewTranslation.Click += new EventHandler(MenuTools_OnClick);
            miLanguageTools.DropDownItems.Add(miNewTranslation);

            ToolStripMenuItem miEditTranslation = new ToolStripMenuItem();
            miEditTranslation.Name   = "miEditTranslation";
            miEditTranslation.Text   = Language.T("Edit Current Translation") + "...";
            miEditTranslation.Image  = Properties.Resources.edit_translation;
            miEditTranslation.Click += new EventHandler(MenuTools_OnClick);
            miLanguageTools.DropDownItems.Add(miEditTranslation);

            miLanguageTools.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miShowEnglishPhrases = new ToolStripMenuItem();
            miShowEnglishPhrases.Name = "miShowEnglishPhrases";
            miShowEnglishPhrases.Text = Language.T("Show English Phrases") + "...";
            miShowEnglishPhrases.Image = Properties.Resources.view_translation;
            miShowEnglishPhrases.Click += new EventHandler(MenuTools_OnClick);
            miLanguageTools.DropDownItems.Add(miShowEnglishPhrases);

            ToolStripMenuItem miShowAltPhrases = new ToolStripMenuItem();
            miShowAltPhrases.Name = "miShowAltPhrases";
            miShowAltPhrases.Text = Language.T("Show Translated Phrases") + "...";
            miShowAltPhrases.Image = Properties.Resources.view_translation;
            miShowAltPhrases.Click += new EventHandler(MenuTools_OnClick);
            miLanguageTools.DropDownItems.Add(miShowAltPhrases);

            ToolStripMenuItem miShowBothPhrases = new ToolStripMenuItem();
            miShowBothPhrases.Name = "miShowAllPhrases";
            miShowBothPhrases.Text = Language.T("Show All Phrases") + "...";
            miShowBothPhrases.Image = Properties.Resources.view_translation;
            miShowBothPhrases.Click += new EventHandler(MenuTools_OnClick);
            miLanguageTools.DropDownItems.Add(miShowBothPhrases);

            miView.DropDownItems.Add(miLanguageTools);

            miView.DropDownItems.Add(new ToolStripSeparator());

            miTabStatus = new ToolStripMenuItem();
            miTabStatus.Text         = Language.T("Status page");
            miTabStatus.Name         = "miStatus";
            miTabStatus.Tag          = 0;
            miTabStatus.Checked      = true;
            miTabStatus.ShortcutKeys = Keys.Control | Keys.D1;
            miTabStatus.Click       += new EventHandler(MenuChangeTabs_OnClick);
            miView.DropDownItems.Add(miTabStatus);

            miTabStrategy = new ToolStripMenuItem();
            miTabStrategy.Text         = Language.T("Strategy page");
            miTabStrategy.Name         = "miStrategy";
            miTabStrategy.Tag          = 1;
            miTabStrategy.Checked      = false;
            miTabStrategy.ShortcutKeys = Keys.Control | Keys.D2;
            miTabStrategy.Click       += new EventHandler(MenuChangeTabs_OnClick);
            miView.DropDownItems.Add(miTabStrategy);

            miTabChart = new ToolStripMenuItem();
            miTabChart.Text         = Language.T("Chart page");
            miTabChart.Name         = "miChart";
            miTabChart.Tag          = 2;
            miTabChart.Checked      = false;
            miTabChart.ShortcutKeys = Keys.Control | Keys.D3;
            miTabChart.Click       += new EventHandler(MenuChangeTabs_OnClick);
            miView.DropDownItems.Add(miTabChart);

            miTabAccount = new ToolStripMenuItem();
            miTabAccount.Text         = Language.T("Account page");
            miTabAccount.Name         = "miAccount";
            miTabAccount.Tag          = 3;
            miTabAccount.Checked      = false;
            miTabAccount.ShortcutKeys = Keys.Control | Keys.D4;
            miTabAccount.Click       += new EventHandler(MenuChangeTabs_OnClick);
            miView.DropDownItems.Add(miTabAccount);

            miTabJournal = new ToolStripMenuItem();
            miTabJournal.Text         = Language.T("Journal page");
            miTabJournal.Name         = "miJournal";
            miTabJournal.Tag          = 4;
            miTabJournal.Checked      = false;
            miTabJournal.ShortcutKeys = Keys.Control | Keys.D5;
            miTabJournal.Click       += new EventHandler(MenuChangeTabs_OnClick);
            miView.DropDownItems.Add(miTabJournal);

            miTabOperation = new ToolStripMenuItem();
            miTabOperation.Text         = Language.T("Operation page");
            miTabOperation.Name         = "miOperation";
            miTabOperation.Tag          = 5;
            miTabOperation.Checked      = false;
            miTabOperation.ShortcutKeys = Keys.Control | Keys.D6;
            miTabOperation.Click       += new EventHandler(MenuChangeTabs_OnClick);
            miView.DropDownItems.Add(miTabOperation);

            miView.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miLoadColor = new ToolStripMenuItem();
            miLoadColor.Text = Language.T("Colour Scheme");
            miLoadColor.Image = Properties.Resources.palette;
            for (int i = 0; i < LayoutColors.ColorSchemeList.Length; i++)
            {
                ToolStripMenuItem miColor = new ToolStripMenuItem();
                miColor.Text = LayoutColors.ColorSchemeList[i];
                miColor.Name = LayoutColors.ColorSchemeList[i];
                miColor.Checked = miColor.Name == Configs.ColorScheme;
                miColor.Click += new EventHandler(MenuLoadColor_OnClick);
                miLoadColor.DropDownItems.Add(miColor);
            }

            miView.DropDownItems.Add(miLoadColor);

            ToolStripMenuItem miGradientView = new ToolStripMenuItem();
            miGradientView.Text    = Language.T("Gradient View");
            miGradientView.Name    = "miGradientView";
            miGradientView.Checked = Configs.GradientView;
            miGradientView.CheckOnClick = true;
            miGradientView.Click  += new EventHandler(MenuGradientView_OnClick);
            miView.DropDownItems.Add(miGradientView);

            // Strategy
            ToolStripMenuItem miStrategy = new ToolStripMenuItem(Language.T("Strategy"));

            ToolStripMenuItem miStrategyOverview = new ToolStripMenuItem();
            miStrategyOverview.Text         = Language.T("Overview") + "...";
            miStrategyOverview.Image        = Properties.Resources.overview;
            miStrategyOverview.ToolTipText  = Language.T("See the strategy overview.");
            miStrategyOverview.ShortcutKeys = Keys.F4;
            miStrategyOverview.Click       += new EventHandler(MenuStrategyOverview_OnClick);
            miStrategy.DropDownItems.Add(miStrategyOverview);

            miStrategy.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miStrategyPublish = new ToolStripMenuItem();
            miStrategyPublish.Text        = Language.T("Publish") + "...";
            miStrategyPublish.Image       = Properties.Resources.publish_strategy;
            miStrategyPublish.ToolTipText = Language.T("Publish the strategy in the program's forum.");
            miStrategyPublish.Click      += new EventHandler(MenuStrategyBBcode_OnClick);
            miStrategy.DropDownItems.Add(miStrategyPublish);

            miStrategy.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miUseLogicalGroups = new ToolStripMenuItem();
            miUseLogicalGroups.Text         = Language.T("Use Logical Groups");
            miUseLogicalGroups.ToolTipText  = Language.T("Groups add AND and OR logic interaction of the indicators.");
            miUseLogicalGroups.Checked      = Configs.UseLogicalGroups;
            miUseLogicalGroups.CheckOnClick = true;
            miUseLogicalGroups.Click       += new EventHandler(MenuUseLogicalGroups_OnClick);
            miStrategy.DropDownItems.Add(miUseLogicalGroups);

            ToolStripMenuItem miOpeningLogicConditions = new ToolStripMenuItem();
            miOpeningLogicConditions.Text = Language.T("Max number of Opening Logic Conditions");
            miOpeningLogicConditions.Image = Properties.Resources.numb_gr;
            miStrategy.DropDownItems.Add(miOpeningLogicConditions);

            for (int i = 2; i < 9; i++)
            {
                ToolStripMenuItem miOpeningLogicSlots = new ToolStripMenuItem();
                miOpeningLogicSlots.Text    = i.ToString();
                miOpeningLogicSlots.Tag     = i;
                miOpeningLogicSlots.Checked = (Configs.MAX_ENTRY_FILTERS == i);
                miOpeningLogicSlots.Click  += new EventHandler(MenuOpeningLogicSlots_OnClick);
                miOpeningLogicConditions.DropDownItems.Add(miOpeningLogicSlots);
            }

            ToolStripMenuItem miClosingLogicConditions = new ToolStripMenuItem();
            miClosingLogicConditions.Text = Language.T("Max number of Closing Logic Conditions");
            miClosingLogicConditions.Image = Properties.Resources.numb_br;
            miStrategy.DropDownItems.Add(miClosingLogicConditions);

            for (int i = 2; i < 9; i++)
            {
                ToolStripMenuItem miClosingLogicSlots = new ToolStripMenuItem();
                miClosingLogicSlots.Text    = i.ToString();
                miClosingLogicSlots.Tag     = i;
                miClosingLogicSlots.Checked = (Configs.MAX_EXIT_FILTERS == i);
                miClosingLogicSlots.Click  += new EventHandler(MenuClosingLogicSlots_OnClick);
                miClosingLogicConditions.DropDownItems.Add(miClosingLogicSlots);
            }

            miStrategy.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miStrategyRemember = new ToolStripMenuItem();
            miStrategyRemember.Text         = Language.T("Remember the Last Strategy");
            miStrategyRemember.ToolTipText  = Language.T("Load the last used strategy at startup.");
            miStrategyRemember.Checked      = Configs.RememberLastStr;
            miStrategyRemember.CheckOnClick = true;
            miStrategyRemember.Click       += new EventHandler(MenuStrategyRemember_OnClick);
            miStrategy.DropDownItems.Add(miStrategyRemember);

            miStrategyAUPBV = new ToolStripMenuItem();
            miStrategyAUPBV.Text         = Language.T("Auto Control of \"Use previous bar value\"");
            miStrategyAUPBV.ToolTipText  = Language.T("Provides automatic setting of the indicators' check box \"Use previous bar value\".");
            miStrategyAUPBV.Checked      = true;
            miStrategyAUPBV.CheckOnClick = true;
            miStrategyAUPBV.Click       += new EventHandler(MenuStrategyAUPBV_OnClick);
            miStrategy.DropDownItems.Add(miStrategyAUPBV);

            miStrategy.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miStrategySettings = new ToolStripMenuItem();
            miStrategySettings.Text   = Language.T("Trade Settings");
            miStrategySettings.Image  = Properties.Resources.strategy_settings;
            miStrategySettings.Click += new EventHandler(MenuTradeSettings_OnClick);
            miStrategy.DropDownItems.Add(miStrategySettings);

            // Tools
            ToolStripMenuItem miTools = new ToolStripMenuItem(Language.T("Tools"));

            ToolStripMenuItem miCustomInd = new ToolStripMenuItem();
            miCustomInd.Name        = "CustomIndicators";
            miCustomInd.Text        = Language.T("Custom Indicators");
            miCustomInd.Image       = Properties.Resources.custom_ind;

            ToolStripMenuItem miReloadInd = new ToolStripMenuItem();
            miReloadInd.Name         = "miReloadInd";
            miReloadInd.Text         = Language.T("Reload the Custom Indicators");
            miReloadInd.Image        = Properties.Resources.reload_ind;
            miReloadInd.ShortcutKeys = Keys.Control | Keys.I;
            miReloadInd.Click       += new EventHandler(MenuTools_OnClick);
            miCustomInd.DropDownItems.Add(miReloadInd);

            ToolStripMenuItem miCheckInd = new ToolStripMenuItem();
            miCheckInd.Name   = "miCheckInd";
            miCheckInd.Text   = Language.T("Check the Custom Indicators");
            miCheckInd.Image  = Properties.Resources.check_ind;
            miCheckInd.Click += new EventHandler(MenuTools_OnClick);
            miCustomInd.DropDownItems.Add(miCheckInd);

            miCustomInd.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miOpenIndFolder = new ToolStripMenuItem();
            miOpenIndFolder.Name   = "miOpenIndFolder";
            miOpenIndFolder.Text   = Language.T("Open the Source Files Folder") + "...";
            miOpenIndFolder.Image  = Properties.Resources.folder_open;
            miOpenIndFolder.Click += new EventHandler(MenuTools_OnClick);
            miCustomInd.DropDownItems.Add(miOpenIndFolder);

            ToolStripMenuItem miCustIndForum = new ToolStripMenuItem();
            miCustIndForum.Text   = Language.T("Custom Indicators Forum") + "...";
            miCustIndForum.Image  = Properties.Resources.forum_icon;
            miCustIndForum.Tag    = "http://forexsb.com/forum/forum/30/";
            miCustIndForum.Click += new EventHandler(MenuHelpContentsOnClick);
            miCustomInd.DropDownItems.Add(miCustIndForum);

            miCustomInd.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miLoadCstomInd = new ToolStripMenuItem();
            miLoadCstomInd.Name         = "miLoadCstomInd";
            miLoadCstomInd.Text         = Language.T("Load the Custom Indicators at Startup");
            miLoadCstomInd.Checked      = Configs.LoadCustomIndicators;
            miLoadCstomInd.CheckOnClick = true;
            miLoadCstomInd.Click       += new EventHandler(LoadCustomIndicators_OnClick);
            miCustomInd.DropDownItems.Add(miLoadCstomInd);

            ToolStripMenuItem miShowCstomInd = new ToolStripMenuItem();
            miShowCstomInd.Name         = "miShowCstomInd";
            miShowCstomInd.Text         = Language.T("Show the Loaded Custom Indicators");
            miShowCstomInd.Checked      = Configs.ShowCustomIndicators;
            miShowCstomInd.CheckOnClick = true;
            miShowCstomInd.Click       += new EventHandler(ShowCustomIndicators_OnClick);
            miCustomInd.DropDownItems.Add(miShowCstomInd);

            miTools.DropDownItems.Add(miCustomInd);
            miTools.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miMultipleInstances = new ToolStripMenuItem();
            miMultipleInstances.Text         = Language.T("Allow multiple working copies of FST");
            miMultipleInstances.Name         = "miMultipleInstances";
            miMultipleInstances.Checked      = Configs.MultipleInstances;
            miMultipleInstances.CheckOnClick = true;
            miMultipleInstances.Click       += new EventHandler(MenuMultipleInstances_OnClick);
            miTools.DropDownItems.Add(miMultipleInstances);

            ToolStripMenuItem miPlaySounds = new ToolStripMenuItem();
            miPlaySounds.Text         = Language.T("Play Sounds");
            miPlaySounds.Name         = "miPlaySounds";
            miPlaySounds.Checked      = Configs.PlaySounds;
            miPlaySounds.CheckOnClick = true;
            miPlaySounds.Click       += new EventHandler(MenuPlaySounds_OnClick);
            miTools.DropDownItems.Add(miPlaySounds);
            miTools.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miAdditional = new ToolStripMenuItem();
            miAdditional.Text  = Language.T("Additional");
            miAdditional.Image = Properties.Resources.tools;

            miTools.DropDownItems.Add(miAdditional);

            ToolStripMenuItem miCommandConsole = new ToolStripMenuItem();
            miCommandConsole.Name   = "CommandConsole";
            miCommandConsole.Text   = Language.T("Command Console") + "...";
            miCommandConsole.Image  = Properties.Resources.prompt;
            miCommandConsole.Click += new EventHandler(MenuTools_OnClick);
            miAdditional.DropDownItems.Add(miCommandConsole);

            miTools.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miInstallExpert = new ToolStripMenuItem();
            miInstallExpert.Name        = "miInstallExpert";
            miInstallExpert.Text        = Language.T("Install MetaTrader Files");
            miInstallExpert.ToolTipText = Language.T("Install the necessary files in the MetaTrader's folder.");
            miInstallExpert.Image       = Properties.Resources.expert_advisor;
            miInstallExpert.Click      += new EventHandler(MenuTools_OnClick);
            miTools.DropDownItems.Add(miInstallExpert);

            ToolStripMenuItem miResetTrader = new ToolStripMenuItem();
            miResetTrader.Name        = "miResetTrader";
            miResetTrader.Text        = Language.T("Reset Data and Statistics");
            miResetTrader.ToolTipText = Language.T("Reset the loaded data and statistics. It will stop the auto trading!");
            miResetTrader.Image       = Properties.Resources.recalculate;
            miResetTrader.Click      += new EventHandler(MenuTools_OnClick);
            miTools.DropDownItems.Add(miResetTrader);

            ToolStripMenuItem miResetConfigs = new ToolStripMenuItem();
            miResetConfigs.Name        = "Reset settings";
            miResetConfigs.Text        = Language.T("Reset Settings");
            miResetConfigs.ToolTipText = Language.T("Reset the program settings to their default values. You need to restart!");
            miResetConfigs.Image       = Properties.Resources.warning;
            miResetConfigs.Click      += new EventHandler(MenuTools_OnClick);
            miTools.DropDownItems.Add(miResetConfigs);

            // Help
            ToolStripMenuItem miHelp = new ToolStripMenuItem(Language.T("Help"));

            ToolStripMenuItem miTipOfTheDay = new ToolStripMenuItem();
            miTipOfTheDay.Text        = Language.T("Tip of the Day") + "...";
            miTipOfTheDay.ToolTipText = Language.T("Show a tip.");
            miTipOfTheDay.Image       = Properties.Resources.hint;
            miTipOfTheDay.Tag         = "tips";
            miTipOfTheDay.Click      += new EventHandler(MenuHelpContentsOnClick);
            miHelp.DropDownItems.Add(miTipOfTheDay);

            ToolStripMenuItem miHelpOnlineHelp = new ToolStripMenuItem();
            miHelpOnlineHelp.Text         = Language.T("Online Help") + "...";
            miHelpOnlineHelp.Image        = Properties.Resources.help;
            miHelpOnlineHelp.ToolTipText  = Language.T("Show the online help.");
            miHelpOnlineHelp.Tag          = "http://forexsb.com/wiki/fst/manual/start";
            miHelpOnlineHelp.ShortcutKeys = Keys.F1;
            miHelpOnlineHelp.Click       += new EventHandler(MenuHelpContentsOnClick);
            miHelp.DropDownItems.Add(miHelpOnlineHelp);

            ToolStripMenuItem miHelpForum = new ToolStripMenuItem();
            miHelpForum.Text        = Language.T("Support Forum") + "...";
            miHelpForum.Image       = Properties.Resources.forum_icon;
            miHelpForum.Tag         = "http://forexsb.com/forum/";
            miHelpForum.ToolTipText = Language.T("Show the program's forum.");
            miHelpForum.Click      += new EventHandler(MenuHelpContentsOnClick);
            miHelp.DropDownItems.Add(miHelpForum);

            miHelp.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miHelpDonateNow = new ToolStripMenuItem();
            miHelpDonateNow.Text        = Language.T("Contribute") + "...";
            miHelpDonateNow.Image       = Properties.Resources.contribute;
            miHelpDonateNow.ToolTipText = Language.T("Donate, Support, Advertise!");
            miHelpDonateNow.Tag         = "http://forexsb.com/wiki/contribution";
            miHelpDonateNow.Click      += new EventHandler(MenuHelpContentsOnClick);
            miHelp.DropDownItems.Add(miHelpDonateNow);

            miHelp.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miHelpUpdates = new ToolStripMenuItem();
            miHelpUpdates.Text         = Language.T("Check for Updates at Startup");
            miHelpUpdates.Checked      = Configs.CheckForUpdates;
            miHelpUpdates.CheckOnClick = true;
            miHelpUpdates.Click       += new EventHandler(MenuHelpUpdates_OnClick);
            miHelp.DropDownItems.Add(miHelpUpdates);

            ToolStripMenuItem miHelpNewBeta = new ToolStripMenuItem();
            miHelpNewBeta.Text         = Language.T("Check for New Beta Versions");
            miHelpNewBeta.Checked      = Configs.CheckForNewBeta;
            miHelpNewBeta.CheckOnClick = true;
            miHelpNewBeta.Click       += new EventHandler(MenuHelpNewBeta_OnClick);
            miHelp.DropDownItems.Add(miHelpNewBeta);


            miHelp.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem miHelpAbout = new ToolStripMenuItem();
            miHelpAbout.Text        = Language.T("About") + " " + Data.ProgramName + "...";
            miHelpAbout.ToolTipText = Language.T("Show the program information.");
            miHelpAbout.Image       = Properties.Resources.information;
            miHelpAbout.Click      += new EventHandler(MenuHelpAboutOnClick);
            miHelp.DropDownItems.Add(miHelpAbout);

            // Forex
            miForex = new ToolStripMenuItem(Language.T("Forex"));

            ToolStripMenuItem miForexBrokers = new ToolStripMenuItem();
            miForexBrokers.Text   = Language.T("Forex Brokers") + "...";
            miForexBrokers.Image  = Properties.Resources.forex_brokers;
            miForexBrokers.Tag    = "http://forexsb.com/forex-brokers/";
            miForexBrokers.Click += new EventHandler(MenuForexContentsOnClick);

            miForex.DropDownItems.Add(miForexBrokers);

            // LiveContent
            miLiveContent = new ToolStripMenuItem(Language.T("New Version"));
            miLiveContent.Alignment = ToolStripItemAlignment.Right;
            miLiveContent.BackColor = Color.Khaki;
            miLiveContent.ForeColor = Color.DarkGreen;
            miLiveContent.Visible   = false;

            // Forex Forum
            ToolStripMenuItem miForum = new ToolStripMenuItem(Properties.Resources.forum_icon);
            miForum.Alignment   = ToolStripItemAlignment.Right;
            miForum.Tag         = "http://forexsb.com/forum/";
            miForum.ToolTipText = Language.T("Show the program's forum.");
            miForum.Click      += new EventHandler(MenuForexContentsOnClick);

            // MainMenu
            ToolStripMenuItem[] mainMenu =
            {
                miFile, miEdit, miView, miStrategy, miTools,
                miHelp, miForex, miLiveContent, miForum
            };

            MainMenuStrip.Items.AddRange(mainMenu);
            MainMenuStrip.ShowItemToolTips = true;
        }

        /// <summary>
        /// Sets the StatusBar
        /// </summary>
        private void InitializeStatusBar()
        {
            statusStrip.GripStyle = ToolStripGripStyle.Hidden;
            statusStrip.SizingGrip = false;

            tsslEquityInfo = new ToolStripStatusLabel();
            tsslEquityInfo.Image = Properties.Resources.currency;
            tsslEquityInfo.Text = string.Format("{0} {1}", Data.AccountEquity, Data.AccountCurrency);
            statusStrip.Items.Add(tsslEquityInfo);

            statusStrip.Items.Add(new ToolStripSeparator());

            tsslPositionInfo = new ToolStripStatusLabel();
            tsslPositionInfo.Image      = null;
            tsslPositionInfo.Text       = String.Empty;
            tsslPositionInfo.Spring     = true;
            tsslPositionInfo.ImageAlign = ContentAlignment.MiddleLeft;
            tsslPositionInfo.TextAlign  = ContentAlignment.MiddleLeft;
            statusStrip.Items.Add(tsslPositionInfo);

            statusStrip.Items.Add(new ToolStripSeparator());

            tsslConnMarket = new ToolStripStatusLabel();
            tsslConnMarket.Text = "Not Connected";
            statusStrip.Items.Add(tsslConnMarket);

            tsslTickInfo = new ToolStripStatusLabel();
            statusStrip.Items.Add(tsslTickInfo);

            statusStrip.Items.Add(new ToolStripSeparator());

            tsslConnIcon = new ToolStripStatusLabel();
            tsslConnIcon.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsslConnIcon.Image = Properties.Resources.not_connected;
            statusStrip.Items.Add(tsslConnIcon);
        }

        /// <summary>
        /// Saves the current strategy
        /// </summary>
        protected virtual void MenuFileSave_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Opens the SaveAs menu
        /// </summary>
        protected virtual void MenuFileSaveAs_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Opens a saved strategy
        /// </summary>
        protected virtual void MenuFileOpen_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Closes the program
        /// </summary>
        private void MenuFileCloseOnClick(object sender, EventArgs e)
        {
            this.Close();
        }

        // Sets the programs language
        protected virtual void Language_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            if (!mi.Checked)
            {
                string sMessageText = Language.T("Restart the program to activate the changes!");
                MessageBox.Show(sMessageText, Language.T("Language Change"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Configs.Language = mi.Name;
            }
            foreach (ToolStripMenuItem tsmi in mi.Owner.Items)
            {
                tsmi.Checked = false;
            }
            mi.Checked = true;

            return;
        }

        /// <summary>
        /// Gradient View Changed
        /// </summary>
        protected virtual void MenuGradientView_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Play Sounds Changed
        /// </summary>
        protected virtual void MenuPlaySounds_OnClick(object sender, EventArgs e)
        {
            Configs.PlaySounds = ((ToolStripMenuItem)sender).Checked;

            return;
        }

        /// <summary>
        /// Menu Multiple Instances Changed
        /// </summary>
        protected virtual void MenuMultipleInstances_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Load a colour scheme
        /// </summary>
        protected virtual void MenuLoadColor_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Loads the default strategy
        /// </summary>
        protected virtual void MenuStrategyNew_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Opens the strategy settings dialogue
        /// </summary>
        protected virtual void MenuStrategySettings_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Opens the strategy settings dialogue
        /// </summary>
        protected virtual void MenuStrategyAUPBV_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Remember the last used strategy
        /// </summary>
        protected void MenuStrategyRemember_OnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            Configs.RememberLastStr = mi.Checked;
            if (mi.Checked == false)
            {
                Configs.LastStrategy = "";
            }

            return;
        }

        /// <summary>
        /// Opens the strategy overview window
        /// </summary>
        protected void MenuStrategyOverview_OnClick(object sender, EventArgs e)
        {
            Browser so = new Browser(Language.T("Strategy Overview"), Data.Strategy.GenerateHTMLOverview());
            so.Show();
        }

        /// <summary>
        /// Undoes the strategy
        /// </summary>
        protected virtual void MenuStrategyUndo_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Copies the strategy to clipboard.
        /// </summary>
        protected virtual void MenuStrategyCopy_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Pastes a strategy from clipboard.
        /// </summary>
        protected virtual void MenuStrategyPaste_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Menu MenuOpeningLogicSlots_OnClick
        /// </summary>
        protected virtual void MenuOpeningLogicSlots_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Menu MenuClosingLogicSlots_OnClick
        /// </summary>
        protected virtual void MenuClosingLogicSlots_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Use logical groups menu item.
        /// </summary>
        protected virtual void MenuUseLogicalGroups_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Add a new Open Filter
        /// </summary>
        protected virtual void MenuStrategyAddOpenFilter_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Add a new Close Filter
        /// </summary>
        protected virtual void MenuStrategyAddCloseFilter_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Export the strategy in BBCode format - ready to post in the forum
        /// </summary>
        protected virtual void MenuStrategyBBcode_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Opens the about window
        /// </summary>
        private void MenuHelpAboutOnClick(object sender, EventArgs e)
        {
            AboutScreen abScr = new AboutScreen();
            abScr.ShowDialog();

            return;
        }

        /// <summary>
        /// Tools menu
        /// </summary>
        protected virtual void MenuTools_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// MenuShowTabs_OnClick
        /// </summary>
        protected virtual void MenuShowTabs_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// MenuChangeTabs_OnClick
        /// </summary>
        protected virtual void MenuChangeTabs_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Trade Settings
        /// </summary>
        void MenuTradeSettings_OnClick(object sender, EventArgs e)
        {
            Trade_Settings ts = new Trade_Settings();
            ts.ShowDialog();

            return;
        }

        /// <summary>
        /// Opens the help window
        /// </summary>
        private void MenuHelpContentsOnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;

            if ((string)mi.Tag == "tips")
            {
                Starting_Tips shv = new Starting_Tips();
                shv.Show();
                return;
            }

            try
            {
                System.Diagnostics.Process.Start((string)mi.Tag);
            }
            catch{ }

            return;
        }

        /// <summary>
        /// Opens the forex news
        /// </summary>
        private void MenuForexContentsOnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;

            try
            {
                System.Diagnostics.Process.Start((string)mi.Tag);
            }
            catch{ }

            return;
        }

        /// <summary>
        /// Menu miHelpUpdates click
        /// </summary>
        protected virtual void MenuHelpUpdates_OnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            Configs.CheckForUpdates = mi.Checked;

            return;
        }

        /// <summary>
        /// Menu miHelpNewBeta  click
        /// </summary>
        protected virtual void MenuHelpNewBeta_OnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            Configs.CheckForNewBeta = mi.Checked;

            return;
        }

        /// <summary>
        /// Menu LoadCustomIndicators click
        /// </summary>
        protected virtual void LoadCustomIndicators_OnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            Configs.LoadCustomIndicators = mi.Checked;

            return;
        }

        /// <summary>
        /// Menu ShowCustomIndicators click
        /// </summary>
        protected virtual void ShowCustomIndicators_OnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            Configs.ShowCustomIndicators = mi.Checked;

            return;
        }

        delegate void SetEquityInfoTextDelegate(string text);
        /// <summary>
        /// Sets the tsslEquityInfo
        /// </summary>
        protected void SetEquityInfoText(string text)
        {
            if (tsslEquityInfo.Owner.InvokeRequired)
            {
                tsslEquityInfo.Owner.BeginInvoke(new SetTickInfoTextDelegate(SetEquityInfoText), new object[] { text });
            }
            else
            {
                tsslEquityInfo.Text = text;
            }
        }

        delegate void SetPositionInfoTextDelegate(Image image, string text);
        /// <summary>
        /// Sets the tsslPositionInfo
        /// </summary>
        protected void SetPositionInfoText(Image image, string text)
        {
            if (tsslPositionInfo.Owner.InvokeRequired)
            {
                tsslPositionInfo.Owner.BeginInvoke(new SetPositionInfoTextDelegate(SetPositionInfoText), new object[] { image, text });
            }
            else
            {
                tsslPositionInfo.Image = image;
                tsslPositionInfo.Text  = text;
            }
        }

        delegate  void SetConnMarketTextDelegate(string text);
        /// <summary>
        /// Sets the tsslConnMarket
        /// </summary>
        protected void SetConnMarketText(string text)
        {
            if (tsslConnMarket.Owner.InvokeRequired)
            {
                tsslConnMarket.Owner.BeginInvoke(new SetConnMarketTextDelegate(SetConnMarketText), new object[] { text });
            }
            else
            {
                tsslConnMarket.Text = text;
            }
        }

        delegate  void SetTickInfoTextDelegate(string text);
        /// <summary>
        /// Sets the tsslTickInfo
        /// </summary>
        protected void SetTickInfoText(string text)
        {
            if (tsslTickInfo.Owner.InvokeRequired)
            {
                tsslTickInfo.Owner.BeginInvoke(new SetTickInfoTextDelegate(SetTickInfoText), new object[] { text });
            }
            else
            {
                tsslTickInfo.Text = text;
            }
        }

        delegate void SetConnIconDelegate(int mode);
        /// <summary>
        /// Sets the tsslConnIcon
        /// </summary>
        protected void SetConnIcon(int mode)
        {
            if (tsslConnIcon.Owner.InvokeRequired)
            {
                tsslConnIcon.Owner.BeginInvoke(new SetConnIconDelegate(SetConnIcon), new object[] { mode });
            }
            else
            {
                switch (mode)
                {
                    case 0: // Not connected
                        tsslConnIcon.Image = Properties.Resources.not_connected;
                        break;
                    case 1: // Connected, No ticks
                        tsslConnIcon.Image = Properties.Resources.connected_no_ticks;
                        break;
                    case 2: // Connected, Ticks
                        tsslConnIcon.Image = Properties.Resources.connected;
                        break;
                    case 3: // Connected, Wrong ping
                        tsslConnIcon.Image = Properties.Resources.connection_wrong_ping;
                        break;
                    case 4: // Connected, Wrong pings
                        tsslConnIcon.Image = Properties.Resources.connection_wrong_pings;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
