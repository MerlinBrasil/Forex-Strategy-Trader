//+--------------------------------------------------------------------+
//| File name:  MT4-FST Expert.mq4                                     |
//| Version:    1.6 2011-08-04                                         |
//| Copyright:  © 2011, Miroslav Popov - All rights reserved!          |
//| Website:    http://forexsb.com/                                    |
//| Support:    http://forexsb.com/forum/                              |
//| License:    Freeware under the following circumstances:            |
//|                                                                    |
//| This code is a part of Forex Strategy Trader. It is free for       |
//| use and distribution as an integral part of Forex Strategy Trader. |
//| One can modify it in order to improve the code or to fit it for    |
//| personal use. This code or any part of it cannot be used in        |
//| another applications without a permission. Contact information     |
//| cannot be changed.                                                 |
//|                                                                    |
//| NO LIABILITY FOR CONSEQUENTIAL DAMAGES                             |
//|                                                                    |
//| In no event shall the author be liable for any damages whatsoever  |
//| (including, without limitation, incidental, direct, indirect and   |
//| consequential damages, damages for loss of business profits,       |
//| business interruption, loss of business information, or other      |
//| pecuniary loss) arising out of the use or inability to use this    |
//| product, even if advised of the possibility of such damages.       |
//+--------------------------------------------------------------------+
#include <WinUser32.mqh>

#property copyright "Copyright © 2011, Miroslav Popov"
#property link      "http://forexsb.com/"

#define EXPERT_VERSION           "1.6"
#define SERVER_SEMA_NAME         "MT4-FST Expert ID - "
#define TRADE_SEMA_NAME          "TradeIsBusy"
#define TRADE_SEMA_WAIT          100
#define TRADE_SEMA_TIMEOUT       10000
#define TRADE_RETRY_COUNT        4
#define TRADE_RETRY_WAIT         100

#define FST_REQ_ACCOUNT_INFO     3
#define FST_REQ_BARS             4
#define FST_REQ_ORDER_SEND       7
#define FST_REQ_ORDER_MODIFY     8
#define FST_REQ_ORDER_CLOSE      9
#define FST_REQ_PING             11
#define FST_REQ_MARKET_INFO_ALL  12
#define FST_REQ_TERMINAL_INFO    13

#define FST_ERR_INVALID_REQUEST  -1
#define FST_ERR_WRONG_ORD_TYPE   -10005
#define FST_ERR_POS_ALREADY_OPEN -10010
#define FST_ERR_NO_OPEN_POSITION -10015

#define OP_SQUARE                -1

#import "MT4-FST Library.dll"
string FST_LibraryVersion();
void FST_OpenConnection(int id);
void FST_CloseConnection(int id);
void FST_Ping(int id, string symbol, int period, int time, double bid, double ask, int spread, double tickval, double rates[][6], int bars,
        double accountBalance, double accountEquity, double accountProfit, double accountFreeMargin, int positionTicket,
        int positionType, double positionLots, double positionOpenPrice, int positionOpenTime, double positionStopLoss,
        double positionTakeProfit, double positionProfit, string positionComment);
int FST_Tick(int id, string symbol, int period, int time, double bid, double ask, int spread, double tickval, double rates[][6], int bars,
        double accountBalance, double accountEquity, double accountProfit, double accountFreeMargin, int positionTicket,
        int positionType, double positionLots, double positionOpenPrice, int positionOpenTime, double positionStopLoss,
        double positionTakeProfit, double positionProfit, string positionComment);
void FST_MarketInfoAll(int id, double point, double digits, double spread, double stoplevel, double lotsize,
        double tickvalue, double ticksize, double swaplong, double swapshort, double starting, double expiration,
        double tradeallowed, double minlot, double lotstep, double maxlot, double swaptype, double profitcalcmode,
        double margincalcmode, double margininit, double marginmaintenance, double marginhedged,
        double marginrequired, double freezelevel);
void FST_AccountInfo(int id, string name, int number, string company, string server, string currency, int leverage,
        double balance, double equity, double profit, double credit, double margin, double freemarginmode,
        double freemargin, int stopoutmode, int stopout, int isdemo);
void FST_TerminalInfo(int id, string symbol, string company, string path, string expertversion);
void FST_Bars(int id, string symbol, int period, double rates[][6], int bars);
int  FST_Request(int id, string &symbol[], int &iargs[], int icount, double &dargs[], int dcount, string &parameters[]);
void FST_Response(int id, int ok, int code);
#import

// -----------------------    External variables   ----------------------- //

// Connection_ID serves to identify the expert when multiple copies of Forex Strategy Trader are used.
// It must be a unique number between 0 and 1000.
// The same number has to be entered in Forex Strategy Trader.
extern int Connection_ID = 0;

// If account equity drops below this value, the expert will close out all positions and stop automatic trade.
// The value must be set in account currency. Example:
// Protection_Min_Account = 700 will close positions if the equity drops below 700 USD (EUR if you account is in EUR).
extern int Protection_Min_Account = 0;

// The expert checks the open positions at every tick and if found no SL or SL lower (higher for short) than selected,
// It sets SL to the defined value. The value is in points. Example:
// Protection_Max_StopLoss = 200 means 200 pips for 4 digit broker and 20 pips for 5 digit broker.
extern int Protection_Max_StopLoss = 0;

// A unique number of the expert's orders.
extern int Expert_Magic = 20011023;

// ----------------------------    Options   ---------------------------- //

// TrailingStop_Moving_Step determines the step of changing the Trailing Stop.
// 0 <= TrailingStop_Moving_Step <= 2000
// If TrailingStop_Moving_Step = 0, the Trailing Stop trails at every new extreme price in the position's direction.
// If TrailingStop_Moving_Step > 0, the Trailing Stop moves at steps equal to the number of pips chosen.
int TrailingStop_Moving_Step = 0;

// FIFO (First In First Out) forces the expert to close positions starting from
// the oldest one. This rule complies with the new NFA regulations.
// If you want to close the positions from the newest one (FILO), change the variable to "false".
// This doesn't change the normal work of Forex Strategy Trader.
bool FIFO_order = true;

// --------------------------------------------------------------------- //

bool     Separated_SL_TP  = false; // It's for brokers like FXCM and FXOpen, which don't allow sending OrderSend(..) with SL and TP.
bool     IsServer         = false; // It shows whether the server is running.
bool     ConnectedToDLL   = false; // It shows whether the expert was connected to the dll.
int      LastError        = 0;     // The number of last error.
bool     FST_Connected    = false; // Shows if FST is connected.
datetime TimeLastPing     = 0;     // Time of last ping from Forex Strategy Trader.

// Aggregate position.
int      PositionTicket     = 0;
int      PositionType       = OP_SQUARE;
datetime PositionTime       = D'2020.01.01 00:00';
double   PositionLots       = 0;
double   PositionOpenPrice  = 0;
double   PositionStopLoss   = 0;
double   PositionTakeProfit = 0;
double   PositionProfit     = 0;
double   PositionCommission = 0;
string   PositionComment    = "";

// Set by Forex Strategy Trader
int    TrailingStop = 0;
string TrailingMode = "";
int    BreakEven    = 0;

datetime barHighTime  = 0;
datetime barLowTime   = 0;
double currentBarHigh = 0;
double currentBarLow  = 1000000;

///
/// Expert's initialization function.
///
int init()
{
    string message = "MT4-FST Expert version " + EXPERT_VERSION + " was started. An environment test is running...";
    Comment(message);
    Print(message);

    // Checks the requirements.
    bool isEnvironmentGood = CheckEnvironment();

    if (!isEnvironmentGood)
    {   // There is a nonfulfilled condition, therefore we must exit.
        Sleep(20 * 1000);
        PostMessageA(WindowHandle(Symbol(), Period()), WM_COMMAND, 33050, 0);

        return (-1);
    }

    message = "The environment test was accomplished successfully.";
    Comment(message);
    Print(message);

    SetBrokersCompatibility();

    ReleaseTradeContext();

    Server(); // It's OK. We start the server.

    return (0);
}

///
/// Expert's start function.
///
int start()
{
    // We don't have to do anything here.
    // The new incoming ticks are processed by the server.

    return (0);
}

///
/// Expert's deinitialization function.
///
int deinit()
{
   Comment("");
   if (ConnectedToDLL)
       FST_CloseConnection(Connection_ID);

   // Releases the global variable so the Expert
   // can be started on another chart.
   ReleaseServerSema();

   return (0);
}

