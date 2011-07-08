// Action Data Feed
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Class Actions : Controls
    /// </summary>
    public partial class Actions : Controls
    {
        MT4Bridge.Bridge bridge;
        Timer timerPing;

        bool isTrading = false;

        bool isSetRootDataError = false;
        DateTime tickLocalTime                = DateTime.MinValue;
        DateTime tickServerTime               = DateTime.MinValue;
        DateTime barOpenTimeForLastCloseEvent = DateTime.MinValue;
        DateTime barOpenTimeForLastOpenTick   = DateTime.MinValue;
        DateTime barOpenTimeForLastCloseTick  = DateTime.MinValue;

        string      symbolReconnect;
        DataPeriods periodReconnect;
        int         accountReconnect;

        bool nullPing    = true;
        int  pingAttempt = 0;

        /// <summary>
        /// Inits data feed.
        /// </summary>
        public void InitDataFeed()
        {
            tickLocalTime = DateTime.MinValue;

            bridge = new MT4Bridge.Bridge();
            bridge.WriteLog = Configs.BridgeWritesLog;
            bridge.Start(Data.ConnectionID);
            bridge.OnTick  += new MT4Bridge.Bridge.TickEventHandler(Bridge_OnTick);

            timerPing = new Timer();
            timerPing.Interval = 1000;
            timerPing.Tick += new EventHandler(TimerPing_Tick);
            timerPing.Start();
        }

        /// <summary>
        /// Deinits data feed.
        /// </summary>
        public void DeinitDataFeed()
        {
            if (timerPing != null)
                timerPing.Stop();

            StopTrade();

            if (bridge != null)
            {
                bridge.OnTick -= new MT4Bridge.Bridge.TickEventHandler(Bridge_OnTick);
                bridge.Stop();
            }
        }

        object lockerTickPing = new object();
        /// <summary>
        /// Pings the server in order to check the connection.
        /// </summary>
        void TimerPing_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now < tickLocalTime.AddSeconds(1))
                return; // The last tick was soon enough.

            lock (lockerTickPing)
            {
                MT4Bridge.PingInfo ping = bridge.GetPingInfo();

                if (ping == null && !nullPing)
                {   // Wrong ping.
                    pingAttempt++;
                    if ((pingAttempt == 1 || pingAttempt % 10 == 0) && JournalShowSystemMessages)
                    {
                        JournalMessage jmsgsys = new JournalMessage(JournalIcons.System, DateTime.Now, Language.T("Unsuccessful ping") + " No " + pingAttempt + ".");
                        AppendJournalMessage(jmsgsys);
                    }
                    if (pingAttempt == 30)
                    {
                        JournalMessage jmsgsys = new JournalMessage(JournalIcons.Warning, DateTime.Now, Language.T("There is no connection with MetaTrader."));
                        AppendJournalMessage(jmsgsys);
                        if (Configs.PlaySounds)
                            Data.SoundError.Play();
                    }
                    if (pingAttempt < 60)
                    {
                        SetConnIcon(pingAttempt < 30 ? 3 : 4);
                        return;
                    }

                    Disconnect();
                }
                else if (ping != null)
                {   // Successful ping.
                    nullPing = false;
                    bool bUpdateData = false;
                    if (!Data.IsConnected || IsChartChangeged(ping.Symbol, (DataPeriods)(int)ping.Period))
                    {   // Disconnected or chart change.
                        pingAttempt = 0;

                        if (JournalShowSystemMessages)
                        {
                            JournalMessage jmsgsys = new JournalMessage(JournalIcons.System, DateTime.Now,
                                ping.Symbol + " " + ping.Period.ToString() + " " + Language.T("Successful ping."));
                            AppendJournalMessage(jmsgsys);
                        }
                        StopTrade();
                        if (!UpdateDataFeedInfo(ping.Symbol, (DataPeriods)(int)ping.Period))
                            return;

                        Data.Bid = ping.Bid;
                        Data.Ask = ping.Ask;
                        Data.InstrProperties.Spread    = ping.Spread;
                        Data.InstrProperties.TickValue = ping.TickValue;

                        Data.IsConnected = true;
                        bUpdateData = true;
                        SetFormText();

                        SetConnIcon(1);
                        SetTradeStrip();

                        if (Configs.PlaySounds)
                            Data.SoundConnect.Play();

                        MT4Bridge.TerminalInfo te = bridge.GetTerminalInfo();
                        string connection = Language.T("Connected to a MetaTrader terminal.");
                        if (te != null)
                        {
                            connection = string.Format(Language.T("Connected to") + " {0} " + Language.T("by") + " {1}", te.TerminalName, te.TerminalCompany);
                            Data.ExpertVersion  = te.ExpertVersion;
                            Data.LibraryVersion = te.LibraryVersion;
                            Data.TerminalName   = te.TerminalName;
                        }

                        SetLblConnectionText(connection);
                        string market = string.Format("{0} {1}", ping.Symbol, ping.Period);
                        SetConnMarketText(market);
                        JournalMessage jmsg = new JournalMessage(JournalIcons.OK, DateTime.Now, market + " " + connection);
                        AppendJournalMessage(jmsg);

                        // Check for reconnection.
                        if (symbolReconnect == Data.Symbol && periodReconnect == Data.Period && accountReconnect == Data.AccountNumber)
                            StartTrade(); // Restart trade.
                    }
                    else if (pingAttempt > 0 && JournalShowSystemMessages)
                    {   // After a wrong ping.
                        pingAttempt = 0;

                        JournalMessage jmsgsys = new JournalMessage(JournalIcons.System, DateTime.Now,
                            ping.Symbol + " " + ping.Period.ToString() + " " + Language.T("Successful ping."));
                        AppendJournalMessage(jmsgsys);
                    }

                    bool isNewPrice = Math.Abs(Data.Bid - ping.Bid) > Data.InstrProperties.Point / 2;
                    DateTime dtPingServerTime = tickServerTime.Add(DateTime.Now - tickLocalTime);

                    string sBid = ping.Bid.ToString(Data.FF);
                    string sAsk = ping.Ask.ToString(Data.FF);
                    SetLblBidAskText(sBid + " / " + sAsk);

                    Data.Bid = ping.Bid;
                    Data.Ask = ping.Ask;
                    Data.InstrProperties.Spread    = ping.Spread;
                    Data.InstrProperties.TickValue = ping.TickValue;

                    int  balanceStatsCount = Data.BalanceStats.Count;
                    bool isAccChanged = Data.SetCurrentAccount(ping.AccountBalance, ping.AccountEquity, ping.AccountProfit, ping.AccountFreeMargin);
                    bool isPosChanged = Data.SetCurrentPosition(ping.PositionTicket, ping.PositionType, ping.PositionLots, ping.PositionOpenPrice,
                                                              ping.PositionStopLoss, ping.PositionTakeProfit, ping.PositionProfit, ping.PositionComment);

                    SetDataAndCalculate(ping.Symbol, ping.Period, dtPingServerTime, isNewPrice, bUpdateData);

                    SetEquityInfoText(string.Format("{0:F2} {1}", ping.AccountEquity, Data.AccountCurrency));
                    ShowCurrentPosition(isPosChanged);

                    if (isAccChanged)
                    {
                        JournalMessage jmsg = new JournalMessage(JournalIcons.Currency, DateTime.Now,
                            string.Format(Language.T("Account Balance") + " {0:F2}, " + Language.T("Equity") + " {1:F2}, " + Language.T("Profit") + ", {2:F2}, " + Language.T("Free Margin") + " {3:F2}",
                            ping.AccountBalance, ping.AccountEquity, ping.AccountProfit, ping.AccountFreeMargin));
                        AppendJournalMessage(jmsg);
                    }

                    if (Data.BalanceStats.Count > balanceStatsCount)
                        UpdateBalanceChart(Data.BalanceStats.ToArray(), Data.EquityStats.ToArray());
                   
                    SetTickInfoText(string.Format("{0} {1} / {2}", ping.Time.ToString("HH:mm:ss"), sBid, sAsk));
                    SetConnIcon(1);

                    // Sends OrderModify on SL/TP errors
                    if (IsWrongStopsExecution())
                        ResendWrongStops();
                }
            }

            return;
        }

        /// <summary>
        /// Stops connection to MT
        /// </summary>
        private void Disconnect()
        {
            nullPing = true;
            pingAttempt = 0;
            if (Data.IsConnected && Configs.PlaySounds)
                Data.SoundDisconnect.Play();

            Data.IsConnected = false;
            StopTrade();

            JournalMessage jmsg = new JournalMessage(JournalIcons.Blocked, DateTime.Now, Language.T("Not Connected"));
            AppendJournalMessage(jmsg);

            Data.Bid = 0;
            Data.Ask = 0;
            Data.SetCurrentAccount(0, 0, 0, 0);
            bool poschanged = Data.SetCurrentPosition(0, -1, 0, 0, 0, 0, 0, "");
            ShowCurrentPosition(poschanged);
            SetEquityInfoText(string.Format("{0} {1}", 0, Data.AccountCurrency));
            UpdateBalanceChart(Data.BalanceStats.ToArray(), Data.EquityStats.ToArray());
            SetTradeStrip();
            SetConnMarketText(Language.T("Not Connected"));
            SetLblConnectionText(Language.T("Not Connected"));
            SetConnIcon(0);
            SetTickInfoText("");
            SetLblSymbolText("");
            SetFormText();
        }

        delegate void DelegateTick(MT4Bridge.TickEventArgs tea);
        /// <summary>
        /// Bridge OnTick 
        /// </summary>
        void Bridge_OnTick(object source, MT4Bridge.TickEventArgs tea)
        {
            lock (lockerTickPing)
            {
                if (pingAttempt > 0 && JournalShowSystemMessages)
                {
                    JournalMessage jmsgsys = new JournalMessage(JournalIcons.System, DateTime.Now,
                        tea.Symbol + " " + tea.Period.ToString() + " " + Language.T("Tick received after an unsuccessful ping."));
                    AppendJournalMessage(jmsgsys);
                }
                pingAttempt = 0;

                if (!Data.IsConnected)
                    return;

                tickLocalTime  = DateTime.Now;
                tickServerTime = tea.Time;
                if (IsChartChangeged(tea.Symbol, (DataPeriods)(int)tea.Period))
                {
                    StopTrade();
                    Data.IsConnected = false;
                    SetFormText();

                    if (Configs.PlaySounds)
                        Data.SoundDisconnect.Play();

                    JournalMessage jmsg = new JournalMessage(JournalIcons.Warning, DateTime.Now,
                        tea.Symbol + " " + tea.Period.ToString() + " " + Language.T("Tick received from a different chart!"));
                    AppendJournalMessage(jmsg);
                    
                    return;
                }

                bool bNewPrice = Math.Abs(Data.Bid - tea.Bid) > Data.InstrProperties.Point / 2;

                Data.Bid = tea.Bid;
                Data.Ask = tea.Ask;
                Data.InstrProperties.Spread    = tea.Spread;
                Data.InstrProperties.TickValue = tea.TickValue;

                Data.SetTick(tea.Bid);
                
                int  balanceStatsCount = Data.BalanceStats.Count;
                bool isAccChanged = Data.SetCurrentAccount(tea.AccountBalance, tea.AccountEquity, tea.AccountProfit, tea.AccountFreeMargin);
                bool isPosChanged = Data.SetCurrentPosition(tea.PositionTicket, tea.PositionType, tea.PositionLots, tea.PositionOpenPrice,
                                                          tea.PositionStopLoss, tea.PositionTakeProfit, tea.PositionProfit, tea.PositionComment);

                bool updateData = true;
                SetDataAndCalculate(tea.Symbol, tea.Period, tea.Time, bNewPrice, updateData);

                string bidText = tea.Bid.ToString(Data.FF);
                string askText = tea.Ask.ToString(Data.FF);
                SetLblBidAskText(bidText + " / " + askText);

                // Tick data label
                if (JournalShowTicks)
                {
                    string tickInfo = string.Format("{0} {1} {2} {3} / {4}", tea.Symbol, tea.Period, tea.Time.ToString("HH:mm:ss"), bidText, askText);
                    JournalMessage jmsg = new JournalMessage(JournalIcons.Globe, DateTime.Now, tickInfo);
                    AppendJournalMessage(jmsg);
                }
                
                UpdateTickChart(Data.InstrProperties.Point, Data.ListTicks.ToArray());
                SetEquityInfoText(string.Format("{0:F2} {1}", tea.AccountEquity, Data.AccountCurrency));
                ShowCurrentPosition(isPosChanged);

                if (isAccChanged)
                {
                    JournalMessage jmsg = new JournalMessage(JournalIcons.Currency, DateTime.Now,
                        string.Format(Language.T("Account Balance") + " {0:F2}, " + Language.T("Equity") + " {1:F2}, " + Language.T("Profit") + ", {2:F2}, " + Language.T("Free Margin") + " {3:F2}",
                        tea.AccountBalance, tea.AccountEquity, tea.AccountProfit, tea.AccountFreeMargin));
                    AppendJournalMessage(jmsg);
                }

                if (Data.BalanceStats.Count > balanceStatsCount)
                    UpdateBalanceChart(Data.BalanceStats.ToArray(), Data.EquityStats.ToArray());
                
                SetTickInfoText(string.Format("{0} {1} / {2}", tea.Time.ToString("HH:mm:ss"), bidText, askText));
                SetConnIcon(2);

                // Sends OrderModify on SL/TP errors
                if (IsWrongStopsExecution())
                    ResendWrongStops();
            }

            return;
        }

        /// <summary>
        /// Check if the incoming data is from the same chart.
        /// </summary>
        bool IsChartChangeged(string symbol, DataPeriods period)
        {
            if (!Data.IsConnected)
                return true;

            if (Data.Symbol != symbol || Data.Period != period)
                return true;

            return false;
        }

        object lockerDataFeed = new object();
        /// <summary>
        /// Sets the instrument's properties after connecting;
        /// </summary>
        bool UpdateDataFeedInfo(string symbol, DataPeriods period)
        {
            lock (lockerDataFeed)
            {
                Data.ResetBidAsk();
                Data.ResetAccountStats();
                Data.ResetPositionStats();
                Data.ResetBarStats();
                Data.ResetTicks();

                // Reads market info from the chart
                MT4Bridge.MarketInfo marketInfo = bridge.GetMarketInfoAll(symbol);
                if (marketInfo == null)
                {
                    if (JournalShowSystemMessages)
                    {
                        JournalMessage jmsgsys = new JournalMessage(JournalIcons.System, DateTime.Now,
                            symbol + " " + (MT4Bridge.PeriodType)(int)period + " " + Language.T("Cannot update market info."));
                        AppendJournalMessage(jmsgsys);
                    }
                    return false;
                }

                // Sets instrument properties
                Data.Period                         = period;
                Data.InstrProperties.Symbol         = symbol;
                Data.InstrProperties.LotSize        = (int)marketInfo.ModeLotSize;
                Data.InstrProperties.MinLot         = marketInfo.ModeMinLot;
                Data.InstrProperties.MaxLot         = marketInfo.ModeMaxLot;
                Data.InstrProperties.LotStep        = marketInfo.ModeLotStep;
                Data.InstrProperties.Digits         = (int)marketInfo.ModeDigits;
                Data.InstrProperties.Spread         = marketInfo.ModeSpread;
                Data.InstrProperties.SwapLong       = marketInfo.ModeSwapLong;
                Data.InstrProperties.SwapShort      = marketInfo.ModeSwapShort;
                Data.InstrProperties.TickValue      = marketInfo.ModeTickValue;
                Data.InstrProperties.StopLevel      = marketInfo.ModeStopLevel;
                Data.InstrProperties.MarginRequired = marketInfo.ModeMarginRequired;

                SetNumUpDownLots(marketInfo.ModeMinLot, marketInfo.ModeLotStep, marketInfo.ModeMaxLot);

                // Sets Market Info
                string[] values = new string[] {
                    symbol,
                    Data.DataPeriodToString(period),
                    marketInfo.ModeLotSize.ToString(),
                    marketInfo.ModePoint.ToString("F" + marketInfo.ModeDigits.ToString()),
                    marketInfo.ModeSpread.ToString(),
                    marketInfo.ModeSwapLong.ToString(),
                    marketInfo.ModeSwapShort.ToString()};
                UpdateStatusPageMarketInfo(values);

                MT4Bridge.Bars bars = bridge.GetBars(symbol, (MT4Bridge.PeriodType)(int)period);
                if (bars == null)
                {
                    if (JournalShowSystemMessages)
                    {
                        Data.SoundError.Play();
                        JournalMessage jmsgsys = new JournalMessage(JournalIcons.System, DateTime.Now,
                            symbol + " " + (MT4Bridge.PeriodType)(int)period + " " + Language.T("Cannot receive bars!"));
                        AppendJournalMessage(jmsgsys);
                    }
                    return false;
                }
                if (bars.Count < MaxBarsCount((int)period))
                {
                    if (JournalShowSystemMessages)
                    {
                        Data.SoundError.Play();
                        JournalMessage jmsg = new JournalMessage(JournalIcons.Error, DateTime.Now,
                            symbol + " " + (MT4Bridge.PeriodType)(int)period + " " + Language.T("Cannot receive enough bars!"));
                        AppendJournalMessage(jmsg);
                    }
                    return false;
                }
                if (JournalShowSystemMessages)
                {
                    JournalMessage jmsgsys = new JournalMessage(JournalIcons.System, DateTime.Now,
                        symbol + " " + (MT4Bridge.PeriodType)(int)period + " " + Language.T("Market data updated, bars downloaded."));
                    AppendJournalMessage(jmsgsys);
                }

                // Account Information.
                MT4Bridge.AccountInfo account = bridge.GetAccountInfo();
                if (account == null)
                {
                    if (JournalShowSystemMessages)
                    {
                        Data.SoundError.Play();
                        JournalMessage jmsg = new JournalMessage(JournalIcons.Error, DateTime.Now,
                            symbol + " " + (MT4Bridge.PeriodType)(int)period + " " + Language.T("Cannot receive account information!"));
                        AppendJournalMessage(jmsg);
                    }
                    return false;
                }
                if (JournalShowSystemMessages)
                {
                    JournalMessage jmsgsys = new JournalMessage(JournalIcons.System, DateTime.Now,
                        symbol + " " + (MT4Bridge.PeriodType)(int)period + " " + Language.T("Account information received."));
                    AppendJournalMessage(jmsgsys);
                }
                Data.AccountName     = account.Name;
                Data.AccountCurrency = account.Currency;
                Data.SetCurrentAccount(account.Balance, account.Equity, account.Profit, account.FreeMargin);
                UpdateBalanceChart(Data.BalanceStats.ToArray(), Data.EquityStats.ToArray());

                SetTradeStrip();
                SetLblSymbolText(symbol);
            }

            return true;
        }

        /// <summary>
        /// Copies data to Data and calculates.
        /// </summary>
        void SetDataAndCalculate(string symbol, MT4Bridge.PeriodType period, DateTime time, bool isPriceChange, bool isUpdateData)
        {
            lock (lockerDataFeed)
            {
                bool isUpdateChart = isUpdateData;

                MT4Bridge.Bars bars = bridge.GetBars(symbol, period);

                if (bars == null && JournalShowSystemMessages)
                {
                    isSetRootDataError = true;
                    Data.SoundError.Play();
                    JournalMessage jmsg = new JournalMessage(JournalIcons.Error, DateTime.Now,
                        symbol + " " + period.ToString() + " " + Language.T("Cannot receive bars!"));
                    AppendJournalMessage(jmsg);
                    return;
                }
                if (bars.Count < MaxBarsCount((int)period) && JournalShowSystemMessages)
                {
                    isSetRootDataError = true;
                    Data.SoundError.Play();
                    JournalMessage jmsg = new JournalMessage(JournalIcons.Error, DateTime.Now,
                        symbol + " " + period.ToString() + " " + Language.T("Cannot receive enough bars!"));
                    AppendJournalMessage(jmsg);
                    return;
                }
                if (isSetRootDataError && JournalShowSystemMessages)
                {
                    isSetRootDataError = false;
                    JournalMessage jmsg = new JournalMessage(JournalIcons.Information, DateTime.Now,
                        symbol + " " + period.ToString() + " " + Language.T("Enough bars received!"));
                    AppendJournalMessage(jmsg);
                }

                int countBars = bars.Count;

                if (countBars < 400)
                    return;

                if (Data.Bars != countBars || Data.Time[countBars - 1] != bars.Time[countBars - 1] ||
                    Data.Volume[countBars - 1] != bars.Volume[countBars - 1] || Data.Close[countBars - 1] != bars.Close[countBars - 1])
                {
                    if (Data.Bars == countBars && Data.Time[countBars - 1] == bars.Time[countBars - 1] && Data.Time[countBars - 10] == bars.Time[countBars - 10])
                    {   // Update the last bar only.
                        Data.Open  [countBars - 1] = bars.Open  [countBars - 1];
                        Data.High  [countBars - 1] = bars.High  [countBars - 1];
                        Data.Low   [countBars - 1] = bars.Low   [countBars - 1];
                        Data.Close [countBars - 1] = bars.Close [countBars - 1];
                        Data.Volume[countBars - 1] = bars.Volume[countBars - 1];
                    }
                    else
                    {   // Update all the bars.
                        Data.Bars   = countBars;
                        Data.Time   = new DateTime[countBars];
                        Data.Open   = new double[countBars];
                        Data.High   = new double[countBars];
                        Data.Low    = new double[countBars];
                        Data.Close  = new double[countBars];
                        Data.Volume = new int[countBars];
                        bars.Time.CopyTo(Data.Time, 0);
                        bars.Open.CopyTo(Data.Open, 0);
                        bars.High.CopyTo(Data.High, 0);
                        bars.Low.CopyTo(Data.Low, 0);
                        bars.Close.CopyTo(Data.Close, 0);
                        bars.Volume.CopyTo(Data.Volume, 0);
                    }

                    // Calculate the strategy indicators.
                    CalculateStrategy(true);
                    isUpdateChart = true;
                }

                bool isBarChanged = IsBarChanged(Data.Time[Data.Bars - 1]);

                if (isTrading)
                {
                    TickType tickType = GetTickType((DataPeriods)(int)period, Data.Time[Data.Bars - 1], time, Data.Volume[Data.Bars - 1]);

                    if (tickType == TickType.Close || isPriceChange || isBarChanged)
                    {
                        if (JournalShowSystemMessages && tickType != TickType.Regular)
                        {
                            JournalIcons icon = JournalIcons.Warning;
                            string text = string.Empty;
                            if (tickType == TickType.Open)
                            {
                                icon = JournalIcons.BarOpen;
                                text = Language.T("A Bar Open event!");
                            }
                            else if (tickType == TickType.Close)
                            {
                                icon = JournalIcons.BarClose;
                                text = Language.T("A Bar Close event!");
                            }
                            else if (tickType == TickType.AfterClose)
                            {
                                icon = JournalIcons.Warning;
                                text = Language.T("A new tick arrived after a Bar Close event!");
                            }
                            JournalMessage jmsg = new JournalMessage(icon, DateTime.Now, symbol + " " + Data.PeriodMTStr + " " + time.ToString("HH:mm:ss") + " " + text);
                            AppendJournalMessage(jmsg);
                        }

                        if (isBarChanged && tickType == TickType.Regular)
                        {
                            if (JournalShowSystemMessages)
                            {
                                JournalMessage jmsg = new JournalMessage(JournalIcons.Warning, DateTime.Now, symbol + " " +
                                    Data.PeriodMTStr + " " + time.ToString("HH:mm:ss") + " A Bar Changed event!");
                                AppendJournalMessage(jmsg);
                            }

                            tickType = TickType.Open;
                        }

                        if (tickType == TickType.Open && barOpenTimeForLastCloseEvent == Data.Time[Data.Bars - 3])
                        {
                            if (JournalShowSystemMessages)
                            {
                                JournalMessage jmsg = new JournalMessage(JournalIcons.Warning, DateTime.Now, symbol + " " +
                                    Data.PeriodMTStr + " " + time.ToString("HH:mm:ss") + " A secondary Bar Close event!");
                                AppendJournalMessage(jmsg);
                            }
                            tickType = TickType.OpenClose;
                        }

                        CalculateTrade(tickType);
                        isUpdateChart = true;

                        if (tickType == TickType.Close || tickType == TickType.OpenClose)
                            barOpenTimeForLastCloseEvent = Data.Time[Data.Bars - 1];
                    }
                }

                if (isUpdateChart)
                    UpdateChart();
            }

            return;
        }

        /// <summary>
        /// Shows if the bar was changed.
        /// </summary>
        bool IsBarChanged(DateTime open)
        {
            bool barChanged = barOpenTimeForLastOpenTick != open;
            barOpenTimeForLastOpenTick = open;

            return barChanged;
        }

        /// <summary>
        /// Gets the tick type depending on its time.
        /// </summary>
        TickType GetTickType(DataPeriods period, DateTime open, DateTime time, int volume)
        {
            TickType tick = TickType.Regular;
            bool isOpen  = volume == 1 && barOpenTimeForLastOpenTick != open;
            bool isClose = (open.AddMinutes((int)period) - time) < TimeSpan.FromSeconds(Configs.BarCloseAdvance);

            if (isOpen)
            {
                barOpenTimeForLastCloseTick = DateTime.MinValue;
                tick = TickType.Open;
            }

            if (isClose)
            {
                if (barOpenTimeForLastCloseTick == open)
                {
                    tick = TickType.AfterClose;
                }
                else
                {
                    barOpenTimeForLastCloseTick = open;
                    tick = isOpen ? TickType.OpenClose : TickType.Close;
                }
            }

            return tick;
        }

        /// <summary>
        /// Returns the count of chart's bars.
        /// </summary>
        int MaxBarsCount(int period)
        {
            return Math.Max(2 * 1440 / period + 10, Configs.MinChartBars);
        }

        /// <summary>
        /// Starts the trade.
        /// </summary>
        void StartTrade()
        {
            isTrading = true;

            // Resets trade global variables.
            InitTrade();

            JournalMessage msg = new JournalMessage(JournalIcons.StartTrading, DateTime.Now, Language.T("Automatic trade started."));
            AppendJournalMessage(msg);

            symbolReconnect  = Data.Symbol;
            periodReconnect  = Data.Period;
            accountReconnect = Data.AccountNumber;
            barOpenTimeForLastCloseEvent = Data.Time[Data.Bars - 1];

            SetTradeStrip();
            
            return;
        }

        /// <summary>
        /// Stops the trade.
        /// </summary>
        void StopTrade()
        {
            if (!isTrading)
                return;

            isTrading = false;

            DeinitTrade();

            JournalMessage msg = new JournalMessage(JournalIcons.StopTrading, DateTime.Now, Language.T("Automatic trade stopped."));
            AppendJournalMessage(msg);
            SetTradeStrip();

            return;
        }

        /// <summary>
        /// Shows current position on the status bar.
        /// </summary>
        void ShowCurrentPosition(bool showInJournal)
        {
            if (!Data.IsConnected)
            {
                SetPositionInfoText(null, String.Empty);
                return;
            }

            string format = Data.FF;
            string text = Language.T("Square");
            JournalIcons  icon  = JournalIcons.PosSquare;
            Image img = Properties.Resources.pos_square;
            if (Data.PositionTicket > 0)
            {
                img  = (Data.PositionType == 0 ? Properties.Resources.pos_buy : Properties.Resources.pos_sell);
                icon = (Data.PositionType == 0 ? JournalIcons.PosBuy : JournalIcons.PosSell);
                text = string.Format((Data.PositionType == 0 ? Language.T("Long") : Language.T("Short")) + " {0} " +
                        (Data.PositionLots == 1 ? Language.T("lot") : Language.T("lots")) + " " + Language.T("at") + " {1}, " +
                        Language.T("Stop Loss") + " {2}, " + Language.T("Take Profit") + " {3}, " + Language.T("Profit") +
                        " {4} " + Data.AccountCurrency, Data.PositionLots, Data.PositionOpenPrice.ToString(format),
                        Data.PositionStopLoss.ToString(format), Data.PositionTakeProfit.ToString(format), Data.PositionProfit.ToString("F2"));
            }

            SetPositionInfoText(img, text);

            if (showInJournal)
            {
                JournalMessage jmsg = new JournalMessage(icon, DateTime.Now, string.Format(Data.Symbol + " " + Data.PeriodMTStr + " " + text));
                AppendJournalMessage(jmsg);
            }
                
            return;
        }

        /// <summary>
        /// Sets the trade button's icon and text.
        /// </summary>
        void SetTradeStrip()
        {
            string connectText = Data.IsConnected ?
                Language.T("Connected to") + " " + Data.Symbol + " " + Data.PeriodMTStr :
                Language.T("Not Connected");

            SetTradeStripThreadSafely(connectText, Data.IsConnected, isTrading);
        }

        delegate void SetTradeStripDelegate(string connectText, bool isConnectedNow, bool isTradingNow);
        void SetTradeStripThreadSafely(string connectText, bool isConnectedNow, bool isTradingNow)
        {
            if (tsTradeControl.InvokeRequired)
            {
                tsTradeControl.BeginInvoke(new SetTradeStripDelegate(SetTradeStripThreadSafely), new object[] { connectText, isConnectedNow, isTradingNow });
            }
            else
            {
                // Sets lable Connection
                tslblConnection.Text = connectText;

                // Sets button Automatic Execution
                if (isTradingNow)
                {
                    tsbtnTrading.Image = Properties.Resources.stop;
                    tsbtnTrading.Text  = Language.T("Stop Automatic Execution");
                }
                else
                {
                    tsbtnTrading.Image = Properties.Resources.play;
                    tsbtnTrading.Text  = Language.T("Start Automatic Execution");
                }
                tsbtnTrading.Enabled = isConnectedNow;
                tsbtnConnectionHelp.Visible = !isConnectedNow;
            }

            return;

        }

        /// <summary>
        /// Menu Multiple Instances Changed
        /// </summary>
        protected override void MenuMultipleInstances_OnClick(object sender, EventArgs e)
        {
            Configs.MultipleInstances = ((ToolStripMenuItem)sender).Checked;
            if (Configs.MultipleInstances)
            {
                Configs.SaveConfigs();
                Disconnect();
                DeinitDataFeed();

                tsTradeControl.SuspendLayout();
                tsbtnChangeID.Visible   = false;
                tslblConnection.Visible = false;
                tsbtnTrading.Visible    = false;

                tslblConnectionID.Visible = true;
                tstbxConnectionID.Visible = true;
                tsTradeControl.ResumeLayout();
            }
            else
            {
                Configs.SaveConfigs();
                Disconnect();
                DeinitDataFeed();
                Data.ConnectionID = 0;

                tsTradeControl.SuspendLayout();
                tsbtnChangeID.Visible   = false;
                tslblConnection.Visible = true;
                tsbtnTrading.Visible    = true;

                tslblConnectionID.Visible = false;
                tstbxConnectionID.Visible = false;
                tsTradeControl.ResumeLayout();

                InitDataFeed();
            }

            symbolReconnect = "";

            return;
        }

        /// <summary>
        /// ID changed.
        /// </summary>
        protected override void TstbxConnectionID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                try
                {
                    int id = int.Parse(tstbxConnectionID.Text);
                    if (id >= 0 && id <= 10000)
                    {
                        Data.ConnectionID = id;
                        Disconnect();

                        tsTradeControl.SuspendLayout();
                        tsbtnChangeID.Text        = "ID = " + tstbxConnectionID.Text;
                        tsbtnChangeID.Visible     = true;
                        tslblConnection.Visible   = true;
                        tsbtnTrading.Visible      = true;

                        tslblConnectionID.Visible = false;
                        tstbxConnectionID.Visible = false;
                        tsTradeControl.ResumeLayout();

                        InitDataFeed();
                    }
                    else
                        tstbxConnectionID.Text = "";

                }
                catch
                {
                    tstbxConnectionID.Text = "";
                }
            }
        }

        /// <summary>
        /// Button Change ID clicked.
        /// </summary>
        protected override void TsbtChangeID_Click(object sender, EventArgs e)
        {
            Disconnect();
            DeinitDataFeed();

            tsTradeControl.SuspendLayout();
            tsbtnChangeID.Visible   = false;
            tslblConnection.Visible = false;
            tsbtnTrading.Visible    = false;

            tslblConnectionID.Visible = true;
            tstbxConnectionID.Visible = true;
            tsTradeControl.ResumeLayout();
        }

        protected override void TsbtTrading_Click(object sender, EventArgs e)
        {
            if (isTrading)
            {
                StopTrade();

                symbolReconnect = "";
                periodReconnect = DataPeriods.week;
                accountReconnect = 0;
            }
            else
            {
                StartTrade();
            }
        }

        protected override void BtnShowAccountInfo_Click(object sender, EventArgs e)
        {
            if (!Data.IsConnected)
            {
                SetBarDataText("   " + Language.T("Not Connected"));
                return;
            }

            MT4Bridge.AccountInfo ai = bridge.GetAccountInfo();
            if (ai == null)
            {
                SetBarDataText("   " + Language.T("Cannot receive account information!"));
                return;
            }

            string[] asParams = new string[] {
                "Name",
                "Number",
                "Company",
                "Server",
                "Currency",
                "Leverage",
                "Balance",
                "Equity",
                "Profit",
                "Credit",
                "Margin",
                "Free margin mode",
                "Free margin",
                "Stop out mode",
                "Stop out level",
            };

            string[] asValues = new string[] {
                ai.Name,
                ai.Number.ToString(),
                ai.Company,
                ai.Server,
                ai.Currency,
                ai.Leverage.ToString(),
                ai.Balance.ToString(),
                ai.Equity.ToString(),
                ai.Profit.ToString(),
                ai.Credit.ToString(),
                ai.Margin.ToString(),
                ai.FreeMarginMode.ToString(),
                ai.FreeMargin.ToString(),
                ai.StopOutMode.ToString(),
                ai.StopOutLevel.ToString(),
            };

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < asParams.Length; i++)
            {
                sb.AppendLine(string.Format("      {0,-25} {1}", asParams[i], asValues[i]));
            }
            SetBarDataText(sb.ToString());

            return;
        }

        protected override void BtnShowMarketInfo_Click(object sender, EventArgs e)
        {
            if (!Data.IsConnected)
            {
                SetBarDataText("   " + Language.T("Not Connected"));
                return;
            }

            MT4Bridge.MarketInfo mi = bridge.GetMarketInfoAll(Data.Symbol);
            if (mi == null)
            {
                SetBarDataText("   " + Language.T("Cannot update market info."));
                return;
            }

            string[] asMIParams = new string[] {
                "Point",
                "Digit",
                "Spread",
                "Stop Level",
                "Lot Size",
                "Tick Value",
                "Tick Size",
                "Swap Long",
                "Swap Short",
                "Starting",
                "Expiration",
                "Trade Allowed",
                "Min Lot",
                "Lot Step",
                "Max Lot",
                "Swap Type",
                "Profit Calc Mode",
                "Margin Calc Mode",
                "Margin Init",
                "Margin Maintenance",
                "Margin Hedged",
                "Margin Required",
                "Freeze Level"};

            string[] asMIValues = new string[] {
                mi.ModePoint.ToString("F" + mi.ModeDigits.ToString()),
                mi.ModeDigits.ToString(),
                mi.ModeSpread.ToString(),
                mi.ModeStopLevel.ToString(),
                mi.ModeLotSize.ToString(),
                mi.ModeTickValue.ToString(),
                mi.ModeTickSize.ToString("F" + mi.ModeDigits.ToString()),
                mi.ModeSwapLong.ToString(),
                mi.ModeSwapShort.ToString(),
                mi.ModeStarting.ToString(),
                mi.ModeExpiration.ToString(),
                mi.ModeTradeAllowed.ToString(),
                mi.ModeMinLot.ToString(),
                mi.ModeLotStep.ToString(),
                mi.ModeMaxLot.ToString(),
                mi.ModeSwapType.ToString(),
                mi.ModeProfitCalcMode.ToString(),
                mi.ModeMarginCalcMode.ToString(),
                mi.ModeMarginInit.ToString(),
                mi.ModeMarginMaintenance.ToString(),
                mi.ModeMarginHedged.ToString(),
                mi.ModeMarginRequired.ToString(),
                mi.ModeFreezeLevel.ToString()};

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < asMIParams.Length; i++)
            {
                sb.AppendLine(string.Format("      {0,-20} {1}", asMIParams[i], asMIValues[i]));
            }
            SetBarDataText(sb.ToString());

            // Sets Market Info
            string[] asValue = new string[] {
                    Data.Symbol,
                    Data.DataPeriodToString(Data.Period),
                    mi.ModeLotSize.ToString(),
                    mi.ModePoint.ToString("F" + mi.ModeDigits.ToString()),
                    mi.ModeSpread.ToString(),
                    mi.ModeSwapLong.ToString(),
                    mi.ModeSwapShort.ToString()};
            UpdateStatusPageMarketInfo(asValue);

            return;
        }

        protected override void BtnShowBars_Click(object sender, EventArgs e)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(Data.Bars + 2);
            
            sb.AppendLine("                  " + 
                Data.Symbol + " " + Data.PeriodMTStr + " " + Data.Time[Data.Bars - 1].ToString());
            sb.AppendLine(string.Format("  {0,-5} {1,-16} {2,-8} {3,-8} {4,-8} {5,-8} {6}",
                   "No", "Bar open time", "Open", "High", "Low", "Close", "Volume"));
            
            for (int i = 0; i < Data.Bars; i++)
            {
                sb.AppendLine(string.Format("  {0,-5} {1,-16} {2,-8} {3,-8} {4,-8} {5,-8} {6}",
                   i + 1, Data.Time[i].ToString(Data.DF) + " " + Data.Time[i].ToString("HH:mm"),
                   Data.Open[i], Data.High[i], Data.Low[i], Data.Close[i], Data.Volume[i]));
            }

            SetBarDataText(sb.ToString());

            return;
        }
    }
}
