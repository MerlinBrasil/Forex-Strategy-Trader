// Fancy_Panel
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Trader
{
    public class Fancy_Panel : Panel
    {
        bool   isShowCaption = true;
        string caption;
        int    border = 2;
        float  width;
        float  height;
        float  captionHeight;
        Pen    penBorder;
        Font   fontCaption;
        Brush  brushCaption;
        Color  colorCaptionBack;
        StringFormat stringFormatCaption;
        RectangleF   rectfCaption;

        /// <summary>
        /// Gets the caption height.
        /// </summary>
        public float CaptionHeight { get { return captionHeight; } }

        /// <summary>
        /// Gets the border's width.
        /// </summary>
        public int BorderWidth { get { return border; } }

        /// <summary>
        /// Gets the client rectangle.
        /// </summary>
        public Rectangle InnerRectangle { get { return new Rectangle(border, (int)captionHeight, Width - 2 * border, Height - border - (int)captionHeight); } }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Fancy_Panel()
        {
            caption = "";
            isShowCaption = false;
            InitializeParameters();
            SetColors();
            Padding = new Padding(border, 2 * border, border, border);
            return;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Fancy_Panel(string sCaption)
        {
            this.caption = sCaption;
            InitializeParameters();
            SetColors();

            return;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Fancy_Panel(string caption, Color colorCaption)
        {
            this.caption = caption;
            colorCaptionBack = colorCaption;
            brushCaption = new SolidBrush(LayoutColors.ColorCaptionText);
            penBorder    = new Pen(Data.ColorChanage(colorCaption, -LayoutColors.DepthCaption), border);

            InitializeParameters();

            return;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Fancy_Panel(string caption, Color colorCaption, Color colorCaptionText)
        {
            this.caption = caption;
            colorCaptionBack = colorCaption;
            brushCaption = new SolidBrush(colorCaptionText);
            penBorder    = new Pen(Data.ColorChanage(colorCaption, -LayoutColors.DepthCaption), border);

            InitializeParameters();

            return;
        }

        /// <summary>
        /// Sets the panel colors
        /// </summary>
        public void SetColors()
        {
            colorCaptionBack = LayoutColors.ColorCaptionBack;
            brushCaption     = new SolidBrush(LayoutColors.ColorCaptionText);
            penBorder        = new Pen(Data.ColorChanage(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), border);
        }

        /// <summary>
        /// Initialize Parameters
        /// </summary>
        void InitializeParameters()
        {
            fontCaption  = new Font(Font.FontFamily, 9);
            stringFormatCaption = new StringFormat();
            stringFormatCaption.Alignment     = StringAlignment.Center;
            stringFormatCaption.LineAlignment = StringAlignment.Center;
            stringFormatCaption.Trimming      = StringTrimming.EllipsisCharacter;
            stringFormatCaption.FormatFlags   = StringFormatFlags.NoWrap;

            captionHeight = isShowCaption ? (float)Math.Max(fontCaption.Height, 18) : 2 * border;

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
            if (isShowCaption)
            {
                g.DrawString(caption, fontCaption, brushCaption, rectfCaption, stringFormatCaption);
            }
            g.DrawLine(penBorder, 1, captionHeight, 1, ClientSize.Height);
            g.DrawLine(penBorder, ClientSize.Width - border + 1, captionHeight, ClientSize.Width - border + 1, ClientSize.Height);
            g.DrawLine(penBorder, 0, ClientSize.Height - border + 1, ClientSize.Width, ClientSize.Height - border + 1);

            // Paint the panel background
            RectangleF rectClient = new RectangleF(border, captionHeight, width, height);
            Data.GradientPaint(g, rectClient, LayoutColors.ColorControlBack, LayoutColors.DepthControl);

            return;
        }

        /// <summary>
        /// On Resize
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);

            if (isShowCaption)
            {
                width = ClientSize.Width - 2 * border;
                height = ClientSize.Height - captionHeight - border;
                rectfCaption = new RectangleF(0, 0, ClientSize.Width, captionHeight);
            }
            else
            {
                width  = ClientSize.Width - 2 * border;
                height = ClientSize.Height - captionHeight - border;
                rectfCaption = new RectangleF(0, 0, ClientSize.Width, captionHeight);
            }

            Invalidate();

            return;
        }

    }
}