///
/// Checks the working conditions.
///
bool CheckEnvironment()
{
    string message;

    // Checks if DLL is allowed.
    if (IsDllsAllowed() == false)
    {
        message = "\n" + "DLL call is not allowed." + "\n" +
                  "Please allow DLL loading in the MT options and restart the expert.";
        Comment(message);
        Print(message);
        return (false);
    }

    // Checks the Library version
    message = "\n" + "Cannot load \"MT4-FST Library.dll\"." + "\n" +
              "Please find more information about this error in the support forum.";
    Comment(message);
    string libraryVersion = FST_LibraryVersion();
    if (libraryVersion != "")
    {
        message = "MT4-FST Library version " + libraryVersion + " loaded successfully.";
        Comment(message);
        Print(message);
    }
    else
    {   // Meta Trader stops if it cannot load the dll.
        // Error 126 is displayed.
        return (false);
    }

    // Checks if you are logged in.
    if (AccountNumber() == 0)
    {
        message = "\n" + "You are not logged in. Please login first.";
        Comment(message);
        for (int attempt = 0; attempt < 200; attempt++)
        {
            if (AccountNumber() == 0)
                Sleep(300);
            else
                break;
        }
        if (AccountNumber() == 0)
            return (false);
    }

    // Checks the amount of bars available.
    int barsNecessary = MathMax(2 * 1440 / Period() + 10, 400); // Two days + 10 bars, min 400.
    if (!CheckChartBarsNumber(Symbol(), Period(), barsNecessary))
    {
        message = "\n" + "Cannot load enough bars! The expert needs minimum " + barsNecessary + " bars for this time frame." + "\n" +
                  "Please load more data in the chart window and restart the expert.";
        Comment(message);
        Print(message);
        return (false);
    }

    // Checks the open positions.
    if (SetAggregatePosition(Symbol()) == -1)
    {   // Some error with the current positions.
        return (false);
    }

    // Checks whether the expert was started on another chart.
    IsServer = GetServerSema();
    if (!IsServer)
    {
        string expert = "MT4-FST Expert ";
        if (Connection_ID > 0)
            expert = "MT4-FST Expert ID = " + Connection_ID + " ";

        message = "\n" + expert + "is already running on another chart!" + "\n" +
                  "Please stop all the instances and restart the expert." + "\n" + "\n" +
                  "If you cannot find the expert on the other charts, " + "\n" +
                  "open the \"Global Variables\" window of MetaTrader (short key F3) and " + "\n" +
                  "delete the MT4-FST Expert ID - X variable. Restart the expert after that." + "\n" + "\n" +
                  "If you want to use the expert on several charts, you have to set a unique Connection_ID for " + "\n" +
                  "all the experts and to do the same in FST. (The option must be switched on from Tools menu).";
        Comment(message);
        Print(message);
        return (false);
    }

    FST_OpenConnection(Connection_ID);
    ConnectedToDLL = true;

    // Everything looks OK.
    return (true);
}

///
/// Closes expert and resets variables.
///
int CloseExpert()
{

   PostMessageA(WindowHandle(Symbol(), Period()), WM_COMMAND, 33050, 0);
   deinit();

   return (0);
}

///
/// MT4-FST Server
///
int Server()
{
    string oldrequest = "";
    string expertID   = "";
    if (Connection_ID > 0)
        expertID = "(ID = " + Connection_ID + ") ";
    datetime tickTime = MarketInfo(Symbol(), MODE_TIME);
    datetime barTime = Time[0];
    string message = expertID + TimeToStr(TimeLocal(), TIME_DATE | TIME_SECONDS) + " Forex Strategy Trader is disconnected.";
    Comment(message);

    if (Protection_Max_StopLoss > 0 && Protection_Max_StopLoss < MarketInfo(Symbol(), MODE_STOPLEVEL))
        Protection_Max_StopLoss = MarketInfo(Symbol(), MODE_STOPLEVEL);

    while (!IsStopped())
    {
        LastError = 0;
        RefreshRates();

        // Checks if FST is connected.
        if (FST_Connected && (TimeLocal() - TimeLastPing > 60))
        {
            FST_Connected = false;
            message = expertID + TimeToStr(TimeLocal(), TIME_DATE | TIME_SECONDS) + " Forex Strategy Trader is disconnected.";
            Comment(message);
        }

        bool     isSucceed = false;
        string   symbol[1]; symbol[0] = Symbol();
        int      iargs[5];
        double   dargs[5];
        string   parameters[1]; parameters[0] = "We have to arrange some space for the string!";
        datetime time = MarketInfo(symbol[0], MODE_TIME);

        // Check for a new tick.
        if (time > tickTime)
        {
            tickTime = time;

            // Checks if minimum account was reached.
            if (Protection_Min_Account > 0 && AccountEquity() < Protection_Min_Account)
            {
                CloseCurrentPosition(Symbol(), 100);

                string account = DoubleToStr(AccountEquity(), 2);
                message = "\n" + "The account equity (" + account + ") dropped below the minimum allowed (" + Protection_Min_Account + ").";
                Comment(message);
                Print(message);

                Sleep(20 * 1000);
                CloseExpert();

                return (-1);
            }

            // Checks and sets Max SL protection.
            if (Protection_Max_StopLoss > 0)
                SetMaxStopLoss(symbol[0]);

            SetAggregatePosition(symbol[0]);

            bool isNewBar = (barTime != Time[0]);
            barTime = Time[0];

            if (BreakEven > 0)
                SetBreakEvenStop(symbol[0]);

            if (TrailingStop > 0)
                SetTrailingStop(symbol[0], isNewBar);

            int tickResponce = SendTick(symbol[0]);

            if (tickResponce == 1)
            {
                FST_Connected = true;
                TimeLastPing  = TimeLocal();
                message = expertID + TimeToStr(TimeLocal(), TIME_DATE | TIME_SECONDS) + " Forex Strategy Trader is connected.";
            }
            else if (tickResponce == 0)
            {
                FST_Connected = false;
                message = expertID + TimeToStr(TimeLocal(), TIME_DATE | TIME_SECONDS) + " Forex Strategy Trader is disconnected.";
            }
            else if (tickResponce == -1)
            {
                message = expertID + TimeToStr(TimeLocal(), TIME_DATE | TIME_SECONDS) + " Error with sending a tick";
            }
            Comment(message);

            TimeLastPing = TimeLocal();
        }

        // Check for a request from Forex Strategy Trader.
        int request = FST_Request(Connection_ID, symbol, iargs, 5, dargs, 5, parameters);
        if (request < 0)
        {
            Comment("MT4-FST Library Server Error: ", request);
        }
        else if (request > 0)
        {
            /*
            // Prints debug info
            string newrequest = symbol[0] +
                " iargs[0]=" + iargs[0] + " iargs[1]=" + iargs[1] + " iargs[2]=" + iargs[2] + " iargs[3]=" + iargs[3] + " iargs[4]=" + iargs[4] +
                " dargs[0]=" + dargs[0] + " dargs[1]=" + dargs[1] + " dargs[2]=" + dargs[2] + " dargs[3]=" + dargs[3] + " dargs[4]=" + dargs[4] +
                " " + parameters[0];
            if (oldrequest != newrequest)
            {
                oldrequest = newrequest;
                Print(newrequest);
            }
            */

            switch (request)
            {
                case FST_REQ_PING:
                    // Forex Strategy Trader sends a ping.
                    GetPing(symbol[0]);
                    FST_Connected = true;
                    TimeLastPing  = TimeLocal();

                    message = expertID + TimeToStr(TimeLocal(), TIME_DATE | TIME_SECONDS) + " Forex Strategy Trader is connected.";
                    Comment(message);
                    break;

                case FST_REQ_MARKET_INFO_ALL:
                    // Forex Strategy Trader requests full market info.
                    GetMarketInfoAll(symbol[0]);
                    break;

                case FST_REQ_ACCOUNT_INFO:
                    // Forex Strategy Trader requests full account info.
                    FST_AccountInfo(Connection_ID, AccountName(), AccountNumber(), AccountCompany(), AccountServer(), AccountCurrency(),
                        AccountLeverage(), AccountBalance(), AccountEquity(), AccountProfit(), AccountCredit(), AccountMargin(),
                        AccountFreeMarginMode(), AccountFreeMargin(), AccountStopoutMode(), AccountStopoutLevel(), IF_I(IsDemo(), 1, 0));
                    break;

                case FST_REQ_TERMINAL_INFO:
                    // Forex Strategy Trader requests terminal info.
                    FST_TerminalInfo(Connection_ID, TerminalName(), TerminalCompany(), TerminalPath(), EXPERT_VERSION);
                    break;

                case FST_REQ_BARS:
                    // Forex Strategy Trader requests historical bars.
                    GetBars(symbol[0], iargs[0], iargs[1]);
                    break;

                case FST_REQ_ORDER_SEND:
                    // Forex Strategy Trader sends an order.
                    ParseOrderParameters(parameters[0]);
                    int orderResponse = ManageOrderSend(symbol[0], iargs[0], dargs[0], dargs[1], iargs[1], dargs[2], dargs[3], "", Expert_Magic, iargs[3]);

                    if (orderResponse < 0)
                    {
                        int lastErrorOrdSend = GetLastError();
                        lastErrorOrdSend = IF_I(lastErrorOrdSend > 0, lastErrorOrdSend, LastError);
                        Print("Error in FST Request OrderSend: ", GetErrorDescription(lastErrorOrdSend));
                        FST_Response(Connection_ID, false, lastErrorOrdSend);
                    }
                    else
                        FST_Response(Connection_ID, true, orderResponse);
                    break;

                case FST_REQ_ORDER_CLOSE:
                    // Forex Strategy Trader wants to close the current position.
                    isSucceed = CloseCurrentPosition(symbol[0], iargs[1]) == 0;

                    int lastErrorOrdClose = GetLastError();
                    lastErrorOrdClose = IF_I(lastErrorOrdClose > 0, lastErrorOrdClose, LastError);
                    if (!isSucceed) Print("Error in OrderClose: ", GetErrorDescription(lastErrorOrdClose));
                    FST_Response(Connection_ID, isSucceed, lastErrorOrdClose);
                    break;

                case FST_REQ_ORDER_MODIFY:
                    // Forex Strategy Trader wants to modify the current position.
                    ParseOrderParameters(parameters[0]);
                    isSucceed = ModifyPosition(symbol[0], dargs[1], dargs[2]);

                    int lastErrorOrdModify = GetLastError();
                    lastErrorOrdModify = IF_I(lastErrorOrdModify > 0, lastErrorOrdModify, LastError);
                    if (!isSucceed) Print("Error in OrderModify: ", GetErrorDescription(lastErrorOrdModify));
                    FST_Response(Connection_ID, isSucceed, lastErrorOrdModify);
                    break;

                default:
                    // Forex Strategy Trader doesn't know what to do.
                    FST_Response(Connection_ID, false, FST_ERR_INVALID_REQUEST);

                    message = TimeToStr(TimeLocal(), TIME_DATE | TIME_SECONDS) + " Error - Forex Strategy Trader sent a wrong request.";
                    Comment(message);
                    Print(message);
                    break;
            }
        }

        Sleep(100);
    }

    return (0);
}

