// Classes
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using MT4Bridge.NamedPipes;

namespace MT4Bridge
{
    public enum PeriodType
    {
        M1  = 1,
        M5  = 5,
        M15 = 15,
        M30 = 30,
        H1  = 60,
        H4  = 240,
        D1  = 1440,
        W1  = 10080,
        MN1 = 43200 
    }

    public enum OrderType
    {
        Buy       = 0,
        Sell      = 1,
        BuyLimit  = 2,
        SellLimit = 3,
        BuyStop   = 4,
        SellStop  = 5
    }

    public class TickEventArgs : EventArgs
    {
        string     symbol;
        PeriodType period;
        DateTime   bartime;
        DateTime   time;
        double     bid;
        double     ask;
        int        spread;
        double     tickValue;
        double     accountBalance;
        double     accountEquity;
        double     accountProfit;
        double     accountFreeMargin;
        int        positionTicket;
        int        positionType;
        double     positionLots;
        double     positionOpenPrice;
        double     positionStopLoss;
        double     positionTakeProfit;
        double     positionProfit;
        string     positionComment;

        public string     Symbol             { get { return symbol;             } }
        public PeriodType Period             { get { return period;             } }
        public DateTime   BarTime            { get { return bartime;            } }
        public DateTime   Time               { get { return time;               } }
        public double     Bid                { get { return bid;                } }
        public double     Ask                { get { return ask;                } }
        public int        Spread             { get { return spread;             } }
        public double     TickValue          { get { return tickValue;          } }
        public double     AccountBalance     { get { return accountBalance;     } }
        public double     AccountEquity      { get { return accountEquity;      } }
        public double     AccountProfit      { get { return accountProfit;      } }
        public double     AccountFreeMargin  { get { return accountFreeMargin;  } }
        public int        PositionTicket     { get { return positionTicket;     } }
        public int        PositionType       { get { return positionType;       } }
        public double     PositionLots       { get { return positionLots;       } }
        public double     PositionOpenPrice  { get { return positionOpenPrice;  } }
        public double     PositionStopLoss   { get { return positionStopLoss;   } }
        public double     PositionTakeProfit { get { return positionTakeProfit; } }
        public double     PositionProfit     { get { return positionProfit;     } }
        public string     PositionComment    { get { return positionComment;    } }

        public TickEventArgs(string symbol, PeriodType period, DateTime bartime, DateTime time, double bid, double ask, int spread, double tickvalue,
                    double accountBalance, double accountEquity, double accountProfit, double accountFreeMargin,
                    int positionTicket, int positionType, double positionLots, double positionOpenPrice,
                    double positionStopLoss, double positionTakeProfit, double positionProfit, string positionComment)
        {
            this.symbol             = symbol;
            this.period             = period;
            this.bartime            = bartime;
            this.time               = time;
            this.bid                = bid;
            this.ask                = ask;
            this.spread             = spread;
            this.tickValue          = tickvalue;
            this.accountBalance     = accountBalance;
            this.accountEquity      = accountEquity;
            this.accountProfit      = accountProfit;
            this.accountFreeMargin  = accountFreeMargin;
            this.positionTicket     = positionTicket;
            this.positionType       = positionType;
            this.positionLots       = positionLots;
            this.positionOpenPrice  = positionOpenPrice;
            this.positionStopLoss   = positionStopLoss;
            this.positionTakeProfit = positionTakeProfit;
            this.positionProfit     = positionProfit;
            this.positionComment    = positionComment;
        }
    };

    public class PingInfo
    {
        string     symbol;
        PeriodType period;
        DateTime   bartime;
        DateTime   time;
        double     bid;
        double     ask;
        int        spread;
        double     tickValue;
        double     accountBalance;
        double     accountEquity;
        double     accountProfit;
        double     accountFreeMargin;
        int        positionTicket;
        int        positionType;
        double     positionLots;
        double     positionOpenPrice;
        double     positionStopLoss;
        double     positionTakeProfit;
        double     positionProfit;
        string     positionComment;

