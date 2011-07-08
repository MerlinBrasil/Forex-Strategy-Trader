// Info Panel Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Trader
{
    public class Info_Panel : Panel
    {
        string   sCaption;
        string[] asParam;
        string[] asValue;

        int iBorder = 2;

        int   iRows;
        int   iVisibleRows;
        float fRowHeight;
        float fCaptionHeight;
        float fMaxParamWidth;
        float fMaxValueWidth;
        float fRequiredHeight;
        float fPaddingParamData = 4;
        float fParamTab;
        float fValueTab;
        float fWidth;
        float fHeight;
        Pen   penBorder;
        Font  fontCaption;
        Font  fontData;
        Brush brushCaption;
        Brush brushParams;
        Brush brushData;
        Color colorCaptionBack;
        Color colorBackroundEvenRows;
        Color colorBackroundOddRows;
        StringFormat stringFormatCaption;
        StringFormat stringFormatData;
        RectangleF   rectfCaption;

        VScrollBar vScrollBar;
        HScrollBar hScrollBar;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Info_Panel()
        {
            InitializeParameters();
            SetColors();

            return;
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Info_Panel(string caption)
        {
            sCaption = caption;
            InitializeParameters();
            SetColors();

            return;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Info_Panel(string[] asParams, string[] asValues, string sCaption)
        {
            this.asParam  = asParams;
            this.asValue  = asValues;
            this.sCaption = sCaption;

            InitializeParameters();

            Update(asParam, asValues, sCaption);

            return;
        }

        /// <summary>
        /// Sets the panel colors
        /// </summary>
        public void SetColors()
        {
            colorCaptionBack         = LayoutColors.ColorCaptionBack;
            colorBackroundEvenRows   = LayoutColors.ColorEvenRowBack;
            colorBackroundOddRows    = LayoutColors.ColorOddRowBack;

            brushCaption = new SolidBrush(LayoutColors.ColorCaptionText);
            brushParams  = new SolidBrush(LayoutColors.ColorControlText);
            brushData    = new SolidBrush(LayoutColors.ColorControlText);

            penBorder    = new Pen(Data.ColorChanage(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), iBorder);

            return;
        }

        /// <summary>
        /// Initialize Parameters
        /// </summary>
        void InitializeParameters()
        {
            // Caption
            stringFormatCaption = new StringFormat();
            stringFormatCaption.Alignment     = StringAlignment.Center;
            stringFormatCaption.LineAlignment = StringAlignment.Center;
            stringFormatCaption.Trimming      = StringTrimming.EllipsisCharacter;
            stringFormatCaption.FormatFlags   = StringFormatFlags.NoWrap;
            fontCaption    = new Font(Font.FontFamily, 9);
            fCaptionHeight = (float)Math.Max(fontCaption.Height, 18);
            rectfCaption   = new RectangleF(0, 0, ClientSize.Width, fCaptionHeight);

            // Data row
            stringFormatData = new StringFormat();
            stringFormatData.Trimming = StringTrimming.EllipsisCharacter;
            fontData   = new Font(Font.FontFamily, 9);
            fRowHeight = fontData.Height + 4;

            Padding = new Padding(iBorder, (int)fCaptionHeight, iBorder, iBorder);
            
            hScrollBar = new HScrollBar();
            hScrollBar.Parent      = this;
            hScrollBar.Dock        = DockStyle.Bottom;
            hScrollBar.Enabled     = false;
            hScrollBar.Visible     = false;
            hScrollBar.SmallChange = 10;
            hScrollBar.LargeChange = 30;
            hScrollBar.Scroll     += new ScrollEventHandler(ScrollBar_Scroll);

            vScrollBar = new VScrollBar();
            vScrollBar.Parent      = this;
            vScrollBar.Dock        = DockStyle.Right;
            vScrollBar.TabStop     = true;
            vScrollBar.Enabled     = false;
            vScrollBar.Visible     = false;
            vScrollBar.SmallChange = 1;
            vScrollBar.LargeChange = 3;
            vScrollBar.Maximum     = 20;
            vScrollBar.Scroll     += new ScrollEventHandler(ScrollBar_Scroll);

            MouseUp += new MouseEventHandler(Info_Panel_MouseUp);

            return;
        }

        /// <summary>
        /// Update
        /// </summary>
        public void Update(string[] asParams, string[] asValues, string sCaption)
        {
            this.asParam  = asParams;
            this.asValue  = asValues;
            this.sCaption = sCaption;

            iRows = asParam.Length;
            fRequiredHeight = fCaptionHeight + iRows * fRowHeight + iBorder;

            // Maximum width
            fMaxParamWidth = 0;
            fMaxValueWidth = 0;
            Graphics g = CreateGraphics();
            for (int i = 0; i < iRows; i++)
            {
                float fWidthParam = g.MeasureString(asParam[i], fontData).Width;
                if (fWidthParam > fMaxParamWidth)
                    fMaxParamWidth = fWidthParam;

                float fValueWidth = g.MeasureString(asValue[i], fontData).Width;
                if (fValueWidth > fMaxValueWidth)
                    fMaxValueWidth = fValueWidth;
            }
            g.Dispose();

            CalculateScrollBarStatus();
            CalculateTabs();
            Invalidate();

            return;
        }

        /// <summary>
        /// On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Caption
            Data.GradientPaint(g, rectfCaption, colorCaptionBack, LayoutColors.DepthCaption);
            g.DrawString(sCaption, fontCaption, brushCaption, rectfCaption, stringFormatCaption);

            float y = fCaptionHeight;
            for (int i = 0; i * fRowHeight + fCaptionHeight < fHeight; i++)
            {
                float fVerticalPosition = i * fRowHeight + fCaptionHeight;
                PointF pointFParam = new PointF(fParamTab + 2, fVerticalPosition);
                PointF pointFValue = new PointF(fValueTab + 2, fVerticalPosition);
                RectangleF rectRow = new RectangleF(iBorder, fVerticalPosition, fWidth, fRowHeight);

                // Row background
                if (i % 2f != 0)
                    g.FillRectangle(new SolidBrush(colorBackroundEvenRows), rectRow);
                else
                    g.FillRectangle(new SolidBrush(colorBackroundOddRows), rectRow);

                if (i + vScrollBar.Value >= iRows)
                    continue;

                g.DrawString(asParam[i + vScrollBar.Value], fontData, brushParams, pointFParam, stringFormatData);
                g.DrawString(asValue[i + vScrollBar.Value], fontData, brushData,   pointFValue, stringFormatData);
            }

            // Border
            g.DrawLine(penBorder, 1, fCaptionHeight, 1, ClientSize.Height);
            g.DrawLine(penBorder, ClientSize.Width - iBorder + 1, fCaptionHeight, ClientSize.Width - iBorder + 1, ClientSize.Height);
            g.DrawLine(penBorder, 0, ClientSize.Height - iBorder + 1, ClientSize.Width, ClientSize.Height - iBorder + 1);

            return;
        }

        /// <summary>
        /// On Resize
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);

            CalculateScrollBarStatus();
            CalculateTabs();
            Invalidate();

            return;
        }

        /// <summary>
        /// Scroll Bars status
        /// </summary>
        void CalculateScrollBarStatus()
        {
            fWidth  = ClientSize.Width  - 2 * iBorder;
            fHeight = ClientSize.Height - iBorder;

            bool bMustHorisontal = fWidth  < fMaxParamWidth + fPaddingParamData + fMaxValueWidth - 2;
            bool bMustVertical   = fHeight < fRequiredHeight;
            bool bIsHorisontal   = bMustHorisontal;
            bool bIsVertical     = bMustVertical;

            if (bMustHorisontal && bMustVertical)
            {
                bIsVertical   = true;
                bIsHorisontal = true;
            }
            else if (bMustHorisontal && !bMustVertical)
            {
                bIsHorisontal = true;
                fHeight       = ClientSize.Height - hScrollBar.Height - iBorder;
                bIsVertical   = fHeight < fRequiredHeight;
            }
            else if (!bMustHorisontal && bMustVertical)
            {
                bIsVertical   = true;
                fWidth        = ClientSize.Width - vScrollBar.Width - 2 * iBorder;
                bIsHorisontal = fWidth < fMaxParamWidth + fPaddingParamData + fMaxValueWidth - 2;
            }
            else
            {
                bIsHorisontal = false;
                bIsVertical   = false;
            }

            if (bIsHorisontal)
                fHeight = ClientSize.Height - hScrollBar.Height - iBorder;

            if (bIsVertical)
                fWidth = ClientSize.Width - vScrollBar.Width - 2 * iBorder;

            vScrollBar.Enabled = bIsVertical;
            vScrollBar.Visible = bIsVertical;
            hScrollBar.Enabled = bIsHorisontal;
            hScrollBar.Visible = bIsHorisontal;

            hScrollBar.Value = 0;
            if (bIsHorisontal)
            {
                int iPoinShort = (int)(fMaxParamWidth + fPaddingParamData + fMaxValueWidth - fWidth);
                hScrollBar.Maximum = iPoinShort + hScrollBar.LargeChange - 2;
            }

            rectfCaption = new RectangleF(0, 0, ClientSize.Width, fCaptionHeight);
            iVisibleRows = (int)Math.Min(((fHeight - fCaptionHeight) / fRowHeight), iRows);

            vScrollBar.Value = 0;
            vScrollBar.Maximum = iRows - iVisibleRows + vScrollBar.LargeChange - 1;

            return;
        }

        /// <summary>
        /// Tabs
        /// </summary>
        void CalculateTabs()
        {
            if (fWidth < fMaxParamWidth + fPaddingParamData + fMaxValueWidth)
            {
                fParamTab = -hScrollBar.Value + iBorder;
                fValueTab = fParamTab + fMaxParamWidth;
            }
            else
            {
                float fSpace = (fWidth - (fMaxParamWidth + fMaxValueWidth)) / 5;
                fParamTab = 2 * fSpace;
                fValueTab = fParamTab + fMaxParamWidth + fSpace;
            }

            return;
        }

        /// <summary>
        /// ScrollBar_Scroll
        /// </summary>
        void ScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            CalculateTabs();
            int ihScrallBarSize = hScrollBar.Visible ? hScrollBar.Height : 0;
            int ivScrallBarSize = vScrollBar.Visible ? vScrollBar.Width  : 0;
            Rectangle rect = new Rectangle(iBorder, (int)fCaptionHeight, ClientSize.Width - ivScrallBarSize - 2 * iBorder, ClientSize.Height - (int)fCaptionHeight - ihScrallBarSize - iBorder);
            Invalidate(rect);
        }

        /// <summary>
        /// Selects the vertical scrollbar
        /// </summary>
        void Info_Panel_MouseUp(object sender, MouseEventArgs e)
        {
            vScrollBar.Select();
        }
    }
}
