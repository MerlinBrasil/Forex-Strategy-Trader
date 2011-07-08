// Edit_Translation Form
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
    /// <summary>
    /// Edit Translation
    /// </summary>
    class Edit_Translation : Form
    {
        Fancy_Panel pnlCommon;
        Fancy_Panel pnlPhrases;
        Label[]     alblInputNames;
        TextBox[]   atbxInputValues;
        TextBox[]   atbxMain;
        TextBox[]   atbxAlt;
        VScrollBar  scrollBar;
        TextBox     tbxSearch;
        Button      btnSearch;
        Button      btnUntranslated;
        Button      btnAccept;
        Button      btnCancel;

        int  TEXTBOXES = 8;
        bool isTranslChanged = false;
        bool isProgramChange = true;
        static Dictionary<String, String> dictLanguage;
        int      phrases;
        string[] asMain;
        string[] asAlt;

        /// <summary>
        /// Constructor
        /// </summary>
        public Edit_Translation()
        {
            // The form
            MaximizeBox     = false;
            MinimizeBox     = false;
            Icon            = Data.Icon;
            BackColor       = LayoutColors.ColorFormBack;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Text            = Language.T("Edit Translation");
            FormClosing    += new FormClosingEventHandler(Actions_FormClosing);

            // Controls
            pnlCommon       = new Fancy_Panel(Language.T("Common Parameters"));
            pnlPhrases      = new Fancy_Panel(Language.T("English Phrase - Translated Phrase"));
            alblInputNames  = new Label[5];
            atbxInputValues = new TextBox[5];
            atbxMain        = new TextBox[TEXTBOXES];
            atbxAlt         = new TextBox[TEXTBOXES];
            scrollBar      = new VScrollBar();
            tbxSearch       = new TextBox();
            btnSearch       = new Button();
            btnUntranslated = new Button();
            btnAccept       = new Button();
            btnCancel       = new Button();

            // Common
            pnlCommon.Parent = this;

            // Phrases
            pnlPhrases.Parent = this;

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
                Configs.Language,
                Language.LanguageFileName,
                Language.Author,
                Language.AuthorsWebsite,
                Language.AuthorsEmail,
            };

            // Common parameters
            for (int i = 0; i < asInputNames.Length;  i++)
            {
                alblInputNames[i] = new Label();
                alblInputNames[i].Parent    = pnlCommon;
                alblInputNames[i].ForeColor = LayoutColors.ColorControlText;
                alblInputNames[i].BackColor = Color.Transparent;
                alblInputNames[i].AutoSize  = true;
                alblInputNames[i].Text      = asInputNames[i];

                atbxInputValues[i] = new TextBox();
                atbxInputValues[i].Parent = pnlCommon;
                atbxInputValues[i].Text   = asInputValues[i];
            }

            // Phrases
            for (int i = 0; i < TEXTBOXES; i++)
            {
                atbxMain[i] = new TextBox();
                atbxMain[i].Parent    = pnlPhrases;
                atbxMain[i].Multiline = true;
                atbxMain[i].ReadOnly  = true;
                atbxMain[i].ForeColor = Color.DarkGray;

                atbxAlt[i] = new TextBox();
                atbxAlt[i].Parent    = pnlPhrases;
                atbxAlt[i].Multiline = true;
                atbxAlt[i].Tag       = i;
                atbxAlt[i].TextChanged += new EventHandler(Edit_Translation_TextChanged);
            }

            // Vertical ScrollBar
            scrollBar.Parent  = pnlPhrases;
            scrollBar.Visible = true;
            scrollBar.Enabled = true;
            scrollBar.ValueChanged += new EventHandler(ScrollBar_ValueChanged);
            scrollBar.TabStop = true;

            // TextBox Search
            tbxSearch.Parent = this;
            tbxSearch.TextChanged += new EventHandler(TbxSearch_TextChanged);

            // Button Search
            btnSearch.Parent = this;
            btnSearch.Name   = "Search";
            btnSearch.Text   = Language.T("Search");
            btnSearch.Click += new EventHandler(Btn_Click);
            btnSearch.UseVisualStyleBackColor = true;

            // Button Untranslated
            btnUntranslated.Parent = this;
            btnUntranslated.Name   = "Untranslated";
            btnUntranslated.Text   = Language.T("Not Translated");
            btnUntranslated.Click += new EventHandler(Btn_Click);
            btnUntranslated.UseVisualStyleBackColor = true;

            // Button Cancel
            btnCancel.Parent       = this;
            btnCancel.Name         = "Cancel";
            btnCancel.Text         = Language.T("Cancel");
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Click       += new EventHandler(Btn_Click);
            btnCancel.UseVisualStyleBackColor = true;

            // Button Accept
            btnAccept.Parent       = this;
            btnAccept.Name         = "Accept";
            btnAccept.Text         = Language.T("Accept");
            btnAccept.DialogResult = DialogResult.OK;
            btnAccept.Click       += new EventHandler(Btn_Click);
            btnAccept.Enabled      = false;
            btnAccept.UseVisualStyleBackColor = true;
        }

        /// <summary>
        /// Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Size = new Size(760, 587);
            InitParams();
            SetTextBoxes();
            scrollBar.Focus();

            return;
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            int buttonHeight = (int)(Data.VerticalDLU * 15.5);
            int buttonWidth  = (int)(Data.HorizontalDLU * 60);
            int btnVertSpace = (int)(Data.VerticalDLU * 5.5);
            int btnHrzSpace  = (int)(Data.HorizontalDLU * 3);
            int border       = btnHrzSpace;
            int textHeight   = Font.Height;

            // pnlCommon
            pnlCommon.Size     = new Size(ClientSize.Width - 2 * border, 85);
            pnlCommon.Location = new Point(border, border);

            Graphics g = CreateGraphics();
            int maxLabelLenght = 0;
            foreach (Label label in alblInputNames)
            {
                int lenght = (int)g.MeasureString(label.Text, Font).Width;
                if (lenght > maxLabelLenght)
                    maxLabelLenght = lenght;
            }
            g.Dispose();

            int labelWidth   = maxLabelLenght + border;
            int textBoxWidth = (pnlCommon.ClientSize.Width - 4 * border - 3 * labelWidth) / 3;

            int shift     = 26;
            int vertSpace = 2;
            int number    = 0;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i == 0 && j == 2) continue;

                    int xLabel   = border + j * (labelWidth + textBoxWidth) + j * border;
                    int yLabel   = i * buttonHeight + (i + 1) * vertSpace + shift;
                    int xTextBox = border + labelWidth + j * (labelWidth + textBoxWidth) + j * border;
                    int yTextBox = i * buttonHeight + (i + 1) * vertSpace + shift - 2;

                    alblInputNames[number].Location  = new Point(xLabel,   yLabel);

                    atbxInputValues[number].Width    = textBoxWidth;
                    atbxInputValues[number].Location = new Point(xTextBox, yTextBox);

                    number++;
                }
            }

            atbxInputValues[0].Enabled = false;
            atbxInputValues[1].Enabled = false;

            // pnlPhrases
            pnlPhrases.Size = new Size(ClientSize.Width - 2 * border, ClientSize.Height - buttonHeight - 2 * btnVertSpace - border - pnlCommon.Bottom);
            pnlPhrases.Location = new Point(border, pnlCommon.Bottom + border);

            shift = 22;
            textBoxWidth = (pnlPhrases.ClientSize.Width - 4 * border - scrollBar.Width) / 2;
            int iTextBoxHeight = (pnlPhrases.ClientSize.Height - shift - (TEXTBOXES + 1) * border) / TEXTBOXES;

            for (int i = 0; i < TEXTBOXES; i++)
            {
                int xMain = border;
                int yMain = i * iTextBoxHeight + (i + 1) * border + shift;
                int xAlt  = 2 * border + textBoxWidth;
                int yAlt  = i * iTextBoxHeight + (i + 1) * border + shift;

                atbxMain[i].Size     = new Size(textBoxWidth, iTextBoxHeight);
                atbxAlt[i].Size      = new Size(textBoxWidth, iTextBoxHeight);
                atbxMain[i].Location = new Point(xMain, yMain);
                atbxAlt[i].Location  = new Point(xAlt,  yAlt);
            }

            scrollBar.Height   = atbxAlt[TEXTBOXES - 1].Bottom - atbxAlt[0].Top;
            scrollBar.Location = new Point(pnlPhrases.ClientSize.Width - border - scrollBar.Width, atbxAlt[0].Top);

            // tbxSearch
            tbxSearch.Size     = new Size(3 * buttonWidth / 2, buttonHeight);
            tbxSearch.Location = new Point(btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace + 2);

            // Button Search
            btnSearch.Size     = new Size(buttonWidth, buttonHeight);
            btnSearch.Location = new Point(tbxSearch.Right + btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Untranslated
            btnUntranslated.Size     = new Size(buttonWidth, buttonHeight);
            btnUntranslated.Location = new Point(btnSearch.Right + btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Cancel
            btnCancel.Size     = new Size(buttonWidth, buttonHeight);
            btnCancel.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Accept
            btnAccept.Size     = new Size(buttonWidth, buttonHeight);
            btnAccept.Location = new Point(btnCancel.Left - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            return;
        }

        /// <summary>
        /// Inits the params.
        /// </summary>
        void InitParams()
        {
            dictLanguage = Language.Translation;
            phrases     = dictLanguage.Values.Count;

            asMain = new string[phrases];
            asAlt  = new string[phrases];
            dictLanguage.Keys.CopyTo(asMain, 0);
            dictLanguage.Values.CopyTo(asAlt, 0);

            scrollBar.SmallChange = 1;
            scrollBar.LargeChange = (int)(TEXTBOXES / 2);
            scrollBar.Maximum     = phrases - TEXTBOXES + scrollBar.LargeChange - 1;
            scrollBar.Value       = 0;

            return;
        }

        /// <summary>
        /// The translation is edited;
        /// </summary>
        void Edit_Translation_TextChanged(object sender, EventArgs e)
        {
            if (isProgramChange) return;

            TextBox tb = (TextBox)sender;
            int index  = scrollBar.Value + (int)tb.Tag;

            asAlt[index]      = tb.Text;
            isTranslChanged   = true;
            btnAccept.Enabled = Configs.Language != "English";

            return;
        }

        /// <summary>
        /// Scroll Bar value changed.
        /// </summary>
        void ScrollBar_ValueChanged(object sender, EventArgs e)
        {
            SetTextBoxes();

            return;
        }

        /// <summary>
        /// Sets the phrases in the text boxes.
        /// </summary>
        void SetTextBoxes()
        {
            isProgramChange = true;

            for (int i = 0; i < TEXTBOXES; i++)
            {
                atbxMain[i].Text = asMain[scrollBar.Value + i];
                atbxAlt[i].Text  = asAlt[scrollBar.Value + i];
            }

            isProgramChange = false;

            return;
        }

        /// <summary>
        /// Button click
        /// </summary>
        void Btn_Click(object sender, EventArgs e)
        {
            Button button     = (Button)sender;
            string buttonName = button.Name;

            if (buttonName == "Search")
            {
                SearchPhrase(tbxSearch.Text);
            }

            if (buttonName == "Untranslated")
            {
                SearchUntranslatedPhrase();
            }

            if (buttonName == "Accept")
            {
                SaveTranslation();
                Language.InitLanguages();

                this.Close();
            }

            if (buttonName == "Cancel")
            {
                this.Close();
            }

            return;
        }

        /// <summary>
        /// Searches a phrase.
        /// </summary>
        void TbxSearch_TextChanged(object sender, EventArgs e)
        {
            SearchPhrase(tbxSearch.Text);

            return;
        }

        /// <summary>
        /// Searches a phrase.
        /// </summary>
        void SearchPhrase(string phrase)
        {
            phrase = phrase.ToLower();
            phrase = phrase.Trim();

            if (phrase == "")
                return;

            for (int i = scrollBar.Value + 1; i < phrases; i++)
            {
                if (asMain[i].ToLower().Contains(phrase) || asAlt[i].ToLower().Contains(phrase))
                {
                    scrollBar.Value = Math.Min(i, phrases - TEXTBOXES);
                    return;
                }
            }

            for (int i = 0; i < scrollBar.Value + 1; i++)
            {
                if (asMain[i].ToLower().Contains(phrase) || asAlt[i].ToLower().Contains(phrase))
                {
                    scrollBar.Value = Math.Min(i, phrases - TEXTBOXES);
                    return;
                }
            }
        }

        /// <summary>
        /// Searches for a untranslated phrase.
        /// </summary>
        void SearchUntranslatedPhrase()
        {
            for (int i = scrollBar.Value + 1; i < phrases; i++)
            {
                if (asMain[i] == asAlt[i])
                {
                    scrollBar.Value = Math.Min(i, phrases - TEXTBOXES);

                    return;
                }
            }

            for (int i = 0; i < scrollBar.Value + 1; i++)
            {
                if (asMain[i] == asAlt[i])
                {
                    scrollBar.Value = Math.Min(i, phrases - TEXTBOXES);

                    return;
                }
            }

            return;
        }

        /// <summary>
        /// Check whether the strategy have been changed.
        /// </summary>
        void Actions_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isTranslChanged && Configs.Language != "English")
            {
                DialogResult dr = MessageBox.Show(Language.T("Do you want to accept the changes?"),
                    Data.ProgramName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (dr == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
                else if (dr == DialogResult.Yes)
                {
                    SaveTranslation();
                    Close();
                }
                else if (dr == DialogResult.No)
                {
                    isTranslChanged = false;
                    Close();
                }
            }

            return;
        }

        /// <summary>
        /// Saves the translation
        /// </summary>
        void SaveTranslation()
        {
            string author   = atbxInputValues[2].Text;
            string website  = atbxInputValues[3].Text;
            string contacts = atbxInputValues[4].Text;

            for (int i = 0; i < phrases; i++)
                dictLanguage[asMain[i]] = asAlt[i];

            Language.SaveLangFile(dictLanguage, author, website, contacts);

            isTranslChanged = false;

            return;
        }
 
        /// <summary>
        /// Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }
    }
}