// ===========================================================================

///
/// Sets the virtual aggregate position.
/// Returns the number of open positions by FST or -1 if an error has occurred.
int SetAggregatePosition(string symbol)
{
    PositionTicket     = 0;
    PositionType       = OP_SQUARE;
    //PositionTime       = D'2020.01.01 00:00';
    PositionLots       = 0;
    PositionOpenPrice  = 0;
    PositionStopLoss   = 0;
    PositionTakeProfit = 0;
    PositionProfit     = 0;
    PositionCommission = 0;
    PositionComment    = "";

    int positions = 0;

    for (int i = 0; i < OrdersTotal(); i++)
    {
        if (!OrderSelect(i, SELECT_BY_POS, MODE_TRADES))
        {
            Print("Error with OrderSelect: ", GetErrorDescription(GetLastError()));
            Comment("Cannot check current position!");
            continue;
        }

        if (OrderMagicNumber() != Expert_Magic || OrderSymbol() != symbol)
            continue; // An order not sent by Forex Strategy Trader.

        if (OrderType() == OP_BUYLIMIT || OrderType() == OP_SELLLIMIT || OrderType() == OP_BUYSTOP || OrderType() == OP_SELLSTOP)
            continue; // A pending order.

        if (PositionType >= 0 && PositionType != OrderType())
        {
            string message = "There are open positions in different directions!";
            Comment(message);
            Print(message);
            return (-1);
        }

        PositionTicket      = OrderTicket();
        PositionType        = OrderType();
        PositionTime        = IF_I(OrderOpenTime() < PositionTime, OrderOpenTime(), PositionTime);
        PositionOpenPrice   = (PositionLots * PositionOpenPrice + OrderLots() * OrderOpenPrice()) / (PositionLots + OrderLots());
        PositionLots       += OrderLots();
        PositionProfit     += OrderProfit() + OrderCommission();
        PositionCommission += OrderCommission();
        PositionStopLoss    = OrderStopLoss();
        PositionTakeProfit  = OrderTakeProfit();
        PositionComment     = OrderComment();

        positions += 1;
    }

    if (PositionOpenPrice > 0)
        PositionOpenPrice = NormalizeDouble(PositionOpenPrice, MarketInfo(symbol, MODE_DIGITS));

    if (PositionLots == 0)
        PositionTime = D'2020.01.01 00:00';

    return (positions);
}

///
/// Sends correct order depending on the request and the current position.
///
int ManageOrderSend (string symbol, int type, double lots, double price, int slippage, double stoploss,
                     double takeprofit, string comment = "", int magic = 0, int expire = 0)
{
    int orderResponse = -1;
    int positions = SetAggregatePosition(symbol);

    if (positions < 0)
        return (-1); // Error in SetAggregatePosition.

    if (positions == 0)
    {   // Open a new position.
        orderResponse = OpenNewPosition(symbol, type, lots, price, slippage, stoploss, takeprofit, magic);
    }
    else if (positions > 0)
    {   // There is a position.
        if ((PositionType == OP_BUY && type == OP_BUY) || (PositionType == OP_SELL && type == OP_SELL))
        {   // Add to the current position.
            orderResponse = AddToCurrentPosition(symbol, type, lots, price, slippage, stoploss, takeprofit, magic);
        }
        else if ((PositionType == OP_BUY && type == OP_SELL) || (PositionType == OP_SELL && type == OP_BUY))
        {
            if (MathAbs(PositionLots - lots) < MarketInfo(symbol, MODE_LOTSTEP) / 2)
            {   // The position's lots are equal to the opposite order's lots. We close the current position.
                orderResponse = CloseCurrentPosition(symbol, slippage);
            }
            else if (PositionLots > lots)
            {   // Reducing a position. (Partially closing).
                orderResponse = ReduceCurrentPosition(symbol, lots, price, slippage, stoploss, takeprofit, magic);
            }
            else if (PositionLots < lots)
            {   // Reversing a position.
                orderResponse = ReverseCurrentPosition(symbol, type, lots, price, slippage, stoploss, takeprofit, magic);
            }
        }
    }

    return (orderResponse);
}

///
/// Opens a new position at market price.
///
int OpenNewPosition (string symbol, int type, double lots, double price, int slippage, double stoploss, double takeprofit, int magic)
{
    int orderResponse = -1;

    if (type != OP_BUY && type != OP_SELL)
    {   // Error. Wrong order type!
        Print("Wrong 'Open new position' request - Wrong order type!");
        return (FST_ERR_WRONG_ORD_TYPE);
    }

    double orderLots = NormalizeEntrySize(symbol, lots);

    string comment = "";
    if (Connection_ID > 0) comment = "ID = " + Connection_ID;

    if (AccountFreeMarginCheck(symbol, type, orderLots) > 0)
    {
        if (Separated_SL_TP)
        {
            orderResponse = SendOrder(symbol, type, lots, price, slippage, 0, 0, comment, magic);
            if (orderResponse > 0)
            {
                double stopLossPrice   = GetStopLossPrice(symbol, type, orderLots, stoploss);
                double takeProfitPrice = GetTakeProfitPrice(symbol, type, takeprofit);
                orderResponse = ModifyPositionByTicket(symbol, orderResponse, stopLossPrice, takeProfitPrice);
            }
        }
        else
        {
            orderResponse = SendOrder(symbol, type, lots, price, slippage, stoploss, takeprofit, comment, magic);

            if (orderResponse < 0 && LastError == 130)
            {   // Invalid Stops. We'll check for forbiden direct set of SL and TP
                orderResponse = SendOrder(symbol, type, lots, price, slippage, 0, 0, comment, magic);
                if (orderResponse > 0)
                {
                    stopLossPrice   = GetStopLossPrice(symbol, type, orderLots, stoploss);
                    takeProfitPrice = GetTakeProfitPrice(symbol, type, takeprofit);
                    orderResponse   = ModifyPositionByTicket(symbol, orderResponse, stopLossPrice, takeProfitPrice);
                    if (orderResponse > 0)
                    {
                        Separated_SL_TP = true;
                        Print(AccountCompany(), " marked for late stops sending.");
                    }
                }
            }
        }
    }

    SetAggregatePosition(symbol);

    return (orderResponse);
}

