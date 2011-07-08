// Forex Strategy Trader
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Trader
{
    public class LinkItem
    {
        string text;
        string url;
        string comment;

        /// <summary>
        /// Sets the text of link.
        /// </summary>
        public string Text
        {
            get { return text; }
        }

        /// <summary>
        /// Sets the web address of link.
        /// </summary>
        public string Url
        {
            get { return url; }
        }

        /// <summary>
        /// Sets the comment to the link.
        /// </summary>
        public string Comment
        {
            get { return comment; }
        }

        /// <summary>
        /// Public constructor.
        /// </summary>
        public LinkItem(string text, string url, string comment)
        {
            this.text    = text;
            this.url     = url;
            this.comment = comment;
        }
    }

    public class LinkPanel : Fancy_Panel
    {
        List<LinkItem>  links  = new List<LinkItem>();
        FlowLayoutPanel holder = new FlowLayoutPanel();

        /// <summary>
        /// Constructor.
        /// </summary>
        public LinkPanel(string caption) :
            base(caption) { }

        /// <summary>
        /// Adds a link to the LinkPanel.
        /// </summary>
        public void AddLink(LinkItem link)
        {
            links.Add(link);
        }

        /// <summary>
        /// Arranges the link labels.
        /// </summary>
        public void SetLinks()
        {
            int linksHeight = 0;
            int linksWidth  = 0;

            foreach (LinkItem link in links)
            {
                LinkLabel label = new LinkLabel();
                label.BackColor    = Color.Transparent;
                label.AutoSize     = false;
                label.Width        = holder.ClientSize.Width - 15;
                label.Height       = Font.Height + 3;
                label.AutoEllipsis = true;
                label.LinkBehavior = LinkBehavior.NeverUnderline;
                label.Text         = "     " + link.Text;
                label.Image        = Properties.Resources.globe;
                label.ImageAlign   = ContentAlignment.MiddleLeft;
                label.TextAlign    = ContentAlignment.MiddleLeft;
                label.Tag          = link.Url;
                label.Font         = new Font(label.Font.FontFamily, label.Font.Size, FontStyle.Regular);
                label.Margin       = new Padding(0, 5, 0, 0);
                label.Padding      = new Padding(0);
                label.LinkClicked += new LinkLabelLinkClickedEventHandler(Label_LinkClicked);

                if (link.Comment != string.Empty)
                {
                    ToolTip tooltip = new ToolTip();
                    tooltip.SetToolTip(label, link.Comment);
                }

                holder.Controls.Add(label);
                linksHeight = label.Bottom;
                linksWidth  = label.Right;
            }

            holder.Parent            = this;
            holder.BackColor         = Color.Transparent;
            holder.FlowDirection     = FlowDirection.TopDown;
            holder.Padding           = new Padding(10, 0, 5, 5);
            holder.AutoScroll        = true;
            holder.AutoScrollMinSize = new Size(linksWidth + 5, linksHeight + 5);
            holder.Scroll           += new ScrollEventHandler(LinkHolder_Scroll);

            return;
        }

        /// <summary>
        /// Repaints panel after scrolling.
        /// </summary>
        void LinkHolder_Scroll(object sender, ScrollEventArgs e)
        {
            base.Invalidate(InnerRectangle);
        }

        /// <summary>
        /// Opens the link in the default browser.
        /// </summary>
        void Label_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = (string)((LinkLabel)sender).Tag;

            try
            {
                System.Diagnostics.Process.Start(url);
            }
            catch { }

            return;
        }

        /// <summary>
        /// Arranges controls size.
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);

            holder.Location = InnerRectangle.Location;
            holder.Size     = InnerRectangle.Size;
        }
    }
}
