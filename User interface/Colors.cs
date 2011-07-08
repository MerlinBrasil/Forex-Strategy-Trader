// Colors Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Structure Colors
    /// </summary>
    public static class LayoutColors
    {
        static string[] asColorList; // List of the languages
        static Dictionary<String, String> dictColorFiles; // Language files <Language><FileName>

        /// <summary>
        /// Gets the language files list.
        /// </summary>
        public static string[] ColorSchemeList { get { return asColorList; } }

        static XmlDocument xmlColors;

        static int iGradientCaptionDepth;
        static int iGradientControlDepth;

        // Workspace
        static Color clrFormBack;
        static Color clrControlBack;
        static Color clrControlText;
        static Color clrCaptionBack;
        static Color clrCaptionText;
        static Color clrEvenRowBack;
        static Color clrSignalRed;

        // Slots
        static Color clrSlotCaptionBackAveraging;
        static Color clrSlotCaptionBackOpen;
        static Color clrSlotCaptionBackOpenFilter;
        static Color clrSlotCaptionBackClose;
        static Color clrSlotCaptionBackCloseFilter;
        static Color clrSlotCaptionText;
        static Color clrSlotBack;
        static Color clrSlotIndicatorText;
        static Color clrSlotLogicText;
        static Color clrSlotParamText;
        static Color clrSlotValueText;
        static Color clrSlotDash;

        // Charts
        static Color clrChartBack;
        static Color clrChartFore;
        static Color clrChartGrid;
        static Color clrChartCross;
        static Color clrLabelBack;
        static Color clrLabelText;
        static Color clrTradeLong;
        static Color clrTradeShort;
        static Color clrTradeClose;
        static Color clrVolume;
        static Color clrBarWhite;
        static Color clrBarBlack;
        static Color clrBarBorder;
        static Color clrChartBalanceLine;
        static Color clrChartEquityLine;

        /// <summary>
        /// Gradient color depth of the caption bars
        /// </summary>
        public static int DepthCaption
        {
            get { return iGradientCaptionDepth; }
            set { iGradientCaptionDepth = value; }
        }

        /// <summary>
        /// Gradient color depth of the controls
        /// </summary>
        public static int DepthControl
        {
            get { return iGradientControlDepth; }
            set { iGradientControlDepth = value; }
        }

        // Workspace
        public static Color ColorFormBack
        {
            get { return clrFormBack; }
            set { clrFormBack = value; }
        }

        public static Color ColorControlBack
        {
            get { return clrControlBack; }
            set { clrControlBack = value; }
        }

        public static Color ColorControlText
        {
            get { return clrControlText; }
            set { clrControlText = value; }
        }

        public static Color ColorCaptionBack
        {
            get { return clrCaptionBack; }
            set { clrCaptionBack = value; }
        }

        public static Color ColorCaptionText
        {
            get { return clrCaptionText; }
            set { clrCaptionText = value; }
        }

        public static Color ColorEvenRowBack
        {
            get { return clrEvenRowBack; }
            set { clrEvenRowBack = value; }
        }

        public static Color ColorOddRowBack
        {
            get { return clrControlBack; }
        }

        public static Color ColorSignalRed
        {
            get { return clrSignalRed; }
            set { clrSignalRed = value; }
        }

        // Slots
        public static Color ColorSlotCaptionBackAveraging
        {
            get { return clrSlotCaptionBackAveraging; }
            set { clrSlotCaptionBackAveraging = value; }
        }

        public static Color ColorSlotCaptionBackOpen
        {
            get { return clrSlotCaptionBackOpen; }
            set { clrSlotCaptionBackOpen = value; }
        }

        public static Color ColorSlotCaptionBackOpenFilter
        {
            get { return clrSlotCaptionBackOpenFilter; }
            set { clrSlotCaptionBackOpenFilter = value; }
        }

        public static Color ColorSlotCaptionBackClose
        {
            get { return clrSlotCaptionBackClose; }
            set { clrSlotCaptionBackClose = value; }
        }

        public static Color ColorSlotCaptionBackCloseFilter
        {
            get { return clrSlotCaptionBackCloseFilter; }
            set { clrSlotCaptionBackCloseFilter = value; }
        }

        public static Color ColorSlotCaptionText
        {
            get { return clrSlotCaptionText; }
            set { clrSlotCaptionText = value; }
        }

        public static Color ColorSlotBackground
        {
            get { return clrSlotBack; }
            set { clrSlotBack = value; }
        }

        public static Color ColorSlotIndicatorText
        {
            get { return clrSlotIndicatorText; }
            set { clrSlotIndicatorText = value; }
        }

        public static Color ColorSlotLogicText
        {
            get { return clrSlotLogicText; }
            set { clrSlotLogicText = value; }
        }

        public static Color ColorSlotParamText
        {
            get { return clrSlotParamText; }
            set { clrSlotParamText = value; }
        }

        public static Color ColorSlotValueText
        {
            get { return clrSlotValueText; }
            set { clrSlotValueText = value; }
        }

        public static Color ColorSlotDash
        {
            get { return clrSlotDash; }
            set { clrSlotDash = value; }
        }

        // Charts
        public static Color ColorChartBack
        {
            get { return clrChartBack; }
            set { clrChartBack = value; }
        }

        public static Color ColorChartFore
        {
            get { return clrChartFore; }
            set { clrChartFore = value; }
        }

        public static Color ColorChartGrid
        {
            get { return clrChartGrid; }
            set { clrChartGrid = value; }
        }

        public static Color ColorChartCross
        {
            get { return clrChartCross; }
            set { clrChartCross = value; }
        }

        public static Color ColorLabelBack
        {
            get { return clrLabelBack; }
            set { clrLabelBack = value; }
        }

        public static Color ColorLabelText
        {
            get { return clrLabelText; }
            set { clrLabelText = value; }
        }

        public static Color ColorTradeLong
        {
            get { return clrTradeLong; }
            set { clrTradeLong = value; }
        }
        public static Color ColorTradeShort
        {
            get { return clrTradeShort; }
            set { clrTradeShort = value; }
        }

        public static Color ColorTradeClose
        {
            get { return clrTradeClose; }
            set { clrTradeClose = value; }
        }

        public static Color ColorVolume
        {
            get { return clrVolume; }
            set { clrVolume = value; }
        }

        public static Color ColorBarWhite
        {
            get { return clrBarWhite; }
            set { clrBarWhite = value; }
        }

        public static Color ColorBarBlack
        {
            get { return clrBarBlack; }
            set { clrBarBlack = value; }
        }

        public static Color ColorBarBorder
        {
            get { return clrBarBorder; }
            set { clrBarBorder = value; }
        }

        public static Color ColorChartBalanceLine
        {
            get { return clrChartBalanceLine; }
            set { clrChartBalanceLine = value; }
        }

        public static Color ColorChartEquityLine
        {
            get { return clrChartEquityLine; }
            set { clrChartEquityLine = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        static LayoutColors()
        {
            SetColorsDefault();
        }

        /// <summary>
        /// Sets the default color scheme.
        /// </summary>
        static void SetColorsDefault()
        {
            iGradientCaptionDepth = 25;
            iGradientControlDepth = 10;

            // Workspace
            clrFormBack        = Color.FromArgb(235, 245, 245);
            clrControlBack     = Color.FromArgb(245, 255, 255);
            clrControlText     = Color.FromArgb(  0,  50,  50);
            clrCaptionBack     = Color.FromArgb(102, 153, 204);
            clrCaptionText     = Color.FromArgb(255, 255, 255);
            clrEvenRowBack     = Color.FromArgb(255, 255, 255);
            clrSignalRed       = Color.FromArgb(255,   0,   0);

            // Slots
            clrSlotCaptionBackAveraging   = Color.FromArgb(150, 100, 100);
            clrSlotCaptionBackOpen        = Color.FromArgb(102, 153,  51);
            clrSlotCaptionBackOpenFilter  = Color.FromArgb(102, 153, 153);
            clrSlotCaptionBackClose       = Color.FromArgb(204, 102,  51);
            clrSlotCaptionBackCloseFilter = Color.FromArgb(210, 140, 140);
            clrSlotCaptionText            = Color.FromArgb(255, 255, 255);
            clrSlotBack                   = Color.FromArgb(245, 255, 255);
            clrSlotIndicatorText          = Color.FromArgb( 80, 130, 180);
            clrSlotLogicText              = Color.FromArgb(  0,  51,  51);
            clrSlotParamText              = Color.FromArgb( 51, 153, 153);
            clrSlotValueText              = Color.FromArgb( 51, 153, 153);
            clrSlotDash                   = Color.FromArgb(204, 204, 153);

            // Chart
            clrChartBack        = Color.FromArgb(245, 255, 255);
            clrChartFore        = Color.FromArgb(  0,  50,  50);
            clrBarWhite         = Color.FromArgb(225, 225, 225);
            clrBarBlack         = Color.FromArgb( 30,  30,  30);
            clrBarBorder        = Color.FromArgb(  0,   0,   0);
            clrTradeLong        = Color.FromArgb( 30, 160,  30);
            clrTradeShort       = Color.FromArgb(225,  30,  30);
            clrTradeClose       = Color.FromArgb(225, 160,  30);
            clrVolume           = Color.FromArgb(150,   0, 210);
            clrLabelBack        = Color.FromArgb(102, 102, 153);
            clrLabelText        = Color.FromArgb(255, 255, 255);
            clrChartGrid        = Color.FromArgb(204, 204, 204);
            clrChartCross       = Color.FromArgb(153, 163, 204);
            clrChartBalanceLine = Color.FromArgb(102, 102, 153);
            clrChartEquityLine  = Color.FromArgb(225, 204,  51);

            return;
        }

        /// <summary>
        /// Loads the color scheme from a file
        /// </summary>
        public static void LoadColorScheme(string sColorScheme)
        {
            try
            {
                xmlColors = new XmlDocument();
                xmlColors.Load(sColorScheme);

                // Workspace
                clrFormBack        = ParseColor("FormBack");
                clrControlBack     = ParseColor("ControlBack");
                clrControlText     = ParseColor("ControlText");
                clrCaptionBack     = ParseColor("CaptionBack");
                clrCaptionText     = ParseColor("CaptionText");
                clrEvenRowBack     = ParseColor("EvenRowBack");
                clrSignalRed       = ParseColor("SignalRed");

                // Strategy Slots
                clrSlotCaptionBackAveraging   = ParseColor("SlotCaptionBackProperties");
                clrSlotCaptionBackOpen        = ParseColor("SlotCaptionBackOpen");
                clrSlotCaptionBackOpenFilter  = ParseColor("SlotCaptionBackOpenFilter");
                clrSlotCaptionBackClose       = ParseColor("SlotCaptionBackClose");
                clrSlotCaptionBackCloseFilter = ParseColor("SlotCaptionBackCloseFilter");
                clrSlotCaptionText            = ParseColor("SlotCaptionText");
                clrSlotBack                   = ParseColor("SlotBack");
                clrSlotIndicatorText          = ParseColor("SlotIndicatorText");
                clrSlotLogicText              = ParseColor("SlotLogicText");
                clrSlotParamText              = ParseColor("SlotParamText");
                clrSlotValueText              = ParseColor("SlotValueText");
                clrSlotDash                   = ParseColor("SlotDash");

                // Chart
                clrChartBack        = ParseColor("ChartBack");
                clrChartFore        = ParseColor("ChartFore");
                clrBarWhite         = ParseColor("BarWhite");
                clrBarBlack         = ParseColor("BarBlack");
                clrBarBorder        = ParseColor("BarBorder");
                clrTradeLong        = ParseColor("TradeLong");
                clrTradeShort       = ParseColor("TradeShort");
                clrTradeClose       = ParseColor("TradeClose");
                clrVolume           = ParseColor("Volume");
                clrChartGrid        = ParseColor("ChartGrid");
                clrChartCross       = ParseColor("ChartCross");
                clrLabelBack        = ParseColor("LabelBack");
                clrLabelText        = ParseColor("LabelText");
                clrChartBalanceLine = ParseColor("ChartBalanceLine");
                clrChartEquityLine  = ParseColor("ChartEquityLine");
            }
            catch (Exception exc)
            {
                System.Windows.Forms.MessageBox.Show(exc.Message, Language.T("Load a color scheme..."));
            }

            return;
        }

        /// <summary>
        /// Parses the color from the given xml node
        /// </summary>
        static Color ParseColor(string sNode)
        {
            int r = int.Parse(LayoutColors.xmlColors.SelectNodes("color/" + sNode).Item(0).Attributes.Item(0).InnerText);
            int g = int.Parse(LayoutColors.xmlColors.SelectNodes("color/" + sNode).Item(0).Attributes.Item(1).InnerText);
            int b = int.Parse(LayoutColors.xmlColors.SelectNodes("color/" + sNode).Item(0).Attributes.Item(2).InnerText);

            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Inits the languages.
        /// </summary>
        public static void InitColorSchemes()
        {
            dictColorFiles = new Dictionary<string, string>();
            string sColorDirectory = Data.ColorDir;

            if (Directory.Exists(sColorDirectory) && Directory.GetFiles(sColorDirectory).Length > 0)
            {
                string[] asColorFiles = Directory.GetFiles(sColorDirectory);

                foreach (string sLangFile in asColorFiles)
                {
                    if (sLangFile.EndsWith(".xml", true, null))
                    {
                        try
                        {
                            string sColorScheme = Path.GetFileNameWithoutExtension(sLangFile);
                            dictColorFiles.Add(sColorScheme, sLangFile);

                            if (sColorScheme == Configs.ColorScheme)
                            {
                                LoadColorScheme(sLangFile);
                            }
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message, "Color Scheme", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                }
            }

            asColorList = new string[dictColorFiles.Count];
            dictColorFiles.Keys.CopyTo(asColorList, 0);
            System.Array.Sort(asColorList);
        }
    }
}
