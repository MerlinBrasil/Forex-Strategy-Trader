// Controls Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System.Windows.Forms;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Class Controls : Menu_and_StatusBar
    /// </summary>
    public partial class Controls : Menu_and_StatusBar
    {
        Balance_Chart balanceChart;

        /// <summary>
        /// Initializes page Account.
        /// </summary>
        public void Initialize_PageAccount()
        {
            tabPageAccount.Name = "tabPageAccount";
            tabPageAccount.Text = Language.T("Account");
            tabPageAccount.ImageIndex = 3;
            tabPageAccount.BackColor  = LayoutColors.ColorFormBack;

            balanceChart = new Balance_Chart();
            balanceChart.Parent = tabPageAccount;
            balanceChart.Dock   = DockStyle.Fill;
            balanceChart.UpdateChartData(Data.BalanceData, Data.BalanceDataPoints);
            balanceChart.Invalidate();

            return;
        }

        /// <summary>
        /// Sets the colors of tab page Account.
        /// </summary>
        void SetAccountColors()
        {
            balanceChart.SetColors();
            balanceChart.Invalidate();

            return;
        }

        delegate void UpdateBalanceChartDelegate(Balance_Chart_Unit[] balanceData, int balancePoints);
        /// <summary>
        /// Updates the chart.
        /// </summary>
        protected void UpdateBalanceChart(Balance_Chart_Unit[] balanceData, int balancePoints)
        {
            if (balanceChart.InvokeRequired)
            {
                balanceChart.BeginInvoke(new UpdateBalanceChartDelegate(UpdateBalanceChart), new object[] { balanceData, balancePoints });
            }
            else
            {
                balanceChart.UpdateChartData(balanceData, balancePoints);
                balanceChart.RefreshChart();
            }

            return;
        }
    }
}
