// Strategy_Properties Form
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Strategy Properties
    /// </summary>
    public class Strategy_Properties : Form
    {
        Fancy_Panel pnlAveraging;
        Fancy_Panel pnlAmounts;
        Fancy_Panel pnlProtection;

        Label lblAveragingSameDir;
        Label lblAveragingOppDir;
        Label lblMaxOpenLots;
        Label lblEntryLots;
        Label lblAddingLots;
        Label lblReducingLots;
        Label lblPercent1;
        Label lblPercent2;
        Label lblPercent3;

        ComboBox cbxSameDirAction;
        ComboBox cbxOppDirAction;
        CheckBox chbPermaSL;
        CheckBox chbPermaTP;
        CheckBox chbBreakEven;
        RadioButton rbConstantUnits;
        RadioButton rbVariableUnits;
        NumericUpDown nudMaxOpenLots;
        NumericUpDown nudEntryLots;
        NumericUpDown nudAddingLots;
        NumericUpDown nudReducingLots;
        NumericUpDown nudPermaSL;
        NumericUpDown nudPermaTP;
        NumericUpDown nudBreakEven;
        ComboBox cbxPermaSLType;
        ComboBox cbxPermaTPType;

        Button  btnDefault;
        Button  btnAccept;
        Button  btnCancel;
        
        ToolTip toolTip = new ToolTip();

        Font   font;
        Color  colorText;

        SameDirSignalAction     sameDirAverg;
        OppositeDirSignalAction oppDirAverg;
        bool   bUseAccountPercentEntry;
        double dMaxOpenLots;
        double dEntryLots;
        double dAddingLots;
        double dReducingLots;
        bool   bUsePermanentSL = false;
        PermanentProtectionType permanentSLType = PermanentProtectionType.Relative;
        int    iPermSL         = 100;
        bool   bUsePermanentTP = false;
        PermanentProtectionType permanentTPType = PermanentProtectionType.Relative;
        int    iPermTP         = 100;
        bool   bUseBreakEven   = false;
        int    iBreakEven      = 100;

        /// <summary>
        /// Same Direction Signal Action
        /// </summary>
        public SameDirSignalAction SameDirAverg
        {
            get { return sameDirAverg; }
            set { sameDirAverg = value; }
        }

        /// <summary>
        /// Opposite Direction Signal Action
        /// </summary>
        public OppositeDirSignalAction OppDirAverg
        {
            get { return oppDirAverg; }
            set { oppDirAverg = value; }
        }

        /// <summary>
        /// Gets or sets the UsePermanentSL
        /// </summary>
        public bool UsePermanentSL
        { 
            get { return bUsePermanentSL; }
            set { bUsePermanentSL = value; }
        }

        /// <summary>
        /// Permanent Stop Loss Type
        /// </summary>
        public PermanentProtectionType PermanentSLType
        {
            get { return permanentSLType; }
            set { permanentSLType = value; }
        }

        /// <summary>
        /// Permanent S/L
        /// </summary>
        public int PermanentSL
        {
            get { return iPermSL; }
            set { iPermSL = value; }
        }

        /// <summary>
        /// Gets or sets the UseBreakEven
        /// </summary>
        public bool UseBreakEven
        {
            get { return bUseBreakEven; }
            set { bUseBreakEven = value; }
        }

        /// <summary>
        /// BreakEven S/L
        /// </summary>
        public int BreakEven
        {
            get { return iBreakEven; }
            set { iBreakEven = value; }
        }

        /// <summary>
        /// Gets or sets the UsePermanentTP
        /// </summary>
        public bool UsePermanentTP
        { 
            get { return bUsePermanentTP; }
            set { bUsePermanentTP = value; }
        }

        /// <summary>
        /// Permanent Take Profit Type
        /// </summary>
        public PermanentProtectionType PermanentTPType
        {
            get { return permanentTPType; }
            set { permanentTPType = value; }
        }

        /// <summary>
        /// Permanent Take Profit
        /// </summary>
        public int PermanentTP
        {
            get { return iPermTP; }
            set { iPermTP = value; }
        }

        /// <summary>
        /// Gets or sets the UseAccountPercentEntry
        /// </summary>
        public bool UseAccountPercentEntry
        { 
            get { return bUseAccountPercentEntry; }
            set { bUseAccountPercentEntry = value; }
        }

        /// <summary>
        /// Max open lots
        /// </summary>
        public double MaxOpenLots
        {
            get { return dMaxOpenLots; }
            set { dMaxOpenLots = value; }
        }

        /// <summary>
        /// Entry Lots
        /// </summary>
        public double EntryLots
        {
            get { return dEntryLots; }
            set { dEntryLots = value; }
        }

        /// <summary>
        /// Adding Lots
        /// </summary>
        public double AddingLots
        {
            get { return dAddingLots; }
            set { dAddingLots = value; }
        }

        /// <summary>
        /// Reducing Lots
        /// </summary>
        public double ReducingLots
        {
            get { return dReducingLots; }
            set { dReducingLots = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Strategy_Properties()
        {
            pnlAveraging  = new Fancy_Panel(Language.T("Handling of Additional Entry Signals"), LayoutColors.ColorSlotCaptionBackAveraging, LayoutColors.ColorSlotCaptionText);
            pnlAmounts    = new Fancy_Panel(Language.T("Trading Size"), LayoutColors.ColorSlotCaptionBackAveraging, LayoutColors.ColorSlotCaptionText);
            pnlProtection = new Fancy_Panel(Language.T("Permanent Protection"), LayoutColors.ColorSlotCaptionBackAveraging, LayoutColors.ColorSlotCaptionText);

            lblPercent1  = new Label();
            lblPercent2  = new Label();
            lblPercent3  = new Label();

            lblAveragingSameDir = new Label();
            lblAveragingOppDir  = new Label();

            cbxSameDirAction = new ComboBox();
            cbxOppDirAction  = new ComboBox();
            nudMaxOpenLots   = new NumericUpDown();
            rbConstantUnits  = new RadioButton();
            rbVariableUnits  = new RadioButton();
            nudEntryLots     = new NumericUpDown();
            nudAddingLots    = new NumericUpDown();
            nudReducingLots  = new NumericUpDown();
            lblMaxOpenLots   = new Label();
            lblEntryLots     = new Label();
            lblAddingLots    = new Label();
            lblReducingLots  = new Label();

            chbPermaSL       = new CheckBox();
            cbxPermaSLType   = new ComboBox();
            nudPermaSL       = new NumericUpDown();
            chbPermaTP       = new CheckBox();
            cbxPermaTPType   = new ComboBox();
            nudPermaTP       = new NumericUpDown();
            chbBreakEven     = new CheckBox();
            nudBreakEven     = new NumericUpDown();

            btnDefault = new Button();
            btnCancel  = new Button();
            btnAccept  = new Button();

            font      = this.Font;
            colorText = LayoutColors.ColorControlText;

            MaximizeBox     = false;
            MinimizeBox     = false;
            ShowInTaskbar   = false;
            Icon            = Data.Icon;
            BackColor       = LayoutColors.ColorFormBack;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton    = btnAccept;
            Text            = Language.T("Strategy Properties");

            // pnlAveraging
            pnlAveraging.Parent = this;

            // pnlAmounts
            pnlAmounts.Parent = this;

            // pnlProtection
            pnlProtection.Parent = this;

            // Label Same dir action
            lblAveragingSameDir.Parent    = pnlAveraging;
            lblAveragingSameDir.ForeColor = colorText;
            lblAveragingSameDir.BackColor = Color.Transparent;
            lblAveragingSameDir.AutoSize  = true;
            lblAveragingSameDir.Text = Language.T("Next same direction signal behaviour");

            // Label Opposite dir action
            lblAveragingOppDir.Parent    = pnlAveraging;
            lblAveragingOppDir.ForeColor = colorText;
            lblAveragingOppDir.BackColor = Color.Transparent;
            lblAveragingOppDir.AutoSize  = true;
            lblAveragingOppDir.Text      = Language.T("Next opposite direction signal behaviour");

            // ComboBox SameDirAction
            cbxSameDirAction.Parent        = pnlAveraging;
            cbxSameDirAction.Name          = "cbxSameDirAction";
            cbxSameDirAction.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxSameDirAction.Items.AddRange(new string[] { Language.T("Nothing"), Language.T("Winner"), Language.T("Add") });
            cbxSameDirAction.SelectedIndex = 0;
            toolTip.SetToolTip(cbxSameDirAction, 
                Language.T("Nothing - cancels the additional orders.") + Environment.NewLine +
                Language.T("Winner - adds to a winning position.")     + Environment.NewLine +
                Language.T("Add - adds to all positions.")); 

            // ComboBox OppDirAction
            cbxOppDirAction.Parent        = pnlAveraging;
            cbxOppDirAction.Name          = "cbxOppDirAction";
            cbxOppDirAction.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxOppDirAction.Items.AddRange(new string[] { Language.T("Nothing"), Language.T("Reduce"), Language.T("Close"), Language.T("Reverse") });
            cbxOppDirAction.SelectedIndex = 0;
            toolTip.SetToolTip(cbxOppDirAction,
                Language.T("Nothing - cancels the additional orders.") + Environment.NewLine +
                Language.T("Reduce - reduces or closes a position.")   + Environment.NewLine +
                Language.T("Close - closes the position.")             + Environment.NewLine +
                Language.T("Reverse - reverses the position.")); 

            // Label MaxOpen Lots
            lblMaxOpenLots.Parent    = pnlAmounts;
            lblMaxOpenLots.ForeColor = colorText;
            lblMaxOpenLots.BackColor = Color.Transparent;
            lblMaxOpenLots.AutoSize  = true;
            lblMaxOpenLots.Text      = Language.T("Maximum number of open lots");

            // NumericUpDown MaxOpen Lots
            nudMaxOpenLots.Parent    = pnlAmounts;
            nudMaxOpenLots.Name      = "nudMaxOpenLots";
            nudMaxOpenLots.BeginInit();
            nudMaxOpenLots.Minimum   = 0.01M;
            nudMaxOpenLots.Maximum   = 100;
            nudMaxOpenLots.Increment = 0.01M;
            nudMaxOpenLots.Value     = (decimal)dMaxOpenLots;
            nudMaxOpenLots.DecimalPlaces = 2;
            nudMaxOpenLots.TextAlign = HorizontalAlignment.Center;
            nudMaxOpenLots.EndInit();

            // Radio Button Constant Units
            rbConstantUnits.Parent    = pnlAmounts;
            rbConstantUnits.ForeColor = colorText;
            rbConstantUnits.BackColor = Color.Transparent;
            rbConstantUnits.Checked   = !UseAccountPercentEntry;
            rbConstantUnits.AutoSize  = true;
            rbConstantUnits.Name      = "rbConstantUnits";
            rbConstantUnits.Text      = Language.T("Trade a constant number of lots");

            // Radio Button Variable Units
            rbVariableUnits.Parent    = pnlAmounts;
            rbVariableUnits.ForeColor = colorText;
            rbVariableUnits.BackColor = Color.Transparent;
            rbVariableUnits.Checked   = UseAccountPercentEntry;
            rbVariableUnits.AutoSize  = false;
            rbVariableUnits.Name      = "rbVariableUnits";
            rbVariableUnits.Text      = Language.T("Trade a variable number of lots depending on your current account equity. The percentage values show the part of the account equity used to cover the required margin.");

            // Label Entry Lots
            lblEntryLots.Parent    = pnlAmounts;
            lblEntryLots.ForeColor = colorText;
            lblEntryLots.BackColor = Color.Transparent;
            lblEntryLots.AutoSize  = true;
            lblEntryLots.Text      = Language.T("Number of entry lots for a new position");

            // NumericUpDown Entry Lots
            nudEntryLots.Parent    = pnlAmounts;
            nudEntryLots.Name      = "nudEntryLots";
            nudEntryLots.BeginInit();
            nudEntryLots.Minimum   = 0.01M;
            nudEntryLots.Maximum   = 100;
            nudEntryLots.Increment = 0.01M;
            nudEntryLots.Value     = (decimal)dEntryLots;
            nudEntryLots.DecimalPlaces = 2;
            nudEntryLots.TextAlign = HorizontalAlignment.Center;
            nudEntryLots.EndInit();

            // Label Entry Lots %
            lblPercent1.Parent    = pnlAmounts;
            lblPercent1.ForeColor = colorText;
            lblPercent1.BackColor = Color.Transparent;
            lblPercent1.AutoSize  = true;
            lblPercent1.Text      = "%";

            // Label Adding Lots
            lblAddingLots.Parent    = pnlAmounts;
            lblAddingLots.ForeColor = colorText;
            lblAddingLots.BackColor = Color.Transparent;
            lblAddingLots.AutoSize  = true;
            lblAddingLots.Text      = Language.T("In case of addition - number of lots to add");

            // NumericUpDown Adding Lots
            nudAddingLots.Parent    = pnlAmounts;
            nudAddingLots.Name      = "nudAddingLots";
            nudAddingLots.BeginInit();
            nudAddingLots.Minimum   = 0.01M;
            nudAddingLots.Maximum   = 100;
            nudAddingLots.Increment = 0.01M;
            nudAddingLots.Value     = (decimal)dAddingLots;
            nudAddingLots.DecimalPlaces = 2;
            nudAddingLots.TextAlign = HorizontalAlignment.Center;
            nudAddingLots.EndInit();

            // Label Adding Lots %
            lblPercent2.Parent    = pnlAmounts;
            lblPercent2.ForeColor = colorText;
            lblPercent2.BackColor = Color.Transparent;
            lblPercent2.AutoSize  = true;
            lblPercent2.Text      = "%";

            // Label Reducing Lots
            lblReducingLots.Parent    = pnlAmounts;
            lblReducingLots.ForeColor = colorText;
            lblReducingLots.BackColor = Color.Transparent;
            lblReducingLots.AutoSize  = true;
            lblReducingLots.Text      = Language.T("In case of reduction - number of lots to close");

            // NumericUpDown Reducing Lots
            nudReducingLots.Parent    = pnlAmounts;
            nudReducingLots.Name      = "nudReducingLots";
            nudReducingLots.BeginInit();
            nudReducingLots.Minimum   = 0.01M;
            nudReducingLots.Maximum   = 100;
            nudReducingLots.Increment = 0.01M;
            nudReducingLots.Value     = (decimal)dReducingLots;
            nudReducingLots.DecimalPlaces = 2;
            nudReducingLots.TextAlign = HorizontalAlignment.Center;
            nudReducingLots.EndInit();

            // Label Reducing Lots %
            lblPercent3.Parent    = pnlAmounts;
            lblPercent3.ForeColor = colorText;
            lblPercent3.BackColor = Color.Transparent;
            lblPercent3.AutoSize  = true;
            lblPercent3.Text      = "%";

            // CheckBox Permanent Stop Loss
            chbPermaSL.Parent    = pnlProtection;
            chbPermaSL.ForeColor = colorText;
            chbPermaSL.BackColor = Color.Transparent;
            chbPermaSL.AutoCheck = true;
            chbPermaSL.AutoSize  = true;
            chbPermaSL.Name      = "chbPermaSL";
            chbPermaSL.Text      = Language.T("Permanent Stop Loss");

            // ComboBox cbxPermaSLType
            cbxPermaSLType.Parent  = pnlProtection;
            cbxPermaSLType.Name    = "cbxPermaSLType";
            cbxPermaSLType.Visible = false;
            cbxPermaSLType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxPermaSLType.Items.AddRange(new string[] { Language.T("Relative"), Language.T("Absolute") });
            cbxPermaSLType.SelectedIndex = 0;

            // NumericUpDown Permanent S/L
            nudPermaSL.Parent    = pnlProtection;
            nudPermaSL.Name      = "nudPermaSL";
            nudPermaSL.BeginInit();
            nudPermaSL.Minimum   = 5;
            nudPermaSL.Maximum   = 5000;
            nudPermaSL.Increment = 1;
            nudPermaSL.Value     = iPermSL;
            nudPermaSL.TextAlign = HorizontalAlignment.Center;
            nudPermaSL.EndInit();

            // CheckBox Permanent Take Profit
            chbPermaTP.Parent    = pnlProtection;
            chbPermaTP.ForeColor = colorText;
            chbPermaTP.BackColor = Color.Transparent;
            chbPermaTP.AutoCheck = true;
            chbPermaTP.AutoSize  = true;
            chbPermaTP.Name      = "chbPermaTP";
            chbPermaTP.Text      = Language.T("Permanent Take Profit");

            // ComboBox cbxPermaTPType
            cbxPermaTPType.Parent  = pnlProtection;
            cbxPermaTPType.Name    = "cbxPermaTPType";
            cbxPermaTPType.Visible = false;
            cbxPermaTPType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxPermaTPType.Items.AddRange(new string[] { Language.T("Relative"), Language.T("Absolute") });
            cbxPermaTPType.SelectedIndex = 0;

            // NumericUpDown Permanent Take Profit
            nudPermaTP.Parent    = pnlProtection;
            nudPermaTP.Name      = "nudPermaTP";
            nudPermaTP.BeginInit();
            nudPermaTP.Minimum   = 5;
            nudPermaTP.Maximum   = 5000;
            nudPermaTP.Increment = 1;
            nudPermaTP.Value     = iPermTP;
            nudPermaTP.TextAlign = HorizontalAlignment.Center;
            nudPermaTP.EndInit();

            // CheckBox Break Even
            chbBreakEven.Parent    = pnlProtection;
            chbBreakEven.ForeColor = colorText;
            chbBreakEven.BackColor = Color.Transparent;
            chbBreakEven.AutoCheck = true;
            chbBreakEven.AutoSize  = true;
            chbBreakEven.Name      = "chbBreakEven";
            chbBreakEven.Text      = Language.T("Break Even") + " [" + Language.T("pips") + "]";

            // NumericUpDown Break Even
            nudBreakEven.Parent    = pnlProtection;
            nudBreakEven.Name      = "nudBreakEven";
            nudBreakEven.BeginInit();
            nudBreakEven.Minimum   = 5;
            nudBreakEven.Maximum   = 5000;
            nudBreakEven.Increment = 1;
            nudBreakEven.Value     = iBreakEven;
            nudBreakEven.TextAlign = HorizontalAlignment.Center;
            nudBreakEven.EndInit();

            //Button Default
            btnDefault.Parent = this;
            btnDefault.Name   = "Default";
            btnDefault.Text   = Language.T("Default");
            btnDefault.Click += new EventHandler(BtnDefault_Click);
            btnDefault.UseVisualStyleBackColor = true;

            //Button Cancel
            btnCancel.Parent       = this;
            btnCancel.Text         = Language.T("Cancel");
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.UseVisualStyleBackColor = true;

            //Button Accept
            btnAccept.Parent       = this;
            btnAccept.Name         = "Accept";
            btnAccept.Text         = Language.T("Accept");
            btnAccept.DialogResult = DialogResult.OK;
            btnAccept.UseVisualStyleBackColor = true;

            return;
        }

        /// <summary>
        /// Perform initializing
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cbxSameDirAction.SelectedIndexChanged += new EventHandler(ParamChanged);
            cbxOppDirAction.SelectedIndexChanged  += new EventHandler(ParamChanged);
            rbConstantUnits.CheckedChanged        += new EventHandler(ParamChanged);
            rbVariableUnits.CheckedChanged        += new EventHandler(ParamChanged);
            nudMaxOpenLots.ValueChanged           += new EventHandler(ParamChanged);
            nudEntryLots.ValueChanged             += new EventHandler(ParamChanged);
            nudAddingLots.ValueChanged            += new EventHandler(ParamChanged);
            nudReducingLots.ValueChanged          += new EventHandler(ParamChanged);
            chbPermaSL.CheckedChanged             += new EventHandler(ParamChanged);
            cbxPermaSLType.SelectedIndexChanged   += new EventHandler(ParamChanged);
            nudPermaSL.ValueChanged               += new EventHandler(ParamChanged);
            chbPermaTP.CheckedChanged             += new EventHandler(ParamChanged);
            cbxPermaTPType.SelectedIndexChanged   += new EventHandler(ParamChanged);
            nudPermaTP.ValueChanged               += new EventHandler(ParamChanged);
            chbBreakEven.CheckedChanged           += new EventHandler(ParamChanged);
            nudBreakEven.ValueChanged             += new EventHandler(ParamChanged);

            int iButtonWidth = (int)(Data.HorizontalDLU * 60);
            int iBtnHrzSpace = (int)(Data.HorizontalDLU * 3);

            ClientSize = new Size(3 * iButtonWidth + 4 * iBtnHrzSpace, 474);

            btnAccept.Focus();

            return;
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            int buttonHeight = (int)(Data.VerticalDLU   * 15.5);
            int buttonWidth  = (int)(Data.HorizontalDLU * 60);
            int btnVertSpace = (int)(Data.VerticalDLU   * 5.5);
            int btnHrzSpace  = (int)(Data.HorizontalDLU * 3);
            int space        = btnHrzSpace;
            int textHeight   = Font.Height;
            int border       = 2;

            // pnlAveraging
            pnlAveraging.Size     = new Size(ClientSize.Width - 2 * space, 84);
            pnlAveraging.Location = new Point(space, space);

            // pnlAmounts
            pnlAmounts.Size     = new Size(ClientSize.Width - 2 * space, 222);
            pnlAmounts.Location = new Point(space, pnlAveraging.Bottom + space);

            // pnlProtection
            pnlProtection.Size     = new Size(ClientSize.Width - 2 * space, 112);
            pnlProtection.Location = new Point(space,pnlAmounts.Bottom + space);

            // Averaging
            int cbxWith = 80;
            int cbxLeft = pnlAveraging.ClientSize.Width - cbxWith - space - border;

            cbxSameDirAction.Width       = cbxWith;
            lblAveragingSameDir.Location = new Point(btnHrzSpace, space + 25);
            cbxSameDirAction.Location    = new Point(cbxLeft,     space + 21);

            if (lblAveragingSameDir.Right + space > cbxSameDirAction.Left)
            {
                Width += lblAveragingSameDir.Right + space - cbxSameDirAction.Left;
                return;
            }

            cbxOppDirAction.Width        = cbxWith;
            lblAveragingOppDir.Location  = new Point(btnHrzSpace, buttonHeight + 2 * space + 23);
            cbxOppDirAction.Location     = new Point(cbxLeft,     buttonHeight + 2 * space + 19);

            if (lblAveragingOppDir.Right + space > cbxOppDirAction.Left)
            {
                Width += lblAveragingOppDir.Right + space - cbxOppDirAction.Left;
                return;
            }

            // Amounts
            int nudWidth = 60;
            int nudLeft  = pnlAmounts.ClientSize.Width - nudWidth - btnHrzSpace - border;

            lblMaxOpenLots.Location = new Point(btnHrzSpace, 0 * buttonHeight + space + 25);
            nudMaxOpenLots.Size     = new Size(nudWidth, buttonHeight);
            nudMaxOpenLots.Location = new Point(nudLeft, 0 * buttonHeight +  space + 22);

            if (lblMaxOpenLots.Right + space > nudMaxOpenLots.Left)
            {
                Width += lblMaxOpenLots.Right + space - nudMaxOpenLots.Left;
                return;
            }
            
            rbConstantUnits.Location = new Point(btnHrzSpace + 3, 55);
            rbVariableUnits.Location = new Point(btnHrzSpace + 3, 79);

            // Measuring rbVariableUnits size
            rbVariableUnits.Size = new Size(pnlAmounts.ClientSize.Width - 2 * btnHrzSpace, 2 * buttonHeight);
            int deltaWidth = 0;
            Graphics g = CreateGraphics();
            while (g.MeasureString(rbVariableUnits.Text, rbVariableUnits.Font, rbVariableUnits.Width - 10).Height > 3 * rbVariableUnits.Font.Height)
            {
                deltaWidth++;
                rbVariableUnits.Width++;
            }
            g.Dispose();
            if (deltaWidth > 0)
            {
                Width += deltaWidth;
                return;
            }

            lblEntryLots.Location = new Point(btnHrzSpace, 139);
            nudEntryLots.Size     = new Size(nudWidth, buttonHeight);
            nudEntryLots.Location = new Point(nudLeft, 137);
            lblPercent1.Location  = new Point(nudEntryLots.Left - 15, lblEntryLots.Top);

            if (lblEntryLots.Right + space > lblPercent1.Left)
            {
                Width += lblEntryLots.Right + space - lblPercent1.Left;
                return;
            }

            lblAddingLots.Location = new Point(btnHrzSpace, 167);
            nudAddingLots.Size     = new Size(nudWidth, buttonHeight);
            nudAddingLots.Location = new Point(nudLeft, 165);
            lblPercent2.Location   = new Point(nudAddingLots.Left - 15, lblAddingLots.Top);

            if (lblAddingLots.Right + space > lblPercent2.Left)
            {
                Width += lblAddingLots.Right + space - lblPercent2.Left;
                return;
            }

            lblReducingLots.Location = new Point(btnHrzSpace, 195);
            nudReducingLots.Size     = new Size(nudWidth, buttonHeight);
            nudReducingLots.Location = new Point(nudLeft, 193);
            lblPercent3.Location     = new Point(nudReducingLots.Left - 15, lblReducingLots.Top);

            if (lblReducingLots.Right + space > lblPercent3.Left)
            {
                Width += lblReducingLots.Right + space - lblPercent3.Left;
                return;
            }

            int rightComboBxWith = 95;
            int comboBxLeft = nudLeft - space - rightComboBxWith;


            // Permanent Stop Loss
            nudPermaSL.Size         = new Size(nudWidth, buttonHeight);
            nudPermaSL.Location     = new Point(nudLeft, 0 * buttonHeight + 1 * space + 23);
            cbxPermaSLType.Width    = rightComboBxWith;
            cbxPermaSLType.Location = new Point(comboBxLeft, 0 * buttonHeight + 1 * space + 23);
            chbPermaSL.Location     = new Point(btnHrzSpace + 3, 0 * buttonHeight + 1 * space + 24);

            // Permanent Take Profit
            nudPermaTP.Size         = new Size(nudWidth, buttonHeight);
            nudPermaTP.Location     = new Point(nudLeft, 1 * buttonHeight + 2 * space + 21);
            cbxPermaTPType.Width    = rightComboBxWith;
            cbxPermaTPType.Location = new Point(comboBxLeft, 1 * buttonHeight + 2 * space + 21);
            chbPermaTP.Location     = new Point(btnHrzSpace + 3, 1 * buttonHeight + 2 * space + 22);

            // Break Even
            nudBreakEven.Size     = new Size(nudWidth, buttonHeight);
            nudBreakEven.Location = new Point(nudLeft,         2 * buttonHeight + 3 * space + 19);
            chbBreakEven.Location = new Point(btnHrzSpace + 3, 2 * buttonHeight + 3 * space + 20);

            buttonWidth = (pnlAveraging.Width - 2 * btnHrzSpace) / 3;

            // Button Cancel
            btnCancel.Size     = new Size(buttonWidth, buttonHeight);
            btnCancel.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Default
            btnDefault.Size     = new Size(buttonWidth, buttonHeight);
            btnDefault.Location = new Point(btnCancel.Left - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Accept
            btnAccept.Size     = new Size(buttonWidth, buttonHeight);
            btnAccept.Location = new Point(btnDefault.Left - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            return;
        }

        /// <summary>
        /// Sets the controls' text
        /// </summary>
        public void SetParams()
        {
            // ComboBox sameDirAverg
            switch (sameDirAverg)
            {
                case SameDirSignalAction.Nothing:
                    cbxSameDirAction.SelectedIndex = 0;
                    break;
                case SameDirSignalAction.Winner:
                    cbxSameDirAction.SelectedIndex = 1;
                    break;
                case SameDirSignalAction.Add:
                    cbxSameDirAction.SelectedIndex = 2;
                    break;
                default:
                    break;
            }

            // ComboBox oppDirAverg
            if (Data.Strategy.Slot[Data.Strategy.CloseSlot].IndicatorName == "Close and Reverse")
                cbxOppDirAction.Enabled = false;

            // ComboBox oppDirAverg
            switch (oppDirAverg)
            {
                case OppositeDirSignalAction.Nothing:
                    cbxOppDirAction.SelectedIndex = 0;
                    break;
                case OppositeDirSignalAction.Reduce:
                    cbxOppDirAction.SelectedIndex = 1;
                    break;
                case OppositeDirSignalAction.Close:
                    cbxOppDirAction.SelectedIndex = 2;
                    break;
                case OppositeDirSignalAction.Reverse:
                    cbxOppDirAction.SelectedIndex = 3;
                    break;
                default:
                    break;
            }

            chbPermaSL.Checked   = bUsePermanentSL;
            chbPermaTP.Checked   = bUsePermanentTP;
            chbBreakEven.Checked = bUseBreakEven;

            cbxPermaSLType.Enabled = bUsePermanentSL;
            cbxPermaSLType.SelectedIndex = (int)permanentSLType;

            cbxPermaTPType.Enabled = bUsePermanentTP;
            cbxPermaTPType.SelectedIndex = (int)permanentTPType;

            // NumericUpDown nudPermaSL
            nudPermaSL.Enabled = chbPermaSL.Checked;
            nudPermaSL.Value   = iPermSL;

            // NumericUpDown nudPermaTP
            nudPermaTP.Enabled = chbPermaTP.Checked;
            nudPermaTP.Value   = iPermTP;

            // NumericUpDown nudBreakEven
            nudBreakEven.Enabled = chbBreakEven.Checked;
            nudBreakEven.Value   = iBreakEven;

            // Use account percent entry
            rbConstantUnits.Checked = !bUseAccountPercentEntry;
            rbVariableUnits.Checked = bUseAccountPercentEntry;

            nudMaxOpenLots.Value  = (decimal)dMaxOpenLots;

            nudEntryLots.Value    = (decimal)dEntryLots;
            nudAddingLots.Value   = (decimal)dAddingLots;
            nudReducingLots.Value = (decimal)dReducingLots;

            lblPercent1.Visible = bUseAccountPercentEntry;
            lblPercent2.Visible = bUseAccountPercentEntry;
            lblPercent3.Visible = bUseAccountPercentEntry;

            return;
        }

        /// <summary>
        /// Sets the params values
        /// </summary>
        void ParamChanged(object sender, EventArgs e)
        {
            string sName = ((Control)sender).Name;

            if (sName == "cbxSameDirAction")
            {   // ComboBox Account Currency
                sameDirAverg = (SameDirSignalAction)cbxSameDirAction.SelectedIndex;
            }

            if (sName == "chbPermaSL")
            {   // NumericUpDown nudPermaSL
                nudPermaSL.Enabled = chbPermaSL.Checked;
                cbxPermaSLType.Enabled = chbPermaSL.Checked;
                bUsePermanentSL = chbPermaSL.Checked;
            }

            if (sName == "cbxPermaSLType")
            {   // ComboBox cbxPermaTPType
                permanentSLType = (PermanentProtectionType)cbxPermaSLType.SelectedIndex;
            }

            if (sName == "nudPermaSL")
            {   // NumericUpDown nudPermaSL
                iPermSL = (int)nudPermaSL.Value;
            }

            if (sName == "chbPermaTP")
            {   // NumericUpDown nudPermaTP
                nudPermaTP.Enabled = chbPermaTP.Checked;
                cbxPermaTPType.Enabled = chbPermaTP.Checked;
                bUsePermanentTP = chbPermaTP.Checked;
            }

            if (sName == "cbxPermaTPType")
            {   // ComboBox cbxPermaTPType
                permanentTPType = (PermanentProtectionType)cbxPermaTPType.SelectedIndex;
            }

            if (sName == "nudPermaTP")
            {   // NumericUpDown nudPermaTP
                iPermTP = (int)nudPermaTP.Value;
            }

            if (sName == "chbBreakEven")
            {   // NumericUpDown nudBreakEven
                nudBreakEven.Enabled = chbBreakEven.Checked;
                bUseBreakEven = chbBreakEven.Checked;
            }

            if (sName == "nudBreakEven")
            {   // NumericUpDown nudBreakEven
                iBreakEven = (int)nudBreakEven.Value;
            }

            if (sName == "cbxOppDirAction")
            {   // ComboBox Leverage
                oppDirAverg = (OppositeDirSignalAction)cbxOppDirAction.SelectedIndex;
            }

            if (sName == "rbConstantUnits")
            {   // Use account percent entry
                bUseAccountPercentEntry = false;
                lblPercent1.Visible = bUseAccountPercentEntry;
                lblPercent2.Visible = bUseAccountPercentEntry;
                lblPercent3.Visible = bUseAccountPercentEntry;
            }

            if (sName == "rbVariableUnits")
            {   // Use account percent entry
                bUseAccountPercentEntry = true;
                lblPercent1.Visible = bUseAccountPercentEntry;
                lblPercent2.Visible = bUseAccountPercentEntry;
                lblPercent3.Visible = bUseAccountPercentEntry;
            }

            if (sName == "nudMaxOpenLots")
            {   // NumericUpDown nudEntryLots
                dMaxOpenLots = (double)nudMaxOpenLots.Value;
            }

            if (sName == "nudEntryLots")
            {   // NumericUpDown nudEntryLots
                dEntryLots = (double)nudEntryLots.Value;
            }

            if (!bUseAccountPercentEntry && dEntryLots > dMaxOpenLots)
            {
                dEntryLots = dMaxOpenLots;
                nudEntryLots.Value = (decimal)dEntryLots;
            }

            if (sName == "nudAddingLots")
            {   // NumericUpDown nudAddingLots
                dAddingLots = (double)nudAddingLots.Value;
            }

            if (sName == "nudReducingLots")
            {   // NumericUpDown nudReducingLots
                dReducingLots = (double)nudReducingLots.Value;
            }

            return;
        }

        /// <summary>
        /// Button Default Click
        /// </summary>
        void BtnDefault_Click(object sender, EventArgs e)
        {
            SameDirAverg   = SameDirSignalAction.Nothing;

            if (Data.Strategy.Slot[Data.Strategy.CloseSlot].IndicatorName == "Close and Reverse")
            {
                cbxOppDirAction.Enabled = false;
                OppDirAverg = OppositeDirSignalAction.Reverse;
            }
            else
            {
                OppDirAverg = OppositeDirSignalAction.Nothing;
            }

            int defaultSL_TP = (Data.InstrProperties.Digits == 5 || Data.InstrProperties.Digits == 3) ? 1000 : 100;

            UsePermanentSL  = false;
            PermanentSLType = PermanentProtectionType.Relative;
            PermanentSL     = defaultSL_TP;
            UsePermanentTP  = false;
            PermanentTPType = PermanentProtectionType.Relative;
            PermanentTP     = defaultSL_TP;
            UseBreakEven    = false;
            BreakEven       = defaultSL_TP;
            UseAccountPercentEntry = false;
            MaxOpenLots     = 20;
            EntryLots       = 1;
            AddingLots      = 1;
            ReducingLots    = 1;

            SetParams();

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
