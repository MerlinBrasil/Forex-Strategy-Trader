// Controls - ChartPage
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Class Controls : Menu_and_StatusBar
    /// </summary>
    public partial class Controls : Menu_and_StatusBar
    {
        Chart chart;

        /// <summary>
        /// Initializes page Chart.
        /// </summary>
        public void Initialize_PageChart()
        {
            tabPageChart.Name = "tabPageChart";
            tabPageChart.Text = Language.T("Chart");
            tabPageChart.ImageIndex = 2;

            return;
        }

        /// <summary>
        /// Chart tab page is shown.
        /// </summary>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (chart != null)
            {
                Chart_Data chartData = GetChartDataObject();
                chart.InitChart(chartData);
            }
        }

        /// <summary>
        /// Creates a new chart.
        /// </summary>
        void CreateChart()
        {
            if (tabControlBase.SelectedTab == tabPageChart)
            {
                Chart_Data chartData = GetChartDataObject();
                chart = new Chart(chartData);
                chart.Parent = tabPageChart;
                chart.Dock   = DockStyle.Fill;
                chart.InitChart(chartData);
            }

            return;
        }

        /// <summary>
        /// Disposes the chart.
        /// </summary>
        public void DisposeChart()
        {
            if (chart != null)
            {
                try
                {
                    chart.Dispose();
                }
                finally
                {
                    chart = null;
                }
            }

            return;
        }

        DateTime    chartTime   = DateTime.Now;
        DateTime    chartTime10 = DateTime.Now;
        string      chartSymbol = "";
        DataPeriods chartPeriod = DataPeriods.day;
        int         chartBars   = 0;


        /// <summary>
        /// Updates the chart.
        /// </summary>
        protected void UpdateChart()
        {
            if (chart == null)
                return;
           
            bool repaintChart = (
                chartSymbol != Data.Symbol ||
                chartPeriod != Data.Period ||
                chartBars   != Data.Bars   || 
                chartTime   != Data.Time[Data.Bars - 1] ||
                chartTime10 != Data.Time[Data.Bars - 11]);

            chartSymbol = Data.Symbol;
            chartPeriod = Data.Period;
            chartBars   = Data.Bars;
            chartTime   = Data.Time[Data.Bars - 1];
            chartTime10 = Data.Time[Data.Bars - 11];

            // Prepares chart data.
            Chart_Data chartData = GetChartDataObject();

            try
            {
                UpdateChartThreadSafely(repaintChart, chartData);
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, Language.T("Indicator Chart"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                DisposeChart();
                CreateChart();
            }
            
            return;
        }

        delegate void UpdateChartDelegate(bool repaintChart, Chart_Data chartData);
        void UpdateChartThreadSafely(bool repaintChart, Chart_Data chartData)
        {
            if (chart.InvokeRequired)
            {
                chart.BeginInvoke(new UpdateChartDelegate(UpdateChartThreadSafely), new object[] { repaintChart, chartData });
            }
            else
            {
                chart.UpdateChartOnTick(repaintChart, chartData);
            }

        }


        Chart_Data GetChartDataObject()
        {
            Chart_Data chartData = new Chart_Data();
            chartData.InstrumentProperties = Data.InstrProperties.Clone();
            chartData.Bars   = Data.Bars;
            chartData.Time   = new DateTime[Data.Bars];
            chartData.Open   = new double[Data.Bars];
            chartData.High   = new double[Data.Bars];
            chartData.Low    = new double[Data.Bars];
            chartData.Close  = new double[Data.Bars];
            chartData.Volume = new int[Data.Bars];
            Data.Time.CopyTo(chartData.Time, 0);
            Data.Open.CopyTo(chartData.Open, 0);
            Data.High.CopyTo(chartData.High, 0);
            Data.Low.CopyTo(chartData.Low, 0);
            Data.Close.CopyTo(chartData.Close, 0);
            Data.Volume.CopyTo(chartData.Volume, 0);
            chartData.StrategyName = Data.StrategyName;
            chartData.Strategy  = Data.Strategy.Clone();
            chartData.FirstBar  = Data.FirstBar;
            chartData.Symbol    = Data.Symbol;
            chartData.PeriodStr = Data.PeriodStr;
            chartData.Bid       = Data.Bid;
            chartData.BarStatistics = new Dictionary<DateTime, BarStats>();
            foreach (KeyValuePair<DateTime, BarStats> timeStat in Data.BarStatistics)
                chartData.BarStatistics.Add(timeStat.Key, timeStat.Value.Clone());
            chartData.PositionDirection  = Data.PositionDirection;
            chartData.PositionOpenPrice  = Data.PositionOpenPrice;
            chartData.PositionProfit     = Data.PositionProfit;
            chartData.PositionStopLoss   = Data.PositionStopLoss;
            chartData.PositionTakeProfit = Data.PositionTakeProfit;

            return chartData;
        }

        /// <summary>
        /// Sets colors of the chart.
        /// </summary>
        void SetChartColors()
        {
            DisposeChart();
            CreateChart();

            return;
        }
    }
}