///
/// Adds more lots to the current position at the market price.
///
int AddToCurrentPosition(string symbol, int type, double lots, double price, int slippage, double stoploss, double takeprofit, int magic)
{
    // Checks if we have enough money.
    if (AccountFreeMarginCheck(symbol, type, lots) <= 0)
        return (-1);

    int orderResponse = OpenNewPosition(symbol, type, lots, price, slippage, stoploss, takeprofit, magic);

    if(orderResponse < 0)
        return (orderResponse);

    double stopLossPrice   = GetStopLossPrice(symbol, type, PositionLots, stoploss);
    double takeProfitPrice = GetTakeProfitPrice(symbol, type, takeprofit);

    orderResponse = SetStopLossAndTakeProfit(symbol, stopLossPrice, takeProfitPrice);

    SetAggregatePosition(symbol);

    return (orderResponse);
}

///
/// Reduces current position at the market price.
///
int ReduceCurrentPosition(string symbol, double lots, double price, int slippage, double stoploss, double takeprofit, int magic)
{
    double newlots = PositionLots - lots;

    int orderstotal = OrdersTotal();
    int orders = 0;
    datetime openPos[][2];

    for (int i = 0; i < orderstotal; i++)
    {
        if (!OrderSelect(i, SELECT_BY_POS, MODE_TRADES))
        {
            LastError = GetLastError();
            Print("Error in OrderSelect: ", GetErrorDescription(LastError));
            continue;
        }

        if (OrderMagicNumber() != magic || OrderSymbol() != symbol)
            continue;

        int orderType = OrderType();
        if (orderType != OP_BUY && orderType != OP_SELL)
            continue;

        orders++;
        ArrayResize(openPos, orders);
        openPos[orders - 1][0] = OrderOpenTime();
        openPos[orders - 1][1] = OrderTicket();
    }

    if (FIFO_order)
        ArraySort(openPos, WHOLE_ARRAY, 0, MODE_ASCEND);
    else
        ArraySort(openPos, WHOLE_ARRAY, 0, MODE_DESCEND);

    for (i = 0; i < orders; i++)
    {
        OrderSelect(openPos[i][1], SELECT_BY_TICKET);
        double orderLots = IF_D(lots >= OrderLots(), OrderLots(), lots);
        ClosePositionByTicket(symbol, OrderTicket(), orderLots, slippage);

        lots -= orderLots;
        if (lots <= 0)
            break;
    }

    double stopLossPrice   = GetStopLossPrice(symbol, PositionType, newlots, stoploss);
    double takeProfitPrice = GetTakeProfitPrice(symbol, PositionType, takeprofit);

    int orderResponse = SetStopLossAndTakeProfit(symbol, stopLossPrice, takeProfitPrice);

    SetAggregatePosition(symbol);

    return (orderResponse);
}

///
/// Closes current position at market price.
/// Returns 0 if successful or negative if an error has occurred.
///
int CloseCurrentPosition(string symbol, int slippage)
{
    int orderResponse = -1;
    int orderstotal   = OrdersTotal();
    int orders        = 0;
    datetime openPos[][2];

    for (int i = 0; i < orderstotal; i++)
    {
        if (!OrderSelect(i, SELECT_BY_POS, MODE_TRADES))
        {
            LastError = GetLastError();
            Print("Error in OrderSelect: ", GetErrorDescription(LastError));
            continue;
        }

        if (OrderMagicNumber() != Expert_Magic || OrderSymbol() != symbol)
            continue;

        int orderType = OrderType();
        if (orderType != OP_BUY && orderType != OP_SELL)
            continue;

        orders++;
        ArrayResize(openPos, orders);
        openPos[orders - 1][0] = OrderOpenTime();
        openPos[orders - 1][1] = OrderTicket();
    }

    if (FIFO_order)
        ArraySort(openPos, WHOLE_ARRAY, 0, MODE_ASCEND);
    else
        ArraySort(openPos, WHOLE_ARRAY, 0, MODE_DESCEND);

    for (i = 0; i < orders; i++)
    {
        OrderSelect(openPos[i][1], SELECT_BY_TICKET);
        orderResponse = ClosePositionByTicket(symbol, OrderTicket(), OrderLots(), slippage);
    }

    SetAggregatePosition(symbol);

    return (orderResponse);
}

///
/// Reverses current position at market price.
///
int ReverseCurrentPosition(string symbol, int type, double lots, double price, int slippage, double stoploss, double takeprofit, int magic)
{
    lots = lots - PositionLots;
    CloseCurrentPosition(symbol, slippage);

    int orderResponse = OpenNewPosition(symbol, type, lots, price, slippage, stoploss, takeprofit, magic);

    SetAggregatePosition(symbol);

    return (orderResponse);
}

///
/// Modify the position's Stop Loss and Take Profit
///
bool ModifyPosition(string symbol, double stoploss, double takeprofit)
{
    if (SetAggregatePosition(symbol) <= 0)
        return (false);

    double stopLossPrice   = GetStopLossPrice(symbol, PositionType, PositionLots, stoploss);
    double takeProfitPrice = GetTakeProfitPrice(symbol, PositionType, takeprofit);

    int responce = SetStopLossAndTakeProfit(symbol, stopLossPrice, takeProfitPrice);

    return (responce >= 0);
}

///
/// Sends an order to the broker.
///
int SendOrder(string symbol, int type, double lots, double price, int slippage, double stoploss, double takeprofit, string comment = "", int magic = 0)
{
    int orderResponse = -1;

    for (int attempt = 0; attempt < TRADE_RETRY_COUNT; attempt++)
    {
        if (!GetTradeContext())
            return (-1);

        double orderLots       = NormalizeEntrySize(symbol, lots);
        double orderPrice      = GetMarketPrice(symbol, type, price);
        double stopLossPrice   = GetStopLossPrice(symbol, type, orderLots, stoploss);
        double takeProfitPrice = GetTakeProfitPrice(symbol, type, takeprofit);
        color  colorDeal       = Lime; if (type == OP_SELL) colorDeal = Red;

        orderResponse = OrderSend(symbol, type, orderLots, orderPrice, slippage, stopLossPrice, takeProfitPrice, comment, magic, 0, colorDeal);

        ReleaseTradeContext();

        LastError = GetLastError();
        if (orderResponse > 0)
            break;

        if (LastError != 135 && LastError != 136 && LastError != 137 && LastError != 138)
            break;

        Print("Error with SendOrder: ", GetErrorDescription(LastError));

        Sleep(TRADE_RETRY_WAIT);
    }

    return (orderResponse);
}

///
/// Closes a position by its ticket.
/// Returns 0 if successful or negative if an error has occurred.
int ClosePositionByTicket(string symbol, int orderTicket, double orderLots, int slippage)
{
    if (!OrderSelect(orderTicket, SELECT_BY_TICKET))
    {
        LastError = GetLastError();
        Print("Error with OrderSelect: ", GetErrorDescription(LastError));
        return (-1);
    }

    int orderType = OrderType();

    for (int attempt = 0; attempt < TRADE_RETRY_COUNT; attempt++)
    {
        if (!GetTradeContext())
            return (-1);

        double orderPrice = IF_D(orderType == OP_BUY, MarketInfo(symbol, MODE_BID), MarketInfo(symbol, MODE_ASK));
        orderPrice = NormalizeDouble(orderPrice, Digits);
        bool responce = OrderClose(orderTicket, orderLots, orderPrice, slippage, Gold);

        ReleaseTradeContext();

        LastError = GetLastError();
        if (responce)
            return (0);

        Print("Error with ClosePositionByTicket: ", GetErrorDescription(LastError), ". Attempt No: ", (attempt + 1));
        Sleep(TRADE_RETRY_WAIT);
   }

    return (-1);
}