        public string     Symbol             { get { return symbol;             } }
        public PeriodType Period             { get { return period;             } }
        public DateTime   BarTime            { get { return bartime;            } }
        public DateTime   Time               { get { return time;               } }
        public double     Bid                { get { return bid;                } }
        public double     Ask                { get { return ask;                } }
        public int        Spread             { get { return spread;             } }
        public double     TickValue          { get { return tickValue;          } }
        public double     AccountBalance     { get { return accountBalance;     } }
        public double     AccountEquity      { get { return accountEquity;      } }
        public double     AccountProfit      { get { return accountProfit;      } }
        public double     AccountFreeMargin  { get { return accountFreeMargin;  } }
        public int        PositionTicket     { get { return positionTicket;     } }
        public int        PositionType       { get { return positionType;       } }
        public double     PositionLots       { get { return positionLots;       } }
        public double     PositionOpenPrice  { get { return positionOpenPrice;  } }
        public double     PositionStopLoss   { get { return positionStopLoss;   } }
        public double     PositionTakeProfit { get { return positionTakeProfit; } }
        public double     PositionProfit     { get { return positionProfit;     } }
        public string     PositionComment    { get { return positionComment;    } }

        public PingInfo(string symbol, PeriodType period, DateTime bartime, DateTime time, double bid, double ask, int spread, double tickvalue,
                    double accountBalance, double accountEquity, double accountProfit, double accountFreeMargin,
                    int positionTicket, int positionType, double positionLots, double positionOpenPrice,
                    double positionStopLoss, double positionTakeProfit, double positionProfit, string positionComment)
        {
            this.symbol             = symbol;
            this.period             = period;
            this.bartime            = bartime;
            this.time               = time;
            this.bid                = bid;
            this.ask                = ask;
            this.spread             = spread;
            this.tickValue          = tickvalue;
            this.accountBalance     = accountBalance;
            this.accountEquity      = accountEquity;
            this.accountProfit      = accountProfit;
            this.accountFreeMargin  = accountFreeMargin;
            this.positionTicket     = positionTicket;
            this.positionType       = positionType;
            this.positionLots       = positionLots;
            this.positionOpenPrice  = positionOpenPrice;
            this.positionStopLoss   = positionStopLoss;
            this.positionTakeProfit = positionTakeProfit;
            this.positionProfit     = positionProfit;
            this.positionComment    = positionComment;
        }
    }

    public class SymbolInfo
    {
        string symbol;
        double bid, ask, point, spread, stoplevel;
        int    digits;

        public string Symbol    { get { return symbol;    } }
        public double Bid       { get { return bid;       } }
        public double Ask       { get { return ask;       } }
        public double Point     { get { return point;     } }
        public double Spread    { get { return spread;    } }
        public double StopLevel { get { return stoplevel; } }
        public int    Digits    { get { return digits;    } }

        public SymbolInfo(string symbol, double bid, double ask, int digits, double point, double spread, double stoplevel)
        {
            this.symbol    = symbol;
            this.bid       = bid;
            this.ask       = ask;
            this.digits    = digits;
            this.point     = point;
            this.spread    = spread;
            this.stoplevel = stoplevel;
        }
    }

    public class MarketInfo
    {
        double modePoint,        modeDigits,         modeSpread,         modeStopLevel,  modeLotSize;
        double modeTickValue,    modeTickSize,       modeSwapLong,       modeSwapShort,  modeStarting;
        double modeExpiration,   modeTradeAllowed,   modeMinLot,         modeLotStep,    modeMaxLot;
        double modeSwapType,     modeProfitCalcMode, modeMarginCalcMode, modeMarginInit, modeMarginMaintenance;
        double modeMarginHedged, modeMarginRequired, modeFreezeLevel;

