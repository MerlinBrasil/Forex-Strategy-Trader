// Configs Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Forex_Strategy_Trader
{
    public static class Configs
    {
        static XmlDocument xmlConfig;
        static string pathToConfigFile;
        static bool   isConfigLoaded   = false;
        static bool   isResetActivated = false;

        // Constant parameters
        static int    MAX_ENTRY_FILTERSDefault          = 4;
        static int    MAX_EXIT_FILTERSDefault           = 2;
        static int    SIGMA_MODE_MAIN_CHARTDefault      = 1;
        static int    SIGMA_MODE_SEPARATED_CHARTDefault = 5;

        // Trade settings
        static string longTradeLogicPriceDefault = "Bid";
        static int    barCloseAdvanceDefault     = 3;
        static bool   autoSlippageDefault        = true;
        static int    slippageEntryOrdersDefault = 5;
        static int    slippageExitOrdersDefault  = 10;
        static int    minChartBarsDefault        = 400;

        // Settings
        static string languageDefault          = "English";
        static bool   showStartingTipDefault   = true;
        static int    currentTipNumberDefault  = -1;
        static bool   gradientViewDefault      = true;
        static string colorSchemeDefault       = "Light Blue";
        static bool   multipleInstancesDefault = false;
        static bool   playSoundsDefault        = true;
        static bool   rememberLastStrDefault   = true;
        static string lastStrategyDefault      = "";
        static bool   checkForUpdatesDefault   = true;
        static bool   checkForNewBetaDefault   = false;
        static bool   loadCustIndDefault       = true;
        static bool   showCustIndDefault       = false;
        static bool   bridgeWritesLogDefault   = false;
        static int    lastTabDefault           = 0;
        static bool   useLogicalGroupsDefault  = false;
        static int    journalLengthDefault     = 1000;

        // Chart
        static int  chartZoomDefault            = 9;
        static bool isChartInfoPanelDefault     = false;
        static bool isChartGridDefault          = true;
        static bool isChartCrossDefault         = false;
        static bool isChartVolumeDefault        = false;
        static bool isChartLotsDefault          = true;
        static bool isChartOrdersDefault        = true;
        static bool isChartPositionPriceDefault = true;
        static bool isChartTrueChartsDefault    = false;
        static bool isChartShiftDefault         = false;
        static bool isChartAutoScrollDefault    = true;

        // Journal
        static bool journalShowSystemMsgDefault = false;
        static bool journalShowTicksDefault     = false;

// ------------------------------------------------------------
        static int iMAX_ENTRY_FILTERS = MAX_ENTRY_FILTERSDefault;
        /// <summary>
        /// Maximum Entry Slots
        /// </summary>
        public static int MAX_ENTRY_FILTERS
        {
            get { return iMAX_ENTRY_FILTERS; }
            set
            {
                iMAX_ENTRY_FILTERS = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/MAX_ENTRY_FILTERS").Item(0).InnerText = value.ToString();
            }
        }

        static int iMAX_EXIT_FILTERS = MAX_EXIT_FILTERSDefault;
        /// <summary>
        /// Maximum Exit Slots
        /// </summary>
        public static int MAX_EXIT_FILTERS
        {
            get { return iMAX_EXIT_FILTERS; }
            set
            {
                iMAX_EXIT_FILTERS = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/MAX_EXIT_FILTERS").Item(0).InnerText = value.ToString();
            }
        }

        static int iSIGMA_MODE_MAIN_CHART = SIGMA_MODE_MAIN_CHARTDefault;
        /// <summary>
        /// Maximum Entry Slots
        /// </summary>
        public static int SIGMA_MODE_MAIN_CHART
        {
            get { return iSIGMA_MODE_MAIN_CHART; }
            set
            {
                iSIGMA_MODE_MAIN_CHART = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/SIGMA_MODE_MAIN_CHART").Item(0).InnerText = value.ToString();
            }
        }

        static int iSIGMA_MODE_SEPARATED_CHART = SIGMA_MODE_SEPARATED_CHARTDefault;
        /// <summary>
        /// Maximum Exit Slots
        /// </summary>
        public static int SIGMA_MODE_SEPARATED_CHART
        {
            get { return iSIGMA_MODE_SEPARATED_CHART; }
            set
            {
                iSIGMA_MODE_SEPARATED_CHART = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/SIGMA_MODE_SEPARATED_CHART").Item(0).InnerText = value.ToString();
            }
        }

// ------------------------------------------------------------
        // Base Price for long trade
        static string songTradeLogicPrice = longTradeLogicPriceDefault;
        /// <summary>
        /// Base Price for long trade
        /// </summary>
        public static string LongTradeLogicPrice
        {
            get { return songTradeLogicPrice; }
            set
            {
                songTradeLogicPrice = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/trade/longTradeLogicPrice").Item(0).InnerText = value.ToString();
            }
        }

        static int barCloseAdvance = barCloseAdvanceDefault;
        /// <summary>
        /// Bar closing advance in seconds.
        /// </summary>
        public static int BarCloseAdvance
        {
            get { return barCloseAdvance; }
            set
            {
                barCloseAdvance = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/trade/barCloseAdvance").Item(0).InnerText = value.ToString();
            }
        }

        static bool autoSlippage = autoSlippageDefault;
        /// <summary>
        /// Gets or sets value showing if the Automatic slippage is on.
        /// </summary>
        public static bool AutoSlippage
        {
            get { return autoSlippage; }
            set
            {
                autoSlippage = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/trade/autoSlippage").Item(0).InnerText = value.ToString();
            }
        }

        static int slippageEntryOrders = slippageEntryOrdersDefault;
        /// <summary>
        /// Max slippage for the entry orders.
        /// </summary>
        public static int SlippageEntry
        {
            get { return slippageEntryOrders; }
            set
            {
                slippageEntryOrders = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/trade/slippageEntryOrders").Item(0).InnerText = value.ToString();
            }
        }

        static int slippageExitOrders = slippageExitOrdersDefault;
        /// <summary>
        /// Max slippage for the exit orders.
        /// </summary>
        public static int SlippageExit
        {
            get { return slippageExitOrders; }
            set
            {
                slippageExitOrders = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/trade/slippageExitOrders").Item(0).InnerText = value.ToString();
            }
        }

        static int minChartBars = minChartBarsDefault;
        /// <summary>
        /// Minimal number of chart's bars.
        /// </summary>
        public static int MinChartBars
        {
            get { return minChartBars; }
            set
            {
                minChartBars = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/trade/minChartBars").Item(0).InnerText = value.ToString();
            }
        }

// ------------------------------------------------------------
        static bool isInstalled = true;
        public static bool IsInstalled
        {
            get { return isInstalled; }
            set
            {
                isInstalled = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/installed").Item(0).InnerText = value.ToString();
            }
        }

        // Program's Language
        static string language = languageDefault;
        /// <summary>
        /// Last Strategy
        /// </summary>
        public static string Language
        {
            get { return language; }
            set
            {
                language = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/language").Item(0).InnerText = value.ToString();
            }
        }

        // Show starting Tips
        static bool showStartingTip = showStartingTipDefault;
        /// <summary>
        /// Whether to show starting tips
        /// </summary>
        public static bool ShowStartingTip
        {
            get { return showStartingTip; }
            set
            {
                showStartingTip = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/showStartingTip").Item(0).InnerText = value.ToString();
            }
        }

        // Current tip number
        static int currentTipNumber = currentTipNumberDefault;
        /// <summary>
        /// Gets or sets the current starting tip number
        /// </summary>
        public static int CurrentTipNumber
        {
            get { return currentTipNumber; }
            set
            {
                currentTipNumber = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/currentTipNumber").Item(0).InnerText = value.ToString();
            }
        }

        // Show Gradient View
        static bool gradientView = gradientViewDefault;
        /// <summary>
        /// Whether to show Gradient View
        /// </summary>
        public static bool GradientView
        {
            get { return gradientView; }
            set
            {
                gradientView = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/gradientView").Item(0).InnerText = value.ToString();
            }
        }

        // ColorScheme
        static string colorScheme = colorSchemeDefault;
        /// <summary>
        /// ColorScheme
        /// </summary>
        public static string ColorScheme
        {
            get { return colorScheme; }
            set
            {
                colorScheme = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/colorScheme").Item(0).InnerText = value.ToString();
            }
        }

        static bool multipleInstances = multipleInstancesDefault;
        /// <summary>
        /// Whether to allow multiple instances.
        /// </summary>
        public static bool MultipleInstances
        {
            get { return multipleInstances; }
            set
            {
                multipleInstances = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/multipleInstances").Item(0).InnerText = value.ToString();
            }
        }

        // Play sounds
        static bool playSounds = playSoundsDefault;
        /// <summary>
        /// Whether to play sounds on an event.
        /// </summary>
        public static bool PlaySounds
        {
            get { return playSounds; }
            set
            {
                playSounds = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/playSounds").Item(0).InnerText = value.ToString();
            }
        }

        // Remember the Last Strategy
        static bool rememberLastStr = rememberLastStrDefault;
        /// <summary>
        /// Remember the Last Strategy
        /// </summary>
        public static bool RememberLastStr
        {
            get { return rememberLastStr; }
            set
            {
                rememberLastStr = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/rememberLastStrategy").Item(0).InnerText = value.ToString();
            }
        }

        // Last Strategy
        static string lastStrategy = lastStrategyDefault;
        /// <summary>
        /// Last Strategy
        /// </summary>
        public static string LastStrategy
        {
            get { return lastStrategy; }
            set
            {
                lastStrategy = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/lastStrategy").Item(0).InnerText = value.ToString();
            }
        }

        // Check for new versions
        static bool checkForUpdates = checkForUpdatesDefault;
        /// <summary>
        /// Check for new versions at startup.
        /// </summary>
        public static bool CheckForUpdates
        {
            get { return checkForUpdates; }
            set
            {
                checkForUpdates = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/checkForUpdates").Item(0).InnerText = value.ToString();
            }
        }

        // Check for new beta
        static bool checkForNewBeta = checkForNewBetaDefault;
        /// <summary>
        /// Check for new new beta at startup.
        /// </summary>
        public static bool CheckForNewBeta
        {
            get { return checkForNewBeta; }
            set
            {
                checkForNewBeta = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/checkForNewBeta").Item(0).InnerText = value.ToString();
            }
        }

        static bool loadCustomIndicators = loadCustIndDefault;
        /// <summary>
        /// Whether to load custom indicators at startup.
        /// </summary>
        public static bool LoadCustomIndicators
        {
            get { return loadCustomIndicators; }
            set
            {
                loadCustomIndicators = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/loadCustomIndicators").Item(0).InnerText = value.ToString();
            }
        }
        static bool showCustomIndicators = showCustIndDefault;
        /// <summary>
        /// Whether to Show custom indicators at startup.
        /// </summary>
        public static bool ShowCustomIndicators
        {
            get { return showCustomIndicators; }
            set
            {
                showCustomIndicators = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/showCustomIndicators").Item(0).InnerText = value.ToString();
            }
        }
        static bool bridgeWritesLog = bridgeWritesLogDefault;
        /// <summary>
        /// Whether Bridge to write a log file.
        /// </summary>
        public static bool BridgeWritesLog
        {
            get { return bridgeWritesLog; }
            set
            {
                bridgeWritesLog = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/bridgeWritesLog").Item(0).InnerText = value.ToString();
            }
        }


        static bool journalShowSystemMsg = journalShowSystemMsgDefault;
        /// <summary>
        /// Whether to show system messages
        /// </summary>
        public static bool JournalShowSystemMessages
        {
            get { return journalShowSystemMsg; }
            set
            {
                journalShowSystemMsg = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/journal/showSystemMessages").Item(0).InnerText = value.ToString();
            }
        }

        static bool journalShowTicks = journalShowTicksDefault;
        /// <summary>
        /// Whether to show ticks
        /// </summary>
        public static bool JournalShowTicks
        {
            get { return journalShowTicks; }
            set
            {
                journalShowTicks = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/journal/showTicks").Item(0).InnerText = value.ToString();
            }
        }

        static int lastTab = lastTabDefault;
        public static int LastTab
        {
            get { return lastTab; }
            set
            {
                lastTab = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/lastTab").Item(0).InnerText = value.ToString();
            }
        }

        static bool useLogicalGroups = useLogicalGroupsDefault;
        /// <summary>
        /// Logical groups for the entry / exit filters.
        /// </summary>
        public static bool UseLogicalGroups
        {
            get { return useLogicalGroups; }
            set
            {
                useLogicalGroups = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/useLogicalGroups").Item(0).InnerText = value.ToString();
            }
        }

        static int journalLength = journalLengthDefault;
        public static int JournalLength
        {
            get { return journalLength; }
            set
            {
                journalLength = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/journalLength").Item(0).InnerText = value.ToString();
            }
        }

// -------------------------------------------------------------
        static int chartZoom = chartZoomDefault;
        public static int ChartZoom
        {
            get { return chartZoom; }
            set
            {
                chartZoom = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/chart/zoom").Item(0).InnerText = value.ToString();
            }
        }

        static bool isChartInfoPanel = isChartInfoPanelDefault;
        public static bool ChartInfoPanel
        {
            get { return isChartInfoPanel; }
            set
            {
                isChartInfoPanel = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/chart/infoPanel").Item(0).InnerText = value.ToString();
            }
        }

        static bool isChartGrid = isChartGridDefault;
        public static bool ChartGrid
        {
            get { return isChartGrid; }
            set
            {
                isChartGrid = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/chart/grid").Item(0).InnerText = value.ToString();
            }
        }

        static bool isChartCross = isChartCrossDefault;
        public static bool ChartCross
        {
            get { return isChartCross; }
            set
            {
                isChartCross = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/chart/cross").Item(0).InnerText = value.ToString();
            }
        }

        static bool isChartVolume = isChartVolumeDefault;
        public static bool ChartVolume
        {
            get { return isChartVolume; }
            set
            {
                isChartVolume = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/chart/volume").Item(0).InnerText = value.ToString();
            }
        }

        static bool isChartLots = isChartLotsDefault;
        public static bool ChartLots
        {
            get { return isChartLots; }
            set
            {
                isChartLots = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/chart/lots").Item(0).InnerText = value.ToString();
            }
        }

        static bool isChartOrders = isChartOrdersDefault;
        public static bool ChartOrders
        {
            get { return isChartOrders; }
            set
            {
                isChartOrders = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/chart/orders").Item(0).InnerText = value.ToString();
            }
        }

        static bool isChartPositionPrice = isChartPositionPriceDefault;
        public static bool ChartPositionPrice
        {
            get { return isChartPositionPrice; }
            set
            {
                isChartPositionPrice = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/chart/positionPrice").Item(0).InnerText = value.ToString();
            }
        }

        static bool isChartTrueCharts = isChartTrueChartsDefault;
        public static bool ChartTrueCharts
        {
            get { return isChartTrueCharts; }
            set
            {
                isChartTrueCharts = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/chart/trueCharts").Item(0).InnerText = value.ToString();
            }
        }

        static bool isChartShift = isChartShiftDefault;
        public static bool ChartShift
        {
            get { return isChartShift; }
            set
            {
                isChartShift = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/chart/shift").Item(0).InnerText = value.ToString();
            }
        }

        static bool isChartAutoScroll = isChartAutoScrollDefault;
        public static bool ChartAutoScroll
        {
            get { return isChartAutoScroll; }
            set
            {
                isChartAutoScroll = value;
                if (isConfigLoaded)
                    xmlConfig.SelectNodes("config/chart/autoScroll").Item(0).InnerText = value.ToString();
            }
        }

        // =========================================================

        /// <summary>
        /// Public constructor
        /// </summary>
        static Configs()
        {
            xmlConfig = new XmlDocument();
            pathToConfigFile = Path.Combine(Data.SystemDir, @"config.xml");

            return;
        }

        /// <summary>
        /// Sets the params to its default value
        /// </summary>
        public static void ResetParams()
        {
            if (!isConfigLoaded)
                return;

            // Constants
            MAX_ENTRY_FILTERS          = MAX_ENTRY_FILTERSDefault;
            MAX_EXIT_FILTERS           = MAX_EXIT_FILTERSDefault;
            SIGMA_MODE_MAIN_CHART      = SIGMA_MODE_MAIN_CHARTDefault;
            SIGMA_MODE_SEPARATED_CHART = SIGMA_MODE_SEPARATED_CHARTDefault;

            // Trade settings
            LongTradeLogicPrice        = longTradeLogicPriceDefault;
            BarCloseAdvance            = barCloseAdvanceDefault;
            AutoSlippage               = autoSlippageDefault;
            SlippageEntry              = slippageEntryOrdersDefault;
            SlippageExit               = slippageExitOrdersDefault;
            MinChartBars               = minChartBarsDefault;

            // Program
            Language                   = languageDefault;
            ShowStartingTip            = showStartingTipDefault;
            CurrentTipNumber           = currentTipNumberDefault;
            ColorScheme                = colorSchemeDefault;
            RememberLastStr            = rememberLastStrDefault;
            LastStrategy               = lastStrategyDefault;
            MultipleInstances          = multipleInstancesDefault;
            PlaySounds                 = playSoundsDefault;
            CheckForUpdates            = checkForUpdatesDefault;
            CheckForNewBeta            = checkForNewBetaDefault;
            LoadCustomIndicators       = loadCustIndDefault;
            ShowCustomIndicators       = showCustIndDefault;
            JournalShowSystemMessages  = journalShowSystemMsgDefault;
            JournalShowTicks           = journalShowTicksDefault;
            BridgeWritesLog            = bridgeWritesLogDefault;
            LastTab                    = lastTabDefault;
            UseLogicalGroups           = useLogicalGroupsDefault;
            JournalLength              = journalLengthDefault;
          
            // Indicator Chart
            ChartZoom                  = chartZoomDefault;
            ChartInfoPanel             = isChartInfoPanelDefault;
            ChartGrid                  = isChartGridDefault;
            ChartCross                 = isChartCrossDefault;
            ChartVolume                = isChartVolumeDefault;
            ChartLots                  = isChartLotsDefault;
            ChartOrders                = isChartOrdersDefault;
            ChartPositionPrice         = isChartPositionPriceDefault;
            ChartTrueCharts            = isChartTrueChartsDefault;
            ChartShift                 = isChartShiftDefault;
            ChartAutoScroll            = isChartAutoScrollDefault;

            SaveConfigs();
            isResetActivated = true;

            return;
        }

        /// <summary>
        /// Parses the config file
        /// </summary>
        static void ParseConfigs()
        {
            // Constants
            iMAX_ENTRY_FILTERS          = int.Parse(xmlConfig.SelectNodes("config/MAX_ENTRY_FILTERS").Item(0).InnerText);
            iMAX_EXIT_FILTERS           = int.Parse(xmlConfig.SelectNodes("config/MAX_EXIT_FILTERS").Item(0).InnerText);
            iSIGMA_MODE_MAIN_CHART      = int.Parse(xmlConfig.SelectNodes("config/SIGMA_MODE_MAIN_CHART").Item(0).InnerText);
            iSIGMA_MODE_SEPARATED_CHART = int.Parse(xmlConfig.SelectNodes("config/SIGMA_MODE_SEPARATED_CHART").Item(0).InnerText);

            // Trade settings
            songTradeLogicPrice        = xmlConfig.SelectNodes("config/trade/longTradeLogicPrice").Item(0).InnerText;
            barCloseAdvance            = int.Parse(xmlConfig.SelectNodes("config/trade/barCloseAdvance").Item(0).InnerText);
            autoSlippage               = bool.Parse(xmlConfig.SelectNodes("config/trade/autoSlippage").Item(0).InnerText);
            slippageEntryOrders        = int.Parse(xmlConfig.SelectNodes("config/trade/slippageEntryOrders").Item(0).InnerText);
            slippageExitOrders         = int.Parse(xmlConfig.SelectNodes("config/trade/slippageExitOrders").Item(0).InnerText);
            minChartBars               = int.Parse(xmlConfig.SelectNodes("config/trade/minChartBars").Item(0).InnerText);

            // Program settings
            isInstalled                 = bool.Parse(xmlConfig.SelectNodes("config/installed").Item(0).InnerText);
            language                   = xmlConfig.SelectNodes("config/language").Item(0).InnerText;
            showStartingTip            = bool.Parse(xmlConfig.SelectNodes("config/showStartingTip").Item(0).InnerText);
            currentTipNumber           = int.Parse(xmlConfig.SelectNodes("config/currentTipNumber").Item(0).InnerText);
            gradientView               = bool.Parse(xmlConfig.SelectNodes("config/gradientView").Item(0).InnerText);
            colorScheme                = xmlConfig.SelectNodes("config/colorScheme").Item(0).InnerText;
            playSounds                 = bool.Parse(xmlConfig.SelectNodes("config/playSounds").Item(0).InnerText);
            multipleInstances          = bool.Parse(xmlConfig.SelectNodes("config/multipleInstances").Item(0).InnerText);
            rememberLastStr            = bool.Parse(xmlConfig.SelectNodes("config/rememberLastStrategy").Item(0).InnerText);
            lastStrategy               = xmlConfig.SelectNodes("config/lastStrategy").Item(0).InnerText;
            checkForUpdates            = bool.Parse(xmlConfig.SelectNodes("config/checkForUpdates").Item(0).InnerText);
            checkForNewBeta            = bool.Parse(xmlConfig.SelectNodes("config/checkForNewBeta").Item(0).InnerText);
            loadCustomIndicators       = bool.Parse(xmlConfig.SelectNodes("config/loadCustomIndicators").Item(0).InnerText);
            showCustomIndicators       = bool.Parse(xmlConfig.SelectNodes("config/showCustomIndicators").Item(0).InnerText);
            bridgeWritesLog            = bool.Parse(xmlConfig.SelectNodes("config/bridgeWritesLog").Item(0).InnerText);
            lastTab                    = int.Parse(xmlConfig.SelectNodes("config/lastTab").Item(0).InnerText);
            useLogicalGroups           = bool.Parse(xmlConfig.SelectNodes("config/useLogicalGroups").Item(0).InnerText);
            journalLength               = int.Parse(xmlConfig.SelectNodes("config/journalLength").Item(0).InnerText);

            // Indicator Chart
            chartZoom                  = int.Parse(xmlConfig.SelectNodes("config/chart/zoom").Item(0).InnerText);
            isChartInfoPanel             = bool.Parse(xmlConfig.SelectNodes("config/chart/infoPanel").Item(0).InnerText);
            isChartGrid                  = bool.Parse(xmlConfig.SelectNodes("config/chart/grid").Item(0).InnerText);
            isChartCross                 = bool.Parse(xmlConfig.SelectNodes("config/chart/cross").Item(0).InnerText);
            isChartVolume                = bool.Parse(xmlConfig.SelectNodes("config/chart/volume").Item(0).InnerText);
            isChartLots                  = bool.Parse(xmlConfig.SelectNodes("config/chart/lots").Item(0).InnerText);
            isChartOrders                = bool.Parse(xmlConfig.SelectNodes("config/chart/orders").Item(0).InnerText);
            isChartPositionPrice         = bool.Parse(xmlConfig.SelectNodes("config/chart/positionPrice").Item(0).InnerText);
            isChartTrueCharts            = bool.Parse(xmlConfig.SelectNodes("config/chart/trueCharts").Item(0).InnerText);
            isChartShift                 = bool.Parse(xmlConfig.SelectNodes("config/chart/shift").Item(0).InnerText);
            isChartAutoScroll            = bool.Parse(xmlConfig.SelectNodes("config/chart/autoScroll").Item(0).InnerText);
            
            // Journal
            journalShowSystemMsg       = bool.Parse(xmlConfig.SelectNodes("config/journal/showSystemMessages").Item(0).InnerText);
            journalShowTicks           = bool.Parse(xmlConfig.SelectNodes("config/journal/showTicks").Item(0).InnerText);

            return;
        }

        /// <summary>
        /// Loads the config file
        /// </summary>
        public static void LoadConfigs()
        {
            try
            {
                xmlConfig.Load(pathToConfigFile);
                ParseConfigs();
                isConfigLoaded = true;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message, "Reading Configuration File");
            }

            return;
        }

        /// <summary>
        /// Saves the config file
        /// </summary>
        public static void SaveConfigs()
        {
            if (isResetActivated || !isConfigLoaded) return;

            try
            {
                xmlConfig.Save(pathToConfigFile);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message, "Saving Configuration File");
            }

            return;
        }
    }
}
