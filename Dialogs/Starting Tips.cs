// Starting_Tips Form
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Forex_Strategy_Trader
{
    class Starting_Tips : Form
    {
        Fancy_Panel pnlBase;
        Panel       pnlControl;
        WebBrowser  browser;
        CheckBox    chboxShow;
        Button      btnPrevTip;
        Button      btnNextTip;
        Button      btnClose;

        XmlDocument xmlTips;
        Random      rnd;
        int  iCurrentTip;
        int  iTipsCount = 0;
        bool bShowTips;

        string sHeader;
        string sCurrentTip;
        string sFooter;

        bool bShowAllTips = false;

        public bool ShowAllTips
        {
            set
            {
                bShowAllTips = value;
                browser.IsWebBrowserContextMenuEnabled = true;
                browser.WebBrowserShortcutsEnabled     = true;
            }
        }

        /// <summary>
        /// Public Constructor
        /// </summary>
        public Starting_Tips()
        {
            pnlBase    = new Fancy_Panel();
            pnlControl = new Panel();
            browser    = new WebBrowser();
            chboxShow  = new CheckBox();
            btnPrevTip = new Button();
            btnNextTip = new Button();
            btnClose   = new Button();

            xmlTips = new XmlDocument();
            rnd     = new Random();
            
            Text            = Language.T("Tip of the Day");
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon            = Data.Icon;
            MaximizeBox     = false;
            MinimizeBox     = false;
            TopMost         = true;

            pnlBase.Parent = this;

            browser.Parent              = pnlBase;
            browser.AllowNavigation     = true;
            browser.AllowWebBrowserDrop = false;
            browser.DocumentText        = Language.T("Loading...");
            browser.Dock                = DockStyle.Fill;
            browser.TabStop             = false;
            browser.DocumentCompleted  += new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            browser.IsWebBrowserContextMenuEnabled = false;
            browser.WebBrowserShortcutsEnabled = false;

            pnlControl.Parent    = this;
            pnlControl.Dock      = DockStyle.Bottom;
            pnlControl.BackColor = Color.Transparent;

            chboxShow.Parent    = pnlControl;
            chboxShow.Text      = Language.T("Show a tip");
            chboxShow.Checked   = Configs.ShowStartingTip;
            chboxShow.TextAlign = ContentAlignment.MiddleLeft;
            chboxShow.AutoSize  = true;
            chboxShow.ForeColor = LayoutColors.ColorControlText;
            chboxShow.CheckStateChanged += new EventHandler(ChboxShow_CheckStateChanged);

            btnPrevTip.Parent   = pnlControl;
            btnPrevTip.Text     = Language.T("Previous Tip");
            btnPrevTip.Name     = "Previous";
            btnPrevTip.Click   += new EventHandler(Navigate);
            btnPrevTip.UseVisualStyleBackColor = true;

            btnNextTip.Parent   = pnlControl;
            btnNextTip.Text     = Language.T("Next Tip");
            btnNextTip.Name     = "Next";
            btnNextTip.Click   += new EventHandler(Navigate);
            btnNextTip.UseVisualStyleBackColor = true;

            btnClose.Parent = pnlControl;
            btnClose.Text   = Language.T("Close");
            btnClose.Name   = "Close";
            btnClose.Click += new EventHandler(Navigate);
            btnClose.UseVisualStyleBackColor = true;

            LoadStartingTips();

            return;
        }

        /// <summary>
        /// On Load
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Width  = (int)(Data.HorizontalDLU * 240);
            Height = (int)(Data.VerticalDLU   * 140);

            return;
        }

        /// <summary>
        /// On Resize
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            int iButtonHeight = (int)(Data.VerticalDLU * 15.5);
            int iButtonWidth  = (int)(Data.HorizontalDLU * 60);
            int iBtnVertSpace = (int)(Data.VerticalDLU * 5.5 );
            int iBtnHrzSpace  = (int)(Data.HorizontalDLU * 3);
            int iBorder       = iBtnHrzSpace;

            pnlControl.Height = iButtonHeight + 2 * iBtnVertSpace;

            pnlBase.Size     = new Size(ClientSize.Width - 2 * iBorder, pnlControl.Top - iBorder);
            pnlBase.Location = new Point(iBorder, iBorder);

            chboxShow.Location = new Point(iBtnHrzSpace, iBtnVertSpace + 5);

            btnClose.Size     = new Size(iButtonWidth, iButtonHeight);
            btnClose.Location = new Point(ClientSize.Width - btnNextTip.Width - iBtnHrzSpace, iBtnVertSpace);

            btnNextTip.Size     = new Size(iButtonWidth, iButtonHeight);
            btnNextTip.Location = new Point(btnClose.Left - btnNextTip.Width - iBtnHrzSpace, iBtnVertSpace);

            btnPrevTip.Size     = new Size(iButtonWidth, iButtonHeight);
            btnPrevTip.Location = new Point(btnNextTip.Left - btnPrevTip.Width - iBtnHrzSpace, iBtnVertSpace);

            // Resize if necessary
            if (btnPrevTip.Left - chboxShow.Right < iBtnVertSpace)
                Width += iBtnVertSpace - btnPrevTip.Left + chboxShow.Right;
            
            return;
        }
 
        /// <summary>
        /// Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);

            return;
        }

        /// <summary>
        /// The Document is ready
        /// </summary>
        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            browser.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(Browser_DocumentCompleted);
            iCurrentTip--;
            ShowTip(true);

            return;
        }

        /// <summary>
        /// Change starting options
        /// </summary>
        void ChboxShow_CheckStateChanged(object sender, EventArgs e)
        {
            bShowTips = chboxShow.Checked;
            Configs.ShowStartingTip = bShowTips;
            Configs.SaveConfigs();

            return;
        }

        /// <summary>
        /// Navigate
        /// </summary>
        void Navigate(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            if (btn.Name == "Previous")
                ShowTip(false);
            else if (btn.Name == "Next")
                ShowTip(true);
            else if (btn.Name == "Close")
                Close();

            return;
        }

        /// <summary>
        /// Show random tip
        /// </summary>
        void ShowTip(bool bNextTip)
        {
            if (iTipsCount == 0)
                return;

            if (bNextTip)
            {
                if (iCurrentTip < iTipsCount - 1)
                    iCurrentTip++;
                else
                    iCurrentTip = 0;
            }
            else
            {
                if (iCurrentTip > 0)
                    iCurrentTip--;
                else
                    iCurrentTip = iTipsCount - 1;
            }

            if (bShowAllTips)
            {
                StringBuilder sbTips = new StringBuilder(iTipsCount);

                foreach (XmlNode node in xmlTips.SelectNodes("tips/tip"))
                    sbTips.AppendLine(node.InnerXml);

                browser.DocumentText = sHeader + sbTips.ToString() + sFooter;

            }
            else
            {
                sCurrentTip = xmlTips.SelectNodes("tips/tip").Item(iCurrentTip).InnerXml;

                browser.DocumentText = sHeader.Replace("###", (iCurrentTip + 1).ToString()) + sCurrentTip + sFooter;

                Configs.CurrentTipNumber = iCurrentTip;
            }

            return;
        }

        /// <summary>
        /// Load tips config file
        /// </summary>
        void LoadStartingTips()
        {
            // Header
            sHeader  = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">";
            sHeader += "<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\">";
            sHeader += "<head><meta http-equiv=\"content-type\" content=\"text/html; charset=UTF-8\" />";
            sHeader += "<title>Tip of the Day</title><style>";
            sHeader += "body {margin: 0px; font-size: 14px; background-color: #fffffd}";
            sHeader += ".number {font-size: 9px}";
            sHeader += ".content {padding: 0 5px 5px 5px;}";
            sHeader += ".content h1 {margin: 0; font-weight: bold; font-size: 14px; color: #000033; text-align: center;}";
            sHeader += ".content p {margin-top: 0.5em; margin-bottom: 2px; color: #000033; text-indent: 1em;}";
            sHeader += "</style></head>";
            sHeader += "<body>";
            sHeader += "<div class=\"content\">";
            sHeader += "<div class=\"number\">(###)</div>";

            // Footer
            sFooter = "</div></body></html>";

            iCurrentTip = Configs.CurrentTipNumber + 1;

            if (bShowAllTips) iCurrentTip = 0;

            string sStartingTipsDir = Data.SystemDir + @"StartingTips";

            if (Directory.Exists(sStartingTipsDir) && Directory.GetFiles(sStartingTipsDir).Length > 0)
            {
                string[] asLangFiles = Directory.GetFiles(sStartingTipsDir);

                foreach (string sLangFile in asLangFiles)
                {
                    if (sLangFile.EndsWith(".xml", true, null))
                    {
                        try
                        {
                            XmlDocument xmlLanguage = new XmlDocument();
                            xmlLanguage.Load(sLangFile);
                            XmlNode node = xmlLanguage.SelectSingleNode("tips//language");

                            if (node == null)
                            {   // There is no language specified int the lang file
                                string sMessageText = "Starting tip file: " + sLangFile + Environment.NewLine + Environment.NewLine + "The language is not specified!";
                                MessageBox.Show(sMessageText, "Tips of the Day File Loading", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                            else if (node.InnerText == Configs.Language)
                            {   // It looks OK
                                xmlTips.Load(sLangFile);
                                iTipsCount = xmlTips.SelectNodes("tips/tip").Count;
                            }
                        }
                        catch (Exception e)
                        {
                            string sMessageText = "Starting tip file: " + sLangFile + Environment.NewLine + Environment.NewLine +
                                "Error in the starting tip file!" + Environment.NewLine + Environment.NewLine + e.Message;
                            MessageBox.Show(sMessageText, "Tips of the Day File Loading", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                }
            }

            if (Configs.Language != "English" && iTipsCount == 0)
            {
                try
                {   // The tips file
                    xmlTips.Load(Data.SystemDir + @"StartingTips\English.xml");
                    iTipsCount = xmlTips.SelectNodes("tips/tip").Count;
                }
                catch (Exception e)
                {
                    string sMessageText = "Starting tip file \"English.xml\"" + Environment.NewLine + Environment.NewLine +
                        "Error in the starting tip file!" + Environment.NewLine + Environment.NewLine + e.Message;
                    MessageBox.Show(sMessageText, "Tips of the Day File Loading", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

            return;
        }
    }
}
