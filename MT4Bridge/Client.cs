// Client
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using MT4Bridge.NamedPipes;

namespace MT4Bridge
{
    internal class Client
    {
        static string _userName = System.Windows.Forms.SystemInformation.UserName;
        static string _clientPipeName = "FST-MT4_" + _userName + "-";
        static int    _clientID = 0;

        static string PipeName { get { return _clientPipeName + _clientID.ToString(); } }

        Bridge bridge;

        public Client(Bridge bridge, int id)
        {
            _clientID   = id;
            this.bridge = bridge;
        }

        public string Command(string command)
        {
            using (ClientPipe pipe = new ClientPipe(PipeName)) {
                if (!pipe.Connect())
                    return "ER Cannot connect to pipe server";
                try {
                    return pipe.Command(command);
                } catch (NamedPipes.PipeException e) {
                    return "ER " + e.Message;
                }
            }
        }

        bool ResponseOK(string response)
        {
            return response.ToUpper().StartsWith("OK");
        }

        Response GetResponse(string rc)
        {
            bool ok = ResponseOK(rc);
            if (rc.Length < 4)
                return new Response(ok);

            string[] reply = rc.Substring(3).Split(new char[] { ' ' });
            int code;
            if (reply.Length != 1 || !int.TryParse(reply[0], out code))
                return new Response(ok);
            return new Response(ok, code);
        }

        DateTime FromTimestamp(string timestamp)
        {
            return FromTimestamp(int.Parse(timestamp));
        }

        DateTime FromTimestamp(int timestamp)
        {
            DateTime time = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return time.AddSeconds(timestamp);
        }

        int ToTimestamp(DateTime time)
        {
            DateTime utc = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            if (time < utc)
                return 0;
            return (int)((time - utc).TotalSeconds);
        }

        string Fixstr(string str)
        {
            return str.Replace('|', '\\').Replace('_', ' ');
        }

        /// <summary>
        /// Ping
        /// </summary>
        public PingInfo Ping()
        {
            string rc = Command(string.Format("PI"));
            if (!ResponseOK(rc) || rc.Length < 3)
                return null;

            string[] reply = rc.Substring(3).Split(new char[] { ' ' });
            if (reply.Length != 26)
                return null;

            try {
                string     symbol  = reply[0];
                PeriodType period  = (PeriodType)int.Parse(reply[1]);

                DateTime   time      = FromTimestamp(reply[2]);
                double     bid       = StringToDouble(reply[3]);
                double     ask       = StringToDouble(reply[4]);
                int        spread    = int.Parse(reply[5]);
                double     tickvalue = StringToDouble(reply[6]);

                DateTime   bartime = FromTimestamp(int.Parse(reply[7]));
                double     open    = StringToDouble(reply[8]);
                double     high    = StringToDouble(reply[9]);
                double     low     = StringToDouble(reply[10]);
                double     close   = StringToDouble(reply[11]);
                int        volume  = int.Parse(reply[12]);

                DateTime   bartime10          = FromTimestamp(int.Parse(reply[13]));

                double     accountBalance     = StringToDouble(reply[14]);
                double     accountEquity      = StringToDouble(reply[15]);
                double     accountProfit      = StringToDouble(reply[16]);
                double     accountFreeMargin  = StringToDouble(reply[17]);

                int        positionTicket     = int.Parse(reply[18]);
                int        positionType       = int.Parse(reply[19]);
                double     positionLots       = StringToDouble(reply[20]);
                double     positionOpenPrice  = StringToDouble(reply[21]);
                double     positionStopLoss   = StringToDouble(reply[22]);
                double     positionTakeProfit = StringToDouble(reply[23]);
                double     positionProfit     = StringToDouble(reply[24]);
                string     positionComment    = reply[25];

                bridge.barsManager.UpdateBar(symbol, period, bartime, open, high, low, close, volume, bartime10);

                PingInfo pingInfo = new PingInfo(symbol, period, bartime, time, bid, ask, spread, tickvalue,
                    accountBalance, accountEquity, accountProfit, accountFreeMargin, positionTicket, positionType,
                    positionLots, positionOpenPrice, positionStopLoss, positionTakeProfit, positionProfit, positionComment);

                return pingInfo;
            } catch (FormatException) {
                return null;
            }
        }