///
/// Updates the Stop Loss and the Take Profit of current positions.
///
int SetStopLossAndTakeProfit(string symbol, double stopLossPrice, double takeProfitPrice)
{
    int responce = 1;

    for (int i = 0; i < OrdersTotal(); i++)
    {
        if (!OrderSelect(i, SELECT_BY_POS, MODE_TRADES))
        {
            LastError = GetLastError();
            Print("Error with OrderSelect: ", GetErrorDescription(LastError));
            continue;
        }

        if (OrderMagicNumber() != Expert_Magic || OrderSymbol() != symbol)
            continue;

        int type = OrderType();
        if (type != OP_BUY && type != OP_SELL)
            continue;

        if (ModifyPositionByTicket(symbol, OrderTicket(), stopLossPrice, takeProfitPrice) < 0)
            responce = -1;
    }

    return (responce);
}

///
/// Modifies the position's Stop Loss and Take Profit.
///
int ModifyPositionByTicket(string symbol, int orderTicket, double stopLossPrice, double takeProfitPrice)
{
    if (!OrderSelect(orderTicket, SELECT_BY_TICKET))
    {
        LastError = GetLastError();
        Print("Error with OrderSelect: ", GetErrorDescription(LastError));
        return (-1);
    }

    stopLossPrice   = NormalizeDouble(stopLossPrice,   Digits);
    takeProfitPrice = NormalizeDouble(takeProfitPrice, Digits);

    double oldStopLoss   = NormalizeDouble(OrderStopLoss(),   Digits);
    double oldTakeProfit = NormalizeDouble(OrderTakeProfit(), Digits);

    if ((stopLossPrice == oldStopLoss) && (takeProfitPrice == oldTakeProfit))
        return (1); // There isn't anything to change.

    double orderOpenPrice = NormalizeDouble(OrderOpenPrice(), Digits);

    for (int attempt = 0; attempt < TRADE_RETRY_COUNT; attempt++)
    {
        if (!GetTradeContext())
            return (-1);
        bool rc = OrderModify(orderTicket, orderOpenPrice, stopLossPrice, takeProfitPrice, 0);
        ReleaseTradeContext();

        if (rc)
        {   // Modification was successful.
            return (1);
        }

        LastError = GetLastError();
        Print("Error with OrderModify(", orderTicket, ", ", orderOpenPrice, ", ", stopLossPrice, ", ", takeProfitPrice, ") ", GetErrorDescription(LastError), ".");
        Sleep(TRADE_RETRY_WAIT);
        RefreshRates();

        if (LastError == 130)
        {   // Invalid stops error.
            stopLossPrice   = CorrectStopLossPrice(symbol,   OrderType(), stopLossPrice);
            takeProfitPrice = CorrectTakeProfitPrice(symbol, OrderType(), takeProfitPrice);
        }

        if (LastError == 4108)
        {   // Invalid ticket error.
            return (-1);
        }
    }

    return (-1);
}

///
/// Checks and corrects the order price.
///
double GetMarketPrice(string symbol, int type, double price)
{
    double orderPrice = price;

    if (type == OP_BUY)
        orderPrice = MarketInfo(symbol, MODE_ASK);
    else // if (type == OP_SELL)
        orderPrice = MarketInfo(symbol, MODE_BID);

    orderPrice = NormalizeDouble(orderPrice, MarketInfo(symbol, MODE_DIGITS));

    return (orderPrice);
}

///
/// Calculates the Take Profit price depending on the position direction and the Take Profit distance.
///
double GetTakeProfitPrice(string symbol, int type, double takeprofit)
{
    double takeProfitPrice = 0;

    if (takeprofit == 0)
        return (takeProfitPrice);

    if (takeprofit < MarketInfo(symbol, MODE_STOPLEVEL))
        takeprofit = MarketInfo(symbol, MODE_STOPLEVEL);

    if (type == OP_BUY)
        takeProfitPrice = MarketInfo(symbol, MODE_BID) + takeprofit * MarketInfo(symbol, MODE_POINT);
    else // if (type == OP_SELL)
        takeProfitPrice = MarketInfo(symbol, MODE_ASK) - takeprofit * MarketInfo(symbol, MODE_POINT);

    takeProfitPrice = NormalizeDouble(takeProfitPrice, MarketInfo(symbol, MODE_DIGITS));

    return (takeProfitPrice);
}

///
/// Calculates the Stop Loss price depending on the position direction and the Stop Loss distance.
///
double GetStopLossPrice(string symbol, int type, double lots, double stoploss)
{
    double stopLossPrice = 0;

    if (stoploss == 0)
        return (stopLossPrice);

    if (stoploss < MarketInfo(symbol, MODE_STOPLEVEL))
        stoploss = MarketInfo(symbol, MODE_STOPLEVEL);

    if (type == OP_BUY)
        stopLossPrice = MarketInfo(symbol, MODE_BID) - stoploss * MarketInfo(symbol, MODE_POINT);
    else // if (type == OP_SELL)
        stopLossPrice = MarketInfo(symbol, MODE_ASK) + stoploss * MarketInfo(symbol, MODE_POINT);

    stopLossPrice = NormalizeDouble(stopLossPrice, MarketInfo(symbol, MODE_DIGITS));

    return (stopLossPrice);
}

///
/// Corrects Take Profit price
///
double CorrectTakeProfitPrice(string symbol, int type, double takeProfitPrice)
{
    if (takeProfitPrice == 0)
        return (takeProfitPrice);

    double bid = MarketInfo(symbol, MODE_BID);
    double ask = MarketInfo(symbol, MODE_ASK);
    double point = MarketInfo(symbol, MODE_POINT);
    double stoplevel = MarketInfo(symbol, MODE_STOPLEVEL);
    double minTPPrice;

    if (type == OP_BUY)
    {
        minTPPrice = bid + point * stoplevel;
        if (takeProfitPrice < minTPPrice)
            takeProfitPrice = minTPPrice;
    }
    else // if (type == OP_SELL)
    {
        minTPPrice = ask - point * stoplevel;
        if (takeProfitPrice > minTPPrice)
            takeProfitPrice = minTPPrice;
    }

    takeProfitPrice = NormalizeDouble(takeProfitPrice, MarketInfo(symbol, MODE_DIGITS));

    return (takeProfitPrice);
}

///
/// Corrects Stop Loss price
///
double CorrectStopLossPrice(string symbol, int type, double stopLossPrice)
{
    if (stopLossPrice == 0)
        return (stopLossPrice);

    double bid = MarketInfo(symbol, MODE_BID);
    double ask = MarketInfo(symbol, MODE_ASK);
    double point = MarketInfo(symbol, MODE_POINT);
    double stoplevel = MarketInfo(symbol, MODE_STOPLEVEL);
    double minSLPrice;

    if (type == OP_BUY)
    {
        minSLPrice = bid - point * stoplevel;
        if (stopLossPrice > minSLPrice)
            stopLossPrice = minSLPrice;
    }
    else // if (type == OP_SELL)
    {
        minSLPrice = ask + point * stoplevel;
        if (stopLossPrice < minSLPrice)
            stopLossPrice = minSLPrice;
    }

    stopLossPrice = NormalizeDouble(stopLossPrice, MarketInfo(symbol, MODE_DIGITS));

    return (stopLossPrice);
}

///
/// Normalizes an entry order's size.
///
double NormalizeEntrySize(string symbol, double size)
{
    double minlot  = MarketInfo(symbol, MODE_MINLOT);
    double maxlot  = MarketInfo(symbol, MODE_MAXLOT);
    double lotstep = MarketInfo(symbol, MODE_LOTSTEP);

    if (size <= 0)
        return (0);

    double steps = NormalizeDouble((size - minlot) / lotstep, 0);
    size = minlot + steps * lotstep;

    if (size <= minlot)
        return (minlot);

    if (size >= maxlot)
        return (maxlot);

    return (size);
}

///
/// Checks and sets Max SL.
///
void SetMaxStopLoss(string symbol)
{
    double point  = MarketInfo(symbol, MODE_POINT);
    double digits = MarketInfo(symbol, MODE_DIGITS);

    for (int i = 0; i < OrdersTotal(); i++)
    {
        if (!OrderSelect(i, SELECT_BY_POS, MODE_TRADES))
        {
            LastError = GetLastError();
            Print("Error with OrderSelect: ", GetErrorDescription(LastError));
            continue;
        }

        if (OrderMagicNumber() != Expert_Magic || OrderSymbol() != symbol)
            continue;

        int type = OrderType();
        if (type != OP_BUY && type != OP_SELL)
            continue;

        int    orderTicket     = OrderTicket();
        double posOpenPrice    = OrderOpenPrice();
        double stopLossPrice   = OrderStopLoss();
        double takeProfitPrice = OrderTakeProfit();
        double stopLossPips    = MathAbs(posOpenPrice - stopLossPrice) / point;

        if (stopLossPrice == 0 || stopLossPips > Protection_Max_StopLoss + 0.01)
        {
            if (type == OP_BUY)
                stopLossPrice = NormalizeDouble(posOpenPrice - point * Protection_Max_StopLoss, digits);
            else if (type == OP_SELL)
                stopLossPrice = NormalizeDouble(posOpenPrice + point * Protection_Max_StopLoss, digits);
            stopLossPrice = CorrectStopLossPrice(symbol, type, stopLossPrice);
            if (ModifyPositionByTicket(symbol, orderTicket, stopLossPrice, takeProfitPrice) > 0)
                Print("Max Stop Loss (", Protection_Max_StopLoss, " ) set Max Stop Loss to ",  stopLossPrice);
        }
    }
}

