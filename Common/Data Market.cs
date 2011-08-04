// Data Market
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;

namespace Forex_Strategy_Trader
{
    /// <summary>
    ///  Base class containing the data.
    /// </summary>
    public static partial class Data
    {
        static bool isConnected = false; // Shows if the program is connected to data feed server.
        public static bool IsConnected { get { return isConnected; } set { isConnected = value; } }

        static int connectionID = 0;
        public static int ConnectionID { get { return connectionID; } set { connectionID = value; } }

        static string terminalName   = "MetaTrader";
        static string expertVersion  = "unknown"; // 'MT4-FST Expert.mql' version.
        static string libraryVersion = "unknown"; // 'MT4-FST Library.dll' version.
        public static string TerminalName   { get { return terminalName;   } set { terminalName   = value; } }
        public static string ExpertVersion  { get { return expertVersion;  } set { expertVersion  = value; } }
        public static string LibraryVersion { get { return libraryVersion; } set { libraryVersion = value; } }

        // The current instrument's properties.
        static Instrument_Properties instrProperties;
        static DataPeriods period; // Time frame

        public static Instrument_Properties InstrProperties { get { return instrProperties; } set { instrProperties = value; } }
        public static string      Symbol { get { return instrProperties.Symbol; } }
        public static DataPeriods Period { get { return period; } set { period = value; } }

        // Bar's data.
        static int        bars;     // Count of bars
        static DateTime[] adtTime;  // The bar's time of opening
        static double[]   adOpen;   // Price Open
        static double[]   adHigh;   // Price High
        static double[]   adLow;    // Price Low
        static double[]   adClose;  // Price Close
        static int[]      aiVolume; // Volume

        public static int        Bars   { get { return bars;     } set { bars     = value; } }
        public static DateTime[] Time   { get { return adtTime;  } set { adtTime  = value; } }
        public static double[]   Open   { get { return adOpen;   } set { adOpen   = value; } }
        public static double[]   High   { get { return adHigh;   } set { adHigh   = value; } }
        public static double[]   Low    { get { return adLow;    } set { adLow    = value; } }
        public static double[]   Close  { get { return adClose;  } set { adClose  = value; } }
        public static int[]      Volume { get { return aiVolume; } set { aiVolume = value; } }

        // Bid Ask prices.
        static double bid;
        static double ask;
        static double oldbid;
        static double oldask;
        public static double Bid    { get { return bid; } set { oldbid = bid; bid = value; } }
        public static double Ask    { get { return ask; } set { oldask = ask; ask = value; } }
        public static double OldBid { get { return oldbid; } }
        public static double OldAsk { get { return oldask; } }
        public static void ResetBidAsk()
        {
            oldbid = oldask = bid = ask = 0;
        }
  
        // Ticks
        static List<double> listTicks = new List<double>();
        public static List<double> ListTicks { get { return listTicks; } }
        public static void SetTick(double tick)
        {
            listTicks.Add(tick);
            if (listTicks.Count > 60)
                listTicks.RemoveRange(0, 1);
        }
        public static void ResetTicks()
        {
            listTicks = new List<double>();
        }

        // Account condition.
        static string accountName;
        static int    accountNumber;
        static bool   isDemoAccount;
        static string accountCurrency;
        static double accountBalance;
        static double accountEquity;
        static double accountProfit;
        static double accountFreeMargin;

        public static string AccountName        { get { return accountName;       } set { accountName     = value; } }
        public static int    AccountNumber      { get { return accountNumber;     } set { accountNumber   = value; } }
        public static bool   IsDemoAccount      { get { return isDemoAccount;     } set { isDemoAccount   = value; } }
        public static string AccountCurrency    { get { return accountCurrency;   } set { accountCurrency = value; } }
        public static double AccountBalance     { get { return accountBalance;    } }
        public static double AccountEquity      { get { return accountEquity;     } }
        public static double AccountProfit      { get { return accountProfit;     } }
        public static double AccountFreeMargin  { get { return accountFreeMargin; } }

        static int BALANCE_LENGHT = 2000;
        static Balance_Chart_Unit[] balanceData = new Balance_Chart_Unit[BALANCE_LENGHT];
        public static Balance_Chart_Unit[] BalanceData { get { return Data.balanceData; } }
        static int balanceDataPoints = 0;
        public static int BalanceDataPoints { get { return Data.balanceDataPoints; } }
        static bool balanceDataChganged = false;
        public static bool IsBalanceDataChganged { get { return balanceDataChganged; } }

        public static bool SetCurrentAccount(DateTime time, double balance, double equity, double profit, double freeMargin)
        {
            bool balanceChanged = false;
            bool equityChanged  = false;
            balanceDataChganged = false;
            if (Math.Abs(accountBalance - balance) > 0.01) balanceChanged = true;
            if (Math.Abs(accountEquity  - equity)  > 0.01) equityChanged  = true;

            accountBalance    = balance;
            accountEquity     = equity;
            accountProfit     = profit;
            accountFreeMargin = freeMargin;

            if (balance > 0.01 && (equityChanged || balanceChanged))
            {
                Balance_Chart_Unit chartUnit = new Balance_Chart_Unit();
                chartUnit.Time    = time;
                chartUnit.Balance = balance;
                chartUnit.Equity  = equity;

                if (balanceDataPoints == 0)
                {
                    balanceData[balanceDataPoints] = chartUnit;
                    balanceDataPoints++;
                }

                if (balanceDataPoints == BALANCE_LENGHT)
                {
                    for (int i = 0; i < BALANCE_LENGHT - 1; i++)
                        balanceData[i] = balanceData[i + 1];
                    balanceDataPoints = BALANCE_LENGHT - 1;
                }

                if (balanceDataPoints < BALANCE_LENGHT)
                {
                    balanceData[balanceDataPoints] = chartUnit;
                    balanceDataPoints++;
                }

                balanceDataChganged = true;
            }

            return balanceChanged;
        }
        public static void ResetAccountStats()
        {
            accountBalance    = 0;
            accountEquity     = 0;
            accountProfit     = 0;
            accountFreeMargin = 0;

            balanceDataPoints = 0;
            balanceData = new Balance_Chart_Unit[BALANCE_LENGHT];
        }

