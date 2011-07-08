// Fancy_Message_Box Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Trader
{
    class Fancy_Message_Box : Form
    {
        Fancy_Panel pnlBase;
        Panel       pnlControl;
        WebBrowser  browser;
        Button      btnClose;

        int width  = 380;
        int height = 230;

        public int BoxWidth  { set { width  = value; } }
        public int BoxHeight { set { height = value; } }

        /// <summary>
        /// Public Constructor
        /// </summary>
        public Fancy_Message_Box(string text, string title)
        {
            pnlBase    = new Fancy_Panel();
            pnlControl = new Panel();
            browser    = new WebBrowser();
            btnClose   = new Button();

            Text          = title;
            Icon          = Data.Icon;
            MaximizeBox   = false;
            MinimizeBox   = false;
            ShowInTaskbar = false;
            TopMost       = true;
            AcceptButton  = btnClose;

            pnlBase.Parent = this;

            browser.Parent              = pnlBase;
            browser.AllowNavigation     = false;
            browser.AllowWebBrowserDrop = false;
            browser.DocumentText        = GetText(text, title);
            browser.Dock                = DockStyle.Fill;
            browser.TabStop             = false;
            browser.IsWebBrowserContextMenuEnabled = false;
            browser.WebBrowserShortcutsEnabled     = true;

            pnlControl.Parent    = this;
            pnlControl.Dock      = DockStyle.Bottom;
            pnlControl.BackColor = Color.Transparent;

            btnClose.Parent = pnlControl;
            btnClose.Text   = Language.T("Close");
            btnClose.Name   = "Close";
            btnClose.Click += new EventHandler(BtnClose_Click);
            btnClose.UseVisualStyleBackColor = true;
        }

        /// <summary>
        /// Button Close OnClick.
        /// </summary>
        void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Form OnLoad.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Width  = width;
            Height = height;
        }

        /// <summary>
        /// OnResize.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            int iButtonHeight = (int)(Data.VerticalDLU * 15.5);
            int iButtonWidth  = (int)(Data.HorizontalDLU * 60);
            int iBtnVertSpace = (int)(Data.VerticalDLU * 5.5);
            int iBtnHrzSpace  = (int)(Data.HorizontalDLU * 3);
            int iBorder       = iBtnHrzSpace;

            pnlControl.Height = iButtonHeight + 2 * iBtnVertSpace;

            pnlBase.Size     = new Size(ClientSize.Width - 2 * iBorder, pnlControl.Top - iBorder);
            pnlBase.Location = new Point(iBorder, iBorder);

            btnClose.Size     = new Size(iButtonWidth, iButtonHeight);
            btnClose.Location = new Point(ClientSize.Width - btnClose.Width - iBtnHrzSpace, iBtnVertSpace);
        }

        /// <summary>
        /// Form OnPaint.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        string GetText(string text, string title)
        {
            string header, footer;

            // Header
            header = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">";
            header += "<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\">";
            header += "<head><meta http-equiv=\"content-type\" content=\"text/html; charset=UTF-8\" />";
            header += "<title>" + title + "</title><style>";
            header += "body {margin: 0px; font-size: 14px; background-color: #fffffd}";
            header += ".content {padding: 5px;}";
            header += ".content h1 {margin: 0.5em 0 0.2em 0; font-weight: bold; font-size: 1.1em; color: #000033;}";
            header += ".content h2 {margin: 0.5em 0 0.2em 0; font-weight: bold; font-size: 1.0em; color: #000033;}";
            header += ".content p {margin-left: 5px; color: #000033;}";
            header += "</style></head>";
            header += "<body>";
            header += "<div class=\"content\">";

            // Footer
            footer = "</div></body></html>";

            return header + text + footer;
        }
    }
}