///
/// Sets a BreakEven stop of current positions.
///
void SetBreakEvenStop(string symbol)
{
    if (SetAggregatePosition(symbol) <= 0)
        return;

    double breakeven = MarketInfo(symbol, MODE_STOPLEVEL);
    if (breakeven < BreakEven)
        breakeven = BreakEven;

    double breakprice = 0; // Break Even price including commission.
    double commission = 0; // Commission in pips.
    if (PositionCommission != 0)
        commission = MathAbs(PositionCommission) / MarketInfo(symbol, MODE_TICKVALUE);

    double point  = MarketInfo(symbol, MODE_POINT);
    double digits = MarketInfo(symbol, MODE_DIGITS);

    if (PositionType == OP_BUY)
    {
        double bid = MarketInfo(symbol, MODE_BID);
        breakprice = NormalizeDouble(PositionOpenPrice + point * commission / PositionLots, digits);
        if (bid - breakprice >= point * breakeven)
            if (PositionStopLoss < breakprice)
            {
                SetStopLossAndTakeProfit(symbol, breakprice, PositionTakeProfit);
                Print("Break Even (", BreakEven, " pips) set Stop Loss to ",  breakprice, ", Bid = ", bid);
            }
    }
    else if (PositionType == OP_SELL)
    {
        double ask = MarketInfo(symbol, MODE_ASK);
        breakprice = NormalizeDouble(PositionOpenPrice - point * commission / PositionLots, digits);
        if (breakprice - ask >= point * breakeven)
            if (PositionStopLoss == 0 || PositionStopLoss > breakprice)
            {
                SetStopLossAndTakeProfit(symbol, breakprice, PositionTakeProfit);
                Print("Break Even (", BreakEven, " pips) set Stop Loss to ",  breakprice, ", Ask = ", ask);
            }
    }
}

///
/// Sets a Trailing Stop of current positions.
///
void SetTrailingStop(string symbol, bool isNewBar)
{
    bool isCheckTS = true;

    if (isNewBar)
    {
       if (PositionType == OP_BUY && PositionTime > barHighTime)
            isCheckTS = false;

       if (PositionType == OP_SELL && PositionTime > barLowTime)
            isCheckTS = false;

        barHighTime = Time[0];
        barLowTime  = Time[0];
        currentBarHigh = High[0];
        currentBarLow  = Low[0];
    }
    else
    {
        if (High[0] > currentBarHigh)
        {
            currentBarHigh = High[0];
            barHighTime = Time[0];
        }
        if (Low[0] < currentBarLow)
        {
            currentBarLow = Low[0];
            barLowTime = Time[0];
        }
    }

    if (SetAggregatePosition(symbol) <= 0)
        return;

    if (TrailingMode == "tick")
        SetTrailingStopTickMode(symbol);
    else if (TrailingMode == "bar" && isNewBar && isCheckTS)
        SetTrailingStopBarMode(symbol);

    return;
}

///
/// Sets a Trailing Stop Bar Mode of current positions.
///
void SetTrailingStopBarMode(string symbol)
{
    double point = MarketInfo(symbol, MODE_POINT);
    double stoplevel = MarketInfo(symbol, MODE_STOPLEVEL);

    if (PositionType == OP_BUY)
    {   // Long position
        double bid = MarketInfo(symbol, MODE_BID);
        double stoploss = High[1] - point * TrailingStop;
        if (PositionStopLoss < stoploss)
        {
            if (stoploss < bid)
            {
                if (stoploss > bid - point * stoplevel)
                    stoploss = bid - point * stoplevel;

                SetStopLossAndTakeProfit(symbol, stoploss, PositionTakeProfit);
                Print("Trailing Stop (", TrailingStop, " pips) moved to: ", stoploss, ", Bid = ", bid);
            }
            else
            {
                bool isSucceed = CloseCurrentPosition(symbol, stoplevel) == 0;
                int lastErrorOrdClose = GetLastError();
                lastErrorOrdClose = IF_I(lastErrorOrdClose > 0, lastErrorOrdClose, LastError);
                if (!isSucceed) Print("Error in OrderClose: ", GetErrorDescription(lastErrorOrdClose));
            }
        }
    }
    else if (PositionType == OP_SELL)
    {   // Short position
        double ask = MarketInfo(symbol, MODE_ASK);
        stoploss = Low[1] + point * TrailingStop;
        if (PositionStopLoss > stoploss)
        {
            if (stoploss > ask)
            {
                if (stoploss < ask + point * stoplevel)
                    stoploss = ask + point * stoplevel;

                SetStopLossAndTakeProfit(symbol, stoploss, PositionTakeProfit);
                Print("Trailing Stop (", TrailingStop, " pips) moved to: ", stoploss, ", Ask = ", ask);
            }
            else
            {
                isSucceed = CloseCurrentPosition(symbol, stoplevel) == 0;
                lastErrorOrdClose = GetLastError();
                lastErrorOrdClose = IF_I(lastErrorOrdClose > 0, lastErrorOrdClose, LastError);
                if (!isSucceed) Print("Error in OrderClose: ", GetErrorDescription(lastErrorOrdClose));
            }
        }
    }
}

///
/// Sets a Trailing Stop Tick Mode of current positions.
///
void SetTrailingStopTickMode(string symbol)
{
    double point = MarketInfo(symbol, MODE_POINT);

    if (PositionType == OP_BUY)
    {   // Long position
        double bid = MarketInfo(symbol, MODE_BID);
        if (bid >= PositionOpenPrice + point * TrailingStop)
            if (PositionStopLoss < bid - point * (TrailingStop + TrailingStop_Moving_Step))
            {
                double stoploss = bid - point * TrailingStop;
                SetStopLossAndTakeProfit(symbol, stoploss, PositionTakeProfit);
                Print("Trailing Stop (", TrailingStop, " pips) moved to: ", stoploss, ", Bid = ", bid);
            }
    }
    else if (PositionType == OP_SELL)
    {   // Short position
        double ask = MarketInfo(symbol, MODE_ASK);
        if (PositionOpenPrice - ask >= point * TrailingStop)
            if (PositionStopLoss > ask + point * (TrailingStop + TrailingStop_Moving_Step))
            {
                stoploss = ask + point * TrailingStop;
                SetStopLossAndTakeProfit(symbol, stoploss, PositionTakeProfit);
                Print("Trailing Stop (", TrailingStop, " pips) moved to: ", stoploss, ", Ask = ", ask);
            }
    }
}

//
// Parses the parameters and sets global variables.
//
void ParseOrderParameters(string parameters)
{
    string param[];

    SplitString(parameters, ";", param, 2);
    if (StringSubstr(param[0], 0, 3) == "TS0")
    {
        TrailingStop = StrToInteger(StringSubstr(param[0], 4));
        TrailingMode = "bar";
    }
    if (StringSubstr(param[0], 0, 3) == "TS1")
    {
        TrailingStop = StrToInteger(StringSubstr(param[0], 4));
        TrailingMode = "tick";
    }
    if (StringSubstr(param[1], 0, 3) == "BRE")
        BreakEven = StrToInteger(StringSubstr(param[1], 4));

    Print("Trailing Stop = ", TrailingStop, ", Mode - ", TrailingMode, ", Break Even = ", BreakEven);

    return;
}

// ===========================================================================

///
/// Answers to a ping from Forex Strategy Trader.
///
void GetPing(string symbol)
{
    double rates[][6];
    if (ArrayCopyRates(rates) != Bars)
        return (false);

    SetAggregatePosition(symbol);

    double bid     = MarketInfo(symbol, MODE_BID);
    double ask     = MarketInfo(symbol, MODE_ASK);
    int    spread  = MathRound(MarketInfo(symbol, MODE_SPREAD));
    double tickval = MarketInfo(symbol, MODE_TICKVALUE);

    FST_Ping(Connection_ID, symbol, Period(), TimeCurrent(), bid, ask, spread, tickval, rates, Bars,
        AccountBalance(), AccountEquity(), AccountProfit(), AccountFreeMargin(),
        PositionTicket, PositionType, PositionLots, PositionOpenPrice, PositionTime, PositionStopLoss, PositionTakeProfit, PositionProfit, PositionComment);

   return;
}

