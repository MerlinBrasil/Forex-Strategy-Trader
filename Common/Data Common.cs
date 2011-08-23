// Data class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Media;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Data Periods.
    /// The value of each period is equal to its duration in minutes.
    /// </summary>
    public enum DataPeriods
    {
        min1  = 1,
        min5  = 5,
        min15 = 15,
        min30 = 30,
        hour1 = 60,
        hour4 = 240,
        day   = 1440,
        week  = 10080
    }

    // Size of the Slot panels
    public enum SlotSizeMinMidMax { min, mid, max };

    /// <summary>
    ///  Base class containing the data.
    /// </summary>
    public static partial class Data
    {
        static bool   isBetaVersion = false; // Is the program Beta

        static string programVersion; // Program version
        static int    programID;      // Program version
        static string programName          = "Forex Strategy Trader"; // Program title
        static string systemDir            = @"System\";     // System dir
        static string languageDir          = @"Languages\";  // Contains the language files
        static string colorDir             = @"Colors\";     // Contains the color scheme files
        static string strategyDir          = @"Strategies\"; // Strategy dir
        static string defaultStrategyDir   = @"Strategies\"; // Strategy dir
        static string sourceFolder         = @"Custom Indicators\"; // Contains the sources of custom indicators
        static string programDir           = @"";            // Program dir
        static string strategyName         = "New.xml";      // Starting strategy name
        static string loadedSavedStrategy  = "";             // The strategy for Configs.LastStrategy
        static string logFileName          = "Logfile.txt";  // Logfile name
        static string dateFormat           = "dd.MM.yy";     // Date Format
        static string shortDateFormat      = "dd.MM";        // Date Format Short
        static char   decimalPoint         = '.';            // Point character
        static int    firstBar             = 40;    // First bar that can be calculated
        static bool   isAutoUsePrvBarValue = true;  // Auto adjust use previous bar value
        static bool   isStrategyChanged    = false; // If the strategy has been changed
        static bool   isStrategyReady      = true;  // If construction is done
        static bool   isToLog              = false; // Log to logfile
        static bool   isDebug              = false; // Debug Mode

        static string[] indicatorsForBacktestOnly = new string[] {
                "Random Filter",
                "Date Filter",
                "Data Bars Filter",
                "Lot Limiter"
            };

        static SoundPlayer soundConnect;
        static SoundPlayer soundDisconnect;
        static SoundPlayer soundError;
        static SoundPlayer soundPositionChanged;
        static SoundPlayer soundOrderSent;

        static StreamWriter swLogFile = StreamWriter.Null; // For the Log file

        static float horizontalDLU;
        static float verticalDLU;

        // The current strategy.
        static Strategy strategy;

        // Strategy Undo
        static Stack<Strategy> stckStrategy;

        /// <summary>
        /// Gets the program name.
        /// </summary>
        public static string ProgramName { get { return programName; } }

        /// <summary>
        /// Gets the program version.
        /// </summary>
        public static string ProgramVersion { get { return programVersion; } }

        /// <summary>
        /// Gets the program Beta state.
        /// </summary>
        public static bool IsProgramBeta { get { return isBetaVersion; } }

        /// <summary>
        /// Gets the program ID
        /// </summary>
        public static int ProgramID { get { return programID; } }

        /// <summary>
        /// Gets the program current working directory.
        /// </summary>
        public static string ProgramDir { get { return programDir; } }

        /// <summary>
        /// Gets the path to System Dir.
        /// </summary>
        public static string SystemDir { get { return systemDir; } }

        /// <summary>
        /// Gets the path to LanguageDir Dir.
        /// </summary>
        public static string LanguageDir { get { return languageDir; } }

        /// <summary>
        /// Gets the path to Color Scheme Dir.
        /// </summary>
        public static string ColorDir { get { return colorDir; } }

        /// <summary>
        /// Gets the path to Default Strategy Dir.
        /// </summary>
        public static string DefaultStrategyDir { get { return defaultStrategyDir; } }

        /// <summary>
        /// Gets or sets the path to dir Strategy.
        /// </summary>
        public static string StrategyDir { get { return strategyDir; } set { strategyDir = value; } }

        /// <summary>
        /// Gets or sets the strategy name with extension.
        /// </summary>
        public static string StrategyName { get { return strategyName; } set { strategyName = value; } }

        /// <summary>
        /// Gets the current strategy full path. 
        /// </summary>
        public static string StrategyPath { get { return Path.Combine(strategyDir, strategyName); } }

        public static bool IsStrategyReady { get { return isStrategyReady; } set { isStrategyReady = value; } }
        public static bool IsStrategyChanged { get { return isStrategyChanged; } set { isStrategyChanged = value; } }
        public static int FirstBar { get { return firstBar; } set { firstBar = value; } }

        /// <summary>
        /// Gets or sets the custom indicators folder
        /// </summary>
        public static string SourceFolder { get { return sourceFolder; } }

        /// <summary>
        /// Gets or sets the strategy name for Configs.LastStrategy
        /// </summary>
        public static string LoadedSavedStrategy { get { return loadedSavedStrategy; } set { loadedSavedStrategy = value; } }

        /// <summary>
        /// Gets the application's icon.
        /// </summary>
        public static Icon Icon { get { return Properties.Resources.Icon; } }

        /// <summary>
        /// The current strategy.
        /// </summary>
        public static Strategy Strategy { get { return strategy; } set { strategy = value; } }

        /// <summary>
        /// The current strategy undo
        /// </summary>
        public static Stack<Strategy> StackStrategy { get { return stckStrategy; } set { stckStrategy = value; } }

        /// <for>
        /// Whether to log
        /// </summary>
        public static bool ToLog { get { return isToLog; } set { isToLog = value; } }

        /// <for>
        /// Debug mode
        /// </summary>
        public static bool Debug { get { return isDebug; } set { isDebug = value; } }

        /// <summary>
        /// Sets or gets value of the AutoUsePrvBarValue
        /// </summary>
        public static bool AutoUsePrvBarValue { get { return isAutoUsePrvBarValue; } set { isAutoUsePrvBarValue = value; } }

        /// <summary>
        /// Gets the number format.
        /// </summary>
        public static string FF { get { return "F" + instrProperties.Digits.ToString(); } }

        /// <summary>
        /// Gets the date format.
        /// </summary>
        public static string DF { get { return dateFormat; } }

        /// <summary>
        /// Gets the short date format.
        /// </summary>
        public static string DFS { get { return shortDateFormat; } }

        /// <summary>
        /// Gets the point character
        /// </summary>
        public static char PointChar { get { return decimalPoint; } }

        /// <summary>
        /// Relative font height
        /// </summary>
        public static float VerticalDLU { get { return verticalDLU; } set { verticalDLU = value; } }

        /// <summary>
        /// Relative font width
        /// </summary>
        public static float HorizontalDLU { get { return horizontalDLU; } set { horizontalDLU = value; } }

        /// <summary>
        /// Gets connect sound
        /// </summary>
        public static SoundPlayer SoundConnect { get { return soundConnect; } }

        /// <summary>
        /// Gets disconnect sound
        /// </summary>
        public static SoundPlayer SoundDisconnect { get { return soundDisconnect; } }

        /// <summary>
        /// Gets error sound
        /// </summary>
        public static SoundPlayer SoundError { get { return soundError; } }

        /// <summary>
        /// Gets order sent sound
        /// </summary>
        public static SoundPlayer SoundOrderSent { get { return soundOrderSent; } }

        /// <summary>
        /// Gets position changed sound
        /// </summary>
        public static SoundPlayer SoundPositionChanged { get { return soundPositionChanged; } }

        /// <summary>
        /// Gets indicators that are for back testing only.
        /// </summary>
        public static string[] IndicatorsForBacktestOnly { get { return indicatorsForBacktestOnly; } }

        static bool bIsData = false;
        public static bool IsData { get { return bIsData; } set { bIsData = value; } }
        public static string PeriodStr { get { return DataPeriodToString(period); } }
        public static string PeriodMTStr { get { return ((MT4Bridge.PeriodType)(int)period).ToString(); } }

        /// <summary>
        /// The default constructor.
        /// </summary>
        static Data()
        {
            programName = "Forex Strategy Trader";
            programVersion = Application.ProductVersion;
            string[] asVersion = programVersion.Split('.');
            programID = 1000000 * int.Parse(asVersion[0]) + 10000 * int.Parse(asVersion[1]) +
                100 * int.Parse(asVersion[2]) + int.Parse(asVersion[3]);
            Strategy.GenerateNew();
            stckStrategy = new Stack<Strategy>();

            return;
        }

        /// <summary>
        /// Initial settings.
        /// </summary>
        public static void Start()
        {
            // Sets the date format.
            dateFormat = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            if (dateFormat == "dd/MM yyyy") dateFormat = "dd/MM/yyyy"; // Fixes the Uzbek (Latin) issue
            dateFormat = dateFormat.Replace(" ", ""); // Fixes the Sloven issue
            char[]   acDS = DateTimeFormatInfo.CurrentInfo.DateSeparator.ToCharArray();
            string[] asSS = dateFormat.Split(acDS, 3);
            asSS[0] = asSS[0].Substring(0, 1) + asSS[0].Substring(0, 1);
            asSS[1] = asSS[1].Substring(0, 1) + asSS[1].Substring(0, 1);
            asSS[2] = asSS[2].Substring(0, 1) + asSS[2].Substring(0, 1);
            dateFormat = asSS[0] + acDS[0].ToString() + asSS[1] + acDS[0].ToString() + asSS[2];

            if (asSS[0].ToUpper() == "YY")
                shortDateFormat = asSS[1] + acDS[0].ToString() + asSS[2];
            else if (asSS[1].ToUpper() == "YY")
                shortDateFormat = asSS[0] + acDS[0].ToString() + asSS[2];
            else
                shortDateFormat = asSS[0] + acDS[0].ToString() + asSS[1];

            // Point character
            CultureInfo culInf = CultureInfo.CurrentCulture;
            decimalPoint = culInf.NumberFormat.NumberDecimalSeparator.ToCharArray()[0];

            // Set the working directories
            programDir   = Directory.GetCurrentDirectory();
            strategyDir  = Path.Combine(programDir, defaultStrategyDir);
            sourceFolder = Path.Combine(programDir, sourceFolder);
            systemDir    = Path.Combine(programDir, systemDir);
            languageDir  = Path.Combine(systemDir,  languageDir);
            colorDir     = Path.Combine(systemDir,  colorDir);
            logFileName  = Path.Combine(programDir, logFileName);

            try
            {
                soundConnect         = new SoundPlayer(Path.Combine(systemDir, @"Sounds\connect.wav"));
                soundDisconnect      = new SoundPlayer(Path.Combine(systemDir, @"Sounds\disconnect.wav"));
                soundError           = new SoundPlayer(Path.Combine(systemDir, @"Sounds\error.wav"));
                soundOrderSent       = new SoundPlayer(Path.Combine(systemDir, @"Sounds\order_sent.wav"));
                soundPositionChanged = new SoundPlayer(Path.Combine(systemDir, @"Sounds\position_changed.wav"));
            }
            catch
            {
                soundConnect         = new SoundPlayer(Properties.Resources.sound_connect);
                soundDisconnect      = new SoundPlayer(Properties.Resources.sound_disconnect);
                soundError           = new SoundPlayer(Properties.Resources.sound_error);
                soundOrderSent       = new SoundPlayer(Properties.Resources.sound_order_sent);
                soundPositionChanged = new SoundPlayer(Properties.Resources.sound_position_changed);
            }

            // Create a new Logfile
            if (isToLog)
            {
                swLogFile = new StreamWriter(logFileName, false);
            }

            return;
        }

        // The names of the strategy indicators
        static string[] asStrategyIndicators;

        /// <summary>
        /// Sets the indicator names
        /// </summary>
        public static void SetStrategyIndicators()
        {
            asStrategyIndicators = new string[Strategy.Slots];
            for (int i = 0; i < Strategy.Slots; i++)
                asStrategyIndicators[i] = Strategy.Slot[i].IndicatorName;
        }

        /// <summary>
        /// It tells if the strategy description is relevant.
        /// </summary>
        public static bool IsStrDescriptionRelevant()
        {
            bool bStrategyIndicatorsChanged = false; // If the strategy indicators have been changed

            if (Strategy.Slots != asStrategyIndicators.Length)
                bStrategyIndicatorsChanged = true;

            if (bStrategyIndicatorsChanged == false)
            {
                for (int i = 0; i < Strategy.Slots; i++)
                    if (asStrategyIndicators[i] != Strategy.Slot[i].IndicatorName)
                        bStrategyIndicatorsChanged = true;
            }

            return !bStrategyIndicatorsChanged;
        }

        /// <summary>
        /// Sets the time when trading starts.
        /// </summary>
        public static void SetStartTradingTime()
        {
            if (IsDemoAccount)
                DemoTradeStartTime = DateTime.Now;
            else
                LiveTradeStartTime = DateTime.Now;
        }

        /// <summary>
        /// Sets the total trading time stats.
        /// </summary>
        public static void SetStopTradingTime()
        {
            if (IsDemoAccount && DemoTradeStartTime > DateTime.MinValue)
                SecondsDemoTrading += (int)(DateTime.Now - DemoTradeStartTime).TotalSeconds;
            else if (LiveTradeStartTime > DateTime.MinValue)
                SecondsLiveTrading += (int)(DateTime.Now - LiveTradeStartTime).TotalSeconds;

            DemoTradeStartTime = DateTime.MinValue;
            LiveTradeStartTime = DateTime.MinValue;
        }

        /// <summary>
        /// Collects usage statistics and sends them if it's allowed.
        /// </summary>
        public static void SendStats()
        {
            string fileURL = "http://forexsb.com/ustats/set-fst.php";

            string mac = "";
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    mac = nic.GetPhysicalAddress().ToString();
                    break;
                }
            }

            string parameters = String.Empty;

            if (Configs.SendUsageStats)
            {
                parameters =
                   "?mac="  + mac +
                   "&reg="  + RegionInfo.CurrentRegion.EnglishName +
                   "&time=" + (int)(DateTime.Now - fstStartTime).TotalSeconds +
                   "&dtt="  + secondsDemoTrading +
                   "&ltt="  + secondsLiveTrading +
                   "&str="  + savedStrategies;
            }

            try
            {
                WebClient webClient = new WebClient();
                Stream data = webClient.OpenRead(fileURL + parameters);
                data.Close();
            }
            catch { }
        }


        /// <summary>
        /// Saves a text line in log file.
        /// </summary>
        /// <param name="sLogLine">The text line.</param>
        public static void Log(string sLogLine)
        {
            if (isToLog == true)
            {
                swLogFile.WriteLine(DateTime.Now.ToString() + "   " + sLogLine);
                swLogFile.Flush();
            }

            return;
        }

        /// <summary>
        /// Closes the log file.
        /// </summary>
        public static void CloseLogFile()
        {
            // Closes the logfile
            if (isToLog == true)
            {
                swLogFile.Flush();
                swLogFile.Close();
                isToLog = false;
            }
        }

        /// <summary>
        /// Converts a data period from DataPeriods type to string.
        /// </summary>
        public static string DataPeriodToString(DataPeriods dataPeriod)
        {
            switch (dataPeriod)
            {
                case DataPeriods.min1:  return "1 "  + Language.T("Minute");
                case DataPeriods.min5:  return "5 "  + Language.T("Minutes");
                case DataPeriods.min15: return "15 " + Language.T("Minutes");
                case DataPeriods.min30: return "30 " + Language.T("Minutes");
                case DataPeriods.hour1: return "1 "  + Language.T("Hour");
                case DataPeriods.hour4: return "4 "  + Language.T("Hours");
                case DataPeriods.day:   return "1 "  + Language.T("Day");
                case DataPeriods.week:  return "1 "  + Language.T("Week");
                default: return String.Empty;
            }
        }

        /// <summary>
        /// Color change
        /// </summary>
        /// <param name="colorBase">The base color</param>
        /// <param name="iDepth">Color change</param>
        /// <returns>The changed color</returns>
        public static Color ColorChanage(Color colorBase, int iDepth)
        {
            if (!Configs.GradientView)
                return colorBase;

            int r = Math.Max(Math.Min(colorBase.R + iDepth, 255), 0);
            int g = Math.Max(Math.Min(colorBase.G + iDepth, 255), 0);
            int b = Math.Max(Math.Min(colorBase.B + iDepth, 255), 0);

            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Paints a rectangle with gradient.
        /// </summary>
        public static void GradientPaint(Graphics g, RectangleF rect, Color color, int depth)
        {
            if (rect.Width <= 0 || rect.Height <= 0)
                return;

            if (depth > 0 && Configs.GradientView)
            {
                Color color1 = ColorChanage(color, +depth);
                Color color2 = ColorChanage(color, -depth);
                RectangleF rect1 = new RectangleF(rect.X, rect.Y - 1, rect.Width, rect.Height + 2);
                LinearGradientBrush lgrdBrush = new LinearGradientBrush(rect1, color1, color2, 90);
                g.FillRectangle(lgrdBrush, rect);
            }
            else
            {
                g.FillRectangle(new SolidBrush(color), rect);
            }

            return;
        }
    }
}