        public double ModePoint             { get { return modePoint;             } }
        public double ModeDigits            { get { return modeDigits;            } }
        public double ModeSpread            { get { return modeSpread;            } }
        public double ModeStopLevel         { get { return modeStopLevel;         } }
        public double ModeLotSize           { get { return modeLotSize;           } }
        public double ModeTickValue         { get { return modeTickValue;         } }
        public double ModeTickSize          { get { return modeTickSize;          } }
        public double ModeSwapLong          { get { return modeSwapLong;          } }
        public double ModeSwapShort         { get { return modeSwapShort;         } }
        public double ModeStarting          { get { return modeStarting;          } }
        public double ModeExpiration        { get { return modeExpiration;        } }
        public double ModeTradeAllowed      { get { return modeTradeAllowed;      } }
        public double ModeMinLot            { get { return modeMinLot;            } }
        public double ModeLotStep           { get { return modeLotStep;           } }
        public double ModeMaxLot            { get { return modeMaxLot;            } }
        public double ModeSwapType          { get { return modeSwapType;          } }
        public double ModeProfitCalcMode    { get { return modeProfitCalcMode;    } }
        public double ModeMarginCalcMode    { get { return modeMarginCalcMode;    } }
        public double ModeMarginInit        { get { return modeMarginInit;        } }
        public double ModeMarginMaintenance { get { return modeMarginMaintenance; } }
        public double ModeMarginHedged      { get { return modeMarginHedged;      } }
        public double ModeMarginRequired    { get { return modeMarginRequired;    } }
        public double ModeFreezeLevel       { get { return modeFreezeLevel;       } }

        public MarketInfo(
            double modePoint,          double modeDigits,         double modeSpread,         double modeStopLevel,  double modeLotSize,
            double modeTickValue,      double modeTickSize,       double modeSwapLong,       double modeSwapShort,  double modeStarting,
            double modeExpiration,     double modeTradeAllowed,   double modeMinLot,         double modeLotStep,    double modeMaxLot,
            double modeSwapType,       double modeProfitCalcMode, double modeMarginCalcMode, double modeMarginInit, double modeMarginMaintenance,
            double modeMarginHedged,   double modeMarginRequired, double modeFreezeLevel)
        {
            this.modePoint             = modePoint;
            this.modeDigits            = modeDigits;
            this.modeSpread            = modeSpread;
            this.modeStopLevel         = modeStopLevel;     
            this.modeLotSize           = modeLotSize;
            this.modeTickValue         = modeTickValue;
            this.modeTickSize          = modeTickSize;
            this.modeSwapLong          = modeSwapLong;
            this.modeSwapShort         = modeSwapShort;
            this.modeStarting          = modeStarting;
            this.modeExpiration        = modeExpiration;
            this.modeTradeAllowed      = modeTradeAllowed;
            this.modeMinLot            = modeMinLot;
            this.modeLotStep           = modeLotStep;
            this.modeMaxLot            = modeMaxLot;
            this.modeSwapType          = modeSwapType;
            this.modeProfitCalcMode    = modeProfitCalcMode;
            this.modeMarginCalcMode    = modeMarginCalcMode;
            this.modeMarginInit        = modeMarginInit;
            this.modeMarginMaintenance = modeMarginMaintenance;
            this.modeMarginHedged      = modeMarginHedged;
            this.modeMarginRequired    = modeMarginRequired;
            this.modeFreezeLevel       = modeFreezeLevel;
        }
    }

    public class AccountInfo
    {
        int    number, leverage, stopoutmode, stopoutlevel;
        string name, company, server, currency;
        double balance, equity, profit, credit, margin, freemarginmode, freemargin;

