// Bar Structure
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Bar structure.
    /// </summary>
    public struct Bar
    {
        DateTime time ; // Open time
        double	 open  ; // Price open
        double	 high  ; // Price high
        double	 low   ; // Price low
        double	 close ; // Price close
        int		 volume; // Volume

        public DateTime	Time   { get { return time  ; } set { time   = value; } }
        public double	Open   { get { return open  ; } set { open   = value; } }
        public double	High   { get { return high  ; } set { high   = value; } }
        public double	Low    { get { return low   ; } set { low    = value; } }
        public double	Close  { get { return close ; } set { close  = value; } }
        public int		Volume { get { return volume; } set { volume = value; } }

        public override string ToString()
        {
            return String.Format("{0:D2}.{1:D2}.{2:D4}\t{3:D2}:{4:D2}\t{5:F5}\t{6:F5}\t{7:F5}\t{8:F5}\t{9:D6}",
                time.Day, time.Month, time.Year, time.Hour, time.Minute, open, high, low, close, volume);
        }
    }
}