///
/// Sends a tick to Forex Strategy Trader.
///
int SendTick(string symbol)
{
    double rates[][6];
    if (ArrayCopyRates(rates) != Bars)
        return (false);

    SetAggregatePosition(symbol);

    double bid     = MarketInfo(symbol, MODE_BID);
    double ask     = MarketInfo(symbol, MODE_ASK);
    int    spread  = MathRound(MarketInfo(symbol, MODE_SPREAD));
    double tickval = MarketInfo(symbol, MODE_TICKVALUE);

    int responce = FST_Tick(Connection_ID, symbol, Period(), TimeCurrent(), bid, ask, spread, tickval, rates, Bars,
        AccountBalance(), AccountEquity(), AccountProfit(), AccountFreeMargin(),
        PositionTicket, PositionType, PositionLots, PositionOpenPrice, PositionTime, PositionStopLoss, PositionTakeProfit, PositionProfit, PositionComment);

    return (responce);
}

///
/// Sends the bars to Forex Strategy Trader.
///
void GetBars(string symbol, int period, int barsNecessary)
{
    CheckChartBarsNumber(symbol, period, barsNecessary);

    RefreshRates();
    double rates[][6];
    int bars = ArrayCopyRates(rates, symbol, period);
    FST_Bars(Connection_ID, symbol, period, rates, bars);
}

//
// Checks if the chart contains enough bars.
//
bool CheckChartBarsNumber(string symbol, int period, int barsNecessary)
{
    int    bars = 0;
    double rates[][6];

    for (int attempt = 0; attempt < 10; attempt++)
    {
        RefreshRates();
        bars = ArrayCopyRates(rates, symbol, period);
        if (bars < barsNecessary && GetLastError() == 4066)
        {
            Comment("Loading...");
            Sleep(500);
        }
        else
            break;

        if (IsStopped())
            break;
    }

    if (bars < barsNecessary)
    {
        int hwnd = WindowHandle(Symbol(), Period());
        int maxbars = 0;
        int nullattempts = 0;
        int Key_HOME = 36;

        for (attempt = 0; attempt < 200; attempt++)
        {
            PostMessageA(hwnd, WM_KEYDOWN, Key_HOME, 0);
            PostMessageA(hwnd, WM_KEYUP,   Key_HOME, 0);
            Sleep(100);

            RefreshRates();
            bars = ArrayCopyRates(rates, symbol, period);

            if (bars > barsNecessary)
            {
                Comment("Loaded bars: ", bars);
                break;
            }

            if (nullattempts > 40)
                break;

            if (IsStopped())
                break;

            nullattempts++;
            if (maxbars < bars)
            {
                nullattempts = 0;
                maxbars = bars;
                Comment("Loading... ", bars, " of ", barsNecessary);
            }
        }
    }

    if (bars < barsNecessary)
    {
        string message = "There isn\'t enough bars. FST needs minimum " + barsNecessary + " bars for this time frame. Currently " + bars + " bars are loaded.";
        Comment(message);
        Print(message);
    }

     return (bars >= barsNecessary);
}

///
/// Sends all the market information.
///
void GetMarketInfoAll(string symbol)
{
    FST_MarketInfoAll(Connection_ID,
          MarketInfo(symbol, MODE_POINT             ),
          MarketInfo(symbol, MODE_DIGITS            ),
          MarketInfo(symbol, MODE_SPREAD            ),
          MarketInfo(symbol, MODE_STOPLEVEL         ),
          MarketInfo(symbol, MODE_LOTSIZE           ),
          MarketInfo(symbol, MODE_TICKVALUE         ),
          MarketInfo(symbol, MODE_TICKSIZE          ),
          MarketInfo(symbol, MODE_SWAPLONG          ),
          MarketInfo(symbol, MODE_SWAPSHORT         ),
          MarketInfo(symbol, MODE_STARTING          ),
          MarketInfo(symbol, MODE_EXPIRATION        ),
          MarketInfo(symbol, MODE_TRADEALLOWED      ),
          MarketInfo(symbol, MODE_MINLOT            ),
          MarketInfo(symbol, MODE_LOTSTEP           ),
          MarketInfo(symbol, MODE_MAXLOT            ),
          MarketInfo(symbol, MODE_SWAPTYPE          ),
          MarketInfo(symbol, MODE_PROFITCALCMODE    ),
          MarketInfo(symbol, MODE_MARGINCALCMODE    ),
          MarketInfo(symbol, MODE_MARGININIT        ),
          MarketInfo(symbol, MODE_MARGINMAINTENANCE ),
          MarketInfo(symbol, MODE_MARGINHEDGED      ),
          MarketInfo(symbol, MODE_MARGINREQUIRED    ),
          MarketInfo(symbol, MODE_FREEZELEVEL       )
    );

    return;
}

///
/// Customizes the expert for some brokers.
///
void SetBrokersCompatibility()
{
    // Chek broker.
    string broker = AccountCompany();

    // FXCM
    if (StringSubstr(broker, 0, 4) == "FXCM")
        Separated_SL_TP = true;

    // FXOpen
    else if (StringSubstr(broker, 0, 6) == "FXOpen")
        Separated_SL_TP = true;

    // ODL
    else if (StringSubstr(broker, 0, 3) == "ODL")
        Separated_SL_TP = true;

    // BenchMark
    else if (StringSubstr(broker, 0, 9) == "BenchMark")
        Separated_SL_TP = true;

    // Dukascopy
    else if (StringSubstr(broker, 0, 9) == "Dukascopy")
        Separated_SL_TP = true;

    // MB Trading
    else if (StringSubstr(broker, 0, 2) == "MB")
        Separated_SL_TP = true;

    // Vantage FX
    else if (StringSubstr(broker, 0, 7) == "Vantage")
        Separated_SL_TP = true;

    // Axis Trader
    else if (StringSubstr(broker, 0, 4) == "Axis")
        Separated_SL_TP = true;

    if (Separated_SL_TP)
        Print(broker, " marked for late stops sending.");

    return;
}

///
/// Checks the global variable in order to determine if
/// the server is running on another chart.
///
bool GetServerSema()
{
    if (GlobalVariableCheck(SERVER_SEMA_NAME + Connection_ID))
    {   // Global variable exists.
        double value = GlobalVariableGet(SERVER_SEMA_NAME + Connection_ID);
        if (value == 0)
        {   // Error in GlobalVariableGet.
            Print("Error in GlobalVariableGet: ", GetLastError());
            return (SaveServerSema());
        }
        else if (value < TimeLocal() - GetTickCount() / 1000)
        {   // Global variable has been set before Windows was started.
            return (SaveServerSema());
        }
        else
        {   // Server is working on another chart.
            return (false);
        }
    }

    // Global variable doesn't exist.
    return (SaveServerSema());
}

///
/// Releases the global variable when server stops.
///
void ReleaseServerSema()
{
    if (IsServer)
        GlobalVariableDel(SERVER_SEMA_NAME + Connection_ID);
}

///
/// Saves a global variable when server starts.
///
bool SaveServerSema()
{
    if (GlobalVariableSet(SERVER_SEMA_NAME + Connection_ID, TimeLocal()) != 0)
    {   // Global variable successfully set.
        return (true);
    }
    else
    {
        Print("Error in GlobalVariableSet: ", GetErrorDescription(GetLastError()));
        return (false);
    }
}

///
/// Sets a global variable to show that the trade content is busy.
///
bool GetTradeContext()
{
    int start = GetTickCount();
    GetLastError();

    while (!GlobalVariableSetOnCondition(TRADE_SEMA_NAME, 1, 0))
    {
        int gle = GetLastError();
        if (gle != 0)
            Print("GTC: Error in GlobalVariableSetOnCondition: ", GetErrorDescription(gle));

        if (IsStopped())
        {
            Print("GTC: Expert was stopped!");
            return (false);
        }

        if (GetTickCount() - start > TRADE_SEMA_TIMEOUT)
        {
            Print("GTC: Timeout!");
            return (false);
        }

        Sleep(TRADE_SEMA_WAIT);
    }

    return (true);
}

