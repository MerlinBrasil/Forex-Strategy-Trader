// Registrar Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.ComponentModel;
using System.IO;
using System.Net;

namespace Forex_Strategy_Trader
{
    class Registrar
    {
        public Registrar()
        {
        }

        public void Register()
        {
            if (!Configs.IsInstalled)
            {
                BackgroundWorker bgWorker = new BackgroundWorker();
                bgWorker.DoWork += new DoWorkEventHandler(DoWork);
                bgWorker.RunWorkerAsync();
            }
        }

        void DoWork(object sender, DoWorkEventArgs e)
        {
            string fileURL = "http://forexsb.com/products/progstats.php";
            string region = System.Globalization.RegionInfo.CurrentRegion.EnglishName;
            string command = "?prog=" + "fst" + "&reg=" + region;
            string request = fileURL + command;

            string respond = SetStats(request);

            if (respond.Contains("ok"))
                Configs.IsInstalled = true;
        }


        string SetStats(string request)
        {
            string respond;

            try
            {
                Uri url = new Uri(request);
                WebClient webClient = new WebClient();
                Stream data = webClient.OpenRead(request);
                StreamReader reader = new StreamReader(data);
                respond = reader.ReadToEnd();
                data.Close();
                reader.Close();
            }
            catch (WebException we)
            {
                respond = we.Message;
            }

            return respond;
        }
    }
}
