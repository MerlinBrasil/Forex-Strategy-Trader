// Bars Manager
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;

namespace MT4Bridge
{
    public class Series<T> : List<T>
    {
        object locker = new object();

        public new T this[int index]
        {
            get { lock (locker) return base[index]; }
            set { lock (locker) base[index] = value; }
        }

        internal new int Count
        {
            get { lock (locker) return base.Count; }
        }

        internal new void Add(T item)
        {
            lock (locker) base.Add(item); 
        }
    }

    public class Bars
    {
        string           symbol;
        PeriodType       period;
        Series<DateTime> time;
        Series<double>   open, high, low, close;
        Series<int>      volume;

        public string     Symbol { get { return symbol;     } }
        public PeriodType Period { get { return period;     } }
        public int        Count  { get { return time.Count; } }

        public Series<DateTime> Time   { get { return time;   } }
        public Series<double>   Open   { get { return open;   } }
        public Series<double>   High   { get { return high;   } }
        public Series<double>   Low    { get { return low;    } }
        public Series<double>   Close  { get { return close;  } }
        public Series<int>      Volume { get { return volume; } }

        public Bars(string symbol, PeriodType period)
        {
            this.symbol = symbol;
            this.period = period;
            time   = new Series<DateTime>();
            open   = new Series<double>();
            high   = new Series<double>();
            low    = new Series<double>();
            close  = new Series<double>();
            volume = new Series<int>();
        }

        internal void Add(DateTime time, double open, double high, double low, double close, int volume)
        {
            this.time.Add(time);
            this.open.Add(open);
            this.high.Add(high);
            this.low.Add(low);
            this.close.Add(close);
            this.volume.Add(volume);
        }

        internal void Update(int index, DateTime time, double open, double high, double low, double close, int volume)
        {
            this.time[index]   = time;
            this.open[index]   = open;
            this.high[index]   = high;
            this.low[index]    = low;
            this.close[index]  = close;
            this.volume[index] = volume;
        }

        internal void Insert(int index, DateTime time, double open, double high, double low, double close, int volume)
        {
            this.time.Insert(index, time);
            this.open.Insert(index, open);
            this.high.Insert(index, high);
            this.low.Insert(index, low);
            this.close.Insert(index, close);
            this.volume.Insert(index, volume);
        }

        internal void Insert(DateTime time, double open, double high, double low, double close, int volume)
        {
            int i;
            for (i = 0; i < Count; i++) {
                if (this.time[i] == time) {
                    Update(i, time, open, high, low, close, volume);
                    return;
                }
                if (this.time[i] > time)
                    break;
            }
            if (i >= Count)
                Add(time, open, high, low, close, volume);
            else
                Insert(i, time, open, high, low, close, volume);
        }

        internal void UpdateLast(DateTime time, double open, double high, double low, double close, int volume)
        {
            if (Count <= 0 || this.time[Count - 1] != time)
                Add(time, open, high, low, close, volume);
            else
                Update(Count - 1, time, open, high, low, close, volume);
        }

        internal void Merge(Bars bars)
        {
            for (int i = 0; i < bars.Count; i++)
                Insert(bars.time[i], bars.open[i], bars.high[i], bars.low[i], bars.close[i], bars.volume[i]);
        }

        internal void SetLenght(int lenght)
        {
            int currentlenght = Count;
            if(currentlenght > lenght)
            {
                this.time.RemoveRange(0, currentlenght - lenght);
                this.open.RemoveRange(0, currentlenght - lenght);
                this.high.RemoveRange(0, currentlenght - lenght);
                this.low.RemoveRange(0, currentlenght - lenght);
                this.close.RemoveRange(0, currentlenght - lenght);
                this.volume.RemoveRange(0, currentlenght - lenght);
            }

            return;
        }
    }

    internal class BarsManager
    {
        Bars   bars;
        object locker = new object();

        int MaxBarsCount(PeriodType period)
        {
            return Math.Max(2 * 1440 / (int)period + 10, Forex_Strategy_Trader.Configs.MinChartBars); // Hack !!!!!!!!!!!!!!!!!!!!!!!!!!!
        }

        bool CheckBars(string symbol, PeriodType period, DateTime barTime, DateTime barTime10)
        {
            if (bars == null || bars.Symbol != symbol || bars.Period != period)
            {
                bars = new Bars(symbol, period);
                return false;
            }

            if (barTime10 > new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc))
            {
                if (bars.Count > 10 && bars.Time[bars.Count - 1] == barTime && bars.Time[bars.Count - 11] != barTime10)
                {
                    bars = new Bars(symbol, period);
                    return false;
                }
            }

           return true;
        }

        public void UpdateBar(string symbol, PeriodType period, DateTime time, double open, double high, double low, double close, int volume, DateTime bartime10)
        {
            lock (locker)
            {
                CheckBars(symbol, period, time, bartime10);
                {
                    bars.UpdateLast(time, open, high, low, close, volume);
                    bars.SetLenght(MaxBarsCount(period));
                }
            }
        }

        public Bars GetBars(string symbol, PeriodType period, Client client)
        {
            if (bars != null && bars.Symbol == symbol && bars.Period == period && bars.Count >= MaxBarsCount(period))
                return bars;

            if (bars == null || bars.Symbol != symbol || bars.Period != period)
                bars = new Bars(symbol, period);

            while (bars.Count < MaxBarsCount(period))
            {
                int count = MaxBarsCount(period);
                Bars mt4bars = client.GetBars(symbol, period, ref count, bars.Count);
                if (mt4bars == null)
                    return bars.Count > 0 ? bars : null;

                bars.Merge(mt4bars);
                if (bars.Count >= count)
                    break;
            }

            bars.SetLenght(MaxBarsCount(period));

            return bars;
        }
    }
}