///
/// Releases the global variable about the trade content.
///
void ReleaseTradeContext()
{
    while (!GlobalVariableSet(TRADE_SEMA_NAME, 0))
    {
        int gle = GetLastError();
        if (gle != 0)
            Print("RTC: Error in GlobalVariableSet: ", GetErrorDescription(gle));

        if (IsStopped())
            return;

        Sleep(TRADE_SEMA_WAIT);
    }
}

///
/// Returns the text meaning of an error code.
///
string GetErrorDescription(int lastError)
{
    string errorDescription;

    switch(lastError)
    {
        case 0:
        case 1:    errorDescription = "No error";                                                 break;
        case 2:    errorDescription = "Common error";                                             break;
        case 3:    errorDescription = "Invalid trade parameters";                                 break;
        case 4:    errorDescription = "Trade server is busy";                                     break;
        case 5:    errorDescription = "Old version of the client terminal";                       break;
        case 6:    errorDescription = "No connection with trade server";                          break;
        case 7:    errorDescription = "Not enough rights";                                        break;
        case 8:    errorDescription = "Too frequent requests";                                    break;
        case 9:    errorDescription = "Malfunctional trade operation (never returned error)";     break;
        case 64:   errorDescription = "Account disabled";                                         break;
        case 65:   errorDescription = "Invalid account";                                          break;
        case 128:  errorDescription = "Trade timeout";                                            break;
        case 129:  errorDescription = "Invalid price";                                            break;
        case 130:  errorDescription = "Invalid stops";                                            break;
        case 131:  errorDescription = "Invalid trade volume";                                     break;
        case 132:  errorDescription = "Market is closed";                                         break;
        case 133:  errorDescription = "Trade is disabled";                                        break;
        case 134:  errorDescription = "Not enough money";                                         break;
        case 135:  errorDescription = "Price changed";                                            break;
        case 136:  errorDescription = "Off quotes";                                               break;
        case 137:  errorDescription = "Broker is busy (never returned error)";                    break;
        case 138:  errorDescription = "Requote";                                                  break;
        case 139:  errorDescription = "Order is locked";                                          break;
        case 140:  errorDescription = "Long positions only allowed";                              break;
        case 141:  errorDescription = "Too many requests";                                        break;
        case 145:  errorDescription = "Modification denied because order too close to market";    break;
        case 146:  errorDescription = "Trade context is busy";                                    break;
        case 147:  errorDescription = "Expirations are denied by broker";                         break;
        case 148:  errorDescription = "Amount of open and pending orders has reached the limit";  break;
        case 149:  errorDescription = "Opening of an opposite position (hedging) is disabled";    break;
        case 150:  errorDescription = "An attempt to close a position contravening the FIFO rule";break;
        case 4000: errorDescription = "No error (never generated code)";                          break;
        case 4001: errorDescription = "Wrong function pointer";                                   break;
        case 4002: errorDescription = "Array index is out of range";                              break;
        case 4003: errorDescription = "No memory for function call stack";                        break;
        case 4004: errorDescription = "Recursive stack overflow";                                 break;
        case 4005: errorDescription = "Not enough stack for parameter";                           break;
        case 4006: errorDescription = "No memory for parameter string";                           break;
        case 4007: errorDescription = "No memory for temp string";                                break;
        case 4008: errorDescription = "Not initialized string";                                   break;
        case 4009: errorDescription = "Not initialized string in array";                          break;
        case 4010: errorDescription = "No memory for array\' string";                             break;
        case 4011: errorDescription = "Too long string";                                          break;
        case 4012: errorDescription = "Remainder from zero divide";                               break;
        case 4013: errorDescription = "Zero divide";                                              break;
        case 4014: errorDescription = "Unknown command";                                          break;
        case 4015: errorDescription = "Wrong jump (never generated error)";                       break;
        case 4016: errorDescription = "Not initialized array";                                    break;
        case 4017: errorDescription = "Dll calls are not allowed";                                break;
        case 4018: errorDescription = "Cannot load library";                                      break;
        case 4019: errorDescription = "Cannot call function";                                     break;
        case 4020: errorDescription = "Expert function calls are not allowed";                    break;
        case 4021: errorDescription = "Not enough memory for temp string returned from function"; break;
        case 4022: errorDescription = "System is busy (never generated error)";                   break;
        case 4050: errorDescription = "Invalid function parameters count";                        break;
        case 4051: errorDescription = "Invalid function parameter value";                         break;
        case 4052: errorDescription = "String function internal error";                           break;
        case 4053: errorDescription = "Some array error";                                         break;
        case 4054: errorDescription = "Incorrect series array using";                             break;
        case 4055: errorDescription = "Custom indicator error";                                   break;
        case 4056: errorDescription = "Arrays are incompatible";                                  break;
        case 4057: errorDescription = "Global variables processing error";                        break;
        case 4058: errorDescription = "Global variable not found";                                break;
        case 4059: errorDescription = "Function is not allowed in testing mode";                  break;
        case 4060: errorDescription = "Function is not confirmed";                                break;
        case 4061: errorDescription = "Send mail error";                                          break;
        case 4062: errorDescription = "String parameter expected";                                break;
        case 4063: errorDescription = "Integer parameter expected";                               break;
        case 4064: errorDescription = "Double parameter expected";                                break;
        case 4065: errorDescription = "Array as parameter expected";                              break;
        case 4066: errorDescription = "Requested history data in update state";                   break;
        case 4099: errorDescription = "End of file";                                              break;
        case 4100: errorDescription = "Some file error";                                          break;
        case 4101: errorDescription = "Wrong file name";                                          break;
        case 4102: errorDescription = "Too many opened files";                                    break;
        case 4103: errorDescription = "Cannot open file";                                         break;
        case 4104: errorDescription = "Incompatible access to a file";                            break;
        case 4105: errorDescription = "No order selected";                                        break;
        case 4106: errorDescription = "Unknown symbol";                                           break;
        case 4107: errorDescription = "Invalid price parameter for trade function";               break;
        case 4108: errorDescription = "Invalid ticket";                                           break;
        case 4109: errorDescription = "Trade is not allowed in the expert properties";            break;
        case 4110: errorDescription = "Longs are not allowed in the expert properties";           break;
        case 4111: errorDescription = "Shorts are not allowed in the expert properties";          break;
        case 4200: errorDescription = "Object is already exist";                                  break;
        case 4201: errorDescription = "Unknown object property";                                  break;
        case 4202: errorDescription = "Object is not exist";                                      break;
        case 4203: errorDescription = "Unknown object type";                                      break;
        case 4204: errorDescription = "No object name";                                           break;
        case 4205: errorDescription = "Object coordinates error";                                 break;
        case 4206: errorDescription = "No specified subwindow";                                   break;
        default:   errorDescription = "Unknown error";
    }

    return (errorDescription);
}

int IF_I(bool condition, int trueVal, int falseVal)
{
    if (condition)
        return (trueVal);
    else
        return (falseVal);
}

double IF_D(bool condition, double trueVal, double falseVal)
{
    if (condition)
        return (trueVal);
    else
        return (falseVal);
}

/// Helper function for parsing a command string.
/// Source: OpenForexPlatform.com
///
bool SplitString(string stringValue, string separatorSymbol, string& results[], int expectedResultCount = 0)
{
   if (StringFind(stringValue, separatorSymbol) < 0)
   {  // No separators found, the entire string is the result.
      ArrayResize(results, 1);
      results[0] = stringValue;
   }
   else
   {
      int separatorPos    = 0;
      int newSeparatorPos = 0;
      int size = 0;

      while(newSeparatorPos > -1)
      {
         size = size + 1;
         newSeparatorPos = StringFind(stringValue, separatorSymbol, separatorPos);

         ArrayResize(results, size);
         if (newSeparatorPos > -1)
         {
            if (newSeparatorPos - separatorPos > 0)
            {  // Evade filling empty positions, since 0 size is considered by the StringSubstr as entire string to the end.
               results[size - 1] = StringSubstr(stringValue, separatorPos, newSeparatorPos - separatorPos);
            }
         }
         else
         {  // Reached final element.
            results[size - 1] = StringSubstr(stringValue, separatorPos, 0);
         }

         //Alert(results[size-1]);
         separatorPos = newSeparatorPos + 1;
      }
   }

   if (expectedResultCount == 0 || expectedResultCount == ArraySize(results))
   {  // Results OK.
      return (true);
   }
   else
   {  // Results are WRONG.
      Print("ERROR - size of parsed string not expected.", true);
      return (false);
   }
}

