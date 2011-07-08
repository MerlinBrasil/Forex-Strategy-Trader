// AboutScreen Form
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Trader
{
    public class AboutScreen : Form
    {
        Fancy_Panel pnlBase;
        Label lblProgramName;
        Label lblProgramVersion;
        Label lblLibraryVersion;
        Label lblExpertVersion;
        Label lblCopyright;
        Label lblWebsite;
        Label lblSupportForum;
        Label lblContacts;
        Button btnOk;
        PictureBox pictureLogo;
        LinkLabel linkWebsite;
        LinkLabel linkForum;
        LinkLabel linkEmail;

        public AboutScreen()
        {
            pnlBase            = new Fancy_Panel();
            lblProgramName     = new Label();
            lblProgramVersion  = new Label();
            lblLibraryVersion  = new Label();
            lblExpertVersion   = new Label();
            lblCopyright       = new Label();
            lblWebsite         = new Label();
            lblSupportForum    = new Label();
            lblContacts        = new Label();
            pictureLogo        = new PictureBox();
            linkWebsite        = new LinkLabel();
            linkForum          = new LinkLabel();
            linkEmail          = new LinkLabel();
            btnOk              = new Button();

            // Panel Base
            pnlBase.Parent = this;

            pictureLogo.TabStop   = false;
            pictureLogo.BackColor = Color.Transparent;
            pictureLogo.Image     = Properties.Resources.Logo;

            lblProgramName.AutoSize  = true;
            lblProgramName.Font      = new Font("Microsoft Sans Serif", 16F, FontStyle.Bold);
            lblProgramName.ForeColor = LayoutColors.ColorControlText;
            lblProgramName.BackColor = Color.Transparent;
            lblProgramName.Text      = Data.ProgramName;

            lblProgramVersion.AutoSize  = true;
            lblProgramVersion.Font      = new Font("Microsoft Sans Serif", 12F);
            lblProgramVersion.ForeColor = LayoutColors.ColorControlText;
            lblProgramVersion.BackColor = Color.Transparent;
            lblProgramVersion.Text      = Language.T("Program version") + ": " + Data.ProgramVersion + (Data.IsProgramBeta ? " " + Language.T("Beta") : "");

            lblLibraryVersion.AutoSize  = true;
            lblLibraryVersion.Font      = new Font("Microsoft Sans Serif", 10F);
            lblLibraryVersion.ForeColor = LayoutColors.ColorControlText;
            lblLibraryVersion.BackColor = Color.Transparent;
            lblLibraryVersion.Text      = Language.T("Library version") + ": " + Data.LibraryVersion;

            // label4
            lblExpertVersion.AutoSize  = true;
            lblExpertVersion.Font      = new Font("Microsoft Sans Serif", 10F);
            lblExpertVersion.ForeColor = LayoutColors.ColorControlText;
            lblExpertVersion.BackColor = Color.Transparent;
            lblExpertVersion.Text      = Language.T("Expert version") + ": " + Data.ExpertVersion;

            // label5
            lblCopyright.AutoSize  = true;
            lblCopyright.Font      = new Font("Microsoft Sans Serif", 10F);
            lblCopyright.ForeColor = LayoutColors.ColorControlText;
            lblCopyright.BackColor = Color.Transparent;
            lblCopyright.Text      = "Copyright (c) 2011 Miroslav Popov" + Environment.NewLine + 
                Language.T("Distributor") + " - Forex Software Ltd." + Environment.NewLine + Environment.NewLine +  
                Language.T("This is a freeware program!");

            // label6
            lblWebsite.AutoSize  = true;
            lblWebsite.ForeColor = LayoutColors.ColorControlText;
            lblWebsite.BackColor = Color.Transparent;
            lblWebsite.Text      = Language.T("Website") + ":";

            // label7
            lblSupportForum.AutoSize  = true;
            lblSupportForum.ForeColor = LayoutColors.ColorControlText;
            lblSupportForum.BackColor = Color.Transparent;
            lblSupportForum.Text      = Language.T("Support forum") + ":";

            // label8
            lblContacts.AutoSize  = true;
            lblContacts.ForeColor = LayoutColors.ColorControlText;
            lblContacts.BackColor = Color.Transparent;
            lblContacts.Text      = Language.T("Contacts") + ":";

            // llWebsite
            linkWebsite.AutoSize  = true;
            linkWebsite.TabStop   = true;
            linkWebsite.BackColor = Color.Transparent;
            linkWebsite.Text      = "http://forexsb.com";
            linkWebsite.LinkClicked += new LinkLabelLinkClickedEventHandler(LinkWebsite_LinkClicked);

            // llForum
            linkForum.AutoSize     = true;
            linkForum.TabStop      = true;
            linkForum.BackColor    = Color.Transparent;
            linkForum.Text         = "http://forexsb.com/forum";
            linkForum.LinkClicked += new LinkLabelLinkClickedEventHandler(LinkForum_LinkClicked);

            // llEmail
            linkEmail.AutoSize     = true;
            linkEmail.TabStop      = true;
            linkEmail.BackColor    = Color.Transparent;
            linkEmail.Text         = "info@forexsb.com";
            linkEmail.LinkClicked += new LinkLabelLinkClickedEventHandler(LinkEmail_LinkClicked);

            // Button Base
            btnOk.Parent = this;
            btnOk.Text   = Language.T("Ok");
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += new EventHandler(BtnOk_Click);

            // AboutScreen
            pnlBase.Controls.Add(lblProgramName);
            pnlBase.Controls.Add(lblProgramVersion);
            pnlBase.Controls.Add(lblLibraryVersion);
            pnlBase.Controls.Add(lblExpertVersion);
            pnlBase.Controls.Add(lblCopyright);
            pnlBase.Controls.Add(lblWebsite);
            pnlBase.Controls.Add(lblSupportForum);
            pnlBase.Controls.Add(lblContacts);
            pnlBase.Controls.Add(linkWebsite);
            pnlBase.Controls.Add(linkForum);
            pnlBase.Controls.Add(linkEmail);
            pnlBase.Controls.Add(pictureLogo);

            StartPosition   = FormStartPosition.CenterScreen;
            Text            = Language.T("About") + " " + Data.ProgramName;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            BackColor       = LayoutColors.ColorFormBack;
            MaximizeBox     = false;
            MinimizeBox     = false;
            ShowInTaskbar   = false;
            ClientSize      = new Size(360, 320);
        }

        /// <summary>
        /// Form On Resize
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            int iButtonHeight = (int)(Data.VerticalDLU * 15.5);
            int iButtonWidth  = (int)(Data.HorizontalDLU * 60);
            int iBtnVertSpace = (int)(Data.VerticalDLU * 5.5);
            int iBtnHrzSpace  = (int)(Data.HorizontalDLU * 3);
            int iBorder       = iBtnHrzSpace;

            btnOk.Size       = new Size(iButtonWidth, iButtonHeight);
            btnOk.Location   = new Point(ClientSize.Width - btnOk.Width - iBorder, ClientSize.Height - btnOk.Height - iBtnVertSpace);
            pnlBase.Size     = new Size(ClientSize.Width - 2 * iBorder, btnOk.Top - iBorder - iBtnVertSpace);
            pnlBase.Location = new Point(iBorder, iBorder);

            pictureLogo.Location       = new Point(10, 3);
            pictureLogo.Size           = new Size(48, 48);
            lblProgramName.Location    = new Point(63, 10);
            lblProgramVersion.Location = new Point(65, 45);
            lblLibraryVersion.Location = new Point(66, 65);
            lblExpertVersion.Location  = new Point(66, 85);
            lblCopyright.Location      = new Point(67, 117);
            lblWebsite.Location        = new Point(67, 200);
            lblSupportForum.Location   = new Point(67, 220);
            lblContacts.Location       = new Point(68, 240);
            linkWebsite.Location       = new Point(lblSupportForum.Right + 5, lblWebsite.Top);
            linkForum.Location         = new Point(lblSupportForum.Right + 5, lblSupportForum.Top);
            linkEmail.Location         = new Point(lblSupportForum.Right + 5, lblContacts.Top);

            pnlBase.Invalidate();
        }

        /// <summary>
        /// Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }

        /// <summary>
        /// Connects to the web site
        /// </summary>
        void LinkWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://forexsb.com/");
            }
            catch { }
        }

        /// <summary>
        /// Connects to the forum
        /// </summary>
        void LinkForum_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://forexsb.com/forum/");
            }
            catch { }
        }

        /// <summary>
        /// Connects to the email
        /// </summary>
        void LinkEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("mailto:info@forexsb.com");
            }
            catch { }
        }

        /// <summary>
        /// Closes the form
        /// </summary>
        void BtnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
