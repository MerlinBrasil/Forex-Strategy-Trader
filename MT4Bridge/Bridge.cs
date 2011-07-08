// Bridge
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.IO;

namespace MT4Bridge
{
    public class Bridge
    {
        Server server;
        Client client;
        internal BarsManager barsManager = new BarsManager();

        int code;
        public int LastError { get { return code; } }

        ~Bridge()
        {
            Stop();
        }

        public void Start(int id)
        {
            server = new Server(this, id);
            client = new Client(this, id);
        }

        public void Stop()
        {
            if (server != null) {
                try {
                    server.Stop();
                } finally {
                    server = null;
                }
            }
        }

        const string LOG_FILENAME = "bridge.log";
        static bool isWriteLog = true;
        /// <summary>
        /// Sets if Bridge writes log file.
        /// </summary>
        public bool WriteLog { set { isWriteLog = value; } }

        /// <summary>
        /// Writes a massage to the log file.
        /// </summary>
        internal static void Log(string message)
        {
            if (!isWriteLog)
                return;
            
            lock (typeof(Bridge))
            {
                try
                {
                    using (StreamWriter sw = File.AppendText(LOG_FILENAME))
                        sw.WriteLine(DateTime.Now.ToString() + " - " + message);
                }
                catch { }
            }

            return;
        }

        public PingInfo GetPingInfo()
        {
            PingInfo ping = client.Ping();
            return ping;
        }

        public delegate void TickEventHandler(object source, TickEventArgs e);
        public event TickEventHandler OnTick;
        internal void Tick(string symbol, PeriodType period, DateTime bartime, DateTime time, double bid, double ask, int spread, double tickvalue,
                double accountBalance, double accountEquity, double accountProfit, double accountFreeMargin,
                int positionTicket, int positionType, double positionLots, double positionOpenPrice,
                double positionStopLoss, double positionTakeProfit, double positionProfit, string positionComment)
        {
            if (OnTick != null)
            {
                TickEventArgs tickea = new TickEventArgs(symbol, period, bartime, time, bid, ask, spread, tickvalue,
                    accountBalance, accountEquity, accountProfit, accountFreeMargin,
                    positionTicket, positionType, positionLots, positionOpenPrice,
                    positionStopLoss, positionTakeProfit, positionProfit, positionComment);

                OnTick.BeginInvoke(this, tickea, null, null);
            }
        }

        public void ResetBarsManager()
        {
            barsManager = new BarsManager();
        }

        public Bars GetBars(string symbol, PeriodType period)
        {
            return barsManager.GetBars(symbol, period, client);
        }

        public SymbolInfo GetSymbolInfo(string symbol)
        {
            return client.Symbol(symbol);
        }

        public AccountInfo GetAccountInfo()
        {
            return client.Account();
        }

        public double GetMarketInfo(string symbol, int mode)
        {
            return client.MarketInfo(symbol, mode);
        }

        public MarketInfo GetMarketInfoAll(string symbol)
        {
            return client.MarketInfoAll(symbol);
        }

        public TerminalInfo GetTerminalInfo()
        {
            return client.Terminal();
        }

        public int[] Orders() { return Orders(null); }
        public int[] Orders(string symbol)
        {
            return client.Orders(symbol);
        }

        public OrderInfo OrderInfo(int ticket)
        {
            return client.OrderInfo(ticket);
        }

        bool SaveCode(Response response)
        {
            code = response.Code;
            return response.OK;
        }

        public int OrderSend(string symbol, OrderType type, double lots, double price, int slippage, double stoploss, double takeprofit, string parameters)
        {
            return OrderSend(symbol, type, lots, price, slippage, stoploss, takeprofit, 0, DateTime.MinValue, parameters);
        }
        public int OrderSend(string symbol, OrderType type, double lots, double price, int slippage, double stoploss, double takeprofit, int magic, DateTime expire, string parameters)
        {
            Response rc = client.OrderSend(symbol, type, lots, price, slippage, stoploss, takeprofit, magic, expire, parameters);
            return SaveCode(rc) ? rc.Code : -1;
        }

        public bool OrderModify(int ticket, double price, double stoploss, double takeprofit, string parameters)
        {
            return OrderModify(ticket, price, stoploss, takeprofit, DateTime.MinValue, parameters);
        }
        public bool OrderModify(int ticket, double price, double stoploss, double takeprofit, DateTime expire, string parameters)
        {
            return SaveCode(client.OrderModify(ticket, price, stoploss, takeprofit, expire, parameters));
        }

        public bool OrderClose(int ticket, double lots, double price, int slippage)
        {
            Response rc = client.OrderClose(ticket, lots, price, slippage);
            return SaveCode(rc);
        }

        public bool OrderDelete(int ticket)
        {
            return SaveCode(client.OrderDelete(ticket));
        }
    }
}