        public SymbolInfo Symbol(string symbol)
        {
            string rc = Command(string.Format("SI {0}", symbol));
            if (!ResponseOK(rc) || rc.Length < 3)
                return null;

            string[] reply = rc.Substring(3).Split(new char[] { ' ' });
            if (reply.Length != 7 || reply[0] != symbol)
                return null;

            try {
                return new SymbolInfo(reply[0], StringToDouble(reply[1]), StringToDouble(reply[2]),
                    int.Parse(reply[3]), StringToDouble(reply[4]), StringToDouble(reply[5]), StringToDouble(reply[6]));
            } catch (FormatException) {
                return null;
            }
        }

        public AccountInfo Account()
        {
            string rc = Command("AI");
            if (!ResponseOK(rc) || rc.Length < 3)
                return null;

            string[] reply = rc.Substring(3).Split(new char[] { ' ' });
            if (reply.Length != 16)
                return null;

            try {
                return new AccountInfo(
                    Fixstr(reply[0]),          int.Parse(reply[1]),       Fixstr(reply[2]),
                    Fixstr(reply[3]),          Fixstr(reply[4]),          int.Parse(reply[5]),
                    StringToDouble(reply[6]),  StringToDouble(reply[7]),  StringToDouble(reply[8]),
                    StringToDouble(reply[9]),  StringToDouble(reply[10]), StringToDouble(reply[11]),
                    StringToDouble(reply[12]), int.Parse(reply[13]),      int.Parse(reply[14]),
                    int.Parse(reply[15]) == 1);
            } catch (FormatException) {
                return null;
            }
        }

        public double MarketInfo(string symbol, int mode)
        {
            string rc = Command(string.Format("MI {0} {1}", symbol, mode));
            if (!ResponseOK(rc) || rc.Length < 3)
                return double.NaN;

            string[] reply = rc.Substring(3).Split(new char[] { ' ' });
            if (reply.Length != 3 || reply[0] != symbol)
                return double.NaN;

            try {
                if (int.Parse(reply[1]) != mode)
                    return double.NaN;
                return StringToDouble(reply[2]);
            } catch (FormatException) {
                return double.NaN;
            }
        }

        public MarketInfo MarketInfoAll(string symbol)
        {
            string rc = Command(string.Format("MA {0}", symbol));
            if (!ResponseOK(rc) || rc.Length < 3)
                return null;

            string[] reply = rc.Substring(3).Split(new char[] { ' ' });
            if (reply.Length != 23)
                return null;

            try {
                return new MarketInfo(
                    StringToDouble(reply[0]),  StringToDouble(reply[1]),  StringToDouble(reply[2]),
                    StringToDouble(reply[3]),  StringToDouble(reply[4]),  StringToDouble(reply[5]),
                    StringToDouble(reply[6]),  StringToDouble(reply[7]),  StringToDouble(reply[8]),
                    StringToDouble(reply[9]),  StringToDouble(reply[10]), StringToDouble(reply[11]),
                    StringToDouble(reply[12]), StringToDouble(reply[13]), StringToDouble(reply[14]),
                    StringToDouble(reply[15]), StringToDouble(reply[16]), StringToDouble(reply[17]),
                    StringToDouble(reply[18]), StringToDouble(reply[19]), StringToDouble(reply[20]),
                    StringToDouble(reply[21]), StringToDouble(reply[22]));
            } catch (FormatException) {
                return null;
            }
        }

        public TerminalInfo Terminal()
        {
            string rc = Command("TE");
            if (!ResponseOK(rc) || rc.Length < 3)
                return null;

            string[] reply = rc.Substring(3).Split(new char[] { ' ' });
            if (reply.Length != 5)
                return null;

            try {
                return new TerminalInfo(Fixstr(reply[0]), Fixstr(reply[1]), Fixstr(reply[2]), Fixstr(reply[3]), Fixstr(reply[4]));
            } catch (FormatException) {
                return null;
            }
        }

        public Bars GetBars(string symbol, PeriodType period, ref int count)
        {
            return GetBars(symbol, period, ref count, 0);
        }