        // Position parameters.
        static int    positionTicket     = 0;
        static int    positionType       = -1;
        static double positionLots       = 0;
        static double positionOpenPrice  = 0;
        static double positionStopLoss   = 0;
        static double positionTakeProfit = 0;
        static double positionProfit     = 0;
        static string positionComment    = "";

        public static int    PositionTicket     { get { return positionTicket;     } }
        public static int    PositionType       { get { return positionType;       } }
        public static double PositionLots       { get { return positionLots;       } }
        public static double PositionOpenPrice  { get { return positionOpenPrice;  } }
        public static double PositionStopLoss   { get { return positionStopLoss;   } }
        public static double PositionTakeProfit { get { return positionTakeProfit; } }
        public static double PositionProfit     { get { return positionProfit;     } }
        public static string PositionComment    { get { return positionComment;    } }
        public static PosDirection PositionDirection
        {
            get
            {
                PosDirection dir = PosDirection.Error;
                switch (positionType)
                {
                    case (int)MT4Bridge.OrderType.Buy:
                        dir = PosDirection.Long;
                        break;
                    case (int)MT4Bridge.OrderType.Sell:
                        dir = PosDirection.Short;
                        break;
                    default:
                        dir = PosDirection.None;
                        break;
                }

                return dir;
            }
        }

        public static bool SetCurrentPosition(int ticket, int type, double lots, double price, double stoploss, double takeprofit, double profit, string comment)
        {
            bool changed = false;

            if (positionType       != type       ||
                positionLots       != lots       ||
                positionOpenPrice  != price      ||
                positionStopLoss   != stoploss   ||
                positionTakeProfit != takeprofit ||
                positionComment    != comment)
                changed = true;

            positionTicket     = ticket;
            positionType       = type;
            positionLots       = lots;
            positionOpenPrice  = price;
            positionStopLoss   = stoploss;
            positionTakeProfit = takeprofit;
            positionProfit     = profit;
            positionComment    = comment;

            DateTime barOpenTime = Time[Bars - 1];
            if (!barStats.ContainsKey(barOpenTime))
            {
                barStats.Add(barOpenTime, new BarStats(barOpenTime, PositionDirection, positionOpenPrice, positionLots));
            }
            else
            {
                barStats[barOpenTime].PositionDir   = PositionDirection;
                barStats[barOpenTime].PositionPrice = positionOpenPrice;
                barStats[barOpenTime].PositionLots  = positionLots;
            }

            if (changed && Configs.PlaySounds)
                Data.SoundPositionChanged.Play();

            return changed;
        }
        public static void ResetPositionStats()
        {
            positionTicket     = 0;
            positionType       = -1;
            positionLots       = 0;
            positionOpenPrice  = 0;
            positionStopLoss   = 0;
            positionTakeProfit = 0;
            positionProfit     = 0;
            positionComment    = "";
        }

        // Bar statistics
        static Dictionary<DateTime, BarStats> barStats = new Dictionary<DateTime, BarStats>();
        public static Dictionary<DateTime, BarStats> BarStatistics { get { return barStats; } }
        public static void AddBarStats(OperationType operationType, double operationLots, double operationPrice)
        {
            DateTime barOpenTime = Time[Bars - 1];

            if (!barStats.ContainsKey(barOpenTime))
                barStats.Add(barOpenTime, new BarStats(barOpenTime, PositionDirection, positionOpenPrice, positionLots));

            barStats[barOpenTime].Operations.Add(new Operation(barOpenTime, operationType, DateTime.Now, operationLots, operationPrice));

            if (barStats.ContainsKey(Time[0]))
                barStats.Remove(Time[0]);
        }
        public static void ResetBarStats()
        {
            barStats = new Dictionary<DateTime, BarStats>();
        }

        // Usage statistics.
        static DateTime fstStartTime = DateTime.Now;
        static DateTime demoTradeStartTime = DateTime.Now;
        static DateTime liveTradeStartTime = DateTime.Now;
        static int secondsDemoTrading = 0;
        static int secondsLiveTrading = 0;
        static int savedStrategies = 0;
        public static DateTime FstStartTime { get { return fstStartTime; } set { fstStartTime = value; } }
        public static DateTime DemoTradeStartTime { get { return demoTradeStartTime; } set { demoTradeStartTime = value; } }
        public static DateTime LiveTradeStartTime { get { return liveTradeStartTime; } set { liveTradeStartTime = value; } }
        public static int SecondsDemoTrading { get { return secondsDemoTrading; } set { secondsDemoTrading = value; } }
        public static int SecondsLiveTrading { get { return secondsLiveTrading; } set { secondsLiveTrading = value; } }
        public static int SavedStrategies { get { return savedStrategies; } set { savedStrategies = value; } }

        // Wrong set SL or TP
        static int wrongStopLoss = 0;
        static int wrongTakeProf = 0;
        static int wrongStopsRetry = 0;
        public static int WrongStopLoss { get { return wrongStopLoss; } set { wrongStopLoss = value; } }
        public static int WrongTakeProf { get { return wrongTakeProf; } set { wrongTakeProf = value; } }
        public static int WrongStopsRetry { get { return wrongStopsRetry; } set { wrongStopsRetry = value; } }
    }
}