        public string Name           { get { return name;           } }
        public int    Number         { get { return number;         } }
        public string Company        { get { return company;        } }
        public string Server         { get { return server;         } }
        public string Currency       { get { return currency;       } }
        public int    Leverage       { get { return leverage;       } }
        public double Balance        { get { return balance;        } }
        public double Equity         { get { return equity;         } }
        public double Profit         { get { return profit;         } }
        public double Credit         { get { return credit;         } }
        public double Margin         { get { return margin;         } }
        public double FreeMarginMode { get { return freemarginmode; } }
        public double FreeMargin     { get { return freemargin;     } }
        public int    StopOutMode    { get { return stopoutmode;    } }
        public int    StopOutLevel   { get { return stopoutlevel;   } }

        public AccountInfo(string name, int number, string company, string server, string currency, int leverage, double balance,
                double equity, double profit, double credit, double margin, double freemarginmode, double freemargin, int stopoutmode, int stopoutlevel)
        {
            this.name     = name;
            this.number   = number;
            this.company  = company;
            this.server   = server;
            this.currency = currency;
            this.leverage = leverage;
            this.balance  = balance;
            this.equity   = equity;
            this.profit   = profit;
            this.credit   = credit;
            this.margin   = margin;
            this.freemarginmode = freemarginmode;
            this.freemargin     = freemargin;
            this.stopoutmode    = stopoutmode;
            this.stopoutlevel   = stopoutlevel;
        }
    }

    public class TerminalInfo
    {
        string sTerminalName, sTerminalCompany, sTerminalPath, sExpertVersion, sLibraryVersion;

        public string TerminalName    { get { return sTerminalName;    } }
        public string TerminalCompany { get { return sTerminalCompany; } }
        public string TerminalPath    { get { return sTerminalPath;    } }
        public string ExpertVersion   { get { return sExpertVersion;   } }
        public string LibraryVersion  { get { return sLibraryVersion;  } }

        public TerminalInfo(string sTerminalName, string sTerminalCompany, string sTerminalPath, string sExpertVersion, string sLibraryVersion)
        {
            this.sTerminalName    = sTerminalName;
            this.sTerminalCompany = sTerminalCompany;
            this.sTerminalPath    = sTerminalPath;
            this.sExpertVersion   = sExpertVersion;
            this.sLibraryVersion  = sLibraryVersion;
        }
    }

    public class OrderInfo
    {
        int       ticket, magic;
        string    symbol;
        OrderType type;
        double    lots, price, closeprice, stoploss, takeprofit, profit;
        DateTime  time, closetime, expire;

        public int       Ticket     { get { return ticket;     } }
        public int       Magic      { get { return magic;      } }
        public OrderType Type       { get { return type;       } }
        public string    Symbol     { get { return symbol;     } }
        public double    Lots       { get { return lots;       } }
        public double    Price      { get { return price;      } }
        public double    ClosePrice { get { return closeprice; } }
        public double    StopLoss   { get { return stoploss;   } }
        public double    TakeProfit { get { return takeprofit; } }
        public double    Profit     { get { return profit;     } }
        public DateTime  Time       { get { return time;       } }
        public DateTime  CloseTime  { get { return closetime;  } }
        public DateTime  Expire     { get { return expire;     } }

        public OrderInfo(int ticket, string symbol, OrderType type, double lots,
            double price, double stoploss, double takeprofit, DateTime time,
            DateTime closetime, double closeprice, double profit, int magic, DateTime expire)
        {
            this.ticket     = ticket;
            this.symbol     = symbol;
            this.type       = type;
            this.lots       = lots;
            this.price      = price;
            this.stoploss   = stoploss;
            this.takeprofit = takeprofit;
            this.time       = time;
            this.closetime  = closetime;
            this.closeprice = closeprice;
            this.profit     = profit;
            this.magic      = magic;
            this.expire     = expire;
        }
    }

    internal struct Response
    {
        public bool OK;
        public int  Code;

        public Response(bool ok) : this(ok, 0)
        { }

        public Response(bool ok, int code)
        {
            OK   = ok;
            Code = code;
        }
    }
}