        public Bars GetBars(string symbol, PeriodType period, ref int count, int offset)
        {
            string rc = Command(string.Format("BR {0} {1} {2} {3}", symbol, (int)period, offset, count));
            if (!ResponseOK(rc) || rc.Length < 3)
                return null;

            string[] reply = rc.Substring(3).Split(new char[] { ' ' });
            if (reply.Length < 5 || reply[0] != symbol)
                return null;

            int rperiod, rbars, roffset, rcount;
            try {
                rperiod = int.Parse(reply[1]);
                rbars   = int.Parse(reply[2]);
                roffset = int.Parse(reply[3]);
                rcount  = int.Parse(reply[4]);
            } catch (FormatException) {
                return null;
            }
            if (rperiod != (int)period || roffset != offset || rcount * 6 != reply.Length - 5)
                return null;

            count = rbars;
            Bars bars = new Bars(symbol, period);
            for (int i = 5; i < reply.Length; i += 6) {
                try {
                    DateTime time   = FromTimestamp(int.Parse(reply[i]));
                    double   open   = StringToDouble(reply[i + 1]);
                    double   high   = StringToDouble(reply[i + 2]);
                    double   low    = StringToDouble(reply[i + 3]);
                    double   close  = StringToDouble(reply[i + 4]);
                    int      volume = int.Parse(reply[i + 5]);

                    bars.Insert(time, open, high, low, close, volume);
                } catch (FormatException) {
                    return null;
                }
            }
            return bars;
        }

        public int[] Orders(string symbol)
        {
            string cmd = "OR";
            if (symbol != null)
                cmd += " " + symbol;
            string rc = Command(cmd);
            if (!ResponseOK(rc) || rc.Length < 3)
                return null;

            string[] reply = rc.Substring(3).Split(new char[] { ' ' });
            int count, start;
            if (symbol != null) {
                if (reply.Length < 2 || reply[0] != symbol)
                    return null;
                count = int.Parse(reply[1]);
                start = 2;
            } else {
                if (reply.Length < 1)
                    return null;
                count = int.Parse(reply[0]);
                start = 1;
            }

            int[] tickets = new int[count];
            for (int i = 0; i < count; i++)
                tickets[i] = int.Parse(reply[start + i]);
            return tickets;
        }

        public OrderInfo OrderInfo(int ticket)
        {
            string rc = Command(string.Format("OI {0}", ticket));
            if (!ResponseOK(rc) || rc.Length < 3)
                return null;

            string[] reply = rc.Substring(3).Split(new char[] { ' ' });
            if (reply.Length != 13)
                return null;

            return new OrderInfo(
                int.Parse(reply[0]),
                reply[1],
                (OrderType)int.Parse(reply[2]),
                StringToDouble(reply[3]),
                StringToDouble(reply[4]),
                StringToDouble(reply[5]),
                StringToDouble(reply[6]),
                FromTimestamp(reply[7]),
                FromTimestamp(reply[8]), 
                StringToDouble(reply[9]),
                StringToDouble(reply[10]),
                int.Parse(reply[11]),
                FromTimestamp(reply[12])
                );
        }

        public Response OrderSend(string symbol, OrderType type, double lots, double price, int slippage,
                                  double stoploss, double takeprofit, int magic, DateTime expire, string parameters)
        {
            string rc = Command(string.Format("OS {0} {1} {2} {3} {4} {5} {6} {7} {8} {9}", symbol, (int)type,
                                DoubleToString(lots), DoubleToString(price), slippage, DoubleToString(stoploss),
                                DoubleToString(takeprofit), magic, ToTimestamp(expire), parameters));
            return GetResponse(rc);
        }

        public Response OrderModify(int ticket, double price, double stoploss, double takeprofit, DateTime expire, string parameters)
        {
            string cmd = string.Format("OM {0} {1} {2} {3} {4} {5}", ticket, DoubleToString(price), DoubleToString(stoploss), DoubleToString(takeprofit), ToTimestamp(expire), parameters);
            return GetResponse(Command(cmd));
        }

        public Response OrderClose(int ticket, double lots, double price, int slippage)
        {
            string cmd = string.Format("OC {0} {1} {2} {3}", ticket, DoubleToString(lots), DoubleToString(price), slippage);
            return GetResponse(Command(cmd));
        }

        public Response OrderDelete(int ticket)
        {
            string cmd = string.Format("OD {0}", ticket);
            return GetResponse(Command(cmd));
        }

        double StringToDouble(string input)
        {
            string decimalPoint = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;

            if (!input.Contains(decimalPoint))
            {
                input = input.Replace(".", decimalPoint);
                input = input.Replace(",", decimalPoint);
            }

            double number;

            try { 
                number = double.Parse(input);
            } catch { 
                number = double.NaN; 
            }

            return number;
        }

        string DoubleToString(double number)
        {
            string decimalPoint = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            string strNumb = number.ToString();

                strNumb = strNumb.Replace(decimalPoint, ".");

            return strNumb;
        }
    }
}
