// Balance_Chart Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Forex_Strategy_Trader
{
    public struct Balance_Chart_Unit
    {
        double balance;
        double equity;
        DateTime time;

        public double Balance
        {
            get { return balance; }
            set { balance = value; }
        }

        public double Equity
        {
            get { return equity; }
            set { equity = value; }
        }

        public DateTime Time
        {
            get { return time; }
            set { time = value; }
        }
    }


    /// <summary>
    /// Draws a balance chart
    /// </summary>
    public class Balance_Chart : Panel
    {
        int    space  = 5;
        int    border = 2;
        int    XLeft;
        int    XRight;
        float  scaleX;
        int    chartPoints;
        int    maxValue;
        int    minValue;
        int    YTop;
        int    YBottom;
        int    labelWidth;
        string chartTitle;
        Font   font;
        float  captionHeight;
        RectangleF rectfCaption;
        Rectangle  chartArea;
        StringFormat stringFormatCaption;
        int   cntLabels;
        float deltaLabels;
        int   stepLabels;
        float scaleY;
        Brush brushFore;
        Pen   penGrid;
        Pen   penBorder;
        PointF[] apntBalance;
        PointF[] apntEquity;
        float balanceY;
        float equityY;
        float netBalance;
        float netEquity;

        int maxBalance;
        int minBalance;
        int maxEquity;
        int minEquity;

        double[] balanceData;
        double[] equityData;
        DateTime startTime;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Balance_Chart()
        {
            SetColors();

            // Chart Title
            chartTitle  = Language.T("Balance / Equity Chart");
            font = new Font(Font.FontFamily, 9);
            captionHeight = (float)Math.Max(font.Height, 18);
            rectfCaption  = new RectangleF(0, 0, ClientSize.Width, captionHeight);
            stringFormatCaption               = new StringFormat();
            stringFormatCaption.Alignment     = StringAlignment.Center;
            stringFormatCaption.LineAlignment = StringAlignment.Center;
            stringFormatCaption.Trimming      = StringTrimming.EllipsisCharacter;
            stringFormatCaption.FormatFlags   = StringFormatFlags.NoWrap;

            penGrid.DashStyle   = DashStyle.Dash;
            penGrid.DashPattern = new float [] {4, 2};

            return;
        }

        /// <summary>
        /// Sets data to be displayed.
        /// </summary>
        public void UpdateChartData(Balance_Chart_Unit[] data, int points)
        {
            if (data == null || points < 1)
                return;

            balanceData = new double[points];
            equityData  = new double[points];;
            for (int p = 0; p < points; p++)
            {
                balanceData[p] = data[p].Balance;
                equityData[p]  = data[p].Equity;
            }

            maxBalance = int.MinValue;
            minBalance = int.MaxValue;
            maxEquity  = int.MinValue;
            minEquity  = int.MaxValue;

            foreach (double balance in balanceData)
            {
                if (balance > maxBalance) maxBalance = (int)balance;
                if (balance < minBalance) minBalance = (int)balance;
            }

            foreach (double equity in equityData)
            {
                if (equity > maxEquity) maxEquity = (int)equity;
                if (equity < minEquity) minEquity = (int)equity;
            }

            startTime = data[0].Time;

            InitChart();

            return;
        }

        /// <summary>
        /// Refreshes the chart.
        /// </summary>
        public void RefreshChart()
        {
            this.Invalidate(chartArea);

            return;
        }

        /// <summary>
        /// Sets the chart params
        /// </summary>
        void InitChart()
        {
            if (balanceData == null || balanceData.Length < 1)
                return;

            chartPoints = Math.Max(balanceData.Length, equityData.Length);

            if (chartPoints == 0) return;

            maxValue = Math.Max(maxBalance, maxEquity) + 1;
            minValue = Math.Min(minBalance, minEquity) - 1;
            minValue = (int)(Math.Floor(minValue / 10f) * 10);

            YTop    = (int)captionHeight + 2 * space + 1;
            YBottom = ClientSize.Height  - space - border - Font.Height;

            Graphics  g = CreateGraphics();
            labelWidth = (int)Math.Max(g.MeasureString(minValue.ToString("F2"), Font).Width, g.MeasureString(maxValue.ToString("F2"), Font).Width);
            g.Dispose();

            labelWidth = Math.Max(labelWidth, 30);
            XLeft  = border + space; 
            XRight = ClientSize.Width - border - space - labelWidth - 6;
            scaleX = (XRight - 2 * space - border - 10) / (float)(chartPoints - 1);

            cntLabels   = (int)Math.Max((YBottom - YTop) / 40, 1);
            deltaLabels = (float)Math.Max(Math.Round((maxValue - minValue) / (float)cntLabels), 10);
            stepLabels  = (int)Math.Ceiling(deltaLabels / 10) * 10;
            cntLabels   = (int)Math.Ceiling((maxValue - minValue) / (float)stepLabels);
            maxValue    = minValue + cntLabels * stepLabels;
            scaleY      = (YBottom - YTop) / (cntLabels * (float)stepLabels);

            apntBalance = new PointF[chartPoints];
            apntEquity  = new PointF[chartPoints];

            int index = 0;
            foreach(double balance in balanceData)
            {
                apntBalance[index].X = XLeft + index * scaleX;
                apntBalance[index].Y = (float)(YBottom - (balance - minValue) * scaleY);
                index++;
            }

            index = 0;
            foreach(double equity in equityData)
            {
                apntEquity[index].X  = XLeft + index * scaleX;
                apntEquity[index].Y  = (float)(YBottom - (equity - minValue) * scaleY);
                index++;
            }

            netBalance = (float)balanceData[balanceData.Length - 1];
            balanceY   = YBottom - (netBalance - minValue) * scaleY;

            netEquity = (float)equityData[equityData.Length - 1];
            equityY   = YBottom - (netEquity - minValue) * scaleY;

            return;
        }

        /// <summary>
        /// Paints the chart
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            try { g.Clear(LayoutColors.ColorChartBack); }
            catch { }

            // Caption bar
            Data.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);
            g.DrawString(chartTitle, Font, new SolidBrush(LayoutColors.ColorCaptionText), rectfCaption, stringFormatCaption);

            // Border
            g.DrawLine(penBorder, 1, captionHeight, 1, ClientSize.Height);
            g.DrawLine(penBorder, ClientSize.Width - border + 1, captionHeight, ClientSize.Width - border + 1, ClientSize.Height);
            g.DrawLine(penBorder, 0, ClientSize.Height - border + 1, ClientSize.Width, ClientSize.Height - border + 1);

            if (balanceData == null || balanceData.Length < 1 ||
                equityData  == null || equityData.Length  < 1)
                return;

            // Grid and Price labels
            for (int iLabel = minValue; iLabel <= maxValue; iLabel += stepLabels)
            {
                int iLabelY = (int)(YBottom - (iLabel - minValue) * scaleY);
                g.DrawString(iLabel.ToString(".00"), Font, brushFore, XRight, iLabelY - Font.Height / 2 - 1);
                g.DrawLine(penGrid, XLeft, iLabelY, XRight, iLabelY);
            }

            // Equity and Balance lines
            g.DrawLines(new Pen(LayoutColors.ColorChartEquityLine), apntEquity);
            g.DrawLines(new Pen(LayoutColors.ColorChartBalanceLine), apntBalance);

            // Coordinate axes
            g.DrawLine(new Pen(LayoutColors.ColorChartFore), XLeft - 1, YTop - space, XLeft - 1, YBottom + 1 + Font.Height);

            // Equity price label.
            Point  pntEquity  = new Point(XRight - space + 2, (int)(equityY - font.Height / 2 - 1));
            Size   sizeEquity = new Size(labelWidth + space, font.Height + 2);
            string equity = (netEquity.ToString("F2"));
            PointF[] apEquity = new PointF[] {
                new PointF(XRight - space - 8, equityY), 
                new PointF(XRight - space, equityY - sizeEquity.Height / 2), 
                new PointF(XRight - space + sizeEquity.Width + 5, equityY - sizeEquity.Height / 2), 
                new PointF(XRight - space + sizeEquity.Width + 5, equityY + sizeEquity.Height / 2), 
                new PointF(XRight - space, equityY + sizeEquity.Height / 2),
            };
            g.FillPolygon(new SolidBrush(LayoutColors.ColorChartEquityLine), apEquity);
            g.DrawString(equity, font, new SolidBrush(LayoutColors.ColorChartBack), pntEquity);

            // Balance price label.
            Point  pntBalance  = new Point(XRight - space + 2, (int)(balanceY - font.Height / 2 - 1));
            Size   sizeBalance = new Size(labelWidth + space, font.Height + 2);
            string balance = (netBalance.ToString("F2"));
            PointF[] apBalance = new PointF[] {
                new PointF(XRight - space - 8, balanceY), 
                new PointF(XRight - space, balanceY - sizeBalance.Height / 2), 
                new PointF(XRight - space + sizeBalance.Width + 5, balanceY - sizeBalance.Height / 2), 
                new PointF(XRight - space + sizeBalance.Width + 5, balanceY + sizeBalance.Height / 2), 
                new PointF(XRight - space, balanceY + sizeBalance.Height / 2),
            };
            g.FillPolygon(new SolidBrush(LayoutColors.ColorChartBalanceLine), apBalance);
            g.DrawString(balance, font, new SolidBrush(LayoutColors.ColorChartBack), pntBalance);

            // Chart Text
            string chartText = startTime.ToString();
            g.DrawString(chartText, font, new SolidBrush(LayoutColors.ColorChartFore), XLeft, YBottom);

            return;
        }

        /// <summary>
        /// Invalidates the chart after resizing
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            rectfCaption = new RectangleF(0, 0, ClientSize.Width, captionHeight);
            chartArea    = new Rectangle(border, (int)captionHeight, ClientSize.Width - 2 * border, ClientSize.Height - border - (int)captionHeight);
            
            InitChart();
            Invalidate();

            return;
        }

        /// <summary>
        /// Sets the panel colors
        /// </summary>
        public void SetColors()
        {
            brushFore = new SolidBrush(LayoutColors.ColorChartFore);
            penGrid   = new Pen(LayoutColors.ColorChartGrid);
            penBorder = new Pen(Data.ColorChanage(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), border);

            return;
        }
    }
}
