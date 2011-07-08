// Forex Strategy Trader
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
    /// Draws a small balance chart
    /// </summary>
    public class Tick_Chart : Panel
    {
        string caption;
        int    border = 2;
        float  captionHeight;
        Pen    penBorder;
        Pen    penChart;
        Font   fontCaption;
        Brush  brushCaption;
        Color  colorCaptionBack;
        StringFormat stringFormatCaption;
        RectangleF   rectfCaption;
        Rectangle    chartArea;

        double[] tickData;
        double point;

        /// <summary>
        /// Constructor
        /// </summary>
        public Tick_Chart(string caption)
        {
            this.caption = caption;

            fontCaption  = new Font(Font.FontFamily, 9);
            stringFormatCaption = new StringFormat();
            stringFormatCaption.Alignment     = StringAlignment.Center;
            stringFormatCaption.LineAlignment = StringAlignment.Center;
            stringFormatCaption.Trimming      = StringTrimming.EllipsisCharacter;
            stringFormatCaption.FormatFlags   = StringFormatFlags.NoWrap;

            captionHeight = (float)Math.Max(fontCaption.Height, 18);

            SetColors();

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
            penChart         = new Pen(LayoutColors.ColorChartBalanceLine, 2);
        }

        /// <summary>
        /// Sets data to be displayed.
        /// </summary>
        public void UpdateChartData(double point, double[] tickList)
        {
            this.point = point;
            this.tickData = tickList;
        }

        /// <summary>
        /// Refreshes the chart.
        /// </summary>
        public void RefreshChart()
        {
            this.Invalidate(chartArea);
        }

        /// <summary>
        /// Sets the chart params
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            try { g.Clear(LayoutColors.ColorChartBack); }
            catch { }

            Data.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);
            g.DrawString(caption, fontCaption, brushCaption, rectfCaption, stringFormatCaption);
            g.DrawLine(penBorder, 1, captionHeight, 1, ClientSize.Height);
            g.DrawLine(penBorder, ClientSize.Width - border + 1, captionHeight, ClientSize.Width - border + 1, ClientSize.Height);
            g.DrawLine(penBorder, 0, ClientSize.Height - border + 1, ClientSize.Width, ClientSize.Height - border + 1);

            if (tickData == null || tickData.Length < 2)
            {
                string text = Language.T("Waiting for ticks...");
                g.DrawString(text, fontCaption, penChart.Brush, chartArea);
                return;
            }

            int ticks = tickData.Length;
            double maximum = double.MinValue;
            double minimum = double.MaxValue;
            foreach (double tick in tickData)
            {
                if (maximum < tick) maximum = tick;
                if (minimum > tick) minimum = tick;
            }

            maximum += point;
            minimum -= point;

            int space  = border + 2;
            int XLeft  = space;
            int XRight = ClientSize.Width - space;
            double scaleX  = (XRight - XLeft) / ((double)ticks - 1);

            int YTop    = (int)captionHeight + space;
            int YBottom = ClientSize.Height - space;
            double scaleY  = (YBottom - YTop) / (maximum - minimum);

            int index = 0;
            PointF[] apntTick = new PointF[ticks];
            foreach (double tick in tickData)
            {
                apntTick[index].X = (float)(XLeft + index * scaleX);
                apntTick[index].Y = (float)(YBottom - (tick - minimum) * scaleY);
                index++;
            }

            g.DrawLines(penChart, apntTick);
        }

        /// <summary>
        /// Invalidates the chart after resizing
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            rectfCaption = new RectangleF(0, 0, ClientSize.Width, captionHeight);
            chartArea    = new Rectangle(border, (int)captionHeight, ClientSize.Width - 2 * border, ClientSize.Height - border - (int)captionHeight);
            Invalidate();
        }
    }
}
