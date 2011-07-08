// Forex Strategy Trader
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Trader
{
    public enum JournalIcons
    {
        Information, System, Warning, Error, OK, Currency, Blocked, Globe,
        StartTrading, StopTrading, OrderBuy, OrderSell, OrderClose, Recalculate,
        BarOpen, BarClose, PosBuy, PosSell, PosSquare
    };

    public class JournalMessage
    {
        JournalIcons icon;
        DateTime     time;
        string       message;

        public JournalIcons Icon
        {
            get { return icon; }
            set { icon = value; }
        }

        public DateTime Time
        {
            get { return time; }
            set { time = value; }
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public JournalMessage(JournalIcons icon, DateTime time, string message)
        {
            this.icon    = icon;
            this.time    = time;
            this.message = message;
        }
    }

    public class Journal : Panel
    {
        List<JournalMessage> messages;

        int space = 2;

        int   rows;
        int   visibleRows;
        float rowHeight;
        float iconWidth  = 16;
        float iconHeight = 16;
        float timeWidth;
        float maxMessageWidth = 400;
        float requiredHeight;
        float width;
        float height;
        Font  fontMessage;
        Brush brushParams;
        Brush brushData;
        Color colorBackroundEvenRows;
        Color colorBackroundOddRows;

        VScrollBar vScrollBar;
        HScrollBar hScrollBar;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Journal()
        {
            InitializeParameters();
            SetColors();

            return;
        }

        /// <summary>
        /// Initialize Parameters
        /// </summary>
        void InitializeParameters()
        {
            messages = new List<JournalMessage>();

            // Data row
            fontMessage = new Font(Font.FontFamily, 9);
            rowHeight   = Math.Max(fontMessage.Height + 4, 18F);

            Graphics g = CreateGraphics();
            timeWidth = g.MeasureString(DateTime.Now.ToString(Data.DF + " HH:mm:ss"), fontMessage).Width;
            g.Dispose();

            hScrollBar = new HScrollBar();
            hScrollBar.Parent  = this;
            hScrollBar.Dock    = DockStyle.Bottom;
            hScrollBar.Enabled = false;
            hScrollBar.Visible = false;
            hScrollBar.SmallChange = 10;
            hScrollBar.LargeChange = 30;
            hScrollBar.Scroll += new ScrollEventHandler(ScrollBar_Scroll);

            vScrollBar = new VScrollBar();
            vScrollBar.Parent  = this;
            vScrollBar.Dock    = DockStyle.Right;
            vScrollBar.TabStop = true;
            vScrollBar.Enabled = false;
            vScrollBar.Visible = false;
            vScrollBar.SmallChange = 1;
            vScrollBar.LargeChange = 3;
            vScrollBar.Maximum     = 20;
            vScrollBar.Scroll += new ScrollEventHandler(ScrollBar_Scroll);

            return;
        }

        /// <summary>
        /// Sets the panel colors
        /// </summary>
        public void SetColors()
        {
            colorBackroundEvenRows = LayoutColors.ColorEvenRowBack;
            colorBackroundOddRows  = LayoutColors.ColorOddRowBack;

            brushParams = new SolidBrush(LayoutColors.ColorControlText);
            brushData   = new SolidBrush(LayoutColors.ColorControlText);

            return;
        }

        /// <summary>
        /// Gets image for the icon type.
        /// </summary>
        /// <param name="icon"></param>
        /// <returns></returns>
        public Image GetImage (JournalIcons icon)
        {
            Image image;
            switch(icon)
            {
                case JournalIcons.Information:
                    image = Properties.Resources.journal_information;
                    break;
                case JournalIcons.System:
                    image = Properties.Resources.journal_system;
                    break;
                case JournalIcons.Warning:
                    image = Properties.Resources.journal_warning;
                    break;
                case JournalIcons.Error:
                    image = Properties.Resources.journal_error;
                    break;
                case JournalIcons.OK:
                    image = Properties.Resources.journal_ok;
                    break;
                case JournalIcons.Currency:
                    image = Properties.Resources.currency;
                    break;
                case JournalIcons.Blocked:
                    image = Properties.Resources.journal_blocked;
                    break;
                case JournalIcons.Globe:
                    image = Properties.Resources.globe;
                    break;
                case JournalIcons.StartTrading:
                    image = Properties.Resources.journal_start_trading;
                    break;
                case JournalIcons.StopTrading:
                    image = Properties.Resources.journal_stop_trading;
                    break;
                case JournalIcons.OrderBuy:
                    image = Properties.Resources.ord_buy;
                    break;
                case JournalIcons.OrderSell:
                    image = Properties.Resources.ord_sell;
                    break;
                case JournalIcons.OrderClose:
                    image = Properties.Resources.ord_close;
                    break;
                case JournalIcons.Recalculate:
                    image = Properties.Resources.recalculate;
                    break;
                case JournalIcons.BarOpen:
                    image = Properties.Resources.journal_bar_open;
                    break;
                case JournalIcons.BarClose:
                    image = Properties.Resources.journal_bar_close;
                    break;
                case JournalIcons.PosBuy:
                    image = Properties.Resources.pos_buy;
                    break;
                case JournalIcons.PosSell:
                    image = Properties.Resources.pos_sell;
                    break;
                case JournalIcons.PosSquare:
                    image = Properties.Resources.pos_square;
                    break;
                default:
                    image = Properties.Resources.journal_error;
                    break;
            }

            return image;
        }

        /// <summary>
        /// Adds a message.
        /// </summary>
        public void AppendMessage(JournalMessage message)
        {
            messages.Add(message);

            rows = messages.Count;
            requiredHeight = rows * rowHeight;

            CalculateScrollBarStatus();
            Invalidate();

            return;
        }

        /// <summary>
        /// Update message list.
        /// </summary>
        public void UpdateMessages(List<JournalMessage> newMessages)
        {
            messages = newMessages;

            rows = messages.Count;
            requiredHeight = rows * rowHeight;

            CalculateScrollBarStatus();
            Invalidate();

            return;
        }

        /// <summary>
        /// Clears all the messages.
        /// </summary>
        public void ClearMessages()
        {
            messages = new List<JournalMessage>();

            rows = messages.Count;
            requiredHeight = rows * rowHeight;

            CalculateScrollBarStatus();
            Invalidate();

            return;
        }

        /// <summary>
        /// Selects the vertical scroll bar.
        /// </summary>
        public void SelectVScrollBar()
        {
            vScrollBar.Select();

            return;
        }

        /// <summary>
        /// On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            for (int row = 0; row * rowHeight < height; row++)
            {
                float  vertPos = row * rowHeight;

                // Row background
                RectangleF rectRow = new RectangleF(space, vertPos, width, rowHeight);
                if (row % 2f != 0)
                    g.FillRectangle(new SolidBrush(colorBackroundEvenRows), rectRow);
                else
                    g.FillRectangle(new SolidBrush(colorBackroundOddRows), rectRow);

                if (row + vScrollBar.Value >= rows)
                    continue;

                PointF pointMessage = new PointF(iconWidth + 2 * space, vertPos);
                int index = rows - row - vScrollBar.Value - 1;
                g.DrawImage(GetImage(messages[index].Icon), space, vertPos, iconWidth, iconHeight);
                string text  = messages[index].Time.ToString(Data.DF + " HH:mm:ss") + "   " + messages[index].Message;
                g.DrawString(text, fontMessage, brushParams, pointMessage);
            }

            return;
        }

        /// <summary>
        /// On Resize
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);

            CalculateScrollBarStatus();
            Invalidate();

            return;
        }

        /// <summary>
        /// Scroll Bars status
        /// </summary>
        void CalculateScrollBarStatus()
        {
            width  = ClientSize.Width;
            height = ClientSize.Height;

            bool mustHorisontal = width  < iconWidth + timeWidth + maxMessageWidth + 2 * space;
            bool mustVertical   = height < requiredHeight;
            bool isHorisontal   = mustHorisontal;
            bool isVertical     = mustVertical;

            if (mustHorisontal && mustVertical)
            {
                isVertical   = true;
                isHorisontal = true;
            }
            else if (mustHorisontal && !mustVertical)
            {
                isHorisontal = true;
                height = ClientSize.Height - hScrollBar.Height;
                isVertical = height < requiredHeight;
            }
            else if (!mustHorisontal && mustVertical)
            {
                isVertical = true;
                width = ClientSize.Width - vScrollBar.Width - 2 * space;
                isHorisontal = width < iconWidth + timeWidth + maxMessageWidth + 2 * space;
            }
            else
            {
                isHorisontal = false;
                isVertical   = false;
            }

            if (isHorisontal)
                height = ClientSize.Height - hScrollBar.Height;

            if (isVertical)
                width = ClientSize.Width - vScrollBar.Width - 2 * space;

            vScrollBar.Enabled = isVertical;
            vScrollBar.Visible = isVertical;
            hScrollBar.Enabled = isHorisontal;
            hScrollBar.Visible = isHorisontal;

            hScrollBar.Value = 0;
            if (isHorisontal)
            {
                int iPoinShort = (int)(iconWidth + timeWidth + maxMessageWidth + 2 * space - width);
                hScrollBar.Maximum = iPoinShort + hScrollBar.LargeChange - space;
            }

            visibleRows = (int)Math.Min((height / rowHeight), rows);

            vScrollBar.Value = 0;
            vScrollBar.Maximum = rows - visibleRows + vScrollBar.LargeChange - 1;

            return;
        }

        /// <summary>
        /// ScrollBar_Scroll
        /// </summary>
        void ScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            Invalidate();

            return;
        }
    }
}
