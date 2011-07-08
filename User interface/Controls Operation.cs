// Operation Tab
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
    /// Class Controls : Menu_and_StatusBar
    /// </summary>
    public partial class Controls : Menu_and_StatusBar
    {
        Fancy_Panel   pnlManualTrade;
        Panel         pnlHolder;
        Label         lblSymbol, lblBidAsk, lblLots, lblStopLoss, lblTakeProfit, lblBreakEven, lblTrailingStop;
        NumericUpDown nudLots, nudStopLoss, nudTakeProfit, nudBreakEven, nudTrailingStop;
        Button        btnBuy, btnSell, btnClose, btnModify;
        Tick_Chart    tickChart;
        Color         colorParameter;

        protected double OperationLots         { get { return (double)nudLots.Value;      } }
        protected int    OperationStopLoss     { get { return (int)nudStopLoss.Value;     } }
        protected int    OperationTakeProfit   { get { return (int)nudTakeProfit.Value;   } }
        protected int    OperationBreakEven    { get { return (int)nudBreakEven.Value;    } }
        protected int    OperationTrailingStop { get { return (int)nudTrailingStop.Value; } }

        /// <summary>
        /// Initializes Operation tab page.
        /// </summary>
        public void Initialize_PageOperation()
        {
            tabPageOperation.Name       = "tabPageOperation";
            tabPageOperation.Text       = Language.T("Operation");
            tabPageOperation.ImageIndex = 5;
            tabPageOperation.BackColor  = LayoutColors.ColorFormBack;

            pnlManualTrade = new Fancy_Panel(Language.T("Manual Operation Execution"));
            pnlManualTrade.Parent  = tabPageOperation;
            pnlManualTrade.Dock    = DockStyle.Fill;
            pnlManualTrade.Resize += new EventHandler(PnlManualTrade_Resize);

            pnlHolder = new Panel();
            pnlHolder.Parent    = pnlManualTrade;
            pnlHolder.BackColor = Color.Transparent;
            pnlHolder.Size      = new Size(750, 350);

            lblBidAsk = new Label();
            lblBidAsk.Parent    = pnlHolder;
            lblBidAsk.Text      = "Bid / Ask";
            lblBidAsk.BackColor = Color.Transparent;
            lblBidAsk.ForeColor = LayoutColors.ColorControlText;
            lblBidAsk.Font      = new Font(Font.FontFamily, 18, FontStyle.Bold);
            lblBidAsk.Width     = 295;
            lblBidAsk.TextAlign = ContentAlignment.MiddleCenter;
            lblBidAsk.Location  = new Point(190, 35);

            lblSymbol = new Label();
            lblSymbol.Parent    = pnlHolder;
            lblSymbol.Text      = "Symbol";
            lblSymbol.BackColor = Color.Transparent;
            lblSymbol.ForeColor = LayoutColors.ColorControlText;
            lblSymbol.Font      = new Font(Font.FontFamily, 18, FontStyle.Bold);
            lblSymbol.Height    = lblSymbol.Font.Height;
            lblSymbol.Width     = 180;
            lblSymbol.TextAlign = ContentAlignment.MiddleRight;
            lblSymbol.Location  = new Point(5, 35);

            lblLots = new Label();
            lblLots.Parent    = pnlHolder;
            lblLots.Text      = Language.T("Lots");
            lblLots.Font      = new Font(Font.FontFamily, 11); 
            lblLots.BackColor = Color.Transparent;
            lblLots.ForeColor = LayoutColors.ColorControlText;
            lblLots.Width     = 90;
            lblLots.Height    = lblLots.Font.Height;
            lblLots.TextAlign = ContentAlignment.MiddleRight;
            lblLots.Location  = new Point(5, 81);

            lblStopLoss = new Label();
            lblStopLoss.Parent    = pnlHolder;
            lblStopLoss.Text      = Language.T("Stop Loss");
            lblStopLoss.Font      = new Font(Font.FontFamily, 11); 
            lblStopLoss.BackColor = Color.Transparent;
            lblStopLoss.ForeColor = LayoutColors.ColorControlText;
            lblStopLoss.Location  = new Point(5, 121);
            lblStopLoss.Width     = 90;
            lblStopLoss.TextAlign = ContentAlignment.MiddleRight;

            lblTakeProfit = new Label();
            lblTakeProfit.Parent    = pnlHolder;
            lblTakeProfit.Font      = new Font(Font.FontFamily, 11); 
            lblTakeProfit.Text      = Language.T("Take Profit");
            lblTakeProfit.BackColor = Color.Transparent;
            lblTakeProfit.ForeColor = LayoutColors.ColorControlText;
            lblTakeProfit.Location  = new Point(5, 151);
            lblTakeProfit.Width     = 90;
            lblTakeProfit.TextAlign = ContentAlignment.MiddleRight;

            lblBreakEven = new Label();
            lblBreakEven.Parent    = pnlHolder;
            lblBreakEven.Font      = new Font(Font.FontFamily, 11); 
            lblBreakEven.Text      = Language.T("Break Even");
            lblBreakEven.BackColor = Color.Transparent;
            lblBreakEven.ForeColor = LayoutColors.ColorControlText;
            lblBreakEven.Location  = new Point(5, 191);
            lblBreakEven.Width     = 90;
            lblBreakEven.TextAlign = ContentAlignment.MiddleRight;

            lblTrailingStop = new Label();
            lblTrailingStop.Parent    = pnlHolder;
            lblTrailingStop.Font      = new Font(Font.FontFamily, 11); 
            lblTrailingStop.Text      = Language.T("Trailing Stop");
            lblTrailingStop.BackColor = Color.Transparent;
            lblTrailingStop.ForeColor = LayoutColors.ColorControlText;
            lblTrailingStop.Location  = new Point(5, 221);
            lblTrailingStop.Width     = 90;
            lblTrailingStop.TextAlign = ContentAlignment.MiddleRight;

            nudLots = new NumericUpDown();
            nudLots.Parent    = pnlHolder;
            nudLots.Font      = new Font(Font.FontFamily, 11);
            nudLots.TextAlign = HorizontalAlignment.Center;
            nudLots.BeginInit();
            nudLots.Minimum       = 0.1M;
            nudLots.Maximum       = 100;
            nudLots.Increment     = 0.1M;
            nudLots.Value         = 1;
            nudLots.DecimalPlaces = 1;
            nudLots.EndInit();
            nudLots.Width    = 80;
            nudLots.Location = new Point(100, 81);

            nudStopLoss = new NumericUpDown();
            nudStopLoss.Parent    = pnlHolder;
            nudStopLoss.Font      = new Font(Font.FontFamily, 11);
            nudStopLoss.TextAlign = HorizontalAlignment.Center;
            nudStopLoss.BeginInit();
            nudStopLoss.Minimum       = 0;
            nudStopLoss.Maximum       = 5000;
            nudStopLoss.Increment     = 1;
            nudStopLoss.Value         = 0;
            nudStopLoss.DecimalPlaces = 0;
            nudStopLoss.EndInit();
            nudStopLoss.ValueChanged += new EventHandler(Parameter_ValueChanged);
            nudStopLoss.Width    = 80;
            nudStopLoss.Location = new Point(100, 121);

            colorParameter = nudStopLoss.ForeColor;

            nudTakeProfit = new NumericUpDown();
            nudTakeProfit.Parent    = pnlHolder;
            nudTakeProfit.Font      = new Font(Font.FontFamily, 11);
            nudTakeProfit.TextAlign = HorizontalAlignment.Center;
            nudTakeProfit.BeginInit();
            nudTakeProfit.Minimum       = 0;
            nudTakeProfit.Maximum       = 5000;
            nudTakeProfit.Increment     = 1;
            nudTakeProfit.Value         = 0;
            nudTakeProfit.DecimalPlaces = 0;
            nudTakeProfit.EndInit();
            nudTakeProfit.ValueChanged += new EventHandler(Parameter_ValueChanged);
            nudTakeProfit.Width    = 80;
            nudTakeProfit.Location = new Point(100, 151);

            nudBreakEven = new NumericUpDown();
            nudBreakEven.Parent    = pnlHolder;
            nudBreakEven.Font      = new Font(Font.FontFamily, 11);
            nudBreakEven.TextAlign = HorizontalAlignment.Center;
            nudBreakEven.BeginInit();
            nudBreakEven.Minimum       = 0;
            nudBreakEven.Maximum       = 5000;
            nudBreakEven.Increment     = 1;
            nudBreakEven.Value         = 0;
            nudBreakEven.DecimalPlaces = 0;
            nudBreakEven.EndInit();
            nudBreakEven.ValueChanged += new EventHandler(Parameter_ValueChanged);
            nudBreakEven.Width    = 80;
            nudBreakEven.Location = new Point(100, 191);

            nudTrailingStop = new NumericUpDown();
            nudTrailingStop.Parent    = pnlHolder;
            nudTrailingStop.Font      = new Font(Font.FontFamily, 11);
            nudTrailingStop.TextAlign = HorizontalAlignment.Center;
            nudTrailingStop.BeginInit();
            nudTrailingStop.Minimum       = 0;
            nudTrailingStop.Maximum       = 5000;
            nudTrailingStop.Increment     = 1;
            nudTrailingStop.Value         = 0;
            nudTrailingStop.DecimalPlaces = 0;
            nudTrailingStop.EndInit();
            nudTrailingStop.ValueChanged += new EventHandler(Parameter_ValueChanged);
            nudTrailingStop.Width    = 80;
            nudTrailingStop.Location = new Point(100, 221);

            btnSell = new Button();
            btnSell.Name       = "btnSell";
            btnSell.Parent     = pnlHolder;
            btnSell.Image      = Properties.Resources.btn_operation_sell;
            btnSell.ImageAlign = ContentAlignment.MiddleLeft;
            btnSell.Text       = Language.T("Sell");
            btnSell.Click      += new EventHandler(BtnOperation_Click);
            btnSell.Width      = 145;
            btnSell.Height     = 40;
            btnSell.Font       = new Font(Font.FontFamily, 16);
            btnSell.ForeColor  = Color.Crimson;
            btnSell.Location   = new Point(190, 80);
            btnSell.UseVisualStyleBackColor = true;

            btnBuy = new Button();
            btnBuy.Name       = "btnBuy";
            btnBuy.Parent     = pnlHolder;
            btnBuy.Image      = Properties.Resources.btn_operation_buy;
            btnBuy.ImageAlign = ContentAlignment.MiddleLeft;
            btnBuy.Text       = Language.T("Buy");
            btnBuy.Click     += new EventHandler(BtnOperation_Click);
            btnBuy.Width      = 145;
            btnBuy.Height     = 40;
            btnBuy.Font       = new Font(Font.FontFamily, 16);
            btnBuy.ForeColor  = Color.Green;
            btnBuy.Location   = new Point(340, 80);
            btnBuy.UseVisualStyleBackColor = true;

            btnClose = new Button();
            btnClose.Name       = "btnClose";
            btnClose.Parent     = pnlHolder;
            btnClose.Image      = Properties.Resources.btn_operation_close;
            btnClose.ImageAlign = ContentAlignment.MiddleLeft;
            btnClose.Text       = Language.T("Close");
            btnClose.Click     += new EventHandler(BtnOperation_Click);
            btnClose.Width      = 295;
            btnClose.Height     = 40;
            btnClose.Font       = new Font(Font.FontFamily, 16, FontStyle.Bold);
            btnClose.ForeColor  = Color.DarkOrange;
            btnClose.Location   = new Point(190, 126);
            btnClose.UseVisualStyleBackColor = true;

            btnModify = new Button();
            btnModify.Name       = "btnModify";
            btnModify.Parent     = pnlHolder;
            btnModify.Image      = Properties.Resources.recalculate;
            btnModify.ImageAlign = ContentAlignment.MiddleLeft;
            btnModify.Text       = Language.T("Modify Stop Loss and Take Profit");
            btnModify.Click     += new EventHandler(BtnOperation_Click);
            btnModify.ForeColor  = Color.Navy;
            btnModify.Width      = 295;
            btnModify.Location   = new Point(190, 172);
            btnModify.UseVisualStyleBackColor = true;

            tickChart = new Tick_Chart(Language.T("Tick Chart"));
            tickChart.Parent   = pnlHolder;
            tickChart.Size     = new Size(250, 200);
            tickChart.Location = new Point(495, 81);

            return;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            tickChart.RefreshChart();
        }

        /// <summary>
        /// Sets the controls position on resizing.
        /// </summary>
        void PnlManualTrade_Resize(object sender, EventArgs e)
        {
            if (pnlHolder.Width < pnlManualTrade.Width)
            {
                int shift = (pnlManualTrade.Width - pnlHolder.Width) / 2;
                pnlHolder.Location = new Point(shift, 0);
            }
            else
                pnlHolder.Location = new Point(0, 0);

            return;
        }

        /// <summary>
        /// Sets the lot parameters.
        /// </summary>
        protected void SetNumUpDownLots(double minlot, double lotstep, double maxlot)
        {
            nudLots.BeginInit();
            nudLots.Minimum       = (decimal)minlot;
            nudLots.Increment     = (decimal)lotstep;
            nudLots.Maximum       = (decimal)maxlot;
            nudLots.DecimalPlaces = lotstep < 0.1 ? 2 : lotstep < 1 ? 1 : 0;
            nudLots.EndInit();
        }

        /// <summary>
        /// Execute operation
        /// </summary>
        virtual protected void BtnOperation_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Validates the Stop or Limit parameters.
        /// </summary>
        void Parameter_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown nud = (NumericUpDown)sender;
            if (nud.Value > 0 && (double)nud.Value < Data.InstrProperties.StopLevel)
                nud.ForeColor = Color.Red;
            else
                nud.ForeColor = colorParameter;

            return;
        }

        /// <summary>
        /// Sets the colors of tab page Operation.
        /// </summary>
        void SetOperationColors()
        {
            tabPageOperation.BackColor = LayoutColors.ColorFormBack;
            pnlManualTrade.SetColors();
            lblBidAsk.ForeColor       = LayoutColors.ColorControlText;
            lblSymbol.ForeColor       = LayoutColors.ColorControlText;
            lblLots.ForeColor         = LayoutColors.ColorControlText;
            lblStopLoss.ForeColor     = LayoutColors.ColorControlText;
            lblTakeProfit.ForeColor   = LayoutColors.ColorControlText;
            lblBreakEven.ForeColor    = LayoutColors.ColorControlText;
            lblTrailingStop.ForeColor = LayoutColors.ColorControlText;
            tickChart.SetColors();

            return;
        }

        delegate  void SetLblBidAskTextDelegate(string text);
        /// <summary>
        /// Sets the lblBidAsk.Text
        /// </summary>
        protected void SetLblBidAskText(string text)
        {
            if (lblBidAsk.InvokeRequired)
            {
                lblBidAsk.BeginInvoke(new SetLblBidAskTextDelegate(SetLblBidAskText), new object[] { text });
            }
            else
            {
                lblBidAsk.Text = text;
            }

            return;
        }

        delegate void SetLblSymbolTextDelegate(string text);
        /// <summary>
        /// Sets the lblSymbol.Text
        /// </summary>
        protected void SetLblSymbolText(string text)
        {
            if (lblSymbol.InvokeRequired)
            {
                lblSymbol.BeginInvoke(new SetLblBidAskTextDelegate(SetLblSymbolText), new object[] { text });
            }
            else
            {
                lblSymbol.Text = text;
            }

            return;
        }

        delegate void UpdateTickChartDelegate(double point, double[] tickList);
        /// <summary>
        /// Updates the Tick Chart.
        /// </summary>
        protected void UpdateTickChart(double point, double[] tickList)
        {
            if (tickChart.InvokeRequired)
            {
                tickChart.BeginInvoke(new UpdateTickChartDelegate(UpdateTickChart), new object[] {point, tickList});
            }
            else
            {
                tickChart.UpdateChartData(point, tickList);
                tickChart.RefreshChart();
            }

            return;
        }
    }
}
