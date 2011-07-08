// New_Translation Form
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// New Translation
    /// </summary>
    class New_Translation : Form
    {
        Fancy_Panel pnlInput;
        Label[]     alblInputNames;
        TextBox[]   atbxInputValues;
        Button      btnAccept;
        Button      btnCancel;

        /// <summary>
        /// Constructor
        /// </summary>
        public New_Translation()
        {
            // The form
            MaximizeBox     = false;
            MinimizeBox     = false;
            Icon            = Data.Icon;
            BackColor       = LayoutColors.ColorFormBack;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Text            = Language.T("New Translation");

            // Controls
            pnlInput        = new Fancy_Panel(Language.T("Common Parameters"));
            alblInputNames  = new Label[5];
            atbxInputValues = new TextBox[5];
            btnAccept       = new Button();
            btnCancel       = new Button();

            // Input
            pnlInput.Parent = this;

            // Input Names
            string[] asInputNames = new string[] {
                Language.T("Language"),
                Language.T("File name"),
                Language.T("Author"),
                Language.T("Website"),
                Language.T("Contacts"),
            };

            // Input Values
            string[] asInputValues = new string[] {
                "Language",
                "Language",
                "Your Name",
                @"http:\\forexsb.com",
                @"info@forexsb.com",
            };

            // Input parameters
            for (int i = 0; i < asInputNames.Length;  i++)
            {
                alblInputNames[i] = new Label();
                alblInputNames[i].Parent    = pnlInput;
                alblInputNames[i].ForeColor = LayoutColors.ColorControlText;
                alblInputNames[i].BackColor = Color.Transparent;
                alblInputNames[i].AutoSize  = true;
                alblInputNames[i].Text      = asInputNames[i];

                atbxInputValues[i] = new TextBox();
                atbxInputValues[i].Parent = pnlInput;
                atbxInputValues[i].Text   = asInputValues[i];
            }

            //Button Cancel
            btnCancel.Parent       = this;
            btnCancel.Text         = Language.T("Cancel");
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Click       += new EventHandler(Btn_Click);
            btnCancel.UseVisualStyleBackColor = true;

            //Button Accept
            btnAccept.Parent       = this;
            btnAccept.Name         = "Accept";
            btnAccept.Text         = Language.T("Accept");
            btnAccept.DialogResult = DialogResult.OK;
            btnAccept.Click       += new EventHandler(Btn_Click);
            btnAccept.UseVisualStyleBackColor = true;

            return;
        }

        /// <summary>
        /// Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ClientSize = new Size(335, 220);

            return;
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            int iButtonHeight = (int)(Data.VerticalDLU * 15.5);
            int iButtonWidth  = (int)(Data.HorizontalDLU * 60);
            int iBtnVertSpace = (int)(Data.VerticalDLU * 5.5);
            int iBtnHrzSpace  = (int)(Data.HorizontalDLU * 3);
            int iBorder       = iBtnHrzSpace;
            int iTextHeight   = Font.Height;
            int iWidth        = 195; // Right side controls

            // pnlInput
            pnlInput.Size     = new Size(ClientSize.Width - 2 * iBorder, 170);
            pnlInput.Location = new Point(iBorder, iBorder);

            int iLeft = pnlInput.ClientSize.Width - iWidth - iBtnHrzSpace - 1;

            int iShift     = 26;
            int iVertSpace = 2;
            for (int i = 0; i < alblInputNames.Length; i++)
            {
                alblInputNames[i].Location = new Point(iBorder, i * iButtonHeight + (i + 1) * iVertSpace + iShift);
            }

            iShift     = 24;
            iVertSpace = 2;
            for (int i = 0; i < atbxInputValues.Length; i++)
            {
                atbxInputValues[i].Width    = iWidth;
                atbxInputValues[i].Location = new Point(iLeft, i * iButtonHeight + (i + 1) * iVertSpace + iShift);
            }

            // Button Cancel
            btnCancel.Size     = new Size(iButtonWidth, iButtonHeight);
            btnCancel.Location = new Point(ClientSize.Width - iButtonWidth - iBtnHrzSpace, ClientSize.Height - iButtonHeight - iBtnVertSpace);

            // Button Accept
            btnAccept.Size     = new Size(iButtonWidth, iButtonHeight);
            btnAccept.Location = new Point(btnCancel.Left - iButtonWidth - iBtnHrzSpace, ClientSize.Height - iButtonHeight - iBtnVertSpace);

            return;
        }

        /// <summary>
        /// Button click
        /// </summary>
        void Btn_Click(object sender, EventArgs e)
        {
            Button btn   = (Button)sender;
            string sName = btn.Name;

            if (sName == "Accept")
            {
                bool bIsCorrect = true;

                string sLanguage = atbxInputValues[0].Text;
                string sFileName = atbxInputValues[1].Text + ".xml";
                string sAuthor   = atbxInputValues[2].Text;
                string sWebsite  = atbxInputValues[3].Text;
                string sContacts = atbxInputValues[4].Text;

                // Language
                if(sLanguage.Length < 2)
                {
                    MessageBox.Show("The language name must be at least two characters in length!", "Language", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    bIsCorrect = false;
                }

                foreach (string lang in Language.LanguageList)
                    if (sLanguage == lang)
                    {
                        MessageBox.Show("A translation in this language exists already!\r\nChange the language name.", "Language", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        bIsCorrect = false;
                    }

                // Language file name
                if (sFileName.Length < 2)
                {
                    MessageBox.Show("The language file name must be at least two characters in length!", "Language File Name", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    bIsCorrect = false;
                }

                if (Directory.Exists(Data.LanguageDir))
                {
                    string[] asFileNames = Directory.GetFiles(Data.LanguageDir);
                    foreach (string path in asFileNames)
                    {
                        if (sFileName == Path.GetFileName(path))
                        {
                            MessageBox.Show("This file name exists already!\r\nChange the file name.", "Language File Name", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            bIsCorrect = false;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Could not find the language files directory!", "Language Files Directory", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    bIsCorrect = false;
                }

                if (bIsCorrect)
                {
                    if (Language.GenerateNewLangFile(sFileName, sLanguage, sAuthor, sWebsite, sContacts))
                    {
                        Configs.Language = sLanguage;
                        string sMassage = "The new language file was successfully created.\r\nRestart the program and edit the translation.";
                        MessageBox.Show(sMassage, "New Translation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
                else
                    return;
            }

            this.Close();
        }
 
        /// <summary>
        /// Form On Paint.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }
    }
}
