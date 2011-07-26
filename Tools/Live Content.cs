// Live Content
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml;

namespace Forex_Strategy_Trader
{
    public class Live_Content
    {
        string updateFileURL = "http://forexsb.com/products/fst-update.xml";
        string pathUpdateFile;
        string pathSystem;

        List<LinkItem> brokers = new List<LinkItem>();
        List<LinkItem> links   = new List<LinkItem>();

        ToolStripMenuItem miLiveContent;
        ToolStripMenuItem miForex;
        LinkPanel pnlUsefulLinks;
        LinkPanel pnlForexBrokers;
        BackgroundWorker  bgWorker;

        XmlDocument xmlUpdate = new XmlDocument();

        /// <summary>
        /// Public constructor
        /// </summary>
        public Live_Content(string pathSystem, ToolStripMenuItem miLiveContent, ToolStripMenuItem miForex, LinkPanel pnlUsefulLinks, LinkPanel pnlForexBrokers)
        {
            this.pathSystem      = pathSystem;
            this.miLiveContent   = miLiveContent;
            this.miForex         = miForex;
            this.pnlUsefulLinks  = pnlUsefulLinks;
            this.pnlForexBrokers = pnlForexBrokers;

            pathUpdateFile = Path.Combine(pathSystem, "fst-update.xml");

            try
            {
                LoadConfigFile();

                ReadFXBrokers();
                SetBrokersInMenu();
                SetBrokersInLinkPanel();

                ReadLinks();
                SetLinksInLinkPanel();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Config");
            }

            // BackGroundWorker
            bgWorker = new BackgroundWorker();
            bgWorker.DoWork += new DoWorkEventHandler(DoWork);
            bgWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Does the job
        /// </summary>
        void DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                UpdateLiveContentConfig();
                CheckProgramsVersionNumber();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Config");
            }
        }

        /// <summary>
        /// Update the config file if it is necessary
        /// </summary>
        void UpdateLiveContentConfig()
        {
            Uri url = new Uri(updateFileURL);
            WebClient webClient = new WebClient();
            try
            {
                xmlUpdate.LoadXml(webClient.DownloadString(url));
                SaveConfigFile();
            }
            catch { }
        }

        /// <summary>
        /// Load config file 
        /// </summary>
        void LoadConfigFile()
        {
            try
            {
                if (!File.Exists(pathUpdateFile))
                {
                    xmlUpdate = new XmlDocument();
                    xmlUpdate.InnerXml = Properties.Resources.fst_update;
                }
                else
                {
                    xmlUpdate.Load(pathUpdateFile);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Config");
            }

            return;
        }

        /// <summary>
        /// Save config file
        /// </summary>
        void SaveConfigFile()
        {
            try
            {
                xmlUpdate.Save(pathUpdateFile);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Config");
            }

            return;
        }

        /// <summary>
        /// Checks the program version
        /// </summary>
        void CheckProgramsVersionNumber()
        {
            string text = "";

            int iProgramVersion = int.Parse(xmlUpdate.SelectSingleNode("update/versions/release").InnerText);
            if (Configs.CheckForUpdates && iProgramVersion > Data.ProgramID)
            {   // A newer version was published
                text = Language.T("New Version");
            }
            else
            {
                int iBetaVersion = int.Parse(xmlUpdate.SelectSingleNode("update/versions/beta").InnerText);
                if (Configs.CheckForNewBeta && iBetaVersion > Data.ProgramID)
                {   // A newer beta version was published
                    text = Language.T("New Beta");
                }
            }

            if (text != "")
            {
                miLiveContent.Text    = text;
                miLiveContent.Visible = true;
                miLiveContent.Click  += new EventHandler(MenuLiveContentOnClick);
            }
        }

        /// <summary>
        /// Opens the Live Content browser
        /// </summary>
        protected void MenuLiveContentOnClick(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://forexsb.com/");
                HideMenuItemLiveContent();
            }
            catch { }
        }

        /// <summary>
        /// Hides the Live Content menu item.
        /// </summary>
        void HideMenuItemLiveContent()
        {
            miLiveContent.Visible = false;
            miLiveContent.Click  -= new EventHandler(MenuLiveContentOnClick);
        }

        /// <summary>
        /// Reads the FX brokers from the XML file.
        /// </summary>
        void ReadFXBrokers()
        {
            XmlNodeList xmlListBrokers = xmlUpdate.GetElementsByTagName("broker");

            foreach (XmlNode nodeBroker in xmlListBrokers)
            {
                string text     = nodeBroker.SelectSingleNode("text").InnerText;
                string url      = nodeBroker.SelectSingleNode("url").InnerText;
                string comment  = nodeBroker.SelectSingleNode("comment").InnerText;

                brokers.Add(new LinkItem(text, url, comment));
            }
            
            return;
        }

        /// <summary>
        /// Sets the brokers in the menu
        /// </summary>
        void SetBrokersInMenu()
        {
            foreach (LinkItem broker in brokers)
            {
                ToolStripMenuItem miBroker = new ToolStripMenuItem();
                miBroker.Text        = broker.Text + "...";
                miBroker.Image       = Properties.Resources.globe;
                miBroker.Tag         = broker.Url;
                miBroker.ToolTipText = broker.Comment;
                miBroker.Click      += new EventHandler(MenuForexContentsOnClick);

                miForex.DropDownItems.Add(miBroker);
            }

            return;
        }

        /// <summary>
        /// Sets the brokers in the menu
        /// </summary>
        void SetBrokersInLinkPanel()
        {
            foreach (LinkItem broker in brokers)
                pnlForexBrokers.AddLink(broker);

            pnlForexBrokers.SetLinks();

            return;
        }

        /// <summary>
        /// Reads the links from the xml file.
        /// </summary>
        void ReadLinks()
        {
            XmlNodeList xmlListLinks = xmlUpdate.GetElementsByTagName("link");

            foreach (XmlNode link in xmlListLinks)
            {
                string text     = link.SelectSingleNode("text").InnerText;
                string url      = link.SelectSingleNode("url").InnerText;
                string comment  = link.SelectSingleNode("comment").InnerText;

                links.Add(new LinkItem(text, url, comment));
            }
            
            return;
        }

        /// <summary>
        /// Sets the brokers in the menu
        /// </summary>
        void SetLinksInLinkPanel()
        {
            foreach (LinkItem link in links)
                pnlUsefulLinks.AddLink(link);

            pnlUsefulLinks.SetLinks();

            return;
        }

        /// <summary>
        /// Opens the forex news
        /// </summary>
        void MenuForexContentsOnClick(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;

            try
            {
                System.Diagnostics.Process.Start((string)mi.Tag);
            }
            catch { }

            return;
        }
    }
}
